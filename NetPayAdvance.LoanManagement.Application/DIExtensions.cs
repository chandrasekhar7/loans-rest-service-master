using FastExpressionCompiler;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetPayAdvance.LoanManagement.Application.Behaviors;
using FluentValidation;
using Mapster;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Mapster;
using NetPayAdvance.LoanManagement.Application.Services;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Events;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application
{
    public static class DIExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services , bool addValidation = false, bool addRequestLogging = false)
        {
            services.Scan(scan =>
                scan
                   .FromCallingAssembly()
                   .AddClasses(classes => classes.AssignableTo(typeof(IDomainEntityToViewModelMapper<,>))).AsImplementedInterfaces().WithSingletonLifetime()
            );
            
            if (addValidation)
            {
                services.AddValidatorsFromAssemblyContaining<ApplicationLayerException>();
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            }
            
            services.AddSingleton<IModelMapper, ModelMapper>();

            services.AddScoped<IHistoryNotesService, HistoryNotesService>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IChargebackService, ChargebackService>();
            TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileFast();

            return services;
        }
    }
}
