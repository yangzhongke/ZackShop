using BackEnd.Shared;

namespace UsersDomain.Shared.Entities;
public class User
{
    public Guid Id { get; init; }
    public string Email { get; set; }
    public string PasswordHash { get;private set; }

    private User()
    {
    }

    public User(string email, string password)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = new HashHelper().SHA256Hash(password);
    }

    public void ChangePassword(string newPassword)
    {
        PasswordHash = new HashHelper().SHA256Hash(newPassword);
    }

    public bool ValidatePassword(string password)
    {
        return PasswordHash == new HashHelper().SHA256Hash(password);
    }
}