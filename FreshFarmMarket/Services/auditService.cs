using FreshFarmMarket.Models;

namespace FreshFarmMarket.Services
{
    public class auditService
    {
        private readonly FreshFarmMarketContext _context;

        public auditService(FreshFarmMarketContext context)
        {
            _context = context;
        }
        public void record(Audit log)
        {
            _context.Audits.Add(log);
            _context.SaveChanges();
        }
    }
}
