using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.Models
{
    public class Publisher
    {
        [Key]
        public string PublisherId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Website { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
