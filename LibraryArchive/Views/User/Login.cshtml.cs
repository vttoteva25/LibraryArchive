using LibraryArchive.ViewModels.HomeViewModel;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryArchive.Pages.User
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginViewModel> _logger;

        public LoginModel(ILogger<LoginViewModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
        }
    }
}
