using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using WebsiteKATJewelry.Models;
using static System.Collections.Specialized.BitVector32;

namespace WebsiteKATJewelry.Controllers
{
    public class AccountController : Controller
    {
        DataJewelryDataContext db = new DataJewelryDataContext();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {

                if (db.KhachHangs.Any(t => t.Password == model.Password))
                {
                    ModelState.AddModelError("Password", "Mật khẩu yếu, vui lòng đổi mật khẩu khác.");
                    return View(model);
                }
                if (db.KhachHangs.Any(k => k.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    return View(model);
                }
                if (db.KhachHangs.Any(k => k.SDT == model.PhoneNum))
                {
                    ModelState.AddModelError("PhonNum", "Số điện thoại đã tồn tại.");
                    return View(model);
                }
                var khachHang = new KhachHang
                {
                    HoTen = model.Fullname,
                    DiaChi = model.Address,
                    Email = model.Email,
                    SDT = model.PhoneNum,
                    Password = model.Password
                };
                db.KhachHangs.InsertOnSubmit(khachHang);
                db.SubmitChanges();
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = db.KhachHangs.Where(s => s.Email.Equals(email) && s.Password.Equals(password)).ToList();
                if (user.Count() > 0)
                {
                    if (user != null)
                    {

                        Session["email"] = user.FirstOrDefault().Email;
                        return RedirectToAction("Index", "KAT");
                    }
                    else
                    {
                        ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                        return RedirectToAction("Login");
                    }
                }
            }
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Remove("email");
            return RedirectToAction("Index", "KAT");
        }
        public ActionResult MyAccount()
        {
            try
            {
                var user = (from s in db.KhachHangs where s.Email == Session["email"].ToString() select s).SingleOrDefault();
                if (user != null)
                {
                    return View(user);
                }
                else
                {
                    return RedirectToAction("LogIn");
                }
            }
            catch { return RedirectToAction("LogIn"); }

        }

        [HttpGet]
        public ActionResult ChangePass()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ChangePass(FormCollection f)
        {
            var user = (from s in db.KhachHangs where s.Email == Session["email"].ToString() select s).SingleOrDefault();

            if (ModelState.IsValid)
            {
                if (f["NewPass"] == f["CormNewPass"] && f["CurPass"] == user.Password)
                {

                    user.HoTen = f["hoten"];
                    user.Email = f["email"];
                    user.Password = f["NewPass"];
                }

                db.SubmitChanges();
                TempData["SuccessMessage"] = "Mật khẩu đã được cập nhật thành công.";
                return RedirectToAction("MyAccount");
            }
            return View(user);
        }
        public ActionResult Orders()
        {
            var user1 = (from s in db.KhachHangs where s.Email == Session["email"].ToString() select s).SingleOrDefault();
            //từ tên tài khoản tìm makh, lọc đơn hàng từ makh đã tạo
            int makh = user1.MaKH;
            var clothes2 = from c2 in db.DonHangs where c2.MaKH == makh select c2;

            return PartialView(clothes2);
        }
        public ActionResult OrdersDetails(int iMaHD)
        {
            using (var db = new DataJewelryDataContext())
            {
                var query = (from sp in db.SanPhams
                             join cthd in db.ChiTietDonHangs on sp.MaSP equals cthd.MaSP
                             join hd in db.DonHangs on cthd.MaHD equals hd.MaHD
                             where hd.MaHD == iMaHD
                             select new DonHang_CT
                             {
                                 MaHD = hd.MaHD,
                                 MaSP = sp.MaSP,
                                 TongTien = (int)hd.TongTien,
                                 NgayMua = (DateTime)hd.NgayMua,
                                 DaThanhToan = (bool)hd.DaThanhToan,
                                 TinhTrangDonHang = (int)hd.TinhTrangDonHang,
                                 TenSanPhams = db.ChiTietDonHangs.Where(ct => ct.MaHD == iMaHD).Join(db.SanPhams, cthd => cthd.MaSP, sp => sp.MaSP, (ct, sp) => sp.TenSP).ToList()
                             }).FirstOrDefault();
                return PartialView(query);
            }
        }
        public ActionResult Address(int iMaHD)
        {
            using (var db = new DataJewelryDataContext())
            {
                var query = (from cthd in db.ChiTietDonHangs
                             join kh in db.KhachHangs on cthd.MaKH equals kh.MaKH

                             where cthd.MaHD == iMaHD
                             select new KH_CT
                             {
                                 MaHD = cthd.MaHD,
                                 MaKH = kh.MaKH,
                                 HoTen = kh.HoTen,
                                 Diachi = kh.DiaChi,
                                 sdt = kh.SDT
                             }).FirstOrDefault();
                return PartialView(query);
            }
        }
        [HttpGet]
        public ActionResult ForgotPass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPass(Email email)
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

            var user = db.KhachHangs.SingleOrDefault(n => n.Email == email.From);
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

            return RedirectToAction("Login");
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