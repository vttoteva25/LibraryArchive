using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryArchive.Models
{
    public class BookGenre
    {
        [ForeignKey("Book Id")]
        public int BookId { get; set; }

        [ForeignKey("Genre Id")]
        public int GenreId { get; set; }

        public Book Book { get; set; }
        public Genre Genre { get; set; }
    }
}
