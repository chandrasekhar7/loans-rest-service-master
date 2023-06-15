using System;

namespace NetPayAdvance.LoanManagement.Infrastructure.Authentication;

public class JwtOptions
{
    public string Secret { get; set; }
    public TimeSpan Expiration { get; set; }
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool RequireExpirationTime { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public string Issuer { get; set; }
    public string Audience { get; set; }
}