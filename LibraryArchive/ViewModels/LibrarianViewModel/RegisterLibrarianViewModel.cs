using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.ViewModels.LibrarianViewModel
{
    public class RegisterLibrarianViewModel
    {
        [Required(ErrorMessage = "Полето не може да е празно")]
        public string LibrarianId { get; set; }

        [Required(ErrorMessage = "Полето не може да е празно")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Полето не може да е празно")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Полето не може да е празно")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Полето не може да е празно")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Полето не може да е празно")]
        public string ConfirmPassword { get; set; }

        public string Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
