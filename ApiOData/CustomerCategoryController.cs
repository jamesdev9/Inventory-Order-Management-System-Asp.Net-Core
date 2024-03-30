using Indotalent.Applications.CustomerCategories;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{
    public class CustomerCategoryController : ODataController
    {
        private readonly CustomerCategoryService _customerCategoryService;

        public CustomerCategoryController(CustomerCategoryService customerCategoryService)
        {
            _customerCategoryService = customerCategoryService;
        }

        [EnableQuery]
        public IQueryable<CustomerCategoryDto> Get()
        {
            return _customerCategoryService
                .GetAll()
                .Select(rec => new CustomerCategoryDto
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
