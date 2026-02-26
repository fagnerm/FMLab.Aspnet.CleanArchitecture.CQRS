// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Api.Configurations;
using FMLab.Aspnet.CleanArchitecture.Application.DependencyInjection;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Environment);
builder.Services.AddApplication();
builder.Services.AddAppProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAppSwagger();

var app = builder.Build();

app.UseAppSwagger();
app.UseApplicationEndpoints();
app.UseAppProblemDetails();

app.Run();

public partial class Program { }