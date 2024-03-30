using AutoMapper;
using Indotalent.Applications.GoodsReceives;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.PurchaseOrders;
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

namespace Indotalent.Pages.GoodsReceives
{
    [Authorize]
    public class GoodsReceiveFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly GoodsReceiveService _goodsReceiveService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly PurchaseOrderService _purchaseOrderService;
        private readonly ProductService _productService;
        private readonly WarehouseService _warehouseService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        public GoodsReceiveFormModel(
            IMapper mapper,
            GoodsReceiveService goodsReceiveService,
            NumberSequenceService numberSequenceService,
            PurchaseOrderService purchaseOrderService,
            ProductService productService,
            WarehouseService warehouseService,
            InventoryTransactionService inventoryTransactionService
            )
        {
            _mapper = mapper;
            _goodsReceiveService = goodsReceiveService;
            _numberSequenceService = numberSequenceService;
            _purchaseOrderService = purchaseOrderService;
            _productService = productService;
            _warehouseService = warehouseService;
            _inventoryTransactionService = inventoryTransactionService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public GoodsReceiveModel GoodsReceiveForm { get; set; } = default!;

        public class GoodsReceiveModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Receive Date")]
            public DateTime ReceiveDate { get; set; } = DateTime.Now;

            [DisplayName("Order Status")]
            public GoodsReceiveStatus Status { get; set; }

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("PurchaseOrder")]
            public int PurchaseOrderId { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<GoodsReceive, GoodsReceiveModel>();
                CreateMap<GoodsReceiveModel, GoodsReceive>();
            }
        }

        public ICollection<SelectListItem> PurchaseOrderLookup { get; set; } = default!;
        public ICollection<object> ProductLookup { get; set; } = default!;
        public ICollection<object> WarehouseLookup { get; set; } = default!;
        private void BindLookup()
        {

            PurchaseOrderLookup = _purchaseOrderService
                .GetAll()
                .Include(x => x.Vendor)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Number} / {x.Vendor!.Name}"
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
                var existing = await _goodsReceiveService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                GoodsReceiveForm = _mapper.Map<GoodsReceiveModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                GoodsReceiveForm = new GoodsReceiveModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(GoodsReceiveForm))] GoodsReceiveModel input)
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
                var newobj = _mapper.Map<GoodsReceive>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(GoodsReceive), "", "GR");
                newobj.Number = Number;

                await _goodsReceiveService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./GoodsReceiveForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _goodsReceiveService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _goodsReceiveService.UpdateAsync(existing);

                var childs = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == existing.Id && x.ModuleName == nameof(GoodsReceive))
                    .ToListAsync();

                foreach (var item in childs)
                {
                    item.ModuleNumber = existing.Number!;
                    item.MovementDate = existing.ReceiveDate!.Value;
                    item.Status = (InventoryTransactionStatus)existing.Status!;

                    await _inventoryTransactionService.UpdateAsync(item);
                }

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./GoodsReceiveForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _goodsReceiveService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _goodsReceiveService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./GoodsReceiveList");
            }
            return Page();
        }

    }
}
