using BookStoreAppSpring.Data;
using BookStoreAppSpring.Models;
using BookStoreAppSpring.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookStoreAppSpring.Areas.Customer.Controllers
{

    [Authorize]

    [Area("Customer")]

    public class CartItemController : Controller
    {

        private readonly BooksDbContext _dbContext;


        public CartItemController(BooksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItemsList = _dbContext.CartItems.Where(c => c.UserId == userId).Include(c => c.Book);

            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                CartItems = cartItemsList,
                Order = new Order()
            };

            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.SubTotal = cartItem.Book.Price * cartItem.Quantity;
                shoppingCartVM.Order.OrderTotal += cartItem.SubTotal;
            }

            return View(shoppingCartVM);
        }

        public IActionResult IncrementByOne(int id)
        {
            CartItem cartItem = _dbContext.CartItems.Find(id);

            cartItem.Quantity++;
            _dbContext.Update(cartItem);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult DecrementByOne(int id)
        {
            CartItem cartItem = _dbContext.CartItems.Find(id);

            if (cartItem.Quantity <= 1)
            {
                _dbContext.CartItems.Remove(cartItem);
                _dbContext.SaveChanges();
            }
            else
            {
                cartItem.Quantity--;
                _dbContext.Update(cartItem);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            CartItem cartItem = _dbContext.CartItems.Find(id);
            _dbContext.CartItems.Remove(cartItem);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult ReviewOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _dbContext.ApplicationUsers.Find(userId);

            var cartItemsList = _dbContext.CartItems.Where(c => c.UserId == userId).Include(c => c.Book);

            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                CartItems = cartItemsList,
                Order = new Order()
                {
                    CustomerName = user.Name,
                    StreetAddress = user.StreetAddress,
                    City = user.City,
                    State = user.State,
                    PostalCode = user.PostalCode,
                    Phone = user.PhoneNumber
                }
            };

            // Calculate the order total
            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.SubTotal = cartItem.Book.Price * cartItem.Quantity;
                shoppingCartVM.Order.OrderTotal += cartItem.SubTotal;
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReviewOrder(ShoppingCartVM shoppingCartVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItemsList = _dbContext.CartItems.Where(c => c.UserId == userId).Include(c => c.Book);

            shoppingCartVM.CartItems = cartItemsList;

            if (!shoppingCartVM.CartItems.Any())
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View("ReviewOrder", shoppingCartVM);
            }

            // Calculate the order total
            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.SubTotal = cartItem.Book.Price * cartItem.Quantity;
                shoppingCartVM.Order.OrderTotal += cartItem.SubTotal;
            }

            shoppingCartVM.Order.ApplicationUserId = userId;
            shoppingCartVM.Order.OrderDate = DateOnly.FromDateTime(DateTime.Now);
            shoppingCartVM.Order.OrderStatus = "Pending";
            shoppingCartVM.Order.PaymentStatus = "Pending";

            // Save order
            _dbContext.Orders.Add(shoppingCartVM.Order);
            _dbContext.SaveChanges();

            // Save order details
            foreach (var eachCartItem in shoppingCartVM.CartItems)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = shoppingCartVM.Order.OrderId,
                    BookId = eachCartItem.BookId,
                    Quantity = eachCartItem.Quantity,
                    Price = eachCartItem.Book.Price
                };
                _dbContext.OrderDetails.Add(orderDetail);
            }

            _dbContext.SaveChanges();

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + "Customer/CartItem/OrderConfirmation?id=" + shoppingCartVM.Order.OrderId,
                CancelUrl = domain + "Customer/CartItem/ReviewOrder",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                },
                Mode = "payment",
            };

            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                var sessionlineItem = new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = cartItem.Book.Price * 100,
                        Currency = "usd",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cartItem.Book.BookTitle,
                        },
                    },
                    Quantity = cartItem.Quantity,
                };
                options.LineItems.Add(sessionlineItem);
            }

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            shoppingCartVM.Order.SessionId = session.Id;
            _dbContext.SaveChanges();

            Response.Headers["Location"] = session.Url;
            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation(int id)
        {
            Order order = _dbContext.Orders.Find(id);

            if (order == null)
            {
                return NotFound();
            }

            var sessID = order.SessionId;

            if (string.IsNullOrEmpty(sessID))
            {
                return View(id);
            }

            var service = new Stripe.Checkout.SessionService();

            Session session = service.Get(sessID);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                order.PaymentStatus = "Paid";
                order.PaymentIntentId = session.PaymentIntentId;
                _dbContext.Orders.Update(order);
                _dbContext.SaveChanges();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                List<CartItem> listOfCartItems = _dbContext.CartItems.ToList().Where(c => c.UserId == userId).ToList();

                _dbContext.CartItems.RemoveRange(listOfCartItems);
                _dbContext.SaveChanges();
            }

            return View(id);
        }
    }
}
