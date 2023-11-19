using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.Models
{
    public class Author
    {
        [Key]
        public string AuthorId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public DateTime DeathDate { get; set; }

        [Required]
        public string Biography { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
