using Indotalent.Applications.VendorContacts;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class VendorContactController : ODataController
    {
        private readonly VendorContactService _vendorContactService;

        public VendorContactController(VendorContactService vendorContactService)
        {
            _vendorContactService = vendorContactService;
        }

        [EnableQuery]
        public IQueryable<VendorContactDto> Get()
        {
            return _vendorContactService
                .GetAll()
                .Include(x => x.Vendor)
                .Select(rec => new VendorContactDto
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
                    Vendor = rec.Vendor!.Name,
                });
        }


    }
}
