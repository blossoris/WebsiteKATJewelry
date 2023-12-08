using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteKATJewelry.Models;

namespace WebsiteKATJewelry.Areas.Admin.Models
{
    public class Details
    {
        public SanPham SanPham { get; set; }
        public List<AnhSP> AnhSPs { get; set; }
    }
}