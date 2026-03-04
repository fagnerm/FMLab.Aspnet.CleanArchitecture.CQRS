// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Gateways;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.ExternalServices;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Context;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Gateways;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace FMLab.Aspnet.CleanArchitecture.Infrastructure.DependencyInjection;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, IHostEnvironment environment)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connection = new NpgsqlConnectionStringBuilder()
            {
                Host = config["Database:Server"],
                Port = int.Parse(config["Database:Port"]),
                Database = config["Database:Name"],
                Username = config["Database:User"],
                Password = config["Database:Password"]
            };

            options.UseNpgsql(connection.ConnectionString)
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging()
                       .LogTo(Console.WriteLine);
            }
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserGateway, UserGateway>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
