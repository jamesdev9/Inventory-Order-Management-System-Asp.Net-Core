using AutoMapper;
using Indotalent.Applications.Customers;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.SalesOrders;
using Indotalent.Applications.Taxes;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Indotalent.Pages.SalesOrders
{
    [Authorize]
    public class SalesOrderFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly SalesOrderService _salesOrderService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly CustomerService _customerService;
        private readonly TaxService _taxService;
        private readonly ProductService _productService;
        public SalesOrderFormModel(
            IMapper mapper,
            SalesOrderService salesOrderService,
            NumberSequenceService numberSequenceService,
            CustomerService customerService,
            TaxService taxService,
            ProductService productService
            )
        {
            _mapper = mapper;
            _salesOrderService = salesOrderService;
            _numberSequenceService = numberSequenceService;
            _taxService = taxService;
            _customerService = customerService;
            _productService = productService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public SalesOrderModel SalesOrderForm { get; set; } = default!;

        public class SalesOrderModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Order Date")]
            public DateTime OrderDate { get; set; } = DateTime.Now;

            [DisplayName("Order Status")]
            public SalesOrderStatus OrderStatus { get; set; }

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("Customer")]
            public int CustomerId { get; set; }

            [DisplayName("Tax")]
            public int TaxId { get; set; }

            [DisplayName("Before Tax Amount")]
            public double? BeforeTaxAmount { get; set; }

            [DisplayName("Tax Amount")]
            public double? TaxAmount { get; set; }

            [DisplayName("After Tax Amount")]
            public double? AfterTaxAmount { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<SalesOrder, SalesOrderModel>();
                CreateMap<SalesOrderModel, SalesOrder>();
            }
        }

        public ICollection<SelectListItem> CustomerLookup { get; set; } = default!;
        public ICollection<SelectListItem> TaxLookup { get; set; } = default!;
        public ICollection<object> ProductLookup { get; set; } = default!;
        public ICollection<object> PriceLookup { get; set; } = default!;
        public ICollection<object> NumberLookup { get; set; } = default!;
        private void BindLookup()
        {

            CustomerLookup = _customerService.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name}"
            }).ToList();

            TaxLookup = _taxService.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name}"
            }).ToList();


            ProductLookup = _productService.GetAll()
                .Select(x => new { ProductId = x.Id, ProductName = $"{x.Name} / {x.UnitPrice}" } as object)
                .ToList();

            PriceLookup = _productService.GetAll()
                .Select(x => new { ProductId = x.Id, ProductPrice = x.UnitPrice } as object)
                .ToList();

            NumberLookup = _productService.GetAll()
                .Select(x => new { ProductId = x.Id, ProductNumber = x.Number } as object)
                .ToList();


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
                var existing = await _salesOrderService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                SalesOrderForm = _mapper.Map<SalesOrderModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                SalesOrderForm = new SalesOrderModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(SalesOrderForm))] SalesOrderModel input)
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
                var newobj = _mapper.Map<SalesOrder>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(SalesOrder), "", "SO");
                newobj.Number = Number;

                await _salesOrderService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./SalesOrderForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _salesOrderService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _salesOrderService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./SalesOrderForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _salesOrderService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _salesOrderService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./SalesOrderList");
            }
            return Page();
        }

    }
}
