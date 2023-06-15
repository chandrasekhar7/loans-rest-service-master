using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NetPayAdvance.LoanManagement.Infrastructure.Authentication
{
    internal class AuthBearerTokenHandler : AuthenticationHandler<AuthBearerTokenOptions>
    {
        private readonly AuthBearerToken[] tokens;
        public AuthBearerTokenHandler(IOptionsMonitor<AuthBearerTokenOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, IOptions<AuthBearerTokenOptions> o) : base(options, logger, encoder, clock)
        {
            tokens = o.Value.Credentials;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(Options.TokenHeaderName))
            {
                return Task.FromResult(AuthenticateResult.Fail($"Missing Header For Token: {Options.TokenHeaderName}"));
            }

            var token = Request.Headers[Options.TokenHeaderName].ToString();
            var key = token.Substring(7);
            AuthBearerToken credentials = tokens?.FirstOrDefault(c => c.Key == key);
            if (credentials != null)
            {
               // var username = "FLX";
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, credentials.Name),
                    // add other claims/roles as you like
                };

                foreach (var role in credentials.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var id = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(id);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult((AuthenticateResult.Success(ticket)));
            }

            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}