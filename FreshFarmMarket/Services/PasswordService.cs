using FreshFarmMarket.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;
using System.Text;

namespace FreshFarmMarket.Services
{
    public class PasswordService

    {
        private readonly SignInManager<MarketUser> _signInManager;

        private readonly UserManager<MarketUser> _userManager;

        public PasswordService(SignInManager<MarketUser> signInManager, UserManager<MarketUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // offically change password
        public async Task<string> ChangePassword(MarketUser user , string newpassword , string Token)
        {
            string final = Encoding.UTF32.GetString(WebEncoders.Base64UrlDecode(Token));
            var dataProtect = DataProtectionProvider.Create("encrypt");
            var protect = dataProtect.CreateProtector("creditcard");
            var encrypted = protect.Unprotect(final);



            DateTime? now = DateTime.Now;
            TimeSpan seconds = (TimeSpan)(user.ResetTokenExpire - now);
            if (seconds.Seconds < 1)
            {
                return "token expire";
            }
            var result = _userManager.ResetPasswordAsync(user, encrypted, newpassword);

            await _userManager.UpdateAsync(user);
            return "success";


        }

        public async Task<string> passwordMatters(string password , string action)
        {
            string passwordHash = "";
            if (action == "hash")
            {
                passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }
            return passwordHash;
        }
    }
}
