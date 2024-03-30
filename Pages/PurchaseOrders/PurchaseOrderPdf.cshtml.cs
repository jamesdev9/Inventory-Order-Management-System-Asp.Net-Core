using Indotalent.Applications.Companies;
using Indotalent.Applications.PurchaseOrderItems;
using Indotalent.Applications.PurchaseOrders;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Pages.PurchaseOrders
{
    public class PurchaseOrderPdfModel : PageModel
    {
        private readonly PurchaseOrderService _purchaseOrderService;
        private readonly PurchaseOrderItemService _purchaseOrderItemService;
        private readonly CompanyService _companyService;
        public PurchaseOrderPdfModel(
            PurchaseOrderService purchaseOrderService,
            PurchaseOrderItemService purchaseOrderItemService,
            CompanyService companyService)
        {
            _purchaseOrderService = purchaseOrderService;
            _purchaseOrderItemService = purchaseOrderItemService;
            _companyService = companyService;
        }

        public PurchaseOrder? PurchaseOrder { get; set; }
        public List<PurchaseOrderItem>? PurchaseOrderItems { get; set; }
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

            PurchaseOrder = await _purchaseOrderService
                .GetAll()
                .Include(x => x.Vendor)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            PurchaseOrderItems = await _purchaseOrderItemService
                .GetAll()
                .Include(x => x.Product)
                    .ThenInclude(x => x!.UnitMeasure)
                .Where(x => x.PurchaseOrderId == id)
                .ToListAsync();

            Vendor = PurchaseOrder?.Vendor;

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
