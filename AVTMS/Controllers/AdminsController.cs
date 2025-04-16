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
    public class AdminsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public AdminsController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            // --Default -- return View(await _context.Admins.ToListAsync());
            var AdminsList = await _context.Admins.ToListAsync();

            foreach (var Admin in AdminsList)
            {
                var identityUser = await _userManager.FindByEmailAsync(Admin.Email);
                Admin.IsRegistered = identityUser != null;
            }

            return View(AdminsList);
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Admin admin)
        {
            admin.UserType = "Admin";
            admin.CreatedByID = User.Identity.Name;
            admin.RegisteredDate = DateTime.Now;
            admin.CreatedOn = DateTime.Now;
            
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Admin admin) //  ---Old code --  public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,NIC,FirstName,MiddleName,LastName,Email,Password,PhoneNumber,RegisteredDate,UserType,UserBranch,CreatedByID,CreatedOn,ModifiedBy,ModifiedOn")] Admin admin)
        {
            admin.ModifiedBy = User.Identity.Name;
            admin.ModifiedOn = DateTime.Now;

            if (id != admin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id))
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
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }



        //////////Aditional register button//////////////////
        ///

        public async Task<IActionResult> Register(int id)
        {
            // Retrieve the BaseUser record using the id
            var Admins = await _context.Admins.FindAsync(id);
            if (Admins == null)
            {
                return NotFound();
            }

            // Map the necessary data to  RegisterViewModel.
            // For example, if you want to prefill the Name and Email fields:
            var model = new RegisterViewModel
            {
                Name = Admins.FirstName,  // or combine names as needed
                Email = Admins.Email,
                Password = Admins.Password,
                ConfirmPassword = Admins.Password,// You can set a default password or leave it empty
                UserType = Admins.UserType,
                // You may leave Password/ConfirmPassword empty or handle them as needed.
            };

            // Return a partial view that contains your registration form
            return PartialView("_RegisterPartial", model);
        }




        // POST: Admin USer Register

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
                    UserType = model.UserType,
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

        // Update User Directly
        [HttpPost]
        public async Task<IActionResult> SyncToRegisterModel(int id)
        {
            var Admins = await _context.Admins.FindAsync(id);
            if (Admins == null)
                return Json(new { success = false, message = "Auth user not found." });

            var identityUser = await _userManager.FindByEmailAsync(Admins.Email);
            if (identityUser == null)
                return Json(new { success = false, message = "Identity user not found." });

            // Update Identity fields
            identityUser.UserName = Admins.Email;
            identityUser.Email = Admins.Email;
            identityUser.PhoneNumber = Admins.PhoneNumber;
            identityUser.PasswordHash = _userManager.PasswordHasher.HashPassword(identityUser, Admins.Password);

            // Cast to custom Identity model
            if (identityUser is Users extendedUser)
            {
                extendedUser.FullName = Admins.FirstName;
                extendedUser.UserType = Admins.UserType;
                extendedUser.PasswordHash = _userManager.PasswordHasher.HashPassword(identityUser, Admins.Password);
            }

            var result = await _userManager.UpdateAsync(identityUser);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Admins User updated successfully!" });
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = $"Update failed: {errors}" });
            }
        }
    }
}
