using LibraryArchive.Data;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.BookViewModel;
using LibraryArchive.ViewModels.GenreViewModel;
using LibraryArchive.ViewModels.ReaderViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

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
            var readers = _db.Users
             .Where(u => EF.Property<string>(u, "UserType") == "User")
             .Include(r => r.Role)
             .Include(b => b.Borrowings)
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
        [Route("reader/profile/{userId}")]
        public IActionResult Profile([FromRoute] string userId)
        {
            User user = _db.Users
                 .Where(u => EF.Property<string>(u, "UserType") == "User")
                 .Include(r => r.Role)
                 .Include(b => b.Borrowings).AsQueryable()
                .FirstOrDefault(x => x.UserId == userId);                

            if (user is null)
            {
                return NotFound();
            }

            dynamic model = new ExpandoObject();

            model.User = user;
            model.Role = _db.Roles.FirstOrDefault(x => x.RoleId == user.RoleId);
            var borrowings = _db.Borrowing.Where(x => x.UserId == userId);
            model.Borrowings = borrowings;
            model.BorrowingsCount = borrowings.Count();
            var previousBorrowings = borrowings.ToList().Where(b => b.ReturnDate is not null);
            model.ReturnedBooks = _db.Books.Where(book => previousBorrowings.Any(br=> br.BookId.Equals(book.BookId)));
            var currentBorrowings = borrowings.ToList().Where(b => b.ReturnDate is null);
            model.BooksToReturn = _db.Books.Where(book => currentBorrowings.Any(br => br.BookId.Equals(book.BookId)));

            return View(model);
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
                User user = new User();
                user.UserId = model.UserId;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.Gender = model.Gender;
                user.BirthDate = GetBirthDateFromUserId(model.UserId);
                user.Email = model.Email;
                Role role = new Role();               
                role = _db.Roles.FirstOrDefault(x => x.RoleName == "Читател");
                user.Role = role;
                              
                try
                {
                    if (!CheckForExistingUser(model.UserId))
                    {
                        _db.Users.Add(user);
                        _db.SaveChanges();
                    }
                    else
                    {
                        throw new ArgumentException("A user with the same user id already exists!");
                    }

                }
                catch (Exception)
                {

                    ModelState.AddModelError("UserIdExists", "Потребител със такова ЕГН име вече съществува!");
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Home");
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

        [Route("reader/delete/{id}")]
        [HttpGet]
        public IActionResult Delete([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = _db.Users
                 .FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            _db.Users.Remove(user);
            _db.SaveChanges();

            return RedirectToAction("Index", "Reader");
        }

        public IActionResult Edit([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = _db.Users?
                    .FirstOrDefault(x => x.UserId.Equals(id));

            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new EditUserViewModel();
            model.UserId = id;
            model.BirthDate = user.BirthDate;
            model.Gender = user.Gender;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;           

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User updateUser = _db.Users?
                    .FirstOrDefault(x => x.UserId.Equals(model.UserId));

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
