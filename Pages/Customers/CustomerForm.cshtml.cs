using AutoMapper;
using Indotalent.Applications.CustomerCategories;
using Indotalent.Applications.CustomerGroups;
using Indotalent.Applications.Customers;
using Indotalent.Applications.NumberSequences;
using Indotalent.Infrastructures.Countries;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Indotalent.Pages.Customers
{
    [Authorize]
    public class CustomerFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly CustomerService _customerService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly CustomerGroupService _customerGroupService;
        private readonly CustomerCategoryService _customerCategoryService;
        private readonly ICountryService _countrySevice;
        public CustomerFormModel(
            IMapper mapper,
            CustomerService customerService,
            NumberSequenceService numberSequenceService,
            CustomerGroupService customerGroupService,
            CustomerCategoryService customerCategoryService,
            ICountryService countrySevice
            )
        {
            _mapper = mapper;
            _customerService = customerService;
            _numberSequenceService = numberSequenceService;
            _customerCategoryService = customerCategoryService;
            _customerGroupService = customerGroupService;
            _countrySevice = countrySevice;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public CustomerModel CustomerForm { get; set; } = default!;

        public class CustomerModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Name")]
            public string Name { get; set; } = string.Empty;

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("Group")]
            public int CustomerGroupId { get; set; }

            [DisplayName("Category")]
            public int CustomerCategoryId { get; set; }

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
                CreateMap<Customer, CustomerModel>();
                CreateMap<CustomerModel, Customer>();
            }
        }

        public ICollection<SelectListItem> CustomerGroupLookup { get; set; } = default!;
        public ICollection<SelectListItem> CustomerCategoryLookup { get; set; } = default!;
        public ICollection<SelectListItem> CountryLookup { get; set; } = default!;
        private void BindLookup()
        {

            CustomerGroupLookup = _customerGroupService.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name}"
            }).ToList();

            CustomerCategoryLookup = _customerCategoryService.GetAll().Select(x => new SelectListItem
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
                var existing = await _customerService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                CustomerForm = _mapper.Map<CustomerModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                CustomerForm = new CustomerModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(CustomerForm))] CustomerModel input)
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
                var newobj = _mapper.Map<Customer>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(Customer), "", "CST");
                newobj.Number = Number;

                await _customerService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./CustomerForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _customerService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _customerService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./CustomerForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _customerService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _customerService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./CustomerList");
            }
            return Page();
        }

    }
}
