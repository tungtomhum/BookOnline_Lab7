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
        public ActionResult DeleteConfirm(int id, FormCollection f)
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
                ViewBag.ThongBao = "Không thể xóa đơn hàng này vì có Chi tiết đặt hàng liên quan đến đơn hàng này. Hãy yêu cầu khách hàng thanh toán các đơn đặt hàng trước khi xóa đơn hàng hoặc xóa chi tiết đặt hàng trước.";
                return View(ddh);
            }

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