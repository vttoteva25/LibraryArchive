using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryArchive.Models
{
    public class Borrowing
    {
        [Key]
        public string BorrowingId { get; set; }

        [ForeignKey("User Id")]
        public string UserId { get; set; }
        public Librarian User { get; set; }

        [ForeignKey("Book Id")]
        public string BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; }

    }
}
