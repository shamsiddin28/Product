using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Product.Data.Interfaces.IRepositories;
using Product.Domain.Configurations;
using Product.Domain.Entities;
using Product.Service.Commons.Helpers;
using Product.Service.DTOs.Products;
using Product.Service.Exceptions;
using Product.Service.Extensions;
using Product.Service.Interfaces.Files;
using Product.Service.Interfaces.Products;
using Product.Service.ViewModels.ProductViewModels;

namespace Product.Service.Services.Products
{
    public class ProductService : IProductService
    {
        public string WebRootPath;

        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IRepository<TestProduct> _repository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductService(
            IRepository<TestProduct> repository,
            IFileService fileService,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment)
        {
            _mapper = mapper;
            _repository = repository;
            _fileService = fileService;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ProductViewModel> CreateAsync(ProductForCreationDto dto)
        {
            var product = await _repository.FirstOrDefaultAsync(x => x.Name == dto.Name);
            if (product is not null)
                throw new AlreadyExistingException(nameof(dto.Name), "This product name is already created.");

            var videoFilePath = await _fileService.UploadVideoAsync(dto.VideoData);

            var mappedTestProduct = _mapper.Map<TestProduct>(dto);
            mappedTestProduct.CreatedAt = TimeHelper.GetCurrentServerTime();
            mappedTestProduct.SortNumber = int.Parse(GenerateOTP());
            mappedTestProduct.VideoData = videoFilePath;

            var newProduct = _repository.Insert(mappedTestProduct);
            var result = await _repository.SaveChangesAsync();
            if (result > 0)
                return (ProductViewModel)(newProduct);
            else
                return null;
        }

        public async Task<byte[]> DownloadAsync(string videoFilePath)
        {
            var wwwroot = "C:\\Users\\shams\\Desktop\\Test\\Product\\src\\Product.Web\\wwwroot\\";
            if (_hostingEnvironment.WebRootPath != null)
            {
                var videoPath = Path.Combine(_hostingEnvironment.WebRootPath, videoFilePath);
                if (File.Exists(videoPath))
                    return await File.ReadAllBytesAsync(videoPath);
            }
            var video = Path.Combine(wwwroot, videoFilePath);
            if (File.Exists(video))
                return await File.ReadAllBytesAsync(video);

            throw new FileNotFoundException();
        }

        public async Task<byte[]> DownloadOnStaticFileAsync(string videoFilePath)
        {
            var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "StaticFilesProducts", videoFilePath);
            if (File.Exists(videoPath))
                return await File.ReadAllBytesAsync(videoPath);
            throw new FileNotFoundException();
        }

        public async Task<ProductViewModel> UpdateAsync(long id, ProductForUpdateDto dto)
        {
            var product = await _repository.FindByIdAsync(id);
            if (product is null) throw new NotFoundException("Product", $"{id} not found");
            _repository.TrackingDeteched(product);
            if (dto != null)
            {
                product.Name = string.IsNullOrEmpty(dto.Name) ? product.Name : dto.Name;
                product.Description = string.IsNullOrEmpty(dto.Description) ? product.Description : dto.Description;
                product.VideoData = string.IsNullOrEmpty(dto.VideoFilePath) ? product.VideoData : dto.VideoFilePath;
                if (dto.VideoData is not null)
                {
                    product.VideoData = await _fileService.UploadVideoAsync(dto.VideoData);
                }
                product.UpdatedAt = TimeHelper.GetCurrentServerTime();
                product.SortNumber = int.Parse(GenerateOTP());
                _repository.Update(id, product);
                var result = await _repository.SaveChangesAsync();
                if (result > 0)
                    return (ProductViewModel)product;
                else
                    return null;
            }
            else throw new ModelErrorException("", "Not found");
        }

        public async Task<bool> UpdateVideoAsync(long id, IFormFile formFile)
        {
            var product = await _repository.FindByIdAsync(id);
            if (product is null) throw new NotFoundException("Product", $"{id} not found");

            var updateVideo = await _fileService.UploadVideoAsync(formFile);
            var deleteOldVideo = await _fileService.DeleteFileAsync(product.VideoData);
            var deleteOnStaticOldVideo = await _fileService.DeleteStaticFileAsync(product.VideoData);
            var productForUpdateDto = new ProductForUpdateDto()
            {
                VideoFilePath = updateVideo
            };
            var result = await UpdateAsync(id, productForUpdateDto);
            return result is not null && deleteOnStaticOldVideo && deleteOldVideo && updateVideo != null ? true : false;
        }

        public async Task<bool> RemoveAsync(long id)
        {
            var product = await _repository.FindByIdAsync(id);
            if (product is null) throw new NotFoundException("Product", $"{id} not found");
            var deletedVideo = await DeleteVideoAsync(id);
            _repository.Delete(id);
            int result = await _repository.SaveChangesAsync();
            product.IsDeleted = true;
            return result > 0 && deletedVideo;
        }

        public async Task<bool> DeleteVideoAsync(long productId)
        {
            var product = await _repository.FindByIdAsync(productId);
            if (product is null) throw new NotFoundException("Product", $"{productId} not found");
            else
            {
                await _fileService.DeleteFileAsync(product.VideoData!);
                await _fileService.DeleteStaticFileAsync(product.VideoData!);
                product.VideoData = "";
                _repository.Update(productId, product);
                var res = await _repository.SaveChangesAsync();
                return res > 0;
            }
        }

        public async Task<IEnumerable<ProductViewModel>> RetrieveAllAsync(PaginationParams @params)
        {
            var product = await _repository.SelectAll()
            .AsNoTracking()
            .ToPagedList<TestProduct>(@params)
            .ToListAsync();


            return _mapper.Map<IEnumerable<ProductViewModel>>(product);
        }

        public Task<List<ProductViewModel>> RetrieveAllProductsAsync()
        {
            var query = _repository.SelectAll().OrderByDescending(x => x.CreatedAt).Select(x => (ProductViewModel)x).ToListAsync();
            return query;
        }

        public async Task<ProductViewModel> RetrieveByIdAsync(long id)
        {
            var product = await _repository.FindByIdAsync(id);
            if (product is null) throw new NotFoundException("Product", $"{id} not found");
            var productView = (ProductViewModel)product;
            return productView;
        }

        public async Task<List<ProductViewModel>> SearchByProperty(string searchTerm)
        {
            var query = _repository.SelectAll();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower()) || x.Description.ToLower().Contains(searchTerm.ToLower()) || x.SortNumber.ToString().Contains(searchTerm.ToLower()));
            }

            var result = await query.OrderByDescending(x => x.CreatedAt).Select(x => (ProductViewModel)x).ToListAsync();
            return result;
        }

        public string GenerateOTP()
        {
            Random random = new Random();
            string otp = "";

            // Create an array of digits from 0 to 9
            int[] digits = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Shuffle the array
            for (int i = digits.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                int temp = digits[i];
                digits[i] = digits[j];
                digits[j] = temp;
            }

            // Take the first 6 digits to form the OTP
            for (int i = 0; i < 6; i++)
            {
                otp += digits[i];
            }

            // Sort the OTP digits in ascending order
            char[] otpArray = otp.ToCharArray();
            Array.Sort(otpArray);
            otp = new string(otpArray);

            return otp;
        }

        public async Task<ProductViewModel> RetrieveBySortNumberAsync(long sortNumber)
        {
            var product = await _repository.SelectAll().Where(p => p.SortNumber == sortNumber).FirstOrDefaultAsync();
            if (product is null) throw new NotFoundException("Product", $"{sortNumber} not found");
            var productView = (ProductViewModel)product;
            return productView;
        }
    }
}
