// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Api.Endpoints.Users;


namespace FMLab.Aspnet.CleanArchitecture.Api.Configurations;

public static class AppEndpoints
{
    public static WebApplication UseApplicationEndpoints(this WebApplication app)
    {
        UserEndpoints.MapUser(app);

        return app;
    }
}
