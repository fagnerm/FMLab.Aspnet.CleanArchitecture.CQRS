// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Api.Handlers;

namespace FMLab.Aspnet.CleanArchitecture.Api.Configurations;

public static class ProblemDetails
{
    public static IServiceCollection AddAppProblemDetails(this IServiceCollection services)
    {
        services.AddExceptionHandler<GenericException>();
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceID"] = context.HttpContext.TraceIdentifier;

                if (context.Exception is Exception ex)
                {
                    context.ProblemDetails.Status = 500;
                    context.ProblemDetails.Title = "Internal Server Error";
                    context.ProblemDetails.Detail = ex.Message;
                }
            };
        });

        return services;
    }

    public static WebApplication UseAppProblemDetails(this WebApplication app)
    {
        app.UseExceptionHandler(options =>
        {
            options.Run(async context =>
            {
                await Results.Problem()
                             .ExecuteAsync(context);
            });
        });
        app.UseStatusCodePages();

        return app;
    }
}
