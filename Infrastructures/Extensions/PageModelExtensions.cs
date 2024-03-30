using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Infrastructures.Extensions
{
    public static class PageModelExtensions
    {

        public static string ExtractGlobalErrorMessage(this PageModel pageModel)
        {
            var errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(pageModel.HttpContext.Session.GetString("ErrorMessage")))
            {
                errorMessage = "Error: " + pageModel.HttpContext.Session.GetString("ErrorMessage");
                pageModel.HttpContext.Session.Remove("ErrorMessage");
            }
            return errorMessage;
        }

        public static void SetupStatusMessage(this PageModel pageModel)
        {
            var errorMessage = pageModel.ExtractGlobalErrorMessage();
            var existingMessage = pageModel.ReadStatusMessage();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                pageModel.WriteStatusMessage(errorMessage);
            }
            else
            {
                pageModel.WriteStatusMessage(existingMessage);
            }
        }

        public static void WriteStatusMessage(this PageModel pageModel, string message)
        {
            pageModel.TempData["StatusMessage"] = message;
        }

        public static string ReadStatusMessage(this PageModel pageModel)
        {
            var result = string.Empty;
            if (pageModel.TempData["StatusMessage"] != null)
            {
                result = pageModel.TempData["StatusMessage"] as string ?? string.Empty;
                pageModel.TempData.Remove("StatusMessage");
            }
            return result;
        }

        public static void SetupViewDataTitleFromUrl(this PageModel pageModel)
        {
            var currentUrl = pageModel.HttpContext.GetCurrentUrl();
            var currentCshtml = currentUrl.ToCshtmlName();
            var currentTitle = currentCshtml.ToTitle();
            pageModel.ViewData["Title"] = currentTitle;

            var currentPageName = currentUrl.ToPageFolderName();
            if (currentPageName.EndsWith("y"))
            {
                currentPageName = currentPageName.Substring(0, currentPageName.Length - 1) + "ies";
            }
            pageModel.ViewData["PageFolderName"] = currentPageName;
            pageModel.ViewData["CshtmlName"] = currentCshtml;
        }
    }
}
