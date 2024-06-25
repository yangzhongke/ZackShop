using AuthOpenAPIs.Services;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Testing.Shared;
using UsersDomain.Shared.Entities;
using UsersDomain.Shared.Exceptions;
using UsersDomain.Shared.Repositories;

namespace AuthenOpenAPIs.UnitTest.Services.UserServiceTests
{
    public class ShouldNot
    {
        [Theory]
        [AutoFakeItEasy]
        public async Task ChangePasswordAsync_When_Email_NotFound(
            [Frozen] IUserRepository userRepository, 
            UserService sut,
            string email, string oldPassword, 
            CancellationToken cancellationToken)
        {
            string newPassword = oldPassword + "_new";
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns((User?)null);
            Func<Task> func = async () => await sut.ChangePasswordAsync(email, oldPassword, newPassword, cancellationToken);
            await func.Should().ThrowAsync<UserException>().WithMessage("email not found");
            A.CallTo(() => userRepository.UpdateAsync(A<User>.Ignored, cancellationToken))
               .MustNotHaveHappened();
        }

        [Theory]
        [AutoFakeItEasy]
        public async Task ChangePasswordAsync_When_Old_Password_Is_Wrong(
            [Frozen] IUserRepository userRepository,
            UserService sut,
            string email, string oldPassword,
            CancellationToken cancellationToken)
        {
            string newPassword = oldPassword + "_new";
            User user = new User(email, oldPassword+"1");
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns(user);
            bool ret = await sut.ChangePasswordAsync(email, oldPassword, newPassword, cancellationToken);
            ret.Should().BeFalse();
            A.CallTo(() => userRepository.UpdateAsync(A<User>.Ignored, cancellationToken))
               .MustNotHaveHappened();
        }

        [Theory]
        [AutoFakeItEasy]
        public async Task CreateUserAsync_When_Email_Exists(
            [Frozen] IUserRepository userRepository,
            UserService sut,
            string email, string password,
            CancellationToken cancellationToken)
        {
            User user = new User(email, password);
            A.CallTo(() => userRepository.GetByEmailAsync(email, cancellationToken))
                .Returns(user);
            Func<Task<Guid>> func = async() => await sut.CreateUserAsync(email, password, cancellationToken);
            await func.Should().ThrowAsync<UserException>().WithMessage("email already exists");
            A.CallTo(() => userRepository.InsertAsync(A<User>.Ignored, cancellationToken))
               .MustNotHaveHappened();
        }
    }
}
