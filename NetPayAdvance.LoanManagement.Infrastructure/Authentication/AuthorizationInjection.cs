using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace NetPayAdvance.LoanManagement.Infrastructure.Authentication;

public static class AuthorizationInjection
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services
            .AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(Schemes.TokenUser, Schemes.CookieUser, Schemes.Paydini,
                        Schemes.ApiKey)
                    .Build();

                options.AddPolicy(Policies.User, policy =>
                {
                    policy.AuthenticationSchemes.Add(Schemes.TokenUser);
                    policy.AuthenticationSchemes.Add(Schemes.CookieUser);
                    policy.RequireRole("client", "application");
                });

                options.AddPolicy(Policies.Admin, policy =>
                {
                    policy.AuthenticationSchemes.Add(Schemes.Paydini);
                    policy.RequireRole("administrator");
                });

                options.AddPolicy(Policies.Management, policy =>
                {
                    policy.AuthenticationSchemes.Add(Schemes.ApiKey);
                    policy.RequireRole("management");
                });
            });
        return services;
    }
}