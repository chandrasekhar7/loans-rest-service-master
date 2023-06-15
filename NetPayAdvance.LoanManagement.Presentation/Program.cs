using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using NetPayAdvance.LoanManagement.Application;
using NetPayAdvance.LoanManagement.Infrastructure;
using NetPayAdvance.LoanManagement.Persistence;
using NetPayAdvance.LoanManagement.Presentation.Constraints;
using NetPayAdvance.LoanManagement.Presentation.Converters;
using NetPayAdvance.LoanManagement.Presentation.Extensions;
using NetPayAdvance.LoanManagement.Presentation.Swagger;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDistributedCache, MemoryDistributedCache>();

builder.Host.UseSerilog((ctx, ls) => ls.ReadFrom.Configuration(builder.Configuration)
    //.WriteTo.Console()
    .WriteTo.MSSqlServer(builder.Configuration.GetConnectionString("Logging"), new MSSqlServerSinkOptions()
    {
        AutoCreateSqlTable = true,
        TableName = "ErrorLogs"
    }, restrictedToMinimumLevel: LogEventLevel.Error)
);

builder.Services.AddControllers(o =>
    {
        o.UseGeneralRoutePrefix("api/loan-management/v{version:apiVersion}");
        o.UseDateOnlyTimeOnlyStringConverters();
    })
    .AddFluentValidation(c =>
    {
        c.RegisterValidatorsFromAssemblyContaining<ApplicationLayerException>();
        // Optionally set validator factory if you have problems with scope resolve inside validators.
        c.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
    }).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
    });

builder.Services.Configure<RouteOptions>(o => { o.ConstraintMap.Add("StatementId", typeof(StatementIdConstraint)); });

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
}).AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

builder.Services.AddOptions();
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddApplication(true, true);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging() || builder.Environment.IsEnvironment("QA"))
{
    builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
    builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();
    builder.Services.ConfigureOptions<ConfigureSwaggerUIOptions>();
    builder.Services.AddSwaggerGen();
    builder.Services.AddFluentValidationRulesToSwagger();
}

var app = builder.Build();

if (!(app.Environment.IsProduction() || app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsEnvironment("QA")))
{
    throw new Exception("Invalid Environment: Use Development, Staging, QA, or Production");
}

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCustomErrors();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireAuthorization(); });

app.Run();