using LibraryArchive.Data;
using LibraryArchive.HelpingTools;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.AuthorViewModel;
using LibraryArchive.ViewModels.ReaderViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                 .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                readers = readers.Where(reader => (reader.FirstName.Contains(searchString) || reader.LastName.Contains(searchString)) 
                || string.Concat("reader.FirstName", " ", "reader.LastName").Contains(searchString));
            }

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
    }
}
