using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Product.Service.Commons.Helpers;
using Product.Service.Helpers;
using Product.Service.Interfaces.Files;

namespace Product.Service.Services.Files
{
    public class FileService : IFileService
    {
        private readonly string MEDIA_FOLDER;
        private readonly string RESOUCE_IMAGE_FOLDER;
        private readonly string RESOUCE_VIDEO_FOLDER;
        private readonly string AVATAR_FOLDER;
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            MEDIA_FOLDER = "media";
            RESOUCE_IMAGE_FOLDER = Path.Combine(MEDIA_FOLDER, "images");
            RESOUCE_VIDEO_FOLDER = Path.Combine(MEDIA_FOLDER, "videos");
            AVATAR_FOLDER = Path.Combine(MEDIA_FOLDER, "avatars");
        }

        public async Task<bool> DeleteFileAsync(string filePartPath)
        {
            string path = Path.Combine(_hostingEnvironment.WebRootPath, filePartPath);

            if (File.Exists(path))
            {
                try
                {
                    // Asynchronously delete the file by offloading the blocking operation to a background thread
                    await Task.Run(() => File.Delete(path));
                    return true;
                }
                catch
                {
                    // Handle any exceptions
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            string fileName = ImageHelper.UniqueName(image.FileName);
            string partPath = Path.Combine(RESOUCE_IMAGE_FOLDER, fileName);
            string path = Path.Combine(_hostingEnvironment.WebRootPath, partPath);
            var stream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(stream);
            stream.Close();
            return partPath;
        }

        public async Task<string> UploadVideoAsync(IFormFile video)
        {
            string fileName = VideoHelper.UniqueName(video.FileName);
            string partPath = Path.Combine(RESOUCE_VIDEO_FOLDER, fileName);
            string path = Path.Combine(_hostingEnvironment.WebRootPath, partPath);
            var stream = new FileStream(path, FileMode.Create);
            await video.CopyToAsync(stream);
            stream.Close();
            return partPath;
        }
    }
}
