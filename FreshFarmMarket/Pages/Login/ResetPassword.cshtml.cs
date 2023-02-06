using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FreshFarmMarket.Pages.Login
{
    public class ResetPasswordModel : PageModel
    {
        private readonly auditService _auditService;
        private readonly PasswordService _passwordService;
        private readonly SignInManager<MarketUser> _signInManager;

        private readonly UserManager<MarketUser> _userManager;
        [BindProperty]
        public MarketUser user1 { get; set; } = new();
        [BindProperty, DisplayName("Confirm Password"), Required]
        public string confirmPassword1 { get; set; }
        [BindProperty]
        public string Token { get; set; }
        [BindProperty]
        public string Email { get; set; }
        public ResetPasswordModel(SignInManager<MarketUser> signInManager, UserManager<MarketUser> userManager , PasswordService passwordService , auditService auditService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordService = passwordService;
            _auditService = auditService;
        }
        public async Task<IActionResult> OnGetAsync(string token , string email)
        {
            Token = token;
            Email = email;
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            MarketUser user = await _userManager.FindByNameAsync(Email);

            var checkHash = new PasswordHasher<MarketUser>();

            Uri address = new Uri(Request.Host.ToString());
            var str = await _passwordService.ChangePassword(user, confirmPassword1, Token);

            Audit a = new Audit()
            {
                UserId = user.UserName,
                userEmail = user.UserName,
                Action = "changed password",
                DateTime = DateTime.Now,
            };
            _auditService.record(a);
            TempData["FlashMessage.Type"] = "success";
            TempData["FlashMessage.Text"] = "Password Changed" + str;
            return RedirectToPage("index");
        }
            
    }
}
