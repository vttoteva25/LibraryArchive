using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.Models
{
    public class Reader : User
    {
        [Required]
        public string ReaderNumber { get; set; }
    }
}
