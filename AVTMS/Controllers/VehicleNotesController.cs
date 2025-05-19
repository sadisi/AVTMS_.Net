using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AVTMS.Data;
using AVTMS.Models;

namespace AVTMS.Controllers
{
    public class VehicleNotesController : Controller
    {
        private readonly AppDbContext _context;

        public VehicleNotesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: VehicleNotes
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.VehicleNotes.Include(v => v.Vehicle);
            return View(await appDbContext.ToListAsync());
        }

        // GET: VehicleNotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleNotes = await _context.VehicleNotes
                .Include(v => v.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleNotes == null)
            {
                return NotFound();
            }

            return View(vehicleNotes);
        }

        // GET: VehicleNotes/Create
        public IActionResult Create()
        {
            //ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "VehicleNumberPlate");

            //get numberplate number with vehicle model
            ViewData["VehicleId"] = new SelectList(
    _context.Vehicles.Select(v => new
    {
        v.Id,
        DisplayName = v.VehicleNumberPlate + " (" + v.VehicleModel + ")"
    }),
    "Id",
    "DisplayName"
);

            return View();
        }

        // POST: VehicleNotes/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleNotes vehicleNotes)
        {
            vehicleNotes.CreatedOn = DateTime.Now;
            vehicleNotes.CreatedByID = User.Identity.Name;
            if (ModelState.IsValid)
            {
                _context.Add(vehicleNotes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //  ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "VehicleNumberPlate", vehicleNotes.VehicleId);

            //disply numberplate number with vehicle model
            ViewData["VehicleId"] = new SelectList(
      _context.Vehicles.Select(v => new
      {
          v.Id,
          DisplayName = v.VehicleNumberPlate + " (" + v.VehicleModel + ")"
      }),
      "Id",
      "DisplayName",
      vehicleNotes.VehicleId
  );

            return View(vehicleNotes);
        }

        // GET: VehicleNotes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleNotes = await _context.VehicleNotes.FindAsync(id);
            if (vehicleNotes == null)
            {
                return NotFound();
            }
            //ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "VehicleNumberPlate", vehicleNotes.VehicleId);


            //get numberplate number with vehicle model
            ViewData["VehicleId"] = new SelectList(
            _context.Vehicles.Select(v => new
                     {
                         v.Id,
                         DisplayName = v.VehicleNumberPlate + " (" + v.VehicleModel + ")"
                      }),
                             "Id",
                             "DisplayName",
                             vehicleNotes.VehicleId
                         );

            return View(vehicleNotes);
        }

        // POST: VehicleNotes/Edit/5
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VehicleNotes vehicleNotes)
        {
            vehicleNotes.ModifiedOn = DateTime.Now;
            vehicleNotes.ModifiedBy = User.Identity.Name;
            if (id != vehicleNotes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicleNotes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleNotesExists(vehicleNotes.Id))
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
            // ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "VehicleNumberPlate", vehicleNotes.VehicleId);

            //disply numberplate number with vehicle model
            ViewData["VehicleId"] = new SelectList(
                  _context.Vehicles.Select(v => new
                    {
                         v.Id,
                         DisplayName = v.VehicleNumberPlate + " (" + v.VehicleModel + ")"
                     }),
                              "Id",
                               "DisplayName",
                              vehicleNotes.VehicleId
                    );

            return View(vehicleNotes);
        }

        // GET: VehicleNotes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleNotes = await _context.VehicleNotes
                .Include(v => v.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleNotes == null)
            {
                return NotFound();
            }

            return View(vehicleNotes);
        }

        // POST: VehicleNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicleNotes = await _context.VehicleNotes.FindAsync(id);
            if (vehicleNotes != null)
            {
                _context.VehicleNotes.Remove(vehicleNotes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleNotesExists(int id)
        {
            return _context.VehicleNotes.Any(e => e.Id == id);
        }
    }
}
