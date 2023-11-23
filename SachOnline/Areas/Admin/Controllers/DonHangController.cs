using SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SachOnline.Controllers;

namespace SachOnline.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
        // GET: Admin/DonHang
        private string connection;
        private dbSachOnlineDataContext db;

        public DonHangController()
        {
            // Khởi tạo chuỗi kết nối
            db = Connect.GetConnect();
        }

        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.DONDATHANGs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(iPageNum, iPageSize));
        }

        public ActionResult Details(int id)
        {
            var donhang = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            if (donhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(donhang);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var dh = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            if (dh == null)
            {
                return HttpNotFound();
            }

            return View(dh);
        }

        [HttpPost]
        public ActionResult Edit(DONDATHANG dh)
        {
            if (ModelState.IsValid)
            {
                var existingdonhang = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == dh.MaDonHang);

                if (existingdonhang != null)
                {
                    existingdonhang.DaThanhToan = dh.DaThanhToan;
                    existingdonhang.TinhTrangGiaoHang = dh.TinhTrangGiaoHang;
                    existingdonhang.NgayDat = dh.NgayDat;
                    existingdonhang.NgayGiao = dh.NgayGiao;

                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(dh);
        }








        [HttpGet]
        public ActionResult Delete(int id)
        {
            var ddh = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            if (ddh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ddh);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var ddh = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            if (ddh == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            var ctdh = db.CHITIETDATHANGs.Where(s => s.MaDonHang == id);
            if (ctdh.Count() > 0)
            {
                // Xóa chi tiết đặt hàng trước
                foreach (var chiTiet in ctdh)
                {
                    db.CHITIETDATHANGs.DeleteOnSubmit(chiTiet);
                }
                db.SubmitChanges();
            }

            // Sau đó mới xóa đơn đặt hàng
            db.DONDATHANGs.DeleteOnSubmit(ddh);
            db.SubmitChanges();

            return RedirectToAction("Index");
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