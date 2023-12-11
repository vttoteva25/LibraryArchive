using LibraryArchive.Data;
using LibraryArchive.HelpingTools;
using LibraryArchive.ViewModels.HomeViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

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
            dynamic model = new ExpandoObject();
            model.Librarians = _db.Librarians;
            model.Administrators = _db.Administrators;
            return View(model);
        }

        #region LoginUser

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string username = model.Username;
                    string password = model.Password;

                    if (_db.Administrators.Any(x => x.Username == username))
                    {
                        if (_db.Administrators.FirstOrDefault(x => x.Username == username).Password == Hasher.Hash(password))
                        {
                            Logged.Administrator = _db.Administrators.FirstOrDefault(x => x.Username == username);
                        }
                        else
                        {
                            throw new ArgumentException("Incorrect username or password");
                        }
                    }
                    else if (_db.Librarians.Any(x => x.Username == username))
                    {
                        if (_db.Librarians.FirstOrDefault(x => x.Username == username).Password == Hasher.Hash(password))
                        {
                            Logged.Librarian = _db.Librarians.FirstOrDefault(x => x.Username == username);
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
            Logged.Administrator = null;

            return RedirectToAction("Index", "Home");
        }

        #endregion       
    }
}
