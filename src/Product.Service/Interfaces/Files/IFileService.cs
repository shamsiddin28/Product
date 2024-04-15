using Microsoft.AspNetCore.Http;

namespace Product.Service.Interfaces.Files
{
    public interface IFileService
    {
        public Task<string> UploadImageAsync(IFormFile image);
        public Task<string> UploadVideoAsync(IFormFile file);
        public Task<bool> DeleteFileAsync(string filePartPath);
    }
}
