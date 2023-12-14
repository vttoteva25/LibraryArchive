using LibraryArchive.Data;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.BorrowViewModel;
using Microsoft.AspNetCore.Mvc;

namespace LibraryArchive.Controllers
{
    public class BorrowController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BorrowController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("borrow/{userId}")]
        [HttpGet]
        public IActionResult Borrow([FromRoute] string userId)
        {
            BorrowBookViewModel model = new BorrowBookViewModel();
            ViewBag.UserId = userId;
            return View(model);
        }

        [Route("borrow/{userId}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Borrow([FromRoute] string userId, [Bind("BookId")] BorrowBookViewModel model)
        {
            var borrowing = new Borrowing()
            {
                BorrowingId = Guid.NewGuid().ToString(),
                BookId = model.BookId,
                UserId = userId,
                BorrowDate = DateTime.Now,
            };

            var book = _db.Books.FirstOrDefault(b => b.BookId == model.BookId);
            if (book == null)
            {
                return NotFound();
            }
            book.Available = false;

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Borrowings.Add(borrowing);
                    _db.Books.Update(book);
                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("Borrowing", "Книгата не може да бъде взета");
                    return View(model);
                }
            }
            
            return RedirectToAction("Profile", "Reader", new { userId });
        }

        [Route("{userId}/borrow/return/{bookId}")]
        [HttpGet]
        public IActionResult Return([FromRoute] string bookId, [FromRoute] string userId)
        {
            var book = _db.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return NotFound();
            }
            book.Available = true;

            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            var borrowing = _db.Borrowings.FirstOrDefault(b => b.BookId == bookId && b.UserId == userId);
            if (borrowing == null)
            {
                return NotFound();
            }
            borrowing.ReturnDate = DateTime.Now;

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Borrowings.Update(borrowing);
                    _db.Books.Update(book);
                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("Borrowing", "Книгата не може да бъде върната");
                }
            }

            return RedirectToAction("Profile", "Reader", new { userId });
        }

    }
}
