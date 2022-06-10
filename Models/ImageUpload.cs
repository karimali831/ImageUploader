using ImageUploader.Helper;

namespace ImageUploader.Models
{
    public class ImageUpload
    {
        public Guid Id { get; set; }
        public byte[] FileData { get; set; }
        public string MimeType { get; set; }
        public string UrlPath { get; set; }
        public DateTime Created { get; set; }
    }
}
