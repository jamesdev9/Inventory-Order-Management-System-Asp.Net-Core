using AutoMapper;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.TransferIns;
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

    public class TransferInItemChildController : ODataController
    {

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<InventoryTransaction, TransferInItemChildDto>();
                CreateMap<TransferInItemChildDto, InventoryTransaction>();
            }
        }

        private readonly NumberSequenceService _numberSequenceService;
        private readonly TransferInService _transferInService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        private readonly IMapper _mapper;

        public TransferInItemChildController(
            NumberSequenceService numberSequenceService,
            TransferInService transferInService,
            IMapper mapper,
            InventoryTransactionService inventoryTransactionService)
        {
            _mapper = mapper;
            _transferInService = transferInService;
            _inventoryTransactionService = inventoryTransactionService;
            _numberSequenceService = numberSequenceService;
        }

        [EnableQuery]
        public IQueryable<TransferInItemChildDto> Get()
        {
            const string HeaderKeyName = "ParentId";
            Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
            var parentId = int.Parse(headerValue.ToString());

            var moduleName = nameof(TransferIn) ?? string.Empty;

            return _inventoryTransactionService
                .GetAll()
                .Where(x => x.ModuleId == parentId && x.ModuleName == moduleName)
                .Select(x => _mapper.Map<TransferInItemChildDto>(x));
        }


        [EnableQuery]
        [HttpGet("{key}")]
        public SingleResult<TransferInItemChildDto> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_inventoryTransactionService
                .GetAll()
                .Where(x => x.Id == key)
            .Select(x => _mapper.Map<TransferInItemChildDto>(x)));
        }



        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<TransferInItemChildDto> delta)
        {
            try
            {
                var moduleName = nameof(TransferIn) ?? string.Empty;
                var child = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.Id == key && x.ModuleName == moduleName)
                    .FirstOrDefaultAsync();

                if (child == null)
                {
                    return NotFound();
                }

                var dto = _mapper.Map<TransferInItemChildDto>(child);
                delta.Patch(dto);
                var entity = _mapper.Map(dto, child);
                await _inventoryTransactionService.UpdateAsync(entity);

                return Ok(_mapper.Map<TransferInItemChildDto>(entity));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TransferInItemChildDto postInput)
        {
            try
            {

                const string HeaderKeyName = "ParentId";
                Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
                var parentId = int.Parse(headerValue.ToString());
                var moduleName = nameof(TransferIn) ?? string.Empty;


                var entity = _mapper.Map<InventoryTransaction>(postInput);

                var parent = await _transferInService
                    .GetAll()
                    .Include(x => x.TransferOut)
                    .Where(x => x.Id == parentId)
                    .FirstOrDefaultAsync();

                if (parent != null)
                {
                    entity.ModuleId = parent.Id;
                    entity.ModuleName = moduleName;
                    entity.ModuleCode = "TO-IN";
                    entity.ModuleNumber = parent.Number ?? string.Empty;
                    entity.MovementDate = parent.TransferReceiveDate!.Value;
                    entity.Status = (InventoryTransactionStatus)parent.Status!;
                    entity.WarehouseId = parent.TransferOut!.WarehouseToId!.Value;
                }

                entity.Number = _numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT");
                await _inventoryTransactionService.AddAsync(entity);

                var dto = _mapper.Map<InventoryTransaction>(entity);
                return Created("TransferInItemChild", dto);

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
