using AutoMapper;
using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.ProductGroups;
using Indotalent.Applications.Products;
using Indotalent.Applications.UnitMeasures;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Indotalent.Pages.Products
{
    [Authorize]
    public class ProductFormModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly ProductService _productService;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly ProductGroupService _productGroupService;
        private readonly UnitMeasureService _unitMeasureService;
        public ProductFormModel(
            IMapper mapper,
            ProductService productService,
            NumberSequenceService numberSequenceService,
            ProductGroupService productGroupService,
            UnitMeasureService unitMeasureService
            )
        {
            _mapper = mapper;
            _productService = productService;
            _numberSequenceService = numberSequenceService;
            _unitMeasureService = unitMeasureService;
            _productGroupService = productGroupService;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public string? Action { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;

        [BindProperty]
        public ProductModel ProductForm { get; set; } = default!;

        public class ProductModel
        {
            public Guid? RowGuid { get; set; }

            [DisplayName("Name")]
            public string Name { get; set; } = string.Empty;

            [DisplayName("Description")]
            public string? Description { get; set; }

            [DisplayName("Product Group")]
            public int ProductGroupId { get; set; }

            [DisplayName("Unit Measure")]
            public int UnitMeasureId { get; set; }

            [DisplayName("Unit Price")]
            public double UnitPrice { get; set; } = 0;

            [DisplayName("Physical Product?")]
            public bool Physical { get; set; } = true;
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Product, ProductModel>();
                CreateMap<ProductModel, Product>();
            }
        }

        public ICollection<SelectListItem> ProductGroupLookup { get; set; } = default!;
        public ICollection<SelectListItem> UnitMeasureLookup { get; set; } = default!;
        private void BindLookup()
        {

            ProductGroupLookup = _productGroupService.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name}"
            }).ToList();

            UnitMeasureLookup = _unitMeasureService.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name}"
            }).ToList();


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
                var existing = await _productService.GetByRowGuidAsync(rowGuid);
                if (existing == null)
                {
                    throw new Exception($"Unable to load: {rowGuid}");
                }
                ProductForm = _mapper.Map<ProductModel>(existing);
                Number = existing.Number ?? string.Empty;
            }
            else
            {
                ProductForm = new ProductModel
                {
                    RowGuid = Guid.Empty
                };
            }
        }

        public async Task<IActionResult> OnPostAsync([Bind(Prefix = nameof(ProductForm))] ProductModel input)
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
                var newobj = _mapper.Map<Product>(input);

                Number = _numberSequenceService.GenerateNumber(nameof(Product), "", "ART");
                newobj.Number = Number;

                await _productService.AddAsync(newobj);

                this.WriteStatusMessage($"Success create new data.");
                return Redirect($"./ProductForm?rowGuid={newobj.RowGuid}&action=edit");
            }
            else if (action == "edit")
            {
                var existing = await _productService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                _mapper.Map(input, existing);
                await _productService.UpdateAsync(existing);

                this.WriteStatusMessage($"Success update existing data.");
                return Redirect($"./ProductForm?rowGuid={existing.RowGuid}&action=edit");
            }
            else if (action == "delete")
            {
                var existing = await _productService.GetByRowGuidAsync(input.RowGuid);
                if (existing == null)
                {
                    var message = $"Unable to load existing data: {input.RowGuid}";
                    throw new Exception(message);
                }

                await _productService.DeleteByRowGuidAsync(input.RowGuid);

                this.WriteStatusMessage($"Success delete existing data.");
                return Redirect("./ProductList");
            }
            return Page();
        }

    }
}
