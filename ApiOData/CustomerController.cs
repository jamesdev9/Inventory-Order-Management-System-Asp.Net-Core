using Indotalent.Applications.Customers;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class CustomerController : ODataController
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [EnableQuery]
        public IQueryable<CustomerDto> Get()
        {
            return _customerService
                .GetAll()
                .Include(x => x.CustomerGroup)
                .Include(x => x.CustomerCategory)
                .Select(rec => new CustomerDto
                {
                    Id = rec.Id,
                    Name = rec.Name,
                    Number = rec.Number,
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
                    CreatedAtUtc = rec.CreatedAtUtc,
                    CustomerGroup = rec.CustomerGroup!.Name,
                    CustomerCategory = rec.CustomerCategory!.Name,
                });
        }


    }
}
