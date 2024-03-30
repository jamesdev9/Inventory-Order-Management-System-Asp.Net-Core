using AutoMapper;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.Products;
using Indotalent.Applications.TransferIns;
using Indotalent.Applications.TransferOuts;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Indotalent.Pages.TransferIns
{
    [Authorize]
    public class TransferInFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly TransferInService _transferInService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly TransferOutService _transferOutService;
        private readonly ProductService _productService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        public TransferInFormModel(
            IMapper mapper,
            TransferInService transferInService,
            NumberSequenceService numberSequenceService,
            TransferOutService transferOutService,
            ProductService productService,
            InventoryTransactionService inventoryTransactionService
            )
        {
            _mapper = mapper;
            _transferInService = transferInService;
            _numberSequenceService = numberSequenceService;
            _transferOutService = transferOutService;
            _productService = productService;
            _inventoryTransactionService = inventoryTransactionService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public TransferInModel TransferInForm { get; set; } = default!;

        public class TransferInModel
        {
            public Guid? RowGuid { get; set; }
            public int? Id { get; set; }

            [DisplayName("Receive Date")]
            public DateTime TransferReceiveDate { get; set; } = DateTime.Now;

            [DisplayName("Transfer Status")]
            public TransferStatus Status { get; set; }

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("Transfer Out")]
            public int TransferOutId { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<TransferIn, TransferInModel>();
                CreateMap<TransferInModel, TransferIn>();
            }
        }

        public ICollection<SelectListItem> TransferOutLookup { get; set; } = default!;
        public ICollection<object> ProductLookup { get; set; } = default!;
        private void BindLookup()
        {

            TransferOutLookup = _transferOutService
                .GetAll()
                .Include(x => x.WarehouseFrom)
                .Include(x => x.WarehouseTo)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Number} / {x.WarehouseFrom!.Name}-{x.WarehouseTo!.Name}"
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
                var existing = await _transferInService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                TransferInForm = _mapper.Map<TransferInModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                TransferInForm = new TransferInModel
                {
                    RowGuid = Guid.Empty,
                    Id = 0
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(TransferInForm))] TransferInModel input)
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
                var newobj = _mapper.Map<TransferIn>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(TransferIn), "", "IN");
                newobj.Number = Number;

                await _transferInService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./TransferInForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _transferInService
                    .GetAll()
                    .Include(x => x.TransferOut)
                    .Where(x => x.RowGuid == input.RowGuid)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _transferInService.UpdateAsync(existing);

                var childs = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == existing.Id && x.ModuleName == nameof(TransferIn))
                    .ToListAsync();

                foreach (var item in childs)
                {
                    item.ModuleNumber = existing.Number!;
                    item.MovementDate = existing.TransferReceiveDate!.Value;
                    item.Status = (InventoryTransactionStatus)existing.Status!;
                    item.WarehouseId = existing.TransferOut!.WarehouseToId!.Value;

                    await _inventoryTransactionService.UpdateAsync(item);
                }

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./TransferInForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _transferInService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _transferInService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./TransferInList");
            }
            return Page();
        }

    }
}
