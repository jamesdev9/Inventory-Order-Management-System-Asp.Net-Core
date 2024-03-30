using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace Indotalent.Infrastructures.Pdfs
{
    public class PdfService : IPdfService
    {

        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] CreatePdfFromHtml(string htmlContent, string documentTitle)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                //Margins = new MarginSettings
                //{
                //    Top = 10,
                //    Left = 0,
                //    Right = 0,
                //},
                DocumentTitle = documentTitle,
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" },
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);

            return file;
        }


        public byte[] CreatePdfFromPage(string pageUrl, string documentTitle)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                //Margins = new MarginSettings
                //{
                //    Top = 10,
                //    Left = 0,
                //    Right = 0,
                //},
                DocumentTitle = documentTitle,
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                Page = pageUrl,
                WebSettings = { DefaultEncoding = "utf-8" },
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);

            return file;
        }

    }
}
