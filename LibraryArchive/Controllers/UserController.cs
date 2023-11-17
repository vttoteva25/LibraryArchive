using LibraryArchive.Data;
using LibraryArchive.HelpingTools;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.HomeViewModel;
using Microsoft.AspNetCore.Mvc;

namespace LibraryArchive.Controllers
{
    [AuthenticationFilter]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Librarian> librariansList = _db.Librarians;
            return View(librariansList);
        }

        #region LoginUser

        [HttpGet]
        public IActionResult Login()
        {
            LoginLibrarianViewModel model = new LoginLibrarianViewModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginLibrarianViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string username = model.Username;
                    string password = model.Password;
                    Librarian librarian = new Librarian();

                    if (_db.Librarians.Any(x => x.Username == username))
                    {
                        if (_db.Librarians.FirstOrDefault(x => x.Username == username).Password == Hasher.Hash(password))
                        {
                            librarian = _db.Librarians.FirstOrDefault(x => x.Username == username);
                        }
                        else
                        {
                            throw new ArgumentException("Incorrect username or password");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Incorrect username or password");
                    }

                    Logged.Librarian = librarian;
                }

                catch (Exception)
                {
                    ModelState.AddModelError("loginError", "Невалидно потребителско име или парола");
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Home"); ;
        }
        #endregion

        #region SignOut

        public IActionResult SigningOut()
        {
            Logged.Librarian = null;

            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
