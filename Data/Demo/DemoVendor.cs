using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.VendorCategories;
using Indotalent.Applications.VendorGroups;
using Indotalent.Applications.Vendors;
using Indotalent.Models.Entities;

namespace Indotalent.Data.Demo
{
    public static class DemoVendor
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var vendorService = services.GetRequiredService<VendorService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var vendorGroupService = services.GetRequiredService<VendorGroupService>();
            var vendorCategoryService = services.GetRequiredService<VendorCategoryService>();

            var groups = vendorGroupService.GetAll().Select(x => x.Id).ToArray();
            var categories = vendorCategoryService.GetAll().Select(x => x.Id).ToArray();
            var cities = new string[] { "New York", "Los Angeles", "San Francisco", "Chicago" };

            Random random = new Random();

            await vendorService.AddAsync(new Vendor
            {
                Name = "Quantum Industries",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Apex Ventures",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Horizon Enterprises",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Nova Innovations",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Phoenix Holdings",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Titan Group",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Zenith Corporation",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Prime Solutions",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Cascade Enterprises",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Aurora Holdings",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Vanguard Industries",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Empyrean Ventures",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Genesis Corporation",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Equinox Enterprises",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Summit Holdings",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Sovereign Solutions",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Spectrum Corporation",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Elysium Enterprises",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Infinity Holdings",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await vendorService.AddAsync(new Vendor
            {
                Name = "Momentum Ventures",
                Number = numberSequenceService.GenerateNumber(nameof(Vendor), "", "VND"),
                VendorGroupId = DbInitializer.GetRandomValue(groups, random),
                VendorCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
        }
    }
}
