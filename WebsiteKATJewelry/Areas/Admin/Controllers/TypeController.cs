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
    public class TypeController : Controller
    {
        DataJewelryDataContext db = new DataJewelryDataContext();
        // GET: Admin/Type
        [HttpGet]
        public ActionResult Index(int? size, int? page, string sortProperty, string searchString, string sortOrder = "")
        {
            ViewBag.Keyword = searchString;

            var t = from c in db.LoaiSPs select c;

            //1.3. Tìm kiếm theo searchString
            if (!String.IsNullOrEmpty(searchString))
                t = t.Where(b => b.TenLoai.Contains(searchString));

            // 2 Đoạn code này dùng để sắp xếp
            // 2.1. Tạo biến ViewBag gồm sortOrder, searchValue, sortProperty và page
            if (sortOrder == "asc") ViewBag.SortOrder = "desc";
            if (sortOrder == "desc") ViewBag.SortOrder = "";
            if (sortOrder == "") ViewBag.SortOrder = "asc";
            // 2.1. Tạo thuộc tính sắp xếp mặc định là "MaLoai"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "MaLoai";

            // 2.2. Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc")
                t = t.OrderBy($"{sortProperty} descending"); // Sử dụng đúng cú pháp cho Dynamic LINQ
            else
                t = t.OrderBy(sortProperty);

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
            int checkTotal = (int)(t.ToList().Count / pageSize) + 1;
            // Nếu trang vượt qua tổng số trang thì thiết lập là 1 hoặc tổng số trang
            if (pageNumber > checkTotal) pageNumber = checkTotal;

            // 4. Trả kết quả về Views
            return View(t.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaDM = new SelectList(db.DanhMucs.ToList().OrderBy(n => n.TenDM), "MaDM", "TenDM");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(LoaiSP type, FormCollection f)
        {
            ViewBag.MaDM = new SelectList(db.DanhMucs.ToList().OrderBy(n => n.TenDM), "MaDM", "TenDM", int.Parse(f["MaDM"]));
            if (ModelState.IsValid)
            {
                type.TenLoai = f["TenLoai"];
                type.MaDM = int.Parse(f["MaDM"]);
                db.LoaiSPs.InsertOnSubmit(type);
                db.SubmitChanges();
                TempData["SuccessMessage"] = "Loại đã được thêm thành công.";
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Details(int id)
        {
            var type = db.LoaiSPs.SingleOrDefault(n => n.MaLoai == id);
            if (type == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(type);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {

            ViewBag.MaDM = new SelectList(db.DanhMucs.ToList().OrderBy(n => n.TenDM), "MaDM", "TenDM");
            var type = db.LoaiSPs.SingleOrDefault(n => n.MaLoai == id);
            if (type == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(type);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f)
        {
            var type = db.LoaiSPs.SingleOrDefault(n => n.MaLoai == int.Parse(f["MaLoai"]));
            ViewBag.MaDM = new SelectList(db.DanhMucs.ToList().OrderBy(n => n.TenDM), "MaDM", "TenDM", int.Parse(f["MaDM"]));
            if (ModelState.IsValid)
            {
                type.TenLoai = f["TenLoai"];
                type.MaDM = int.Parse(f["MaDM"]);
                db.SubmitChanges();
                TempData["SuccessMessage"] = "Cập nhật thành công.";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var type = db.LoaiSPs.SingleOrDefault(n => n.MaLoai == id);
            if (type == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(type);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var type = db.LoaiSPs.SingleOrDefault(n => n.MaLoai == id);
            if (type == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var proc = db.SanPhams.Where(ct => ct.MaLoai == id);
            var s = db.LoaiSPs.Where(bl => bl.MaDM == id);
            if (proc.Count() > 0)
            {
                ViewBag.ThongBao = "Loại này đang có trong bảng Sản phẩm <br>" +
                    "Nếu muốn xóa thì phải xóa hết sản phẩm thuộc loại này trong bảng Sản phẩm";
                return View(type);
            }

            db.LoaiSPs.DeleteOnSubmit(type);
            db.SubmitChanges();
            TempData["SuccessMessage"] = "Đã xóa thành công.";
            return RedirectToAction("Index");
        }
    }
}