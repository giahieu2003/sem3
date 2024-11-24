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
    public class CategoriesController : Controller
    {
        private readonly prosem3Context _context;

        public CategoriesController(prosem3Context context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index(int? page)
        {
            // Số lượng bản ghi hiển thị trên mỗi trang
            int pageSize = 10;
            // Trang hiện tại (nếu không có giá trị thì mặc định là 1)
            int pageNumber = (page ?? 1);

            // Kiểm tra nếu bảng Categories tồn tại
            if (_context.Categories != null)
            {
                // Lấy danh sách Category theo phân trang
                var categories = await _context.Categories
                                               .OrderBy(c => c.CategoryId)
                                               .Skip((pageNumber - 1) * pageSize) // Bỏ qua các bản ghi trước đó dựa trên trang hiện tại
                                               .Take(pageSize)                     // Lấy số lượng bản ghi tương ứng với pageSize
                                               .ToListAsync();

                // Tính tổng số bản ghi để tính tổng số trang
                int totalCategories = await _context.Categories.CountAsync();
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalCategories / pageSize);
                ViewBag.CurrentPage = pageNumber;

                // Trả về view với dữ liệu phân trang
                return View(categories);
            }
            else
            {
                // Trả về thông báo lỗi nếu Categories là null
                return Problem("Entity set 'prosem3Context.Categories' is null.");
            }
        }

        //public async Task<IActionResult> Index()
        //{
        //      return _context.Categories != null ? 
        //                  View(await _context.Categories.ToListAsync()) :
        //                  Problem("Entity set 'prosem3Context.Categories'  is null.");
        //}

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Categories == null)
            {
                TempData["ErrorMessage"] = "Category data not found.";
                return RedirectToAction(nameof(Index));
            }

            var category = await _context.Categories
                .Include(c => c.Products) // Include danh sách sản phẩm liên quan
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra nếu có sản phẩm liên quan
            if (category.Products != null && category.Products.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete this category because it has associated products.";
                return RedirectToAction(nameof(Index));
            }

            // Xóa danh mục
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }



        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
