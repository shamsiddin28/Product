using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Domain.Configurations;
using Product.Service.DTOs.Products;
using Product.Service.Interfaces.Products;

namespace Product.Web.Areas.Products
{
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IProductService productService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] ProductForCreationDto dto)
            => Ok(await this._productService.CreateAsync(dto));

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllAsync()
            => Ok(await this._productService.RetrieveAllProductsAsync());

        [HttpGet("GetAllProducts/ByPagination")]
        public async Task<IActionResult> GetAllProductsAsync([FromQuery] PaginationParams @params)
            => Ok(await this._productService.RetrieveAllAsync(@params));

        [HttpGet("GetProductById/{id:long}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")] long id)
            => Ok(await this._productService.RetrieveByIdAsync(id));

        [HttpGet("GetByPropertyName/{propertyName}")]
        public async Task<IActionResult> GetByPropertyAsync([FromRoute(Name = "propertyName")] string propertyName)
            => Ok(await this._productService.SearchByProperty(propertyName));

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteProductById/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] long id)
            => Ok(await this._productService.RemoveAsync(id));

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteVideoById/{id}")]
        public async Task<IActionResult> DeleteVideoAsync([FromRoute(Name = "id")] long id)
            => Ok(await this._productService.DeleteVideoAsync(id));

        [HttpGet("DownloadByVideoPartPath/{videoPartPath}")]
        public async Task<IActionResult> DownloadAsync(string videoPartPath)
        {
            return File(await this._productService.DownloadAsync(videoPartPath), "application/octet-stream", videoPartPath);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("UpdateProductById/{id}")]
        public async Task<IActionResult> PutAsync([FromRoute(Name = "id")] long id, [FromForm] ProductForUpdateDto dto)
            => Ok(await this._productService.UpdateAsync(id, dto));

        [Authorize(Roles = "admin")]
        [HttpPut("UpdateVideoById/{id}")]
        public async Task<IActionResult> PutVideoAsync([FromRoute(Name = "id")] long id, [FromForm] IFormFile formFile)
            => Ok(await this._productService.UpdateVideoAsync(id, formFile));

        [HttpGet("OpenVideoStreamAsync/{videoPartPath}")]
        public IActionResult OpenVideoStreamAsync(string videoPartPath)
        {
            string videoFilePath = Path.Combine(_webHostEnvironment.WebRootPath, videoPartPath);
            try
            {
                // Open the video file as a stream
                FileStream fileStream = System.IO.File.OpenRead(videoFilePath);
                return File(fileStream, "application/octet-stream"); // Assuming the video file is in mp4 format
            }
            catch (FileNotFoundException)
            {
                // Handle file not found error
                return NotFound();
            }
            catch (IOException)
            {
                // Handle IO error
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                // Handle other unexpected errors
                return StatusCode(500, ex.Message);
            }
        }
    }
}
