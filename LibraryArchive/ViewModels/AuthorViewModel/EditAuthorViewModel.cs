namespace LibraryArchive.ViewModels.AuthorViewModel
{
    public class EditAuthorViewModel
    {
        public string AuthorId { get; set; }
      
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public string? Biography { get; set; }
    }
}
