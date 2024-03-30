using Indotalent.Applications.Companies;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.StockCounts;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Pages.StockCounts
{
    public class StockCountPdfModel : PageModel
    {
        private readonly StockCountService _stockCountService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        private readonly CompanyService _companyService;
        public StockCountPdfModel(
            StockCountService stockCountService,
            InventoryTransactionService inventoryTransactionService,
            CompanyService companyService)
        {
            _stockCountService = stockCountService;
            _inventoryTransactionService = inventoryTransactionService;
            _companyService = companyService;
        }

        public StockCount? StockCount { get; set; }
        public List<InventoryTransaction>? InventoryTransactions { get; set; }
        public Company? Company { get; set; }
        public Warehouse? Warehouse { get; set; }
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

            StockCount = await _stockCountService
                .GetAll()
                .Include(x => x.Warehouse)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            Warehouse = StockCount?.Warehouse;

            InventoryTransactions = await _inventoryTransactionService
                .GetAll()
                .Where(x => x.ModuleId == id && x.ModuleName == nameof(StockCount))
                .Include(x => x.Product)
                    .ThenInclude(x => x!.UnitMeasure)
                .ToListAsync();
        }
    }
}
