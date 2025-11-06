using MessApi.Models;
using MessApi.UnitOfWork;
using MessManagement.Shared.DTOs;

namespace MessApi.IRepositories
{
    public interface IMessMemberRepository:IRepository<MessMember>
    {
        Task<List<MessMemberSummaryDto>> GetMessMemberSummaryAsync(int messId, int userId);
    }
}
