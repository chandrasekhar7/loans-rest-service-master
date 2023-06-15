using System;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;
using NetPayAdvance.LoanManagement.Persistence.Database;
using NetPayAdvance.LoanManagement.Persistence.Handlers;

namespace NetPayAdvance.LoanManagement.Persistence
{
    public static class DIExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ILoanContext,LoanContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging(true));

            //services.AddScoped<ILoanContext, LoanContext>(provider => provider.GetService<LoanContext>() ?? throw new Exception("Could not get database context for persistence layer"));
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddScoped<IStatementRepository, StatementRepository>();
            services.AddScoped<IAdjustmentRepository, AdjustmentRepository>();
            services.AddScoped<IAdjustmentAggregateRepository, AdjustmentAggregateRepository>();
            services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();
            services.AddScoped<IDbFacade, DbFacade>();

            SqlMapper.AddTypeHandler(new DateOnlyHandler());
            return services;
        }
    }
}
