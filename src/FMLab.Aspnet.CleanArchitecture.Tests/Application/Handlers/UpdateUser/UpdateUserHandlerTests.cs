// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Handlers.UpdateUser;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using NSubstitute;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Application.Handlers.UpdateUser;

public class UpdateUserHandlerTests
{
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly UpdateUserHandler _handler;

    public UpdateUserHandlerTests()
    {
        _handler = new UpdateUserHandler(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ReturnsSuccess()
    {
        var user = new User(new Name("Fagner"), null);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new UpdateUserCommand(1, "John", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSuccessful_CallsRepositoryUpdate()
    {
        var user = new User(new Name("Fagner"), null);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        await _handler.Handle(new UpdateUserCommand(1, "John", null), CancellationToken.None);

        _repository.Received(1).Update(user);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ReturnsNotFound()
    {
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new UpdateUserCommand(99, "John", null), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.NotFound, result.Type);
        Assert.Equal("User not found", result.Error);
    }
}
