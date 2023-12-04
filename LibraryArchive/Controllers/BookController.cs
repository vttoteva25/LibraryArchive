using LibraryArchive.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index(string searchString, int page = 1, int pageSize = 20)
        {
            dynamic model = new ExpandoObject();
            model.BooksCount = _db.Books.Count();

            if (string.IsNullOrEmpty(searchString))
            {
                var books = _db.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Genres)
                    .Include(b => b.Publisher)
                    .ToList();

                var totalPages = (int)Math.Ceiling((double)books.Count() / pageSize);
                var currentPageBooks = books.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                model.Books = currentPageBooks;
                model.TotalPages = totalPages;
                model.CurrentPage = page;
                model.PageSize = pageSize;
                model.SearchString = searchString;
                model.FiltredBooksCount = model.Books.Count;
                return View(model);
            }
            else
            {
                model.Deliveries = _db.Books.Where(x => x.Title.Contains(searchString));
                model.FiltredDeliveriesCount = _db.Books.Where(x => x.Title.Contains(searchString)).Count();
                return View(model);
            }
        }       
    }
}
