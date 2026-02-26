// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Handlers.CreateUser;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Application.UseCases.CreateUser;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator = new();

    [Fact]
    public void Validate_WithValidName_IsValid()
    {
        var result = _validator.Validate(new CreateUserCommand("Fagner", null));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithValidNameAndEmail_IsValid()
    {
        var result = _validator.Validate(new CreateUserCommand("Fagner", "fagner@example.com"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyName_IsInvalid()
    {
        var result = _validator.Validate(new CreateUserCommand("", null));

        Assert.False(result.IsValid);
        Assert.Equal("Name is required", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public void Validate_WithNullName_IsInvalid()
    {
        var result = _validator.Validate(new CreateUserCommand(null!, null));

        Assert.False(result.IsValid);
        Assert.Equal("Name is required", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public void Validate_WithNullEmail_IsValid()
    {
        var result = _validator.Validate(new CreateUserCommand("Fagner", null));

        Assert.True(result.IsValid);
    }
}
