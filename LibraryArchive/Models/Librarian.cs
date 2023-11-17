using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.Models
{
    public class Librarian : User
    {
        [Required(ErrorMessage = "Field cannot be empty!")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Field cannot be empty!")]
        public string Password { get; set; }
    }
}
