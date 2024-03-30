using Indotalent.Infrastructures.Docs;
using Indotalent.Infrastructures.Images;
using Indotalent.Models.Configurations;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FileImage> FileImages { get; set; } = default!;
        public DbSet<FileDocument> FileDocument { get; set; } = default!;
        public DbSet<Company> Company { get; set; } = default!;
        public DbSet<NumberSequence> NumberSequence { get; set; } = default!;
        public DbSet<LogSession> LogSession { get; set; } = default!;
        public DbSet<LogError> LogError { get; set; } = default!;
        public DbSet<LogAnalytic> LogAnalytic { get; set; } = default!;
        public DbSet<CustomerGroup> CustomerGroup { get; set; } = default!;
        public DbSet<CustomerCategory> CustomerCategory { get; set; } = default!;
        public DbSet<VendorGroup> VendorGroup { get; set; } = default!;
        public DbSet<VendorCategory> VendorCategory { get; set; } = default!;
        public DbSet<Warehouse> Warehouse { get; set; } = default!;
        public DbSet<Customer> Customer { get; set; } = default!;
        public DbSet<Vendor> Vendor { get; set; } = default!;
        public DbSet<UnitMeasure> UnitMeasure { get; set; } = default!;
        public DbSet<ProductGroup> ProductGroup { get; set; } = default!;
        public DbSet<Product> Product { get; set; } = default!;
        public DbSet<CustomerContact> CustomerContact { get; set; } = default!;
        public DbSet<VendorContact> VendorContact { get; set; } = default!;
        public DbSet<Tax> Tax { get; set; } = default!;
        public DbSet<SalesOrder> SalesOrder { get; set; } = default!;
        public DbSet<SalesOrderItem> SalesOrderItem { get; set; } = default!;
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; } = default!;
        public DbSet<PurchaseOrderItem> PurchaseOrderItem { get; set; } = default!;
        public DbSet<InventoryTransaction> InventoryTransaction { get; set; } = default!;
        public DbSet<DeliveryOrder> DeliveryOrder { get; set; } = default!;
        public DbSet<GoodsReceive> GoodsReceive { get; set; } = default!;
        public DbSet<SalesReturn> SalesReturn { get; set; } = default!;
        public DbSet<PurchaseReturn> PurchaseReturn { get; set; } = default!;
        public DbSet<TransferIn> TransferIn { get; set; } = default!;
        public DbSet<TransferOut> TransferOut { get; set; } = default!;
        public DbSet<StockCount> StockCount { get; set; } = default!;
        public DbSet<AdjustmentMinus> AdjustmentMinus { get; set; } = default!;
        public DbSet<AdjustmentPlus> AdjustmentPlus { get; set; } = default!;
        public DbSet<Scrapping> Scrapping { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FileImage>().HasKey(f => f.Id);
            modelBuilder.Entity<FileImage>().Property(f => f.OriginalFileName).HasMaxLength(100);
            modelBuilder.Entity<FileDocument>().HasKey(f => f.Id);
            modelBuilder.Entity<FileDocument>().Property(f => f.OriginalFileName).HasMaxLength(100);

            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new LogAnalyticConfiguration());
            modelBuilder.ApplyConfiguration(new LogErrorConfiguration());
            modelBuilder.ApplyConfiguration(new LogSessionConfiguration());
            modelBuilder.ApplyConfiguration(new NumberSequenceConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerGroupConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new VendorGroupConfiguration());
            modelBuilder.ApplyConfiguration(new VendorCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new VendorConfiguration());
            modelBuilder.ApplyConfiguration(new UnitMeasureConfiguration());
            modelBuilder.ApplyConfiguration(new ProductGroupConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerContactConfiguration());
            modelBuilder.ApplyConfiguration(new VendorContactConfiguration());
            modelBuilder.ApplyConfiguration(new TaxConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new DeliveryOrderConfiguration());
            modelBuilder.ApplyConfiguration(new GoodsReceiveConfiguration());
            modelBuilder.ApplyConfiguration(new SalesReturnConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseReturnConfiguration());
            modelBuilder.ApplyConfiguration(new TransferInConfiguration());
            modelBuilder.ApplyConfiguration(new TransferOutConfiguration());
            modelBuilder.ApplyConfiguration(new StockCountConfiguration());
            modelBuilder.ApplyConfiguration(new AdjustmentMinusConfiguration());
            modelBuilder.ApplyConfiguration(new AdjustmentPlusConfiguration());
            modelBuilder.ApplyConfiguration(new ScrappingConfiguration());
        }

    }
}
