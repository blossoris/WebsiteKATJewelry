using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteKATJewelry.Models
{
    public class Product
    {
        public int MaSP { get; set; }
        [Display(Name = "Tên sản phẩm")]
        public string TenSP { get; set; }
        [Display(Name = "Loại")]
        public int MaLoai { get; set; }
        [Display(Name = "Danh mục")]
        public int MaDM { get; set; }
        [Display(Name = "Mô tả")]
        public string Mota { get; set; }
        [Display(Name = "Ảnh sản phẩm")]
        public string AnhBia { get; set; }
        [Display(Name = "Giá")]
        public int GiaSP { get; set; }
        [Display(Name = "Ngày nhập")]
        public DateTime NgayNhap { get; set; }
        [Display(Name = "Số lượng bán")]
        public int SoLuongBan { get; set; }
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }
        [Display(Name = "Story")]
        public string Story { get; set; }
        public LoaiSP LoaiSP { get; set; }
        public DanhMuc DanhMuc { get; set; }
        public List<string> DanhSachAnh { get; set; }
    }
}