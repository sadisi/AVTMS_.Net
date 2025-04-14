using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AVTMS.Data;
using AVTMS.Models;
using AVTMS.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace AVTMS.Controllers
{
    public class BaseUsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public BaseUsersController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
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




        //////////Aditional register button//////////////////
        ///

        public async Task<IActionResult> Register(int id)
        {
            // Retrieve the BaseUser record using the id
            var baseUser = await _context.BaseUser.FindAsync(id);
            if (baseUser == null)
            {
                return NotFound();
            }

            // Map the necessary data to  RegisterViewModel.
            // For example, if you want to prefill the Name and Email fields:
            var model = new RegisterViewModel
            {
                Name = baseUser.FirstName,  // or combine names as needed
                Email = baseUser.Email,
                Password = baseUser.Password,
                ConfirmPassword = baseUser.Password // You can set a default password or leave it empty
                // You may leave Password/ConfirmPassword empty or handle them as needed.
            };

            // Return a partial view that contains your registration form
            return PartialView("_RegisterPartial", model);
        }




        // POST: Base USer/Register

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new user instance without setting the PasswordHash directly.
                var user = new Users
                {
                    FullName = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    NormalizedEmail = model.Email.ToUpper(),
                    NormalizedUserName = model.Email.ToUpper(),
                };

                // Use UserManager to create the user; it will hash the password internally.
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //message to user in frontend
                    TempData["SuccessMessage"] = "User registered successfully!";
                    // Optionally, sign the user in or perform other actions.
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Add any errors to the ModelState to be displayed in the view.
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // Return the partial view with validation errors, if any.
            return PartialView("_RegisterPartial", model);
        }


    }
}
