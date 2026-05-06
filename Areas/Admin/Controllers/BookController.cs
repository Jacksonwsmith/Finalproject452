using BookStoreAppSpring.Data;
using BookStoreAppSpring.Models;
using BookStoreAppSpring.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreAppSpring.Areas.Admin.Controllers
{

       [Area("Admin")]
    public class BookController : Controller
    {

        private readonly BooksDbContext _dbContext;

        private IWebHostEnvironment _environment;
        

        public BookController(BooksDbContext dbContext, IWebHostEnvironment environment) //dependency injection
        {
            _dbContext = dbContext;
            _environment = environment;
        }
        public IActionResult Index()
        {
            var listOfBooks = _dbContext.Books.ToList();
            return View(listOfBooks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> listOfCategories = _dbContext.Categories.ToList().Select(o => new SelectListItem { Text = o.Name, Value = o.CategoryId.ToString() });
            //projection: taking an onbject of a certain data type and projecting or transofrimg it to something else which we will be using as a dropdown

            //ViewBag.ListOfCategories = listOfCategories;

            //ViewData["ListOfCategoriesVD"] = listOfCategories;

            BookWithCategoriesVM bookWithCategoriesVMobj = new BookWithCategoriesVM();
            bookWithCategoriesVMobj.Book = new Book();
            bookWithCategoriesVMobj.ListOfCategories = listOfCategories;

            

            return View(bookWithCategoriesVMobj);
        }

        [HttpPost]
        public IActionResult Create(BookWithCategoriesVM bookWithCategoriesVMobj, IFormFile imgFile)
        {
            if (ModelState.IsValid)
            {
                string wwwrootpath = _environment.WebRootPath;

                if(imgFile != null)
                {
                    using (var fileStream = new FileStream(Path.Combine(wwwrootpath, @"Images\" + imgFile.FileName), FileMode.Create))
                    {
                        imgFile.CopyTo(fileStream);//saves the file in the specified location



                    }

                    bookWithCategoriesVMobj.Book.ImgUrl = @"\Images\" + imgFile.FileName;
                }





                _dbContext.Books.Add(bookWithCategoriesVMobj.Book);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bookWithCategoriesVMobj);
        }

        public IActionResult Edit(int id)
        {
            Book book = _dbContext.Books.Find(id);

            IEnumerable<SelectListItem> listOfCategories = _dbContext.Categories.ToList().Select(o => new SelectListItem { Text = o.Name, Value = o.CategoryId.ToString() });

            BookWithCategoriesVM bookWithCategoriesVMobj = new BookWithCategoriesVM();
            bookWithCategoriesVMobj.Book = book;
            bookWithCategoriesVMobj.ListOfCategories = listOfCategories;

            return View(bookWithCategoriesVMobj);
        }

        [HttpPost]
        public IActionResult Edit(BookWithCategoriesVM bookWithCategoriesVMobj, IFormFile? imgFile)
        {
            if (ModelState.IsValid)
            {
                string wwwrootpath = _environment.WebRootPath;

                if (imgFile != null)
                {
                    if (!string.IsNullOrEmpty(bookWithCategoriesVMobj.Book.ImgUrl))
                    {
                        var oldImgPath = Path.Combine(wwwrootpath, bookWithCategoriesVMobj.Book.ImgUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(wwwrootpath, @"Images\" + imgFile.FileName), FileMode.Create))
                    {
                        imgFile.CopyTo(fileStream);
                    }

                    bookWithCategoriesVMobj.Book.ImgUrl = @"\Images\" + imgFile.FileName;
                }

                _dbContext.Books.Update(bookWithCategoriesVMobj.Book);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bookWithCategoriesVMobj);
        }
    }
}
