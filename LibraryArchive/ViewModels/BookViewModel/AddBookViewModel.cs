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
        public List<string> BookAuthorsIds { get; set; }

        [Required(ErrorMessage = "Изберете издателство")]
        public string PublisherId { get; set; }

        [Required(ErrorMessage = "Изберете поне един жанр")]
        public List<string> BookGenresIds { get; set; }
        public string PublicationYear { get; set; }
        public string Language {  get; set; }
        public bool Scrapped { get; set; } = false;
        public bool Available { get; set; } = false;
    }
}
