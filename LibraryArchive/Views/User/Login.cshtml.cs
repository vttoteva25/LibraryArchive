using LibraryArchive.ViewModels.HomeViewModel;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryArchive.Pages.User
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginLibrarianViewModel> _logger;

        public LoginModel(ILogger<LoginLibrarianViewModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
        }
    }
}
