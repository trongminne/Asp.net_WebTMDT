using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;

namespace WebBanHang.Controllers
{
    public class BookedController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();

        public ActionResult Index()
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            int userId = (int)Session["idUser"];

            var orderDetails = (from od in objWebBanHangEntities.OrderDetails
                                join p in objWebBanHangEntities.Products on od.ProductId equals p.id
                                join o in objWebBanHangEntities.Orders on od.OrderId equals o.Id
                                join u in objWebBanHangEntities.Users on od.idUser equals u.Id
                                where od.idUser == userId
                                select new OrderDetailDTO
                                {
                                    Id = od.Id,
                                    ProductId = p.id,
                                    ProductName = p.Name,
                                    ProductAvatar = p.Avatar,
                                    ProductPrice = p.Price,
                                    Quantity = od.Quantity,
                                    SumPrice = p.Price * od.Quantity,
                                    CreatedOnUtc = o.CreatedOnUtc,
                                    Phone = u.Phone,
                                    Address = u.Address,
                                    LastName = u.LastName,
                                    FirstName = u.FirstName,
                                    Status = od.Status,
                                    Pay = od.Pay
                                }).ToList();

            foreach (var orderDetail in orderDetails)
            {
                orderDetail.PayToWord = (orderDetail.Pay != null) ? NumberToWordsConverter.ConvertNumberToWords((int)orderDetail.Pay) : "";
            }

            // Đếm số lượng đơn hàng của user
            int count = objWebBanHangEntities.OrderDetails.Count(od => od.idUser == userId);

            // Lưu số lượng đơn hàng vào Session["order"]
            Session["order"] = count;
            return View(orderDetails);
        }

        // Convert number price to word
        public class NumberToWordsConverter
        {
            private static string[] units = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín", "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười năm", "mười sáu", "mười bảy", "mười tám", "mười chín" };
            private static string[] tens = { "", "", "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };
            private static string[] powers = { "", "ngàn", "triệu", "tỷ" };

            public static string ConvertNumberToWords(int number)
            {
                if (number < 20)
                {
                    return units[number];
                }
                else if (number < 100)
                {
                    return tens[number / 10] + ((number % 10 != 0) ? " " : "") + units[number % 10];
                }
                else if (number < 1000)
                {
                    return units[number / 100] + " trăm" + ((number % 100 != 0) ? " " : "") + ConvertNumberToWords(number % 100);
                }
                else
                {
                    string words = "";
                    int power = 0;
                    while (number > 0)
                    {
                        int chunk = number % 1000;
                        if (chunk != 0)
                        {
                            string chunkWords = ConvertNumberToWords(chunk);
                            words = chunkWords + " " + powers[power] + " " + words;
                        }
                        number /= 1000;
                        power++;
                    }
                    return words.Trim();
                }
            }
        }


        // Xoá
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var objOD = objWebBanHangEntities.OrderDetails.Where(n => n.Id == id).FirstOrDefault();
            return View(objOD);
        }

        [HttpPost]
        public ActionResult Delete(OrderDetail objODetail)
        {
            var objOD = objWebBanHangEntities.OrderDetails.Where(n => n.Id == objODetail.Id).FirstOrDefault();
            if (objOD == null)
            {
                return HttpNotFound();
            }

            var orderDetail = objWebBanHangEntities.OrderDetails.Find(objODetail.Id);

            if (orderDetail != null)
            {
                var productId = orderDetail.ProductId;
                var quantity = orderDetail.Quantity;

                objWebBanHangEntities.OrderDetails.Remove(orderDetail);

                var product = objWebBanHangEntities.Products.FirstOrDefault(p => p.id == productId);

                if (product != null)
                {
                    product.quantity += quantity;
                }


            }
            objWebBanHangEntities.OrderDetails.Remove(objOD);
            objWebBanHangEntities.SaveChanges();

            return Json(new { success = true });
        }


        [HttpPost]
        public ActionResult DeleteSelected(int[] ids)
        {
            foreach (var id in ids)
            {
                var orderDetail = objWebBanHangEntities.OrderDetails.Find(id);

                if (orderDetail != null)
                {
                    var productId = orderDetail.ProductId;
                    var quantity = orderDetail.Quantity;

                    objWebBanHangEntities.OrderDetails.Remove(orderDetail);

                    var product = objWebBanHangEntities.Products.FirstOrDefault(p => p.id == productId);

                    if (product != null)
                    {
                        product.quantity += quantity;
                    }

                    Session["order"] = (int)Session["order"] - 1;
                }
            }

            objWebBanHangEntities.SaveChanges();

            return RedirectToAction("Index");
        }


        // Xác nhận đã nhận hàng
        [HttpPost]
        public ActionResult UpdateStatus(int orderId)
        {
            // Tìm đơn hàng cần cập nhật trạng thái
            var order = objWebBanHangEntities.OrderDetails.FirstOrDefault(o => o.Id == orderId);

            if (order != null)
            {

                // Cập nhật trạng thái của đơn hàng thành 2
                order.Status = 2;

                
                var product = objWebBanHangEntities.Products.FirstOrDefault(p => p.id == order.ProductId);
               
                // Gửi email thông báo khi đơn hàng được chuyển sang trạng thái "đang giao hàng"
                if (order.Status == 2)
                {
                    var user = objWebBanHangEntities.Users.Where(u => u.Id == order.idUser).FirstOrDefault();

                    var toEmail = user.Email; // Email của người dùng

                    var toProduct = product.Name; // Product của người dùng
                    var productId = product.id; // id product người mua

                    string subject = "Đơn hàng " + toProduct + " của bạn đã được giao dịch thành công";
                    string body = "Đơn hàng <a href='http://localhost:51296/Product/Detail/" + productId + "'>" + toProduct + "</a> giao dịch thành công. \n Cảm ơn bạn đã ủng hộ shop! \n Chúc bạn ngày mới hạnh phúc vv...!";

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress("aevantho4@gmail.com"); // Email của người gửi
                        mailMessage.To.Add(toEmail);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = true;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
                        {
                            smtp.Port = 587; // Cổng SMTP
                            smtp.Credentials = new NetworkCredential("aevantho4@gmail.com", "gvwzlmtjjjtwswxs"); // Tài khoản email người gửi
                            smtp.EnableSsl = true;
                            smtp.Send(mailMessage);
                        }
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    objWebBanHangEntities.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            return View();
        }
    }
}