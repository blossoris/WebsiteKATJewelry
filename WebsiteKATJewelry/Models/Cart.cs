using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteKATJewelry.Models;
namespace WebsiteKATJewelry.Models
{
    public class Cart
    {
        DataJewelryDataContext db = new DataJewelryDataContext();
        public int iMaSP { get; set; }
        public string sTenSP { get; set; }
        public string sAnhBia { get; set; }
        public double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public DateTime Ngaytao { get; set; }
        public double dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        public Cart(int ms, int? iSoLuong)
        {
            iMaSP = ms;
            SanPham s = db.SanPhams.Single(n => n.MaSP == iMaSP);
            sTenSP = s.TenSP;
            sAnhBia = s.AnhBia;
            dDonGia = double.Parse(s.GiaSP.ToString());
            //iSoLuong = 1;
            if (iSoLuong.HasValue && iSoLuong > 0)
            {
                this.iSoLuong = iSoLuong.Value;
            }
            else
            {
                this.iSoLuong = 1;
            }

        }
    }
}