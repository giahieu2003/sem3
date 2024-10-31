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
        public IActionResult Shop()
        {
            return View(_context.Products.ToList());
        }
        [Route("/Cart")]
        public IActionResult Cart()
        {
            return View();
        }
        [Route("/ProductDetail/{id}")]
        public async Task<IActionResult> ProductDetail(string id)
        {
            var product = await _context.Products
                .Include(p => p.Category) // Kết hợp thông tin danh mục
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm
            }

            return View(product); // Truyền thông tin sản phẩm cho View
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/Profile")]
        public IActionResult Profile()
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
