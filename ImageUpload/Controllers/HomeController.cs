using ImageUpload.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;


namespace ImageUpload.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly string folder = "uploads";

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env; 
        }

        public IActionResult Index()
        {
            //var image = Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), folder)).Select(a => Path.GetFileName(a)).ToList();
            var image = Directory.EnumerateFiles(Path.Combine(_env.WebRootPath, folder)).Select(a => Path.GetFileName(a)).ToList();
            ViewBag.Message = TempData["Message"];
            return View(image);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            if (file == null)
            {
                ViewBag.Message = "Please upload an image file.";
                return RedirectToAction("Index");
            }
            var allowedExtensions = new[] { ".jpg", ".png" };
            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension))
            {
                TempData["Message"] = "Please select a valid image file.";
                return RedirectToAction("Index");
            }

            var fileName = Guid.NewGuid().ToString() + extension;
            var uploadPath = Path.Combine(_env.WebRootPath, folder);
            var filePath = Path.Combine(uploadPath, fileName);

            Directory.CreateDirectory(uploadPath);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }





            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
