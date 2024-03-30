using AutoMapper;
using Indotalent.Applications.SalesOrderItems;
using Indotalent.Applications.SalesOrders;
using Indotalent.DTOs;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{

    public class SalesOrderItemChildController : ODataController
    {

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<SalesOrderItem, SalesOrderItemChildDto>();
                CreateMap<SalesOrderItemChildDto, SalesOrderItem>();
            }
        }

        private readonly SalesOrderService _salesOrderService;
        private readonly SalesOrderItemService _salesOrderItemService;
        private readonly IMapper _mapper;

        public SalesOrderItemChildController(
            SalesOrderService salesOrderService,
            IMapper mapper,
            SalesOrderItemService salesOrderItemService)
        {
            _mapper = mapper;
            _salesOrderService = salesOrderService;
            _salesOrderItemService = salesOrderItemService;
        }

        [EnableQuery]
        public IQueryable<SalesOrderItemChildDto> Get()
        {
            const string HeaderKeyName = "ParentId";
            Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
            var parentId = int.Parse(headerValue.ToString());

            return _salesOrderItemService
                .GetAll()
                .Where(x => x.SalesOrderId == parentId)
                .Select(x => _mapper.Map<SalesOrderItemChildDto>(x));
        }


        [EnableQuery]
        [HttpGet("{key}")]
        public SingleResult<SalesOrderItemChildDto> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_salesOrderItemService
                .GetAll()
                .Where(x => x.Id == key)
            .Select(x => _mapper.Map<SalesOrderItemChildDto>(x)));
        }



        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<SalesOrderItemChildDto> delta)
        {
            try
            {
                var salesOrderItem = await _salesOrderItemService
                    .GetAll()
                    .Where(x => x.Id == key)
                    .FirstOrDefaultAsync();

                if (salesOrderItem == null)
                {
                    return NotFound();
                }

                var dto = _mapper.Map<SalesOrderItemChildDto>(salesOrderItem);
                delta.Patch(dto);
                var entity = _mapper.Map(dto, salesOrderItem);
                await _salesOrderItemService.UpdateAsync(entity);

                return Ok(_mapper.Map<SalesOrderItemChildDto>(entity));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SalesOrderItemChildDto salesOrderItem)
        {
            try
            {

                const string HeaderKeyName = "ParentId";
                Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
                var parentId = int.Parse(headerValue.ToString());

                salesOrderItem.SalesOrderId = parentId;
                var entity = _mapper.Map<SalesOrderItem>(salesOrderItem);
                await _salesOrderItemService.AddAsync(entity);

                var dto = _mapper.Map<SalesOrderItem>(entity);
                return Created("SalesOrderItemChild", dto);

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
                var salesOrderItem = await _salesOrderItemService.GetAll()
                    .Where(x => x.Id == key)
                    .FirstOrDefaultAsync();

                if (salesOrderItem == null)
                {
                    return BadRequest();
                }

                await _salesOrderItemService.DeleteByIdAsync(salesOrderItem.Id);

                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
