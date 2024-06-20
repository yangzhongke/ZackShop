using AuthOpenAPIs.Services;
using AutoFixture;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
