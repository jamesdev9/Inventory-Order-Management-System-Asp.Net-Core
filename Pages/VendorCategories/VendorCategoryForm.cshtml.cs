using AutoMapper;
using Indotalent.Applications.VendorCategories;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;

namespace Indotalent.Pages.VendorCategories
{
    [Authorize]
    public class VendorCategoryFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly VendorCategoryService _vendorCategoryService;
        public VendorCategoryFormModel(
            IMapper mapper,
            VendorCategoryService vendorCategoryService
            )
        {
            _mapper = mapper;
            _vendorCategoryService = vendorCategoryService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;

        [BindProperty]
        public VendorCategoryModel VendorCategoryForm { get; set; } = default!;

        public class VendorCategoryModel
        {
            public Guid? RowGuid { get; set; }

            [DisplayName("Name")]
            public string Name { get; set; } = string.Empty;

            [DisplayName("Description")]
            public string? Description { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<VendorCategory, VendorCategoryModel>();
                CreateMap<VendorCategoryModel, VendorCategory>();
            }
        }

        private void BindLookup()
        {

        }

        public async Task OnGetAsync(Guid? rowGuid)
        {

            this.SetupViewDataTitleFromUrl();
            this.SetupStatusMessage();
            StatusMessage = this.ReadStatusMessage();

            var action = Request.Query["action"];
            Action = action;

            BindLookup();

            if (rowGuid.HasValue)
            {
                var existing = await _vendorCategoryService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                VendorCategoryForm = _mapper.Map<VendorCategoryModel>(existing);
            }
            else
            {
                VendorCategoryForm = new VendorCategoryModel
                {
                    RowGuid = Guid.Empty
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(VendorCategoryForm))] VendorCategoryModel input)
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
                var newobj = _mapper.Map<VendorCategory>(input);
                await _vendorCategoryService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./VendorCategoryForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _vendorCategoryService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _vendorCategoryService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./VendorCategoryForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _vendorCategoryService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _vendorCategoryService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./VendorCategoryList");
            }
            return Page();
        }

    }
}
