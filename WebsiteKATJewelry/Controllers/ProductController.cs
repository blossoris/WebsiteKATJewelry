using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Data.Entity;
using System.Linq.Dynamic.Core;
using WebsiteKATJewelry.Models;

namespace WebsiteKATJewelry.Controllers
{
    public class ProductController : Controller
    {
        DataJewelryDataContext db = new DataJewelryDataContext();

        // GET: Product
        public ActionResult Index(int? size, int? page, string sortProperty, string searchString, string sortOrder = "", int categoryID = 0, int? priceRange = null)
        {
            // 1. Đoạn code dùng để tìm kiếm
            // 1.1. Lưu tư khóa tìm kiếm
            ViewBag.Keyword = searchString;
            //1.2 Lưu chủ đề tìm kiếm
            ViewBag.Subject = categoryID;
            //1.2.Tạo câu truy vấn kết 3 bảng
            var proc = db.SanPhams.Include(b => b.DanhMuc).Include(b => b.LoaiSP);

            //1.3. Tìm kiếm theo searchString
            if (!String.IsNullOrEmpty(searchString))
                proc = proc.Where(b => b.TenSP.Contains(searchString));

            //1.4. Tìm kiếm theo CategoryID
            if (!String.IsNullOrEmpty(searchString))
                proc = proc.Where(b => b.TenSP.Contains(searchString));

            //1.5. Tìm kiếm theo CategoryID
            if (categoryID != 0)
                proc = proc.Where(c => c.MaDM == categoryID);

            //1.6. Tìm kiếm theo Danh sách danh mục
            ViewBag.CategoryID = new SelectList(db.DanhMucs, "MaDM", "TenDM"); // danh sách Category               
                                                                                
            //1.7. Lọc theo giá
            if (priceRange != 0)
            {
                if (priceRange.HasValue)
                {
                    proc = proc.Where(p => p.GiaSP <= priceRange);
                }
            }
               
            ViewBag.PriceRange = priceRange ?? 0;

            // 2 Đoạn code này dùng để sắp xếp
            // 2.1. Tạo biến ViewBag gồm sortOrder, searchValue, sortProperty và page
            if (sortOrder == "asc") ViewBag.SortOrder = "desc";
            if (sortOrder == "desc") ViewBag.SortOrder = "";
            if (sortOrder == "") ViewBag.SortOrder = "asc";
            // 2.1. Tạo thuộc tính sắp xếp mặc định là "Title"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "TenSP";

            // 2.2. Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc")
                proc = proc.OrderBy(sortProperty + " desc");
            else
                proc = proc.OrderBy(sortProperty);

            // 3 Đoạn code sau dùng để phân trang
            ViewBag.Page = page;

            // 3.1. Tạo danh sách chọn số trang
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "6", Value = "6" });
            items.Add(new SelectListItem { Text = "12", Value = "12" });
            items.Add(new SelectListItem { Text = "24", Value = "24" });
            items.Add(new SelectListItem { Text = "48", Value = "48" });
            items.Add(new SelectListItem { Text = "60", Value = "60" });
            items.Add(new SelectListItem { Text = "180", Value = "180" });

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
            int pageSize = (size ?? 6);

            ViewBag.pageSize = pageSize;

            // 3.5. Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);

            // 3.6 Lấy tổng số record chia cho kích thước để biết bao nhiêu trang
            int checkTotal = (int)(proc.ToList().Count / pageSize) + 1;
            // Nếu trang vượt qua tổng số trang thì thiết lập là 1 hoặc tổng số trang
            if (pageNumber > checkTotal) pageNumber = checkTotal;

            // 4. Trả kết quả về Views
            return View(proc.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ProductByCategory(int categoryID, int? size, int? page, string sortProperty, string searchString, string sortOrder = "", int subCategories = 0, int? priceRange = null)
        {
            ViewBag.Keyword = searchString;
            ViewBag.Subject = subCategories;
            ViewBag.id = categoryID;
            var procs = db.SanPhams.Include(b => b.LoaiSP).Where(c => c.MaDM == categoryID);

            //1.3. Tìm kiếm theo searchString
            if (!String.IsNullOrEmpty(searchString))
                procs = procs.Where(b => b.TenSP.Contains(searchString));

            var subCategoriesList = db.LoaiSPs.Where(d => d.MaDM == categoryID);
            if (subCategories != 0)
                procs = procs.Where(c => c.MaLoai == subCategories);

            ViewBag.subCategories = new SelectList(subCategoriesList, "MaLoai", "TenLoai"); // danh sách Loại               
           
            //1.7. Lọc theo giá
            if (priceRange.HasValue)
            {
                procs = procs.Where(p => p.GiaSP <= priceRange);
            }
            ViewBag.PriceRange = priceRange ?? 0;

            // 2 Đoạn code này dùng để sắp xếp
            // 2.1. Tạo biến ViewBag gồm sortOrder, searchValue, sortProperty và page
            if (sortOrder == "asc") ViewBag.SortOrder = "desc";
            if (sortOrder == "desc") ViewBag.SortOrder = "";
            if (sortOrder == "") ViewBag.SortOrder = "asc";
            // 2.1. Tạo thuộc tính sắp xếp mặc định là "Title"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "TenSP";

            // 2.2. Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc")
                procs = procs.OrderBy(sortProperty + " desc");
            else
                procs = procs.OrderBy(sortProperty);

            // 3 Đoạn code sau dùng để phân trang
            ViewBag.Page = page;

            // 3.1. Tạo danh sách chọn số trang
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "6", Value = "6" });
            items.Add(new SelectListItem { Text = "12", Value = "12" });
            items.Add(new SelectListItem { Text = "24", Value = "24" });
            items.Add(new SelectListItem { Text = "48", Value = "48" });
            items.Add(new SelectListItem { Text = "60", Value = "60" });
            items.Add(new SelectListItem { Text = "180", Value = "180" });

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
            int pageSize = (size ?? 6);

            ViewBag.pageSize = pageSize;

            // 3.5. Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);

            // 3.6 Lấy tổng số record chia cho kích thước để biết bao nhiêu trang
            int checkTotal = (int)(procs.ToList().Count / pageSize) + 1;
            // Nếu trang vượt qua tổng số trang thì thiết lập là 1 hoặc tổng số trang
            if (pageNumber > checkTotal) pageNumber = checkTotal;

            // 4. Trả kết quả về Views
            return View(procs.ToPagedList(pageNumber, pageSize));
        }

        public List<SingleProduct> GetSingleProduct()
        {
            List<SingleProduct> SingleProduct = Session["SingleProduct"] as List<SingleProduct>;
            if (SingleProduct == null)
            {
                SingleProduct = new List<SingleProduct>();
                Session["ProcductDetails"] = SingleProduct;
            }
            return SingleProduct;
        }
        public ActionResult SingleProduct(int id, int? page)
        {
            var product = db.SanPhams.FirstOrDefault(p => p.MaSP == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var danhmuc = db.DanhMucs.FirstOrDefault(dm => dm.MaDM == product.MaDM);
            var loaisp = db.LoaiSPs.FirstOrDefault(loai => loai.MaLoai == product.MaLoai);
            var binhLuans = db.BinhLuans.Where(bl => bl.MaSP == id).ToList();
            var anhsp = db.AnhSPs.Where(pic => pic.MaSP == id).ToList();
            // Lấy danh sách bình luận và phân trang
            int pageSize = 4; // Số lượng bình luận trên mỗi trang
            int pageNumber = (page ?? 1);
            var comments = db.BinhLuans.Where(c => c.MaSP == id).OrderByDescending(c => c.NgayBL).ToPagedList<WebsiteKATJewelry.Models.BinhLuan>(pageNumber, pageSize);            // Kiểm tra người dùng đã bình luận vào sản phẩm chưa
            string user = Session["email"] as string;
            bool hasComment = db.BinhLuans.Any(c => c.MaSP == id && c.Email == user);
            ViewBag.HasComment = hasComment;
            List<AnhSP> danhSachAnh = db.AnhSPs.Where(anh => anh.MaSP == id).ToList();
            ViewBag.DanhSachAnh = danhSachAnh;
            // Lưu thông tin sản phẩm vào Session để sử dụng cho phần bình luận
            Session["Product"] = product;
            var productdetails = new SingleProduct
            {
                SanPham = product,
                BinhLuans = binhLuans,
                anhSPs = anhsp,
                DanhMuc = danhmuc,
                LoaiSP = loaisp,
                ProductId = id,
            };
            return View(new ProCom
            {
                SingleProduct = productdetails,
                Comments = comments
            });
        }



        [HttpPost]
        public ActionResult AddComment(int productId, string comment)
        {
            string user = Session["email"] as string;
            string selectedRating = Request.Form["Rating"];

            // Tạo một bình luận mới
            var newComment = new BinhLuan
            {
                MaSP = productId,
                Email = user,
                NgayBL = DateTime.Now,
                Danhgia = selectedRating,
                Noidung = comment
            };

            db.BinhLuans.InsertOnSubmit(newComment);
            db.SubmitChanges();

            var commentedProducts = GetCommentedProducts();
            commentedProducts.Add(productId);
            return RedirectToAction("SingleProduct", new { id = productId });
        }

        private List<int> GetCommentedProducts()
        {
            List<int> commentedProducts = Session["CommentedProducts"] as List<int>;
            if (commentedProducts == null)
            {
                commentedProducts = new List<int>();
                Session["CommentedProducts"] = commentedProducts;
            }
            return commentedProducts;
        }


    }
}
