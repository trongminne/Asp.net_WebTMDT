using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;
using static WebBanHang.Controllers.BookedController;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();

        // Hiển thị
        public ActionResult Index()
        {

            var orderDetails = (from od in objWebBanHangEntities.OrderDetails
                                join p in objWebBanHangEntities.Products on od.ProductId equals p.id
                                join o in objWebBanHangEntities.Orders on od.OrderId equals o.Id
                                join u in objWebBanHangEntities.Users on od.idUser equals u.Id
                             
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


            return View(orderDetails);
        }

        // Xuất excel
        public ActionResult ExportExcel()
        {
            int? status = null;
            if (!string.IsNullOrEmpty(Request.Form["status"]) && int.TryParse(Request.Form["status"], out int parsedStatus))
            {
                status = parsedStatus;
            }
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Order List");
            IQueryable<OrderDetailDTO> orders;
            if (status == 0)
            {
                // Lấy danh sách order theo trạng thái
                orders = from od in objWebBanHangEntities.OrderDetails
                         join p in objWebBanHangEntities.Products on od.ProductId equals p.id
                         join o in objWebBanHangEntities.Orders on od.OrderId equals o.Id
                         join u in objWebBanHangEntities.Users on od.idUser equals u.Id
                         where (od.Status == status)
                         select new OrderDetailDTO
                         {
                             Id = od.Id,
                             ProductId = p.id,
                             ProductName = p.Name,
                             ProductAvatar = p.Avatar,
                             ProductPrice = p.Price,
                             Quantity = od.Quantity,
                             CreatedOnUtc = o.CreatedOnUtc,
                             Phone = u.Phone,
                             Address = u.Address,
                             LastName = u.LastName,
                             FirstName = u.FirstName,
                             Status = od.Status,
                             Pay = od.Pay
                         };
                // Tạo file Excel từ danh sách order

                ws.Cell(1, 1).Value = "STT";
                ws.Cell(1, 2).Value = "Tên sản phẩm";
                ws.Cell(1, 3).Value = "Tên khách hàng";
                ws.Cell(1, 4).Value = "Số điện thoại";
                ws.Cell(1, 5).Value = "Số lượng";
                ws.Cell(1, 6).Value = "Giá";
                ws.Cell(1, 7).Value = "Thanh toán";
                ws.Cell(1, 8).Value = "Thành chữ"; // Thêm cột "Thành chữ"
                ws.Cell(1, 9).Value = "Ngày đặt";
                ws.Cell(1, 10).Value = "Trạng thái";
                int row = 2;
                foreach (var order in orders)
                {
                    ws.Cell(row, 1).Value = row - 1;
                    ws.Cell(row, 2).Value = order.ProductName;
                    ws.Cell(row, 3).Value = order.FirstName + " " + order.LastName;
                    ws.Cell(row, 4).Value = order.Phone;
                    ws.Cell(row, 5).Value = order.Quantity;
                    ws.Cell(row, 6).Value = String.Format("{0:#,###}", order.ProductPrice) + "₫";
                    ws.Cell(row, 7).Value = String.Format("{0:#,###}", order.Pay) + "₫";
                    ws.Cell(row, 8).Value = order.Pay.HasValue ? NumberToWordsConverter.ConvertNumberToWords((int)order.Pay) + " đồng" : ""; // Kiểm tra giá trị null trước khi chuyển đổi // Chuyển đổi số thành chữ
                    ws.Cell(row, 9).Value = order.CreatedOnUtc;
                    ws.Cell(row, 10).Value = order.Status == 1 ? "Đang giao hàng" :
                                            (order.Status == 2 ? "Đã nhận hàng" : "Chờ xác nhận");
                    row++;
                }

                foreach (var order in orders)
                {
                    if (order.Status == 0) // Order đang chờ xác nhận
                    {
                        order.Status = 1; // Chuyển sang trạng thái đang giao hàng
                        var orderToUpdate = objWebBanHangEntities.OrderDetails.Single(o => o.Id == order.Id);
                        orderToUpdate.Status = order.Status;
                    }
                }
                objWebBanHangEntities.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }
            else
            {
                // Lấy danh sách order theo trạng thái
                orders = from od in objWebBanHangEntities.OrderDetails
                         join p in objWebBanHangEntities.Products on od.ProductId equals p.id
                         join o in objWebBanHangEntities.Orders on od.OrderId equals o.Id
                         join u in objWebBanHangEntities.Users on od.idUser equals u.Id
                         where (status == null || od.Status == status)
                         select new OrderDetailDTO
                         {
                             Id = od.Id,
                             ProductId = p.id,
                             ProductName = p.Name,
                             ProductAvatar = p.Avatar,
                             ProductPrice = p.Price,
                             Quantity = od.Quantity,
                             CreatedOnUtc = o.CreatedOnUtc,
                             Phone = u.Phone,
                             Address = u.Address,
                             LastName = u.LastName,
                             FirstName = u.FirstName,
                             Status = od.Status,
                             Pay = od.Pay
                         };
                // Tạo file Excel từ danh sách order

                ws.Cell(1, 1).Value = "STT";
                ws.Cell(1, 2).Value = "Tên sản phẩm";
                ws.Cell(1, 3).Value = "Tên khách hàng";
                ws.Cell(1, 4).Value = "Số điện thoại";
                ws.Cell(1, 5).Value = "Số lượng";
                ws.Cell(1, 6).Value = "Giá";
                ws.Cell(1, 7).Value = "Thanh toán";
                ws.Cell(1, 8).Value = "Thành chữ"; // Thêm cột "Thành chữ"
                ws.Cell(1, 9).Value = "Ngày đặt";
                ws.Cell(1, 10).Value = "Trạng thái";
                int row = 2;
                foreach (var order in orders)
                {
                    ws.Cell(row, 1).Value = row - 1;
                    ws.Cell(row, 2).Value = order.ProductName;
                    ws.Cell(row, 3).Value = order.FirstName + " " + order.LastName;
                    ws.Cell(row, 4).Value = order.Phone;
                    ws.Cell(row, 5).Value = order.Quantity;
                    ws.Cell(row, 6).Value = String.Format("{0:#,###}", order.ProductPrice) + "₫";
                    ws.Cell(row, 7).Value = String.Format("{0:#,###}", order.Pay) + "₫";
                    ws.Cell(row, 8).Value = order.Pay.HasValue ? NumberToWordsConverter.ConvertNumberToWords((int)order.Pay) + " đồng" : ""; // Kiểm tra giá trị null trước khi chuyển đổi // Chuyển đổi số thành chữ
                    ws.Cell(row, 9).Value = order.CreatedOnUtc;
                    ws.Cell(row, 10).Value = order.Status == 1 ? "Đang giao hàng" :
                                            (order.Status == 2 ? "Đã nhận hàng" : "Chờ xác nhận");
                    row++;
                }
            }

            ws.Columns().AdjustToContents();

            // Xuất file Excel
            var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;
            var fileName = "donhang_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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



        public ActionResult EditStatus(int id, int status)
        {
            try
            {
                var objOD = objWebBanHangEntities.OrderDetails.Find(id);
                if (objOD == null)
                {
                    return HttpNotFound();
                }
                objOD.Status = status;
                objWebBanHangEntities.SaveChanges();


                // Gửi email thông báo khi đơn hàng được chuyển sang trạng thái "đang giao hàng"
                if (objOD.Status == 1)
                {
                    var user = objWebBanHangEntities.Users.Where(u => u.Id == objOD.idUser).FirstOrDefault();

                    var toEmail = user.Email; // Email của người dùng

                    var product = objWebBanHangEntities.Products.Where(u => u.id == objOD.ProductId).FirstOrDefault();

                    var toProduct = product.Name; // Product của người dùng
                    var productId = product.id; // id product người mua

                    string subject = "Đơn hàng " + toProduct + " của bạn đã được giao đi";
                    string body = "Đơn hàng <a href='http://localhost:51296/Product/Detail/" + productId + "'>" + toProduct + "</a> của bạn đã được giao đi. \n Cảm ơn bạn đã ủng hộ shop! \n Chúc bạn ngày mới hạnh phúc vv...!";

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
                }


            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Hãy kiểm tra lại thông tin Đơn hàng!";
            }

            return RedirectToAction("Edit");
        }

        // Sửa
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var objOD = objWebBanHangEntities.OrderDetails.Where(n => n.Id == id).FirstOrDefault();

            return View(objOD);
        }
        [HttpPost]
        public ActionResult Edit(OrderDetail objOD)
        {
            try
            {
                objOD.OrderId = objOD.OrderId;
                objOD.ProductId = objOD.ProductId;
                objOD.Quantity = objOD.Quantity;
                objOD.idUser = objOD.idUser;
                objWebBanHangEntities.Entry(objOD).State = EntityState.Modified;
                objWebBanHangEntities.SaveChanges();

                // Gửi email thông báo khi đơn hàng được chuyển sang trạng thái "đang giao hàng"
                if (objOD.Status == 1)
                {
                    var user = objWebBanHangEntities.Users.Where(u => u.Id == objOD.idUser).FirstOrDefault();

                    var toEmail = user.Email; // Email của người dùng

                    var product = objWebBanHangEntities.Products.Where(u => u.id == objOD.ProductId).FirstOrDefault();

                    var toProduct = product.Name; // Product của người dùng
                    var productId = product.id; // id product người mua

                    string subject = "Đơn hàng " + toProduct + " của bạn đã được giao đi";
                    string body = "Đơn hàng <a href='http://localhost:51296/Product/Detail/" + productId + "'>" + toProduct + "</a> của bạn đã được giao đi. \n Cảm ơn bạn đã ủng hộ shop! \n Chúc bạn ngày mới hạnh phúc vv...!";

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
                }
                TempData["SuccessMessage"] = "Sửa Đơn hàng thành công!";

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Hãy kiểm tra lại thông tin Đơn hàng!";
            }

            return RedirectToAction("Edit");
        }

    }
}