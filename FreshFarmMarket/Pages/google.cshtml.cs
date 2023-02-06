using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FreshFarmMarket.Pages
{
    public class googleModel : PageModel
    {
        private readonly SignInManager<MarketUser> _signInManager;

        private readonly UserManager<MarketUser> _userManager;
        public googleModel(SignInManager<MarketUser> signInManager, UserManager<MarketUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                TempData["FlashMessage.Type"] = "success";
                TempData["FlashMessage.Text"] = "Password is ok";
                return Page();
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await _userManager.FindByNameAsync(email);
                    if (user == null)
                    {
                        user = new MarketUser()
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),

                        };
                        await _userManager.CreateAsync(user);
                    }

                    await _userManager.AddLoginAsync(user, info); //adds exteranl login info that links the email together 
                    await _signInManager.SignInAsync(user, isPersistent: true);

                }
                TempData["FlashMessage.Type"] = "success";
                TempData["FlashMessage.Text"] = "Password is ok";
                return RedirectToPage("/user/information");
            }

        }
    }
}
