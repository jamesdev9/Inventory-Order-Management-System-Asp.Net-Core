using AutoMapper;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.VendorContacts;
using Indotalent.Applications.Vendors;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Indotalent.Pages.VendorContacts
{
    [Authorize]
    public class VendorContactFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly VendorContactService _vendorContactService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly VendorService _vendorService;
        public VendorContactFormModel(
            IMapper mapper,
            VendorContactService vendorContactService,
            NumberSequenceService numberSequenceService,
            VendorService vendorService
            )
        {
            _mapper = mapper;
            _vendorContactService = vendorContactService;
            _numberSequenceService = numberSequenceService;
            _vendorService = vendorService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public VendorContactModel VendorContactForm { get; set; } = default!;

        public class VendorContactModel
        {
            public Guid? RowGuid { get; set; }

            [DisplayName("Name")]
            public string Name { get; set; } = string.Empty;

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("Vendor")]
            public int VendorId { get; set; }

            [DisplayName("Phone Number")]
            public string? PhoneNumber { get; set; }

            [DisplayName("Email Address")]
            public string? EmailAddress { get; set; }

            [DisplayName("Job Title")]
            public string? JobTitle { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<VendorContact, VendorContactModel>();
                CreateMap<VendorContactModel, VendorContact>();
            }
        }

        public ICollection<SelectListItem> VendorLookup { get; set; } = default!;
        private void BindLookup()
        {

            VendorLookup = _vendorService.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name}"
            }).ToList();
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
                var existing = await _vendorContactService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                VendorContactForm = _mapper.Map<VendorContactModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                VendorContactForm = new VendorContactModel
                {
                    RowGuid = Guid.Empty
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(VendorContactForm))] VendorContactModel input)
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
                var newobj = _mapper.Map<VendorContact>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(VendorContact), "", "VC");
                newobj.Number = Number;

                await _vendorContactService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./VendorContactForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _vendorContactService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _vendorContactService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./VendorContactForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _vendorContactService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _vendorContactService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./VendorContactList");
            }
            return Page();
        }

    }
}
