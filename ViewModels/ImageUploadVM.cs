using ImageUploader.Helper;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ImageUploader.ViewModels
{
    public class ImageUploadVM : IValidatableObject
    {
        [Required(ErrorMessage = "No file selected")]
        public IFormFile File { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var supportedFileTypes = new[] { ".jpg", ".png" };
            var maxDimensions = (Width: 1024, Height: 1024);
            var image = ((ImageUploadVM)validationContext.ObjectInstance).File;

            // Check if exceeds max dimenions
            using (var img = Image.FromStream(image.OpenReadStream()))
            {
                if (img.Width > maxDimensions.Width || img.Height > maxDimensions.Height)
                {
                    yield return new ValidationResult(@$"
                        Exceeded the maximum dimensions allowed: {maxDimensions.Width}/{maxDimensions.Height}"
                    );
                }
            }

            // Check extension is valid
            var extension = Path.GetExtension(image.FileName);

            if (!supportedFileTypes.Contains(extension.ToLower())) 
            {
                yield return new ValidationResult(@$"
                    {extension} is not supported. Supported file types: {string.Join(", ", supportedFileTypes)}"
                );
            }
        }
    }
}
