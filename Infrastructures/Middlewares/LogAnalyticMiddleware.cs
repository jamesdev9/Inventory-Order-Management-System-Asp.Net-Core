using Indotalent.Applications.LogAnalytics;

namespace Indotalent.Infrastructures.Middlewares
{
    public class LogAnalyticMiddleware
    {
        private readonly RequestDelegate _next;

        public LogAnalyticMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, LogAnalyticService logAnalyticService)
        {
            if (IsRazorPage(context))
            {
                await logAnalyticService.CollectAnalyticDataAsync();
            }
            await _next(context);
        }

        private bool IsRazorPage(HttpContext context)
        {
            bool hasExtension = !Path.HasExtension(context.Request.Path);
            bool hasHandler = context.Request.QueryString.HasValue && context.Request.QueryString.Value.Contains("handler");
            return hasExtension && !hasHandler;
        }
    }
}
