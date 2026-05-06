using BookStoreAppSpring.Data;
using BookStoreAppSpring.Models;
using BookStoreAppSpring.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAppSpring.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class OrderController : Controller
    {
        private readonly BooksDbContext _dbContext;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(BooksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            IEnumerable<Order> listOfOrders = _dbContext.Orders.Include(o => o.ApplicationUser);

            return View(listOfOrders);
        }

        public IActionResult Details(int id)
        {
            var order = _dbContext.Orders.Include(o => o.ApplicationUser).FirstOrDefault(o => o.OrderId == id);

            var orderDetails = _dbContext.OrderDetails.Include(od => od.Book).Where(od => od.OrderId == id).ToList();

            OrderVM orderVM = new OrderVM()
            {
                Order = order,
                OrderDetails = orderDetails
            };

            return View(orderVM);
        }

        [HttpPost]
        public IActionResult UpdateOrderInformation()
        {
            Order orderFromDB = _dbContext.Orders.Find(OrderVM.Order.OrderId);

            orderFromDB.CustomerName = OrderVM.Order.CustomerName;
            orderFromDB.StreetAddress = OrderVM.Order.StreetAddress;
            orderFromDB.City = OrderVM.Order.City;
            orderFromDB.State = OrderVM.Order.State;
            orderFromDB.PostalCode = OrderVM.Order.PostalCode;
            orderFromDB.Phone = OrderVM.Order.Phone;
            orderFromDB.Carrier = OrderVM.Order.Carrier;
            orderFromDB.ShippingDate = OrderVM.Order.ShippingDate;
            orderFromDB.TrackingNumber = OrderVM.Order.TrackingNumber;

            _dbContext.Orders.Update(orderFromDB);
            _dbContext.SaveChanges();

            return RedirectToAction("Details", new { id = orderFromDB.OrderId });
        }

        [HttpPost]
        public IActionResult ProcessOrder()
        {
            Order order = _dbContext.Orders.Find(OrderVM.Order.OrderId);
            order.OrderStatus = "Processing";
            order.ShippingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
            order.Carrier = "UPS";
            _dbContext.Orders.Update(order);
            _dbContext.SaveChanges();
            return RedirectToAction("Details", new { id = order.OrderId });
        }

        [HttpPost]
        public IActionResult CompleteOrder()
        {
            Order order = _dbContext.Orders.Find(OrderVM.Order.OrderId);
            order.OrderStatus = "Shipped and Completed";
            order.ShippingDate = DateOnly.FromDateTime(DateTime.Now);
            order.Carrier = "UPS";
            _dbContext.Orders.Update(order);
            _dbContext.SaveChanges();
            return RedirectToAction("Details", new { id = order.OrderId });
        }
    }
}
