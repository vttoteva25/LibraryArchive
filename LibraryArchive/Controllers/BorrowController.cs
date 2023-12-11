﻿using LibraryArchive.Data;
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

        [HttpGet]
        public IActionResult Borrow([FromRoute] string userId)
        {
            BorrowBookViewModel model = new BorrowBookViewModel();
            model.UserId = userId;
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Borrow(BorrowBookViewModel model)
        {
            var borrowing = new Borrowing()
            {
                BorrowingId = Guid.NewGuid().ToString(),
                BookId = model.BookId,
                UserId = model.UserId,
                BorrowDate = DateTime.Now,
            };

            var book = _db.Books.FirstOrDefault(b => b.BookId == model.BookId);
            if (book != null)
            {
                return NotFound();
            }
            book.Availability = false;

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Borrowing.Add(borrowing);
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
            
            return RedirectToAction("Profile", "Reader", model.UserId);
        }

        [Route("borrowing/return/{bookId}")]
        [HttpPost]
        public IActionResult Return([FromRoute] string bookId, [FromRoute] string userId)
        {
            var book = _db.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return NotFound();
            }
            book.Availability = true;

            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            var borrowing = _db.Borrowing.FirstOrDefault(b => b.BookId == bookId && b.UserId == userId);
            if (borrowing == null)
            {
                return NotFound();
            }
            borrowing.ReturnDate = DateTime.Now;

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Borrowing.Update(borrowing);
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

            return RedirectToAction("Profile", "Reader", userId);
        }

    }
}
