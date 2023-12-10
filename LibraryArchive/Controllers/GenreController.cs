using LibraryArchive.Data;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.GenreViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace LibraryArchive.Controllers
{
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GenreController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("genres/index")]
        [HttpGet]
        public IActionResult Index(string sortOrder, int page = 1, int pageSize = 20)
        {
            var genres = _db.Genres.Include(b => b.Books).AsQueryable();
            dynamic model = new ExpandoObject();

            ViewBag.GenreSortParam = string.IsNullOrEmpty(sortOrder) ? "Genre_desc" : "";
          
            switch (sortOrder)
            {
                case "Genre_desc":
                    genres = genres.OrderByDescending(g => g.Name);
                    break;
                default:
                    genres = genres.OrderBy(g => g.Name);
                    break;
            }

            var totalGenres = genres.Count();
            var totalPages = (int)Math.Ceiling((double)totalGenres / pageSize);

            var currentPageGenres = genres
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            model.Genres = currentPageGenres;
            model.GenresCount = totalGenres;
            model.TotalPages = totalPages;
            model.CurrentPage = page;
            model.PageSize = pageSize;
 
            return View(model);
        }

        [Route("Genre/DeleteGenre/{id}")]
        [HttpGet]
        public IActionResult DeleteGenre([FromRoute]string id)
        {
            if(string.IsNullOrEmpty(id))
                return NotFound();

            var genre = _db.Genres.FirstOrDefault(g => g.GenreId == id);

            if (genre == null)
            {
                return NotFound();
            }

            _db.Genres.Remove(genre);
            _db.SaveChanges();

            return RedirectToAction("Index", "Genre");
        }

        [HttpGet]
        public IActionResult Add()
        {
            AddGenreViewModel model = new AddGenreViewModel();
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Add(AddGenreViewModel model)
        {
            if (ModelState.IsValid)
            {
                Genre genre = new Genre();
                genre.Name = model.GenreName;

                try
                {
                    if (!CheckForExistingGenre(model.GenreName))
                    {
                        genre.GenreId = Guid.NewGuid().ToString();
                        _db.Genres.Add(genre);
                        _db.SaveChanges();
                    }
                    else
                    {
                        throw new ArgumentException("A genre with the same name already exists!");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("GenreNameExists", "Този жанр вече съществува!");
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Genre");
        }

        private bool CheckForExistingGenre(string genreName) => _db.Genres.Any(x => x.Name == genreName);
    }
}
