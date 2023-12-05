using LibraryArchive.Data;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.BookViewModel;
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

        public IActionResult Index(string sortOrder, string searchString, int page = 1, int pageSize = 20)
        {
            dynamic model = new ExpandoObject();
            model.BooksCount = _db.Books.Count();

            ViewBag.TitleSortParam = string.IsNullOrEmpty(sortOrder) ? "Title_desc" : "";
           
            if (string.IsNullOrEmpty(searchString))
            {
                var books = _db.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Genres)
                    .Include(b => b.Publisher)
                    .ToList();

                switch (sortOrder)
                {
                    case "Title_desc":
                        books = books.OrderByDescending(b => b.Title).ToList();
                        break;                  
                    default:
                        books = books.OrderBy(b => b.Title).ToList();
                        break;
                }

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
                model.Books = _db.Books.Where(x => x.Title.Contains(searchString));
                model.FiltredBooksCount = _db.Books.Where(x => x.Title.Contains(searchString)).Count();
                return View(model);
            }
        }

        [Route("book/edit/{id}")]
        [HttpGet]
        public IActionResult Edit([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Book book = _db.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Genres)
                    .Include(b => b.Publisher)
                    .FirstOrDefault(x => x.BookId.Equals(id));

            if (book == null)
            {
                return NotFound();
            }

            EditBookViewModel model = new EditBookViewModel();
            model.Id = id;
            model.Title = book.Title;
            model.Genres = string.Join(", \n", book.Genres.Select(g => g.Name));
            model.Description = book.Description;
            model.Authors = string.Join(", \n", book.Authors.Select(a => a.Name));
            model.PublisherName = book.Publisher.Name;
            model.PublicationYear = book.PublicationYear;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                Book updateBook = _db.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Genres)
                    .Include(b => b.Publisher)
                    .FirstOrDefault(x => x.BookId.Equals(model.Id));
                updateBook.Description = model.Description;

                if (updateBook is null)
                {
                    return NotFound();
                }

                _db.Books.Update(updateBook);
                _db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        // Log or display the error message
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }
            return View(model);
        }
    }
}
