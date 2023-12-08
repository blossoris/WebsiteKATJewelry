using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WebsiteKATJewelry.Models;
using static System.Collections.Specialized.BitVector32;


namespace WebsiteKATJewelry.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        DataJewelryDataContext db = new DataJewelryDataContext();
        public List<Cart> LayGioHang()
        {
            List<Cart> lst = Session["Cart"] as List<Cart>;
            if (lst == null)
            {
                lst = new List<Cart>();
                Session["Cart"] = lst;
            }
            return lst;
        }

        public ActionResult ThemGioHang(int ms, string url)
        {
            List<Cart> lst = LayGioHang();
            Cart sp = lst.Find(n => n.iMaSP == ms);

            if (sp == null)
            {

                sp = new Cart(ms, 1);
                lst.Add(sp);
            }
            else
            {
                sp.iSoLuong++;
            }
            return Redirect(url);
        }
        //Thêm mới giỏ hàng trong chi tiết giỏ hàng
        [HttpPost]
        public ActionResult ThemGioHang1(int ms, string url, FormCollection f)
        {
            List<Cart> lst = LayGioHang();
            Cart sp = lst.Find(n => n.iMaSP == ms);

            if (sp == null)
            {
                int soLuong;
                if (int.TryParse(f["iSoLuong"], out soLuong))
                {
                    sp = new Cart(ms, soLuong);
                    lst.Add(sp);
                }
                else
                {
                    // Xử lý lỗi nếu giá trị không hợp lệ
                    // Ví dụ: Đặt giá trị mặc định hoặc thông báo lỗi cho người dùng
                    ModelState.AddModelError("iSoLuong", "Giá trị không hợp lệ");
                    return View(); // hoặc chuyển hướng đến một trang lỗi khác
                }
            }
            else
            {
                sp.iSoLuong++;
            }
            return Redirect(url);
        }

        private int TongSoLuong()
        {
            int iTongSL = 0;
            List<Cart> lst = Session["Cart"] as List<Cart>;
            if (lst != null)
            {
                iTongSL = lst.Sum(n => n.iSoLuong);
            }
            return iTongSL;
        }
        private double TongTien()
        {
            double dTongTien = 0;
            List<Cart> lst = Session["Cart"] as List<Cart>;
            if (lst != null)
            {
                dTongTien = lst.Sum(n => n.dThanhTien);
            }
            return dTongTien;
        }

        public ActionResult GioHang()
        {
            List<Cart> lst = LayGioHang();
            if (lst.Count == 0)
            {
                ViewBag.tbGioHang = "Chưa có sản phẩm nào trong giỏ hàng";
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            return View(lst);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        public ActionResult XoaGioHang(int iMaSP, string url)
        {
            List<Cart> lst = LayGioHang();
            Cart sp = lst.SingleOrDefault(n => n.iMaSP == iMaSP);
            if (sp != null)
            {
                lst.RemoveAll(n => n.iMaSP == iMaSP);
                if (lst.Count == 0)
                {
                    ViewBag.tbGioHang = "Chưa có sản phẩm nào trong giỏ hàng";
                }
            }
            return Redirect(url);
        }
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            List<Cart> lst = LayGioHang();
            Cart sp = lst.SingleOrDefault(n => n.iMaSP == iMaSP);
            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtsl"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapNhatGioHang_dathang(int iMaSP, FormCollection f)
        {
            List<Cart> lst = LayGioHang();
            Cart sp = lst.SingleOrDefault(n => n.iMaSP == iMaSP);
            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtsl"].ToString());
            }
            return RedirectToAction("DatHang");
        }
        public ActionResult XoaAllGioHang()
        {
            List<Cart> lst = LayGioHang();
            lst.Clear();
            return RedirectToAction("GioHang", "Cart");
        }
        // phần đặt hàng
        public ActionResult DatHang()
        {
            if (Session["email"] == null || Session["email"].ToString() == "")
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["Cart"] == null)
            {
                return View();
            }
            List<Cart> lst = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            return View(lst);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f, infAddress inf)
        {

            inf.fullName = f["fullName"];
            inf.phoneNumber = f["phoneNumber"];
            inf.note = f["note"];
            Session["FormData"] = inf;
            DonHang ddh = new DonHang();
            string username = Session["email"] as string;
            // var khach = db.KhachHangs.FirstOrDefault(kh => kh.Username == username);
            KhachHang khach = db.KhachHangs.FirstOrDefault(kh => kh.Email == username);

            if (khach != null)
            {
                int maKhachHang = khach.MaKH;
                List<Cart> lst = LayGioHang();
                ddh.MaKH = maKhachHang;
                DateTime aDateTime = DateTime.Now;
                ddh.NgayMua = aDateTime;
                ddh.NgayGiao = aDateTime.AddDays(7);
                ddh.TinhTrangDonHang = 3;
                ddh.TongSP = TongSoLuong();
                ddh.TongTien = Convert.ToInt32(TongTien());
                ddh.DaThanhToan = false;
                db.DonHangs.InsertOnSubmit(ddh);
                db.SubmitChanges();
                foreach (var i in lst)
                {
                    ChiTietDonHang ctdh = new ChiTietDonHang();
                    ctdh.MaHD = ddh.MaHD;
                    ctdh.MaSP = i.iMaSP;
                    ctdh.MaKH = maKhachHang;
                    ctdh.SoLuong = i.iSoLuong;
                    ctdh.ThanhTien = (int)i.dDonGia;
                    db.ChiTietDonHangs.InsertOnSubmit(ctdh);
                    SanPham sanPham = db.SanPhams.FirstOrDefault(sp => sp.MaSP == i.iMaSP);

                    if (sanPham != null)
                    {
                        sanPham.SoLuongTon--;
                        sanPham.SoLuongBan++;
                        db.SubmitChanges();
                    }
                }
                GuiEmailXacNhanDonHang(ddh, khach, lst);
                db.SubmitChanges();
                Session["Cart"] = null;
                return RedirectToAction("XacNhanGioHang", "Cart");
            }
            else
            {
                ViewBag.tbGioHang = "Thông báo: Đơn hàng của bạn không thành công!, vui lòng kiểm tra  thông tin.";
                List<Cart> lstGioHang = LayGioHang();
                ViewBag.TongSoLuong = TongSoLuong();
                ViewBag.TongTien = TongTien();
                return View(lstGioHang);
            }
        }
        public ActionResult XacNhanGioHang()
        {

            return View();
        }
        // thanh toán online
        private double TongTienUSD()
        {
            double dTongTien = 0;
            List<Cart> lst = Session["Cart"] as List<Cart>;
            if (lst != null)
            {
                dTongTien = lst.Sum(n => Math.Round(n.dDonGia / 24000, 2) * n.iSoLuong);
            }
            return dTongTien;
        }

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Cart/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            // thêm vào database
            DonHang ddh = new DonHang();
            string username = Session["email"] as string;
            // var khach = db.KhachHangs.FirstOrDefault(kh => kh.Username == username);
            KhachHang khach = db.KhachHangs.FirstOrDefault(kh => kh.Email == username);

            if (khach != null)
            {
                int maKhachHang = khach.MaKH;
                List<Cart> lst = LayGioHang();
                ddh.MaKH = maKhachHang;
                DateTime aDateTime = DateTime.Now;
                ddh.NgayMua = aDateTime;
                ddh.NgayGiao = aDateTime.AddDays(7);
                ddh.TinhTrangDonHang = 3;
                ddh.TongSP = TongSoLuong();
                ddh.TongTien = Convert.ToInt32(TongTien());
                ddh.DaThanhToan = true;
                db.DonHangs.InsertOnSubmit(ddh);
                db.SubmitChanges();
                foreach (var i in lst)
                {
                    ChiTietDonHang ctdh = new ChiTietDonHang();
                    ctdh.MaHD = ddh.MaHD;
                    ctdh.MaSP = i.iMaSP;
                    ctdh.MaKH = maKhachHang;
                    ctdh.SoLuong = i.iSoLuong;
                    ctdh.ThanhTien = (int)i.dDonGia;
                    db.ChiTietDonHangs.InsertOnSubmit(ctdh);
                    SanPham sanPham = db.SanPhams.FirstOrDefault(sp => sp.MaSP == i.iMaSP);

                    if (sanPham != null)
                    {
                        sanPham.SoLuongTon--;
                        sanPham.SoLuongBan++;
                        db.SubmitChanges();
                    }
                }
                GuiEmailXacNhanDonHang(ddh, khach, lst);
                db.SubmitChanges();
                Session["Cart"] = null;
                return RedirectToAction("XacNhanGioHang", "Cart");
            }
            else
            {
                ViewBag.tbGioHang = "Thông báo: Đơn hàng của bạn không thành công!, vui lòng kiểm tra  thông tin.";
                return RedirectToAction("GioHang", new { MaDH = (int)Session["MaHD"] });
            }
            //on successful payment, show success page to user.  

        }


        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var lst = Session["Cart"] as List<Cart>;
            var tonggia = TongTienUSD().ToString();

            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            if (lst != null)
            {
                //create itemlist and add item objects to it  

                foreach (var i in lst)
                {
                    itemList.items.Add(new Item()
                    {
                        name = i.sTenSP,
                        currency = "USD",
                        price = Math.Round(i.dDonGia / 24000, 2).ToString(),
                        quantity = i.iSoLuong.ToString(),
                        sku = "sku",
                    });
                }

            }
            //Adding Item Details like name, currency, price etc  

            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                // thuế, vận chuyển, tổng phụ
                tax = "0",
                shipping = "0",
                subtotal = tonggia
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = tonggia,
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        private void GuiEmailXacNhanDonHang(DonHang ddh, KhachHang kh, List<Cart> lstGioHang)
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

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUsername),
                Subject = "Xác nhận đơn hàng",
                IsBodyHtml = true
            };
            mailMessage.To.Add(kh.Email);
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

            body += "<h1>Xác nhận đơn hàng</h1>";
            body += $"<p>Chào {kh.HoTen},</p>";
            body += "<p>Đơn hàng của bạn đã được đặt thành công.</p>";
            body += "<h2>Chi tiết đơn hàng:</h2>";

            // Thêm thông tin chi tiết đơn hàng vào email
            body += "<table>";
            body += "<tr><th>Sản phẩm</th><th>Số lượng</th><th>Đơn giá</th></tr>";
            foreach (var item in lstGioHang)
            {
                body += $"<tr><td>{item.sTenSP}</td><td>{item.iSoLuong}</td><td>{item.dDonGia:N0} VNĐ</td></tr>";

            }
            body += "</table>";

            body += $"<p><strong>Tổng tiền: {TongTien():N0} VNĐ</strong></p>";
            body += "<p>Cảm ơn bạn đã mua hàng tại cửa hàng chúng tôi!</p>";
            body += "</body></html>";
            mailMessage.Body = body;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {

            }
        }

    }
}