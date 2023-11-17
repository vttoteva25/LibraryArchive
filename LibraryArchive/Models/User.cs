using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryArchive.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }      

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [ForeignKey("Role Id")]
        public int? RoleId { get; set; }
        public Role Role { get; set; }

        public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
    }
}
