using FreshFarmMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreshFarmMarket.Pages.Login
{
    public class logoutModel : PageModel
    {
        private readonly SignInManager<MarketUser> signInManager;
        public logoutModel(SignInManager<MarketUser> signInManager)
        {
            this.signInManager = signInManager;
        }
        public void OnGet() { }
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Email");
            return RedirectToPage("../Index");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("../Index");
        }
    }
}
