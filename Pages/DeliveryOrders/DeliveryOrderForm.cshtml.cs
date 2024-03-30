using AutoMapper;
using Indotalent.Applications.DeliveryOrders;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.SalesOrders;
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

namespace Indotalent.Pages.DeliveryOrders
{
    [Authorize]
    public class DeliveryOrderFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly DeliveryOrderService _deliveryOrderService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly SalesOrderService _salesOrderService;
        private readonly ProductService _productService;
        private readonly WarehouseService _warehouseService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        public DeliveryOrderFormModel(
            IMapper mapper,
            DeliveryOrderService deliveryOrderService,
            NumberSequenceService numberSequenceService,
            SalesOrderService salesOrderService,
            ProductService productService,
            WarehouseService warehouseService,
            InventoryTransactionService inventoryTransactionService
            )
        {
            _mapper = mapper;
            _deliveryOrderService = deliveryOrderService;
            _numberSequenceService = numberSequenceService;
            _salesOrderService = salesOrderService;
            _productService = productService;
            _warehouseService = warehouseService;
            _inventoryTransactionService = inventoryTransactionService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public DeliveryOrderModel DeliveryOrderForm { get; set; } = default!;

        public class DeliveryOrderModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Delivery Date")]
            public DateTime DeliveryDate { get; set; } = DateTime.Now;

            [DisplayName("Order Status")]
            public DeliveryOrderStatus Status { get; set; }

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("SalesOrder")]
            public int SalesOrderId { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<DeliveryOrder, DeliveryOrderModel>();
                CreateMap<DeliveryOrderModel, DeliveryOrder>();
            }
        }

        public ICollection<SelectListItem> SalesOrderLookup { get; set; } = default!;
        public ICollection<object> ProductLookup { get; set; } = default!;
        public ICollection<object> WarehouseLookup { get; set; } = default!;
        private void BindLookup()
        {

            SalesOrderLookup = _salesOrderService
                .GetAll()
                .Include(x => x.Customer)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Number} / {x.Customer!.Name}"
                }).ToList();


            ProductLookup = _productService.GetAll()
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
                var existing = await _deliveryOrderService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                DeliveryOrderForm = _mapper.Map<DeliveryOrderModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                DeliveryOrderForm = new DeliveryOrderModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(DeliveryOrderForm))] DeliveryOrderModel input)
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
                var newobj = _mapper.Map<DeliveryOrder>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(DeliveryOrder), "", "DO");
                newobj.Number = Number;

                await _deliveryOrderService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./DeliveryOrderForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _deliveryOrderService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _deliveryOrderService.UpdateAsync(existing);

                var childs = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == existing.Id && x.ModuleName == nameof(DeliveryOrder))
                    .ToListAsync();

                foreach (var item in childs)
                {
                    item.ModuleNumber = existing.Number!;
                    item.MovementDate = existing.DeliveryDate!.Value;
                    item.Status = (InventoryTransactionStatus)existing.Status!;

                    await _inventoryTransactionService.UpdateAsync(item);
                }

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./DeliveryOrderForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _deliveryOrderService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _deliveryOrderService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./DeliveryOrderList");
            }
            return Page();
        }

    }
}
