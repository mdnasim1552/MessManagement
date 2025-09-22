using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Models;
using MessApi.UnitOfWork;

namespace MessApi.Repositories
{
    public class MessMemberRepository:Repository<MessMember>,IMessMemberRepository
    {
        private readonly ApplicationDbContext _db;
        public MessMemberRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }
    }
}
