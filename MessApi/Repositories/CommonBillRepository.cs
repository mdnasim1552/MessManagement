using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Models;
using MessApi.UnitOfWork;

namespace MessApi.Repositories
{
    public class CommonBillRepository:Repository<CommonBill>,ICommonBillRepository
    {
        private readonly ApplicationDbContext _db;
        public CommonBillRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
