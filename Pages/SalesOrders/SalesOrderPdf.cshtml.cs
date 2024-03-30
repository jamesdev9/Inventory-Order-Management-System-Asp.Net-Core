using Indotalent.Applications.Companies;
using Indotalent.Applications.SalesOrderItems;
using Indotalent.Applications.SalesOrders;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Pages.SalesOrders
{
    public class SalesOrderPdfModel : PageModel
    {
        private readonly SalesOrderService _salesOrderService;
        private readonly SalesOrderItemService _salesOrderItemService;
        private readonly CompanyService _companyService;
        public SalesOrderPdfModel(
            SalesOrderService salesOrderService,
            SalesOrderItemService salesOrderItemService,
            CompanyService companyService)
        {
            _salesOrderService = salesOrderService;
            _salesOrderItemService = salesOrderItemService;
            _companyService = companyService;
        }

        public SalesOrder? SalesOrder { get; set; }
        public List<SalesOrderItem>? SalesOrderItems { get; set; }
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

            SalesOrder = await _salesOrderService
                .GetAll()
                .Include(x => x.Customer)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            SalesOrderItems = await _salesOrderItemService
                .GetAll()
                .Include(x => x.Product)
                    .ThenInclude(x => x!.UnitMeasure)
                .Where(x => x.SalesOrderId == id)
                .ToListAsync();

            Customer = SalesOrder?.Customer;

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
