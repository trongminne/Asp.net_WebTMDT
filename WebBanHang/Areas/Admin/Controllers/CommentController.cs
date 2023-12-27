using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class CommentController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();

        // GET: Admin/Comment
        public ActionResult Index()
        {

            var comments = from c in objWebBanHangEntities.Comments
                               join p in objWebBanHangEntities.Products on c.ProductId equals p.id
                               join u in objWebBanHangEntities.Users on c.UserId equals u.Id
                               
                               select new CommentDTO
                               {
                                   id = c.id,
                                  Avatar = u.Avatar,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  Comment1 = c.Comment1,
                                  ProductId = p.id,
                                  NameProduct = p.Name,
                                  AvatarProduct = p.Avatar

                               };
            return View(comments.ToList());
        }

        // Xoá
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var objCM = objWebBanHangEntities.Comments.Where(n => n.id == id).FirstOrDefault();
            return View(objCM);
        }

        [HttpPost]
        public ActionResult Delete(Comment objOComment)
        {
            var objCM = objWebBanHangEntities.Comments.Where(n => n.id == objOComment.id).FirstOrDefault();
            if (objCM == null)
            {
                return HttpNotFound();
            }
            objWebBanHangEntities.Comments.Remove(objCM);
            objWebBanHangEntities.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult DeleteSelected(int[] ids)
        {
            foreach (var id in ids)
            {
                var Comment = objWebBanHangEntities.Comments.Find(id);
                if (Comment != null)
                {
                    objWebBanHangEntities.Comments.Remove(Comment);
                }
            }
            objWebBanHangEntities.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}