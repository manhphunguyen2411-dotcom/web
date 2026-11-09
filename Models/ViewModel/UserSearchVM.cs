using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace HoangHaPCVN.Models.ViewModel
{
    public class UserSearchVM
    {
        //Từ khóa tìm theo tên đăng nhập
        public string SearchTerm { get; set; }
        //Phân trang
        public IPagedList<User> Users { get; set; }
    }
}