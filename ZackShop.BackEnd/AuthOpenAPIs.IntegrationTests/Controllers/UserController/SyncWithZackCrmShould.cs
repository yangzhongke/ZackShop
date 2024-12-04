using AuthOpenAPIs.IntegrationTests.Helpers;
using AuthOpenAPIs.Settings;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UsersDomain.Shared;
using UsersDomain.Shared.Entities;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace AuthOpenAPIs.IntegrationTests.Controllers.UserController;

public class SyncWithZackCrmShould : IClassFixture<WebApplicationFactory<Program>>
{
    //1. Install-Package Microsoft.AspNetCore.Mvc.Testing
    //2. add <InternalsVisibleTo Include="AuthOpenAPIs.IntegrationTests" /> to tested project
    //3. add public partial class Program { } to the end of Program.cs in tested project
    //https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly DataBaseHelper _dataBaseHelper;
    public SyncWithZackCrmShould(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
        //_webApplicationFactory.Services.GetRequiredService<UsersDbContext>() cannot be used here with creation of scope.
        var scope = _webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        this._dataBaseHelper = new DataBaseHelper(dbContext);
    }

    [Fact]
    public async Task Sync_Correctly_WhenNoLocalUsers()
    {
        //arrange mock server
        this._dataBaseHelper.ClearUsers();

        string newEmailFromCrm1 = Guid.NewGuid() + "@qq.com";
        string newEmailFromCrm2 = Guid.NewGuid() + "@qq.com";
        using var mockServerCrm = WireMockServer.Start();
        mockServerCrm.Given(Request.Create().WithPath("/api/users").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(new string[] {
            newEmailFromCrm1, newEmailFromCrm2
            }));
        //arrange test data
        var zackCRMSettings = _webApplicationFactory.Services.GetRequiredService<IOptions<ZackCRMSettings>>();
        zackCRMSettings.Value.BaseUrl = mockServerCrm.Urls[0];//!!!
        var client = _webApplicationFactory.CreateClient();
        //act
        var response = await client.PostAsync("/api/User/SyncWithZackCrm", null);
        //assert
        response.IsSuccessStatusCode.Should().BeTrue();
        _dataBaseHelper.GetUserByEmail(newEmailFromCrm1).Should().NotBeNull();
        _dataBaseHelper.GetUserByEmail(newEmailFromCrm2).Should().NotBeNull();
        _dataBaseHelper.GetAllUsers().Should().HaveCount(2);
    }
    
    [Fact]
    public async Task Sync_Correctly_WhenNoRemoteUsers()
    {
        //arrange mock server
        this._dataBaseHelper.ClearUsers();
        string newEmailFromDB1 = Guid.NewGuid() + "@test.com";
        string newEmailFromDB2 = Guid.NewGuid() + "@test.com";
        _dataBaseHelper.AddUser(new User(newEmailFromDB1, "123"));
        _dataBaseHelper.AddUser(new User(newEmailFromDB2, "123"));
        
        using var mockServerCrm = WireMockServer.Start();
        mockServerCrm.Given(Request.Create().WithPath("/api/users").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200));
        mockServerCrm.Given(Request.Create().WithPath("/api/users").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(Array.Empty<string>()));
        //arrange test data
        var zackCRMSettings = _webApplicationFactory.Services.GetRequiredService<IOptions<ZackCRMSettings>>();
        zackCRMSettings.Value.BaseUrl = mockServerCrm.Urls[0];
        var client = _webApplicationFactory.CreateClient();
        //act
        var response = await client.PostAsync("/api/User/SyncWithZackCrm", null);
        //assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        mockServerCrm.LogEntries.Should().Contain(e => e.RequestMessage.Path == "/api/users" && e.RequestMessage.Method == "POST"
            &&e.RequestMessage.Body.Contains(newEmailFromDB1)&&e.ResponseMessage.StatusCode.Equals(200));
        mockServerCrm.LogEntries.Should().Contain(e => e.RequestMessage.Path == "/api/users" && e.RequestMessage.Method == "POST"
            &&e.RequestMessage.Body.Contains(newEmailFromDB2)&&e.ResponseMessage.StatusCode.Equals(200));
        
    }
}