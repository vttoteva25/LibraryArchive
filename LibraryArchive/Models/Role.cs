using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibraryArchive.Models
{
    public class Role
    {

        [Key]
        public int Id { get; set; }

        [DisplayName("Role name")]
        [Required(ErrorMessage = "Field cannot be empty!")]
        public string RoleName { get; set; }
    }
}
