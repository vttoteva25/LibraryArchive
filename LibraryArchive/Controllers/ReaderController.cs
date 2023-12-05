using LibraryArchive.Data;
using Microsoft.AspNetCore.Mvc;

namespace LibraryArchive.Controllers
{
    public class ReaderController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReaderController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
