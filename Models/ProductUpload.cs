using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HoangHaPCVN.Models
{
    //Partial: Phần viết thêm của class Product do EF tự gen ra
    //Để ở file riêng thì Update from DB sẽ không bị xóa mất
    //Thay vì thêm thẳng vào file "Product.cs" do file đó EF tự gen ra nên mỗi lần Update Model from DB thì phần thêm vào sẽ bị xóa. Vì vậy đặt ở file riêng dùng PV để ghép vào class Product 
        public partial class Product
        {
            [NotMapped] //Đánh dấu đây là trường dữ liệu để không lưu vào DB
            public HttpPostedFileBase UploadImg { get; set; } //Thêm trường dữ liệu UploadImg kiểu HttpPostedFileBase để upload file ảnh
        }
}