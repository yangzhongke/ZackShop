using BackEnd.Shared;
using Microsoft.EntityFrameworkCore;
using UsersDomain.Shared.Entities;

namespace UsersDomain.Shared.Repositories
{
    public interface IUserRepository: IService
    {
        Task InsertAsync(User user, CancellationToken cancellationToken);
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

        Task<User?> IUserRepository.GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return _context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        Task IUserRepository.InsertAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Add(user);
            return _context.SaveChangesAsync(cancellationToken);
        }

        Task IUserRepository.UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
