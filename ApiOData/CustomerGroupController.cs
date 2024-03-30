using Indotalent.Applications.CustomerGroups;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{
    public class CustomerGroupController : ODataController
    {
        private readonly CustomerGroupService _customerGroupService;

        public CustomerGroupController(CustomerGroupService customerGroupService)
        {
            _customerGroupService = customerGroupService;
        }

        [EnableQuery]
        public IQueryable<CustomerGroupDto> Get()
        {
            return _customerGroupService
                .GetAll()
                .Select(rec => new CustomerGroupDto
                {
                    Id = rec.Id,
                    RowGuid = rec.RowGuid,
                    Name = rec.Name,
                    Description = rec.Description,
                    CreatedAtUtc = rec.CreatedAtUtc
                });
        }


    }
}
