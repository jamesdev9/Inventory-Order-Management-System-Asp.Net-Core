using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.ProductGroups;
using Indotalent.Applications.Products;
using Indotalent.Applications.UnitMeasures;
using Indotalent.Models.Entities;

namespace Indotalent.Data.Demo
{
    public static class DemoProduct
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var productService = services.GetRequiredService<ProductService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var productGroupService = services.GetRequiredService<ProductGroupService>();
            var unitMeasureService = services.GetRequiredService<UnitMeasureService>();

            var groups = productGroupService.GetAll().Select(x => x.Id).ToArray();
            var measures = unitMeasureService.GetAll().Select(x => x.Id).ToArray();
            var prices = new double[] { 500.0, 1000.0, 2000.0, 3000.0, 4000.0, 5000.0 };

            Random random = new Random();

            await productService.AddAsync(new Product
            {
                Name = "Dell Servers",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Hardware").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 5000.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Dell Desktop Computers",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Hardware").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 2000.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Dell Laptops",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Hardware").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 3000.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Hitachi Storage",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Hardware").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 1500.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Epson Printers",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Hardware").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 1000.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Network Cables",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Networking").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "m").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 100.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Routers and Switches",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Networking").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 1000.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Antennas and Signal Boosters",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Networking").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 2000.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Wifii",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Networking").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 1000.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "HDD 500",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Storage").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 500.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "HDD 1T",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Storage").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 800.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "SSD 500",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Storage").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 1000.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "SSD 1T",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Storage").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 1500.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Dell Keyboard",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Device").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 700.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Dell Mouse",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Device").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 500.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Dell Monitor 27inch",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Device").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 1000.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Dell Monitor 32inch",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Device").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 1500.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "Dell Webcams",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Device").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = true,
                UnitPrice = 500.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "D365 License",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Software").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "unit").FirstOrDefault()!.Id,
                Physical = false,
                UnitPrice = 800.0,
            });
            await productService.AddAsync(new Product
            {
                Name = "IT Security",
                Number = numberSequenceService.GenerateNumber(nameof(Product), "", "ART"),
                ProductGroupId = productGroupService.GetAll().Where(x => x.Name == "Service").FirstOrDefault()!.Id,
                UnitMeasureId = unitMeasureService.GetAll().Where(x => x.Name == "hour").FirstOrDefault()!.Id,
                Physical = false,
                UnitPrice = 500.0,
            });
        }
    }
}
