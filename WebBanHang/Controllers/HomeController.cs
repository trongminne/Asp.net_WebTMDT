using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Context;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        WebBanHangEntities objWebBanHangEntities = new WebBanHangEntities();
        public ActionResult Index()
        {
            HomeModel objHomeModel = new HomeModel();

            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            // Lấy dữ liệu bảng category
            objHomeModel.listCategory = objWebBanHangEntities.Categories.ToList();

            // Lấy dữ liệu bảng Brand
            objHomeModel.listBrand = objWebBanHangEntities.Brands.ToList();

            // Lấy dữ liệu bảng Product
            objHomeModel.listProduct = objWebBanHangEntities.Products.ToList();

            return View(objHomeModel);
        }

        // login
        [HttpGet]
        public ActionResult Login()
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var data = objWebBanHangEntities.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["idUser"] = data.FirstOrDefault().Id;
                    Session["admin"] = data.FirstOrDefault().IsAdmin;
                    Session["avatar"] = data.FirstOrDefault().Avatar;

                    // hiển thị số lượng đơn hàng
                    int userId = (int)Session["idUser"];

                    // Đếm số lượng đơn hàng của user
                    int count = objWebBanHangEntities.OrderDetails.Where(od => od.idUser == userId).Count();

                    // Lưu số lượng đơn hàng vào Session["order"]
                    Session["order"] = count;

                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Thông tài khoản hoặc mật khẩu sai!";

                }
            }
            return View();
        }

        //GET: Register
        [HttpGet]
        public ActionResult Register()
        {
            Response.ContentType = "text/html; charset=utf-8";

            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            return View();
        }

        //POST: Register
        [HttpPost]

        public ActionResult Register(User _user)
        {
            Response.ContentType = "text/html; charset=utf-8";

            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            _user.LastName = System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Unicode.GetBytes(_user.LastName));

            if (ModelState.IsValid)
            {
                var check = objWebBanHangEntities.Users.FirstOrDefault(s => s.Email == _user.Email && s.Id != _user.Id);

                if (check == null)
                {
                    if (objWebBanHangEntities.Users.Any())
                    {
                        _user.Id = objWebBanHangEntities.Users.Max(u => u.Id) + 1;
                    }

                    // Avatar
                    if (_user.ImageUpload != null)
                    {
                        string extension = Path.GetExtension(_user.ImageUpload.FileName);

                        string fileName = Path.GetFileNameWithoutExtension(_user.ImageUpload.FileName);

                        fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;

                        _user.Avatar = fileName;

                        _user.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/content/images/upload/"), fileName));
                    }
                    _user.Password = GetMD5(_user.Password);

                    objWebBanHangEntities.Configuration.ValidateOnSaveEnabled = false;
                    objWebBanHangEntities.Users.Add(_user);
                    objWebBanHangEntities.SaveChanges();
                    TempData["SuccessMessage"] = "Đăng kí thành công!";
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Đăng kí thất bại - hãy kiểm tra lại email!";
                }
            }
            return View();
        }


        //Sửa
        [HttpGet]
        public ActionResult Edit()
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login");
            }

            // Lấy thông tin tài khoản từ Database
            int idUser = Convert.ToInt32(Session["idUser"]);
            var user = objWebBanHangEntities.Users.FirstOrDefault(u => u.Id == idUser);



            // Hiển thị trang sửa tài khoản
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout


            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login");
            }

            // Lấy thông tin tài khoản từ Database
            int idUser = Convert.ToInt32(Session["idUser"]);
            var userFromDb = objWebBanHangEntities.Users.FirstOrDefault(u => u.Id == idUser);


            var oldPassword = Request.Form["oldPassword"];
            var newPassword = Request.Form["newPassword"];
            var confirmNewPassword = Request.Form["confirmNewPassword"];


            // Kiểm tra xem mật khẩu cũ nhập vào có đúng với mật khẩu hiện tại của người dùng hay không
            if (GetMD5(oldPassword) != userFromDb.Password)
            {
                TempData["ErrorMessage"] = "Mật khẩu cũ không đúng!";
                return RedirectToAction("Edit");
            }

            // Kiểm tra xem mật khẩu mới và xác nhận lại mật khẩu mới có giống nhau hay không
            if (newPassword != confirmNewPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới và xác nhận lại mật khẩu mới không giống nhau!";
                return RedirectToAction("Edit");
            }

            // Thay đổi mật khẩu
            userFromDb.Password = GetMD5(newPassword);

            // Nếu người dùng chọn ảnh mới, lưu ảnh mới vào CSDL và xóa ảnh cũ
            if (user.ImageUpload != null && user.ImageUpload.ContentLength > 0)
            {
                //ảnh đại diện
                // Xóa ảnh cũ
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    string imagePath = Path.Combine(Server.MapPath("~/Content/images/upload"), user.Avatar);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Lưu ảnh mới vào CSDL
                string extension = Path.GetExtension(user.ImageUpload.FileName);
                string fileName = Path.GetFileNameWithoutExtension(user.ImageUpload.FileName);
                fileName = fileName + "-" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                userFromDb.Avatar = fileName;
                user.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/upload/"), fileName));
            }

            // Nếu người dùng không chọn ảnh mới, giữ nguyên ảnh cũ
            else
            {
                userFromDb.Avatar = user.Avatar;
            }

            // Cập nhật thông tin tài khoản và lưu vào Database
            userFromDb.FirstName = user.FirstName;
            userFromDb.LastName = user.LastName;
            userFromDb.Email = user.Email;
          

            userFromDb.Phone = user.Phone;
            userFromDb.Address = user.Address;

            objWebBanHangEntities.SaveChanges();


            // Hiển thị thông báo thành công và quay trở lại trang chủ
            TempData["SuccessMessage"] = "Cập nhật thông tin tài khoản thành công!";
            return RedirectToAction("Logout");
        }

        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }



        //GET: Forgot
        [HttpGet]
        public ActionResult Forgot()
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            return View();
        }

        //POST: Forgot
        [HttpPost]
        public ActionResult Forgot(User _user)
        {
            // Lấy danh sách cho layout
            ViewBag.Categories = objWebBanHangEntities.Categories.ToList(); // Xuất danh sách category cho layout
            ViewBag.Brands = objWebBanHangEntities.Brands.ToList(); // Xuất danh sách brand cho layout

            // Lấy email từ người dùng
            string email = _user.Email;

            // Kiểm tra email có tồn tại trong cơ sở dữ liệu hay không
            var user = objWebBanHangEntities.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                // Tạo mật khẩu mới và lưu vào cơ sở dữ liệu
                string newPassword = GenerateRandomPassword(8);
                user.Password = GetMD5(newPassword);
                objWebBanHangEntities.SaveChanges();

                // Gửi email chứa mật khẩu mới đến địa chỉ email được đăng ký
                SendNewPasswordEmail(email, newPassword);

                // Hiển thị thông báo cho người dùng
                TempData["SuccessMessage"] = "Mật khẩu đã được gửi vào email!";
            }
            else
            {
                // Hiển thị thông báo lỗi nếu email không tồn tại trong cơ sở dữ liệu
                TempData["ErrorMessage"] = "Email không tồn tại trong hệ thống!";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        private void SendNewPasswordEmail(string email, string newPassword)
        {
            MailMessage message = new MailMessage();
            message.To.Add(email);
            message.Subject = "Mật khẩu mới của bạn";
            message.Body = "Mật khẩu mới: " + newPassword;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("aevantho4@gmail.com", "gvwzlmtjjjtwswxs");
            message.From = new MailAddress("aevantho4@gmail.com");

            smtpClient.Send(message);

            // Cập nhật mật khẩu mới đã được mã hoá vào cơ sở dữ liệu
            var user = objWebBanHangEntities.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                string hashedPassword = GetMD5(newPassword);
                user.Password = hashedPassword;
                objWebBanHangEntities.SaveChanges();
            }
        }

        private string GenerateRandomPassword(int v)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] chars = new char[8];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = allowedChars[random.Next(allowedChars.Length)];
            }

            return new string(chars);
        }

        // end forgot

        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}