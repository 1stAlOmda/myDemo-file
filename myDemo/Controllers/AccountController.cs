using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myDemo.Services.EmailMessageService;
using myDemo.ViewModels;
using ORMEFCoreDA.Models;

namespace myDemo.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IEmailService emailService;

        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager, IEmailService emailService)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterVM RegObj)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = RegObj.Email, Email = RegObj.Email, city = RegObj.city };
                var res = await userManager.CreateAsync(user, RegObj.Password);
                if (res.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, HttpContext.Request.Scheme);

                    EmailMessage Msg = new EmailMessage
                    {
                        ToAddresses = new EmailAddress { Name = user.UserName, Address = user.Email },
                        FromAddresses = new EmailAddress { Name = user.UserName, Address = user.Email },
                        Subject = "Confirm your account",
                        Content = $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>"
                    };
                    await emailService.Send(Msg);
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Administration"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    ViewBag.Msg = "Check your email and confirm your account, you must be confirmed "
                         + "before you can log in.";
                    return View("Info");
                    //await signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("Index", "Home");
                }


                foreach (var error in res.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(RegObj);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginVM model = new LoginVM
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM LoginObj, string ReturnUrl)
        {
            LoginObj.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(LoginObj.Email);

                if (user != null && !user.EmailConfirmed &&
                            (await userManager.CheckPasswordAsync(user, LoginObj.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(LoginObj);
                }
                var res = await signInManager.PasswordSignInAsync(LoginObj.Email, LoginObj.Password
                                                      , LoginObj.RemmeberMe, false);
                if (res.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "login faild");

            }
            return View(LoginObj);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailIsInUsed(string Email)
        {
            var Result = await userManager.FindByEmailAsync(Email);

            if (Result != null)
            {
                return Json($"Email {Email} is already in use");
                //return Json(false);

            }

            return Json(true);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
            //return new ChallengeResult();

        }

        [AllowAnonymous]
        public async Task<IActionResult>
ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginVM loginViewModel = new LoginVM
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty,
                    $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            AppUser user = null;

            if (email != null)
            {
                user = await userManager.FindByEmailAsync(email);

                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("Login", loginViewModel);
                }
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(
                                        info.LoginProvider, info.ProviderKey,
                                        isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if (email != null)
                {
                    if (user == null)
                    {
                        user = new AppUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await userManager.CreateAsync(user);

                        // After a local user account is created, generate and log the
                        // email confirmation link
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                        new { userId = user.Id, token = token }, Request.Scheme);

                        EmailMessage Msg = new EmailMessage
                        {
                            ToAddresses = new EmailAddress { Name = user.UserName, Address = user.Email },
                            FromAddresses = new EmailAddress { Name = user.UserName, Address = user.Email },
                            Subject = "Confirm your account",
                            Content = $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>"
                        };
                        await emailService.Send(Msg);
                    }

                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

                return View("Error");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("GeneralError");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorMessage = "Email cannot be confirmed";
            return View("GeneralError");
        }

        //private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        //{
        //    string code = await userManager.GenerateEmailConfirmationTokenAsync(userID);
        //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userID, code = code }, protocol: Request.Url.Scheme);
        //    string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Text" + ".cshtml");
        //    body = body.Replace("@ViewBag.ConfirmationLink", callbackUrl);
        //    body = body.ToString();

        //    await userManager.SendEmailAsync(userID, subject, body);
        //    return callbackUrl;
        //}

    }
}