// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Handlers.ListUsers;
using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.Enums;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Gateways;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Infrastructure.Persistence.Gateways;

public class UserGatewayTests
{
    private static async Task<string> SeedAsync(params User[] users)
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = DbContextFactory.Create(dbName);
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        return dbName;
    }

    [Fact]
    public async Task ListAsync_WithNoStatusFilter_ReturnsAllUsers()
    {
        var active = new User(new Name("Fagner"), new Email("fagner@example.com"));
        var deactivated = new User(new Name("John"), new Email("john@example.com"));
        deactivated.Deactivate();
        var dbName = await SeedAsync(active, deactivated);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);
        var filter = new ListUsersFilter(null, 1, 20);

        var result = await gateway.ListAsync(filter, CancellationToken.None);

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task ListAsync_WithActiveFilter_ReturnsOnlyActiveUsers()
    {
        var active = new User(new Name("Fagner"), new Email("fagner@example.com"));
        var deactivated = new User(new Name("John"), new Email("john@example.com"));
        deactivated.Deactivate();
        var dbName = await SeedAsync(active, deactivated);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);
        var filter = new ListUsersFilter(UserStatus.Active, 1, 20);

        var result = await gateway.ListAsync(filter, CancellationToken.None);

        Assert.Equal(1, result.TotalItems);
        Assert.All(result.Items, item => Assert.Equal("Active", item.Status));
    }

    [Fact]
    public async Task ListAsync_WithDeactivatedFilter_ReturnsOnlyDeactivatedUsers()
    {
        var active = new User(new Name("Fagner"), new Email("fagner@example.com"));
        var deactivated = new User(new Name("John"), new Email("john@example.com"));
        deactivated.Deactivate();
        var dbName = await SeedAsync(active, deactivated);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);
        var filter = new ListUsersFilter(UserStatus.Deactivated, 1, 20);

        var result = await gateway.ListAsync(filter, CancellationToken.None);

        Assert.Equal(1, result.TotalItems);
        Assert.All(result.Items, item => Assert.Equal("Deactivated", item.Status));
    }

    [Fact]
    public async Task ListAsync_ReturnsCorrectPaginationMetadata()
    {
        var names = new string[] { "Fagner", "John", "Jane", "Joseph", "Janice" };
        var users = Enumerable.Range(0, 5)
            .Select(i => new User(new Name($"{names[i]}"), new Email($"user{names[i]}@example.com")))
            .ToArray();
        var dbName = await SeedAsync(users);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);
        var filter = new ListUsersFilter(null, page: 1, pageSize: 2);

        var result = await gateway.ListAsync(filter, CancellationToken.None);

        Assert.Equal(5, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public async Task ListAsync_SecondPage_ReturnsNextItems()
    {
        var names = new string[] { "Fagner", "John", "Jane", "Joseph", "Janice" };
        var users = Enumerable.Range(0, 5)
            .Select(i => new User(new Name($"{names[i]}"), new Email($"user{names[i]}@example.com")))
            .ToArray();
        var dbName = await SeedAsync(users);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);
        var filter = new ListUsersFilter(null, page: 2, pageSize: 2);

        var result = await gateway.ListAsync(filter, CancellationToken.None);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.Page);
    }

    [Fact]
    public async Task ListAsync_WhenEmpty_ReturnsTotalItemsZero()
    {
        await using var context = DbContextFactory.Create();
        var gateway = new UserGateway(context);
        var filter = new ListUsersFilter(null, 1, 20);

        var result = await gateway.ListAsync(filter, CancellationToken.None);

        Assert.Equal(0, result.TotalItems);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task ListUserByIdAsync_WhenUserExists_ReturnsCorrectDto()
    {
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
        var dbName = await SeedAsync(user);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);

        var result = await gateway.ListUserByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("Fagner", result.Name);
        Assert.Equal("fagner@example.com", result.Email);
        Assert.Equal("Active", result.Status);
    }

    [Fact]
    public async Task ListUserByIdAsync_WhenUserHasNoEmail_ReturnsNullEmail()
    {
        var user = new User(new Name("Fagner"), null);
        var dbName = await SeedAsync(user);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);

        var result = await gateway.ListUserByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Null(result.Email);
    }

    [Fact]
    public async Task ListUserByIdAsync_WhenUserNotFound_ReturnsNull()
    {
        await using var context = DbContextFactory.Create();
        var gateway = new UserGateway(context);

        var result = await gateway.ListUserByIdAsync(999, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsByKeyAsync_WhenNameMatches_ReturnsTrue()
    {
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
        var dbName = await SeedAsync(user);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);

        var exists = await gateway.ExistsByKeyAsync("Fagner", null, CancellationToken.None);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsByKeyAsync_WhenEmailMatches_ReturnsTrue()
    {
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
        var dbName = await SeedAsync(user);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);

        var exists = await gateway.ExistsByKeyAsync(null, "fagner@example.com", CancellationToken.None);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsByKeyAsync_WhenNeitherMatches_ReturnsFalse()
    {
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
        var dbName = await SeedAsync(user);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);

        var exists = await gateway.ExistsByKeyAsync("Unknown", "unknown@example.com", CancellationToken.None);

        Assert.False(exists);
    }

    [Fact]
    public async Task ExistsByKeyAsync_WhenBothParametersNull_ReturnsFalse()
    {
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
        var dbName = await SeedAsync(user);

        await using var context = DbContextFactory.Create(dbName);
        var gateway = new UserGateway(context);

        var exists = await gateway.ExistsByKeyAsync(null, null, CancellationToken.None);

        Assert.False(exists);
    }
}
