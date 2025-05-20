using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AVTMS.Data;
using AVTMS.Models;
using AVTMS.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.Extensions.Hosting;

namespace AVTMS.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailServices _emailService;//
        private readonly IWebHostEnvironment _hostEnvironment;

        public VehiclesController(AppDbContext context, EmailServices emailService, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _emailService = emailService; //
            _hostEnvironment = hostEnvironment;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
      
            var appDbContext = _context.Vehicles.Include(v => v.VehicleOwner);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {


            //geting vehicle owners using nic as primary key
            // ViewData["VehicleOwnerNIC"] = new SelectList(_context.Set<VehicleOwner>(), "NIC", "NIC");
            // Get NIC with name for dropdown
            ViewData["VehicleOwnerNIC"] = new SelectList(
                _context.VehicleOwner.Select(vo => new
                {
                    vo.NIC,
                    DisplayName = vo.NIC + " (" + vo.OwnerName + ")"  // Assuming Owner  name is the property
                }),
                "NIC",
                "DisplayName"
            );
            return View();
        }

        // POST: Vehicles/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Vehicle vehicle, IFormFile imageFile)
        {
            vehicle.CreatedByID = User.Identity.Name;
            vehicle.CreatedOn = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    // Save the image file if provided
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "vehicleimages");
                        Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        vehicle.ImagePath = "/vehicleimages/" + uniqueFileName; // For web access
                    }

                    _context.Add(vehicle);
                    await _context.SaveChangesAsync();

                    // Fetch vehicle owner email
                    var owner = await _context.VehicleOwner
                        .FirstOrDefaultAsync(o => o.NIC == vehicle.VehicleOwnerNIC);

                    if (owner != null && !string.IsNullOrEmpty(owner.OwnerEmail))
                    {
                        string subject = "Vehicle Registration Successful";
                        string body = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f6f8;
            margin: 0;
            padding: 0;
            color: #333;
        }}
        .container {{
            max-width: 700px;
            margin: 30px auto;
            background-color: #ffffff;
            border: 1px solid #e0e0e0;
            border-radius: 8px;
            padding: 25px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.05);
        }}
        .header {{
            background-color: #4CAF50;
            color: white;
            padding: 15px;
            text-align: center;
            font-size: 22px;
            border-top-left-radius: 8px;
            border-top-right-radius: 8px;
            font-weight: bold;
        }}
        h2 {{
            color: #333;
            margin-top: 20px;
        }}
        p {{
            font-size: 16px;
            margin: 10px 0;
            color: #555;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
            font-size: 16px;
        }}
        th, td {{
            padding: 12px 15px;
            text-align: left;
        }}
        th {{
            background-color: #f0f0f0;
            color: #333;
            width: 35%;
        }}
        td {{
            background-color: #fafafa;
            color: #555;
        }}
        .footer {{
            margin-top: 30px;
            font-size: 13px;
            color: #888;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>Vehicle Registration Successful</div>

        <h2>Vehicle Registration Confirmation</h2>
        <p>Dear {owner.OwnerName},</p>
        <p>Your vehicle has been successfully registered in the system. Below are the registration details:</p>

        <table>
            <tr>
                <th>Vehicle Number Plate:</th>
                <td>{vehicle.VehicleNumberPlate}</td>
            </tr>
            <tr>
                <th>Owner NIC:</th>
                <td>{owner.NIC}</td>
            </tr>
            <tr>
                <th>Vehicle Model:</th>
                <td>{vehicle.VehicleModel}</td>
            </tr>
            <tr>
                <th>Registered Time:</th>
                <td>{vehicle.CreatedOn}</td>
            </tr>
        </table>

        <p>Thank you for using our system.</p>

        <div class='footer'>
            <p><strong>Note:</strong> This is a system-generated message. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";

                        await _emailService.SendEmailAsync(owner.OwnerEmail, subject, body);
                    }

                    Console.WriteLine("Vehicle saved and email sent successfully");
                    TempData["SuccessMessage"] = "The vehicle has been successfully added to the system.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("ModelState is INVALID");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                    }
                }
            }

            ViewData["VehicleOwnerNIC"] = new SelectList(_context.Set<VehicleOwner>(), "NIC", "NIC", vehicle.VehicleOwnerNIC);
            return View(vehicle);
        }



        /* original default code wieth normal vehicle added code
         * public async Task<IActionResult> Create(Vehicle vehicle)
        {
            vehicle.CreatedByID = User.Identity.Name;
            vehicle.CreatedOn = DateTime.Now;

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(vehicle);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Vehicle saved successfully");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Console.WriteLine("ModelState is INVALID");
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key].Errors;
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            ViewData["VehicleOwnerNIC"] = new SelectList(_context.Set<VehicleOwner>(), "NIC", "NIC", vehicle.VehicleOwnerNIC);
            return View(vehicle);
        }*/



        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["VehicleOwnerNIC"] = new SelectList(_context.Set<VehicleOwner>(), "NIC", "NIC", vehicle.VehicleOwnerNIC);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
        {
            if (id != vehicle.Id)
                return NotFound();

            vehicle.ModifiedBy = User.Identity.Name;
            vehicle.ModifiedOn = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    if (vehicle.ImageFile != null && vehicle.ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "vehicleimages");
                        Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(vehicle.ImagePath))
                        {
                            string oldPath = Path.Combine(_hostEnvironment.WebRootPath, vehicle.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(vehicle.ImageFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await vehicle.ImageFile.CopyToAsync(fileStream);
                        }

                        vehicle.ImagePath = "/vehicleimages/" + uniqueFileName;
                    }

                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["VehicleOwnerNIC"] = new SelectList(_context.VehicleOwner, "NIC", "NIC", vehicle.VehicleOwnerNIC);
            return View(vehicle);
        }
        /*   original default code only edit
         *  public async Task<IActionResult> Edit(int id, Vehicle vehicle)
           {
               vehicle.ModifiedBy = User.Identity.Name;
               vehicle.ModifiedOn = DateTime.Now;
               if (id != vehicle.Id)
               {
                   return NotFound();
               }

               if (ModelState.IsValid)
               {
                   try
                   {
                       _context.Update(vehicle);
                       await _context.SaveChangesAsync();
                   }
                   catch (DbUpdateConcurrencyException)
                   {
                       if (!VehicleExists(vehicle.Id))
                       {
                           return NotFound();
                       }
                       else
                       {
                           throw;
                       }
                   }
                   return RedirectToAction(nameof(Index));
               }
               ViewData["VehicleOwnerNIC"] = new SelectList(_context.Set<VehicleOwner>(), "NIC", "NIC", vehicle.VehicleOwnerNIC);
               return View(vehicle);
           }
        */
        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }


        //Serch vehicle add using NIC
        [HttpGet]
        [Route("Vehicles/SearchNICs")]
        public JsonResult SearchNICs(string term)
        {
            //only show using NIC allow for NIC search only
            //var nics = _context.VehicleOwner
            //    .Where(v => v.NIC.Contains(term))
            //    .Select(v => new { id = v.NIC, text = v.NIC })
            //    .ToList();

            // return Json(nics);


            //allow to search nVehicle Owner NIC with name
            var nics = _context.VehicleOwner
           .Where(v => v.NIC.Contains(term) || v.OwnerName.Contains(term)) // Allow searching by name too
           .Select(v => new
                     {
                       id = v.NIC,
                       text = v.NIC + " (" + v.OwnerName + ")"
                     })
                        .ToList();

            return Json(nics);
        }


    }
}
