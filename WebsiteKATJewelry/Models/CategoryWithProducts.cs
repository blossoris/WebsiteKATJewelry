using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteKATJewelry.Models
{
    public class CategoryWithProducts
    {
        public List<DanhMuc> Categories { get; set; }
        public IPagedList<SanPham> Products { get; set; }

    }
}