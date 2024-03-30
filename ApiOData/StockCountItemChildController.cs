using AutoMapper;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.StockCounts;
using Indotalent.DTOs;
using Indotalent.Models.Entities;
using Indotalent.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{

    public class StockCountItemChildController : ODataController
    {

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<InventoryTransaction, StockCountItemChildDto>();
                CreateMap<StockCountItemChildDto, InventoryTransaction>();
            }
        }

        private readonly NumberSequenceService _numberSequenceService;
        private readonly StockCountService _stockCountService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        private readonly IMapper _mapper;

        public StockCountItemChildController(
            NumberSequenceService numberSequenceService,
            StockCountService stockCountService,
            IMapper mapper,
            InventoryTransactionService inventoryTransactionService)
        {
            _mapper = mapper;
            _stockCountService = stockCountService;
            _inventoryTransactionService = inventoryTransactionService;
            _numberSequenceService = numberSequenceService;
        }

        [EnableQuery]
        public IQueryable<StockCountItemChildDto> Get()
        {
            const string HeaderKeyName = "ParentId";
            Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
            var parentId = int.Parse(headerValue.ToString());

            var moduleName = nameof(StockCount) ?? string.Empty;

            return _inventoryTransactionService
                .GetAll()
                .Where(x => x.ModuleId == parentId && x.ModuleName == moduleName)
                .Select(x => _mapper.Map<StockCountItemChildDto>(x));
        }


        [EnableQuery]
        [HttpGet("{key}")]
        public SingleResult<StockCountItemChildDto> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_inventoryTransactionService
                .GetAll()
                .Where(x => x.Id == key)
            .Select(x => _mapper.Map<StockCountItemChildDto>(x)));
        }



        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<StockCountItemChildDto> delta)
        {
            try
            {
                var moduleName = nameof(StockCount) ?? string.Empty;
                var child = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.Id == key && x.ModuleName == moduleName)
                    .FirstOrDefaultAsync();

                if (child == null)
                {
                    return NotFound();
                }

                var dto = _mapper.Map<StockCountItemChildDto>(child);
                delta.Patch(dto);
                var entity = _mapper.Map(dto, child);

                entity.QtySCSys = _inventoryTransactionService.GetStock(entity.WarehouseId, entity.ProductId);
                entity.QtySCCount = entity.QtySCCount > 0 ? entity.QtySCCount : 0;

                _inventoryTransactionService.CalculateInvenTrans(entity);
                await _inventoryTransactionService.UpdateAsync(entity);

                return Ok(_mapper.Map<StockCountItemChildDto>(entity));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StockCountItemChildDto postInput)
        {
            try
            {

                const string HeaderKeyName = "ParentId";
                Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
                var parentId = int.Parse(headerValue.ToString());
                var moduleName = nameof(StockCount) ?? string.Empty;


                var entity = _mapper.Map<InventoryTransaction>(postInput);

                var parent = await _stockCountService.GetByIdAsync(parentId);
                if (parent != null)
                {
                    entity.ModuleId = parent.Id;
                    entity.ModuleName = moduleName;
                    entity.ModuleCode = "Opname";
                    entity.ModuleNumber = parent.Number ?? string.Empty;
                    entity.MovementDate = parent.CountDate!.Value;
                    entity.Status = (InventoryTransactionStatus)parent.Status!;
                    entity.WarehouseId = parent.WarehouseId;
                    entity.QtySCSys = _inventoryTransactionService.GetStock(entity.WarehouseId, entity.ProductId);
                    entity.QtySCCount = entity.QtySCCount > 0 ? entity.QtySCCount : 0;
                    entity.Number = _numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT");

                    _inventoryTransactionService.CalculateInvenTrans(entity);
                    await _inventoryTransactionService.AddAsync(entity);

                }

                var dto = _mapper.Map<InventoryTransaction>(entity);
                return Created("StockCountItemChild", dto);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            try
            {
                var child = await _inventoryTransactionService.GetAll()
                    .Where(x => x.Id == key)
                    .FirstOrDefaultAsync();

                if (child == null)
                {
                    return BadRequest();
                }

                await _inventoryTransactionService.DeleteByIdAsync(child.Id);

                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
