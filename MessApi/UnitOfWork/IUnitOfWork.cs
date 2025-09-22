using MessApi.IRepositories;

namespace MessApi.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        //IProductRepository Product { get; }
        IUserRepository User { get; }
        IRefreshTokensRepository RefreshTokens { get; }
        IUserRolesRepository UserRoles { get; }
        IMessMemberRepository MessMember { get; }
        IMessRepository Mess {  get; }
        //ICartRepository Cart { get; }
        void Saved();
        Task<bool> SaveAsync();
    }
}
