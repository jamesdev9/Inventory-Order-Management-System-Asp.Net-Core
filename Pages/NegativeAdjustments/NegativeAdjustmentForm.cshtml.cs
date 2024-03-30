using AutoMapper;
using Indotalent.Applications.AdjustmentMinuss;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.Warehouses;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Indotalent.Pages.NegativeAdjustments
{
    [Authorize]
    public class NegativeAdjustmentFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly AdjustmentMinusService _adjustmentMinusService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly ProductService _productService;
        private readonly WarehouseService _warehouseService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        public NegativeAdjustmentFormModel(
            IMapper mapper,
            AdjustmentMinusService adjustmentMinusService,
            NumberSequenceService numberSequenceService,
            ProductService productService,
            WarehouseService warehouseService,
            InventoryTransactionService inventoryTransactionService
            )
        {
            _mapper = mapper;
            _adjustmentMinusService = adjustmentMinusService;
            _numberSequenceService = numberSequenceService;
            _productService = productService;
            _warehouseService = warehouseService;
            _inventoryTransactionService = inventoryTransactionService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public AdjustmentMinusModel AdjustmentMinusForm { get; set; } = default!;

        public class AdjustmentMinusModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Adjustment Date")]
            public DateTime AdjustmentDate { get; set; } = DateTime.Now;

            [DisplayName("Adjustment Status")]
            public AdjustmentStatus Status { get; set; }

            [DisplayName("Description")]
            public string? Description { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<AdjustmentMinus, AdjustmentMinusModel>();
                CreateMap<AdjustmentMinusModel, AdjustmentMinus>();
            }
        }

        public ICollection<object> ProductLookup { get; set; } = default!;
        public ICollection<object> WarehouseLookup { get; set; } = default!;
        private void BindLookup()
        {

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
                var existing = await _adjustmentMinusService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                AdjustmentMinusForm = _mapper.Map<AdjustmentMinusModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                AdjustmentMinusForm = new AdjustmentMinusModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(AdjustmentMinusForm))] AdjustmentMinusModel input)
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
                var newobj = _mapper.Map<AdjustmentMinus>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(AdjustmentMinus), "", "ADJ-");
                newobj.Number = Number;

                await _adjustmentMinusService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./NegativeAdjustmentForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _adjustmentMinusService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _adjustmentMinusService.UpdateAsync(existing);

                var childs = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == existing.Id && x.ModuleName == nameof(AdjustmentMinus))
                    .ToListAsync();

                foreach (var item in childs)
                {
                    item.ModuleNumber = existing.Number!;
                    item.MovementDate = existing.AdjustmentDate!.Value;
                    item.Status = (InventoryTransactionStatus)existing.Status!;

                    await _inventoryTransactionService.UpdateAsync(item);
                }

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./NegativeAdjustmentForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _adjustmentMinusService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _adjustmentMinusService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./NegativeAdjustmentList");
            }
            return Page();
        }

    }
}
