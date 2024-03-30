using AutoMapper;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.VendorCategories;
using Indotalent.Applications.VendorGroups;
using Indotalent.Applications.Vendors;
using Indotalent.Infrastructures.Countries;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Indotalent.Pages.Vendors
{
    [Authorize]
    public class VendorFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly VendorService _vendorService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly VendorGroupService _vendorGroupService;
        private readonly VendorCategoryService _vendorCategoryService;
        private readonly ICountryService _countrySevice;
        public VendorFormModel(
            IMapper mapper,
            VendorService vendorService,
            NumberSequenceService numberSequenceService,
            VendorGroupService vendorGroupService,
            VendorCategoryService vendorCategoryService,
            ICountryService countrySevice
            )
        {
            _mapper = mapper;
            _vendorService = vendorService;
            _numberSequenceService = numberSequenceService;
            _vendorCategoryService = vendorCategoryService;
            _vendorGroupService = vendorGroupService;
            _countrySevice = countrySevice;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public VendorModel VendorForm { get; set; } = default!;

        public class VendorModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Name")]
            public string Name { get; set; } = string.Empty;

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("Group")]
            public int VendorGroupId { get; set; }

            [DisplayName("Category")]
            public int VendorCategoryId { get; set; }

            [DisplayName("Street")]
            public string? Street { get; set; }

            [DisplayName("City")]
            public string? City { get; set; }

            [DisplayName("State")]
            public string? State { get; set; }

            [DisplayName("Zip Code")]
            public string? ZipCode { get; set; }

            [DisplayName("Country")]
            public string? Country { get; set; }

            [DisplayName("Phone Number")]
            public string? PhoneNumber { get; set; }

            [DisplayName("Fax Number")]
            public string? FaxNumber { get; set; }

            [DisplayName("Email Address")]
            public string? EmailAddress { get; set; }

            [DisplayName("Website")]
            public string? Website { get; set; }

            [DisplayName("WhatsApp")]
            public string? WhatsApp { get; set; }

            [DisplayName("LinkedIn")]
            public string? LinkedIn { get; set; }

            [DisplayName("Facebook")]
            public string? Facebook { get; set; }

            [DisplayName("Instagram")]
            public string? Instagram { get; set; }

            [DisplayName("TwitterX")]
            public string? TwitterX { get; set; }

            [DisplayName("TikTok")]
            public string? TikTok { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Vendor, VendorModel>();
                CreateMap<VendorModel, Vendor>();
            }
        }

        public ICollection<SelectListItem> VendorGroupLookup { get; set; } = default!;
        public ICollection<SelectListItem> VendorCategoryLookup { get; set; } = default!;
        public ICollection<SelectListItem> CountryLookup { get; set; } = default!;
        private void BindLookup()
        {

            VendorGroupLookup = _vendorGroupService.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name}"
            }).ToList();

            VendorCategoryLookup = _vendorCategoryService.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name}"
            }).ToList();

            CountryLookup = _countrySevice.GetCountries();

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
                var existing = await _vendorService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                VendorForm = _mapper.Map<VendorModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                VendorForm = new VendorModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(VendorForm))] VendorModel input)
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
                var newobj = _mapper.Map<Vendor>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND");
                newobj.Number = Number;

                await _vendorService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./VendorForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _vendorService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _vendorService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./VendorForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _vendorService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _vendorService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./VendorList");
            }
            return Page();
        }

    }
}
