using LibraryArchive.Data;
using LibraryArchive.HelpingTools;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.HomeViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            model.Librarians = _db.Librarians
                .Where(u => EF.Property<string>(u, "UserType") == "Librarian");
            model.Administrators = _db.Administrators
                .Where(u => EF.Property<string>(u, "UserType") == "Administrator");
            return View(model);
        }

        [HttpGet]
        [Route("user/profile/{id}")]
        public IActionResult Profile([FromRoute] string id)
        {
            Librarian user;
            if(Logged.Librarian != null)
            {
                user = _db.Librarians?.FirstOrDefault(x => x.UserId == id);
            }
            else
            {
                user = _db.Administrators?.FirstOrDefault(x => x.UserId == id);
            }

            if (user is null)
            {
                return NotFound();
            }

            dynamic model = new ExpandoObject();

            model.User = user;
            model.Role = _db.Roles.FirstOrDefault(x => x.RoleId == user.RoleId);

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
                        if (_db.Administrators
                            .Where(u => EF.Property<string>(u, "UserType") == "Administrator")?
                            .FirstOrDefault(x => x.Username == username)?.Password == Hasher.Hash(password))
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
                        if (_db.Librarians
                            .Where(u => EF.Property<string>(u, "UserType") == "Librarian")?
                            .FirstOrDefault(x => x.Username == username)?.Password == Hasher.Hash(password))
                        {
                            Logged.Librarian = _db.Librarians?.FirstOrDefault(x => x.Username == username);
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
            if(Logged.Librarian != null)
                Logged.Librarian = null;

            if(Logged.Administrator != null)
                Logged.Administrator = null;

            return RedirectToAction("Index", "Home");
        }

        #endregion       
    }
}
