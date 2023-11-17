using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryArchive.Models
{
    public class Borrowing
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User Id")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Book Id")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }

        public DateTime ReturnDate { get; set; }

    }
}
