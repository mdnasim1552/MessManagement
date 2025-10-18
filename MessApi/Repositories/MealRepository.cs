using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Models;
using MessApi.UnitOfWork;

namespace MessApi.Repositories
{
    public class MealRepository: Repository<Meal>, IMealRepository
    {
        private readonly ApplicationDbContext _db;
        public MealRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
