using AutoMapper;
using Indotalent.Applications.CustomerContacts;
using Indotalent.Applications.Customers;
using Indotalent.Applications.NumberSequences;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Indotalent.Pages.CustomerContacts
{
    [Authorize]
    public class CustomerContactFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly CustomerContactService _customerContactService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly CustomerService _customerService;
        public CustomerContactFormModel(
            IMapper mapper,
            CustomerContactService customerContactService,
            NumberSequenceService numberSequenceService,
            CustomerService customerService
            )
        {
            _mapper = mapper;
            _customerContactService = customerContactService;
            _numberSequenceService = numberSequenceService;
            _customerService = customerService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public CustomerContactModel CustomerContactForm { get; set; } = default!;

        public class CustomerContactModel
        {
            public Guid? RowGuid { get; set; }

            [DisplayName("Name")]
            public string Name { get; set; } = string.Empty;

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("Customer")]
            public int CustomerId { get; set; }

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
                CreateMap<CustomerContact, CustomerContactModel>();
                CreateMap<CustomerContactModel, CustomerContact>();
            }
        }

        public ICollection<SelectListItem> CustomerLookup { get; set; } = default!;
        private void BindLookup()
        {

            CustomerLookup = _customerService.GetAll().Select(x => new SelectListItem
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
                var existing = await _customerContactService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                CustomerContactForm = _mapper.Map<CustomerContactModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                CustomerContactForm = new CustomerContactModel
                {
                    RowGuid = Guid.Empty
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(CustomerContactForm))] CustomerContactModel input)
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
                var newobj = _mapper.Map<CustomerContact>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(CustomerContact), "", "CC");
                newobj.Number = Number;

                await _customerContactService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./CustomerContactForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _customerContactService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _customerContactService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./CustomerContactForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _customerContactService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _customerContactService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./CustomerContactList");
            }
            return Page();
        }

    }
}
