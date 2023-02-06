using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace FreshFarmMarket.Pages.Login
{
    public class forgotPasswordModel : PageModel
    {
        private readonly auditService _auditService;

        private readonly PasswordService _passwordService;

        private readonly EmailService _emailService;

        private readonly SignInManager<MarketUser> _signInManager;

        private readonly UserManager<MarketUser> _userManager;

        public forgotPasswordModel(SignInManager<MarketUser> signInManager, UserManager<MarketUser> userManager , PasswordService passwordService , EmailService emailService , auditService auditService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordService = passwordService;
            _emailService = emailService;
            _auditService = auditService;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostForgotpasswordAsync(string email)
        {
            MarketUser? userIdentity = await _userManager.FindByNameAsync(email);

            if (userIdentity == null)
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "Email does not belong to a user";
                return Page();
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(userIdentity);

            var dataProtect = DataProtectionProvider.Create("encrypt");
            var protect = dataProtect.CreateProtector("creditcard");
            var encrypted = protect.Protect(token);

            var finalToken = WebEncoders.Base64UrlEncode(Encoding.UTF32.GetBytes(encrypted));
            userIdentity.ResetTokenExpire = DateTime.Now.AddDays(1);
            await _userManager.UpdateAsync(userIdentity);

            Audit a = new Audit()
            {
                UserId = userIdentity.UserName,
                userEmail = userIdentity.UserName,
                Action = "sent forgot password link",
                DateTime = DateTime.Now,
            };
            _auditService.record(a);

            // send user an email link to change their password
            var link = $"https://localhost:7141/Login/ResetPassword/{finalToken}/{email}";

            _emailService.SendingEmail(email, link, "Reset Password").Wait();

            TempData["FlashMessage.Type"] = "success";
            TempData["FlashMessage.Text"] = "Email sent to you! at " + email;
            return Page();
        }

    }
}
