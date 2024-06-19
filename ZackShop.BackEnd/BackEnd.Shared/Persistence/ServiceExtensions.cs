using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BackEnd.Shared.Persistence
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDb<TDbContext>(this IServiceCollection services, IConfiguration configuration, string connectionStringName = "Default") where TDbContext : DbContext
        {
            string? connectionString = configuration.GetConnectionString(connectionStringName);
            if(string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException($"Connection string not found, please set the {connectionStringName} section of ConnectionStrings");
            }
            services.AddDbContext<TDbContext>(options => options.UseNpgsql(connectionString));
            return services;
        }

       
    }
}