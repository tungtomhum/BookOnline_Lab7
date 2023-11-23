using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SachOnline.Models;
using System.IO;
using System.Web.UI;
using PagedList;
using System.Collections;
using System.Security.Cryptography;
using System.Web.UI.WebControls;
using SachOnline.Controllers;

namespace SachOnline.Areas.Admin.Controllers
{
    public class SachController : Controller
    {
        private string connection;
        private dbSachOnlineDataContext db;

        public SachController()
        {
            // Khởi tạo chuỗi kết nối
            db = Connect.GetConnect();
        }       
        // GET: Admin/Sach
        public ActionResult Index( int ? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.SACHes.ToList().OrderBy(n => n.MaSach).ToPagedList(iPageNum, iPageSize));
        }


        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(SACH sach, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            if(fFileUpload == null)
            {
                ViewBag.ThongBao = "Hãy chọn ảnh bìa cho sách!";

                ViewBag.TenSach = f["sTenSach"];
                ViewBag.MoTa = f["sMoTa"];
                ViewBag.SoLuong = int.Parse(f["iSoLuong"]);
                ViewBag.GiaBan = decimal.Parse(f["mGiaBan"]);
                ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", int.Parse(f["MaCD"]));
                ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", int.Parse(f["MaNXB"]));

                return View();
            }
            else
            {
                if(ModelState.IsValid)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);

                    if(!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }

                    sach.TenSach = f["sTenSach"];
                    sach.MoTa = f["sMoTa"];
                    sach.AnhBia = sFileName;
                    sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                    sach.SoLuongBan = int.Parse(f["iSoLuong"]);
                    sach.GiaBan = decimal.Parse(f["mGiaBan"]);
                    sach.MaCD = int.Parse(f["MaCD"]);
                    sach.MaNXB = int.Parse(f["MaNXB"]);
                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();
                    //Vé trang Quån sich
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        public ActionResult Details(int id)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if(sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if(sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id, FormCollection f)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if(sach == null)
            {
                Response.StatusCode = 404; 
                return null;
            }
            var ctdh = db.CHITIETDATHANGs.Where(ct => ct.MaSach == id);
            if(ctdh.Count() > 0)
            {
                ViewBag.ThongBao = "Sách này đang có trong bảng Chi tiết đặt hàng <br>" + "Nếu muốn xóa thì phải xóa hết mã sách này trong bảng Chi tiết đặt hàng";
                return View(sach);
            }
            var vietsach =db.VIETSACHes.Where(vs => vs.MaSach == id).ToList();
            if(vietsach!=null)
            {
                db.VIETSACHes.DeleteAllOnSubmit(vietsach);
                db.SubmitChanges();
            }
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD );
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);

            return View(sach);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == int.Parse(f["iMaSach"]));
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);

            if(ModelState.IsValid)
            {
                if(fFileUpload != null)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);

                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    sach.AnhBia = sFileName;
                }

                sach.TenSach = f["sTenSach"];
                sach.MoTa = f["sMoTa"];
                // Kiểm tra xem ngày cập nhật có giá trị hợp lệ hay không
                if (f["dNgayCapNhat"] != null)
                {
                    // Chuyển ngày cập nhật từ chuỗi sang kiểu DateTime
                    sach.NgayCapNhat = DateTime.Now;
                }
                else
                {
                    // Xử lý trường hợp ngày cập nhật không hợp lệ (nếu cần)
                    sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                }
                
                sach.SoLuongBan = int.Parse(f["iSoLuong"]);
                sach.GiaBan = decimal.Parse(f["mGiaBan"]);
                sach.MaCD = int.Parse(f["MaCD"]);
                sach.MaNXB = int.Parse(f["MaNXB"]);

                db.SubmitChanges();
                //Vé trang QL SACH
                return RedirectToAction("Index");
            }
            return View(sach);
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