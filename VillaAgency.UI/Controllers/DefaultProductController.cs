using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.ProductDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.WebUI.Controllers
{
    public class DefaultProductController(IProductService _productService, ICounterService _counterService) : Controller
    {
        public async Task<IActionResult> Index(string category = "All", int page = 1)
        {
            int pageSize = 15;
            List<ResultProductDto> products;

            bool isCategoryFiltered = !string.IsNullOrEmpty(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase);

            Expression<Func<Product, bool>> filter;

            if (isCategoryFiltered)
            {
                filter = p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && p.Status == ProductStatus.Active;
            }
            else
            {
                filter = p => p.Status == ProductStatus.Active;
            }

            products = await _productService.TGetFilteredListAsync(filter, page, pageSize);

            int totalItems = await _counterService.TGetAllProductsCountAsync(filter);
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewBag.Categories = await _productService.TGetUniqueCategoriesAsync();
            ViewBag.SelectedCategory = category;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages < 1 ? 1 : totalPages;

            return View(products);
        }
    }
}