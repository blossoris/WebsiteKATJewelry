using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using WebsiteKATJewelry.Models;
namespace WebsiteKATJewelry.Controllers
{
    public class KATController : Controller
    {
        DataJewelryDataContext db = new DataJewelryDataContext();
        // GET: KAT
        private List<SanPham> GetNewProc(int count)
        {
            return db.SanPhams.OrderByDescending(a => a.NgayNhap).Take(count).ToList();
        }
        public ActionResult Index()
        {
            var listNewProc = GetNewProc(8);
            return View(listNewProc);
        }
        public ActionResult NavPartial()
        {
            var danhmuc = from s in db.DanhMucs select s;
            return PartialView(danhmuc);
        }
        public ActionResult SliderPartial()
        {
            return PartialView();
        }
        public ActionResult FooterPartial()
        {
            return PartialView();
        }
        public ActionResult Instagram()
        {
            return View();
        }
        [HttpGet]
        public ActionResult LienHe()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LienHe(Email email)
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

            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(email.From);
            mailMessage.ReplyToList.Add(email.From);
            mailMessage.Subject = "Yêu cầu từ khách hàng";
            mailMessage.IsBodyHtml = true;

            mailMessage.To.Add(new MailAddress(email.To));
            string body = "<html><head>";
            body += "<style>";
            body += "  body { font-family: Arial, sans-serif; background-color: burlywood; }";
            body += "  h1 { color: #333; }";
            body += "  p { color: #555; }";
            body += "  table { width: 100%; border-collapse: collapse; }";
            body += "  th, td { padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }";
            body += "  th { background-color: #f2f2f2; }";
            body += "  strong { color: #333; }";
            body += "</style>";
            body += "</head><body>";

            body += "<h1>Thông tin yêu cầu</h1>";
            body += $"<p>Khách hàng: {email.Name},</p>";
            body += $"<p>Email: {email.From},</p>";
            body += $"<p>Nội dung: {email.Notes}</p>";
            body += "</body></html>";
            mailMessage.Body = body;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {

            }
            return View();
        }
    }
}