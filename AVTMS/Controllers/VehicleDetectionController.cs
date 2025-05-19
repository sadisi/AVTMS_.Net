// VehicleDetectionController.cs
using AVTMS.Data;
using AVTMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AVTMS.Controllers
{
    public class VehicleDetectionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<VehicleDetectionController> _logger;

        public VehicleDetectionController(AppDbContext context, IWebHostEnvironment env, ILogger<VehicleDetectionController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task<IActionResult> Upload()
        {
            var detections = await _context.VehicleDetects.ToListAsync();
            return View(detections);
            //return View();

        }


        [HttpPost]
        public async Task<IActionResult> UploadVideo(IFormFile videoFile)
        {
            if (videoFile != null && videoFile.Length > 0)
            {
                var fileName = Path.GetFileName(videoFile.FileName);
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var filePath = Path.Combine(uploads, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(fileStream);
                }

                // Set TempData with relative path for video preview
                TempData["VideoPath"] = "/uploads/" + fileName;

                // Run python process as before...
                var psi = new ProcessStartInfo
                {
                    FileName = @"D:\etc\Python\VehicleTrack\vehicleNumberPlateTrack\.venv\Scripts\python.exe",
                    Arguments = $"avtms.py \"{filePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = "D:\\etc\\Python\\VehicleTrack\\vehicleNumberPlateTrack\\yolov10"
                };

                //pass python console out put to upload page

                TempData["Success"] = $"Running Python with: {psi.FileName} {psi.Arguments}";

                try
                {
                    var process = new Process { StartInfo = psi };
                    process.Start();

                    string output = await process.StandardOutput.ReadToEndAsync();
                    string errors = await process.StandardError.ReadToEndAsync();

                    await process.WaitForExitAsync();

                    TempData["Success"] += $"<br/>Output: {output}<br/>Errors: {errors}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running Python script");
                    TempData["Error"] = "There was a problem processing your video. Please check the server logs.";
                }

                //new
                var detectedVehicles = _context.VehicleDetects
                .OrderByDescending(v => v.DetectId)
                .Take(10) // Optional: just show latest 10
                .ToList();

                return View("Upload", detectedVehicles);

                //

                return RedirectToAction("Upload");
            }

            ModelState.AddModelError("", "Please upload a valid video file.");
            return View("Upload");
        }



        /*  private async Task<DetectionResult> SendVideoToYoloApi(string filePath)
           {
               try
               {
                   var pythonScript = "D:\\etc\\Python\\VehicleTrack\\vehicleNumberPlateTrack\\yolov10\\avtms.py";
                   var psi = new ProcessStartInfo
                   {
                       FileName = "python",
                       Arguments = $"\"{pythonScript}\" \"{filePath}\"",
                       RedirectStandardOutput = true,
                       RedirectStandardError = true,
                       UseShellExecute = false,
                       CreateNoWindow = true
                   };

                   using (var process = Process.Start(psi))
                   {
                       string output = await process.StandardOutput.ReadToEndAsync();
                       string error = await process.StandardError.ReadToEndAsync();

                       if (!string.IsNullOrWhiteSpace(error))
                           _logger.LogError("Python error: " + error);

                       _logger.LogInformation("Python output: " + output);

                       return JsonConvert.DeserializeObject<DetectionResult>(output);
                   }
               }
               catch (Exception ex)
               {
                   _logger.LogError("Error running detection script: " + ex.Message);
                   return null;
               }
           }*/


        public IActionResult LiveCCTVStream()
        {
            return View();
        }

        public class DetectionResult
        {
            public string NumberPlate { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
