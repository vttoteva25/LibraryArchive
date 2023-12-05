using LibraryArchive.Data;
using LibraryArchive.ViewModels.AuthorViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryArchive.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AuthorController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("authors/index")]
        [HttpGet]
        public IActionResult Index(string searchString, string sortOrder, int page = 1, int pageSize = 20)
        {
            var query = _db.Authors.Include(b => b.Books).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(author => author.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    query = query.OrderByDescending(b => b.Name);
                    break;
                default:
                    query = query.OrderBy(b => b.Name);
                    break;
            }

            var totalAuthors = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalAuthors / pageSize);

            var currentPageAuthors = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new AuthorViewModel
            {
                Authors = currentPageAuthors,
                AuthorsCount = totalAuthors,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                SearchString = searchString
            };

            return View(model);
        }
    }
}
