using LibraryArchive.Data;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace LibraryArchive.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BookController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string searchString)
        {
            dynamic model = new ExpandoObject();
            model.BooksCount = _db.Books.Count();

            if (string.IsNullOrEmpty(searchString))
            {
                model.Books = _db.Books;
                model.FiltredBooksCount = 0;
                return View(model);
            }
            else
            {
                model.Deliveries = _db.Books.Where(x => x.Title.Contains(searchString));
                model.FiltredDeliveriesCount = _db.Books.Where(x => x.Title.Contains(searchString)).Count();
                return View(model);
            }

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
