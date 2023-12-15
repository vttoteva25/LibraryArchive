using LibraryArchive.Data;
using LibraryArchive.Models;
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

            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";

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

        [Route("author/delete/{authorId}")]
        [HttpGet]
        public IActionResult Delete([FromRoute] string authorId)
        {
            if (string.IsNullOrEmpty(authorId))
                return NotFound();

            var author = _db.Authors
                 .FirstOrDefault(u => u.AuthorId == authorId);

            if (author == null)
            {
                return NotFound();
            }

            _db.Authors.Remove(author);
            _db.SaveChanges();

            return RedirectToAction("Index", "Author");
        }

        [Route("author/edit/{authorId}")]
        [HttpGet]
        public IActionResult Edit([FromRoute] string authorId)
        {
            if (string.IsNullOrEmpty(authorId))
            {
                return NotFound();
            }

            var author = _db.Authors
                    .FirstOrDefault(x => x.AuthorId.Equals(authorId));

            if (author == null)
            {
                return NotFound();
            }

            EditAuthorViewModel model = new EditAuthorViewModel();
            model.AuthorId = authorId;
            model.Name = author.Name;
            model.BirthDate = author.BirthDate;
            model.DeathDate = author.DeathDate;
            model.Biography = author.Biography;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditAuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateAuthor = _db.Authors
                    .FirstOrDefault(x => x.AuthorId.Equals(model.AuthorId));
                updateAuthor.DeathDate = model.DeathDate;
                updateAuthor.Biography = model.Biography;

                if (updateAuthor is null)
                {
                    return NotFound();
                }

                _db.Authors.Update(updateAuthor);
                _db.SaveChanges();

                return RedirectToAction("Index", "Author");
            }

            return View(model);
        }

        [Route("author/add")]
        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddAuthorViewModel();
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Add(AddAuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var author = new Author();
                author.AuthorId = Guid.NewGuid().ToString();
                author.Name = model.Name;
                author.BirthDate = model.BirthDate;
                author.Biography = model.Biography;
                author.DeathDate = model.DeathDate;

                try
                {
                    _db.Authors.Add(author);
                    _db.SaveChanges();

                }
                catch (Exception)
                {
                    ModelState.AddModelError("Authors", "Възникна проблем при добавянето на автор");
                    return View(model);
                }

            }

            return RedirectToAction("Index", "Author");
        }
    }

}
