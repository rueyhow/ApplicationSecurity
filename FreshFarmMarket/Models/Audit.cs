namespace FreshFarmMarket.Models
{
    public class Audit
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public string? Action { get; set; }
        public DateTime DateTime { get; set; }

        public string? userEmail { get; set; }
    }
}
