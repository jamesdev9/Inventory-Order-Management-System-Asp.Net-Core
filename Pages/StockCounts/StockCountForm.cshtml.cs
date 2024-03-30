using AutoMapper;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.StockCounts;
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

namespace Indotalent.Pages.StockCounts
{
    [Authorize]
    public class StockCountFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly StockCountService _stockCountService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductService _productService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        public StockCountFormModel(
            IMapper mapper,
            StockCountService stockCountService,
            NumberSequenceService numberSequenceService,
            WarehouseService warehouseService,
            ProductService productService,
            InventoryTransactionService inventoryTransactionService
            )
        {
            _mapper = mapper;
            _stockCountService = stockCountService;
            _numberSequenceService = numberSequenceService;
            _warehouseService = warehouseService;
            _productService = productService;
            _inventoryTransactionService = inventoryTransactionService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public StockCountModel StockCountForm { get; set; } = default!;

        public class StockCountModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Count Date")]
            public DateTime CountDate { get; set; } = DateTime.Now;

            [DisplayName("Count Status")]
            public StockCountStatus Status { get; set; }

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("Warehouse")]
            public int WarehouseId { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<StockCount, StockCountModel>();
                CreateMap<StockCountModel, StockCount>();
            }
        }

        public ICollection<SelectListItem> WarehouseLookup { get; set; } = default!;
        public ICollection<object> ProductLookup { get; set; } = default!;
        private void BindLookup()
        {

            WarehouseLookup = _warehouseService
                .GetAll()
                .Where(x => x.SystemWarehouse == false)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();


            ProductLookup = _productService.GetAll().Where(x => x.Physical == true)
                .Select(x => new { ProductId = x.Id, ProductName = $"{x.Name}" } as object)
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
                var existing = await _stockCountService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                StockCountForm = _mapper.Map<StockCountModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                StockCountForm = new StockCountModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(StockCountForm))] StockCountModel input)
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
                var newobj = _mapper.Map<StockCount>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(StockCount), "", "SC");
                newobj.Number = Number;

                await _stockCountService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./StockCountForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _stockCountService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _stockCountService.UpdateAsync(existing);

                var childs = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == existing.Id && x.ModuleName == nameof(StockCount))
                    .ToListAsync();

                foreach (var item in childs)
                {
                    item.ModuleNumber = existing.Number!;
                    item.MovementDate = existing.CountDate!.Value;
                    item.Status = (InventoryTransactionStatus)existing.Status!;
                    item.WarehouseId = existing.WarehouseId;

                    await _inventoryTransactionService.UpdateAsync(item);
                }

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./StockCountForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _stockCountService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _stockCountService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./StockCountList");
            }
            return Page();
        }

    }
}
