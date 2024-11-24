using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sem3.Models;

namespace Sem3.Controllers
{
    public class CartController : Controller
    {
        // Kiểm tra xem người dùng đã đăng nhập chưa
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("CustomerId") != null; // Thay "CustomerId" bằng tên ID thực tế bạn sử dụng
        }

        // Hiển thị giỏ hàng
        [Route("/cart")]
        public IActionResult Cart()
        {
            if (!IsUserLoggedIn())
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Login", "Customer"); // Chuyển đến trang đăng nhập
            }
            var cart = GetCartFromSession();
            List<Product> productsInCart = new List<Product>();

            using (var db = new prosem3Context())
            {
                foreach (var item in cart)
                {
                    var product = db.Products.Find(item.Key);
                    if (product != null)
                    {
                        ViewData[item.Key] = item.Value;
                        productsInCart.Add(product);
                    }
                }
            }

            return View(productsInCart);
        }

        // Thêm sản phẩm vào giỏ hàng với số lượng từ chi tiết sản phẩm
        [HttpPost]
        public IActionResult AddToCart(string productId, int quantity = 1)
        {
            if (!IsUserLoggedIn())
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Login", "Customer"); // Chuyển đến trang đăng nhập
            }
            var cart = GetCartFromSession();

            if (cart.ContainsKey(productId))
            {
                cart[productId] += quantity;
            }
            else
            {
                cart[productId] = quantity;
            }

            SaveCartToSession(cart);
            return RedirectToAction("Cart");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult RemoveFromCart(string productId)
        {
            var cart = GetCartFromSession();

            if (cart.ContainsKey(productId))
            {
                cart.Remove(productId);
            }

            SaveCartToSession(cart);
            return RedirectToAction("Cart");
        }

        // Xóa toàn bộ giỏ hàng
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Cart");
        }

        // Phương thức hỗ trợ lấy giỏ hàng từ Session
        private Dictionary<string, int> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            return cartJson != null
                ? JsonConvert.DeserializeObject<Dictionary<string, int>>(cartJson)
                : new Dictionary<string, int>();
        }

        // Phương thức hỗ trợ lưu giỏ hàng vào Session
        private void SaveCartToSession(Dictionary<string, int> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }
        [HttpPost]
        public ActionResult UpdateCart(Dictionary<string, int> quantity)
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            if (!IsUserLoggedIn())
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để cập nhật giỏ hàng.";
                return RedirectToAction("Login", "Customer"); // Chuyển đến trang đăng nhập
            }

            var cart = GetCartFromSession(); // Lấy giỏ hàng từ Session

            foreach (var item in quantity)
            {
                var productId = item.Key;
                var qty = item.Value;

                if (cart.ContainsKey(productId))
                {
                    if (qty > 0) // Cập nhật số lượng
                    {
                        cart[productId] = qty;
                    }
                    else // Nếu số lượng nhỏ hơn hoặc bằng 0, xóa sản phẩm khỏi giỏ hàng
                    {
                        cart.Remove(productId);
                    }
                }
            }

            SaveCartToSession(cart); // Lưu giỏ hàng đã cập nhật vào Session
            return RedirectToAction("Cart"); // Quay về trang giỏ hàng
        }



    }
}
