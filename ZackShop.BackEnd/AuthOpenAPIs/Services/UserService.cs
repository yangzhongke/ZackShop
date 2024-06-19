using BackEnd.Shared;
using UsersDomain.Shared.Entities;
using UsersDomain.Shared.Exceptions;
using UsersDomain.Shared.Repositories;

namespace AuthOpenAPIs.Services;

public interface IUserService: IService
{
    Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ValidateUserCredentialsAsync(string email, string password, CancellationToken cancellationToken);
    Task<Guid> CreateUserAsync(string email, string password, CancellationToken cancellationToken);
    Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword, CancellationToken cancellationToken);
    Task<bool> LockUserAsync(string email, CancellationToken cancellationToken);
    Task<bool> UnlockUserAsync(string email, CancellationToken cancellationToken);
    Task<bool> ResetPasswordAsync(string email, string newPassword, CancellationToken cancellationToken);
    Task IncreaseLoginAttemptsAsync(string email, CancellationToken cancellationToken);
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

    public Task<Guid> CreateUserAsync(string email, string password, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task IncreaseLoginAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LockUserAsync(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ResetPasswordAsync(string email, string newPassword, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UnlockUserAsync(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ValidateUserCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}