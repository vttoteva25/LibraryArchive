using LibraryArchive.Models;

namespace LibraryArchive.ViewModels.LibrarianViewModel
{
    public class LibrarianViewModel
    {
        public List<Librarian> Librarians { get; set; }
        public int LibrariansCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
    }
}
