using AutoFixture.Xunit2;
using FluentAssertions;
using UsersDomain.Shared.Entities;

namespace UsersDomain.Shared.UnitTests.Entities.UserTests;
public class UserShould
{
    [Theory]//xUnit2
    [AutoData]//AutoFixture
    public void HandleCorrectLogin_Successfully(string email, string password)
    {
        User user = new User(email, password);
        user.ValidatePassword(password).Should().BeTrue();//Should().Be(): FluentAssertions
    }

    [Theory]
    [AutoData]
    public void HandleWrongLogin_Successfully(string email, string password)
    {
        User user = new User(email, password);
        user.ValidatePassword(password + "1").Should().BeFalse();
    }

    [Theory]
    [AutoData]
    public void ChangePassword_Successfully(string email, string password, string newPassword)
    {
        User user = new User(email, password);
        user.ChangePassword(newPassword);
        user.ValidatePassword(newPassword).Should().BeTrue();
    }
}