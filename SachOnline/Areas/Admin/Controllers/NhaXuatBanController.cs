using SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using System.Web.UI.WebControls;
using SachOnline.Controllers;

namespace SachOnline.Areas.Admin.Controllers
{
    public class NhaXuatBanController : Controller
    {
        private string connection;
        private dbSachOnlineDataContext db;

        public NhaXuatBanController()
        {
            // Khởi tạo chuỗi kết nối
            db = Connect.GetConnect();
        }
        // GET: Admin/NhaXuatBan
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.NHAXUATBANs.ToList().OrderBy(n => n.MaNXB).ToPagedList(iPageNum, iPageSize));
        }


        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.MaNXB), "MaNXB", "TenNXB");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(NHAXUATBAN nhaxuatban, FormCollection f)
        {
            if (ModelState.IsValid)
            {
                nhaxuatban.TenNXB = f["sTenNXB"];
                nhaxuatban.DiaChi = f["sDiaChi"];
                nhaxuatban.DienThoai = f["sDienThoai"];

                db.NHAXUATBANs.InsertOnSubmit(nhaxuatban);
                db.SubmitChanges();
                //Vé trang Quån sach
                return RedirectToAction("Index");
            }

            return View(nhaxuatban);
        }

        public ActionResult Details(int id)
        {
            var nhaxuatban = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);
            if (nhaxuatban == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nhaxuatban);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var nhaxuatban = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);
            if (nhaxuatban == null)
            {
                return HttpNotFound();
            }
            return View(nhaxuatban);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, FormCollection f)
        {
            var nhaxuatban = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);
            if (nhaxuatban == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            var sach = db.SACHes.Where(s => s.MaNXB == id);
            if (sach.Count() > 0)
            {
                ViewBag.ThongBao = "Không thể xóa nhà xuất bản này vì có sách liên quan đến nó. Hãy xóa các sách trước khi xóa nhà xuất bản.";
                return View(nhaxuatban);
            }

            db.NHAXUATBANs.DeleteOnSubmit(nhaxuatban);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var nhaxuatban = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);
            if (nhaxuatban == null)
            {
                return HttpNotFound();
            }

            return View(nhaxuatban);
        }

        [HttpPost]
        public ActionResult Edit(NHAXUATBAN nhaxuatban)
        {
            if (ModelState.IsValid)
            {
                var existingnhaxuatban = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == nhaxuatban.MaNXB);

                if (existingnhaxuatban != null)
                {
                    existingnhaxuatban.TenNXB = nhaxuatban.TenNXB;

                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(nhaxuatban);
        }

        // Hàm kiểm tra đăng nhập
        private bool IsAdminLoggedIn()
        {
            return Session["Admin"] != null;
        }

        // Hàm chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
        private ActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Admin");
        }

        // Hàm xác định xem người dùng đã đăng nhập hay chưa trước khi thực hiện các hành động
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Kiểm tra đăng nhập ở đây
            if (!IsAdminLoggedIn())
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                filterContext.Result = RedirectToLogin();
            }
        }


    }
}