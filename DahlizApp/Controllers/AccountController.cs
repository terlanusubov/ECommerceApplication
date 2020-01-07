using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DahlizApp.Data;
using DahlizApp.Models;
using DahlizApp.Models.Enums;
using DahlizApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DahlizApp.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using DahlizApp.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DahlizApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly DahlizDb db;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly PasswordValidator<User> passwordValidator;


        public AccountController(DahlizDb _db, SignInManager<User> _signInmanager,
                                    UserManager<User> _userManager)
        {
            db = _db;
            signInManager = _signInmanager;
            userManager = _userManager;
            passwordValidator = new PasswordValidator<User>();
        }

        [HttpGet]
        [AllowAnonymous]
        public Task ExternalLogin(string provider)
        {
            return HttpContext.ChallengeAsync(provider, new AuthenticationProperties() { RedirectUri = "/Account/ExternalLoginCallback" });
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallBack()
        {
            var authResult = await HttpContext.AuthenticateAsync("TempCookie");
            if (!authResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
            var email = authResult.Principal.FindFirstValue(ClaimTypes.Email);
            var name = authResult.Principal.FindFirstValue(ClaimTypes.GivenName);
            var surname = authResult.Principal.FindFirstValue(ClaimTypes.Surname);

            await HttpContext.SignOutAsync("TempCookie");
            var findUserResult = await userManager.FindByEmailAsync(email);
            var userNameGuid = db.Users.Count();
            if (findUserResult == null)
            {
                var user = new User()
                {
                    Email = email,
                    Surname = surname,
                    Name = name,
                    UserName = name + surname + userNameGuid,
                    EmailConfirmed = true
                };
                var createUserResult = await userManager.CreateAsync(user);
                if (createUserResult.Succeeded)
                {
                    HttpContext.SetUserInfoToSession("user_name", user.Name);
                    HttpContext.SetUserInfoToSession("user_id", user.Id);
                    HttpContext.SetUserInfoToSession("user_surname", user.Surname);

                    await signInManager.SignInAsync(user, true);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Detail", "Account");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                var user = await userManager.FindByEmailAsync(email);
                HttpContext.SetUserInfoToSession("user_name", user.Name);
                HttpContext.SetUserInfoToSession("user_id", user.Id);
                HttpContext.SetUserInfoToSession("user_surname", user.Surname);

                await signInManager.SignInAsync(user, true);
                return RedirectToAction("Detail", "Account");
            }
        }

        public async Task<IActionResult> Detail()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                string userId = HttpContext.GetUserInfoFromSession("user_id");
                if (userId == null)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    User user = await db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
                    return View(user);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDetail(DetailModel model)
        {
            if (ModelState.IsValid)
            {
                string userId = HttpContext.GetUserInfoFromSession("user_id");
                User user = await userManager.FindByIdAsync(userId);
                user.Name = model.Name;
                user.UserName = model.Username;
                user.Surname = model.Surname;

                if (model.Email != user.Email)
                {
                    user.EmailConfirmed = false;
                }

                user.Email = model.Email;
                user.NormalizedEmail = model.Email.ToUpper();


                user.PhoneNumber = model.Phone;
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Detail));
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong.Fill all required blanks.");
                return RedirectToAction(nameof(Detail));
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByEmailAsync(loginModel.Email);
                if (user != null)
                {
                    bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");
                    if (!isAdmin)
                    {
                        Microsoft.AspNetCore.Identity.SignInResult signInResult = await signInManager.PasswordSignInAsync(user, loginModel.Password, true, false);
                        if (signInResult.Succeeded)
                        {
                            HttpContext.SetUserInfoToSession("user_name", user.Name);
                            HttpContext.SetUserInfoToSession("user_id", user.Id);
                            HttpContext.SetUserInfoToSession("user_surname", user.Surname);
                            return RedirectToAction(nameof(Detail));
                        }
                        else
                        {
                            ModelState.AddModelError("", "Email or Password is wrong");
                            return View(loginModel);
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User is not exists");
                    return View(loginModel);
                }
            }
            else
            {
                ModelState.AddModelError("", "Check your informatiom.Something is wrong");
                return View(loginModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            int langId = HttpContext.GetLanguage("langId");
            if (langId == 0)
            {
                return RedirectToAction("SetLanguage", "Language", new { culture = "en", returnUrl = "/" });
            }

            List<Country> countries = await db.Countries
                                                  .ToListAsync();

            RegisterModel model = new RegisterModel()
            {
                Countries = new SelectList(countries, "Name", "Name")
            };
            HttpContext.Session.SetObjectAsJson("registerStatus", RegisterStatus.NotBegin);
            return View(model);
        }

        #region Register by Terlan
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RegisterStatus(RegisterModel model, [FromServices]EmailService service)
        //{
        //    int langId = HttpContext.GetLanguage("langId");
        //    Language lang = await db.Languages.Where(l => l.Id == langId).FirstOrDefaultAsync();
        //    model.RememberMe = true;
        //    string current_step = HttpContext.Session.GetString("step");

        //    if (current_step == "1")
        //    {
        //        if (model.Name != null && model.Surname != null && model.Email != null && model.Username != null && model.Password != null && model.ConfirmPassword != null)
        //        {
        //            User findUser = await userManager.FindByEmailAsync(model.Email);
        //            if (findUser == null)
        //            {
        //                User user = new User()
        //                {
        //                    Name = model.Name,
        //                    Surname = model.Surname,
        //                    UserName = model.Username,
        //                    Email = model.Email
        //                };
        //                IdentityResult userCreateResult = await userManager.CreateAsync(user, model.Password);
        //                if (userCreateResult.Succeeded)
        //                {
        //                    HttpContext.SetUserInfoToSession("user_name", user.Name);
        //                    HttpContext.SetUserInfoToSession("user_id", user.Id);
        //                    HttpContext.SetUserInfoToSession("user_email", user.Email);
        //                    HttpContext.SetUserInfoToSession("user_surname", user.Surname);
        //                    HttpContext.SetUserInfoToSession("isPresistent", model.RememberMe.ToString());
        //                    ViewBag.Step = 2;
        //                    return RedirectToAction(nameof(Login));
        //                }
        //                else
        //                {
        //                    return View(model);
        //                }
        //            }
        //            else
        //            {
        //                return View(model);
        //            }
        //        }
        //        else
        //        {
        //            return View(model);
        //        }
        //    }
        //    else
        //    {
        //        if (model.City != null && model.Address != null && model.Post != null && model.Country != null)
        //        {
        //            string user_id = HttpContext.Session.GetString("user_id");
        //            User user = await userManager.FindByIdAsync(user_id);
        //            if (user != null)
        //            {
        //                user.City = model.City;
        //                user.Country = model.Country;
        //                user.Address = model.Address;
        //                user.Post = model.Post;

        //                #region Send Confirmation Email
        //                //string Token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        //                //string Tokenlink = Url.Action("ConfirmEmail", "Account", new
        //                //{
        //                //    userId = user.Id,
        //                //    token = Token
        //                //}, protocol: HttpContext.Request.Scheme);

        //                //string htmlMessagedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Email", $"confirmation{lang.Key}.html");

        //                //string message = service.GetEmailTemplate(htmlMessagePath,(x)=>{
        //                //    var text = x.Replace("{ConfirmationLink}", Tokenlink);
        //                //    return text;
        //                //});

        //                //await service.SendMailAsync(user.Email, "Confirmation", message);
        //                #endregion

        //                await db.SaveChangesAsync();
        //                return RedirectToAction(nameof(Login));
        //            }
        //            else
        //            {
        //                return View(model);
        //            }
        //        }
        //        else
        //        {
        //            return View();
        //        }

        //    }

        //}
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegisterStepOne(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User findUser = await userManager.FindByEmailAsync(model.StepOneModel.Email);
                if (findUser == null)
                {
                    User user = new User()
                    {
                        Name = model.StepOneModel.Name,
                        Surname = model.StepOneModel.Surname,
                        UserName = model.StepOneModel.Username,
                        Email = model.StepOneModel.Email
                    };

                    IdentityResult result = await userManager.CreateAsync(user, model.StepOneModel.Password);
                    if (result.Succeeded)
                    {
                        HttpContext.Session.SetObjectAsJson("RegisteredUser", user);
                        HttpContext.Session.SetObjectAsJson("registerStatus", RegisterStatus.Incomplete);
                        return Json(new
                        {
                            status = 200
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        status = 400,
                        error = "The email address is already in use. Please try another email."
                    });
                }
            }
            return Json(new
            {
                status = 400
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model, [FromServices]EmailService service)
        {
            int langId = HttpContext.GetLanguage("langId");
            Language lang = await db.Languages.Where(l => l.Id == langId).FirstOrDefaultAsync();

            User userSession = HttpContext.Session.GetObject<User>("RegisteredUser");

            User user = await userManager.FindByEmailAsync(userSession?.Email ?? "");

            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    user.PhoneNumber = model.StepTwoModel.Phone;
                    user.Country = model.StepTwoModel.Country;
                    user.City = model.StepTwoModel.City;
                    user.Address = model.StepTwoModel.Address;
                    user.Post = model.StepTwoModel.Post;

                    #region Send Confirmation Email
                    string Token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    string Tokenlink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userId = user.Id,
                        token = Token
                    }, protocol: HttpContext.Request.Scheme);


                    string htmlMessagedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Email", $"confirmation{lang.Key}.html");

                    string message = service.GetEmailTemplate(htmlMessagedPath, (x) =>
                    {
                        var text = x.Replace("{ConfirmationLink}", Tokenlink);
                        return text;
                    });

                    await service.SendMailAsync(user.Email, "Confirmation", message);
                    #endregion

                    TempData["IsSend"] = "true";

                    HttpContext.Session.SetObjectAsJson("registerStatus", RegisterStatus.Complete);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Login));
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetCitiesByCountry(string name)
        {
            Country country = await db.Countries.Include(c => c.Cities)
                                           .FirstOrDefaultAsync(c => c.Name == name);

            if (country != null)
            {
                if (country.Cities.Count > 0)
                {
                    return Json(new
                    {
                        status = 200,
                        data = country.Cities
                    });
                }
                return Json(new
                {
                    status = 404,
                    error = "The selected country does not have a city."
                });
            }
            return Json(new
            {
                status = 400,
                error = "The country is not found."
            });
        }

        [HttpPost]
        public async Task<JsonResult> UnfinishedRegister()
        {
            RegisterStatus status = HttpContext.Session.GetObject<RegisterStatus>("registerStatus");
            if (status == RegisterStatus.Incomplete)
            {
                User userSession = HttpContext.Session.GetObject<User>("RegisteredUser");

                User user = await userManager.FindByEmailAsync(userSession?.Email ?? "");

                if (user != null)
                {
                    await userManager.DeleteAsync(user);

                    return Json(new StepOneRegisterModel()
                    {
                        Name = user.Name,
                        Surname = user.Surname,
                        Username = user.UserName,
                        Email = user.Email
                    });
                }
            }

            return Json(null);
        }

        //[HttpPost]
        //public async Task<IActionResult> Confirmation(string email, [FromServices]EmailService service)
        //{
        //    User user = await userManager.FindByEmailAsync(email);
        //    string Token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        //    string TokenLink = Url.Action("ConfirmEmail", "Account", new
        //    {
        //        userId = user.Id,
        //        token = Token
        //    }, protocol: HttpContext.Request.Scheme);
        //    string message = $@"Click link to activate your account {TokenLink}";
        //    await service.SendMailAsync(user.Email, "ACTIVATION", message);

        //    return RedirectToAction(nameof(Detail));
        //}
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            User user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await userManager.ConfirmEmailAsync(user, token);
                return RedirectToAction(nameof(Detail));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> Signout()
        {
            await signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}