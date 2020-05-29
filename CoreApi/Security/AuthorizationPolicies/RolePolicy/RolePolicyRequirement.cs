using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CoreApi.Security.AuthorizationPolicies.RolePolicy
{
    public class RolePolicyRequirement : IAuthorizationRequirement
    {
        internal UserRole UserRole { get; set; }

        public RolePolicyRequirement(UserRole role)
        {
            UserRole = role;
        }
    }
}
