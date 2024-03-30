using AutoMapper;
using Indotalent.Applications.ApplicationUsers;
using Indotalent.Applications.LogErrors;
using Indotalent.AppSettings;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Indotalent.Pages.UserProfiles
{
    [Authorize]
    public class ChangePasswordFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationUserService _applicationUserService;
        private readonly ApplicationConfiguration _appConfig;
        public ChangePasswordFormModel(
            IMapper mapper,
            ILogger<UserFormModel> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationUserService applicationUserService,
            LogErrorService logErrorService,
            IOptions<ApplicationConfiguration> appConfig
            )
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationUserService = applicationUserService;
            _appConfig = appConfig.Value;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public UserModel UserForm { get; set; } = default!;

        public class UserModel
        {
            public string Id { get; set; } = Guid.Empty.ToString();

            [DisplayName("Old Password")]
            [DataType(DataType.Password)]
            public string OldPassword { get; set; } = string.Empty;


            [DisplayName("New Password")]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; } = string.Empty;


            [DisplayName("Confirm Password")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ApplicationUser, UserModel>();
                CreateMap<UserModel, ApplicationUser>();
            }
        }

        public async Task OnGetAsync(string? id)
        {
            this.SetupViewDataTitleFromUrl();
            this.SetupStatusMessage();
            StatusMessage = this.ReadStatusMessage();

            UserForm = new UserModel
            {
                Id = Guid.Empty.ToString()
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

            if (action == "edit")
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

                if (input.NewPassword != input.ConfirmPassword)
                {
                    throw new Exception("New password did not match confirm password");
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(existing, input.OldPassword, input.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                    throw new Exception(message);
                }


                await _signInManager.RefreshSignInAsync(existing);

                this.WriteStatusMessage($"Success update existing password.");
                return Redirect($"./ChangePasswordForm?id={existing.Id}&action=edit");
            }

            return Page();
        }

    }
}
