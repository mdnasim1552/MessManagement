using MessApi.Models;
using MessApi.UnitOfWork;
using MessManagement.Shared.DTOs;

namespace MessApi.IRepositories
{
    public interface IMessRepository:IRepository<Mess>
    {
        Task<List<MessDto>> GetMessSummaryAsync(int userId);
    }
}
