using ImageUploader.Models;

namespace ImageUploader.ViewModels
{
    public class ImageVM
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Created { get; set; }

        public static ImageVM MapFrom(ImageUpload image)
        {
            return new ImageVM
            {
                Id = image.Id,
                FileName = image.FileName,
                FilePath = image.FilePath,
                Created = image.Created.ToString("dd/MM/yyyy HH:mm")
            };
        }
    }
}
