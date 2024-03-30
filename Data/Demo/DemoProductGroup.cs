using Indotalent.Applications.ProductGroups;
using Indotalent.Models.Entities;

namespace Indotalent.Data.Demo
{
    public static class DemoProductGroup
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var service = services.GetRequiredService<ProductGroupService>();

            await service.AddAsync(new ProductGroup { Name = "Hardware" });
            await service.AddAsync(new ProductGroup { Name = "Networking" });
            await service.AddAsync(new ProductGroup { Name = "Storage" });
            await service.AddAsync(new ProductGroup { Name = "Device" });
            await service.AddAsync(new ProductGroup { Name = "Software" });
            await service.AddAsync(new ProductGroup { Name = "Service" });
        }
    }
}
