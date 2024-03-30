using Indotalent.Applications.AdjustmentMinuss;
using Indotalent.Applications.AdjustmentPluss;
using Indotalent.Applications.DeliveryOrders;
using Indotalent.Applications.GoodsReceives;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.PurchaseOrderItems;
using Indotalent.Applications.PurchaseOrders;
using Indotalent.Applications.PurchaseReturns;
using Indotalent.Applications.SalesOrderItems;
using Indotalent.Applications.SalesOrders;
using Indotalent.Applications.SalesReturns;
using Indotalent.Applications.Scrappings;
using Indotalent.Applications.StockCounts;
using Indotalent.Applications.TransferIns;
using Indotalent.Applications.TransferOuts;
using Indotalent.Applications.Warehouses;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Indotalent.Pages.Dashboards
{
    [Authorize]
    public class DefaultDashboardModel : PageModel
    {
        private readonly SalesOrderService _salesOrderService;
        private readonly PurchaseOrderService _purchaseOrderService;
        private readonly SalesReturnService _salesReturnService;
        private readonly PurchaseReturnService _purchaseReturnService;
        private readonly DeliveryOrderService _deliveryOrderService;
        private readonly GoodsReceiveService _goodsReceiveService;
        private readonly TransferOutService _transferOutService;
        private readonly TransferInService _transferInService;
        private readonly AdjustmentPlusService _adjustmentPlusService;
        private readonly AdjustmentMinusService _adjustmentMinusService;
        private readonly ScrappingService _scrappingService;
        private readonly StockCountService _stockCountService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        private readonly SalesOrderItemService _salesOrderItemService;
        private readonly PurchaseOrderItemService _purchaseOrderItemService;
        private readonly WarehouseService _warehouseService;

        public DefaultDashboardModel(
                SalesOrderService salesOrderService,
                PurchaseOrderService purchaseOrderService,
                SalesReturnService salesReturnService,
                PurchaseReturnService purchaseReturnService,
                DeliveryOrderService deliveryOrderService,
                GoodsReceiveService goodsReceiveService,
                TransferOutService transferOutService,
                TransferInService transferInService,
                AdjustmentPlusService adjustmentPlusService,
                AdjustmentMinusService adjustmentMinusService,
                ScrappingService scrappingService,
                StockCountService stockCountService,
                InventoryTransactionService inventoryTransactionService,
                SalesOrderItemService salesOrderItemService,
                PurchaseOrderItemService purchaseOrderItemService,
                WarehouseService warehouseService
            )
        {
            _salesOrderService = salesOrderService;
            _purchaseOrderService = purchaseOrderService;
            _purchaseReturnService = purchaseReturnService;
            _deliveryOrderService = deliveryOrderService;
            _goodsReceiveService = goodsReceiveService;
            _transferOutService = transferOutService;
            _transferInService = transferInService;
            _adjustmentPlusService = adjustmentPlusService;
            _adjustmentMinusService = adjustmentMinusService;
            _salesReturnService = salesReturnService;
            _scrappingService = scrappingService;
            _stockCountService = stockCountService;
            _inventoryTransactionService = inventoryTransactionService;
            _salesOrderItemService = salesOrderItemService;
            _purchaseOrderItemService = purchaseOrderItemService;
            _warehouseService = warehouseService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        //card

        public string CardSalesQty { get; set; } = string.Empty;
        public string CardSalesReturnQty { get; set; } = string.Empty;
        public string CardPurchaseQty { get; set; } = string.Empty;
        public string CardPurchaseReturnQty { get; set; } = string.Empty;
        public string CardDeliveryOrderQty { get; set; } = string.Empty;
        public string CardGoodsReceiveQty { get; set; } = string.Empty;
        public string CardTransferOutQty { get; set; } = string.Empty;
        public string CardTransferInQty { get; set; } = string.Empty;

        //chart
        public string SalesCusomerGroupChartJson { get; set; } = string.Empty;
        public string PurchaseVendorGroupChartJson { get; set; } = string.Empty;
        public string SalesCusomerCategoryChartJson { get; set; } = string.Empty;
        public string PurchaseVendorCategoryChartJson { get; set; } = string.Empty;
        public string SalesPurchaseProductGroupChartJson { get; set; } = string.Empty;
        public string InventoryStockGroupChartJson { get; set; } = string.Empty;

        //grid
        public string LatestPurchaseOrderJson { get; set; } = string.Empty;
        public string LatestSalesOrderJson { get; set; } = string.Empty;

        public void OnGet()
        {
            this.SetupViewDataTitleFromUrl();

            //card

            CardSalesQty = _salesOrderItemService
                .GetAll()
                .Include(x => x.SalesOrder)
                .Include(x => x.Product)
                .Where(x => x.Product!.Physical == true && x.SalesOrder!.OrderStatus >= Models.Enums.SalesOrderStatus.Confirmed)
                .Sum(x => x.Quantity)!.Value
                .ToString("N2") + " Qty.";

            CardSalesReturnQty = _inventoryTransactionService
                .GetAll()
                .Include(x => x.Warehouse)
                .Where(x => x.ModuleName == nameof(SalesReturn) && x.Warehouse!.SystemWarehouse == false && x.Status >= Models.Enums.InventoryTransactionStatus.Confirmed)
                .Sum(x => x.Movement)
                .ToString("N2") + " Qty.";

            CardPurchaseQty = _purchaseOrderItemService
                .GetAll()
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Product)
                .Where(x => x.Product!.Physical == true && x.PurchaseOrder!.OrderStatus >= Models.Enums.PurchaseOrderStatus.Confirmed)
                .Sum(x => x.Quantity)!.Value
                .ToString("N2") + " Qty.";

            CardPurchaseReturnQty = _inventoryTransactionService
                .GetAll()
                .Include(x => x.Warehouse)
                .Where(x => x.ModuleName == nameof(PurchaseReturn) && x.Warehouse!.SystemWarehouse == false && x.Status >= Models.Enums.InventoryTransactionStatus.Confirmed)
                .Sum(x => x.Movement)
                .ToString("N2") + " Qty.";

            CardDeliveryOrderQty = _inventoryTransactionService
                .GetAll()
                .Include(x => x.Warehouse)
                .Where(x => x.ModuleName == nameof(DeliveryOrder) && x.Warehouse!.SystemWarehouse == false && x.Status >= Models.Enums.InventoryTransactionStatus.Confirmed)
                .Sum(x => x.Movement)
                .ToString("N2") + " Qty.";

            CardGoodsReceiveQty = _inventoryTransactionService
                .GetAll()
                .Include(x => x.Warehouse)
                .Where(x => x.ModuleName == nameof(GoodsReceive) && x.Warehouse!.SystemWarehouse == false && x.Status >= Models.Enums.InventoryTransactionStatus.Confirmed)
                .Sum(x => x.Movement)
                .ToString("N2") + " Qty.";

            CardTransferOutQty = _inventoryTransactionService
                .GetAll()
                .Include(x => x.Warehouse)
                .Where(x => x.ModuleName == nameof(TransferOut) && x.Warehouse!.SystemWarehouse == false && x.Status >= Models.Enums.InventoryTransactionStatus.Confirmed)
                .Sum(x => x.Movement)
                .ToString("N2") + " Qty.";

            CardTransferInQty = _inventoryTransactionService
                .GetAll()
                .Include(x => x.Warehouse)
                .Where(x => x.ModuleName == nameof(TransferIn) && x.Warehouse!.SystemWarehouse == false && x.Status >= Models.Enums.InventoryTransactionStatus.Confirmed)
                .Sum(x => x.Movement)
                .ToString("N2") + " Qty.";



            SalesCustomerGroupChart();
            PurchaseVendorGroupChart();
            SalesCustomerCategoryChart();
            PurchaseVendorCategoryChart();
            SalesPurchaseProductGroupChart();

            LatestPurchaseOrderJson = JsonConvert.SerializeObject(
                _purchaseOrderItemService
                .GetAll()
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Product)
                .Where(x => x.PurchaseOrder!.OrderStatus >= PurchaseOrderStatus.Confirmed)
                .OrderByDescending(x => x.PurchaseOrder!.OrderDate)
                .Take(30)
                .Select(x => new
                {
                    OrderDate = x.PurchaseOrder!.OrderDate,
                    PurchaseOrder = x.PurchaseOrder!.Number,
                    Product = x.Product!.Name,
                    Total = x.Total
                })
                .ToList()
            );

            LatestSalesOrderJson = JsonConvert.SerializeObject(
                _salesOrderItemService
                .GetAll()
                .Include(x => x.SalesOrder)
                .Include(x => x.Product)
                .Where(x => x.SalesOrder!.OrderStatus >= SalesOrderStatus.Confirmed)
                .OrderByDescending(x => x.SalesOrder!.OrderDate)
                .Take(30)
                .Select(x => new
                {
                    OrderDate = x.SalesOrder!.OrderDate,
                    SalesOrder = x.SalesOrder!.Number,
                    Product = x.Product!.Name,
                    Total = x.Total
                })
                .ToList()
            );

            InventoryStockGroupChart();


        }

        private void InventoryStockGroupChart()
        {
            var data =
                _inventoryTransactionService
                .GetAll()
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .Where(x =>
                    x.Status >= Models.Enums.InventoryTransactionStatus.Confirmed &&
                    x.Warehouse!.SystemWarehouse == false &&
                    x.Product!.Physical == true
                )
                .GroupBy(x => new { x.WarehouseId, x.ProductId })
                .Select(group => new
                {
                    WarehouseId = group.Key.WarehouseId,
                    ProductId = group.Key.ProductId,
                    Warehouse = group.Max(x => x.Warehouse!.Name),
                    Product = group.Max(x => x.Product!.Name),
                    Stock = group.Sum(x => x.Stock),
                    Id = group.Max(x => x.Id),
                    RowGuid = group.Max(x => x.RowGuid),
                    CreatedAtUtc = group.Max(x => x.CreatedAtUtc)
                })
                .ToList();

            var groups = _warehouseService.GetAll().Where(x => x.SystemWarehouse == false).Select(x => x.Name).ToList();

            InventoryStockGroupChartJson = JsonConvert.SerializeObject(
                    groups
                    .Select(wh => new BarSeries
                    {
                        type = "Column",
                        xName = "x",
                        width = 2,
                        yName = "y",
                        name = wh,
                        columnSpacing = 0.1,
                        tooltipMappingName = "tooltipMappingName",
                        dataSource = data
                            .Where(x => x.Warehouse == wh)
                            .Select(x => new BarDataItem
                            {
                                x = x.Product ?? string.Empty,
                                tooltipMappingName = x.Product ?? string.Empty,
                                y = (int)x.Stock
                            }).ToList()
                    })
                    .ToList()
                );
        }
        private void SalesPurchaseProductGroupChart()
        {
            var sales = _salesOrderItemService
                .GetAll()
                .Include(x => x.Product)
                    .ThenInclude(x => x!.ProductGroup)
                .Where(x => x.Product!.Physical == true)
                .Select(x => new
                {
                    Transaction = "Sales",
                    Group = x.Product!.ProductGroup!.Name,
                    Quantity = x.Quantity
                });

            var purchase = _purchaseOrderItemService
                .GetAll()
                .Include(x => x.Product)
                    .ThenInclude(x => x!.ProductGroup)
                .Where(x => x.Product!.Physical == true)
                .Select(x => new
                {
                    Transaction = "Purchase",
                    Group = x.Product!.ProductGroup!.Name,
                    Quantity = x.Quantity
                });

            var combination = sales.Concat(purchase).ToList();

            var data = combination
                .GroupBy(x => new { x.Transaction, x.Group })
                .Select(g => new
                {
                    Transaction = g.Key.Transaction,
                    Group = g.Key.Group,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            var transactions = new List<string>()
            {
                "Sales",
                "Purchase"
            };

            SalesPurchaseProductGroupChartJson = JsonConvert.SerializeObject(
                    transactions
                    .Select(trx => new BarSeries
                    {
                        type = "StackingColumn",
                        xName = "x",
                        width = 2,
                        yName = "y",
                        name = trx,
                        columnSpacing = 0.1,
                        tooltipMappingName = "tooltipMappingName",
                        dataSource = data
                            .Where(x => x.Transaction == trx)
                            .Select(x => new BarDataItem
                            {
                                x = x.Group,
                                tooltipMappingName = x.Group,
                                y = (int)x.Quantity!.Value
                            }).ToList()
                    })
                    .ToList()
                );
        }

        private void PurchaseVendorCategoryChart()
        {
            var data = _purchaseOrderItemService
                .GetAll()
                .Include(x => x.PurchaseOrder)
                    .ThenInclude(x => x!.Vendor)
                        .ThenInclude(x => x!.VendorCategory)
                .Include(x => x.Product)
                .Where(x => x.Product!.Physical == true)
                .Select(x => new
                {
                    Status = x.PurchaseOrder!.OrderStatus,
                    VendorCategoryName = x.PurchaseOrder!.Vendor!.VendorCategory!.Name,
                    Quantity = x.Quantity
                })
                .GroupBy(x => new { x.Status, x.VendorCategoryName })
                .Select(g => new
                {
                    Status = g.Key.Status,
                    VendorCategoryName = g.Key.VendorCategoryName,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            PurchaseVendorCategoryChartJson = JsonConvert.SerializeObject(
                    Enum.GetValues(typeof(PurchaseOrderStatus))
                    .Cast<PurchaseOrderStatus>()
                    .Select(status => new BarSeries
                    {
                        type = "Column",
                        xName = "x",
                        width = 2,
                        yName = "y",
                        name = Enum.GetName(typeof(PurchaseOrderStatus), status)!,
                        columnSpacing = 0.1,
                        tooltipMappingName = "tooltipMappingName",
                        dataSource = data
                            .Where(x => x.Status == status)
                            .Select(x => new BarDataItem
                            {
                                x = x.VendorCategoryName,
                                tooltipMappingName = x.VendorCategoryName,
                                y = (int)x.Quantity!.Value
                            }).ToList()
                    })
                    .ToList()
                );
        }
        private void SalesCustomerCategoryChart()
        {
            var data = _salesOrderItemService
                .GetAll()
                .Include(x => x.SalesOrder)
                    .ThenInclude(x => x!.Customer)
                        .ThenInclude(x => x!.CustomerCategory)
                .Include(x => x.Product)
                .Where(x => x.Product!.Physical == true)
                .Select(x => new
                {
                    Status = x.SalesOrder!.OrderStatus,
                    CustomerCategoryName = x.SalesOrder!.Customer!.CustomerCategory!.Name,
                    Quantity = x.Quantity
                })
                .GroupBy(x => new { x.Status, x.CustomerCategoryName })
                .Select(g => new
                {
                    Status = g.Key.Status,
                    CustomerCategoryName = g.Key.CustomerCategoryName,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            SalesCusomerCategoryChartJson = JsonConvert.SerializeObject(
                    Enum.GetValues(typeof(SalesOrderStatus))
                    .Cast<SalesOrderStatus>()
                    .Select(status => new BarSeries
                    {
                        type = "Bar",
                        xName = "x",
                        width = 2,
                        yName = "y",
                        name = Enum.GetName(typeof(SalesOrderStatus), status)!,
                        columnSpacing = 0.1,
                        tooltipMappingName = "tooltipMappingName",
                        dataSource = data
                            .Where(x => x.Status == status)
                            .Select(x => new BarDataItem
                            {
                                x = x.CustomerCategoryName,
                                tooltipMappingName = x.CustomerCategoryName,
                                y = (int)x.Quantity!.Value
                            }).ToList()
                    })
                    .ToList()
                );
        }

        private void PurchaseVendorGroupChart()
        {
            var data = _purchaseOrderItemService
                .GetAll()
                .Include(x => x.PurchaseOrder)
                    .ThenInclude(x => x!.Vendor)
                        .ThenInclude(x => x!.VendorGroup)
                .Include(x => x.Product)
                .Where(x => x.Product!.Physical == true)
                .Select(x => new
                {
                    Status = x.PurchaseOrder!.OrderStatus,
                    VendorGroupName = x.PurchaseOrder!.Vendor!.VendorGroup!.Name,
                    Quantity = x.Quantity
                })
                .GroupBy(x => new { x.Status, x.VendorGroupName })
                .Select(g => new
                {
                    Status = g.Key.Status,
                    VendorGroupName = g.Key.VendorGroupName,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            PurchaseVendorGroupChartJson = JsonConvert.SerializeObject(
                    Enum.GetValues(typeof(PurchaseOrderStatus))
                    .Cast<PurchaseOrderStatus>()
                    .Select(status => new BarSeries
                    {
                        type = "Bar",
                        xName = "x",
                        width = 2,
                        yName = "y",
                        name = Enum.GetName(typeof(PurchaseOrderStatus), status)!,
                        columnSpacing = 0.1,
                        tooltipMappingName = "tooltipMappingName",
                        dataSource = data
                            .Where(x => x.Status == status)
                            .Select(x => new BarDataItem
                            {
                                x = x.VendorGroupName,
                                tooltipMappingName = x.VendorGroupName,
                                y = (int)x.Quantity!.Value
                            }).ToList()
                    })
                    .ToList()
                );
        }


        private void SalesCustomerGroupChart()
        {
            var data = _salesOrderItemService
                .GetAll()
                .Include(x => x.SalesOrder)
                    .ThenInclude(x => x!.Customer)
                        .ThenInclude(x => x!.CustomerGroup)
                .Include(x => x.Product)
                .Where(x => x.Product!.Physical == true)
                .Select(x => new
                {
                    Status = x.SalesOrder!.OrderStatus,
                    CustomerGroupName = x.SalesOrder!.Customer!.CustomerGroup!.Name,
                    Quantity = x.Quantity
                })
                .GroupBy(x => new { x.Status, x.CustomerGroupName })
                .Select(g => new
                {
                    Status = g.Key.Status,
                    CustomerGroupName = g.Key.CustomerGroupName,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            SalesCusomerGroupChartJson = JsonConvert.SerializeObject(
                    Enum.GetValues(typeof(SalesOrderStatus))
                    .Cast<SalesOrderStatus>()
                    .Select(status => new BarSeries
                    {
                        type = "Column",
                        xName = "x",
                        width = 2,
                        yName = "y",
                        name = Enum.GetName(typeof(SalesOrderStatus), status)!,
                        columnSpacing = 0.1,
                        tooltipMappingName = "tooltipMappingName",
                        dataSource = data
                            .Where(x => x.Status == status)
                            .Select(x => new BarDataItem
                            {
                                x = x.CustomerGroupName,
                                tooltipMappingName = x.CustomerGroupName,
                                y = (int)x.Quantity!.Value
                            }).ToList()
                    })
                    .ToList()
                );
        }

    }

    public class BarSeries
    {
        public string type { get; set; } = string.Empty;
        public string xName { get; set; } = string.Empty;
        public int width { get; set; }
        public string yName { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public double columnSpacing { get; set; }
        public string tooltipMappingName { get; set; } = string.Empty;
        public List<BarDataItem> dataSource { get; set; } = new List<BarDataItem>();
    }

    public class BarDataItem
    {
        public string x { get; set; } = string.Empty;
        public int y { get; set; }
        public string tooltipMappingName { get; set; } = string.Empty;
    }




}
