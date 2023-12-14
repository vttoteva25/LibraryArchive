using LibraryArchive.Models;

namespace LibraryArchive.ViewModels.BookViewModel
{
    public class BorrowedBookViewModel
    {
        public List<Book> Books { get; set; }
        public List<User> Users { get; set; }
        public List<Borrowing> Borrowings { get; set; }
        public int BooksCount { get; set; }
        public string SortOrder { get; set; } = "Title_asc";
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
