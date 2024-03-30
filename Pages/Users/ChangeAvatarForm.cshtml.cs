using AutoMapper;
using Indotalent.Applications.ApplicationUsers;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Infrastructures.Images;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.Users
{
    [Authorize]
    public class ChangeAvatarFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly IFileImageService _fileImageService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationUserService _applicationUserService;
        public ChangeAvatarFormModel(
            IMapper mapper,
            IFileImageService fileImageService,
            UserManager<ApplicationUser> userManager,
            ApplicationUserService applicationUserService
            )
        {
            _mapper = mapper;
            _fileImageService = fileImageService;
            _userManager = userManager;
            _applicationUserService = applicationUserService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public UserModel UserForm { get; set; } = default!;

        public class UserModel
        {
            public string Id { get; set; } = string.Empty;
            public IFormFile File { get; set; } = default!;
        }

        public string LogoImageUrl { get; set; } = string.Empty;

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

                LogoImageUrl = await _fileImageService.GetImageUrlFromImageIdAsync(existing.Avatar ?? Guid.Empty.ToString());
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

                if (input.File == null || input.File.Length == 0)
                {
                    throw new Exception("Please select a file.");
                }

                var ava = Guid.Empty.ToString();

                if (!String.IsNullOrEmpty(existing.Avatar))
                {
                    ava = existing.Avatar;
                }

                var existingAvatar = await _fileImageService.GetImageAsync(new Guid(ava));

                if (existingAvatar.Id == Guid.Empty)
                {

                    var avatarId = await _fileImageService.UploadImageAsync(input.File);

                    existing.Avatar = avatarId.ToString();

                    var updateResult = await _userManager.UpdateAsync(existing);

                    if (!updateResult.Succeeded)
                    {
                        throw new Exception("Upload avatar failed");
                    }

                }
                else
                {

                    await _fileImageService.UpdateImageAsync(existingAvatar.Id, input.File);

                }


                this.WriteStatusMessage($"Success update existing avatar.");
                return Redirect($"./ChangeAvatarForm?id={existing.Id}&action=edit");
            }

            return Page();
        }

    }
}
