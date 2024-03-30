using AutoMapper;
using Indotalent.Applications.InventoryTransactions;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.SalesReturns;
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

    public class SalesReturnItemChildController : ODataController
    {

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<InventoryTransaction, SalesReturnItemChildDto>();
                CreateMap<SalesReturnItemChildDto, InventoryTransaction>();
            }
        }

        private readonly NumberSequenceService _numberSequenceService;
        private readonly SalesReturnService _salesReturnService;
        private readonly InventoryTransactionService _inventoryTransactionService;
        private readonly IMapper _mapper;

        public SalesReturnItemChildController(
            NumberSequenceService numberSequenceService,
            SalesReturnService salesReturnService,
            IMapper mapper,
            InventoryTransactionService inventoryTransactionService)
        {
            _mapper = mapper;
            _salesReturnService = salesReturnService;
            _inventoryTransactionService = inventoryTransactionService;
            _numberSequenceService = numberSequenceService;
        }

        [EnableQuery]
        public IQueryable<SalesReturnItemChildDto> Get()
        {
            const string HeaderKeyName = "ParentId";
            Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
            var parentId = int.Parse(headerValue.ToString());

            var moduleName = nameof(SalesReturn) ?? string.Empty;

            return _inventoryTransactionService
                .GetAll()
                .Where(x => x.ModuleId == parentId && x.ModuleName == moduleName)
                .Select(x => _mapper.Map<SalesReturnItemChildDto>(x));
        }


        [EnableQuery]
        [HttpGet("{key}")]
        public SingleResult<SalesReturnItemChildDto> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_inventoryTransactionService
                .GetAll()
                .Where(x => x.Id == key)
            .Select(x => _mapper.Map<SalesReturnItemChildDto>(x)));
        }



        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<SalesReturnItemChildDto> delta)
        {
            try
            {
                var moduleName = nameof(SalesReturn) ?? string.Empty;
                var child = await _inventoryTransactionService
                    .GetAll()
                    .Where(x => x.Id == key && x.ModuleName == moduleName)
                    .FirstOrDefaultAsync();

                if (child == null)
                {
                    return NotFound();
                }

                var dto = _mapper.Map<SalesReturnItemChildDto>(child);
                delta.Patch(dto);
                var entity = _mapper.Map(dto, child);
                await _inventoryTransactionService.UpdateAsync(entity);

                return Ok(_mapper.Map<SalesReturnItemChildDto>(entity));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SalesReturnItemChildDto postInput)
        {
            try
            {

                const string HeaderKeyName = "ParentId";
                Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
                var parentId = int.Parse(headerValue.ToString());
                var moduleName = nameof(SalesReturn) ?? string.Empty;


                var entity = _mapper.Map<InventoryTransaction>(postInput);

                var parent = await _salesReturnService.GetByIdAsync(parentId);
                if (parent != null)
                {
                    entity.ModuleId = parent.Id;
                    entity.ModuleName = moduleName;
                    entity.ModuleCode = "SRN";
                    entity.ModuleNumber = parent.Number ?? string.Empty;
                    entity.MovementDate = parent.ReturnDate!.Value;
                    entity.Status = (InventoryTransactionStatus)parent.Status!;
                }

                entity.Number = _numberSequenceService.GenerateNumber(nameof(InventoryTransaction), "", "IVT");
                await _inventoryTransactionService.AddAsync(entity);

                var dto = _mapper.Map<InventoryTransaction>(entity);
                return Created("SalesReturnItemChild", dto);

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
