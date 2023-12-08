using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.ComponentModel;
using System.IO;
using WebsiteKATJewelry.Models;
namespace WebsiteKATJewelry.Models
{
    public class Email
    {
        [DisplayName("Người gửi: ")]
        public string From { get; set; }
        [DisplayName("Người nhận: ")]
        public string To { get; set; }
        [DisplayName("Tiêu đề: ")]
        public string Subject { get; set; }
        [DisplayName("Nội dung: ")]
        public string Notes { get; set; }
        [DisplayName("Họ tên: ")]
        public string Name { get; set; }

    }
}