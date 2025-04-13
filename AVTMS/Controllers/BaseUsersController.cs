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
    public class BaseUsersController : Controller
    {
        private readonly AppDbContext _context;

        public BaseUsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: BaseUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.BaseUser.ToListAsync());
        }

        // GET: BaseUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baseUser = await _context.BaseUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baseUser == null)
            {
                return NotFound();
            }

            return View(baseUser);
        }

        // GET: BaseUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BaseUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( BaseUser baseUser)
        {

            baseUser.UserType = "Base User";
            baseUser.CreatedByID = User.Identity.Name;
            baseUser.RegisteredDate = DateTime.Now;
            baseUser.CreatedOn = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(baseUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(baseUser);
        }

        // GET: BaseUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baseUser = await _context.BaseUser.FindAsync(id);
            if (baseUser == null)
            {
                return NotFound();
            }
            return View(baseUser);
        }

        // POST: BaseUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  BaseUser baseUser)
        {
            baseUser.ModifiedBy = User.Identity.Name;
            baseUser.ModifiedOn = DateTime.Now;
            if (id != baseUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(baseUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BaseUserExists(baseUser.Id))
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
            return View(baseUser);
        }

        // GET: BaseUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baseUser = await _context.BaseUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baseUser == null)
            {
                return NotFound();
            }

            return View(baseUser);
        }

        // POST: BaseUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var baseUser = await _context.BaseUser.FindAsync(id);
            if (baseUser != null)
            {
                _context.BaseUser.Remove(baseUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaseUserExists(int id)
        {
            return _context.BaseUser.Any(e => e.Id == id);
        }
    }
}
