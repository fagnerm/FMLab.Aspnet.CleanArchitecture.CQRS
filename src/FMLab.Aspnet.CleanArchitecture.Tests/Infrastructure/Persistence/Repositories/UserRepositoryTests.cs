// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Repositories;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Infrastructure.Persistence.Repositories;

public class UserRepositoryTests
{
    // ── AddAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_WhenCalled_PersistsUserToDatabase()
    {
        var dbName = Guid.NewGuid().ToString();
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));

        await using (var context = DbContextFactory.Create(dbName))
        {
            var repository = new UserRepository(context);
            await repository.AddAsync(user, CancellationToken.None);
            await context.SaveChangesAsync();
        }

        await using (var context = DbContextFactory.Create(dbName))
        {
            var saved = await context.Users.FindAsync(user.Id);
            Assert.NotNull(saved);
            Assert.Equal("Fagner", saved.Name.Value);
            Assert.Equal("fagner@example.com", saved.Email!.Value);
        }
    }

    [Fact]
    public async Task AddAsync_AssignsGeneratedId()
    {
        var dbName = Guid.NewGuid().ToString();
        var user = new User(new Name("Fagner"), null);

        await using var context = DbContextFactory.Create(dbName);
        var repository = new UserRepository(context);
        await repository.AddAsync(user, CancellationToken.None);
        await context.SaveChangesAsync();

        Assert.True(user.Id > 0);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
    {
        var dbName = Guid.NewGuid().ToString();
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));

        await using (var context = DbContextFactory.Create(dbName))
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        await using (var context = DbContextFactory.Create(dbName))
        {
            var repository = new UserRepository(context);
            var found = await repository.GetByIdAsync(user.Id, CancellationToken.None);

            Assert.NotNull(found);
            Assert.Equal(user.Id, found.Id);
            Assert.Equal("Fagner", found.Name.Value);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserNotFound_ReturnsNull()
    {
        await using var context = DbContextFactory.Create();
        var repository = new UserRepository(context);

        var found = await repository.GetByIdAsync(999, CancellationToken.None);

        Assert.Null(found);
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_WhenCalled_RemovesUserFromDatabase()
    {
        var dbName = Guid.NewGuid().ToString();
        var user = new User(new Name("Fagner"), null);

        await using (var context = DbContextFactory.Create(dbName))
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        await using (var context = DbContextFactory.Create(dbName))
        {
            var repository = new UserRepository(context);
            var found = await repository.GetByIdAsync(user.Id, CancellationToken.None);
            repository.Delete(found!);
            await context.SaveChangesAsync();
        }

        await using (var context = DbContextFactory.Create(dbName))
        {
            var deleted = await context.Users.FindAsync(user.Id);
            Assert.Null(deleted);
        }
    }

    // ── Update ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_WhenCalled_PersistsChanges()
    {
        var dbName = Guid.NewGuid().ToString();
        var user = new User(new Name("Fagner"), null);

        await using (var context = DbContextFactory.Create(dbName))
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        await using (var context = DbContextFactory.Create(dbName))
        {
            var repository = new UserRepository(context);
            var existing = await repository.GetByIdAsync(user.Id, CancellationToken.None);
            existing!.Update(new Name("John"), new Email("john@example.com"));
            repository.Update(existing);
            await context.SaveChangesAsync();
        }

        await using (var context = DbContextFactory.Create(dbName))
        {
            var updated = await context.Users.FindAsync(user.Id);
            Assert.Equal("John", updated!.Name.Value);
            Assert.Equal("john@example.com", updated.Email!.Value);
        }
    }

    [Fact]
    public async Task Update_ReturnsTheUpdatedUser()
    {
        var dbName = Guid.NewGuid().ToString();
        var user = new User(new Name("Fagner"), null);

        await using var context = DbContextFactory.Create(dbName);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);
        var returned = repository.Update(user);

        Assert.Same(user, returned);
    }
}
