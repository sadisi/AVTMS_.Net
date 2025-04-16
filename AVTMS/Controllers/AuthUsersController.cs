using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AVTMS.Data;
using AVTMS.Models;
using Microsoft.AspNetCore.Identity;
using AVTMS.ViewModels;

namespace AVTMS.Controllers
{
    public class AuthUsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public AuthUsersController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AuthUsers
        public async Task<IActionResult> Index()
        {
            // --Default ---- return View(await _context.AuthUsers.ToListAsync());
            var authUsersList = await _context.AuthUsers.ToListAsync();

            foreach (var authUser in authUsersList)
            {
                var identityUser = await _userManager.FindByEmailAsync(authUser.Email);
                authUser.IsRegistered = identityUser != null;
            }

            return View(authUsersList);
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
            authUsers.UserType = "Autharized Users";
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


        //////////Aditional register button//////////////////
        ///

        public async Task<IActionResult> Register(int id)
        {
            // Retrieve the BaseUser record using the id
            var AuthUsers = await _context.AuthUsers.FindAsync(id);
            if (AuthUsers == null)
            {
                return NotFound();
            }

            // Map the necessary data to  RegisterViewModel.
            // For example, if you want to prefill the Name and Email fields:
            var model = new RegisterViewModel
            {
                Name = AuthUsers.FirstName,  // or combine names as needed
                Email = AuthUsers.Email,
                Password = AuthUsers.Password,
                ConfirmPassword = AuthUsers.Password ,
                UserType = AuthUsers.UserType,// You can set a default password or leave it empty
                // You may leave Password/ConfirmPassword empty or handle them as needed.
            };

            // Return a partial view that contains your registration form
            return PartialView("_RegisterPartial", model);
        }




        // POST: AuthUser USer Register

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
            var authUser = await _context.AuthUsers.FindAsync(id);
            if (authUser == null)
                return Json(new { success = false, message = "Auth user not found." });

            var identityUser = await _userManager.FindByEmailAsync(authUser.Email);
            if (identityUser == null)
                return Json(new { success = false, message = "Identity user not found." });

            // Update Identity fields
            identityUser.UserName = authUser.Email;
            identityUser.Email = authUser.Email;
            identityUser.PhoneNumber = authUser.PhoneNumber;
            identityUser.PasswordHash = _userManager.PasswordHasher.HashPassword(identityUser, authUser.Password);

            // Cast to custom Identity model
            if (identityUser is Users extendedUser)
            {
                extendedUser.FullName = authUser.FirstName;
                extendedUser.UserType = authUser.UserType;
                extendedUser.PasswordHash = _userManager.PasswordHasher.HashPassword(identityUser, authUser.Password);
            }

            var result = await _userManager.UpdateAsync(identityUser);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Auth user model updated successfully!" });
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = $"Update failed: {errors}" });
            }
        }
    }
}

