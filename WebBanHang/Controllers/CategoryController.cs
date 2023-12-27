using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;

namespace WebBanHang.Controllers
{
    public class CategoryController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();
        // GET: Category
        public ActionResult Index()
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            var lstCategory = objWebBanHangEntities.Categories.ToList();
            return View(lstCategory);
        }
        public ActionResult ProductCategory(int Id, int? page)
        {
            int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang
            int pageNumber = (page ?? 1); // Số trang hiện tại

            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            var lstProduct = objWebBanHangEntities.Products.Where(n => n.CategoryId == Id && n.quantity > 0)
                               .OrderBy(n => n.id)
                               .Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            // Tính tổng số trang
            int totalProducts = objWebBanHangEntities.Products.Count(n => n.CategoryId == Id && n.quantity > 0);
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            // Lưu thông tin phân trang vào ViewBag để truyền qua View
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            // đếm số lượng sản phẩm tìm được
            ViewBag.ProductCount = lstProduct.Count();

            return View(lstProduct);
        }


    }
}