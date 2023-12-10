using LibraryArchive.Models;

namespace LibraryArchive.ViewModels.ReaderViewModel
{
    public class ReaderViewModel
    {
        public List<User> Readers { get; set; }
        public int ReadersCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
    }
}
