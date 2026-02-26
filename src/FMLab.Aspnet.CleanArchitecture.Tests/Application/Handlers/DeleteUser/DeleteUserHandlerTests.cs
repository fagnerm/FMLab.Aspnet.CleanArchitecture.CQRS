// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.DeleteUser;
using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using NSubstitute;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Application.Handlers.DeleteUser;

public class DeleteUserHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _handler = new DeleteUserHandler(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserExists_ReturnsSuccess()
    {
        var user = new User(new Name("Fagner"), null);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new DeleteUserCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserExists_CallsRepositoryDelete()
    {
        var user = new User(new Name("Fagner"), null);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        await _handler.Handle(new DeleteUserCommand(1), CancellationToken.None);

        _repository.Received(1).Delete(user);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ReturnsNotFound()
    {
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new DeleteUserCommand(99), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.NotFound, result.Type);
        Assert.Equal("User not found", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_DoesNotCommit()
    {
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        await _handler.Handle(new DeleteUserCommand(99), CancellationToken.None);

        await _unitOfWork.DidNotReceive().CommitTransactionAsync(Arg.Any<CancellationToken>());
    }
}
