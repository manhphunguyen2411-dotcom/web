using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using PagedList;

namespace HoangHaPCVN.Models.ViewModel
{
    public class CustomerSearchVM
    {
        //Tìm theo tên, sđt, email hoặc tên đăng nhập
        public string SearchTerm { get; set; }

        //Thứ tự sắp xếp
        public string SortOrder { get; set; }

        //Phân trang
        public IPagedList<Customer> Customers { get; set; }
    }
}