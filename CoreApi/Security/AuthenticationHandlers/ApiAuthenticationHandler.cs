using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CoreApi.Data.Repositories;
using CoreApi.Helpers.Contants;
using CoreApi.Models;
using CoreApi.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace CoreApi.Security.AuthenticationHandlers
{
    public class ApiAuthenticationHandler : AuthenticationHandler<ApiAuthenticationOptions>
    {
        private readonly IUserRepository _userRepository;

        public ApiAuthenticationHandler(
            IOptionsMonitor<ApiAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, IUserRepository userRepository) : base(options, logger, encoder, clock)
        {
            _userRepository = userRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            StringValues headerTokenValue;

            // Get Token
            if (!Request.Headers.TryGetValue(Options.HeaderName, out headerTokenValue)) return AuthenticateResult.NoResult();

            if (string.IsNullOrEmpty(headerTokenValue)) return AuthenticateResult.Fail("Empty token value");

            if (!headerTokenValue.ToString().StartsWith($"{Options.AuthorizationScheme} ", StringComparison.OrdinalIgnoreCase)) return AuthenticateResult.Fail("Wrong format token value");

            // Get Token Value
            string tokenValue = headerTokenValue.ToString().Remove(0, Options.AuthorizationScheme.Length + 1);

            // Check
            if (string.IsNullOrEmpty(tokenValue)) return AuthenticateResult.Fail("Empty token value");

            // Decrypt Token
            var tokenDecrypt = CryptoUtils.AES.DecryptTwoLevel(tokenValue);
            if (string.IsNullOrEmpty(tokenDecrypt)) return AuthenticateResult.Fail("Wrong token");

            try
            {
                var claimsObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenDecrypt);

                // Setup new Identity
                var identity = new ClaimsIdentity(SchemeConstants.Api);

                // Add claims to Identity
                foreach (var claim in claimsObject)
                {
                    identity.AddClaim(new Claim(claim.Key, claim.Value));
                }

                // Check User Status
                var user = await _userRepository.FindByIdAsync(claimsObject[ClaimTypes.NameIdentifier]);
                if (user == null) return AuthenticateResult.Fail($"Cant find user with Id: {claimsObject[ClaimTypes.NameIdentifier]}");

                // Check lockout
                if (user.LockoutStart.HasValue && user.LockoutStart.Value <= DateTimeOffset.UtcNow)
                    if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                        return AuthenticateResult.Fail($"UserId: {claimsObject[ClaimTypes.NameIdentifier]} have been lockout started.");

                // Check Session
                var session = user.UserDevices.FirstOrDefault(x => x.DeviceId.Equals(claimsObject[ClaimContants.DeviceId]));
                if (!(!string.IsNullOrEmpty(session?.Token) && session.Token.Equals(claimsObject[ClaimContants.SessionToken])))
                    return AuthenticateResult.Fail("Session expired");

                // Create new ticket
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), new AuthenticationProperties(), SchemeConstants.Api);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail("Cant get token value: " + e.Message);
            }
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            // Handle request outside API
            if (!Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                await Context.ForbidAsync(IdentityConstants.ApplicationScheme);
                return;
            }

            // Handle forbidden request api
            var respData = new DataResponse()
            {
                StatusCode = 403,
                Message = "You dont have permission to see this"
            };

            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Response.ContentType = ResponseContentType.JsonContent;
            await Response.WriteAsync(JsonConvert.SerializeObject(respData));
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // Handle request outside API
            if (!Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                await Context.ChallengeAsync(IdentityConstants.ApplicationScheme);
                return;
            }

            // Handle not authorized API
            var respData = new DataResponse()
            {
                StatusCode = 401,
                Message = "Access Denied"
            };

            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            Response.ContentType = ResponseContentType.JsonContent;
            await Response.WriteAsync(JsonConvert.SerializeObject(respData));
        }
    }
}
