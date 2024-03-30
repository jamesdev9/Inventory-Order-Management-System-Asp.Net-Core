using AutoMapper;
using Indotalent.Applications.CustomerContacts;
using Indotalent.Applications.NumberSequences;
using Indotalent.DTOs;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{

    public class CustomerContactChildController : ODataController
    {

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CustomerContact, CustomerContactChildDto>();
                CreateMap<CustomerContactChildDto, CustomerContact>();
            }
        }


        private readonly CustomerContactService _customerContactService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly IMapper _mapper;

        public CustomerContactChildController(
            IMapper mapper,
            CustomerContactService customerContactService,
            NumberSequenceService numberSequenceService)
        {
            _mapper = mapper;
            _customerContactService = customerContactService;
            _numberSequenceService = numberSequenceService;
        }

        [EnableQuery]
        public IQueryable<CustomerContactChildDto> Get()
        {
            const string HeaderKeyName = "ParentId";
            Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
            var parentId = int.Parse(headerValue.ToString());

            return _customerContactService
                .GetAll()
                .Where(x => x.CustomerId == parentId)
                .Select(x => _mapper.Map<CustomerContactChildDto>(x));
        }


        [EnableQuery]
        [HttpGet("{key}")]
        public SingleResult<CustomerContactChildDto> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_customerContactService
                .GetAll()
                .Where(x => x.Id == key)
                .Select(x => _mapper.Map<CustomerContactChildDto>(x)));
        }



        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<CustomerContactChildDto> delta)
        {
            try
            {
                var customerContact = await _customerContactService.GetByIdAsync(key);
                if (customerContact == null)
                {
                    return NotFound();
                }

                var dto = _mapper.Map<CustomerContactChildDto>(customerContact);
                delta.Patch(dto);
                var entity = _mapper.Map(dto, customerContact);
                await _customerContactService.UpdateAsync(entity);

                return Ok(_mapper.Map<CustomerContactChildDto>(entity));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerContactChildDto customerContact)
        {
            try
            {

                const string HeaderKeyName = "ParentId";
                Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
                var parentId = int.Parse(headerValue.ToString());

                customerContact.CustomerId = parentId;
                customerContact.Number = _numberSequenceService.GenerateNumber(nameof(CustomerContact), "", "CC");
                var entity = _mapper.Map<CustomerContact>(customerContact);
                await _customerContactService.AddAsync(entity);
                var dto = _mapper.Map<CustomerContact>(entity);

                return Created("CustomerContactChild", dto);

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
                var customerContact = await _customerContactService.GetByIdAsync(key);
                if (customerContact == null)
                {
                    return BadRequest();
                }

                await _customerContactService.DeleteByIdAsync(customerContact.Id);

                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
