using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryArchive.Models
{
    public class Book
    {
        [Key]
        public string BookId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string PublicationYear { get; set; }

        [Required]
        public string Description { get; set; }

        [ForeignKey("Publisher Id")]
        public string PublisherId { get; set; }
        public Publisher Publisher { get; set; }

        [Required]
        public bool Availability { get; set; }

        [Required]
        public string Language { get; set; }       

        [Required]
        public bool Scrapped { get; set; }

        [Required]
        public ICollection<Author> Authors { get; set; }

        [Required]
        public ICollection<Genre> Genres { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
