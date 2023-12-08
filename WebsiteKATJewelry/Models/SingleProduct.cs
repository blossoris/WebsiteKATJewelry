using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;

namespace WebsiteKATJewelry.Models
{
    public class SingleProduct
    {
        public SanPham SanPham { get; set; } = new SanPham();
        public DanhMuc DanhMuc { get; set; }
        public LoaiSP LoaiSP { get; set; }
        public List<BinhLuan> BinhLuans { get; set; }
        public List<AnhSP> anhSPs { get; set; }
        public int ProductId { get; set; }
        public bool HasCommented { get; set; }
    }
}