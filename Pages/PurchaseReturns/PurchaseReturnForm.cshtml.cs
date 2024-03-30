using AutoMapper;
using Indotalent.Applications.GoodsReceives;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.PurchaseReturns;
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

namespace Indotalent.Pages.PurchaseReturns
{
    [Authorize]
    public class PurchaseReturnFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly PurchaseReturnService _purchaseReturnService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly GoodsReceiveService _goodsReceiveService;
        private readonly ProductService _productService;
        private readonly WarehouseService _warehouseService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        public PurchaseReturnFormModel(
            IMapper mapper,
            PurchaseReturnService purchaseReturnService,
            NumberSequenceService numberSequenceService,
            GoodsReceiveService goodsReceiveService,
            ProductService productService,
            WarehouseService warehouseService,
            InventoryTransactionService inventoryTransactionService
            )
        {
            _mapper = mapper;
            _purchaseReturnService = purchaseReturnService;
            _numberSequenceService = numberSequenceService;
            _goodsReceiveService = goodsReceiveService;
            _productService = productService;
            _warehouseService = warehouseService;
            _inventoryTransactionService = inventoryTransactionService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public PurchaseReturnModel PurchaseReturnForm { get; set; } = default!;

        public class PurchaseReturnModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Return Date")]
            public DateTime ReturnDate { get; set; } = DateTime.Now;

            [DisplayName("Return Status")]
            public PurchaseReturnStatus Status { get; set; }

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("GoodsReceive")]
            public int GoodsReceiveId { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<PurchaseReturn, PurchaseReturnModel>();
                CreateMap<PurchaseReturnModel, PurchaseReturn>();
            }
        }

        public ICollection<SelectListItem> GoodsReceiveLookup { get; set; } = default!;
        public ICollection<object> ProductLookup { get; set; } = default!;
        public ICollection<object> WarehouseLookup { get; set; } = default!;
        private void BindLookup()
        {

            GoodsReceiveLookup = _goodsReceiveService
                .GetAll()
                .Include(x => x.PurchaseOrder)
                    .ThenInclude(x => x!.Vendor)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Number} / {x.PurchaseOrder!.Vendor!.Name}"
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
                var existing = await _purchaseReturnService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                PurchaseReturnForm = _mapper.Map<PurchaseReturnModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                PurchaseReturnForm = new PurchaseReturnModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(PurchaseReturnForm))] PurchaseReturnModel input)
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
                var newobj = _mapper.Map<PurchaseReturn>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(PurchaseReturn), "", "PRN");
                newobj.Number = Number;

                await _purchaseReturnService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./PurchaseReturnForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _purchaseReturnService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _purchaseReturnService.UpdateAsync(existing);

                var childs = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == existing.Id && x.ModuleName == nameof(PurchaseReturn))
                    .ToListAsync();

                foreach (var item in childs)
                {
                    item.ModuleNumber = existing.Number!;
                    item.MovementDate = existing.ReturnDate!.Value;
                    item.Status = (InventoryTransactionStatus)existing.Status!;

                    await _inventoryTransactionService.UpdateAsync(item);
                }

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./PurchaseReturnForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _purchaseReturnService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _purchaseReturnService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./PurchaseReturnList");
            }
            return Page();
        }

    }
}
