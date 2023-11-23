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
    public class KhachHangController : Controller
    {
        private string connection;
        private dbSachOnlineDataContext db;

        public KhachHangController()
        {
            // Khởi tạo chuỗi kết nối
            db = Connect.GetConnect();
        }
        // GET: Admin/khachhang
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.KHACHHANGs.ToList().OrderBy(n => n.MaKH).ToPagedList(iPageNum, iPageSize));
        }


        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaKH = new SelectList(db.KHACHHANGs.ToList().OrderBy(n => n.MaKH), "MaKH", "HoTen");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(KHACHHANG khachhang, FormCollection f)
        {
            if (ModelState.IsValid)
            {
                khachhang.HoTen = f["sHoTen"];
                khachhang.TaiKhoan = f["sTaiKhoan"];
                khachhang.MatKhau = f["sMatKhau"];
                khachhang.Email = f["sEmail"];
                khachhang.DiaChi = f["sDiaChi"];
                khachhang.DienThoai = f["sDienThoai"];
                khachhang.NgaySinh = Convert.ToDateTime(f["sNgaySinh"]);

                db.KHACHHANGs.InsertOnSubmit(khachhang);
                db.SubmitChanges();
                //Vé trang Quån sach
                return RedirectToAction("Index");
            }

            return View(khachhang);
        }

        public ActionResult Details(int id)
        {
            var khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(khachhang);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(khachhang);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id, FormCollection f)
        {
            var khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            var ddh = db.DONDATHANGs.Where(s => s.MaKH == id);
            if (ddh.Count() > 0)
            {
                ViewBag.ThongBao = "Không thể xóa khách hàng này vì có Đơn đặt hàng liên quan đến khách hàng này. Hãy yêu cầu khách hàng thanh toán các đơn đặt hàng trước khi xóa khách hàng.";
                return View(khachhang);
            }

            db.KHACHHANGs.DeleteOnSubmit(khachhang);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (khachhang == null)
            {
                return HttpNotFound();
            }

            return View(khachhang);
        }

        [HttpPost]
        public ActionResult Edit(KHACHHANG khachhang)
        {
            if (ModelState.IsValid)
            {
                var existingkhachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == khachhang.MaKH);

                if (existingkhachhang != null)
                {
                    existingkhachhang.HoTen = khachhang.HoTen;
                    existingkhachhang.TaiKhoan = khachhang.TaiKhoan;
                    existingkhachhang.Email = khachhang.Email;
                    existingkhachhang.DiaChi = khachhang.DiaChi;
                    existingkhachhang.DienThoai = khachhang.DienThoai;
                    existingkhachhang.NgaySinh = khachhang.NgaySinh;

                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(khachhang);
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