using Indotalent.Applications.Companies;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{
    public class CompanyController : ODataController
    {
        private readonly CompanyService _companyService;

        public CompanyController(CompanyService companyService)
        {
            _companyService = companyService;
        }

        [EnableQuery]
        public IQueryable<CompanyDto> Get()
        {
            return _companyService
                .GetAll()
                .Select(rec => new CompanyDto
                {
                    Id = rec.Id,
                    Name = rec.Name,
                    Currency = rec.Currency,
                    TimeZone = rec.TimeZone,
                    Description = rec.Description,
                    Street = rec.Street,
                    City = rec.City,
                    State = rec.State,
                    ZipCode = rec.ZipCode,
                    Country = rec.Country,
                    PhoneNumber = rec.PhoneNumber,
                    FaxNumber = rec.FaxNumber,
                    EmailAddress = rec.EmailAddress,
                    Website = rec.Website,
                    WhatsApp = rec.WhatsApp,
                    LinkedIn = rec.LinkedIn,
                    Facebook = rec.Facebook,
                    Instagram = rec.Instagram,
                    TwitterX = rec.TwitterX,
                    TikTok = rec.TikTok,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc
                });
        }


    }
}
