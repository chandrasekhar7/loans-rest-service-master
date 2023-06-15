using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Builder;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Infrastructure.Authentication;
using NetPayAdvance.LoanManagement.Infrastructure.Contracts.Services;
using NetPayAdvance.LoanManagement.Infrastructure.ErrorHandling;
using System.Net.Http.Headers;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Infrastructure.Accounting;
using NetPayAdvance.LoanManagement.Infrastructure.Users;

namespace NetPayAdvance.LoanManagement.Infrastructure
{
    public static class DIExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IBaseContractService, BaseContractService>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("Contracts").GetSection("BaseAddress").Value);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", configuration.GetSection("Contracts").GetSection("BearerToken").Value);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            
            services.AddHttpClient<ITransactionService, TransactionService>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("Payments").GetSection("BaseAddress").Value);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", configuration.GetSection("Payments").GetSection("BearerToken").Value);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            
            services.AddScoped<ISkipPaymentService, SkipPaymentService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IUserService, UserService>();

            services.AddCustomAuthentication(configuration);
            services.AddCustomAuthorization();

            return services;
        }
        
        public static void UseCustomErrors(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomErrorHandlingMiddleware>();
        }
    }
    
    
}
