using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Models;
using MessApi.UnitOfWork;
using MessManagement.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Repositories
{
    public class MessRepository:Repository<Mess>,IMessRepository
    {
        private readonly ApplicationDbContext _db;
        public MessRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }
        public async Task<List<MessDto>> GetMessSummaryAsync(int userId)
        {
            return await _db.MessSummaryResults
                .FromSqlRaw("EXEC GET_MESS_BY_USER @UserId = {0}", userId)
                .ToListAsync();
        }
    }
}
