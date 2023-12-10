using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.ViewModels.GenreViewModel
{
    public class AddGenreViewModel
    {
        [Required(ErrorMessage = "Полето не може да е празно")]
        public string GenreName { get; set; }
    }
}
