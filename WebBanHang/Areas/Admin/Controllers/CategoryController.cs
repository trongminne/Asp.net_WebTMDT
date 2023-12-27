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
    public class CategoryController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();

        // Hiển thị
        public ActionResult Index()
        {
            // Xuất danh sách danh mục có tên danh mục

            var Categories = objWebBanHangEntities.Categories.ToList();

            return View(Categories);
        }

        // Thêm
        [HttpGet]
        public ActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Create(Category objCategory)
        {
            try
            {
                if (objCategory.ImageUpload != null)
                {
                    string extension = Path.GetExtension(objCategory.ImageUpload.FileName);

                    string fileName = Path.GetFileNameWithoutExtension(objCategory.ImageUpload.FileName);

                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;

                    objCategory.Avatar = fileName;

                    objCategory.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/content/images/upload/"), fileName));
                }


                if (objWebBanHangEntities.Categories.Any())
                {
                    var maxId = objWebBanHangEntities.Categories.Max(p => p.id);

                    objCategory.id = maxId + 1;
                }


                if (ModelState.IsValid)
                {

                    objWebBanHangEntities.Categories.Add(objCategory);
                    objWebBanHangEntities.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                }

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Hãy kiểm tra lại thông tin danh mục!";
            }
            return View();


        }


        // Xoá
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var objCategory = objWebBanHangEntities.Categories.Where(n => n.id == id).FirstOrDefault();
            return View(objCategory);
        }

        [HttpPost]
        public ActionResult Delete(Category objCate)
        {
            var objCategory = objWebBanHangEntities.Categories.Where(n => n.id == objCate.id).FirstOrDefault();
            if (objCategory == null)
            {
                return HttpNotFound();
            }
            objWebBanHangEntities.Categories.Remove(objCategory);
            objWebBanHangEntities.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult DeleteSelected(int[] ids)
        {
            foreach (var id in ids)
            {
                var Category = objWebBanHangEntities.Categories.Find(id);
                if (Category != null)
                {
                    objWebBanHangEntities.Categories.Remove(Category);
                }
            }
            objWebBanHangEntities.SaveChanges();
            return RedirectToAction("Index");
        }

        // Sửa
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var objCategory = objWebBanHangEntities.Categories.Where(n => n.id == id).FirstOrDefault();

            return View(objCategory);
        }
        [HttpPost]
        public ActionResult Edit(Category objCategory)
        {
            try
            {
                // Nếu người dùng chọn ảnh mới, lưu ảnh mới vào CSDL và xóa ảnh cũ
                if (objCategory.ImageUpload != null && objCategory.ImageUpload.ContentLength > 0)
                {
                    //ảnh đại diện
                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(objCategory.Avatar))
                    {
                        string imagePath = Path.Combine(Server.MapPath("~/Content/images/upload"), objCategory.Avatar);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Lưu ảnh mới vào CSDL
                    string extension = Path.GetExtension(objCategory.ImageUpload.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(objCategory.ImageUpload.FileName);
                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                    objCategory.Avatar = fileName;
                    objCategory.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/upload/"), fileName));
                }

                // Nếu người dùng không chọn ảnh mới, giữ nguyên ảnh cũ
                else
                {
                    objCategory.Avatar = objCategory.Avatar;
                }


                objWebBanHangEntities.Entry(objCategory).State = EntityState.Modified;
                objWebBanHangEntities.SaveChanges();
                TempData["SuccessMessage"] = "Sửa danh mục thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Hãy kiểm tra lại thông tin danh mục!";
            }

            return RedirectToAction("Edit");
        }

    }
}