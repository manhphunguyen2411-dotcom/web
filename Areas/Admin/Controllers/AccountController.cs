using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HoangHaPCVN.Models;
using HoangHaPCVN.Models.ViewModel;

namespace HoangHaPCVN.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private HoangHaPCVNEntities db = new HoangHaPCVNEntities();

        // GET: Admin/Account
        public ActionResult Login(string returnUrl)
        {
            //Đăng nhập Admin thành công rồi thì vào trang chủ Admin
            if (Session["UserRole"] as string == "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Admin/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //Tìm tài khoản Admin theo tên đăng nhập
                var user = db.Users.SingleOrDefault(u =>
                    u.Username == model.Username && u.UserRole == "Admin");

                //Phân biệt chữ hoa chữ thường
                if (user != null && user.Password == model.Password)
                {
                    //Lưu trạng thái đăng nhập vào session
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    //Lưu thông tin xác thực người dùng vào cookie
                    FormsAuthentication.SetAuthCookie(user.Username, false);

                    //Quay lại trang đang muốn vào trước khi bị chặn (nếu có)
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản không có quyền Admin."); //Bắt lỗi 
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // GET: Admin/Account/Logout
        public ActionResult Logout()
        {
            Session.Clear(); //Xóa thông tin trong session
            FormsAuthentication.SignOut(); //Xóa cookie xác thực
            return RedirectToAction("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}