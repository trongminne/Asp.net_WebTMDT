using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class CartController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();
        // GET: Cart
        public ActionResult Index()
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            string code = Request.QueryString["code"];

            if (!string.IsNullOrEmpty(code))
            {
                using (var db = new WebBanHangEntities())
                {
                    var codeObj = db.Codes.FirstOrDefault(c => c.Code1 == code);

                    if (codeObj != null)
                    {
                        var name = codeObj.Name;
                        var price = codeObj.Price;
                        var formattedPrice = string.Format("{0:#,##0}", price);

                        ViewBag.Name = name;
                        ViewBag.Price = price;

                        TempData["SuccessMessage"] = "Nhập mã thành công phiếu giảm giá " + name + " ! Bạn được giảm " + formattedPrice + " đ. Tổng tiền sẽ được chia đều";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Mã giảm giá sai - Hãy kiểm tra lại!";
                    }
                }
            }



            // Lấy danh sách sản phẩm trong giỏ hàng
            return View((List<CartModel>)Session["cart"]);
        }

        public ActionResult AddToCart(int id, int quantity)
        {
            if (Session["cart"] == null)
            {
                List<CartModel> cart = new List<CartModel>();
                cart.Add(new CartModel { Product = objWebBanHangEntities.Products.Find(id), Quantity = quantity });
                Session["cart"] = cart;
                Session["count"] = 1;
            }
            else
            {
                List<CartModel> cart = (List<CartModel>)Session["cart"];
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartModel { Product = objWebBanHangEntities.Products.Find(id), Quantity = quantity });
                    Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                }
                Session["cart"] = cart;
            }
            return Json(new { Message = "Thành công", JsonRequestBehavior.AllowGet });
        }

        private int isExist(int id)
        {
            List<CartModel> cart = (List<CartModel>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.id.Equals(id))
                    return i;
            return -1;
        }

        //xóa sản phẩm khỏi giỏ hàng theo id
        public ActionResult Remove(int Id)
        {
            List<CartModel> li = (List<CartModel>)Session["cart"];
            li.RemoveAll(x => x.Product.id == Id);
            Session["cart"] = li;
            Session["count"] = Convert.ToInt32(Session["count"]) - 1;
            return Json(new { Message = "Thành công", JsonRequestBehavior.AllowGet });
        }

        // Xoá nhiều
        [HttpPost]
        public ActionResult RemoveMultiple(List<int> Ids)
        {
            List<CartModel> cart = (List<CartModel>)Session["cart"];

            foreach (var id in Ids)
            {
                cart.RemoveAll(x => x.Product.id == id);
            }

            Session["cart"] = cart;
            Session["count"] = cart.Count;

            return Json(new { Message = "Thành công", JsonRequestBehavior.AllowGet });
        }

        // Lưu lại giỏ hàng
        public ActionResult SaveCart(List<CartItemModel> cartItems)
        {
            if (cartItems == null)
            {
                return Json(new { Message = "Danh sách sản phẩm rỗng" });
            }

            if (Session["cart"] == null)
            {
                List<CartModel> cart = new List<CartModel>();
                foreach (var cartItem in cartItems)
                {
                    int id = cartItem.itemId;
                    int quantity = cartItem.quantity;
                    cart.Add(new CartModel { Product = objWebBanHangEntities.Products.Find(id), Quantity = quantity });
                }

            }
            else
            {
                List<CartModel> cart = (List<CartModel>)Session["cart"];
                foreach (var cartItem in cartItems)
                {
                    int id = cartItem.itemId;
                    int quantity = cartItem.quantity;
                    int index = isExist(id);
                    if (index != -1)
                    {
                        cart[index].Quantity = quantity;
                    }
                    else
                    {
                        cart.Add(new CartModel { Product = objWebBanHangEntities.Products.Find(id), Quantity = quantity });
                    }
                }

            }

            return Json(new { Message = "Thành công" });
        }

    }
}