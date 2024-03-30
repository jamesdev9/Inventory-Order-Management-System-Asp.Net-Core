using AutoMapper;
using Indotalent.Applications.ApplicationUsers;
using Indotalent.Applications.Companies;
using Indotalent.AppSettings;
using Indotalent.Infrastructures.Countries;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Security.Claims;
using System.Text;

namespace Indotalent.Pages.Users
{
    [Authorize]
    public class UserFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly ApplicationUserService _applicationUserService;
        private readonly CompanyService _companyService;
        private readonly ICountryService _countrySevice;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationConfiguration _appConfig;
        public UserFormModel(
            IMapper mapper,
            ApplicationUserService applicationUserService,
            CompanyService companyService,
            ICountryService countrySevice,
            UserManager<ApplicationUser> userManager,
            IOptions<ApplicationConfiguration> appConfig
            )
        {
            _mapper = mapper;
            _applicationUserService = applicationUserService;
            _companyService = companyService;
            _countrySevice = countrySevice;
            _userManager = userManager;
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

            [DisplayName("User Type")]
            public UserType UserType { get; set; } = UserType.Internal;

            [DisplayName("Is Default Admin")]
            public bool IsDefaultAdmin { get; set; } = false;

            [DisplayName("Selected Company")]
            public int SelectedCompanyId { get; set; }

            [DisplayName("Password")]
            public string Password { get; set; } = string.Empty;

            [DisplayName("Confirm Password")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [DisplayName("Email")]
            public string Email { get; set; } = string.Empty;

            [DisplayName("Email Confirmed")]
            public bool EmailConfirmed { get; set; } = false;
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
                Id = Guid.NewGuid().ToString()
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
                if (input.Password == string.Empty || input.ConfirmPassword == string.Empty)
                {
                    throw new Exception("The password or confirmation password must not empty.");

                }

                if (input.Password != input.ConfirmPassword)
                {
                    throw new Exception("The password and confirmation password do not match");
                }

                var alreadyUsedEmail = _applicationUserService.IsEmailAlreadyExist(input.Email);

                if (alreadyUsedEmail)
                {
                    throw new Exception("Email already used");
                }

                var newobj = _mapper.Map<ApplicationUser>(input);
                newobj.UserName = newobj.Email;
                newobj.CreatedAtUtc = DateTime.UtcNow;

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                newobj.CreatedByUserId = userId;

                var password = input.Password;
                var createUserResult = await _userManager.CreateAsync(newobj, password ?? string.Empty);
                if (!createUserResult.Succeeded)
                {
                    throw new Exception("Creating new user not success.");
                }
                if (newobj.UserType == UserType.Internal)
                {
                    await _userManager.AddToRoleAsync(newobj, _appConfig.RoleInternalName ?? string.Empty);
                }
                if (newobj.UserType == UserType.Customer)
                {
                    await _userManager.AddToRoleAsync(newobj, _appConfig.RoleCustomerName ?? string.Empty);
                }
                if (newobj.UserType == UserType.Vendor)
                {
                    await _userManager.AddToRoleAsync(newobj, _appConfig.RoleVendorName ?? string.Empty);
                }

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

                if (input.Password != input.ConfirmPassword)
                {
                    throw new Exception("The password and confirmation password do not match");
                }

                var newEmail = input.Email;
                var oldEmail = existing.Email;

                if (newEmail != oldEmail)
                {
                    var alreadyUsedEmail = _applicationUserService.IsEmailAlreadyExist(newEmail);

                    if (alreadyUsedEmail)
                    {
                        throw new Exception("Email already used");
                    }

                    var code = await _userManager.GenerateChangeEmailTokenAsync(existing, newEmail);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                    var result = await _userManager.ChangeEmailAsync(existing, newEmail, code);
                    if (!result.Succeeded)
                    {
                        var message = $"Changing email not success.";
                        throw new Exception(message);
                    }

                    var setUserNameResult = await _userManager.SetUserNameAsync(existing, newEmail);
                    if (!setUserNameResult.Succeeded)
                    {
                        var message = $"Changing username not success.";
                        throw new Exception(message);
                    }
                }

                var newUserType = input.UserType;
                var oldUserType = existing.UserType;
                if (newUserType != oldUserType)
                {
                    var deleteRoles = await _userManager.GetRolesAsync(existing);
                    if (deleteRoles.Any())
                    {
                        var result = await _userManager.RemoveFromRolesAsync(existing, deleteRoles);
                        if (!result.Succeeded)
                        {
                            throw new Exception("Reset roles not success.");
                        }
                    }
                    if (newUserType == UserType.Internal)
                    {
                        await _userManager.AddToRoleAsync(existing, _appConfig.RoleInternalName ?? string.Empty);
                    }
                    if (newUserType == UserType.Customer)
                    {
                        await _userManager.AddToRoleAsync(existing, _appConfig.RoleCustomerName ?? string.Empty);
                    }
                    if (newUserType == UserType.Vendor)
                    {
                        await _userManager.AddToRoleAsync(existing, _appConfig.RoleVendorName ?? string.Empty);
                    }
                }

                _mapper.Map(input, existing);
                existing.UserName = existing.Email;
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
