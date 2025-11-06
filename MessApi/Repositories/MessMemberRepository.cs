using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Models;
using MessApi.UnitOfWork;
using MessManagement.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Repositories
{
    public class MessMemberRepository:Repository<MessMember>,IMessMemberRepository
    {
        private readonly ApplicationDbContext _db;
        public MessMemberRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }
        public async Task<List<MessMemberSummaryDto>> GetMessMemberSummaryAsync(int messId,int userId)
        {
            return await _db.MessMemberSummaryResults
                .FromSqlRaw("EXEC GetMessMemberSummary @MessId = {0},@UserId = {1}", messId, userId)
                .ToListAsync();
        }

    }
}
