using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace HoangHaPCVN.Areas.Admin.Filters
{
    //Chỉ tài khoản có role Admin vào được các trang được gắn 
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        //Check xem đã đăng nhập và có role Admin chưa
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session == null)
            {
                return false;
            }
            string role = httpContext.Session["UserRole"] as string;
            return role == "Admin";
        }

        //Không được duyệt thì chuyển về trang đăng nhập Admin
        //Đính kèm địa chỉ trang muốn vào (returnUrl) để đăng nhập xong quay lại đúng chỗ
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            {
                area = "Admin",
                controller = "Account",
                action = "Login",
                returnUrl = filterContext.HttpContext.Request.RawUrl
            }));
        }
    }
}