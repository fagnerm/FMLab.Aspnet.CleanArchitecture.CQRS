// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Context;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Infrastructure.Persistence.Context;

public class UnitOfWorkTests
{
    [Fact]
    public async Task CommitAsync_WhenCalled_PersistsPendingChanges()
    {
        var dbName = Guid.NewGuid().ToString();
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));

        await using (var context = DbContextFactory.Create(dbName))
        {
            await context.Users.AddAsync(user);
            var unitOfWork = new UnitOfWork(context);
            await unitOfWork.CommitTransactionAsync(CancellationToken.None);
        }

        await using (var context = DbContextFactory.Create(dbName))
        {
            var saved = await context.Users.FindAsync(user.Id);
            Assert.NotNull(saved);
            Assert.Equal("Fagner", saved.Name.Value);
        }
    }

    [Fact]
    public async Task CommitAsync_WithNoChanges_CompletesWithoutError()
    {
        await using var context = DbContextFactory.Create();
        var unitOfWork = new UnitOfWork(context);

        var exception = await Record.ExceptionAsync(
            () => unitOfWork.CommitTransactionAsync(CancellationToken.None));

        Assert.Null(exception);
    }

    [Fact]
    public async Task CommitAsync_WithMultipleEntities_PersistsAll()
    {
        var dbName = Guid.NewGuid().ToString();
        var user1 = new User(new Name("Fagner"), new Email("fagner@example.com"));
        var user2 = new User(new Name("John"), new Email("john@example.com"));

        await using (var context = DbContextFactory.Create(dbName))
        {
            await context.Users.AddRangeAsync(user1, user2);
            var unitOfWork = new UnitOfWork(context);
            await unitOfWork.CommitTransactionAsync(CancellationToken.None);
        }

        await using (var context = DbContextFactory.Create(dbName))
        {
            Assert.Equal(2, context.Users.Count());
        }
    }
}
