using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sem3.Models;

namespace Sem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly prosem3Context _context;

        public ProductsController(prosem3Context context)
        {
            _context = context;
        }

        // GET: Admin/Products
        //public async Task<IActionResult> Index()
        //{
        //    var prosem3Context = _context.Products.Include(p => p.Category);
        //    return View(await prosem3Context.ToListAsync());
        //}
        public async Task<IActionResult> Index(int? page)
        {
            // Số lượng sản phẩm trên mỗi trang
            int pageSize = 3;
            // Trang hiện tại (mặc định là 1)
            int pageNumber = (page ?? 1);

            // Lấy danh sách sản phẩm và include Category
            var prosem3Context = _context.Products.Include(p => p.Category);

            // Phân trang với Skip và Take
            var pagedProducts = await prosem3Context
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Tính tổng số lượng trang
            int totalProducts = await prosem3Context.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(pagedProducts);
        }



        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ProductId,ProductName,Description,Price,Image,CategoryId")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
        //    return View(product);
        //}
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Description,Price,CategoryId")] Product product, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                // Xử lý ảnh
                if (Image != null && Image.Length > 0)
                {
                    // Đặt tên file ảnh bằng tên file gốc
                    var fileName = Path.GetFileName(Image.FileName);
                    // Đường dẫn lưu ảnh vào thư mục wwwroot/images
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    // Lưu ảnh vào đường dẫn
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    // Gán đường dẫn ảnh vào thuộc tính Image của sản phẩm
                    product.Image = "/images/" + fileName;
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, hiển thị lại form
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);

        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(string id, [Bind("ProductId,ProductName,Description,Price,CategoryId")] Product product, IFormFile Image)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra nếu có ảnh mới được upload
                    if (Image != null && Image.Length > 0)
                    {
                        // Tạo đường dẫn và lưu file ảnh mới vào wwwroot/images
                        var fileName = Path.GetFileName(Image.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        // Lưu ảnh vào thư mục
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }

                        // Cập nhật đường dẫn ảnh mới cho sản phẩm
                        product.Image = "/images/" + fileName;
                    }

                    // Cập nhật sản phẩm vào cơ sở dữ liệu
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // Trả về dropdown danh mục nếu có lỗi
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Kiểm tra nếu _context.Products null
            if (_context.Products == null)
            {
                return Problem("Entity set 'prosem3Context.Products' is null.");
            }

            // Kiểm tra nếu id là null hoặc không hợp lệ
            if (string.IsNullOrEmpty(id))
            {
                return NotFound(); // Nếu không có id, trả về NotFound
            }

            // Tìm sản phẩm dựa trên ProductId
            var product = await _context.Products.FindAsync(id);

            // Kiểm tra nếu sản phẩm tồn tại trong cơ sở dữ liệu
            if (product == null)
            {
                return NotFound(); // Nếu sản phẩm không tồn tại, trả về NotFound
            }

            // Xóa sản phẩm nếu tồn tại
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Chuyển hướng về trang Index sau khi xóa
            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(string id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
