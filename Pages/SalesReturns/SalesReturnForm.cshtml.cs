using AutoMapper;
using Indotalent.Applications.DeliveryOrders;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.SalesReturns;
using Indotalent.Applications.Warehouses;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Indotalent.Pages.SalesReturns
{
    [Authorize]
    public class SalesReturnFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly SalesReturnService _salesReturnService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly DeliveryOrderService _deliveryOrderService;
        private readonly ProductService _productService;
        private readonly WarehouseService _warehouseService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        public SalesReturnFormModel(
            IMapper mapper,
            SalesReturnService salesReturnService,
            NumberSequenceService numberSequenceService,
            DeliveryOrderService deliveryOrderService,
            ProductService productService,
            WarehouseService warehouseService,
            InventoryTransactionService inventoryTransactionService
            )
        {
            _mapper = mapper;
            _salesReturnService = salesReturnService;
            _numberSequenceService = numberSequenceService;
            _deliveryOrderService = deliveryOrderService;
            _productService = productService;
            _warehouseService = warehouseService;
            _inventoryTransactionService = inventoryTransactionService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public SalesReturnModel SalesReturnForm { get; set; } = default!;

        public class SalesReturnModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Return Date")]
            public DateTime ReturnDate { get; set; } = DateTime.Now;

            [DisplayName("Return Status")]
            public SalesReturnStatus Status { get; set; }

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("DeliveryOrder")]
            public int DeliveryOrderId { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<SalesReturn, SalesReturnModel>();
                CreateMap<SalesReturnModel, SalesReturn>();
            }
        }

        public ICollection<SelectListItem> DeliveryOrderLookup { get; set; } = default!;
        public ICollection<object> ProductLookup { get; set; } = default!;
        public ICollection<object> WarehouseLookup { get; set; } = default!;
        private void BindLookup()
        {

            DeliveryOrderLookup = _deliveryOrderService
                .GetAll()
                .Include(x => x.SalesOrder)
                    .ThenInclude(x => x!.Customer)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Number} / {x.SalesOrder!.Customer!.Name}"
                }).ToList();


            ProductLookup = _productService.GetAll().Where(x => x.Physical == true)
                .Select(x => new { ProductId = x.Id, ProductName = $"{x.Name}" } as object)
                .ToList();

            WarehouseLookup = _warehouseService.GetAll().Where(x => x.SystemWarehouse == false)
                .Select(x => new { WarehouseId = x.Id, WarehouseName = x.Name } as object)
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
                var existing = await _salesReturnService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                SalesReturnForm = _mapper.Map<SalesReturnModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                SalesReturnForm = new SalesReturnModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(SalesReturnForm))] SalesReturnModel input)
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
                var newobj = _mapper.Map<SalesReturn>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(SalesReturn), "", "SRN");
                newobj.Number = Number;

                await _salesReturnService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./SalesReturnForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _salesReturnService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _salesReturnService.UpdateAsync(existing);

                var childs = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == existing.Id && x.ModuleName == nameof(SalesReturn))
                    .ToListAsync();

                foreach (var item in childs)
                {
                    item.ModuleNumber = existing.Number!;
                    item.MovementDate = existing.ReturnDate!.Value;
                    item.Status = (InventoryTransactionStatus)existing.Status!;

                    await _inventoryTransactionService.UpdateAsync(item);
                }

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./SalesReturnForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _salesReturnService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _salesReturnService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./SalesReturnList");
            }
            return Page();
        }

    }
}
