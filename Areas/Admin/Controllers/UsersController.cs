using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoangHaPCVN.Models;
using HoangHaPCVN.Models.ViewModel;
using PagedList;
using HoangHaPCVN.Areas.Admin.Filters;

namespace HoangHaPCVN.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class UsersController : Controller
    {
        private HoangHaPCVNEntities db = new HoangHaPCVNEntities();

        private const string RoleAdmin = "Admin";

        // GET: Admin/Users
        // Danh sách tài khoản Admin: Phần tìm kiếm và phân trang
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new UserSearchVM();

            // Chỉ lấy tài khoản có vai trò Admin
            var users = db.Users.Where(u => u.UserRole == RoleAdmin);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                users = users.Where(u => u.Username.Contains(searchTerm));
            }

            int pageNumber = page ?? 1;
            int pageSize = 10;
            model.Users = users.OrderBy(u => u.Username).ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            return View(new AdminUserVM());
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminUserVM model)
        {
            //Bỏ dấu cách thừa ở đầu và cuối tên đăng nhập
            if (model.Username != null)
            {
                model.Username = model.Username.Trim();
            }

            //Kiểm tra tên đăng nhập đã tồn tại chưa (kể cả tài khoản Customer)
            if (db.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại");
            }

            if (ModelState.IsValid)
            {
                //Lấy dữ liệu từ ViewModel sang class User để lưu
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    UserRole = RoleAdmin
                };
                db.Users.Add(user);
                db.SaveChanges();

                TempData["Message"] = "Đã thêm tài khoản Admin" + " " + user.Username + ".";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Admin/Users/Edit/admin
        // Đổi mật khẩu tài khoản Admin
        public ActionResult Edit(string id)
        {
            User user = TimAdmin(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            //Chỉ giữ tên đăng nhập sang form, không đưa mật khẩu cũ sang
            return View(new AdminUserVM { Username = user.Username });
        }

        // POST: Admin/Users/Edit/admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminUserVM model)
        {
            User user = TimAdmin(model.Username);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                user.Password = model.Password;   //Chỉ đổi mật khẩu
                db.SaveChanges();

                TempData["Message"] = "Đã đổi mật khẩu cho tài khoản" + " " + user.Username + ".";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Admin/Users/Delete/admin
        public ActionResult Delete(string id)
        {
            User user = TimAdmin(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Delete/admin
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = TimAdmin(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            //Không cho xóa chính tài khoản đang đăng nhập
            if (id == Session["Username"] as string)
            {
                ModelState.AddModelError("", "Không thể xóa tài khoản bạn đang đăng nhập.");
                return View(user);
            }

            //Không cho xóa nếu đây là tài khoản Admin cuối cùng (phải có một tài khoản Admin)
            int soAdmin = db.Users.Count(u => u.UserRole == RoleAdmin);
            if (soAdmin <= 1)
            {
                ModelState.AddModelError("", "Không thể xóa tài khoản Admin này. Vui lòng tạo thêm tài khoản Admin khác !");
                return View(user);
            }

            db.Users.Remove(user);
            db.SaveChanges();

            TempData["Message"] = "Đã xóa tài khoản" + " " + id + ".";
            return RedirectToAction("Index");
        }

        //Tìm tài khoản theo tên đăng nhập, chỉ trả về nếu là Admin
        private User TimAdmin(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            User user = db.Users.Find(username);
            if (user == null || user.UserRole != RoleAdmin)
            {
                return null;
            }
            return user;
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