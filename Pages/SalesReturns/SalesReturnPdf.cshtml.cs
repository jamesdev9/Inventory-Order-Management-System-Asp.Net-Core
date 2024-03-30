using Indotalent.Applications.Companies;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.SalesReturns;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Pages.SalesReturns
{
    public class SalesReturnPdfModel : PageModel
    {
        private readonly SalesReturnService _salesReturnService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        private readonly CompanyService _companyService;
        public SalesReturnPdfModel(
            SalesReturnService salesReturnService,
            InventoryTransactionService inventoryTransactionService,
            CompanyService companyService)
        {
            _salesReturnService = salesReturnService;
            _inventoryTransactionService = inventoryTransactionService;
            _companyService = companyService;
        }

        public SalesReturn? SalesReturn { get; set; }
        public List<InventoryTransaction>? InventoryTransactions { get; set; }
        public Company? Company { get; set; }
        public Customer? Customer { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CustomerAddress { get; set; }

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

            SalesReturn = await _salesReturnService
                .GetAll()
                .Include(x => x.DeliveryOrder)
                    .ThenInclude(x => x!.SalesOrder)
                        .ThenInclude(x => x!.Customer)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            InventoryTransactions = await _inventoryTransactionService
                .GetAll()
                .Where(x => x.ModuleId == id && x.ModuleName == nameof(SalesReturn))
                .Include(x => x.Product)
                    .ThenInclude(x => x!.UnitMeasure)
                .ToListAsync();

            Customer = SalesReturn!.DeliveryOrder!.SalesOrder!.Customer;

            CustomerAddress = string.Join(", ", new List<string>()
            {
                Customer?.Street ?? string.Empty,
                Customer?.City ?? string.Empty,
                Customer?.State ?? string.Empty,
                Customer?.Country ?? string.Empty,
                Customer?.ZipCode ?? string.Empty

            }.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
