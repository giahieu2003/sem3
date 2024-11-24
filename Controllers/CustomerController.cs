using Microsoft.AspNetCore.Mvc;
using Sem3.Models;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sem3.Controllers
{
    public class CustomerController : Controller
    {
        private readonly prosem3Context _context;

        public CustomerController(prosem3Context context)
        {
            _context = context;
        }

        // Đăng ký tài khoản mới
        [Route("/register")]
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View(); // Trả về view "Login"
        }
        [Route("/register")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(Customer customer)
        {
            // Kiểm tra sự tồn tại của username không đồng bộ
            bool usernameExists = await _context.Customers.AnyAsync(c => c.Username == customer.Username);
            if (usernameExists)
            {
                return BadRequest("Username đã tồn tại.");
            }

            // Kiểm tra sự tồn tại của email không đồng bộ
            bool emailExists = await _context.Customers.AnyAsync(c => c.Email == customer.Email);
            if (emailExists)
            {
                return BadRequest("Email đã được sử dụng.");
            }

            // Không mã hóa mật khẩu
            customer.RegistrationDate = DateTime.Now;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Lưu thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";

            // Chuyển hướng đến trang login
            return RedirectToAction("Login");
        }

        // Đăng nhập
        [Route("/login")]
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View(); // Trả về view "Login"
        }
        [Route("/login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null || customer.Password != password)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                return RedirectToAction("Login");
            }

            // Lưu trạng thái đăng nhập vào Session
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
            TempData["SuccessMessage"] = "Đăng nhập thành công.";
            return RedirectToAction("Index", "Home");
        }


        //public async Task<IActionResult> Login(string username, string password)
        //{
        //    var customer = await _context.Customers
        //        .FirstOrDefaultAsync(c => c.Username == username);

        //    if (customer == null || customer.Password != password)
        //    {
        //        TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không chính xác.";
        //        return RedirectToAction("Login"); // Giả sử bạn có action Login trong controller
        //    }

        //    // Nếu đăng nhập thành công
        //    TempData["SuccessMessage"] = "Đăng nhập thành công.";
        //    return RedirectToAction("Index", "Home");
        //}

        // Lấy thông tin tài khoản
        [Route("/profile")]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            // Kiểm tra session CustomerId
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem trang cá nhân.";
                return RedirectToAction("Login");
            }

            // Lấy thông tin người dùng dựa vào customerId từ session
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound("Tài khoản không tồn tại.");
            }

            // Trả về view Profile với dữ liệu của người dùng
            return View("Profile", customer);
        }

        [Route("/logout")]
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            // Xóa thông tin CustomerId khỏi session
            HttpContext.Session.Remove("CustomerId");

            // Hiển thị thông báo đăng xuất thành công (tuỳ chọn)
            TempData["SuccessMessage"] = "Đăng xuất thành công.";

            // Chuyển hướng đến trang chủ hoặc trang đăng nhập
            return RedirectToAction("Index", "Home");
        }


    }
}
