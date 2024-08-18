using UsersDomain.Shared;
using BackEnd.Shared;
using RestClients.Shared.ZackCRM;
using AuthOpenAPIs.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UsersDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.RegisterAllServices(typeof(UsersDbContext).Assembly);

builder.Services.AddHttpClient<IZackCRMClient, ZackCRMClient>((sp, httpClient) => {
    var config = sp.GetRequiredService<ZackCRMSettings>();
    httpClient.BaseAddress = new Uri(config.BaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }