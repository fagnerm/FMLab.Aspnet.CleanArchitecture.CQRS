// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FluentValidation;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.CreateUser;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Mediator.Pipeline;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FMLab.Aspnet.CleanArchitecture.Application.DependencyInjection;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationModule).Assembly);

            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        });

        services.AddScoped<IValidator<CreateUserCommand>, CreateUserValidator>();

        return services;
    }
}
