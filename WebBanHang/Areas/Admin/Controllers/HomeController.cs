using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {

        // GET: Admin/Home
        public ActionResult Index()
        {
            if (Session["admin"] != null && (bool)Session["admin"] == true)
            {
                // Người dùng có quyền truy cập vào trang Admin
                return View();
            }
            else
            {

                // Người dùng không có quyền truy cập vào trang Admin, đưa họ trở lại trang chủ hoặc trang thông báo lỗi
                return Redirect("~/Home/Index");
            }

        }
    }
}