using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.ViewModels.BorrowViewModel
{
    public class BorrowBookViewModel
    {
        [Required(ErrorMessage = "Полето не може да е празно")]
        public string BookId { get; set; }
    }
}
