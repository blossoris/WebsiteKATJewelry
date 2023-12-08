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
    public class CategoryController : Controller
    {
        DataJewelryDataContext db = new DataJewelryDataContext();
        // GET: Admin/Category
        [HttpGet]
        public ActionResult Index(int? size, int? page, string sortProperty, string searchString, string sortOrder = "")
        {
            ViewBag.Keyword = searchString;

            var cate = from c in db.DanhMucs select c;

            //1.3. Tìm kiếm theo searchString
            if (!String.IsNullOrEmpty(searchString))
                cate = cate.Where(b => b.TenDM.Contains(searchString));

            // 2 Đoạn code này dùng để sắp xếp
            // 2.1. Tạo biến ViewBag gồm sortOrder, searchValue, sortProperty và page
            if (sortOrder == "asc") ViewBag.SortOrder = "desc";
            if (sortOrder == "desc") ViewBag.SortOrder = "";
            if (sortOrder == "") ViewBag.SortOrder = "asc";
            // 2.1. Tạo thuộc tính sắp xếp mặc định là "MaDM"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "MaDM";

            // 2.2. Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc")
                cate = cate.OrderBy($"{sortProperty} descending"); // Sử dụng đúng cú pháp cho Dynamic LINQ
            else
                cate = cate.OrderBy(sortProperty);

            // 3 Đoạn code sau dùng để phân trang
            ViewBag.Page = page;

            // 3.1. Tạo danh sách chọn số trang
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });

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
            int checkTotal = (int)(cate.ToList().Count / pageSize) + 1;
            // Nếu trang vượt qua tổng số trang thì thiết lập là 1 hoặc tổng số trang
            if (pageNumber > checkTotal) pageNumber = checkTotal;

            // 4. Trả kết quả về Views
            return View(cate.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(DanhMuc cate, FormCollection f)
        {
            if (ModelState.IsValid)
            {
                cate.TenDM = f["TenDM"];
                db.DanhMucs.InsertOnSubmit(cate);
                db.SubmitChanges();
                TempData["SuccessMessage"] = "Danh mục đã được thêm thành công.";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var cate = db.DanhMucs.SingleOrDefault(n => n.MaDM == id);
            if (cate == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(cate);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var cate = db.DanhMucs.SingleOrDefault(n => n.MaDM == id);
            if (cate == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var proc = db.SanPhams.Where(ct => ct.MaDM == id);
            var type = db.LoaiSPs.Where(bl => bl.MaDM == id);
            if (proc.Count() > 0)
            {
                ViewBag.ThongBao = "Danh mục này đang có trong bảng Sản phẩm <br>" +
                    "Nếu muốn xóa thì phải xóa hết sản phẩm thuộc danh mục này trong bảng Sản phẩm";
                return View(cate);
            }
            if (type.Count() > 0)
            {
                ViewBag.ThongBao = "Danh mục này đang có trong bảng Loại sản phẩm <br>" +
                    "Nếu muốn xóa thì phải xóa hết loại thuộc danh mục này trong bảng Loại sản phẩm";
                return View(cate);
            }
            db.DanhMucs.DeleteOnSubmit(cate);
            db.SubmitChanges();
            TempData["SuccessMessage"] = "Danh mục đã được xóa thành công.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var cate = db.DanhMucs.SingleOrDefault(n => n.MaDM == id);
            if (cate == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(cate);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f)
        {
            var cate = db.DanhMucs.SingleOrDefault(n => n.MaDM == int.Parse(f["MaDM"]));
            if (cate == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            if (ModelState.IsValid)
            {
                cate.TenDM = f["TenDM"];
                db.SubmitChanges();
                TempData["SuccessMessage"] = "Danh mục đã được cập nhật thành công.";
                return RedirectToAction("Index");
            }
            return View(cate);
        }

        public ActionResult Details(int id)
        {
            var cate = db.DanhMucs.SingleOrDefault(n => n.MaDM == id);
            if (cate == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(cate);
        }
    }
}