using LibraryArchive.Data;
using LibraryArchive.HelpingTools;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.LibrarianViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace LibraryArchive.Controllers
{
    public class LibrarianController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LibrarianController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("librarians/index")]
        [HttpGet]
        public IActionResult Index(string searchString, int page = 1, int pageSize = 20)
        {
            var librarians = _db.Librarians
             .Where(u => EF.Property<string>(u, "UserType") == "Librarian")
             .Include(r => r.Role)
             .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                librarians = librarians
                    .Where(librarian =>
                        librarian.FirstName.ToLower().Equals(searchString) || librarian.LastName.ToLower().Contains(searchString) ||
                        (librarian.FirstName + " " + librarian.LastName).ToLower().Contains(searchString))
                    .ToList();
            }

            var result = librarians.ToList();

            var totalLibrarians = librarians.Count();
            var totalPages = (int)Math.Ceiling((double)totalLibrarians / pageSize);

            var currentPageLibrarians = librarians
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new LibrarianViewModel
            {
                Librarians = currentPageLibrarians,
                LibrariansCount = totalLibrarians,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                SearchString = searchString,
            };

            return View(model);
        }

        [HttpGet]
        [Route("librarian/profile/{id}")]
        public IActionResult Profile([FromRoute] string id)
        {
            Librarian librarian = _db.Librarians
                 .Where(u => EF.Property<string>(u, "UserType") == "Librarian")
                 .Include(r => r.Role)
                .FirstOrDefault(x => x.UserId == id);

            if (librarian is null)
            {
                return NotFound();
            }

            dynamic model = new ExpandoObject();

            model.Librarian = librarian;
            model.Role = _db.Roles.FirstOrDefault(x => x.RoleId == librarian.RoleId);           

            return View(model);
        }

        #region RegisterUser

        [HttpGet]
        public IActionResult Register()
        {
            RegisterLibrarianViewModel model = new RegisterLibrarianViewModel();
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Register(RegisterLibrarianViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("passConfirm", "Паролите не съвпадат!");
                    return View(model);
                }

                var librarian = new Librarian();
                librarian.UserId = model.LibrarianId;
                librarian.Username = model.Username;
                librarian.Password = Hasher.Hash(model.Password);
                librarian.FirstName = model.FirstName;
                librarian.LastName = model.LastName;
                librarian.PhoneNumber = model.PhoneNumber;
                librarian.Gender = model.Gender;
                librarian.BirthDate = GetBirthDateFromUserId(model.LibrarianId);
                librarian.Email = model.Email;
                Role role = new Role();
                role = _db.Roles.FirstOrDefault(x => x.RoleName == "Администратор");
                librarian.Role = role;

                try
                {
                    if (!CheckForExistingLibrarian(model.LibrarianId))
                    {
                        _db.Librarians.Add(librarian);
                        _db.SaveChanges();
                    }
                    else
                    {
                        throw new ArgumentException("A librarian with the same user id already exists!");
                    }

                }
                catch (Exception)
                {

                    ModelState.AddModelError("UserIdExists", "Библиотекар със такова ЕГН вече съществува!");
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Librarian");
        }

        private bool CheckForExistingLibrarian(string userId) => _db.Librarians.Any(x => x.UserId == userId);

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

        [Route("librarian/delete/{id}")]
        [HttpGet]
        public IActionResult Delete([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var librarian = _db.Librarians
                 .FirstOrDefault(u => u.UserId == id);

            if (librarian == null)
            {
                return NotFound();
            }

            _db.Librarians.Remove(librarian);
            _db.SaveChanges();

            return RedirectToAction("Index", "Librarian");
        }

        public IActionResult Edit([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Librarian librarian = _db.Librarians?
                    .FirstOrDefault(x => x.UserId.Equals(id));

            if (librarian == null)
            {
                return NotFound();
            }

            EditLibrarianViewModel model = new EditLibrarianViewModel();
            model.LibrarianId = id;
            model.BirthDate = librarian.BirthDate;
            model.Gender = librarian.Gender;
            model.FirstName = librarian.FirstName;
            model.LastName = librarian.LastName;
            model.Email = librarian.Email;
            model.PhoneNumber = librarian.PhoneNumber;
            model.Username = librarian.Username;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditLibrarianViewModel model)
        {
            if (ModelState.IsValid)
            {
                Librarian updateLibrarian = _db.Librarians?
                    .FirstOrDefault(x => x.UserId.Equals(model.LibrarianId));

                if (updateLibrarian is null)
                {
                    return NotFound();
                }

                if(!string.IsNullOrEmpty(model.Password) || !string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    if (model.Password != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("passConfirm", "Паролите не съвпадат!");
                        return View(model);
                    }
                    else
                    {
                        updateLibrarian.Password = Hasher.Hash(model.Password);
                    }
                }

                updateLibrarian.PhoneNumber = model.PhoneNumber;
                updateLibrarian.Email = model.Email;

                _db.Librarians.Update(updateLibrarian);
                _db.SaveChanges();

                return RedirectToAction("Index", "Librarian");
            }
            return View(model);
        }
    }
}
