using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using VillaAgency.Business.Abstract;
using VillaAgency.Business.Constants;
using VillaAgency.Dto.ProductDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public async Task<IActionResult> Index(string status = "All", string category = "All", int page = 1)
        {
            int pageSize = 15;
            List<ResultProductDto> products;

            bool isStatusFiltered = !string.IsNullOrEmpty(status) && !status.Equals("All", StringComparison.OrdinalIgnoreCase);
            bool isCategoryFiltered = !string.IsNullOrEmpty(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase);

            if (isStatusFiltered && isCategoryFiltered && Enum.TryParse<ProductStatus>(status, true, out var statusEnum))
            {
                products = await _productService.TGetPagedFilteredListAsync(page, pageSize,
                    p => p.Status == statusEnum && p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }
            else if (isCategoryFiltered)
            {
                products = await _productService.TGetPagedFilteredListAsync(page, pageSize,
                    p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }
            else if (isStatusFiltered && Enum.TryParse<ProductStatus>(status, true, out statusEnum))
            {
                products = await _productService.TGetPagedFilteredListAsync(page, pageSize, p => p.Status == statusEnum);
            }
            else if (!isStatusFiltered && !isCategoryFiltered)
            {
                products = await _productService.TGetPagedFilteredListAsync(page, pageSize);
            }
            else
            {
                products = new List<ResultProductDto>();
            }

            ViewBag.Categories = await _productService.TGetUniqueCategoriesAsync();
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedStatus = status;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(products);
        }

        public async Task<IActionResult> CreateAsync()
        {
            ViewBag.Categories = await _productService.TGetUniqueCategoriesAsync();
            ViewBag.Statuses = Enum.GetValues<ProductStatusDto>();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _productService.TCreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _productService.TDeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(string id)
        {
            ViewBag.Categories = await _productService.TGetUniqueCategoriesAsync();
            ViewBag.Statuses = Enum.GetValues<ProductStatusDto>();
            var value = await _productService.TGetByIdAsync(id);
            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _productService.TUpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

    }
}
