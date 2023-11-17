using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryArchive.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string PublicationYear { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public string Description { get; set; }

        [ForeignKey("Publisher Id")]
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }

        [Required]
        public bool Availability { get; set; }

        [ForeignKey("User Id")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int PageCount { get; set; }

        [Required]
        public string Language { get; set; }       

        [Required]
        public bool Scrapped { get; set; }

        [Required]
        public ICollection<Author> Authors { get; set; } = new List<Author>();

        [Required]
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
