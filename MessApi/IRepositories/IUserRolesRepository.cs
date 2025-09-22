using MessApi.Models;
using MessApi.UnitOfWork;

namespace MessApi.IRepositories
{
    public interface IUserRolesRepository:IRepository<UserRole>
    {
        Task<List<string>> GetRoleNamesByUserIdAsync(int userId);

    }
}
