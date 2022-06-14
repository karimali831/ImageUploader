using ImageUploader.Service;
using ImageUploader.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ImageUploader.Controllers
{
    public class ImageUploadController : Controller
    {
        private readonly IImageUploadService imageUploadService;

        public ImageUploadController(IImageUploadService imageUploadService)
        {
            this.imageUploadService = imageUploadService ?? throw new ArgumentNullException(nameof(imageUploadService));
        }

        public IActionResult Upload()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var getImages = await imageUploadService.GetAllImagesAsync();
            var viewModel = getImages.Select(image => ImageVM.MapFrom(image));

            return View(viewModel);
        }

        public async Task<IActionResult> View(Guid Id)
        {
            var getImage = await imageUploadService.GetImageAsync(Id);

            if (getImage == null)
            {
                return NotFound();
            }

            return View(ImageVM.MapFrom(getImage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(ImageUploadVM image)
        {
            if (ModelState.IsValid)
            {
                var imageId = await imageUploadService.UploadImageAsync(image.File);

                if (imageId.HasValue && imageId.Value != Guid.Empty)
                {
                    TempData["UploadResponseMsg"] = "Image uploaded successfully";
                    return RedirectToAction(nameof(View), new { Id = imageId });
                }
                else
                {
                    TempData["UploadResponseMsg"] = "An error occured";
                    return RedirectToAction(nameof(Index));

                }
            }

            return View();
        }
    }
}
