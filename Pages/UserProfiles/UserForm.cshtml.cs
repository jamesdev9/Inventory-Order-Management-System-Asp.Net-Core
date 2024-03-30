using AutoMapper;
using Indotalent.Applications.ApplicationUsers;
using Indotalent.Applications.Companies;
using Indotalent.AppSettings;
using Indotalent.Infrastructures.Countries;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace Indotalent.Pages.UserProfiles
{
    [Authorize]
    public class UserFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly ApplicationUserService _applicationUserService;
        private readonly CompanyService _companyService;
        private readonly ICountryService _countrySevice;
        private readonly ApplicationConfiguration _appConfig;
        public UserFormModel(
            IMapper mapper,
            ApplicationUserService applicationUserService,
            CompanyService companyService,
            ICountryService countrySevice,
            IOptions<ApplicationConfiguration> appConfig
            )
        {
            _mapper = mapper;
            _applicationUserService = applicationUserService;
            _companyService = companyService;
            _countrySevice = countrySevice;
            _appConfig = appConfig.Value;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;

        [BindProperty]
        public UserModel UserForm { get; set; } = default!;

        public class UserModel
        {
            public string Id { get; set; } = string.Empty;

            [DisplayName("Full Name")]
            public string? FullName { get; set; }

            [DisplayName("Job Title")]
            public string? JobTitle { get; set; }

            [DisplayName("Address")]
            public string? Address { get; set; }

            [DisplayName("City")]
            public string? City { get; set; }

            [DisplayName("State")]
            public string? State { get; set; }

            [DisplayName("Country")]
            public string? Country { get; set; }

            [DisplayName("Zip Code")]
            public string? ZipCode { get; set; }

            [DisplayName("Selected Company")]
            public required string SelectedCompanyId { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ApplicationUser, UserModel>();
                CreateMap<UserModel, ApplicationUser>();
            }
        }

        public ICollection<SelectListItem> CompanyLookup { get; set; } = default!;

        public ICollection<SelectListItem> CountryLookup { get; set; } = default!;

        private void BindLookup()
        {
            var companies = _companyService.GetAll();

            CompanyLookup = companies
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            CountryLookup = _countrySevice.GetCountries();
        }

        public async Task OnGetAsync(string? id)
        {

            this.SetupViewDataTitleFromUrl();
            this.SetupStatusMessage();
            StatusMessage = this.ReadStatusMessage();


            var action = Request.Query["action"];
            Action = action;

            BindLookup();

            UserForm = new UserModel
            {
                Id = Guid.NewGuid().ToString(),
                SelectedCompanyId = Guid.Empty.ToString()
            };

            if (!(string.IsNullOrEmpty(id) || id.Equals(Guid.Empty.ToString())))
            {
                var existing = await _applicationUserService.GetByIdAsync(id);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {id}");
                }
                UserForm = _mapper.Map<UserModel>(existing);
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(UserForm))] UserModel input)
        {

            if (!ModelState.IsValid)
            {
                var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                throw new Exception(message);
            }

            var action = "create";

            if (!string.IsNullOrEmpty(Request.Query["action"]))
            {
                action = Request.Query["action"];
            }

            if (action == "create")
            {
                var newobj = _mapper.Map<ApplicationUser>(input);
                await _applicationUserService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./UserForm?id={newobj.Id}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _applicationUserService.GetByIdAsync(input.Id);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.Id}";
                    throw new Exception(message);
                }

                if (_appConfig.IsDemoVersion == true && existing.FullName == "Administrator")
                {

                    throw new Exception("Modifying an Administrator on the Demo Version is Prohibited");
                }

                _mapper.Map(input, existing);
                await _applicationUserService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./UserForm?id={existing.Id}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _applicationUserService.GetByIdAsync(input.Id);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.Id}";
                    throw new Exception(message);
                }

                if (_appConfig.IsDemoVersion == true && existing.FullName == "Administrator")
                {

                    throw new Exception("Modifying an Administrator on the Demo Version is Prohibited");
                }

                await _applicationUserService.DeleteByIdAsync(input.Id);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./UserList");
            }

            return Page();
        }

    }
}
