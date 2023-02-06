using System.ComponentModel.DataAnnotations;

namespace FreshFarmMarket.Models
{
    public class CaptchaView
    {
        [Required]
        public string CaptCha { get; set; }
    }
}
