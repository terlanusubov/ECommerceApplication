using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Areas.Admin.Models;
using DahlizApp.Data;
using DahlizApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DahlizApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly DahlizDb db;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(DahlizDb _db,UserManager<User> _usermanager,SignInManager<User> _signinmanager)
        {
            db = _db;
            userManager = _usermanager;
            signInManager = _signinmanager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    bool isInRole =  await userManager.IsInRoleAsync(user, "Admin");
                    if (isInRole)
                    {
                        Microsoft.AspNetCore.Identity.SignInResult result =  await signInManager.PasswordSignInAsync(user, model.Password, true, true);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Username or password is not correct");
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "You dont have a permission to enter the system");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User is not exists");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Check blanks again!");
                return View();
            }
        }

        public async Task<IActionResult> Signout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}