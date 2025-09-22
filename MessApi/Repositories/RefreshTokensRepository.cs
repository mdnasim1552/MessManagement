using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Models;
using MessApi.UnitOfWork;

namespace MessApi.Repositories
{
    public class RefreshTokensRepository:Repository<RefreshToken>, IRefreshTokensRepository
    {
        private readonly ApplicationDbContext _db;
        public RefreshTokensRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
