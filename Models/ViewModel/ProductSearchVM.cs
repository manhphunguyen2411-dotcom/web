using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace HoangHaPCVN.Models.ViewModel
{
    public class ProductSearchVM
    {
        //Tiêu chí để tìm kiếm theo tên, mô tả sản phẩm hoặc loại sản phẩm
        public string SearchTerm { get; set; }

        //Các tiêu chí để tìm kiếm theo giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        //Thứ tự sắp xếp
        public string SortOrder { get; set; }

        //Danh sách sản phẩm đã phân trang
        public IPagedList<Product> Products { get; set; }
    }
}