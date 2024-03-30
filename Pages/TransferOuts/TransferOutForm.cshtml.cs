using AutoMapper;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.TransferOuts;
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

namespace Indotalent.Pages.TransferOuts
{
    [Authorize]
    public class TransferOutFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly TransferOutService _transferOutService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductService _productService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        public TransferOutFormModel(
            IMapper mapper,
            TransferOutService transferOutService,
            NumberSequenceService numberSequenceService,
            WarehouseService warehouseService,
            ProductService productService,
            InventoryTransactionService inventoryTransactionService
            )
        {
            _mapper = mapper;
            _transferOutService = transferOutService;
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
        public TransferOutModel TransferOutForm { get; set; } = default!;

        public class TransferOutModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Release Date")]
            public DateTime TransferReleaseDate { get; set; } = DateTime.Now;

            [DisplayName("Transfer Status")]
            public TransferStatus Status { get; set; }

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("Warehouse From")]
            public int WarehouseFromId { get; set; }

            [DisplayName("Warehouse To")]
            public int WarehouseToId { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<TransferOut, TransferOutModel>();
                CreateMap<TransferOutModel, TransferOut>();
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
                var existing = await _transferOutService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                TransferOutForm = _mapper.Map<TransferOutModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                TransferOutForm = new TransferOutModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(TransferOutForm))] TransferOutModel input)
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
                var newobj = _mapper.Map<TransferOut>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(TransferOut), "", "OUT");
                newobj.Number = Number;

                await _transferOutService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./TransferOutForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _transferOutService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _transferOutService.UpdateAsync(existing);

                var childs = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == existing.Id && x.ModuleName == nameof(TransferOut))
                    .ToListAsync();

                foreach (var item in childs)
                {
                    item.ModuleNumber = existing.Number!;
                    item.MovementDate = existing.TransferReleaseDate!.Value;
                    item.Status = (InventoryTransactionStatus)existing.Status!;
                    item.WarehouseId = existing.WarehouseFromId!.Value;

                    await _inventoryTransactionService.UpdateAsync(item);
                }

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./TransferOutForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _transferOutService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _transferOutService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./TransferOutList");
            }
            return Page();
        }

    }
}
