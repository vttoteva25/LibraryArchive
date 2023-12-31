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

        [Route("borrow/{readerNumber}")]
        [HttpGet]
        public IActionResult Borrow([FromRoute] string readerNumber)
        {
            BorrowBookViewModel model = new BorrowBookViewModel();
            ViewBag.ReaderNumber = readerNumber;
            return View(model);
        }

        [Route("borrow/{readerNumber}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Borrow([FromRoute] string readerNumber, [Bind("BookId")] BorrowBookViewModel model)
        {
            var reader = _db.Readers.FirstOrDefault(r => r.ReaderNumber == readerNumber);
            if (reader == null)
            {
                return NotFound();
            }

            var borrowing = new Borrowing()
            {
                BorrowingId = Guid.NewGuid().ToString(),
                BookId = model.BookId,
                UserId = reader.UserId,
                BorrowDate = DateTime.Now,
            };

            var book = _db.Books.FirstOrDefault(b => b.BookId == model.BookId);
            if (book == null)
            {
                ModelState.AddModelError("Borrowing", "Книга с такъв библиотечен номер не е намерена в библиотеката!");
                return View(model);            
            }

            if(!book.Available|| book.Scrapped)
            {
                ModelState.AddModelError("Borrowing", "Книгата е бракувана или е вече взета!");
                return View(model);
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
            
            return RedirectToAction("Profile", "Reader", new { readerNumber });
        }

        [Route("{readerNumber}/borrow/return/{bookId}")]
        [HttpGet]
        public IActionResult Return([FromRoute] string bookId, [FromRoute] string readerNumber)
        {
            var book = _db.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return NotFound();
            }
            book.Available = true;

            var reader = _db.Readers.FirstOrDefault(u => u.ReaderNumber == readerNumber);
            if (reader == null)
            {
                return NotFound();
            }

            var borrowing = _db.Borrowings.FirstOrDefault(b => b.BookId == bookId && b.UserId == reader.UserId);
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

            return RedirectToAction("Profile", "Reader", new { readerNumber });
        }

    }
}
