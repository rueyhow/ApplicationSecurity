using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FreshFarmMarket.Models
{
    public class FreshFarmMarketContext : IdentityDbContext
    {
        private readonly IConfiguration _configuration;
        public FreshFarmMarketContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("MyConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<MarketUser> MarketUsers { get; set; }
        public DbSet<Audit> Audits { get; set; }
    }
}
