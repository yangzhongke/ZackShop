using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace AuthOpenAPIs.IntegrationTests
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly ITestOutputHelper _output;

        //1. Install-Package Microsoft.AspNetCore.Mvc.Testing
        //2. add <InternalsVisibleTo Include="AuthOpenAPIs.IntegrationTests" /> to tested project
        //3. add public partial class Program { } to the end of Program.cs in tested project
        //https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
        private readonly WebApplicationFactory<Program> _webApplicationFactory;

        public UnitTest1(WebApplicationFactory<Program> webApplicationFactory,
            ITestOutputHelper output)
        {
            _webApplicationFactory = webApplicationFactory;
            _output = output;
        }

        [Fact]
        public async Task Test1()
        {
            var client = _webApplicationFactory.CreateClient();
            var s = await client.GetStringAsync("/api/User/SyncWithZackCrm");
            Assert.NotNull(s);
        }
    }
}