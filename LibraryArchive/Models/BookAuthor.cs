using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryArchive.Models
{
    public class BookAuthor
    {
        [ForeignKey("Book Id")]
        public string BookId { get; set; }

        [ForeignKey("Author Id")]
        public string AuthorId { get; set; }

        public Book Book { get; set; }
        public Author Author { get; set; }
    }
}
