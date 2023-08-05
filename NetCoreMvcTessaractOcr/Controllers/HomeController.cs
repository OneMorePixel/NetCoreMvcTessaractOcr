using Microsoft.AspNetCore.Mvc;
using NetCoreMvcTessaractOcr.Models;
using System.Diagnostics;
using Tesseract;

namespace NetCoreMvcTessaractOcr.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AnalyzeImage(IFormFile file)
        {
            var imageBytes = await GetBytes(file);
            var output = ProcessImage(imageBytes);
            return Json(new { ocrResult = output });
        }

        public async Task<byte[]> GetBytes(IFormFile formFile)
        {
            await using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        string ProcessImage(byte[] imageBytes)
        {
            var output = string.Empty;
            string webRootPath = _webHostEnvironment.WebRootPath;
            string tessDataPath = Path.Combine(webRootPath, "tessdata");
            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromMemory(imageBytes))
                {
                    var page = engine.Process(img);
                    output = page.GetText();
                }
            }
            return output;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}