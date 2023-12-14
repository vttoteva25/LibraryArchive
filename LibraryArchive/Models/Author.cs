using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.Models
{
    public class Author
    {
        [Key]
        public string AuthorId { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public string? Biography { get; set; }

        public ICollection<Book>? Books { get; set; } = new List<Book>();
    }
}
