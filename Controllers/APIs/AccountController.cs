using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreApi.Models;
using CoreApi.Security;
using CoreApi.Security.AuthorizationPolicies.RolePolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.Controllers.APIs
{
    
    public class AccountController : BaseController
    {
        [Route("")]
        public DataResponse Get()
        {
            
            return new DataResponse()
            {
                StatusCode = 200,
                Data = "Xin chào bạn"
            };
        }

        [Route("minrole")]
        [Authorize(Policy = RoleMinimumPolicy.Developer)]
        public DataResponse MinRole()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userEmail = User.FindFirst(ClaimTypes.Name).Value;

            return new DataResponse()
            {
                StatusCode = 200,
                Data = "Xin chào bạn"
            };
        }
    }
}
