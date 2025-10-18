using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Models;
using MessApi.UnitOfWork;

namespace MessApi.Repositories
{
    public class UnitRepository:Repository<Unit>,IUnitRepository
    {
        private readonly ApplicationDbContext _db;
        public UnitRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
