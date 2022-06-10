using Azure.Storage.Blobs;
using ImageUploader.Helper;
using ImageUploader.Models;
using ImageUploader.Repository;
using ImageUploader.ViewModels;

namespace ImageUploader.Service
{
    public interface IImageUploadService
    {
        Task<ImageUpload> GetImageAsync(Guid Id);
        Task<IEnumerable<ImageUpload>> GetAllImagesAsync();
        Task<Guid?> UploadImageAsync(ImageUploadVM image);
    }

    public class ImageUploadService : IImageUploadService
    {
        private readonly IImageUploadRepository imageUploadRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IBlobStorageService blobStorageService;


        public ImageUploadService(
            IImageUploadRepository imageUploadRepository,
            IWebHostEnvironment webHostEnvironment,
            IBlobStorageService blobStorageService)
        {
            this.imageUploadRepository = imageUploadRepository ?? throw new ArgumentNullException(nameof(imageUploadRepository));
            this.webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            this.blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
        }

        public async Task<ImageUpload> GetImageAsync(Guid Id)
        {
            return await imageUploadRepository.GetImageAsync(Id);
        }

        public async Task<IEnumerable<ImageUpload>> GetAllImagesAsync()
        {
            return (await imageUploadRepository.GetAllImagesAsync()) ?? Enumerable.Empty<ImageUpload>();
        }

        public async Task<Guid?> UploadImageAsync(ImageUploadVM image)
        {
            byte[] fileData;
            using (var target = new MemoryStream())
            {
                image.File.CopyTo(target);
                fileData = target.ToArray();
            }

            string urlPath = await blobStorageService.Upload(image.File.FileName, fileData, image.File.ContentType);

            if (urlPath == null) {
                urlPath = await UploadLocalFile(image.File.FileName);
            }

            if (urlPath != null)
            {
                var model = new ImageUpload
                {
                    Id = Guid.NewGuid(),
                    FileData = fileData,
                    MimeType = image.File.ContentType,
                    UrlPath = urlPath,
                    Created = DateTime.UtcNow
                };

                if (await imageUploadRepository.UploadImageAsync(model))
                {
                    return model.Id;
                }
            }

            return null;
        }

        private async Task<string> UploadLocalFile(string fileName)
        {

            string localFolder = Path.Combine(webHostEnvironment.WebRootPath, "media");
            string filePath = Path.Combine(localFolder, fileName);

            if (!Directory.Exists(localFolder))
                Directory.CreateDirectory(localFolder);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                stream.Position = 0;
                await stream.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}
