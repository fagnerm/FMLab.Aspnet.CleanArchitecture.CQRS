// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Infrastructure;

internal static class DbContextFactory
{
    /// <summary>
    /// Creates an isolated InMemory context.
    /// Each call with the same name shares the same in-memory database.
    /// Use a unique name (e.g. Guid.NewGuid().ToString()) for test isolation.
    /// </summary>
    public static ApplicationDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
