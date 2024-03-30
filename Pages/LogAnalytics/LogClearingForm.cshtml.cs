using Indotalent.Applications.LogAnalytics;
using Indotalent.Infrastructures.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.LogAnalytics
{
    [Authorize]
    public class LogClearingFormModel : PageModel
    {
        private readonly LogAnalyticService _logAnalyticService;
        public LogClearingFormModel(
            LogAnalyticService logAnalyticService
            )
        {
            _logAnalyticService = logAnalyticService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            this.SetupViewDataTitleFromUrl();
            this.SetupStatusMessage();
            StatusMessage = this.ReadStatusMessage();
        }

        public IActionResult OnPost()
        {

            var action = "create";

            if (!string.IsNullOrEmpty(Request.Query["action"]))
            {
                action = Request.Query["action"];
            }

            if (action == "purge")
            {
                _logAnalyticService.PurgeAllData();

                this.WriteStatusMessage($"Success purge all data.");
                return RedirectToPage();
            }

            return Page();
        }

    }
}
