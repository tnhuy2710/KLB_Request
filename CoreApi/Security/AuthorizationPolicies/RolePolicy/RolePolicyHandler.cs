using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreApi.Data.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace CoreApi.Security.AuthorizationPolicies.RolePolicy
{
    public class RolePolicyHandler : AuthorizationHandler<RolePolicyRequirement>
    {
        private readonly IRoleRepository _roleRepository;

        public RolePolicyHandler(IRoleRepository roleRepository)
        {
            this._roleRepository = roleRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RolePolicyRequirement requirement)
        {
            // Set always fail passed all requirement
            var isSuccess = false;

            // Check Has Claims
            var userIdClaim = context.User.FindFirst(claim => claim.Type.Equals(ClaimTypes.NameIdentifier));

            // Check UserId ClaimType is exists
            var userId = userIdClaim?.Value;
            if (string.IsNullOrEmpty(userId))
                goto end;

            var role = await _roleRepository.GetRoleOfUserAsync(userId);
            if (role != null)
            {
                // Check role level is greater than or not
                if (role.Level >= (int)requirement.UserRole)
                {
                    // Set Success
                    isSuccess = true;
                }
            }

            end:

            if (isSuccess)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
