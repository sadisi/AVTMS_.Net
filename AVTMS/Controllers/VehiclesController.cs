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

namespace AVTMS.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailServices _emailService;//

        public VehiclesController(AppDbContext context, EmailServices emailService)
        {
            _context = context;
            _emailService = emailService; //
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
            ViewData["VehicleOwnerNIC"] = new SelectList(_context.Set<VehicleOwner>(), "NIC", "NIC");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            vehicle.CreatedByID = User.Identity.Name;
            vehicle.CreatedOn = DateTime.Now;

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(vehicle);
                    await _context.SaveChangesAsync();

                    // Fetch vehicle owner email
                    var owner = await _context.VehicleOwner
                        .FirstOrDefaultAsync(o => o.NIC == vehicle.VehicleOwnerNIC);

                    if (owner != null && !string.IsNullOrEmpty(owner.OwnerEmail))
                    {
                        string subject = "Vehicle Registration Successful";
                        string body = $@" <html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                color: #333;
                line-height: 1.6;
                margin: 0;
                padding: 0;
            }}
            h2 {{
                color: #4CAF50;
            }}
            p {{
                font-size: 16px;
                color: #555;
            }}
            ul {{
                list-style-type: none;
                padding: 0;
            }}
            li {{
                margin-bottom: 10px;
            }}
            li strong {{
                color: #333;
            }}
            table {{
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
            }}
            th, td {{
                border: 1px solid #ddd;
                padding: 10px;
                text-align: left;
            }}
            th {{
                background-color: #4CAF50;
                color: white;
            }}
            td {{
                background-color: #f9f9f9;
            }}
            .footer {{
                font-size: 12px;
                color: #777;
                margin-top: 20px;
            }}
            .subject {{
                background-color: #4CAF50;
                color: white;
                padding: 10px;
                text-align: center;
                font-size: 20px;
                font-weight: bold;
            }}
        </style>
    </head>
    <body>
        <div class='subject'>Vehicle Registration Successful</div>
        <h2>Vehicle Registration Confirmation</h2>
        <p>Dear {owner.OwnerName},</p>
        <p>Your vehicle has been successfully registered in the system.</p>
        
        <table>
            <tr>
                <th>Vehicle Number Plate :</th>
                <td>{vehicle.VehicleNumberPlate}</td>
            </tr>
            <tr>
                <th>Owner NIC : </th>
                <td>{owner.NIC}</td>
            </tr>
            <tr>
                <th>Registered Time :</th>
                <td>{vehicle.CreatedOn}</td>
            </tr>
        </table>

        <p>Thank you for using our system.</p>
        
        <div class='footer'>
            <p><strong>Note:</strong> This is a system-generated receipt. Please do not reply to this email.</p>
        </div>
    </body>
    </html>";

                        await _emailService.SendEmailAsync(owner.OwnerEmail, subject, body);
                    }

                    Console.WriteLine("Vehicle saved and email sent successfully");
                    //message to frontend user
                    TempData["SuccessMessage"] = "The vehicle has been successfully added to the system.";
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
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
    }
}
