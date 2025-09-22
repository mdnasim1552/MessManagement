using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Models;
using MessApi.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Repositories
{
    public class UserRolesRepository:Repository<UserRole>, IUserRolesRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRolesRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public async Task<List<string>> GetRoleNamesByUserIdAsync(int userId)
        {
            return await _db.UserRoles.Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();
        }
    }
}
