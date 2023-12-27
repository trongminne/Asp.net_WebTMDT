using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class PaymentsController : Controller
    {
        // GET: Payments
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();

        [HttpPost]
        // GET: Payment
        public ActionResult Index()
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout


            // Lây thông tin giỏ hàng từ  session
            var LstCart = (List<CartModel>)Session["cart"];

            // Gán dữ liệu cho Order
            Order objOrder = new Order();
            objOrder.Name = "Donhang-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            objOrder.UserId = int.Parse(Session["idUser"].ToString());
            objOrder.CreatedOnUtc = DateTime.Now;
            objOrder.Status = 1;
            objWebBanHangEntities.Orders.Add(objOrder);

            // Lưu thông tin dữ liệu vào bảng order
            objWebBanHangEntities.SaveChanges();


            // Lưu OrderId vừa mới tạo để lưu vào bảng Orderdetail.
            int intOrderId = objOrder.Id;

            List<OrderDetail> lstOrderDetail = new List<OrderDetail>();

            // Kiểm tra có nhập code không
            int countProduct = 0;
            double totalPriceB = 0;
            foreach (var item in LstCart)
            {
                countProduct++;
                var product = objWebBanHangEntities.Products.FirstOrDefault(p => p.id == item.Product.id);

                var totalPriceA = Convert.ToDouble(item.Quantity * product.Price); // tổng tiền đặt
                totalPriceB += totalPriceA;
            }

            double SumPrice = Convert.ToDouble(Request.Form["SumPrice"]); // Tiến chuyền sang

            if (totalPriceB == SumPrice)
            {
                foreach (var item in LstCart)
                {

                    // tạo đối tượng OrderDetail
                    OrderDetail obj = new OrderDetail();
                    // Trừ quantity của sản phẩm trong bảng Product với quantity trong OrderDetail
                    var product = objWebBanHangEntities.Products.FirstOrDefault(p => p.id == item.Product.id);
                    if (product != null)
                    {
                        product.quantity -= item.Quantity;
                    }

                    obj.OrderId = intOrderId;
                    obj.Status = 0;
                    obj.idUser = int.Parse(Session["idUser"].ToString());
                    obj.ProductId = item.Product.id;
                    obj.Quantity = item.Quantity;
                    obj.Pay = item.Quantity * product.Price;
                    lstOrderDetail.Add(obj);

                }
            }
            else
            {
                //Chia đều tiền cho các đơn nếu khuyến mãi
                double pay = SumPrice / countProduct;
                foreach (var item in LstCart)
                {

                    // tạo đối tượng OrderDetail
                    OrderDetail obj = new OrderDetail();
                    // Trừ quantity của sản phẩm trong bảng Product với quantity trong OrderDetail
                    var product = objWebBanHangEntities.Products.FirstOrDefault(p => p.id == item.Product.id);
                    if (product != null)
                    {
                        product.quantity -= item.Quantity;
                    }

                    obj.OrderId = intOrderId;
                    obj.Status = 0;
                    obj.idUser = int.Parse(Session["idUser"].ToString());
                    obj.ProductId = item.Product.id;
                    obj.Quantity = item.Quantity;
                    obj.Pay = pay;
                    lstOrderDetail.Add(obj);

                }
            }
            
            // Xoá toàn bộ sản phẩm đang lưu trong giỏ hàng
            Session.Remove("cart");
            Session.Remove("count");


            objWebBanHangEntities.OrderDetails.AddRange(lstOrderDetail);
            objWebBanHangEntities.SaveChanges();

            return View();

        }

        public ActionResult BuyNow(int id)
        {
            // Lấy thông tin sản phẩm
            var product = objWebBanHangEntities.Products.FirstOrDefault(p => p.id == id);

            //// Lấy số lượng sản phẩm từ input
            //int quantity;
            //if (!int.TryParse(Request.Form["quantity"], out quantity))
            //{
            //    // Nếu không thể chuyển đổi thành số nguyên, trả về lỗi hoặc chuyển hướng đến trang lỗi
            //    TempData["ErrorMessage"] = "Không có số lượng sản phẩm";
            //    return RedirectToAction("Detail", "Product", new { id = id });
            //}

            //// Trừ quantity của sản phẩm trong bảng Product với quantity trong OrderDetail

            int quantity = int.Parse(Request.Form["quantity"]);



            // Kiểm tra người dùng đã đăng nhập hay chưa
            if (Session["idUser"] == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để mua hàng!";
                return RedirectToAction("Login", "Home");
            }

            // Gán dữ liệu cho Order
            Order objOrder = new Order();
            objOrder.Name = "Donhang-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            objOrder.UserId = int.Parse(Session["idUser"].ToString());
            objOrder.CreatedOnUtc = DateTime.Now;
            objOrder.Status = 1;
            objWebBanHangEntities.Orders.Add(objOrder);

            // Lưu thông tin dữ liệu vào bảng order
            objWebBanHangEntities.SaveChanges();

            // Lưu thông tin dữ liệu vào bảng orderdetail
            OrderDetail objOrderDetail = new OrderDetail();
            objOrderDetail.OrderId = objOrder.Id;
            objOrderDetail.ProductId = product.id;
            objOrderDetail.idUser = int.Parse(Session["idUser"].ToString());

            objOrderDetail.Quantity = quantity;
            objOrderDetail.Status = 0;
            objWebBanHangEntities.OrderDetails.Add(objOrderDetail);

            product.quantity -= quantity;
            // Lưu thông tin sản phẩm vào cơ sở dữ liệu
            objWebBanHangEntities.Entry(product).State = EntityState.Modified;

            objWebBanHangEntities.SaveChanges();

            TempData["SuccessMessage"] = "Đặt hàng thành công!";
            return RedirectToAction("Index", "Booked");
        }


    }
}