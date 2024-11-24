using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sem3.Models;

namespace Sem3.Controllers
{
    public class OrderController : Controller
    {
        private readonly prosem3Context _context;

        public OrderController(prosem3Context context)
        {
            _context = context;
        }

        // Lấy dữ liệu giỏ hàng từ Session
        private Dictionary<string, int> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            return cartJson != null
                ? JsonConvert.DeserializeObject<Dictionary<string, int>>(cartJson)
                : new Dictionary<string, int>();
        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
            var cart = GetCartFromSession();
            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Cart", "Cart");
            }

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["ErrorMessage"] = "Please log in to create an order.";
                return RedirectToAction("Login", "Customer");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("Login", "Customer");
            }

            ViewData["CustomerName"] = customer.FullName;
            ViewData["CustomerAddress"] = customer.Address;
            ViewData["CustomerEmail"] = customer.Email;

            var orderDetails = new List<OrderDetail>();
            foreach (var item in cart)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == item.Key);
                if (product != null)
                {
                    orderDetails.Add(new OrderDetail
                    {
                        ProductId = product.ProductId,
                        Quantity = item.Value,
                        UnitPrice = product.Price,
                        Product = product
                    });
                }
            }

            return View(orderDetails);
        }


        [HttpPost]
        public IActionResult ConfirmOrder(string PaymentMethod)
        {
            var cart = GetCartFromSession();
            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Cart", "Cart");
            }

            // Lấy CustomerId từ Session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["ErrorMessage"] = "Please log in to confirm the order.";
                return RedirectToAction("Login", "Customer");
            }

            // Truy vấn thông tin khách hàng từ bảng Customer
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("Login", "Customer");
            }

            // Tạo OrderId theo số thứ tự với tiền tố "ORD-"
            var lastOrder = _context.Orders.OrderByDescending(o => o.OrderId).FirstOrDefault();
            int newOrderNumber = (lastOrder != null ? int.Parse(lastOrder.OrderId.Substring(4)) + 1 : 1);
            string newOrderId = "ORD" + newOrderNumber.ToString("D3"); // D3 để có 3 chữ số (ví dụ: ORD-001)

            var order = new Order
            {
                OrderId = newOrderId,
                CustomerId = HttpContext.Session.GetInt32("CustomerId"),
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                PaymentStatus = "Pending",
                DeliveryStatus = "Pending",
                DeliveryType = "VPP",
            };


            _context.Orders.Add(order);

            foreach (var item in cart)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == item.Key);
                if (product != null)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = product.ProductId,
                        Quantity = item.Value,
                        UnitPrice = product.Price
                    };
                    order.TotalAmount += orderDetail.Quantity * orderDetail.UnitPrice;
                    _context.OrderDetails.Add(orderDetail);
                }
            }

            _context.SaveChanges();
            HttpContext.Session.Remove("Cart");

            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Details", "Order", new { id = customer.CustomerId });
        }

        [Route("/Oderdetail/{id}")]
        public IActionResult Details(string id)
        {
            var orders = _context.Orders
                .Where(o => o.CustomerId.ToString() == id)  // Lọc theo CustomerId hoặc OrderId nếu cần
                .Include(o => o.Customer) // Bao gồm thông tin Customer
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToList(); // Dùng ToList() để lấy tất cả các đơn hàng

            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found.");
            }

            return View(orders); // Truyền danh sách các đơn hàng đến view
        }



    }
}
