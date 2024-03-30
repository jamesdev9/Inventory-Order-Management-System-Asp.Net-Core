using Indotalent.Applications.ApplicationUsers;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class ApplicationUserController : ODataController
    {
        private readonly ApplicationUserService _applicationUserService;

        public ApplicationUserController(ApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }

        [EnableQuery]
        public IQueryable<ApplicationUserDto> Get()
        {
            return _applicationUserService
                .GetAll()
                .Include(x => x.SelectedCompany)
                .Select(rec => new ApplicationUserDto
                {
                    Id = rec.Id,
                    FullName = rec.FullName,
                    JobTitle = rec.JobTitle,
                    Address = rec.Address,
                    City = rec.City,
                    State = rec.State,
                    Country = rec.Country,
                    ZipCode = rec.ZipCode,
                    Avatar = rec.Avatar,
                    UserType = rec.UserType.ToString(),
                    IsDefaultAdmin = rec.IsDefaultAdmin,
                    IsOnline = rec.IsOnline,
                    SelectedCompany = rec.SelectedCompany!.Name,
                    CreatedAtUtc = rec.CreatedAtUtc,
                    EmailConfirmed = rec.EmailConfirmed
                });
        }


    }
}
