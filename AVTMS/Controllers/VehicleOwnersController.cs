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
    public class VehicleOwnersController : Controller
    {
        private readonly AppDbContext _context;

        public VehicleOwnersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: VehicleOwners
        public async Task<IActionResult> Index()
        {
            return View(await _context.VehicleOwner.ToListAsync());
        }

        // GET: VehicleOwners/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleOwner = await _context.VehicleOwner
                //get vehicle detils according to owner nic
                 .Include(v => v.Vehicles)
                 //
                .FirstOrDefaultAsync(m => m.NIC == id);
            if (vehicleOwner == null)
            {
                return NotFound();
            }

            return View(vehicleOwner);
        }

        // GET: VehicleOwners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VehicleOwners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( VehicleOwner vehicleOwner)
        {

            vehicleOwner.CreatedByID = User.Identity.Name;
            vehicleOwner.CreatedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                /// new
                if (_context.VehicleOwner.Any(vo => vo.NIC == vehicleOwner.NIC))
                {
                    ModelState.AddModelError("NIC", "This NIC is already registered.");
                    return View(vehicleOwner);
                }
                ///
                _context.Add(vehicleOwner);
                await _context.SaveChangesAsync();
                //after the user register redirect to vehicle register page
                return RedirectToAction("Create", "Vehicles");
                //return RedirectToAction(nameof(Index));
            }
            return View(vehicleOwner);
        }

        // GET: VehicleOwners/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleOwner = await _context.VehicleOwner.FindAsync(id);
            if (vehicleOwner == null)
            {
                return NotFound();
            }
            return View(vehicleOwner);
        }

        // POST: VehicleOwners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,  VehicleOwner vehicleOwner)
        {
            vehicleOwner.ModifiedBy = User.Identity.Name;
            vehicleOwner.ModifiedOn = DateTime.Now;
            if (id != vehicleOwner.NIC)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicleOwner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleOwnerExists(vehicleOwner.NIC))
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
            return View(vehicleOwner);
        }

        // GET: VehicleOwners/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleOwner = await _context.VehicleOwner
                .FirstOrDefaultAsync(m => m.NIC == id);
            if (vehicleOwner == null)
            {
                return NotFound();
            }

            return View(vehicleOwner);
        }

        // POST: VehicleOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vehicleOwner = await _context.VehicleOwner.FindAsync(id);
            if (vehicleOwner != null)
            {
                _context.VehicleOwner.Remove(vehicleOwner);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleOwnerExists(string id)
        {
            return _context.VehicleOwner.Any(e => e.NIC == id);
        }
    }
}
