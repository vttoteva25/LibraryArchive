using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryArchive.Models
{
    public class User
    {
        [Key]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "UserId must be exactly 10 characters.")]
        public string UserId { get; set; } //егн   

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [ForeignKey("Role Id")]
        public string? RoleId { get; set; }
        public Role Role { get; set; }

        public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();

        public ICollection<Book> Books { get; set; }
    }
}
