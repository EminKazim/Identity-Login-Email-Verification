using LessonMigration.Models;
using LessonMigration.Services;
using LessonMigration.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace LessonMigration.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailservice;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailservice = emailService;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);
            AppUser newUser = new AppUser()
            {
                FullName = registerVM.FullName,
                UserName = registerVM.Username,
                Email = registerVM.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(registerVM);
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var link = Url.Action(nameof(VerfyEmail), "Account", new { userId = newUser.Id, token = code, Request.Scheme, Request.Host});
            await _emailservice.SendEmail(newUser.Email, link);
            return RedirectToAction(nameof(EmailVerification));
            
        }
        //public async Task SendEmail(string emailaddress, string url)
        //{
        //    var apiKey = "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";
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress("arastunsa@code.edu.az", "Fiorello");
        //    var subject = "Sending with SendGrid is Fun";
        //    var to = new EmailAddress(emailaddress, "Example User");
        //    var plainTextContent = "and easy to do anywhere, even with C#";
        //    var htmlContent = $"<a href={url}>Click here</a>";
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //    var response = await client.SendEmailAsync(msg);
        //}
        public async Task<IActionResult> VerfyEmail(string userId,string token)
        {
            if (userId == null || token == null) return BadRequest();
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user is null) return BadRequest();
            await _userManager.ConfirmEmailAsync(user, token);
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index","Home");
        }
        public IActionResult EmailVerification()
        {
            return View();
        }
        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);
            AppUser user = await _userManager.FindByEmailAsync(loginVM.UsernameorEmail);
            if(user is null)
            {
                user = await _userManager.FindByNameAsync(loginVM.UsernameorEmail);
            }
            if (user is null)
            {
                ModelState.AddModelError("", "Email or password in wrong");
                return View();
            }
            if (user.IsActiveted)
            {
                ModelState.AddModelError("", "Contact with admin");
                return View(loginVM);
            }
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
            if(!signInResult.Succeeded)
            {
                if (signInResult.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Please confirm your account");
                }
                ModelState.AddModelError("", "Email or password is wrong");
                return View();
            }
            return RedirectToAction("Index","Home");
        }
    }
}
