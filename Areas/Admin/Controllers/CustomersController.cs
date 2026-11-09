using HoangHaPCVN.Models;
using HoangHaPCVN.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace HoangHaPCVN.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private HoangHaPCVNEntities db = new HoangHaPCVNEntities();

        // GET: Admin/Customers
        //Xem, tìm kiếm, sắp xếp, phân trang danh sách khách hàng
        public ActionResult Index(string searchTerm, string sortOrder, int? page)
        {
            var model = new CustomerSearchVM();
            var customers = db.Customers.AsQueryable();

            //Tìm theo tên, số điện thoại, email hoặc tên đăng nhập
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                customers = customers.Where(c =>
                    c.CustomerName.Contains(searchTerm) ||
                    c.CustomerPhone.Contains(searchTerm) ||
                    c.CustomerEmail.Contains(searchTerm) ||
                    c.Username.Contains(searchTerm));
            }

            //Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    customers = customers.OrderByDescending(c => c.CustomerName);
                    break;
                case "orders_desc":   //Khách mua nhiều đơn nhất lên đầu
                    customers = customers.OrderByDescending(c => c.Orders.Count());
                    break;
                default:              //Mặc định: tên A -> Z
                    customers = customers.OrderBy(c => c.CustomerName);
                    break;
            }
            model.SortOrder = sortOrder;

            //Phân trang: 1 trang 10 khách hàng
            int pageNumber = page ?? 1;
            int pageSize = 10;
            model.Customers = customers.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Admin/Customers/Details/5
        //Thông tin 1 khách hàng và danh sách đơn hàng của khách đó
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Include: lấy luôn các đơn hàng của khách
            Customer customer = db.Customers
                .Include(c => c.Orders)
                .SingleOrDefault(c => c.CustomerID == id);

            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
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