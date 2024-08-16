using AuthOpenAPIs.Services;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Testing.Shared;
using UsersDomain.Shared.Entities;
using UsersDomain.Shared.Repositories;

namespace AuthenOpenAPIs.UnitTest.Services.UserServiceTests
{
    public class Should
    {
        [Theory]
        [AutoFakeItEasy]
        public async Task ChangePasswordAsync_Successfully(
            [Frozen] IUserRepository userRepository, 
            UserService sut,
            string email, string oldPassword, 
            CancellationToken cancellationToken)
        {
            string newPassword = oldPassword + "_new";
            User user =new User(email, oldPassword);
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns(user);
            bool ret = await sut.ChangePasswordAsync(email, oldPassword, newPassword, cancellationToken);
            ret.Should().BeTrue();
            A.CallTo(() => userRepository.UpdateAsync(user, cancellationToken))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [AutoFakeItEasy]
        public async Task CreateUserAsync_Successfully(
            [Frozen] IUserRepository userRepository,
            UserService sut,
            string email, string password,
            CancellationToken cancellationToken)
        {
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns((User?)null);
            Guid userId = await sut.CreateUserAsync(email, password, cancellationToken);
            userId.Should().NotBeEmpty();
            A.CallTo(() => userRepository.InsertAsync(A<User>.That.Matches(u=>u.Email==email&&u.ValidatePassword(password)), cancellationToken))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [AutoFakeItEasy]
        public async Task GetUserByEmailAsync_Successfully(
            [Frozen] IUserRepository userRepository,
            UserService sut,
            User existingUser,
            string email,
            CancellationToken cancellationToken)
        {
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns(existingUser);
            User? user  = await sut.GetUserByEmailAsync(email, cancellationToken);
            user.Should().Be(existingUser);
        }

        [Theory]
        [AutoFakeItEasy]
        public async Task GetUserByEmailAsync_WhenEmailNotExists_Successfully(
            [Frozen] IUserRepository userRepository,
            UserService sut,
            string email,
            CancellationToken cancellationToken)
        {
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns((User?)null);
            User? user = await sut.GetUserByEmailAsync(email, cancellationToken);
            user.Should().Be(null);
        }

        //todo: ExportUsersAsync
    }
}
