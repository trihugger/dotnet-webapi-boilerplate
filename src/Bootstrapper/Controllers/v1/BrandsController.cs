using DN.WebApi.Application.Abstractions.Services.Catalog;
using DN.WebApi.Domain.Constants;
using DN.WebApi.Infrastructure.Identity.Permissions;
using DN.WebApi.Shared.DTOs.Catalog;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace DN.WebApi.Bootstrapper.Controllers.v1
{
    public class BrandsController : BaseController
    {
        private readonly IBrandService _service;

        public BrandsController(IBrandService service)
        {
            _service = service;
        }

        [HttpGet]
        [MustHavePermission(PermissionConstants.Brands.View)]
        [SwaggerOperation(Summary = "Get Brands using available Filters and taking advantage of the caching server side.")]
        public async Task<IActionResult> GetAsync(string filter)
        {
            BrandListFilter listFilter = JsonSerializer.Deserialize<BrandListFilter>(filter);

            var brands = await _service.SearchAsync(listFilter);
            return Ok(brands);
        }

        [HttpPost("search")]
        [MustHavePermission(PermissionConstants.Brands.Search)]
        [SwaggerOperation(Summary = "Search Brands using available Filters.")]
        public async Task<IActionResult> SearchAsync(BrandListFilter filter)
        {
            var brands = await _service.SearchAsync(filter);
            return Ok(brands);
        }

        [HttpPost]
        [MustHavePermission(PermissionConstants.Brands.Register)]
        public async Task<IActionResult> CreateAsync(CreateBrandRequest request)
        {
            return Ok(await _service.CreateBrandAsync(request));
        }

        [HttpPut("{id}")]
        [MustHavePermission(PermissionConstants.Brands.Update)]
        public async Task<IActionResult> UpdateAsync(UpdateBrandRequest request, Guid id)
        {
            return Ok(await _service.UpdateBrandAsync(request, id));
        }

        [HttpDelete("{id}")]
        [MustHavePermission(PermissionConstants.Brands.Remove)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var brandId = await _service.DeleteBrandAsync(id);
            return Ok(brandId);
        }
    }
}