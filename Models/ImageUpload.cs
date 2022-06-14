using ImageUploader.Helper;

namespace ImageUploader.Models
{
    public class ImageUpload
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string UrlPath { get; set; }
        public string MimeType { get; set; }
        public DateTime Created { get; set; }
        [DbIgnore]
        public string FilePath { get; set; }
    }
}
