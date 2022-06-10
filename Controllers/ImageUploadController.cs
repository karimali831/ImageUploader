using ImageUploader.Service;
using ImageUploader.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ImageUploader.Controllers
{
    public class ImageUploadController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IImageUploadService imageUploadService;

        public ImageUploadController(IImageUploadService imageUploadService, IWebHostEnvironment env)
        {
            _env = env;
            this.imageUploadService = imageUploadService ?? throw new ArgumentNullException(nameof(imageUploadService));
        }

        public IActionResult Upload()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await imageUploadService.GetAllImagesAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> View(Guid? Id)
        {
            if (!Id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var viewModel = await imageUploadService.GetImageAsync(Id.Value);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(ImageUploadVM image)
        {
            if (ModelState.IsValid)
            {
                var imageId = await imageUploadService.UploadImageAsync(image);

                if (imageId.HasValue && imageId.Value != Guid.Empty)
                {
                    TempData["UploadResponnseMsg"] = "Image uploaded successfully";
                    return RedirectToAction(nameof(View), new { Id = imageId });
                }
                else
                {
                    TempData["UploadResponnseMsg"] = "An error occured";
                    return RedirectToAction(nameof(Index), new { Id = imageId });

                }
            }

            return View();
        }
    }
}
