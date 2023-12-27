using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();

        // Hiển thị
        public ActionResult Index()
        {
            using (var db = new WebBanHangEntities())
            {
                var users = db.Users.ToList();
                ViewBag.Users = users;
            }
            return View();
        }
        
        // Xoá
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var objUser = objWebBanHangEntities.Users.Where(n => n.Id == id).FirstOrDefault();
            return View(objUser);
        }

        [HttpPost]
        public ActionResult Delete(User objUs)
        {
            var objUser = objWebBanHangEntities.Users.Where(n => n.Id == objUs.Id).FirstOrDefault();
            if (objUser == null)
            {
                return HttpNotFound();
            }
            objWebBanHangEntities.Users.Remove(objUser);
            objWebBanHangEntities.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult DeleteSelected(int[] ids)
        {
            foreach (var id in ids)
            {
                var User = objWebBanHangEntities.Users.Find(id);
                if (User != null)
                {
                    objWebBanHangEntities.Users.Remove(User);
                }
            }
            objWebBanHangEntities.SaveChanges();
            return RedirectToAction("Index");
        }

        
    }
}
