using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Service.DTOs.Products;
using Product.Service.Interfaces.Products;
using Product.Service.ViewModels.ProductViewModels;

namespace Product.Web.Controllers.Products
{
    [Authorize(Roles = "admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            this._productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchValue)
        {
            List<ProductViewModel> products;
            if (!string.IsNullOrEmpty(searchValue))
            {
                ViewBag.AdminSearch = searchValue;
                products = await _productService.SearchByProperty(searchValue);
            }
            else
            {
                products = await _productService.SearchByProperty(searchValue);
            }
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductForCreationDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.CreateAsync(dto);
                if (result is not null)
                {
                    return RedirectToAction("Index", "Products", new { area = "" });
                }
                else
                {
                    return Create();
                }
            }
            else return Create();
        }

        [HttpGet]
        public async Task<ViewResult> Update(long id)
        {
            var admin = await _productService.RetrieveByIdAsync(id);
            var adminUpdate = new ProductForUpdateDto()
            {
                Name = admin.Name,
                Description = admin.Description,
                VideoFilePath = admin.VideoData
            };
            ViewBag.CreatedAt = admin.CreatedAt;
            ViewBag.SortNumber = admin.SortNumber;
            ViewBag.UpdatedAt = admin.UpdatedAt;
            ViewBag.VideoData = admin.VideoData;

            return View("Update", adminUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> Update(long id, ProductForUpdateDto dto)
        {
            var product = await _productService.UpdateAsync(id, dto);
            if (product is not null)
            {
                TempData["SuccessMessage"] = "Product Updated Successfully !";
                return RedirectToAction("Index", "Products");
            }
            else
            {
                TempData["InfoMessage"] = $"This product {id} not found !";
                return await Update(id);
            }

        }

        [HttpGet]
        public async Task<ViewResult> Remove(long id)
        {
            var product = await _productService.RetrieveByIdAsync(id);
            if (product is not null) return View("Remove", product);
            else return View("Remove", product);
        }

        [HttpPost, ActionName("Remove")]
        public async Task<IActionResult> RemoveConfirmed(long id)
        {
            var product = await _productService.RemoveAsync(id);
            if (product)
            {
                TempData["SuccessMessage"] = "Product Deleted Successfully !";
                return RedirectToAction("Index", "Products");
            }
            else
            {
                TempData["InfoMessage"] = $"This product {id} not found !";
                return await Remove(id);
            }

        }
    }
}
