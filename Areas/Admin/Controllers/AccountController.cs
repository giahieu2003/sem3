using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem3.Models;
using System.Security.Claims;

namespace Sem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly prosem3Context _context;

        public AccountController(prosem3Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            if (User.Identity.IsAuthenticated)
            {
                // Nếu đã đăng nhập, chuyển hướng đến trang Home của Admin
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Kiểm tra xem có Employee nào có Email và Password phù hợp không
            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == email && e.Password == password);

            if (employee != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, employee.Username),
            new Claim(ClaimTypes.Email, employee.Email),
            new Claim(ClaimTypes.Role, employee.Role)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Sử dụng TempData để truyền thông báo thành công
                TempData["SuccessMessage"] = "Login successful!";

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            // Nếu đăng nhập không thành công, truyền thông báo lỗi qua ViewBag
            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }


        // GET: /Account/Register
        public IActionResult Register()
        {
            ViewBag.RoleList = new List<string> { "Admin", "Employee" };
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(Employee model)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // Logout action
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
