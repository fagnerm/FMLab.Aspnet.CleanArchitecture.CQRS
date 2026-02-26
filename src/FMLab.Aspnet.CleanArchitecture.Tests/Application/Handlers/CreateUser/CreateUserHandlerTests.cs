// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FluentValidation;
using FluentValidation.Results;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Gateways;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.CreateUser;
using NSubstitute;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Application.Handlers.CreateUser;

public class CreateUserHandlerTests
{
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly IUserGateway _gateway = Substitute.For<IUserGateway>();
    private readonly IValidator<CreateUserCommand> _validator = Substitute.For<IValidator<CreateUserCommand>>();
    private readonly CreateUserHandler _mediator;

    public CreateUserHandlerTests()
    {
        _mediator = new CreateUserHandler(_repository, _gateway);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ReturnsSuccessWithData()
    {
        _validator.Validate(Arg.Any<CreateUserCommand>()).Returns(new ValidationResult());
        _gateway.ExistsByKeyAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _mediator.Handle(new CreateUserCommand("Fagner", "fagner@example.com"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data<CreateUserOutputDTO>());
        Assert.Equal("Fagner", result.Data<CreateUserOutputDTO>().Name);
        Assert.Equal("fagner@example.com", result.Data<CreateUserOutputDTO>().Email);
        Assert.Equal("Active", result.Data<CreateUserOutputDTO>().Status);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullEmail_ReturnsSuccessWithNullEmail()
    {
        _validator.Validate(Arg.Any<CreateUserCommand>()).Returns(new ValidationResult());
        _gateway.ExistsByKeyAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _mediator.Handle(new CreateUserCommand("Fagner", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Data<CreateUserOutputDTO>()!.Email);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserAlreadyExists_ReturnsConflict()
    {
        _validator.Validate(Arg.Any<CreateUserCommand>()).Returns(new ValidationResult());
        _gateway.ExistsByKeyAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(true);

        var result = await _mediator.Handle(new CreateUserCommand("Fagner", "fagner@example.com"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Conflict, result.Type);
        Assert.Equal("User already exists", result.Error);
    }
}
