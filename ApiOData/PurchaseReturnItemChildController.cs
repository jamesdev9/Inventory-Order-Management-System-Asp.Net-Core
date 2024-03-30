using AutoMapper;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.PurchaseReturns;
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

    public class PurchaseReturnItemChildController : ODataController
    {

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<InventoryTransaction, PurchaseReturnItemChildDto>();
                CreateMap<PurchaseReturnItemChildDto, InventoryTransaction>();
            }
        }

        private readonly NumberSequenceService _numberSequenceService;
        private readonly PurchaseReturnService _purchaseReturnService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        private readonly IMapper _mapper;

        public PurchaseReturnItemChildController(
            NumberSequenceService numberSequenceService,
            PurchaseReturnService purchaseReturnService,
            IMapper mapper,
            InventoryTransactionService inventoryTransactionService)
        {
            _mapper = mapper;
            _purchaseReturnService = purchaseReturnService;
            _inventoryTransactionService = inventoryTransactionService;
            _numberSequenceService = numberSequenceService;
        }

        [EnableQuery]
        public IQueryable<PurchaseReturnItemChildDto> Get()
        {
            const string HeaderKeyName = "ParentId";
            Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
            var parentId = int.Parse(headerValue.ToString());

            var moduleName = nameof(PurchaseReturn) ?? string.Empty;

            return _inventoryTransactionService
                .GetAll()
                .Where(x => x.ModuleId == parentId && x.ModuleName == moduleName)
                .Select(x => _mapper.Map<PurchaseReturnItemChildDto>(x));
        }


        [EnableQuery]
        [HttpGet("{key}")]
        public SingleResult<PurchaseReturnItemChildDto> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_inventoryTransactionService
                .GetAll()
                .Where(x => x.Id == key)
            .Select(x => _mapper.Map<PurchaseReturnItemChildDto>(x)));
        }



        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<PurchaseReturnItemChildDto> delta)
        {
            try
            {
                var moduleName = nameof(PurchaseReturn) ?? string.Empty;
                var child = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.Id == key && x.ModuleName == moduleName)
                    .FirstOrDefaultAsync();

                if (child == null)
                {
                    return NotFound();
                }

                var dto = _mapper.Map<PurchaseReturnItemChildDto>(child);
                delta.Patch(dto);
                var entity = _mapper.Map(dto, child);
                await _inventoryTransactionService.UpdateAsync(entity);

                return Ok(_mapper.Map<PurchaseReturnItemChildDto>(entity));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PurchaseReturnItemChildDto postInput)
        {
            try
            {

                const string HeaderKeyName = "ParentId";
                Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
                var parentId = int.Parse(headerValue.ToString());
                var moduleName = nameof(PurchaseReturn) ?? string.Empty;


                var entity = _mapper.Map<InventoryTransaction>(postInput);

                var parent = await _purchaseReturnService.GetByIdAsync(parentId);
                if (parent != null)
                {
                    entity.ModuleId = parent.Id;
                    entity.ModuleName = moduleName;
                    entity.ModuleCode = "PRN";
                    entity.ModuleNumber = parent.Number ?? string.Empty;
                    entity.MovementDate = parent.ReturnDate!.Value;
                    entity.Status = (InventoryTransactionStatus)parent.Status!;
                }

                entity.Number = _numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT");
                await _inventoryTransactionService.AddAsync(entity);

                var dto = _mapper.Map<InventoryTransaction>(entity);
                return Created("PurchaseReturnItemChild", dto);

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
