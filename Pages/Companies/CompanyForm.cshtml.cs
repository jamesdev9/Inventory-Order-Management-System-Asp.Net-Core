using AutoMapper;
using Indotalent.Applications.Companies;
using Indotalent.Infrastructures.Countries;
using Indotalent.Infrastructures.Currencies;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Infrastructures.TimeZones;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Indotalent.Pages.Companies
{
    [Authorize]
    public class CompanyFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly CompanyService _companyService;
        private readonly ICountryService _countrySevice;
        private readonly ICurrencyService _currencyService;
        private readonly ITimeZoneService _timeZoneService;
        public CompanyFormModel(
            IMapper mapper,
            CompanyService companyService,
            ICountryService countrySevice,
            ICurrencyService currencyService,
            ITimeZoneService timeZoneService
            )
        {
            _mapper = mapper;
            _companyService = companyService;
            _countrySevice = countrySevice;
            _currencyService = currencyService;
            _timeZoneService = timeZoneService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;

        [BindProperty]
        public CompanyModel CompanyForm { get; set; } = default!;

        public class CompanyModel
        {
            public int? Id { get; set; }
            public Guid? RowGuid { get; set; }

            [DisplayName("Name")]
            public string Name { get; set; } = string.Empty;

            [DisplayName("Currency")]
            public string Currency { get; set; } = string.Empty;

            [DisplayName("Time Zone")]
            public string TimeZone { get; set; } = string.Empty;

            [DisplayName("Description")]
            public string? Description { get; set; }

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
                CreateMap<Company, CompanyModel>();
                CreateMap<CompanyModel, Company>();
            }
        }

        public ICollection<SelectListItem> CountryLookup { get; set; } = default!;
        public ICollection<SelectListItem> CurrencyLookup { get; set; } = default!;
        public ICollection<SelectListItem> TimeZoneLookup { get; set; } = default!;

        private void BindLookup()
        {

            CountryLookup = _countrySevice.GetCountries();
            CurrencyLookup = _currencyService.GetCurrencies();
            TimeZoneLookup = _timeZoneService.GetAllTimeZones();
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
                var existing = await _companyService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                CompanyForm = _mapper.Map<CompanyModel>(existing);
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(CompanyForm))] CompanyModel input)
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
                var newobj = _mapper.Map<Company>(input);
                await _companyService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./CompanyForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _companyService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _companyService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./CompanyForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _companyService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _companyService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./CompanyList");
            }
            return Page();
        }

    }
}
