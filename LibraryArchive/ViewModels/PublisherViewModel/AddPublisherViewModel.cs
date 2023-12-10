using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.ViewModels.PublisherViewModel
{
    public class AddPublisherViewModel
    {
        [Required]
        public string PublisherName { get; set; }

        public string? Address { get; set; }

        public string? Website { get; set; }
    }
}
