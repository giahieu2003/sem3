using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem3.Models;
using System.Diagnostics;

namespace Sem3.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        private readonly prosem3Context _context;

        public HomeController(prosem3Context context)
        {
            _context = context;
        }

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            return View(_context.Products.Take(8).ToList());
        }
        [Route("/Shop")]
        public IActionResult Shop(int? categoryId, string searchTerm)
        {
            
            ViewBag.Categories = _context.Categories.ToList();

            // Lọc sản phẩm theo danh mục và tìm kiếm
            var products = _context.Products.AsQueryable();

            // Lọc theo categoryId nếu có
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            // Lọc theo từ khóa tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.ProductName.Contains(searchTerm));
            }

            return View(products.ToList());
        }





        //[Route("/Cart")]
        //public IActionResult Cart()
        //{
        //    return View();
        //}
        [Route("/ProductDetail/{id}")]
        public async Task<IActionResult> ProductDetail(string id)
        {
            var product = await _context.Products
                .Include(p => p.Category) // Kết hợp thông tin danh mục
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(); 
            }

            return View(product); 
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
