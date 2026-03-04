// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.DTOs;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.ListUsers;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Gateways;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Domain.Enums;
using NSubstitute;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Application.Handlers.ListUsers;

public class ListUsersHandlerTests
{
    private readonly IUserGateway _gateway = Substitute.For<IUserGateway>();
    private readonly ListUsersHandler _handler;

    public ListUsersHandlerTests()
    {
        _handler = new ListUsersHandler(_gateway);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsSuccessWithCollectionResult()
    {
        var items = new List<UserSummaryDTO>
        {
            new(1, "Fagner", "fagner@example.com", "Active"),
            new(2, "John", null, "Deactivated"),
        };
        var collection = new CollectionResult<UserSummaryDTO>(items, page: 1, pageSize: 20, totalItems: 2);
        _gateway.ListAsync(Arg.Any<ListUsersFilter>(), Arg.Any<CancellationToken>()).Returns(collection);

        var result = await _handler.Handle(new ListUsersFilter(null, 1, 20), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.TotalItems);
        Assert.Equal(2, result.Data.Items.Count);
        Assert.Equal(1, result.Data.Page);
        Assert.Equal(20, result.Data.PageSize);
    }

    [Fact]
    public async Task ExecuteAsync_WithStatusFilter_PassesFilterToGateway()
    {
        var items = new List<UserSummaryDTO>();
        var collection = new CollectionResult<UserSummaryDTO>(items, 1, 20, 0);
        _gateway.ListAsync(Arg.Any<ListUsersFilter>(), Arg.Any<CancellationToken>()).Returns(collection);

        await _handler.Handle(new ListUsersFilter(UserStatus.Active, 1, 20), CancellationToken.None);

        await _gateway.Received(1).ListAsync(
            Arg.Is<ListUsersFilter>(f => f.Status == UserStatus.Active && f.Page == 1 && f.PageSize == 20),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithPagination_PassesPageValuesToGateway()
    {
        var items = new List<UserSummaryDTO>();
        var collection = new CollectionResult<UserSummaryDTO>(items, 2, 10, 0);
        _gateway.ListAsync(Arg.Any<ListUsersFilter>(), Arg.Any<CancellationToken>()).Returns(collection);

        await _handler.Handle(new ListUsersFilter(null, 2, 10), CancellationToken.None);

        await _gateway.Received(1).ListAsync(
            Arg.Is<ListUsersFilter>(f => f.Page == 2 && f.PageSize == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmpty_ReturnsSuccessWithZeroItems()
    {
        var collection = new CollectionResult<UserSummaryDTO>(new List<UserSummaryDTO>(), 1, 20, 0);
        _gateway.ListAsync(Arg.Any<ListUsersFilter>(), Arg.Any<CancellationToken>()).Returns(collection);

        var result = await _handler.Handle(new ListUsersFilter(null, 1, 20), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!.Items);
        Assert.Equal(0, result.Data.TotalItems);
    }
}
