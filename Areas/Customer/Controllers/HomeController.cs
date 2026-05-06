using System.Diagnostics;
using System.Security.Claims;
using BookStoreAppSpring.Data;
using BookStoreAppSpring.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAppSpring.Areas.Customer.Controllers
{
        [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly BooksDbContext _dbContext;

        public HomeController(BooksDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var listOfBooks = _dbContext.Books.Include(c => c.category);

            return View(listOfBooks.ToList());
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

        public IActionResult Details(int id)
        {
            var book = _dbContext.Books.Include(c => c.category).FirstOrDefault(b => b.BookId == id);

            var cartItem = new CartItem()
            {
                BookId = id,
                Book = book,
                Quantity = 1
            };

            return View(cartItem);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(CartItem cartItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            cartItem.UserId = userId;

            CartItem existingCart = _dbContext.CartItems.FirstOrDefault(c => c.BookId == cartItem.BookId && c.UserId == userId);

            if (existingCart != null)
            {
                existingCart.Quantity += cartItem.Quantity;
                _dbContext.CartItems.Update(existingCart);
            }
            else
            {
                _dbContext.CartItems.Add(cartItem);
            }

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
