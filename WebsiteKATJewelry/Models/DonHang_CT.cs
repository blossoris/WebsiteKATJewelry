using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteKATJewelry.Models;
namespace WebsiteKATJewelry.Models
{
    public class DonHang_CT
    {
        DataJewelryDataContext db = new DataJewelryDataContext();
        public int MaHD { get; set; }
        public int MaSP { get; set; }
        public List<string> TenSanPhams { get; set; }
        public int TongTien { get; set; }
        public DateTime NgayMua { get; set; }
        public int TinhTrangDonHang { get; set; }
        public bool DaThanhToan { get; set; }
    }
}