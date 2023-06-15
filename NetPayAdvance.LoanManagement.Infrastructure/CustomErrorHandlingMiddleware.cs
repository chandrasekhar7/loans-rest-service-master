using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetPayAdvance.LoanManagement.Application;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace NetPayAdvance.LoanManagement.Infrastructure.ErrorHandling;

public class CustomErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;
    private static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
    {
        Converters = new List<JsonConverter>()
        {
            new StringEnumConverter()
        },
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public CustomErrorHandlingMiddleware(RequestDelegate next, ILogger<ApplicationLayer> logger)
    {
        this.logger = logger;
        this.next = next;
    }

    public async Task Invoke(HttpContext context, IWebHostEnvironment env)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, env);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex, IWebHostEnvironment env)
    {
        var code = HttpStatusCode.InternalServerError; // 500 if unexpected
        var level = LogLevel.None;
        
        if (ex is AccessDeniedException)
        {
            level = LogLevel.Warning;
            code = HttpStatusCode.Forbidden;
        }

        if (ex is AuthorizationException)
        {
            level = LogLevel.Warning;
            code = HttpStatusCode.Unauthorized;
        }
        else if (ex is NotFoundException || ex is NullReferenceException)
        {
            level = LogLevel.Information;
            code = HttpStatusCode.NotFound;
        }
        else if (ex is ValidationException || ex is InvalidOperationException)
        {
            level = LogLevel.Information;
            code = HttpStatusCode.BadRequest;
        }
        else if (ex is ConflictException)
        {
            level = LogLevel.Error;
            code = HttpStatusCode.Conflict;
        }
        else if (ex is DataLayerException)
        {
            level = LogLevel.Warning;
            code = HttpStatusCode.BadRequest;
        }
        else if (ex is ApplicationLayerException)
        {
            level = LogLevel.Warning;
            code = HttpStatusCode.BadRequest;
        }
        else if (ex is UnprocessableException)
        {
            level = LogLevel.Error;
            code = HttpStatusCode.UnprocessableEntity;
        }
        else
        {
            // Code stays at 500 because we dont know how to handle it
            level = LogLevel.Error;
        }

        var includeDetails = env.IsDevelopment() || env.IsStaging() || env.IsEnvironment("Azure-Staging");
        var details = includeDetails ? ex.ToString() : null;

        var problem = new ProblemDetails
        {
            Status = (int?) code,
            Title = ex.Message,
            Detail = details
        };
        logger.Log(level, ex, ex.Message);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int) code;

        var result = JsonConvert.SerializeObject(problem, serializerSettings);
        return context.Response.WriteAsync(result);
    }
}