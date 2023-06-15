using System;
using Microsoft.AspNetCore.Authentication;

namespace NetPayAdvance.LoanManagement.Infrastructure.Authentication;

public class AuthBearerTokenOptions : AuthenticationSchemeOptions
{
    public string TokenHeaderName { get; set; } = "Authorization";
    public string Load { get; set; } = String.Empty;
    public AuthBearerToken[] Credentials { get; set; }
    public string Scheme => Schemes.ApiKey;
    public string AuthenticationType = Schemes.ApiKey;

}