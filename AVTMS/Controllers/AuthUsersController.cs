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
    public class AuthUsersController : Controller
    {
        private readonly AppDbContext _context;

        public AuthUsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AuthUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.AuthUsers.ToListAsync());
        }

        // GET: AuthUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authUsers = await _context.AuthUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authUsers == null)
            {
                return NotFound();
            }

            return View(authUsers);
        }

        // GET: AuthUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AuthUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthUsers authUsers)
        {
            authUsers.UserType = "Admin";
            authUsers.CreatedByID = User.Identity.Name;
            authUsers.RegisteredDate = DateTime.Now;
            authUsers.CreatedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(authUsers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(authUsers);
        }

        // GET: AuthUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authUsers = await _context.AuthUsers.FindAsync(id);
            if (authUsers == null)
            {
                return NotFound();
            }
            return View(authUsers);
        }

        // POST: AuthUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuthUsers authUsers)
        {
            authUsers.ModifiedBy = User.Identity.Name;
            authUsers.ModifiedOn = DateTime.Now;
            if (id != authUsers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(authUsers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthUsersExists(authUsers.Id))
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
            return View(authUsers);
        }

        // GET: AuthUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authUsers = await _context.AuthUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authUsers == null)
            {
                return NotFound();
            }

            return View(authUsers);
        }

        // POST: AuthUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var authUsers = await _context.AuthUsers.FindAsync(id);
            if (authUsers != null)
            {
                _context.AuthUsers.Remove(authUsers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthUsersExists(int id)
        {
            return _context.AuthUsers.Any(e => e.Id == id);
        }
    }
}
