using BackEnd.Shared;
using RestClients.Shared.ZackCRM;
using UsersDomain.Shared.Entities;
using UsersDomain.Shared.Exceptions;
using UsersDomain.Shared.Helpers;
using UsersDomain.Shared.Repositories;

namespace AuthOpenAPIs.Services;

public interface IUserService: IService
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Guid> CreateUserAsync(string email, string password, CancellationToken cancellationToken);
    Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword, CancellationToken cancellationToken);
    public Task SyncWithZackCrmAsync(CancellationToken cancellationToken);
}
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITimeSpanHelper _timeSpanHelper;
    private readonly IZackCRMClient _zackCRMClient;
    public UserService(IUserRepository userRepository, ITimeSpanHelper timeSpanHelper, IZackCRMClient zackCRMClient)
    {
        _userRepository = userRepository;
        _timeSpanHelper = timeSpanHelper;
        _zackCRMClient = zackCRMClient;
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

    public async Task ExportUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsersAsync(cancellationToken);
        foreach(var user in users)
        {
            int monthsWithUs = _timeSpanHelper.GetMonthsWithUs(user.CreatedAt);
            //todo:
        }
    }

    public async Task SyncWithZackCrmAsync(CancellationToken cancellationToken)
    {
        var emailsInCRM = await _zackCRMClient.GetAllUserEmailsAsync(cancellationToken);
        var emailsInDB = (await _userRepository.GetAllUsersAsync(cancellationToken)).Select(u=>u.Email);
        var emailsToAddToCrm = emailsInDB.Except(emailsInCRM);//要添加到CRM中的邮件
        var emailsToAddToDB = emailsInCRM.Except(emailsInDB);//要添加到DB中的邮件
        foreach (var email in emailsToAddToCrm)
        {
            await _zackCRMClient.AddUserAsync(email, cancellationToken);
        }
        foreach (var email in emailsToAddToDB)
        {
            await _userRepository.InsertAsync(new User(email, "123456"), cancellationToken);
        }
    }
}