using Indotalent.Applications.AdjustmentMinuss;
using Indotalent.Applications.AdjustmentPluss;
using Indotalent.Applications.ApplicationUsers;
using Indotalent.Applications.Companies;
using Indotalent.Applications.CustomerCategories;
using Indotalent.Applications.CustomerContacts;
using Indotalent.Applications.CustomerGroups;
using Indotalent.Applications.Customers;
using Indotalent.Applications.DeliveryOrders;
using Indotalent.Applications.GoodsReceives;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.LogAnalytics;
using Indotalent.Applications.LogErrors;
using Indotalent.Applications.LogSessions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.ProductGroups;
using Indotalent.Applications.Products;
using Indotalent.Applications.PurchaseOrderItems;
using Indotalent.Applications.PurchaseOrders;
using Indotalent.Applications.PurchaseReturns;
using Indotalent.Applications.SalesOrderItems;
using Indotalent.Applications.SalesOrders;
using Indotalent.Applications.SalesReturns;
using Indotalent.Applications.Scrappings;
using Indotalent.Applications.StockCounts;
using Indotalent.Applications.Taxes;
using Indotalent.Applications.TransferIns;
using Indotalent.Applications.TransferOuts;
using Indotalent.Applications.UnitMeasures;
using Indotalent.Applications.VendorCategories;
using Indotalent.Applications.VendorContacts;
using Indotalent.Applications.VendorGroups;
using Indotalent.Applications.Vendors;
using Indotalent.Applications.Warehouses;
using Indotalent.Infrastructures.Countries;
using Indotalent.Infrastructures.Currencies;
using Indotalent.Infrastructures.Docs;
using Indotalent.Infrastructures.Emails;
using Indotalent.Infrastructures.Images;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Infrastructures.TimeZones;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Indotalent
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IEmailSender, SMTPEmailService>();
            services.AddScoped<IFileImageService, FileImageService>();
            services.AddScoped<IFileDocumentService, FileDocumentService>();
            services.AddScoped<ITimeZoneService, TimeZoneService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IAuditColumnTransformer, AuditColumnTransformer>();
            services.AddScoped<CompanyService>();
            services.AddScoped<ApplicationUserService>();
            services.AddScoped<NumberSequenceService>();
            services.AddScoped<LogErrorService>();
            services.AddScoped<LogSessionService>();
            services.AddScoped<LogAnalyticService>();
            services.AddScoped<CustomerGroupService>();
            services.AddScoped<CustomerCategoryService>();
            services.AddScoped<VendorGroupService>();
            services.AddScoped<VendorCategoryService>();
            services.AddScoped<WarehouseService>();
            services.AddScoped<CustomerService>();
            services.AddScoped<VendorService>();
            services.AddScoped<UnitMeasureService>();
            services.AddScoped<ProductGroupService>();
            services.AddScoped<ProductService>();
            services.AddScoped<CustomerContactService>();
            services.AddScoped<VendorContactService>();
            services.AddScoped<TaxService>();
            services.AddScoped<SalesOrderService>();
            services.AddScoped<SalesOrderItemService>();
            services.AddScoped<PurchaseOrderService>();
            services.AddScoped<PurchaseOrderItemService>();
            services.AddScoped<InventoryTransactionService>();
            services.AddScoped<DeliveryOrderService>();
            services.AddScoped<GoodsReceiveService>();
            services.AddScoped<SalesReturnService>();
            services.AddScoped<PurchaseReturnService>();
            services.AddScoped<TransferInService>();
            services.AddScoped<TransferOutService>();
            services.AddScoped<StockCountService>();
            services.AddScoped<AdjustmentMinusService>();
            services.AddScoped<AdjustmentPlusService>();
            services.AddScoped<ScrappingService>();

            return services;
        }
    }
}
