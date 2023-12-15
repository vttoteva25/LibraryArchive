using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.ViewModels.AuthorViewModel
{
    public class AddAuthorViewModel
    {
        [Required(ErrorMessage = "Полето не може да е празно")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Полето не може да е празно")]
        public DateTime BirthDate { get; set; }

        public DateTime? DeathDate { get; set; }

        [Required(ErrorMessage = "Полето не може да е празно")]
        public string? Biography { get; set; }
    }
}
