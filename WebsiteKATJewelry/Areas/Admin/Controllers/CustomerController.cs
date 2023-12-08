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

namespace WebsiteKATJewelry.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        DataJewelryDataContext db = new DataJewelryDataContext();
        // GET: Admin/Customer
        [HttpGet]
        public ActionResult Index(int? size, int? page, string sortProperty, string searchString, string sortOrder = "")
        {
            ViewBag.Keyword = searchString;

            var cust = from c in db.KhachHangs select c;

            //1.3. Tìm kiếm theo searchString
            if (!String.IsNullOrEmpty(searchString))
                cust = cust.Where(b => b.HoTen.Contains(searchString));

            // 2 Đoạn code này dùng để sắp xếp
            // 2.1. Tạo biến ViewBag gồm sortOrder, searchValue, sortProperty và page
            if (sortOrder == "asc") ViewBag.SortOrder = "desc";
            if (sortOrder == "desc") ViewBag.SortOrder = "";
            if (sortOrder == "") ViewBag.SortOrder = "asc";
            // 2.1. Tạo thuộc tính sắp xếp mặc định là "idColor"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "HoTen";

            // 2.2. Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc")
                cust = cust.OrderBy($"{sortProperty} descending"); // Sử dụng đúng cú pháp cho Dynamic LINQ
            else
                cust = cust.OrderBy(sortProperty);

            // 3 Đoạn code sau dùng để phân trang
            ViewBag.Page = page;

            // 3.1. Tạo danh sách chọn số trang
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            items.Add(new SelectListItem { Text = "25", Value = "25" });
            items.Add(new SelectListItem { Text = "50", Value = "50" });
            items.Add(new SelectListItem { Text = "100", Value = "100" });
            items.Add(new SelectListItem { Text = "200", Value = "200" });

            // 3.2. Thiết lập số trang đang chọn vào danh sách List<SelectListItem> items
            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            ViewBag.Size = items;
            ViewBag.CurrentSize = size;
            // 3.3. Nếu page = null thì đặt lại là 1.
            page = page ?? 1; //if (page == null) page = 1;

            // 3.4. Tạo kích thước trang (pageSize), mặc định là 5.
            int pageSize = (size ?? 5);

            ViewBag.pageSize = pageSize;

            // 3.5. Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);

            // 3.6 Lấy tổng số record chia cho kích thước để biết bao nhiêu trang
            int checkTotal = (int)(cust.ToList().Count / pageSize) + 1;
            // Nếu trang vượt qua tổng số trang thì thiết lập là 1 hoặc tổng số trang
            if (pageNumber > checkTotal) pageNumber = checkTotal;

            // 4. Trả kết quả về Views
            return View(cust.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(int id)
        {
            var cust = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            if (cust == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(cust);
        }
    }
}