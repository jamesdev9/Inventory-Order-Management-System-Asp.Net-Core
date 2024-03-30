namespace Indotalent.Infrastructures.Pdfs
{
    public interface IPdfService
    {
        byte[] CreatePdfFromHtml(string htmlContent, string documentTitle);
        byte[] CreatePdfFromPage(string pageUrl, string documentTitle);

    }
}
