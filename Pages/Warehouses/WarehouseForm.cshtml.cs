using AutoMapper;
using Indotalent.Applications.Warehouses;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;

namespace Indotalent.Pages.Warehouses
{
    [Authorize]
    public class WarehouseFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly WarehouseService _warehouseService;
        public WarehouseFormModel(
            IMapper mapper,
            WarehouseService warehouseService
            )
        {
            _mapper = mapper;
            _warehouseService = warehouseService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;

        [BindProperty]
        public WarehouseModel WarehouseForm { get; set; } = default!;

        public class WarehouseModel
        {
            public Guid? RowGuid { get; set; }

            [DisplayName("Name")]
            public string Name { get; set; } = string.Empty;

            [DisplayName("Description")]
            public string? Description { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Warehouse, WarehouseModel>();
                CreateMap<WarehouseModel, Warehouse>();
            }
        }

        private void BindLookup()
        {

        }

        public async Task OnGetAsync(Guid? rowGuid)
        {

            this.SetupViewDataTitleFromUrl();
            this.SetupStatusMessage();
            StatusMessage = this.ReadStatusMessage();

            var action = Request.Query["action"];
            Action = action;

            BindLookup();

            if (rowGuid.HasValue)
            {
                var existing = await _warehouseService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                WarehouseForm = _mapper.Map<WarehouseModel>(existing);
            }
            else
            {
                WarehouseForm = new WarehouseModel
                {
                    RowGuid = Guid.Empty
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(WarehouseForm))] WarehouseModel input)
        {


            if (!ModelState.IsValid)
            {
                var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                throw new Exception(message);
            }

            var action = "create";

            if (!string.IsNullOrEmpty(Request.Query["action"]))
            {
                action = Request.Query["action"];
            }

            if (action == "create")
            {
                var newobj = _mapper.Map<Warehouse>(input);
                await _warehouseService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./WarehouseForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _warehouseService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _warehouseService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./WarehouseForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _warehouseService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _warehouseService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./WarehouseList");
            }
            return Page();
        }

    }
}
