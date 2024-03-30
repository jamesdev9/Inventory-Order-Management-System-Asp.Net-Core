using AutoMapper;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.Scrappings;
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

namespace Indotalent.Pages.Scrappings
{
    [Authorize]
    public class ScrappingFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly ScrappingService _scrappingService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductService _productService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        public ScrappingFormModel(
            IMapper mapper,
            ScrappingService scrappingService,
            NumberSequenceService numberSequenceService,
            WarehouseService warehouseService,
            ProductService productService,
            InventoryTransactionService inventoryTransactionService
            )
        {
            _mapper = mapper;
            _scrappingService = scrappingService;
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
        public ScrappingModel ScrappingForm { get; set; } = default!;

        public class ScrappingModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Scrapping Date")]
            public DateTime ScrappingDate { get; set; } = DateTime.Now;

            [DisplayName("Scrapping Status")]
            public ScrappingStatus Status { get; set; }

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("Warehouse")]
            public int WarehouseId { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Scrapping, ScrappingModel>();
                CreateMap<ScrappingModel, Scrapping>();
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
                var existing = await _scrappingService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                ScrappingForm = _mapper.Map<ScrappingModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                ScrappingForm = new ScrappingModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(ScrappingForm))] ScrappingModel input)
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
                var newobj = _mapper.Map<Scrapping>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(Scrapping), "", "SCRP");
                newobj.Number = Number;

                await _scrappingService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./ScrappingForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _scrappingService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _scrappingService.UpdateAsync(existing);

                var childs = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == existing.Id && x.ModuleName == nameof(Scrapping))
                    .ToListAsync();

                foreach (var item in childs)
                {
                    item.ModuleNumber = existing.Number!;
                    item.MovementDate = existing.ScrappingDate!.Value;
                    item.Status = (InventoryTransactionStatus)existing.Status!;
                    item.WarehouseId = existing.WarehouseId;

                    await _inventoryTransactionService.UpdateAsync(item);
                }

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./ScrappingForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _scrappingService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _scrappingService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./ScrappingList");
            }
            return Page();
        }

    }
}
