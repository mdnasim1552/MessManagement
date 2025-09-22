using MessApi.Data;
using MessApi.IRepositories;
using MessApi.Repositories;
namespace MessApi.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        //public IProductRepository Product { get; private set; }
        public IUserRepository User { get; private set; }
        public IRefreshTokensRepository RefreshTokens { get; private set; }
        public IUserRolesRepository UserRoles { get; private set; }
        public IMessRepository Mess { get; private set; }
        public IMessMemberRepository MessMember { get; private set; }
        //public ICartRepository Cart { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            //Product = new ProductRepository(context);
            User = new UserRepository(context);
            RefreshTokens = new RefreshTokensRepository(context);
            UserRoles = new UserRolesRepository(context);
            Mess = new MessRepository(context);
            MessMember = new MessMemberRepository(context);
            //Cart = new CartRepository(context);
        }

       

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Saved()
        {
            _context.SaveChanges();
        }
        public async Task<bool> SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Handle exceptions as needed
                return false;
            }
        }

    }
}
