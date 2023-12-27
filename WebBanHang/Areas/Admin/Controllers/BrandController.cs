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
    public class BrandController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();

        // Hiển thị
        public ActionResult Index()
        {
            // Xuất danh sách thương hiệu có tên danh mục

            var Bands = objWebBanHangEntities.Brands.ToList();

            return View(Bands);
        }

        // Thêm
        [HttpGet]
        public ActionResult Create()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult Create(Brand objBrand)
        {
            try
            {
                if (objBrand.ImageUpload != null)
                {
                    string extension = Path.GetExtension(objBrand.ImageUpload.FileName);

                    string fileName = Path.GetFileNameWithoutExtension(objBrand.ImageUpload.FileName);

                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;

                    objBrand.Avatar = fileName;

                    objBrand.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/content/images/upload/"), fileName));
                }

                
                if (objWebBanHangEntities.Brands.Any())
                {
                    var maxId = objWebBanHangEntities.Brands.Max(p => p.id);

                    objBrand.id = maxId + 1;
                }

            
                if (ModelState.IsValid)
                {
                   
                    objWebBanHangEntities.Brands.Add(objBrand);
                    objWebBanHangEntities.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm thương hiệu thành công!";
                }

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Hãy kiểm tra lại thông tin thương hiệu!";
            }
            return View();


        }


        // Xoá
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var objBrand = objWebBanHangEntities.Brands.Where(n => n.id == id).FirstOrDefault();
            return View(objBrand);
        }

        [HttpPost]
        public ActionResult Delete(Brand objBra)
        {
            var objBrand = objWebBanHangEntities.Brands.Where(n => n.id == objBra.id).FirstOrDefault();
            if (objBrand == null)
            {
                return HttpNotFound();
            }
            objWebBanHangEntities.Brands.Remove(objBrand);
            objWebBanHangEntities.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult DeleteSelected(int[] ids)
        {
            foreach (var id in ids)
            {
                var Brand = objWebBanHangEntities.Brands.Find(id);
                if (Brand != null)
                {
                    objWebBanHangEntities.Brands.Remove(Brand);
                }
            }
            objWebBanHangEntities.SaveChanges();
            return RedirectToAction("Index");
        }

        // Sửa
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var objBrand = objWebBanHangEntities.Brands.Where(n => n.id == id).FirstOrDefault();

            return View(objBrand);
        }
        [HttpPost]
        public ActionResult Edit(Brand objBrand)
        {
            try
            {
                // Nếu người dùng chọn ảnh mới, lưu ảnh mới vào CSDL và xóa ảnh cũ
                if (objBrand.ImageUpload != null && objBrand.ImageUpload.ContentLength > 0)
                {
                    //ảnh đại diện
                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(objBrand.Avatar))
                    {
                        string imagePath = Path.Combine(Server.MapPath("~/Content/images/upload"), objBrand.Avatar);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Lưu ảnh mới vào CSDL
                    string extension = Path.GetExtension(objBrand.ImageUpload.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(objBrand.ImageUpload.FileName);
                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                    objBrand.Avatar = fileName;
                    objBrand.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/upload/"), fileName));
                }

                // Nếu người dùng không chọn ảnh mới, giữ nguyên ảnh cũ
                else
                {
                    objBrand.Avatar = objBrand.Avatar;
                }

               
                objWebBanHangEntities.Entry(objBrand).State = EntityState.Modified;
                objWebBanHangEntities.SaveChanges();
                TempData["SuccessMessage"] = "Sửa thương hiệu thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Hãy kiểm tra lại thông tin thương hiệu!";
            }

            return RedirectToAction("Edit");
        }

    }
}