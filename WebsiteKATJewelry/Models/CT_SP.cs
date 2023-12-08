using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteKATJewelry.Models
{
    public class CT
    {
        public int MaHD { get; set; }
        public int TongTien { get; set; }
    }
    public class SP
    {
        public string TenSP { get; set; }
    }
    public class DH
    {
        public DateTime NgayMua { get; set; }
    }
    public class CT_SP
    { 
            public int MaHD { get; set; }
            public DateTime NgayMua { get; set; }
            public int TongTien { get; set; }
            public string TenSP { get; set; }
        
    }

}