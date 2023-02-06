using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshFarmMarket.Models
{
    public class MarketUser : IdentityUser
    {
        public int Id { get; set; }
        [Required, MinLength(2), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Your FullName should not have any special characters")]
        public string FullName { get; set; } = null!;

        [Required, MinLength(2), MaxLength(500)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid credit card number")]
        public string CreditCardNo { get; set; } = null!;

        [Required, MaxLength(1)]
        public string Gender { get; set; } = null!;

        [Required, MinLength(1), MaxLength(8)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid phone number")]
        public string MobileNo { get; set; } = null!;

        [Required, MinLength(10), MaxLength(40)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Your Address should not have any special characters")]
        public string Address { get; set; } = null!;

        [Required, MinLength(10), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be valid email address")]
        public string Email { get; set; } = null!;
        [Required, MinLength(12), MaxLength(100)]
        public string Password { get; set; } = null!;

        [Required]
        public string PhotoPath { get; set; } = null!;
        [Required, MinLength(1), MaxLength(100)]
        public string AboutMe { get; set; } = null!;

        public string? PasswordResetToken { get; set; }

        public DateTime? ResetTokenExpire { get; set; }

        public DateTime? passwordAge { get; set; }

        public string? PasswordHistory { get; set; }

    }
}
