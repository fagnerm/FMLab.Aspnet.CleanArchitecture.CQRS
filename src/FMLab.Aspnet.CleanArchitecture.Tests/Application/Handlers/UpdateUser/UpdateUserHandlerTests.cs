// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FluentValidation;
using FluentValidation.Results;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.UpdateUser;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using MediatR;
using NSubstitute;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Application.Handlers.UpdateUser;

public class UpdateUserHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly IValidator<UpdateUserCommand> _validator = Substitute.For<IValidator<UpdateUserCommand>>();
    private readonly UpdateUserHandler _handler;
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    public UpdateUserHandlerTests()
    {
        _handler = new UpdateUserHandler(_repository, _validator);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ReturnsSuccess()
    {
        var user = new User(new Name("Fagner"), null);
        _validator.Validate(Arg.Any<UpdateUserCommand>()).Returns(new ValidationResult());
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new UpdateUserCommand(1, "John", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSuccessful_CallsRepositoryUpdate()
    {
        var user = new User(new Name("Fagner"), null);
        _validator.Validate(Arg.Any<UpdateUserCommand>()).Returns(new ValidationResult());
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        await _handler.Handle(new UpdateUserCommand(1, "John", null), CancellationToken.None);

        _repository.Received(1).Update(user);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidationFails_ReturnsValidationResult()
    {
        var failure = new ValidationFailure("Name", "Name is required");
        _validator.Validate(Arg.Any<UpdateUserCommand>()).Returns(new ValidationResult(new[] { failure }));

        var result = await _handler.Handle(new UpdateUserCommand(1, null, null), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Validation, result.Type);
        Assert.Equal("Name is required", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidationFails_DoesNotQueryRepository()
    {
        var failure = new ValidationFailure("Name", "Name is required");
        _validator.Validate(Arg.Any<UpdateUserCommand>()).Returns(new ValidationResult(new[] { failure }));

        await _handler.Handle(new UpdateUserCommand(1, null, null), CancellationToken.None);

        await _repository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ReturnsNotFound()
    {
        _validator.Validate(Arg.Any<UpdateUserCommand>()).Returns(new ValidationResult());
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new UpdateUserCommand(99, "John", null), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.NotFound, result.Type);
        Assert.Equal("User not found", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_DoesNotCommit()
    {
        _validator.Validate(Arg.Any<UpdateUserCommand>()).Returns(new ValidationResult());
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        await _handler.Handle(new UpdateUserCommand(99, "John", null), CancellationToken.None);

        await _unitOfWork.DidNotReceive().CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    // Patch

    //[Fact]
    //public async Task ExecuteAsync_WithNewName_UpdatesOnlyName()
    //{
    //    var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
    //    _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

    //    var result = await _handler.Handle(new UpdateUserCommand(1, "John", null), CancellationToken.None);

    //    Assert.True(result.IsSuccess);
    //    Assert.Equal("John", result.Data!.Name);
    //    Assert.Equal("fagner@example.com", result.Data.Email);
    //}

    //[Fact]
    //public async Task ExecuteAsync_WithNewEmail_UpdatesOnlyEmail()
    //{
    //    var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
    //    _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

    //    var result = await _handler.Handle(new UpdateUserCommand(1, null, "new@example.com"), CancellationToken.None);

    //    Assert.True(result.IsSuccess);
    //    Assert.Equal("Fagner", result.Data!.Name);
    //    Assert.Equal("new@example.com", result.Data.Email);
    //}

    //[Fact]
    //public async Task ExecuteAsync_WithBothFields_UpdatesBoth()
    //{
    //    var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
    //    _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

    //    var result = await _handler.Handle(new UpdateUserCommand(1, "John", "john@example.com"), CancellationToken.None);

    //    Assert.True(result.IsSuccess);
    //    Assert.Equal("John", result.Data!.Name);
    //    Assert.Equal("john@example.com", result.Data.Email);
    //}

    //[Fact]
    //public async Task ExecuteAsync_WithNoFields_PreservesBoth()
    //{
    //    var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
    //    _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

    //    var result = await _handler.Handle(new UpdateUserCommand(1, null, null), CancellationToken.None);

    //    Assert.True(result.IsSuccess);
    //    Assert.Equal("Fagner", result.Data!.Name);
    //    Assert.Equal("fagner@example.com", result.Data.Email);
    //}

    //[Fact]
    //public async Task ExecuteAsync_WhenSuccessful_ReturnsPopulatedOutputDTO()
    //{
    //    var user = new User(new Name("Fagner"), null);
    //    _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

    //    var result = await _handler.Handle(new UpdateUserCommand(1, "John", null), CancellationToken.None);

    //    Assert.True(result.IsSuccess);
    //    Assert.NotNull(result.Data);
    //    Assert.Equal("John", result.Data.Name);
    //    Assert.Null(result.Data.Email);
    //}

    //[Fact]
    //public async Task ExecuteAsync_WhenSuccessful_CommitsUnitOfWork()
    //{
    //    var user = new User(new Name("Fagner"), null);
    //    _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

    //    await _handler.Handle(new UpdateUserCommand(1, "John", null), CancellationToken.None);

    //    await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    //}

    //[Fact]
    //public async Task ExecuteAsync_WhenUserNotFound_ReturnsNotFound()
    //{
    //    _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

    //    var result = await _handler.Handle(new UpdateUserCommand(99, "John", null), CancellationToken.None);

    //    Assert.False(result.IsSuccess);
    //    Assert.Equal(ResultType.NotFound, result.Type);
    //    Assert.Equal("User not found", result.Error);
    //}
}
