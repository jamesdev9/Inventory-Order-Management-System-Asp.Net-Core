using Indotalent.Applications.Companies;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.TransferOuts;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Pages.TransferOuts
{
    public class TransferOutPdfModel : PageModel
    {
        private readonly TransferOutService _transferOutService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        private readonly CompanyService _companyService;
        public TransferOutPdfModel(
            TransferOutService transferOutService,
            InventoryTransactionService inventoryTransactionService,
            CompanyService companyService)
        {
            _transferOutService = transferOutService;
            _inventoryTransactionService = inventoryTransactionService;
            _companyService = companyService;
        }

        public TransferOut? TransferOut { get; set; }
        public List<InventoryTransaction>? InventoryTransactions { get; set; }
        public Company? Company { get; set; }
        public Warehouse? WarehouseFrom { get; set; }
        public Warehouse? WarehouseTo { get; set; }
        public string? CompanyAddress { get; set; }

        public async Task OnGetAsync(int? id)
        {
            Company = await _companyService.GetDefaultCompanyAsync();

            CompanyAddress = string.Join(", ", new List<string>()
            {
                Company?.Street ?? string.Empty,
                Company?.City ?? string.Empty,
                Company?.State ?? string.Empty,
                Company?.Country ?? string.Empty,
                Company?.ZipCode ?? string.Empty
            }.Where(s => !string.IsNullOrEmpty(s)));

            TransferOut = await _transferOutService
                .GetAll()
                .Include(x => x.WarehouseFrom)
                .Include(x => x.WarehouseTo)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            WarehouseFrom = TransferOut?.WarehouseFrom;
            WarehouseTo = TransferOut?.WarehouseTo;

            InventoryTransactions = await _inventoryTransactionService
                .GetAll()
                .Where(x => x.ModuleId == id && x.ModuleName == nameof(TransferOut))
                .Include(x => x.Product)
                    .ThenInclude(x => x!.UnitMeasure)
                .ToListAsync();
        }
    }
}
