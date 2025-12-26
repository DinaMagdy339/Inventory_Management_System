using Inventory.Models;
using Inventory.Repository;
using Inventory.Repository.Emails;
using Inventory.Utility;
using Inventory.ViewModel.AppUser;
using Inventory_Management_System.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Inventory.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ApplicationDbContext context


        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        #region Register
        [AllowAnonymous]

        [HttpGet]

        public IActionResult Register() => View();
        [AllowAnonymous]

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return RedirectToAction("Login");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        #endregion

        #region Login
        [AllowAnonymous]

        [HttpGet]

        public IActionResult Login() => View();
        [AllowAnonymous]

        [HttpPost]

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Email");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user,model.Password, model.RememberMe,lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account is locked.");
                return View(model);
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("", "Please confirm your email first.");
                return View(model);
            }

            ModelState.AddModelError("", "Invalid Email or Password");
            return View(model);
        }


        #endregion


        #region SignOut
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        #endregion


        #region ForgetPassword

        [HttpGet]
        public  IActionResult ForgetPassword()=>View();

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordLink(ForgotPasswordViewModel model)
        {
            if(!ModelState.IsValid) return View(nameof(ForgetPassword),model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is not null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var ResetPasswordLink = Url.Action("ResetPassword", "Account",
                    new { email = model.Email, token = token }, Request.Scheme);
                var email = new Email()
                {
                    To = model.Email,
                    Subject = "Reset Password ",
                    Body = ResetPasswordLink,
                };
                EmailSetting.SendEmail(email);
                return RedirectToAction(nameof(CheckYourInbox));
            }
            ModelState.AddModelError(string.Empty, "Invalid Operation");
            return View(nameof(ForgetPassword), model);
        }
        #endregion

        [HttpGet]
        public IActionResult CheckYourInbox() => View();

        [HttpGet]
        public IActionResult ResetPassword(string email, string Token)
        {
            TempData["email"] = email;
            TempData["Token"] = Token;
            return  View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            string email = TempData["email"]as string ?? string.Empty;
            string token = TempData["Token"]as string ?? string.Empty;

            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return View(nameof(ResetPassword),model);
        }

    }
}      