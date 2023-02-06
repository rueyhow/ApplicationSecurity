using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace FreshFarmMarket.Pages.user
{
    [Authorize]
    public class informationModel : PageModel
    {
        
        public MarketUser currentUser { get; set; }

        private readonly IConfiguration _configuration;

        private readonly FreshFarmMarketContext _context;

        private readonly IWebHostEnvironment _environment;

        private UserManager<MarketUser> userManager { get; }
        private SignInManager<MarketUser> signInManager { get; }
        public informationModel(UserManager<MarketUser> userManager, SignInManager<MarketUser> signInManager, FreshFarmMarketContext context, IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string email = HttpContext.Session.GetString("Email");

            System.Security.Claims.ClaimsPrincipal currentUser1 = this.User;
            var user = await userManager.GetUserAsync(User);

            FreshFarmMarket.Models.MarketUser? current = await userManager.FindByNameAsync(user.UserName);

            if (current == null)
            {
                TempData["FlashMessage.Type"] = "error";
                TempData["FlashMessage.Text"] = "User was not found in database";
                return RedirectToPage("../index");
            }
            // setting up data protector 
            var dataProtect = DataProtectionProvider.Create("encrypt");
            var protect = dataProtect.CreateProtector("creditcard");
            var str = "hello";

            // decrypting credit card number
            currentUser = user;
            currentUser.CreditCardNo = protect.Unprotect(user.CreditCardNo);
            return Page();
        }
    }
}
