using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task4_registration_and_authentication.Models;
using Task4_registration_and_authentication.ViewModels;

namespace Task4_registration_and_authentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            this.signInManager = signInManager;

            this.userManager = userManager; 

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(user.UserName!, model.Password!, isPersistent: false, lockoutOnFailure: false);

                    if (user.IsBlocked)
                    {
                        ModelState.AddModelError("", "This account is blocked.");
                        return View(model);
                    }

                    if (result.Succeeded)
                    {
                        user.LastLoginTime = DateTime.Now;
                        await userManager.UpdateAsync(user);
                        return RedirectToAction("UserManagement", "Account");
                    }
                    else if (result.IsLockedOut)
                    {
                        ModelState.AddModelError("", "This account has been locked out, please try again later.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            return View(model);
        }


        public async Task<IActionResult> UserManagement()
        {
            var users = await userManager.Users.Select(u => new UserManagementVM
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email,
                LastLoginTime = u.LastLoginTime,
                IsBlocked = u.IsBlocked
            }).ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> BlockUsers(List<string> selectedUsers)
        {
            foreach (var userId in selectedUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.IsBlocked = true;
                    await userManager.UpdateAsync(user);
                }
            }
            // Log out the current user if all users are blocked
            if (selectedUsers.Count == userManager.Users.Count())
            {
                await signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("UserManagement");
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUsers(List<string> selectedUsers)
        {
            foreach (var userId in selectedUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.IsBlocked = false;
                    await userManager.UpdateAsync(user);
                }
            }
            return RedirectToAction("UserManagement");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUsers(List<string> selectedUsers)
        {
            foreach (var userId in selectedUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await userManager.DeleteAsync(user);
                }
            }
            return RedirectToAction("UserManagement");
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Name = model.Name,
                    UserName = model.Name, //changed email
                    Email = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
