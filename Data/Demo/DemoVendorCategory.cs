using Indotalent.Applications.VendorCategories;
using Indotalent.Models.Entities;

namespace Indotalent.Data.Demo
{
    public static class DemoVendorCategory
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var service = services.GetRequiredService<VendorCategoryService>();

            await service.AddAsync(new VendorCategory { Name = "Large" });
            await service.AddAsync(new VendorCategory { Name = "Medium" });
            await service.AddAsync(new VendorCategory { Name = "Small" });
            await service.AddAsync(new VendorCategory { Name = "Specialty" });
            await service.AddAsync(new VendorCategory { Name = "Local" });
            await service.AddAsync(new VendorCategory { Name = "Global" });
        }
    }
}
