using LibraryArchive.Data;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.BookViewModel;
using LibraryArchive.ViewModels.ReaderViewModel;
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

        [Route("book/add")]
        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddBookViewModel();
            ViewBag.Genres = _db.Genres.ToList();
            ViewBag.Publishers = _db.Publishers.ToList();
            ViewBag.Authors = _db.Authors.ToList();
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Add(AddBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var book = new Book();
                book.BookId = model.BookId;
                book.Title = model.Title;
                book.Description = model.Description;
                book.PublicationYear = model.PublicationYear;
                book.Scrapped = model.Scrapped;
                book.Availability = model.Availability;
                book.Language = model.Language;

                var publisher = _db.Publishers.FirstOrDefault(p => p.PublisherId == model.PublisherId);
                if (publisher != null)
                {
                    book.Publisher = publisher;
                    book.PublisherId = publisher.PublisherId;
                }

                var authors = _db.Authors.Where(a => model.BookAuthorsIds.Contains(a.AuthorId)).ToList();
                book.Authors = authors;
                var booksAuthors = authors.Select(ba => new BookAuthor()
                {
                    AuthorId = ba.AuthorId,
                    Author = ba,
                    BookId = book.BookId,
                    Book = book,
                }).ToList();
                
                var genres = _db.Genres.Where(g => model.BookGenresIds.Contains(g.GenreId)).ToList();
                book.Genres = genres;
                var booksGenres = genres.Select(bg => new BookGenre()
                {
                    Book = book,
                    BookId = book.BookId,
                    Genre = bg,
                    GenreId = bg.GenreId,
                }).ToList();

                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        _db.Books.Add(book);
                        booksAuthors.Select(ba => _db.BookAuthors.Add(ba));
                        booksGenres.Select(bg => _db.BookGenres.Add(bg));
                        _db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("Books", "Възникна проблем при добавянето на книга");
                        return View(model);
                    }
                }
            }
         
            return RedirectToAction("Index", "Book");
        }

        private bool CheckForExistingBook(string bookId) => _db.Books.Any(x => x.BookId == bookId);

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
            model.BookId = id;
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
                    .FirstOrDefault(x => x.BookId.Equals(model.BookId));
                updateBook.Description = model.Description;

                if (updateBook is null)
                {
                    return NotFound();
                }

                _db.Books.Update(updateBook);
                _db.SaveChanges();

                return RedirectToAction("Index", "Book");
            }
           
            return View(model);
        }
    }
}
