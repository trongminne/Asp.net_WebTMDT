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
    public class CodeController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();

        // GET: Admin/Code
        public ActionResult Index()
        {
            var Codes = objWebBanHangEntities.Codes.ToList();
            return View(Codes);
        }

        // Thêm
        [HttpGet]
        public ActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Create(Code objCode)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    objWebBanHangEntities.Codes.Add(objCode);
                    objWebBanHangEntities.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm Code thành công!";
                }

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Hãy kiểm tra lại thông tin Code!";
            }
            return View();


        }


        // Xoá
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var objCode = objWebBanHangEntities.Codes.Where(n => n.id == id).FirstOrDefault();
            return View(objCode);
        }

        [HttpPost]
        public ActionResult Delete(Code objCod)
        {
            var objCode = objWebBanHangEntities.Codes.Where(n => n.id == objCod.id).FirstOrDefault();
            if (objCode == null)
            {
                return HttpNotFound();
            }
            objWebBanHangEntities.Codes.Remove(objCode);
            objWebBanHangEntities.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult DeleteSelected(int[] ids)
        {
            foreach (var id in ids)
            {
                var Code = objWebBanHangEntities.Codes.Find(id);
                if (Code != null)
                {
                    objWebBanHangEntities.Codes.Remove(Code);
                }
            }
            objWebBanHangEntities.SaveChanges();
            return RedirectToAction("Index");
        }

        // Sửa
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var objCode = objWebBanHangEntities.Codes.Where(n => n.id == id).FirstOrDefault();

            return View(objCode);
        }
        [HttpPost]
        public ActionResult Edit(Code objCode)
        {
            try
            {
                
                objWebBanHangEntities.Entry(objCode).State = EntityState.Modified;
                objWebBanHangEntities.SaveChanges();
                TempData["SuccessMessage"] = "Sửa Code thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Hãy kiểm tra lại thông tin Code!";
            }

            return RedirectToAction("Edit");
        }
    }
}