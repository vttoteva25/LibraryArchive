using LibraryArchive.Data;
using LibraryArchive.Models;
using LibraryArchive.ViewModels.GenreViewModel;
using LibraryArchive.ViewModels.PublisherViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace LibraryArchive.Controllers
{
    public class PublisherController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PublisherController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("publishers/index")]
        [HttpGet]
        public IActionResult Index(string sortOrder, int page = 1, int pageSize = 20)
        {
            var publishers = _db.Publishers.Include(p => p.Books).AsQueryable();
            dynamic model = new ExpandoObject();

            ViewBag.PublisherSortParam = string.IsNullOrEmpty(sortOrder) ? "Publisher_desc" : "";

            switch (sortOrder)
            {
                case "Publisher_desc":
                    publishers = publishers.OrderByDescending(p => p.Name);
                    break;
                default:
                    publishers = publishers.OrderBy(p => p.Name);
                    break;
            }

            var totalPublishers = publishers.Count();
            var totalPages = (int)Math.Ceiling((double)totalPublishers / pageSize);

            var currentPagePublishers = publishers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            model.Publishers = currentPagePublishers;
            model.PublishersCount = totalPublishers;
            model.TotalPages = totalPages;
            model.CurrentPage = page;
            model.PageSize = pageSize;

            return View(model);
        }

        [Route("publisher/deletePublisher/{id}")]
        [HttpGet]
        public IActionResult DeletePublisher([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var publisher = _db.Publishers.FirstOrDefault(p => p.PublisherId == id);

            if (publisher == null)
            {
                return NotFound();
            }

            _db.Publishers.Remove(publisher);
            _db.SaveChanges();

            return RedirectToAction("Index", "Publisher");
        }

        [HttpGet]
        public IActionResult Add()
        {
            AddPublisherViewModel model = new AddPublisherViewModel();
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Add(AddPublisherViewModel model)
        {
            if (ModelState.IsValid)
            {
                Publisher publisher = new Publisher();
                publisher.Name = model.PublisherName;

                try
                {
                    if (!CheckForExistingPublisher(model.PublisherName))
                    {
                        publisher.PublisherId = Guid.NewGuid().ToString();
                        _db.Publishers.Add(publisher);
                        _db.SaveChanges();
                    }
                    else
                    {
                        throw new ArgumentException("A publisher with the same name already exists!");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("PublisherNameExists", "Този издател вече съществува!");
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Publisher");
        }

        private bool CheckForExistingPublisher(string publisherName) => _db.Publishers.Any(x => x.Name == publisherName);
    }
}
