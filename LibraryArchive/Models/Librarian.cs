using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.Models
{
    public class Librarian : User
    {
        [StringLength(50)]
        public string? Username { get; set; }

        public string? Password { get; set; }

    }
}
