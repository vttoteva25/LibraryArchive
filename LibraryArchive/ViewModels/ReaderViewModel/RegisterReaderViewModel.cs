using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.ViewModels.ReaderViewModel
{
    public class RegisterReaderViewModel
    {

        [Required(ErrorMessage = "Полето не може да е празно")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Полето не може да е празно")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Полето не може да е празно")]
        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

    }
}
