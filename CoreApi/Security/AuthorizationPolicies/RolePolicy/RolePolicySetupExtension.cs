using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace CoreApi.Security.AuthorizationPolicies.RolePolicy
{
    public static class RolePolicySetupExtension
    {
        /// <summary>
        /// Setup Minimum Role Policy Authorization.
        /// </summary>
        /// <param name="service"></param>
        public static void SetupMinimunRolePolicy(this IServiceCollection service)
        {
            // Add Policy
            service.AddAuthorization(options =>
            {
                options.AddPolicy(RoleMinimumPolicy.Register, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new RolePolicyRequirement(UserRole.Registered));
                });

                options.AddPolicy(RoleMinimumPolicy.Supporter, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new RolePolicyRequirement(UserRole.Supporter));
                });

                options.AddPolicy(RoleMinimumPolicy.Developer, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new RolePolicyRequirement(UserRole.Developer));
                });

                options.AddPolicy(RoleMinimumPolicy.Moderator, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new RolePolicyRequirement(UserRole.Moderator));
                });

                options.AddPolicy(RoleMinimumPolicy.Administrator, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new RolePolicyRequirement(UserRole.Administrator));
                });
            });

            // Handler custom Policy
            service.AddScoped<IAuthorizationHandler, RolePolicyHandler>();
        }
    }
}
