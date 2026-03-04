// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Handlers.GetUser;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Gateways;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.DTOs;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using NSubstitute;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Application.Handlers.GetUser;

public class GetUserHandlerTests
{
    private readonly IUserGateway _gateway = Substitute.For<IUserGateway>();
    private readonly GetUserHandler _handler;

    public GetUserHandlerTests()
    {
        _handler = new GetUserHandler(_gateway);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserExists_ReturnsSuccessWithData()
    {
        var dto = new UserSummaryDTO(1, "Fagner", "fagner@example.com", "Active");
        _gateway.ListUserByIdAsync(1, Arg.Any<CancellationToken>()).Returns(dto);

        var result = await _handler.Handle(new GetUserQuery(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("Fagner", result.Data.Name);
        Assert.Equal("fagner@example.com", result.Data.Email);
        Assert.Equal("Active", result.Data.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserExists_PassesCorrectIdToGateway()
    {
        var dto = new UserSummaryDTO(42, "Fagner", null, "Active");
        _gateway.ListUserByIdAsync(42, Arg.Any<CancellationToken>()).Returns(dto);

        await _handler.Handle(new GetUserQuery(42), CancellationToken.None);

        await _gateway.Received(1).ListUserByIdAsync(42, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ReturnsNotFound()
    {
        _gateway.ListUserByIdAsync(99, Arg.Any<CancellationToken>()).Returns((UserSummaryDTO?)null);

        var result = await _handler.Handle(new GetUserQuery(99), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.NotFound, result.Type);
        Assert.Equal("User not found", result.Error);
    }
}
