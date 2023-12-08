using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;

namespace WebsiteKATJewelry.Models
{
    public class ProCom
    {
        public SingleProduct SingleProduct { get; set; }
        public IPagedList<BinhLuan> Comments { get; set; }
    }
}