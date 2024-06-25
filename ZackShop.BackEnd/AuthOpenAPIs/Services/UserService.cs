using BackEnd.Shared;
using UsersDomain.Shared.Entities;
using UsersDomain.Shared.Exceptions;
using UsersDomain.Shared.Repositories;

namespace AuthOpenAPIs.Services;

public interface IUserService: IService
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Guid> CreateUserAsync(string email, string password, CancellationToken cancellationToken);
    Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword, CancellationToken cancellationToken);
}
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            throw new UserException("email not found");
        }
        if(!user.ValidatePassword(oldPassword))
        {
            return false;
        }
        user.ChangePassword(newPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        return true;
    }

    public async Task<Guid> CreateUserAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user != null)
        {
            throw new UserException("email already exists");
        }
        User newUser = new User(email, password);
        await _userRepository.InsertAsync(newUser, cancellationToken);
        return newUser.Id;
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _userRepository.GetByEmailAsync(email, cancellationToken);
    }
}