using Indotalent.Infrastructures.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.LogAnalytics
{
    [Authorize]
    public class LogAnalyticListModel : PageModel
    {
        public LogAnalyticListModel() { }

        public void OnGet()
        {
            this.SetupViewDataTitleFromUrl();
        }

    }
}
