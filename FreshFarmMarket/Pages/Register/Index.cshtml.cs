using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FreshFarmMarket.Pages.Register
{
    public class IndexModel : PageModel
    {
        private auditService _auditService;
        private readonly PasswordService _passwordService;
        private readonly IConfiguration _configuration;

        private readonly FreshFarmMarketContext _context;

        private readonly IWebHostEnvironment _environment;


        private UserManager<MarketUser> userManager { get; }
        private SignInManager<MarketUser> signInManager { get; }
        public IndexModel(UserManager<MarketUser> userManager,SignInManager<MarketUser> signInManager , FreshFarmMarketContext context, IWebHostEnvironment environment, IConfiguration configuration , PasswordService passwordService , auditService auditService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
            _environment = environment;
            _configuration = configuration;
            _passwordService = passwordService;
            _auditService = auditService;
        }
        public static List<String> ELists = new List<String>();
        [BindProperty]
        public MarketUser user { get; set; } = new();

        [BindProperty, DisplayName("New Post Image (jpg ONLY)")]
        public IFormFile? Upload { get; set; }
        [BindProperty , DisplayName("Confirm Password") , Required]
        public string confirmPassword1 { get; set; }
        public static Boolean PasswordCheck(string password)
        {
        List<String> errorLists = new List<String>();
        var regexItem = new Regex("^[a-zA-Z0-9 ]*$");

            if (password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsNumber) && !regexItem.IsMatch(password)){
                return true;
            }
            if (!password.Any(char.IsUpper))
            {
                errorLists.Add("Password needs to have uppercase letters");
            }
            if (!password.Any(char.IsLower))
            {
                errorLists.Add("Password should have lowercase letters");
            }
            if (!password.Any(char.IsNumber))
            {
                errorLists.Add("Password should contain a number");
            }
            if (regexItem.IsMatch(password))
            {
                errorLists.Add("Password should contain a special character");
            }
            ELists = errorLists;
            return false;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // password complexity check
            if (PasswordCheck(user.Password) == true)
            {
                TempData["FlashMessage.Type"] = "success";
                TempData["FlashMessage.Text"] = "Password is ok";
            }
            else
            {

                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "Password is not ok";
                {
                    string error = "";
                    foreach (var error1 in ELists)
                    {
                        error += error1 + " + ";
                    };
                    TempData["FlashMessage.Type"] = "error";
                    TempData["FlashMessage.Text"] = error;
                    return Page();
                }
            }
            // check if email is unique
            MarketUser? userCheck = await userManager.FindByNameAsync(user.Email);
            if (userCheck != null)
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "Email has already been used";
                return Page();
            }
            else
            {
                if (user.Password != confirmPassword1)
                {
                    TempData["FlashMessage.Type"] = "danger";
                    TempData["FlashMessage.Text"] = "Passwords do not match!";
                    return Page();
                }
                user.UserName = user.FullName;
                // encrypting user credit card information
                // setting up data protector 
                var dataProtect = DataProtectionProvider.Create("encrypt");
                var protect = dataProtect.CreateProtector("creditcard");
                var encrypted = protect.Protect(user.CreditCardNo);
                user.CreditCardNo = encrypted;

                // handling uploading of photo
                string random = Guid.NewGuid().ToString();
                string webRootPath = "/uploads/profilePicture/" + random + "-" + Upload.FileName;
                var file = Path.Combine(_environment.WebRootPath, "uploads", "profilePicture", random + "-" + Upload.FileName);
                await using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await Upload.CopyToAsync(fileStream);
                }

                user.PhotoPath = webRootPath;
                //user.Password = await _passwordService.passwordMatters(user.Password, "hash");

                var user1 = new MarketUser()
                {
                    FullName = user.FullName,
                    UserName = user.Email,
                    Email = user.Email,
                    CreditCardNo = user.CreditCardNo,
                    Gender = user.Gender,
                    MobileNo = user.MobileNo,
                    Address = user.Address,
                    PhotoPath = user.PhotoPath,
                    AboutMe = user.AboutMe,
                };
                var result = await userManager.CreateAsync(user1, user.Password);

                Audit a = new Audit()
                {
                    UserId = user.UserName,
                    userEmail = user.UserName,
                    Action = "Register",
                    DateTime = DateTime.Now,
                };
                
                if (result.Succeeded) {
                    _auditService.record(a);
                    HttpContext.Session.SetString("Email", user.Email);
                    await signInManager.SignInAsync(user, false);
                    TempData["FlashMessage.Type"] = "success";
                    TempData["FlashMessage.Text"] = "User Successfully added" + user.Email;
                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["FlashMessage.Type"] = "danger";
                    TempData["FlashMessage.Text"] = "Error";
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return Page();
                }
            }

        }
    }
}
