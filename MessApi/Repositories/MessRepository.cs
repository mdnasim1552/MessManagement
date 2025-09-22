using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Models;
using MessApi.UnitOfWork;

namespace MessApi.Repositories
{
    public class MessRepository:Repository<Mess>,IMessRepository
    {
        private readonly ApplicationDbContext _db;
        public MessRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }
    }
}
