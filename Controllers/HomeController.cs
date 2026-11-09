using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using HoangHaPCVN.Models;

namespace HoangHaPCVN.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult HoangHaIndex()
        {
            return View();
        }

        // ================= CÁC TRANG DANH MỤC =================
        public ActionResult PCgiare() { return View(); }
        public ActionResult CPU() { return View(); }
        public ActionResult VGA() { return View(); }
        public ActionResult LinhKienMayTinh() { return View(); }
        public ActionResult MangHinhMayTinh() { return View(); }
        public ActionResult TangNhietCooling() { return View(); }
        public ActionResult GamingGear() { return View(); }
        public ActionResult ThietBiMang() { return View(); }

        // ================= GIỎ HÀNG =================

        // Lấy giỏ hàng từ Session (chưa có thì tạo mới)
        private List<CartItem> LayGioHang()
        {
            var gio = Session["GioHang"] as List<CartItem>;
            if (gio == null)
            {
                gio = new List<CartItem>();
                Session["GioHang"] = gio;
            }
            return gio;
        }

        // Trang giỏ hàng
        public ActionResult GioHang()
        {
            return View(LayGioHang());
        }

        // Bấm nút + ở trang sản phẩm
        [HttpPost]
        public ActionResult ThemVaoGio(string ten, long gia, string hinh)
        {
            var gio = LayGioHang();
            var sp = gio.FirstOrDefault(x => x.Hinh == hinh);

            if (sp != null)
            {
                sp.SoLuong++;
            }
            else
            {
                gio.Add(new CartItem { Ten = ten, Gia = gia, Hinh = hinh, SoLuong = 1 });
            }
            return RedirectToAction("GioHang");
        }

        // Tăng số lượng
        [HttpPost]
        public ActionResult TangSoLuong(int viTri)
        {
            var gio = LayGioHang();
            if (viTri >= 0 && viTri < gio.Count)
            {
                gio[viTri].SoLuong++;
            }
            return RedirectToAction("GioHang");
        }

        // Giảm số lượng (thấp nhất là 1, muốn xóa thì bấm thùng rác)
        [HttpPost]
        public ActionResult GiamSoLuong(int viTri)
        {
            var gio = LayGioHang();
            if (viTri >= 0 && viTri < gio.Count && gio[viTri].SoLuong > 1)
            {
                gio[viTri].SoLuong--;
            }
            return RedirectToAction("GioHang");
        }

        // Xóa 1 sản phẩm
        [HttpPost]
        public ActionResult XoaSanPham(int viTri)
        {
            var gio = LayGioHang();
            if (viTri >= 0 && viTri < gio.Count)
            {
                gio.RemoveAt(viTri);
            }
            return RedirectToAction("GioHang");
        }

        // Làm trống giỏ hàng
        [HttpPost]
        public ActionResult XoaHet()
        {
            LayGioHang().Clear();
            return RedirectToAction("GioHang");
        }

        // Đặt hàng
        [HttpPost]
        public ActionResult DatHang(string hoTen, string soDienThoai, string email, string diaChi, string ghiChu)
        {
            var gio = LayGioHang();
            if (gio.Count == 0)
            {
                return RedirectToAction("GioHang");
            }

            // Kiểm tra lại ở server (phòng khi trình duyệt tắt JavaScript)
            bool hopLe = !string.IsNullOrWhiteSpace(hoTen)
                      && !string.IsNullOrWhiteSpace(diaChi)
                      && Regex.IsMatch(soDienThoai ?? "", @"^0\d{9}$");

            if (!hopLe)
            {
                ViewBag.Loi = "Vui lòng nhập đầy đủ Họ tên, Số điện thoại (10 số, bắt đầu bằng 0) và Địa chỉ nhận hàng.";
                ViewBag.HoTen = hoTen;
                ViewBag.SoDienThoai = soDienThoai;
                ViewBag.Email = email;
                ViewBag.DiaChi = diaChi;
                ViewBag.GhiChu = ghiChu;
                return View("GioHang", gio);
            }

            // TODO: sau này lưu đơn hàng vào database ở đây

            TempData["ThongBao"] = "Đặt hàng thành công! Cảm ơn " + hoTen + ", nhân viên sẽ liên hệ với bạn sớm nhất.";
            gio.Clear();
            return RedirectToAction("GioHang");
        }

    }
}