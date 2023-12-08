using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using WebsiteKATJewelry.Models;
using System.Data.Entity;
using System.Linq.Dynamic.Core;
namespace WebsiteKATJewelry.Areas.Admin.Models
{
    public class Bill
    {
        public DonHang DonHang { get; set; }
        public List<ChiTietDonHang> chiTietDonHangs { get; set; }
    }
}