using LibraryArchive.Models;

namespace LibraryArchive.ViewModels.ReaderViewModel
{
    public class ProfileViewModel
    {
        public Reader Reader { get; set; }

        public List<Borrowing> Borrowings { get; set; }

        public int BorrowingsCount { get; set; }

        public List<BorrowedBookViewModel> BooksToReturn { get; set; }

        public List<BorrowedBookViewModel> ReturnedBooks { get; set; }
    }

    public class BorrowedBookViewModel
    {
        public string Title { get; set; }
        public string BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}
