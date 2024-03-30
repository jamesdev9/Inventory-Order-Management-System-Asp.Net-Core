using Indotalent.Applications.CustomerCategories;
using Indotalent.Applications.CustomerGroups;
using Indotalent.Applications.Customers;
using Indotalent.Applications.NumberSequences;
using Indotalent.Models.Entities;

namespace Indotalent.Data.Demo
{
    public static class DemoCustomer
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var customerService = services.GetRequiredService<CustomerService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var customerGroupService = services.GetRequiredService<CustomerGroupService>();
            var customerCategoryService = services.GetRequiredService<CustomerCategoryService>();

            var groups = customerGroupService.GetAll().Select(x => x.Id).ToArray();
            var categories = customerCategoryService.GetAll().Select(x => x.Id).ToArray();
            var cities = new string[] { "New York", "Los Angeles", "San Francisco", "Chicago" };

            Random random = new Random();

            await customerService.AddAsync(new Customer
            {
                Name = "Citadel LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Ironclad LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Armada LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Shield LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Alpha LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Capitol LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Federal LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Statewide LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Harmony LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Hope LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Unity LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Prosperity LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Global LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Sunset LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Luxe LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Serenity LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Oasis LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Grandeur LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Bright LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
            await customerService.AddAsync(new Customer
            {
                Name = "Stellar LLC",
                Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                CustomerGroupId = DbInitializer.GetRandomValue(groups, random),
                CustomerCategoryId = DbInitializer.GetRandomValue(categories, random),
                City = DbInitializer.GetRandomString(cities, random)
            });
        }
    }
}
