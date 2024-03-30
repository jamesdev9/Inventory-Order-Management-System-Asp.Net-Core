using Indotalent.Applications.LogErrors;

namespace Indotalent.Infrastructures.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, LogErrorService logErrorService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await CollectAndHandleErrorAsync(context, ex, logErrorService);
            }
        }

        private async Task CollectAndHandleErrorAsync(HttpContext context, Exception ex, LogErrorService logErrorService)
        {
            string? errorMessage = ex.Message;
            string? stackTrace = ex.StackTrace;
            string? source = ex.InnerException?.Message;

            var redirectUrl = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.QueryString.ToString()))
            {
                redirectUrl += context.Request.Path.ToString();
                redirectUrl += context.Request.QueryString.ToString();
            }
            else
            {
                redirectUrl += "/Error";
            }


            await logErrorService.CollectErrorDataAsync(errorMessage, stackTrace, source);
            context.Session.SetString("ErrorMessage", errorMessage);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.Redirect(redirectUrl);
        }
    }
}
