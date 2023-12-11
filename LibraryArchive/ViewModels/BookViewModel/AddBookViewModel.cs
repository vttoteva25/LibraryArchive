using LibraryArchive.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.ViewModels.BookViewModel
{
    public class AddBookViewModel
    {
        [Required(ErrorMessage = "Библиотечният номер на книгата не може да бъде празен")]
        public string BookId { get; set; }

        [Required(ErrorMessage = "Заглавието не може да бъде празно")]
        public string Title { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Изберете поне един автор")]
        public List<string> Author { get; set; }
        public List<Author> Authors { get; set; }

        [Required(ErrorMessage = "Изберете издателство")]
        public string Publisher { get; set; }
        public List<Publisher> PublisherList { get; set; }

        [Required(ErrorMessage = "Изберете поне един жанр")]
        public List<string> BookGenresIds { get; set; }
        public List<Genre> Genres { get; set; }
        public string PublicationYear { get; set; }
        public string Language {  get; set; }
        public bool Scrapped { get; set; } = false;
        public bool Availability { get; set; } = false;
    }
}
