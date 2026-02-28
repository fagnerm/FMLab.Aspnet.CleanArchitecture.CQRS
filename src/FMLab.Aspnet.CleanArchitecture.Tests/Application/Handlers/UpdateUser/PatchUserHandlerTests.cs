// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Handlers.UpdateUser;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Application.Handlers.UpdateUser;
public class PatchUserHandlerTests
{
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly PatchUserHandler _handler;

    public PatchUserHandlerTests()
    {
        _handler = new PatchUserHandler(_repository);
    }


    [Fact]
    public async Task ExecuteAsync_WithNewName_UpdatesOnlyName()
    {
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new PatchUserCommand(1, "John", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("John", result.Data!.Name);
        Assert.Equal("fagner@example.com", result.Data.Email);
    }

    [Fact]
    public async Task ExecuteAsync_WithNewEmail_UpdatesOnlyEmail()
    {
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new PatchUserCommand(1, null, "new@example.com"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Fagner", result.Data!.Name);
        Assert.Equal("new@example.com", result.Data.Email);
    }

    [Fact]
    public async Task ExecuteAsync_WithBothFields_UpdatesBoth()
    {
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new PatchUserCommand(1, "John", "john@example.com"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("John", result.Data!.Name);
        Assert.Equal("john@example.com", result.Data.Email);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoFields_PreservesBoth()
    {
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new PatchUserCommand(1, null, null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Fagner", result.Data!.Name);
        Assert.Equal("fagner@example.com", result.Data.Email);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSuccessful_ReturnsPopulatedOutputDTO()
    {
        var user = new User(new Name("Fagner"), null);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new PatchUserCommand(1, "John", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("John", result.Data.Name);
        Assert.Null(result.Data.Email);
    }
}
