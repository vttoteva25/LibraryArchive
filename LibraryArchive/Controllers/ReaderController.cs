﻿using LibraryArchive.Data;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.ReaderViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryArchive.Controllers
{
    public class ReaderController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReaderController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("readers/index")]
        [HttpGet]
        public IActionResult Index(string searchString,  int page = 1, int pageSize = 20)
        {
            var readers = _db.Readers
             .Where(u => EF.Property<string>(u, "UserType") == "Reader")
             .OfType<Reader>()
             .Include(r => r.Role)
             .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                readers = readers
                    .Where(reader =>
                        reader.FirstName.ToLower().Equals(searchString) ||reader.LastName.ToLower().Contains(searchString) ||
                        (reader.FirstName + " " + reader.LastName).ToLower().Contains(searchString))
                    .ToList();
            }

            var result = readers.ToList();

            var totalReaders = readers.Count();
            var totalPages = (int)Math.Ceiling((double)totalReaders / pageSize);

            var currentPageReaders = readers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new ReaderViewModel
            {
                Readers = currentPageReaders,
                ReadersCount = totalReaders,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                SearchString = searchString,
            };

            return View(model);
        }

        [HttpGet]
        [Route("reader/profile/{readerNumber}")]
        public IActionResult Profile([FromRoute] string readerNumber)
        {
            var reader = _db.Readers
                 .Where(u => EF.Property<string>(u, "UserType") == "Reader")
                 .Include(r => r.Role)
                .FirstOrDefault(x => x.ReaderNumber == readerNumber);                

            if (reader is null)
            {
                return NotFound();
            }

            var borrowings = _db.Borrowings.Where(x => x.UserId == reader.UserId).ToList();
            var previousBorrowings = borrowings.ToList().Where(b => b.ReturnDate is not null);
            var returnedBooks = previousBorrowings.Any() ? 
                _db.Books.AsEnumerable().Where(book => previousBorrowings.Any(br => br.BookId.Equals(book.BookId))).ToList() : 
                new List<Book>();
            var currentBorrowings = borrowings.ToList().Where(b => b.ReturnDate is null);
            var booksToReturn = currentBorrowings.Any() ?
                _db.Books.AsEnumerable().Where(book => currentBorrowings.Any(br => br.BookId == book.BookId)).ToList() :
                new List<Book>();

            var profileViewModel = new ProfileViewModel()
            {
                Reader = reader,
                Borrowings = borrowings,
                BorrowingsCount = borrowings.Count,
                ReturnedBooks = returnedBooks.Select(book => 
                    new BorrowedBookViewModel()
                    {
                        BookId = book.BookId,
                        Title = book.Title,
                        BorrowDate = previousBorrowings?.FirstOrDefault(bd => bd.BookId == book.BookId)?.BorrowDate ?? default,
                        ReturnDate = previousBorrowings?.FirstOrDefault(bd => bd.BookId == book.BookId)?.ReturnDate ?? default
                    }).ToList(),
                BooksToReturn = booksToReturn.Select(book => 
                    new BorrowedBookViewModel()
                    {
                        BookId = book.BookId,
                        Title = book.Title,
                        BorrowDate = currentBorrowings?.FirstOrDefault(bd => bd.BookId == book.BookId)?.BorrowDate ?? default,
                        ReturnDate = currentBorrowings?.FirstOrDefault(bd => bd.BookId == book.BookId)?.ReturnDate ?? default
                    }).ToList(),
            };          

            return View(profileViewModel);
        }

        #region RegisterUser

        [HttpGet]
        public IActionResult Register()
        {
            RegisterReaderViewModel model = new RegisterReaderViewModel();
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Register(RegisterReaderViewModel model)
        {
            if (ModelState.IsValid)
            {             
                var reader = new Reader();
                reader.UserId = model.UserId;
                reader.ReaderNumber = model.ReaderNumber;
                reader.FirstName = model.FirstName;
                reader.LastName = model.LastName;
                reader.PhoneNumber = model.PhoneNumber;
                reader.Gender = model.Gender;
                reader.BirthDate = GetBirthDateFromUserId(model.UserId);
                reader.Email = model.Email;
                Role role = new Role();               
                role = _db.Roles.FirstOrDefault(x => x.RoleName == "Читател");
                reader.Role = role;
                              
                try
                {
                    if (!CheckForExistingUser(model.UserId))
                    {
                        _db.Readers.Add(reader);
                        _db.SaveChanges();
                    }
                    else
                    {
                        throw new ArgumentException("A user with the same user id already exists!");
                    }

                }
                catch (Exception)
                {

                    ModelState.AddModelError("UserIdExists", "Потребител със такова ЕГН вече съществува!");
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Reader");
        }

        private bool CheckForExistingUser(string userId) => _db.Users.Any(x => x.UserId == userId);

        private DateTime GetBirthDateFromUserId(string userId)
        {
            var dateString = userId.Substring(0, 6);

            int year = int.Parse(dateString.Substring(0, 2));
            int month = int.Parse(dateString.Substring(2, 2));
            int day = int.Parse(dateString.Substring(4, 2));

            int baseYear = (month >= 40 && month <= 52) ? 2000 : 1900;

            year += baseYear;

            return new DateTime(year, month % 40, day);
        }

        #endregion

        [Route("reader/delete/{readerNumber}")]
        [HttpGet]
        public IActionResult Delete([FromRoute] string readerNumber)
        {
            if (string.IsNullOrEmpty(readerNumber))
                return NotFound();

            var reader = _db.Readers
                 .FirstOrDefault(u => u.ReaderNumber == readerNumber);

            if (reader == null)
            {
                return NotFound();
            }

            _db.Readers.Remove(reader);
            _db.SaveChanges();

            return RedirectToAction("Index", "Reader");
        }

        [Route("reader/edit/{readerNumber}")]
        public IActionResult Edit([FromRoute] string readerNumber)
        {
            if (string.IsNullOrEmpty(readerNumber))
            {
                return NotFound();
            }

            var reader = _db.Readers?
                    .FirstOrDefault(x => x.ReaderNumber.Equals(readerNumber));

            if (reader == null)
            {
                return NotFound();
            }

            EditReaderViewModel model = new EditReaderViewModel();
            model.ReaderId = reader.UserId;
            model.ReaderNumber = readerNumber;
            model.BirthDate = reader.BirthDate;
            model.Gender = reader.Gender;
            model.FirstName = reader.FirstName;
            model.LastName = reader.LastName;
            model.Email = reader.Email;
            model.PhoneNumber = reader.PhoneNumber;           

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditReaderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateUser = _db.Users?
                    .FirstOrDefault(x => x.UserId.Equals(model.ReaderId));

                if (updateUser is null)
                {
                    return NotFound();
                }

                updateUser.PhoneNumber = model.PhoneNumber;
                updateUser.Email = model.Email;

                _db.Users.Update(updateUser);
                _db.SaveChanges();

                return RedirectToAction("Index", "Reader");
            }
            return View(model);
        }
    }
}
