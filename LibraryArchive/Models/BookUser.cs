using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryArchive.Models
{
    public class BookUser
    {
        [ForeignKey("Book Id")]
        public string BookId { get; set; }

        [ForeignKey("User Id")]
        public string UserId { get; set; }

        public Book Book { get; set; }
        public User User { get; set; }
    }
}
