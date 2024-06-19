using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BackEnd.Shared
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterAllServices(this IServiceCollection services, Assembly assembly)
        {
            var serviceInterfaceTypes = assembly.GetExportedTypes()
                .Where(x => x.IsInterface && x.IsAssignableTo(typeof(IService)));
            foreach (var interfaceType in serviceInterfaceTypes)
            {
                var implmentationTypes =assembly.GetExportedTypes().Where(x => x.IsClass && !x.IsAbstract && x.IsAssignableTo(interfaceType));
                foreach (var implementationType in implmentationTypes)
                {
                    services.AddScoped(interfaceType, implementationType);
                }
            }
            return services;
        }
    }
}
