using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.TransferIns;
using Indotalent.Applications.TransferOuts;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;

namespace Indotalent.Data.Demo
{
    public static class DemoTransferIn
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var transferInService = services.GetRequiredService<TransferInService>();
            var transferOutService = services.GetRequiredService<TransferOutService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var inventoryTransactionService = services.GetRequiredService<InventoryTransactionService>();
            Random random = new Random();
            int transferInStatusLength = Enum.GetNames(typeof(TransferStatus)).Length;

            var transferOuts = transferOutService
                .GetAll()
                .Where(x => x.Status >= TransferStatus.Confirmed)
                .ToList();

            foreach (var transferOut in transferOuts)
            {
                bool process = random.Next(2) == 0;

                if (process)
                {
                    continue;
                }

                var transferIn = new TransferIn
                {
                    Number = numberSequenceService.GenerateNumber(nameof(TransferIn), "", "IN"),
                    TransferReceiveDate = transferOut.TransferReleaseDate?.AddDays(random.Next(1, 5)),
                    Status = (TransferStatus)random.Next(0, transferInStatusLength),
                    TransferOutId = transferOut.Id,
                };
                await transferInService.AddAsync(transferIn);

                var items = inventoryTransactionService
                    .GetAll()
                    .Where(x => x.ModuleId == transferOut.Id && x.ModuleName == nameof(TransferOut))
                    .ToList();

                foreach (var item in items)
                {
                    var inventoryTransaction = new InventoryTransaction
                    {
                        ModuleId = transferIn.Id,
                        ModuleName = nameof(TransferIn),
                        ModuleCode = "TO-IN",
                        ModuleNumber = transferIn.Number,
                        MovementDate = transferIn.TransferReceiveDate!.Value,
                        Status = (InventoryTransactionStatus)transferIn.Status,
                        Number = numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT"),
                        WarehouseId = transferOut.WarehouseToId!.Value,
                        ProductId = item.ProductId,
                        Movement = item.Movement
                    };

                    inventoryTransactionService.CalculateInvenTrans(inventoryTransaction);
                    await inventoryTransactionService.AddAsync(inventoryTransaction);
                }


            }

        }
    }
}
