using BackEnd.Shared;
using Microsoft.EntityFrameworkCore;
using UsersDomain.Shared.Entities;

namespace UsersDomain.Shared.Repositories
{
    public interface IUserRepository: IService
    {
        Task<Guid> InsertAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
    }
    public class UserRepository : IUserRepository
    {
        private readonly UsersDbContext _context;

        public UserRepository(UsersDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return _context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<Guid> InsertAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user.Id;
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
