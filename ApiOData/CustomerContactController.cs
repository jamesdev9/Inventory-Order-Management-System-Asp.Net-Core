using Indotalent.Applications.CustomerContacts;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class CustomerContactController : ODataController
    {
        private readonly CustomerContactService _customerContactService;

        public CustomerContactController(CustomerContactService customerContactService)
        {
            _customerContactService = customerContactService;
        }

        [EnableQuery]
        public IQueryable<CustomerContactDto> Get()
        {
            return _customerContactService
                .GetAll()
                .Include(x => x.Customer)
                .Select(rec => new CustomerContactDto
                {
                    Id = rec.Id,
                    Name = rec.Name,
                    Number = rec.Number,
                    JobTitle = rec.JobTitle,
                    Description = rec.Description,
                    PhoneNumber = rec.PhoneNumber,
                    EmailAddress = rec.EmailAddress,
                    RowGuid = rec.RowGuid,
                    CreatedAtUtc = rec.CreatedAtUtc,
                    Customer = rec.Customer!.Name,
                });
        }


    }
}
