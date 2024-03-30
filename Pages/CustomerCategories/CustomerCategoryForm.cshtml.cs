using AutoMapper;
using Indotalent.Applications.CustomerCategories;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;

namespace Indotalent.Pages.CustomerCategories
{
    [Authorize]
    public class CustomerCategoryFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly CustomerCategoryService _customerCategoryService;
        public CustomerCategoryFormModel(
            IMapper mapper,
            CustomerCategoryService customerCategoryService
            )
        {
            _mapper = mapper;
            _customerCategoryService = customerCategoryService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;

        [BindProperty]
        public CustomerCategoryModel CustomerCategoryForm { get; set; } = default!;

        public class CustomerCategoryModel
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
                CreateMap<CustomerCategory, CustomerCategoryModel>();
                CreateMap<CustomerCategoryModel, CustomerCategory>();
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
                var existing = await _customerCategoryService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                CustomerCategoryForm = _mapper.Map<CustomerCategoryModel>(existing);
            }
            else
            {
                CustomerCategoryForm = new CustomerCategoryModel
                {
                    RowGuid = Guid.Empty
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(CustomerCategoryForm))] CustomerCategoryModel input)
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
                var newobj = _mapper.Map<CustomerCategory>(input);
                await _customerCategoryService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./CustomerCategoryForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _customerCategoryService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _customerCategoryService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./CustomerCategoryForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _customerCategoryService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _customerCategoryService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./CustomerCategoryList");
            }
            return Page();
        }

    }
}
