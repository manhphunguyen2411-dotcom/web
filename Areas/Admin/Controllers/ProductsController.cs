using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HoangHaPCVN.Models;
using System.IO;
using HoangHaPCVN.Models.ViewModel;
using PagedList;
using HoangHaPCVN.Areas.Admin.Filters;

namespace HoangHaPCVN.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductsController : Controller
    {
        private HoangHaPCVNEntities db = new HoangHaPCVNEntities();

        //Thư mục lưu ảnh và ảnh mặc định
        private const string ThuMucAnh = "~/Content/images/";
        private const string AnhMacDinh = "~/Content/images/default_img.png";

        // GET: Admin/Products
        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new ProductSearchVM();
            var products = db.Products.Include(p => p.Category).AsQueryable();

            //Tìm kiếm sản phẩm dự trên từ khóa (tên sp, mô tả hoặc tên danh mục)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p =>
                p.ProductName.Contains(searchTerm) ||
                p.ProductDescription.Contains(searchTerm) ||
                p.Category.CategoryName.Contains(searchTerm));
            }

            //Tìm sản phẩm có giá thấp nhất
            if (minPrice.HasValue)
            {
                model.MaxPrice = minPrice.Value;
                products = products.Where(p => p.ProductPrice >= maxPrice.Value);
            }

            //Tìm sản phẩm có giá cao dần
            if (maxPrice.HasValue)
            {
                model.MaxPrice = maxPrice.Value;
                products = products.Where(p => p.ProductPrice <= maxPrice.Value);
            }

            //Dựa trên lựa chọn của người dùng
            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy (p => p.ProductName);
                    break;
                case "name_desc":
                    products = products.OrderBy (p => p.ProductName);
                    break;
                case "price_asc":
                    products = products.OrderBy (p => p.ProductPrice);
                    break;
                case "price_desc":
                    products = products.OrderBy (p => p.ProductPrice);
                    break;
                default: //Xếp mặc định theo tên
                    products = products.OrderBy (p => p.ProductName);
                    break;
            }

            model.SortOrder = sortOrder;

            //Phân trang
            int pageNumber = page ?? 1; //Mặc địch là 1, trang hiện tại
            int pageSize = 10; //Số sản phẩm mỗi trang

            model.Products = products.ToPagedList(pageNumber, pageSize);
            return View(model);

        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName"); //Chuẩn bị danh sách danh mục cho ô thả xuống
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,UploadImg")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.UploadImg != null)
                {
                    LuuAnh(product); //Có chọn ảnh thì lưu ảnh vào Content/images
                }
                else
                {
                    product.ProductImage = AnhMacDinh; //Không chọn ảnh thì dùng ảnh mặc định
                }

                if (ModelState.IsValid) //Kiểm tra lại, phòng khi ảnh sai định dạng
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,UploadImg")] Product product)
        {
            if (ModelState.IsValid)
            {
                //Chọn ảnh mới thì lưu ảnh mới, không chọn thì giữ ảnh cũ
                if (product.UploadImg != null)
                {
                    LuuAnh(product);
                }

                if (ModelState.IsValid)
                {
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void LuuAnh(Product product)
        {
            // Chỉ chấp nhận các đuôi file ảnh này
            string[] duoiHopLe = { ".png", ".jpg", ".jpeg", ".gif" };
            string duoiFile = Path.GetExtension(product.UploadImg.FileName).ToLower();
            if (!duoiHopLe.Contains(duoiFile))
            {
                ModelState.AddModelError("UploadImg", "Chỉ chấp nhận các định dạng ảnh PNG, JPG, JPEG và GIF");
                return;
            }

            string filename = Path.GetFileName(product.UploadImg.FileName);
            product.ProductImage = ThuMucAnh + filename;
            product.UploadImg.SaveAs(Path.Combine(Server.MapPath(ThuMucAnh), filename));
        }
    }
}
