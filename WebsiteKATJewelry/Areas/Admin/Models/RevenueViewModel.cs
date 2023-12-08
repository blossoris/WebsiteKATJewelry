using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteKATJewelry.Areas.Admin.Models
{
    public class RevenueViewModel
    {
        public int? Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }

    }
}