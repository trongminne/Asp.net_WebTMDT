using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;

namespace WebBanHang.Controllers
{
    public class BrandController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();

        // GET: Brand
        public ActionResult Index()
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            var lstBrand = objWebBanHangEntities.Brands.ToList();


            return View(lstBrand);
        }
        // Danh sách sp thuộc thương hiệu
        public ActionResult ProductBrand(int Id, int page = 1)
        {
            int pageSize = 12; // số lượng sản phẩm trên mỗi trang
                               // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            var lstProduct = objWebBanHangEntities.Products.Where(n => n.BrandId == Id && n.quantity > 0)
                                                .OrderBy(n => n.id)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToList();
           
            int totalItems = objWebBanHangEntities.Products.Where(n => n.BrandId == Id && n.quantity > 0).Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // đếm số lượng sản phẩm tìm được
            ViewBag.ProductCount = lstProduct.Count();

            return View(lstProduct);
        }

    }
}