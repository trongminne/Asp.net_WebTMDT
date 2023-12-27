using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();

        // Hiển thị
        public ActionResult Index()
        {
            // Xuất danh sách sản phẩm có tên danh mục
            var products = from p in objWebBanHangEntities.Products
                           join c in objWebBanHangEntities.Categories on p.CategoryId equals c.id
                           join b in objWebBanHangEntities.Brands on p.BrandId equals b.id
                           select new ProductDTO
                           {
                               id = p.id,
                               Name = p.Name,
                               Avatar = p.Avatar,
                               CategoryName = c.Name,
                               BrandName = b.Name,
                               Price = p.Price,
                               PriceDiscount = p.PriceDiscount,
                               Slug = p.Slug,
                               quantity = p.quantity

                           };


            return View(products.ToList());
        }

        // Thêm
        [HttpGet]
        public ActionResult Create()
        {
            // Xuất danh sách danh mục
            // Lấy danh sách danh mục và đưa vào SelectList
            var categories = objWebBanHangEntities.Categories.ToList();
            var categoryList = new SelectList(categories, "id", "Name");

            // Đưa SelectList vào ViewData để có thể truy cập từ trong view
            ViewData["CategoryList"] = categoryList;

            // Xuất danh sách thương hiệu
            // Lấy danh sách danh mục và đưa vào SelectList
            var Bands = objWebBanHangEntities.Brands.ToList();
            var bandList = new SelectList(Bands, "id", "Name");

            // Đưa SelectList vào ViewData để có thể truy cập từ trong view
            ViewData["bandList"] = bandList;
            return View();
        }
        [HttpPost]
        public ActionResult Create(Product objProduct)
        {
            try
            {
                if (objProduct.ImageUpload != null)
                {
                    string extension = Path.GetExtension(objProduct.ImageUpload.FileName);

                    string fileName = Path.GetFileNameWithoutExtension(objProduct.ImageUpload.FileName);

                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;

                    objProduct.Avatar = fileName;

                    objProduct.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/content/images/upload/"), fileName));
                }

                // Ảnh 3
                if (objProduct.ImageUpload3 != null)
                {
                    string extension = Path.GetExtension(objProduct.ImageUpload3.FileName);

                    string fileName = Path.GetFileNameWithoutExtension(objProduct.ImageUpload3.FileName);

                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;

                    objProduct.Avatar3 = fileName;

                    objProduct.ImageUpload3.SaveAs(Path.Combine(Server.MapPath("~/content/images/upload/"), fileName));
                }

                // Ảnh 1
                if (objProduct.ImageUpload1 != null)
                {
                    string extension = Path.GetExtension(objProduct.ImageUpload1.FileName);

                    string fileName = Path.GetFileNameWithoutExtension(objProduct.ImageUpload1.FileName);

                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;

                    objProduct.Avatar1 = fileName;

                    objProduct.ImageUpload1.SaveAs(Path.Combine(Server.MapPath("~/content/images/upload/"), fileName));
                }
                // Ảnh 2

                if (objProduct.ImageUpload2 != null)
                {
                    string extension = Path.GetExtension(objProduct.ImageUpload2.FileName);

                    string fileName = Path.GetFileNameWithoutExtension(objProduct.ImageUpload2.FileName);

                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;

                    objProduct.Avatar2 = fileName;

                    objProduct.ImageUpload2.SaveAs(Path.Combine(Server.MapPath("~/content/images/upload/"), fileName));
                }
                if (objWebBanHangEntities.Products.Any())
                {
                    var maxId = objWebBanHangEntities.Products.Max(p => p.id);

                    objProduct.id = maxId + 1;
                }

                var categories = objWebBanHangEntities.Categories.ToList();
                var categoryList = new SelectList(categories, "id", "Name");
                ViewData["CategoryList"] = categoryList;

                var Bands = objWebBanHangEntities.Brands.ToList();
                var bandList = new SelectList(Bands, "id", "Name");
                ViewData["bandList"] = bandList;


                if (ModelState.IsValid)
                {
                    objProduct.CategoryId = int.Parse(Request.Form["CategoryId"]);
                    objProduct.BrandId = int.Parse(Request.Form["BrandId"]);
                    objWebBanHangEntities.Products.Add(objProduct);
                    objWebBanHangEntities.SaveChanges();

                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                }

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Hãy kiểm tra lại thông tin sản phẩm!";
            }
            return View();


        }


        // Xoá
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var objProduct = objWebBanHangEntities.Products.Where(n => n.id == id).FirstOrDefault();
            return View(objProduct);
        }

        [HttpPost]
        public ActionResult Delete(Product objPro)
        {
            var objProduct = objWebBanHangEntities.Products.Where(n => n.id == objPro.id).FirstOrDefault();
            if (objProduct == null)
            {
                return HttpNotFound();
            }
            objWebBanHangEntities.Products.Remove(objProduct);
            objWebBanHangEntities.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult DeleteSelected(int[] ids)
        {
            foreach (var id in ids)
            {
                var product = objWebBanHangEntities.Products.Find(id);
                if (product != null)
                {
                    objWebBanHangEntities.Products.Remove(product);
                }
            }
            objWebBanHangEntities.SaveChanges();
            return RedirectToAction("Index");
        }

        // Sửa
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var objProduct = objWebBanHangEntities.Products.Where(n => n.id == id).FirstOrDefault();


            // Lấy danh sách danh mục và lưu vào ViewBag
            ViewBag.Categories = objWebBanHangEntities.Categories.Where(c => c != null).ToList();

            // Lấy danh sách thương hiệu và lưu vào ViewBag
            ViewBag.Brands = objWebBanHangEntities.Brands.Where(c => c != null).ToList();

            return View(objProduct);
        }
        [HttpPost]
        public ActionResult Edit(Product objProduct)
        {
            try
            {
                // Nếu người dùng chọn ảnh mới, lưu ảnh mới vào CSDL và xóa ảnh cũ
                if (objProduct.ImageUpload != null && objProduct.ImageUpload.ContentLength > 0)
                {
                    //ảnh đại diện
                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(objProduct.Avatar))
                    {
                        string imagePath = Path.Combine(Server.MapPath("~/Content/images/upload"), objProduct.Avatar);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Lưu ảnh mới vào CSDL
                    string extension = Path.GetExtension(objProduct.ImageUpload.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(objProduct.ImageUpload.FileName);
                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                    objProduct.Avatar = fileName;
                    objProduct.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/upload/"), fileName));
                }

                // Nếu người dùng không chọn ảnh mới, giữ nguyên ảnh cũ
                else
                {
                    objProduct.Avatar = objProduct.Avatar;
                }

                // ảnh 1
                // Nếu người dùng chọn ảnh mới, lưu ảnh mới vào CSDL và xóa ảnh cũ
                if (objProduct.ImageUpload1 != null && objProduct.ImageUpload1.ContentLength > 0)
                {
                    
                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(objProduct.Avatar1))
                    {
                        string imagePath = Path.Combine(Server.MapPath("~/Content/images/upload"), objProduct.Avatar1);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Lưu ảnh mới vào CSDL
                    string extension = Path.GetExtension(objProduct.ImageUpload1.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(objProduct.ImageUpload1.FileName);
                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                    objProduct.Avatar1 = fileName;
                    objProduct.ImageUpload1.SaveAs(Path.Combine(Server.MapPath("~/Content/images/upload/"), fileName));
                }

                // Nếu người dùng không chọn ảnh mới, giữ nguyên ảnh cũ
                else
                {
                    objProduct.Avatar1 = objProduct.Avatar1;
                }
                // end ảnh 1

                // ảnh 2
                // Nếu người dùng chọn ảnh mới, lưu ảnh mới vào CSDL và xóa ảnh cũ
                if (objProduct.ImageUpload2 != null && objProduct.ImageUpload2.ContentLength > 0)
                {

                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(objProduct.Avatar2))
                    {
                        string imagePath = Path.Combine(Server.MapPath("~/Content/images/upload"), objProduct.Avatar2);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Lưu ảnh mới vào CSDL
                    string extension = Path.GetExtension(objProduct.ImageUpload2.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(objProduct.ImageUpload2.FileName);
                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                    objProduct.Avatar2 = fileName;
                    objProduct.ImageUpload2.SaveAs(Path.Combine(Server.MapPath("~/Content/images/upload/"), fileName));
                }

                // Nếu người dùng không chọn ảnh mới, giữ nguyên ảnh cũ
                else
                {
                    objProduct.Avatar2 = objProduct.Avatar2;
                }
                // end ảnh 2

                // ảnh 3
                // Nếu người dùng chọn ảnh mới, lưu ảnh mới vào CSDL và xóa ảnh cũ
                if (objProduct.ImageUpload3 != null && objProduct.ImageUpload3.ContentLength > 0)
                {

                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(objProduct.Avatar3))
                    {
                        string imagePath = Path.Combine(Server.MapPath("~/Content/images/upload"), objProduct.Avatar3);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Lưu ảnh mới vào CSDL
                    string extension = Path.GetExtension(objProduct.ImageUpload3.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(objProduct.ImageUpload3.FileName);
                    fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                    objProduct.Avatar3 = fileName;
                    objProduct.ImageUpload3.SaveAs(Path.Combine(Server.MapPath("~/Content/images/upload/"), fileName));
                }

                // Nếu người dùng không chọn ảnh mới, giữ nguyên ảnh cũ
                else
                {
                    objProduct.Avatar3 = objProduct.Avatar3;
                }
                // end ảnh 3

                // Kiểm tra xem TypeId đã thay đổi hay chưa
                if (objProduct.TypeId == ViewBag.OldTypeId)
                {
                    // Nếu không có sự thay đổi, giữ lại TypeId cũ
                    objProduct.TypeId = ViewBag.OldTypeId;
                }

                // Kiểm tra xem CategoryId đã thay đổi hay chưa
                if (objProduct.CategoryId == null)
                {
                    // Nếu người dùng không chọn danh mục mới, giữ lại danh mục cũ
                    objProduct.CategoryId = ViewBag.OldCategoryId;
                }

                // Kiểm tra xem BrandId đã thay đổi hay chưa
                if (objProduct.BrandId == null)
                {
                    // Nếu người dùng không chọn thương hiệu mới, giữ lại thương hiệu cũ
                    objProduct.BrandId = ViewBag.OldBrandId;
                }

                objWebBanHangEntities.Entry(objProduct).State = EntityState.Modified;
                objWebBanHangEntities.SaveChanges();
                TempData["SuccessMessage"] = "Sửa sản phẩm thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Hãy kiểm tra lại thông tin sản phẩm!";
            }

            return RedirectToAction("Edit");
        }

    }
}