using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Models;
using MessApi.UnitOfWork;

namespace MessApi.Repositories
{
    public class MarketCostsRepository:Repository<MarketCost>, IMarketCostsRepository
    {
        private readonly ApplicationDbContext _db;
        public MarketCostsRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
