using AVTMS.Models;
using AVTMS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AVTMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;


        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        //User Login 
        public IActionResult Login()
        {
            return View();
        }

        //User Login method
        [HttpPost]
        public async Task<IActionResult > Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else 
                {
                    ModelState.AddModelError("", "Email or Password is incorrect. Check again !");
                    return View(model);
                }
            }
            return View(model);
        }

        //User Register     
        public IActionResult Register()
        {
            return View();
        }



        // Method for Register users
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid) 
            {
                Users users = new Users
                {
                    FullName = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    UserType = model.UserType
                };

                var result = await userManager.CreateAsync(users, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

            }
            return View(model);
        }

        //Email Verification  
        public IActionResult VerifyEmail()
        {
            return View();
        }

        //Email Verification method
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VeryfyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Something is wrong !");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
                }

            }
            return View(model);
        }


        //Changed password  
        public IActionResult ChangePassword(string username )
        {
            if (string.IsNullOrEmpty(username ))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel { Email = username});
        }

        [HttpPost]

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email not found !");
                    return View(model);
                }
            }
            else 
            {
                ModelState.AddModelError("", "Something went wrong. try again !");
                return View(model);
            }

        }


        //Logut in navbar
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


        //Display all registerd Users
        public async Task<IActionResult> RegisteredUsers()
        {
            var users = userManager.Users.ToList(); // await userManager.Users.ToListAsync(); 
            return View(users);
        }



        //Get//Edit register users
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        //Post //Edit register users
        [HttpPost]
        public async Task<IActionResult> EditUser(Users model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.Email;// Identity usually links Email and UserName
                user.UserType = model.UserType;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("RegisteredUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        //Delete registerd users
        //Get: Delete
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("RegisteredUsers");
            }

            // Optional: Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("RegisteredUsers");
        }
        /// <summary>
        /// /////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>

        //change password in registered users

        //Email Verification  
        public IActionResult RegisteredUserEmailVerify()
        {
            return View();
        }


        //Email Verification method
        [HttpPost]
        public async Task<IActionResult> RegisteredUserEmailVerify(VeryfyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Something is wrong !");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("RegisteredUserPasswordChange", "Account", new { username = user.UserName });
                }

            }
            return View(model);
        }

        //Registered User Password change 
        //Changed password  
        public IActionResult RegisteredUserPasswordChange(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("RegisteredUserEmailVerify", "Account");
            }
            return View(new ChangePasswordViewModel { Email = username });
        }

        // <summary>
        /// 
        /// </summary>
        /* */
        [HttpPost]

        public async Task<IActionResult> RegisteredUserPasswordChange(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("RegisteredUsers", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);

                    }

                }
                else
                {
                    ModelState.AddModelError("", "Email not found !");
                    return View(model);
                }
            }
            else
            {

                ModelState.AddModelError("", "Something went wrong. try again !");
                return View(model);


            }

        }
       
    }
}
