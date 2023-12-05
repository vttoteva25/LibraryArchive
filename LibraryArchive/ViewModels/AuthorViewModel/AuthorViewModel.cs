using LibraryArchive.Models;

namespace LibraryArchive.ViewModels.AuthorViewModel
{
    public class AuthorViewModel
    {
        public List<Author> Authors { get; set; }
        public int AuthorsCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
    }
}
