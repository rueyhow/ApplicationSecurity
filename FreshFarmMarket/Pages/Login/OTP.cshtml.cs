using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Asn1.Eac;

namespace FreshFarmMarket.Pages.Login
{
    public class OTPModel : PageModel
    {
        private readonly auditService _auditService;
        private readonly SignInManager<MarketUser> _signInManager;

        private readonly UserManager<MarketUser> _userManager;
        public OTPModel(SignInManager<MarketUser> signInManager, UserManager<MarketUser> userManager, auditService auditService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auditService = auditService;
        }
        [BindProperty]
        public string? pw { get; set; }
        [BindProperty]
        public string? em { get; set; }
        [BindProperty]
        public string? RM { get; set; }
        public async Task<IActionResult> OnGetAsync(string Password , string Email, string RememberMe)
        {
            pw = Password;
            em = Email;
            RM = RememberMe;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string pass , string mail , string Remember , string digit1, string digit2, string digit3, string digit4, string digit5, string digit6 , string OTPText)
        {
            // finding the user
            string OTPText1 = digit1 + digit2 + digit3 + digit4 + digit5 + digit6;
            bool t = false;
            var userIdentity = await _userManager.FindByNameAsync(mail);
            if (userIdentity == null)
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "error";
                return Page();
            }
            if (RM == "true")
            {
                t = true;
            }
            
            if (OTPText == OTPText1)
            {
                var identity = await _signInManager.PasswordSignInAsync(userIdentity, pass, t, false);
                if (identity.Succeeded)
                {
                    // adding session information
                    if (HttpContext.Session.GetString("Email") == null)
                    {
                        Audit a = new Audit()
                        {
                            UserId = userIdentity.UserName,
                            userEmail = userIdentity.UserName,
                            Action = "Login",
                            DateTime = DateTime.Now,
                        };
                        _auditService.record(a);
                        HttpContext.Session.Remove("OTP");
                        HttpContext.Session.SetString("Email", mail);
                        TempData["FlashMessage.Type"] = "success";
                        TempData["FlashMessage.Text"] = "User Successfully Logged In";
                        return RedirectToPage("../user/information");
                    }
                    TempData["FlashMessage.Type"] = "success";
                    TempData["FlashMessage.Text"] = "You have already logged in";
                    return RedirectToPage("../user/information");

                }

                // see if the user has already met the max attempts
                if (userIdentity.AccessFailedCount >= 3)
                {
                    userIdentity.LockoutEnabled = true;
                    await _userManager.UpdateAsync(userIdentity);
                    TempData["FlashMessage.Type"] = "danger";
                    TempData["FlashMessage.Text"] = "User not Logged In. Account lockout after 3 login failures";
                    return RedirectToPage("./index");
                }
                // setting limits of failed attempts
                userIdentity.AccessFailedCount += 1;
                await _userManager.UpdateAsync(userIdentity);


                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "User not Logged In. Username or password is incorrect. Number of attempts left : " + (3 - userIdentity.AccessFailedCount);
                return RedirectToPage("index");
            }
            TempData["FlashMessage.Type"] = "danger";
            TempData["FlashMessage.Text"] = "OTP was wrong, please request enter again " + OTPText + OTPText1;
            return RedirectToPage("index");
        }
    }
}
