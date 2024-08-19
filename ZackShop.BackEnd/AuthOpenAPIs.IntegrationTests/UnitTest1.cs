using AuthOpenAPIs.Settings;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
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
            using var mockServerCrm = WireMockServer.Start();
            mockServerCrm.Given(Request.Create().WithPath("/api/users").UsingPost())
                .RespondWith(Response.Create().WithStatusCode(200));
            mockServerCrm.Given(Request.Create().WithPath("/api/users").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(new string[] { 
                Guid.NewGuid()+"@test.com", Guid.NewGuid()+"@test.com"
                }));
            //arrange test data
            var zackCRMSettings = _webApplicationFactory.Services.GetRequiredService<IOptions<ZackCRMSettings>>();
            zackCRMSettings.Value.BaseUrl = mockServerCrm.Urls[0];
            var client = _webApplicationFactory.CreateClient();
            var s = await client.PostAsync("/api/User/SyncWithZackCrm",null);
            Assert.NotNull(s);
        }
    }
}