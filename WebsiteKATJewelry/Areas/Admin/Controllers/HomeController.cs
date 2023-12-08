using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteKATJewelry.Models;
using WebsiteKATJewelry.Areas.Admin.Models;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace WebsiteKATJewelry.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        DataJewelryDataContext db = new DataJewelryDataContext();
        // GET: Admin/Home
        public ActionResult Index()
        {
            var revenueByMonth = db.DonHangs
          .Where(dh => dh.NgayGiao != null && dh.NgayGiao.Value.Year == DateTime.Now.Year && dh.DaThanhToan==true)
          .GroupBy(dh => new { Year = dh.NgayGiao.Value.Year, Month = dh.NgayGiao.Value.Month })
         .Select(group => new RevenueViewModel
         {
             Year = group.Key.Year,
             Month = group.Key.Month,
             Revenue = group.Sum(dh => dh.TongTien ?? 0) // Coalesce to 0 if TongTien is null
             })
          .ToList();


            ViewBag.RevenueByMonth = revenueByMonth;

            return View();
        }
        [HttpGet]
        public ActionResult LoginAd()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAd(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = db.Admins.Where(s => s.Email.Equals(email) && s.Password.Equals(password)).ToList();
                if (user.Count() > 0)
                {
                    if (user != null)
                    {
                        Session["Username"] = user.FirstOrDefault().Email;
                        Session["Name"] = user.FirstOrDefault().HoTen;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                        return RedirectToAction("LoginAd");
                    }
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult ChangePassAd()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ChangePassAd(FormCollection f)
        {
            var user = (from s in db.Admins where s.Email == Session["Username"].ToString() select s).SingleOrDefault();

            if (ModelState.IsValid)
            {
                if (f["NewPass"] == f["CormNewPass"] && f["CurPass"] == user.Password)
                {
                    user.Password = f["NewPass"];
                }

                db.SubmitChanges();
                TempData["SuccessMessage"] = "Mật khẩu đã được cập nhật thành công.";
                return RedirectToAction("Details");
            }
            return View(user);
        }
        public ActionResult Details()
        {
            var user = (from s in db.Admins where s.Email == Session["Username"].ToString() select s).SingleOrDefault(); // Lấy một đối tượng Admin
            return View(user);
        }

        public ActionResult LogOut()
        {
            Session.Remove("Username");
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult ForgotPassAd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassAd(Email email)
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "2124802010728@student.tdmu.edu.vn";
            string smtpPassword = "xhpe ltdu adra ahvl";

            SmtpClient smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var user = db.Admins.SingleOrDefault(n => n.Email == email.From);
            string randomPassword = GenerateRandomPassword(12);
            user.Password = randomPassword;
            db.SubmitChanges();
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(smtpUsername);
            mailMessage.ReplyToList.Add(smtpUsername);
            mailMessage.Subject = "Đặt lại mật khẩu";
            mailMessage.IsBodyHtml = true;

            mailMessage.To.Add(email.From);
            string body = "<html><head>";
            body += "<style>";
            body += "  body { font-family: Arial, sans-serif; background-color: burlywood; }";
            body += "  h4 { color: #333; }";
            body += "  p { color: #555; }";
            body += "  table { width: 100%; border-collapse: collapse; }";
            body += "  th, td { padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }";
            body += "  th { background-color: #f2f2f2; }";
            body += "  strong { color: #333; }";
            body += "</style>";
            body += "</head><body>";

            body += "<h4>Mật khẩu mới</h4>";
            body += $"<p>Xin chào {user.HoTen}, đây là mật khẩu mới của bạn: {randomPassword},</p>";
            body += $"<p>Vui lòng đăng nhập và đổi lại mật khẩu!</p>";
            body += "</body></html>";
            mailMessage.Body = body;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {

            }
            TempData["SuccessMessage"] = "Mật khẩu mới đã được gửi qua email, vui lòng kiểm tra email!";

            return RedirectToAction("LoginAd");
        }
        public static string GenerateRandomPassword(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-=_+";

            // Sử dụng Random để tạo chuỗi ngẫu nhiên
            Random random = new Random();
            StringBuilder password = new StringBuilder();

            // Tạo mật khẩu ngẫu nhiên với độ dài đã cho
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                password.Append(chars[index]);
            }

            return password.ToString();
        }
    }
}