using Indotalent.Applications.Companies;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.PurchaseReturns;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Pages.PurchaseReturns
{
    public class PurchaseReturnPdfModel : PageModel
    {
        private readonly PurchaseReturnService _purchaseReturnService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        private readonly CompanyService _companyService;
        public PurchaseReturnPdfModel(
            PurchaseReturnService purchaseReturnService,
            InventoryTransactionService inventoryTransactionService,
            CompanyService companyService)
        {
            _purchaseReturnService = purchaseReturnService;
            _inventoryTransactionService = inventoryTransactionService;
            _companyService = companyService;
        }

        public PurchaseReturn? PurchaseReturn { get; set; }
        public List<InventoryTransaction>? InventoryTransactions { get; set; }
        public Company? Company { get; set; }
        public Vendor? Vendor { get; set; }
        public string? CompanyAddress { get; set; }
        public string? VendorAddress { get; set; }

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

            PurchaseReturn = await _purchaseReturnService
                .GetAll()
                .Include(x => x.GoodsReceive)
                    .ThenInclude(x => x!.PurchaseOrder)
                        .ThenInclude(x => x!.Vendor)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            InventoryTransactions = await _inventoryTransactionService
                .GetAll()
                .Where(x => x.ModuleId == id && x.ModuleName == nameof(PurchaseReturn))
                .Include(x => x.Product)
                    .ThenInclude(x => x!.UnitMeasure)
                .ToListAsync();

            Vendor = PurchaseReturn!.GoodsReceive!.PurchaseOrder!.Vendor;

            VendorAddress = string.Join(", ", new List<string>()
            {
                Vendor?.Street ?? string.Empty,
                Vendor?.City ?? string.Empty,
                Vendor?.State ?? string.Empty,
                Vendor?.Country ?? string.Empty,
                Vendor?.ZipCode ?? string.Empty

            }.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
