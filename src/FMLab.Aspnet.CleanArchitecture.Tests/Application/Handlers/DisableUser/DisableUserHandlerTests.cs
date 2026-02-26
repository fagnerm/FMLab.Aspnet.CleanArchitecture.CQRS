// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.DisableUser;
using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.Enums;
using FMLab.Aspnet.CleanArchitecture.Domain.Exceptions;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using NSubstitute;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Application.Handlers.DisableUser;

public class DisableUserHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly DisableUserHandler _handler;

    public DisableUserHandlerTests()
    {
        _handler = new DisableUserHandler(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserActive_ReturnsSuccess()
    {
        var user = new User(new Name("Fagner"), null);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new DisableUserCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserActive_SetsStatusToDeactivated()
    {
        var user = new User(new Name("Fagner"), null);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        await _handler.Handle(new DisableUserCommand(1), CancellationToken.None);

        Assert.Equal(UserStatus.Deactivated, user.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ReturnsNotFound()
    {
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new DisableUserCommand(99), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.NotFound, result.Type);
        Assert.Equal("User not found", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserAlreadyDeactivated_ReturnsDomainException()
    {
        var user = new User(new Name("Fagner"), null);
        user.Deactivate();
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(new DisableUserCommand(1), CancellationToken.None));
    }
}
