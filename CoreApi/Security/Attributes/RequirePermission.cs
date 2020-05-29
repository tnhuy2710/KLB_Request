using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreApi.Data.Repositories;
using CoreApi.Extensions;
using CoreApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace CoreApi.Security.Attributes
{
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(string controllerName, RequirePermissionType action) : base(typeof(RequirePermissionActionFilter))
        {
            Arguments = new object[] { controllerName, action };
        }
    }
    

    public class RequirePermissionActionFilter : IAsyncActionFilter
    {
        private readonly string _controllerName;
        private readonly RequirePermissionType _action;
        
        public RequirePermissionActionFilter(string controllerName, RequirePermissionType action)
        {
            this._controllerName = controllerName;
            this._action = action;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool isAuthorized = context.HttpContext.User.Identity.IsAuthenticated;
            if (!isAuthorized)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get Service
            var userRepository = context.HttpContext.RequestServices.GetService<IUserRepository>();
            var roleRepository = context.HttpContext.RequestServices.GetService<IRoleRepository>();

            if (userRepository != null && roleRepository != null)
            {
                var userId = context.HttpContext.User.FindFirst(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    bool isAllow = false;
                    var permissionText = $"{_controllerName}_{_action.ToString()}".MakeLowerCase();

                    // Check Role Permission first
                    var roleId = await roleRepository.GetRoleIdByUserIdAsync(userId);
                    if (!string.IsNullOrEmpty(roleId))
                    {
                        var roleClaims = await roleRepository.GetPermissionByRoleIdAsync(roleId);

                        if (!string.IsNullOrEmpty(roleClaims))
                        {
                            if (roleClaims.Contains($"allow_{permissionText}"))
                                isAllow = true;
                            if (roleClaims.Contains($"deny_{permissionText}"))
                                isAllow = false;
                        }
                    }

                    // Next, check user permission
                    var userClaims = await userRepository.GetPermissionsByUserIdAsync(userId);
                    if (!string.IsNullOrEmpty(userClaims))
                    {
                        if (userClaims.Contains($"allow_{permissionText}"))
                            isAllow = true;
                        if (userClaims.Contains($"deny_{permissionText}"))
                            isAllow = false;
                    }

                    // Finally
                    if (isAllow)
                    {
                        // Success
                        await next();
                        return;
                    }
                }
            }

            Debug.WriteLine("Error cant validate user permission.");
            context.Result = new ForbidResult();
        }
    }
}
