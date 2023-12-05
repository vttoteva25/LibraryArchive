using LibraryArchive.Data;
using Microsoft.AspNetCore.Mvc;

namespace LibraryArchive.Controllers
{
    public class PublisherController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PublisherController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
