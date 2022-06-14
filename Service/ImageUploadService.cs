using ImageUploader.Helper;
using ImageUploader.Models;
using ImageUploader.Repository;

namespace ImageUploader.Service
{
    public interface IImageUploadService
    {
        Task<ImageUpload> GetImageAsync(Guid Id);
        Task<IEnumerable<ImageUpload>> GetAllImagesAsync();
        Task<Guid?> UploadImageAsync(IFormFile imageFile);
    }

    public class ImageUploadService : IImageUploadService
    {
        private readonly IImageUploadRepository imageUploadRepository;
        private readonly IBlobStorageService blobStorageService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfigHelper configHelper;

        public ImageUploadService(
            IImageUploadRepository imageUploadRepository,
            IBlobStorageService blobStorageService,
            IWebHostEnvironment webHostEnvironment,
            IConfigHelper configHelper)
        {
            this.imageUploadRepository = imageUploadRepository ?? throw new ArgumentNullException(nameof(imageUploadRepository));
            this.blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
            this.webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            this.configHelper = configHelper ?? throw new ArgumentNullException(nameof(configHelper));
        }

        private string GetFilePath(string urlPath, string fileName) => urlPath ?? $"/{configHelper.LocalImageUploadDir}/{fileName}";

        public async Task<ImageUpload> GetImageAsync(Guid Id)
        {
            var image = await imageUploadRepository.GetImageAsync(Id);
            image.FilePath = GetFilePath(image.UrlPath, image.FileName);

            return image;
        }

        public async Task<IEnumerable<ImageUpload>> GetAllImagesAsync()
        {
            return (await imageUploadRepository.GetAllImagesAsync())
                .Select(image =>
                {
                    image.FilePath = GetFilePath(image.UrlPath, image.FileName);
                    return image;
                }) ?? Enumerable.Empty<ImageUpload>();
        }

        public async Task<Guid?> UploadImageAsync(IFormFile imageFile)
        {
            byte[] fileData;
            using (var target = new MemoryStream())
            {
                imageFile.CopyTo(target);
                fileData = target.ToArray();
            }

            string urlPath = await blobStorageService.Upload(imageFile.FileName, fileData, imageFile.ContentType);

            if (urlPath == null) {
                await Upload(imageFile);
            }

            var model = new ImageUpload
            {
                Id = Guid.NewGuid(),
                FileName = imageFile.FileName,
                FileData = fileData,
                UrlPath = urlPath,
                MimeType = imageFile.ContentType,
                Created = DateTime.UtcNow
            };

            if (await imageUploadRepository.UploadImageAsync(model))
            {
                return model.Id;
            }
            
            return null;
        }

        private async Task Upload(IFormFile imageFile)
        {
            try
            {
                string localFolder = Path.Combine(webHostEnvironment.WebRootPath, configHelper.LocalImageUploadDir);
                string filePath = Path.Combine(localFolder, imageFile.FileName);

                if (!Directory.Exists(localFolder))
                    Directory.CreateDirectory(localFolder);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
