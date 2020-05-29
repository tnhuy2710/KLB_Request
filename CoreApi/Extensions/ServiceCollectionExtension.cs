using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreApi.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CoreApi.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Auto find and register Service to Interface
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {

            // Get Data Folder Namespace
            var dataNamespace = typeof(ApplicationDbContext);

            // Get all type in this project have namespace start with dataNamespace
            var types = Assembly.GetEntryAssembly().GetTypes().Where(x => !string.IsNullOrEmpty(x?.Namespace) && x.Namespace.StartsWith(dataNamespace.Namespace)).ToArray();

            // Get all interface have name endwith "Repository"
            var repositoryInterfaces = types.Where(x => x.GetTypeInfo().IsInterface && x.Name.EndsWith("Repository")).ToArray();

            // Loop all interface found
            foreach (var interfaceType in repositoryInterfaces)
            {
                // Find all class inheritance from that interface
                foreach (var classType in types.Where(x => interfaceType != x && interfaceType.IsAssignableFrom(x) && !x.IsInterface))
                {
                    // Register dependency injection service this class to interface
                    services.AddScoped(interfaceType, classType);
                }

            }

            return services;
        }
    }
}
