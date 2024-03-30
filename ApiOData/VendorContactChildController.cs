using AutoMapper;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.VendorContacts;
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

    public class VendorContactChildController : ODataController
    {

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<VendorContact, VendorContactChildDto>();
                CreateMap<VendorContactChildDto, VendorContact>();
            }
        }


        private readonly VendorContactService _vendorContactService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly IMapper _mapper;

        public VendorContactChildController(
            IMapper mapper,
            VendorContactService vendorContactService,
            NumberSequenceService numberSequenceService)
        {
            _mapper = mapper;
            _vendorContactService = vendorContactService;
            _numberSequenceService = numberSequenceService;
        }

        [EnableQuery]
        public IQueryable<VendorContactChildDto> Get()
        {
            const string HeaderKeyName = "ParentId";
            Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
            var parentId = int.Parse(headerValue.ToString());

            return _vendorContactService
                .GetAll()
                .Where(x => x.VendorId == parentId)
                .Select(x => _mapper.Map<VendorContactChildDto>(x));
        }


        [EnableQuery]
        [HttpGet("{key}")]
        public SingleResult<VendorContactChildDto> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_vendorContactService
                .GetAll()
                .Where(x => x.Id == key)
                .Select(x => _mapper.Map<VendorContactChildDto>(x)));
        }



        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<VendorContactChildDto> delta)
        {
            try
            {
                var vendorContact = await _vendorContactService.GetByIdAsync(key);
                if (vendorContact == null)
                {
                    return NotFound();
                }

                var dto = _mapper.Map<VendorContactChildDto>(vendorContact);
                delta.Patch(dto);
                var entity = _mapper.Map(dto, vendorContact);
                await _vendorContactService.UpdateAsync(entity);

                return Ok(_mapper.Map<VendorContactChildDto>(entity));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VendorContactChildDto vendorContact)
        {
            try
            {

                const string HeaderKeyName = "ParentId";
                Request.Headers.TryGetValue(HeaderKeyName, out var headerValue);
                var parentId = int.Parse(headerValue.ToString());

                vendorContact.VendorId = parentId;
                vendorContact.Number = _numberSequenceService.GenerateNumber(nameof(VendorContact), "", "CC");
                var entity = _mapper.Map<VendorContact>(vendorContact);
                await _vendorContactService.AddAsync(entity);
                var dto = _mapper.Map<VendorContact>(entity);

                return Created("VendorContactChild", dto);

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
                var vendorContact = await _vendorContactService.GetByIdAsync(key);
                if (vendorContact == null)
                {
                    return BadRequest();
                }

                await _vendorContactService.DeleteByIdAsync(vendorContact.Id);

                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
