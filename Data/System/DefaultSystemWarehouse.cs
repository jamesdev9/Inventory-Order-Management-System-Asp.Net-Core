using Indotalent.Applications.Warehouses;
using Indotalent.Models.Entities;

namespace Indotalent.Data.System
{
    public static class DefaultSystemWarehouse
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var service = services.GetRequiredService<WarehouseService>();

            await service.AddAsync(new Warehouse { Name = "Customer", SystemWarehouse = true });
            await service.AddAsync(new Warehouse { Name = "Vendor", SystemWarehouse = true });
            await service.AddAsync(new Warehouse { Name = "Transfer", SystemWarehouse = true });
            await service.AddAsync(new Warehouse { Name = "Adjustment", SystemWarehouse = true });
            await service.AddAsync(new Warehouse { Name = "StockCount", SystemWarehouse = true });
            await service.AddAsync(new Warehouse { Name = "Scrapping", SystemWarehouse = true });
        }
    }
}
