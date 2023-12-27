using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();
        // GET: Product
        public ActionResult Detail(int id)
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            int productId = id; // id của sản phẩm cần lấy điểm đánh giá
            int userId = 0; // khởi tạo giá trị mặc định của userId

            // Lấy đánh giá product của user khi đã đăng nhập
            if (Session != null && Session["idUser"] != null)
            {
                userId = (int)Session["idUser"]; // id của người dùng đang đăng nhập

                // Lấy điểm đánh giá của sản phẩm productId từ người dùng userId
                var rating = objWebBanHangEntities.danhgias.FirstOrDefault(r => r.ProductId == productId && r.UserId == userId);
                if (rating != null)
                {
                    ViewBag.PointRatingUser = rating.Point;
                }
            }
            else // Lấy làm tròn lên trung bình các đánh giá của product
            {
                double averageRating = 0;
                double totalRatingPoints = 0;

                // Lấy tất cả các đánh giá của sản phẩm productId
                int totalRatings = 0; // khởi tạo biến đếm đánh giá

                var ratings = objWebBanHangEntities.danhgias.Where(r => r.ProductId == productId);

                // Đếm số lượng đánh giá
                totalRatings = ratings.Count();
                ViewBag.CountRating = totalRatings;

                // Lấy tất cả lượt đặt hàng của sản phẩm productId
                int totalOrdertails = 0; // khởi tạo biến đếm order

                var Ordertails = objWebBanHangEntities.OrderDetails.Where(r => r.ProductId == productId);

                // Đếm số lượng order
                totalOrdertails = Ordertails.Count();
                ViewBag.CountOrderdetail = totalOrdertails;

                // Tính tổng số điểm
                if (totalRatings > 0)
                {
                    totalRatingPoints = ratings.Sum(r => r.Point) ?? 0;

                    // Tính điểm trung bình và làm tròn lên
                    averageRating = Math.Ceiling(totalRatingPoints / totalRatings);
                }

                ViewBag.PointRatingUser = averageRating;
            }

         

            var product = objWebBanHangEntities.Products.Find(id);
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList();
            return View(product);
        }

        public ActionResult Search(string name, string price, int? page, string sort)
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            var products = objWebBanHangEntities.Products.Where(p => p.quantity > 0).AsQueryable();

            // tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.Contains(name));
            }

            // tìm kiếm theo giá
            if (!string.IsNullOrEmpty(price))
            {
                string[] priceRange = price.Split('-');
                int minPrice = int.Parse(priceRange[0]);
                int maxPrice = priceRange.Length == 2 ? int.Parse(priceRange[1]) : int.MaxValue;

                products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
            }

            // đếm số lượng sản phẩm tìm được
            ViewBag.ProductCount = products.Count();

            // phân trang
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            var pagedProducts = products.OrderBy(p => p.id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)products.Count() / pageSize);

            ViewBag.SearchTerm = name;
            // trả về view với danh sách sản phẩm tìm được

            ViewBag.Count = products.Count(); // Trả về số lượng sản phẩm tìm được

            return View("Search", pagedProducts);
        }

        public ActionResult Rating(int productId, int rating)
        {
            // Lấy thông tin đăng nhập của người dùng
            int userId = (int)Session["idUser"];

            // Lấy đánh giá đầu tiên của người dùng trên sản phẩm đó (nếu có)
            var userRating = objWebBanHangEntities.danhgias.FirstOrDefault(r => r.ProductId == productId && r.UserId == userId);

            if (userRating != null) // Nếu có đánh giá cũ
            {
                objWebBanHangEntities.danhgias.Remove(userRating); // Xoá đánh giá cũ
            }

            // Thêm đánh giá mới của người dùng vào database
            danhgia ratingRecord = new danhgia
            {
                Point = rating,
                ProductId = productId,
                UserId = userId
            };
            objWebBanHangEntities.danhgias.Add(ratingRecord);
            objWebBanHangEntities.SaveChanges();

            // Trả về thông tin đánh giá đã lưu trữ
            double point = rating;

            return Json(new { point = point }, JsonRequestBehavior.AllowGet);
        }

        // Bình luận sản phẩm
        public ActionResult Comment()
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout
            return View();
        }

        [HttpPost]
        public ActionResult Comment(Comment comment)
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            // Lấy Id người dùng đang đăng nhập từ Session
            int userId = (int)Session["idUser"];
            comment.UserId = userId;

            // Lấy ProductId
            int ProductId = int.Parse(Request.Form["ProductId"]);
            comment.ProductId = ProductId;

            // Lấy đánh giá đầu tiên của người dùng trên sản phẩm đó (nếu có)
            var userComment = objWebBanHangEntities.Comments.FirstOrDefault(r => r.ProductId == ProductId && r.UserId == userId);

            if (userComment != null) // Nếu có đánh giá cũ
            {
                objWebBanHangEntities.Comments.Remove(userComment); // Xoá đánh giá cũ
            }

            // Lấy comment
            string Comment = Request.Form["binhluan"];
            comment.Comment1 = Comment;

            comment.CreatTime = DateTime.Now;

            // Lưu ảnh 1 nếu người dùng đã chọn ảnh
            if (comment.ImageUpload1 != null)
            {
                string extension = Path.GetExtension(comment.ImageUpload1.FileName);

                string fileName = Path.GetFileNameWithoutExtension(comment.ImageUpload1.FileName);

                fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;

                comment.img1 = fileName;

                comment.ImageUpload1.SaveAs(Path.Combine(Server.MapPath("~/content/images/upload/"), fileName));
            }

            // Lưu ảnh 2 nếu người dùng đã chọn ảnh
            if (comment.ImageUpload2 != null)
            {
                string extension = Path.GetExtension(comment.ImageUpload2.FileName);

                string fileName = Path.GetFileNameWithoutExtension(comment.ImageUpload2.FileName);

                fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;

                comment.img2 = fileName;

                comment.ImageUpload2.SaveAs(Path.Combine(Server.MapPath("~/content/images/upload/"), fileName));
            }

            // Lưu video nếu người dùng đã chọn video
            if (comment.VideoFile != null)
            {
                string extension = Path.GetExtension(comment.VideoFile.FileName);

                string fileName = Path.GetFileNameWithoutExtension(comment.VideoFile.FileName);

                fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;

                comment.video = fileName;

                comment.VideoFile.SaveAs(Path.Combine(Server.MapPath("~/content/video/upload/"), fileName));
            }

            objWebBanHangEntities.Configuration.ValidateOnSaveEnabled = false;
            objWebBanHangEntities.Comments.Add(comment);
            objWebBanHangEntities.SaveChanges();

            TempData["SuccessMessage"] = "Bình luận hàng thành công!";

            return RedirectToAction("Detail", "Product", new { id = comment.ProductId });
        }


    }
}