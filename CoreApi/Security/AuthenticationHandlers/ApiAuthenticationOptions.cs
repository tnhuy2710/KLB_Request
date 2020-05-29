using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace CoreApi.Security.AuthenticationHandlers
{
    public class ApiAuthenticationOptions : AuthenticationSchemeOptions
    {
        private const string DefaultHeaderName = "Authorization";
        private const string DefaultAuthorizationScheme = "Bearer";

        public string HeaderName { get; set; } = DefaultHeaderName;
        public string AuthorizationScheme { get; set; } = DefaultAuthorizationScheme;
    }
}
