using AuthOpenAPIs.Services;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using RestClients.Shared.ZackCRM;
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
            //Arrange
            string newPassword = oldPassword + "_new";
            User user =new User(email, oldPassword);
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns(user);//为Fake类的GetByEmailAsync方法设置模拟实现
            //Act
            bool ret = await sut.ChangePasswordAsync(email, oldPassword, newPassword, cancellationToken);

            //Assert
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
            //Arrange
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns((User?)null);
            //Act
            Guid userId = await sut.CreateUserAsync(email, password, cancellationToken);
            //Assert
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
            //Arrange
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns(existingUser);
            //Act
            User? user  = await sut.GetUserByEmailAsync(email, cancellationToken);
            //Assert
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
            //Arrange
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns((User?)null);
            //Act
            User? user = await sut.GetUserByEmailAsync(email, cancellationToken);
            
            //Assert
            user.Should().Be(null);
        }

        [Theory]
        [AutoFakeItEasy]
        public async Task SyncWithZackCrmAsync_Successfully(
            [Frozen] IUserRepository userRepository,
            [Frozen] IZackCRMClient zackCRMClient,
            UserService sut,
            CancellationToken cancellationToken)
        {
            //Arrange
            string emailInCRMOnly = "emailInCRMOnly@test.com";
            string emailInDBOnly = "emailInDBOnly@test.com";
            string emailInDBAndCRM = "inBoth@test.com";

            A.CallTo(() => zackCRMClient.GetAllUserEmailsAsync(cancellationToken))
                .Returns([emailInCRMOnly , emailInDBAndCRM]);
            A.CallTo(() => userRepository.GetAllUsersAsync(cancellationToken))
                .Returns([new User(emailInDBOnly,"123"), new User(emailInDBAndCRM,"123")]);
            //Act
            await sut.SyncWithZackCrmAsync(cancellationToken);

            //Assert
            A.CallTo(() => userRepository.InsertAsync(A<User>.That.Matches(u => u.Email == emailInCRMOnly), cancellationToken))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => zackCRMClient.AddUserAsync(emailInDBOnly, cancellationToken))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userRepository.InsertAsync(A<User>.That.Matches(u => u.Email == emailInDBAndCRM), cancellationToken))
                .MustNotHaveHappened();
            A.CallTo(() => zackCRMClient.AddUserAsync(emailInDBAndCRM, cancellationToken))
                .MustNotHaveHappened();
        }
    }
}
