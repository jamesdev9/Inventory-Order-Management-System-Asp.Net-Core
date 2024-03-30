using Indotalent.Infrastructures.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.Users
{
    [Authorize]
    public class UserListModel : PageModel
    {
        public UserListModel() { }

        public void OnGet()
        {
            this.SetupViewDataTitleFromUrl();
        }





    }
}
