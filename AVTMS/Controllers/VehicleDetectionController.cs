// VehicleDetectionController.cs
using AVTMS.Data;
using AVTMS.Models;
using AVTMS.Services;
using AVTMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        //for send viloation emails
        private readonly EmailServices _emailServices;

        public VehicleDetectionController(AppDbContext context, IWebHostEnvironment env, ILogger<VehicleDetectionController> logger, EmailServices emailServices)
        {
            _context = context;
            _env = env;
            _logger = logger;
            _emailServices = emailServices;
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

                //display last 10 detected vehicles
                var detectedVehicles = _context.VehicleDetects
                .OrderByDescending(v => v.DetectId)
                .Take(10) // Optional: just show latest 8
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




        //geting vehicle details after the detection of the vehicle using vehicle model, vehicle notemodel and vOwner model
        /*   public async Task<IActionResult> VehicleMatchDetailsPartial(int id)
           {
               var detect = await _context.VehicleDetects.FindAsync(id);
               if (detect == null)
                   return NotFound();

               var vehicle = await _context.Vehicles
                   .Include(v => v.VehicleOwner)
                   .Include(v => v.VehicleNotes)
                   .FirstOrDefaultAsync(v => v.VehicleNumberPlate == detect.license_plate);

               if (vehicle == null)
                   return NotFound();

               var model = new DetectedVehicleViewModel
               {
                   Detection = detect,
                   MatchedVehicle = vehicle
               };

               return PartialView("_VehicleMatchDetailsPartial", model);
           }
        */
        // In VehicleDetectionController.cs

        [HttpGet]
        public async Task<IActionResult> _VehicleMatchDetailsPartial(int id)
        {
            var detect = await _context.VehicleDetects.FindAsync(id);
            if (detect == null)
                return NotFound();

            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleOwner)
                .Include(v => v.VehicleNotes)
                .FirstOrDefaultAsync(v => v.VehicleNumberPlate == detect.license_plate);

            if (vehicle == null)
                return NotFound();

            var model = new DetectedVehicleViewModel
            {
                Detection = detect,
                MatchedVehicle = vehicle
            };

            return PartialView("_VehicleMatchDetailsPartial", model);
        }



        //to show all detected vehicle at once
        [HttpGet]
        public async Task<IActionResult> AllVehicleMatchDetails()
        {
            var vehicleDetects = await _context.VehicleDetects.ToListAsync();
            var vehicles = await _context.Vehicles
                .Include(v => v.VehicleOwner)
                .Include(v => v.VehicleNotes)
                .ToListAsync();

            // Match detected license plates with existing vehicles
            foreach (var detect in vehicleDetects)
            {
                var matchedVehicle = vehicles.FirstOrDefault(v =>
                    v.VehicleNumberPlate.ToLower() == detect.license_plate.ToLower());

                detect.MatchedVehicle = matchedVehicle;
            }

            return View(vehicleDetects);
        }



        //one by one details get (dev)

        [HttpGet]
        public async Task<IActionResult> _VehicleMatchDetailsPartialByPlate(string plate)
        {
            if (string.IsNullOrEmpty(plate))
                return BadRequest();

            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleOwner)
                .Include(v => v.VehicleNotes)
                .FirstOrDefaultAsync(v => v.VehicleNumberPlate == plate);

            if (vehicle == null)
                return NotFound();

            var detect = await _context.VehicleDetects
                .FirstOrDefaultAsync(d => d.license_plate == plate);

            if (detect == null)
                return NotFound();

            var model = new DetectedVehicleViewModel
            {
                Detection = detect,
                MatchedVehicle = vehicle
            };

            return PartialView("_VehicleMatchDetailsPartial", model);
        }


        //Viloation email send
        public async Task<IActionResult> SendViolationEmail(string licensePlate)
        {
            // Check if license plate parameter is provided
            if (string.IsNullOrEmpty(licensePlate))
                return BadRequest("License plate is missing.");

            // Find the detection record based on the provided license plate (case insensitive)
            var detect = await _context.VehicleDetects
                .FirstOrDefaultAsync(d => d.license_plate.ToLower() == licensePlate.ToLower());

            // Return 404 if no detection record found
            if (detect == null)
                return NotFound("Detection not found.");

            // Find the vehicle record and include related VehicleOwner and VehicleNotes
            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleOwner)
                .Include(v => v.VehicleNotes)
                .FirstOrDefaultAsync(v => v.VehicleNumberPlate.ToLower() == detect.license_plate.ToLower());

            // Return 404 if vehicle or its owner is not found
            if (vehicle == null || vehicle.VehicleOwner == null)
            {
                TempData["VDTE"] = "That vehicle data does not exist in the database";
                return NotFound("Vehicle or owner not found.");
               
            }

            var owner = vehicle.VehicleOwner;

            // Get the first available note content, or set default if none
            var noteContent = vehicle.VehicleNotes?.FirstOrDefault()?.NoteContent ?? "No Notes";

            // Parse the detection's start time or use current time as fallback
            DateTime capturedTime = DateTime.Now;
            if (!string.IsNullOrEmpty(detect.start_time))
                DateTime.TryParse(detect.start_time, out capturedTime);

            // Construct the HTML body of the email
            var body = $@"
<div style='font-family: Arial, sans-serif; padding: 25px; background-color: #f4f6f8; border-radius: 8px; border: 1px solid #e0e0e0; max-width: 700px; margin: auto;'>
    <h2 style='color: #c0392b; border-bottom: 2px solid #e74c3c; padding-bottom: 10px;'>Vehicle Violation Notification</h2>
    <p style='font-size: 16px; color: #333;'>The following vehicle has been identified for a violation. Please review the details below:</p>

    <div style='background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 5px rgba(0,0,0,0.05); margin-top: 20px;'>
        <table style='width: 100%; font-size: 16px; color: #444; border-collapse: collapse;'>
            <tr>
                <td style='padding: 10px; font-weight: bold; width: 40%;'> Numberplate:</td>
                <td style='padding: 10px;'>{detect.license_plate}</td>
            </tr>
            <tr style='background-color: #f9f9f9;'>
                <td style='padding: 10px; font-weight: bold;'>Vehicle Model:</td>
                <td style='padding: 10px;'>{vehicle.VehicleModel}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-weight: bold;'>Owner Name:</td>
                <td style='padding: 10px;'>{owner.OwnerName}</td>
            </tr>
            <tr style='background-color: #f9f9f9;'>
                <td style='padding: 10px; font-weight: bold;'>NIC:</td>
                <td style='padding: 10px;'>{owner.NIC}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-weight: bold;'>Captured Time:</td>
                <td style='padding: 10px;'>{capturedTime}</td>
            </tr>
            <tr style='background-color: #f9f9f9;'>
                <td style='padding: 10px; font-weight: bold;'>Reason:</td>
                <td style='padding: 10px;'>{noteContent}</td>
            </tr>
        </table>
    </div>

    <div style='margin-top: 25px; padding: 15px; background-color: #fff3cd; border-left: 5px solid #f0ad4e; border-radius: 5px; font-size: 16px; color: #8a6d3b;'>
        <strong>Important:</strong> You are required to <span style='color: #c9302c; font-weight: bold;'>report to the nearest police station within 24 hours</span>.
    </div>

    <p style='margin-top: 30px; font-size: 14px; color: #999; text-align: center;'>
        This is an automated message. Please do not reply to this email.
    </p>
</div>";



            // Send the email to the vehicle owner
            await _emailServices.SendEmailAsync(owner.OwnerEmail, "Violation Alert", body);

            // Log the sent email details to the database
            _context.ViolationEmails.Add(new ViolationEmail
            {
                LicensePlate = detect.license_plate,
                VehicleModel = vehicle.VehicleModel,
                NoteContent = noteContent,
                CapturedTime = capturedTime,
                OwnerName = owner.OwnerName,
                OwnerNIC = owner.NIC,
                OwnerEmail = owner.OwnerEmail,
                SentAt = DateTime.Now
            });

            // Persist changes to the database
            await _context.SaveChangesAsync();

            // Show success message on the redirected page
            TempData["SuccessMessage"] = $"Violation email sent to {owner.OwnerEmail}";

            // Redirect back to the index page
            return RedirectToAction(nameof(AllVehicleMatchDetails));
        }




    }
}
