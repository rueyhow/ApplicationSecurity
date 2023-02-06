using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using FreshFarmMarket.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using FreshFarmMarket.Services;
using GoogleReCaptcha.V3.Interface;
using System.Configuration;
using System.Security.Policy;

namespace FreshFarmMarket.Pages.Login
{
    [ValidateAntiForgeryToken]
    public class indexModel : PageModel
    {
        private readonly EmailService _emailService;
        private readonly PasswordService _passwordService;
        private readonly reCaptchaService _reCaptchaService;

        [BindProperty]
        public FreshFarmMarket.Models.Login LModel { get; set; }

        private readonly SignInManager<MarketUser> _signInManager;

        private readonly UserManager<MarketUser> _userManager;
        [BindProperty]
        public string OTPText { get; set; }

        public indexModel(SignInManager<MarketUser> signInManager , UserManager<MarketUser> userManager , reCaptchaService recaptchaService , PasswordService passwordService , EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _reCaptchaService = recaptchaService;
            _passwordService = passwordService;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostGoogleauthAsync()
        {
            return new ChallengeResult("Google" , _signInManager.ConfigureExternalAuthenticationProperties("Google" , "/google"));
        }
        public async Task<IActionResult> OnPostAsync(string token)
        {
            //if (!ModelState.IsValid)
            //{
            //    TempData["FlashMessage.Type"] = "error";
            //    TempData["FlashMessage.Text"] = "error";
            //    return Page();
            //}

            var reCaptcharesult = _reCaptchaService.tokenVerify(token);
            if (!reCaptcharesult.Result.success && reCaptcharesult.Result.score <= 0.5)
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "You are not a human, try again" + token;
                return Page();

            }
            // searching for user identity object
            var userIdentity = await _userManager.FindByNameAsync(LModel.Email);
            var OTP = await _userManager.GenerateTwoFactorTokenAsync(userIdentity , "Email");
            await _emailService.SendingEmail(LModel.Email, OTP, "OTP For login");
            TempData["FlashMessage.Type"] = "success";
            TempData["FlashMessage.Text"] = "Have sent OTP to your email at " + LModel.Email;
            if (HttpContext.Session.GetString("OTP") == null)
            {
                HttpContext.Session.SetString("OTP", OTP);
            }
            return RedirectToPage("OTP", new { Password = LModel.Password , Email = LModel.Email, RememberMe = LModel.RememberMe });
        }
    }
}
