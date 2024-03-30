using Indotalent.Applications.ApplicationUsers;
using Indotalent.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{
    public class UserProfileController : ODataController
    {
        private readonly ApplicationUserService _applicationUserService;

        public UserProfileController(ApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }

        [EnableQuery]
        public IQueryable<UserProfileDto> Get()
        {

            const string HeaderKeyName = "CurrentUserId";
            Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
            var currentUserId = headerValue.ToString();

            var result = _applicationUserService
                .GetAll()
                .Include(x => x.SelectedCompany)
                .Where(x => x.Id == currentUserId)
                .Select(rec => new UserProfileDto
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
                });

            return result;
        }


    }
}
