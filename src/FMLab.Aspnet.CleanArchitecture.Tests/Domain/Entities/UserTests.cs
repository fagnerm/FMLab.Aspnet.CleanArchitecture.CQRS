// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.Enums;
using FMLab.Aspnet.CleanArchitecture.Domain.Exceptions;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Domain.Entities;

public class UserTests
{

    [Fact]
    public void Constructor_WithNameAndEmail_SetsPropertiesCorrectly()
    {
        var name = new Name("Fagner");
        var email = new Email("fagner@example.com");

        var user = new User(name, email);

        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(UserStatus.Active, user.Status);
    }

    [Fact]
    public void Constructor_WithNullEmail_LeavesEmailNull()
    {
        var user = new User(new Name("Fagner"), null);

        Assert.Null(user.Email);
        Assert.Equal(UserStatus.Active, user.Status);
    }

    [Fact]
    public void Constructor_NewUser_StatusIsActive()
    {
        var user = new User(new Name("Fagner"), null);

        Assert.Equal(UserStatus.Active, user.Status);
    }


    [Fact]
    public void Deactivate_WhenActive_SetsStatusToDeactivated()
    {
        var user = new User(new Name("Fagner"), null);

        user.Deactivate();

        Assert.Equal(UserStatus.Deactivated, user.Status);
    }

    [Fact]
    public void Deactivate_WhenAlreadyDeactivated_ThrowsDomainException()
    {
        var user = new User(new Name("Fagner"), null);
        user.Deactivate();

        var ex = Assert.Throws<DomainException>(() => user.Deactivate());

        Assert.Equal("User already deactivated", ex.Message);
    }


    [Fact]
    public void Update_WithNewNameAndEmail_UpdatesBothProperties()
    {
        var user = new User(new Name("Fagner"), new Email("fagner@example.com"));
        var newName = new Name("John");
        var newEmail = new Email("john@example.com");

        user.Update(newName, newEmail);

        Assert.Equal(newName, user.Name);
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void Update_WithNullEmail_ClearsEmail()
    {
        var user = new User(new Name("Fagner"), new Email("Fagner@example.com"));

        user.Update(new Name("Fagner"), null);

        Assert.Null(user.Email);
    }

    [Fact]
    public void Update_DoesNotChangeStatus()
    {
        var user = new User(new Name("Fagner"), null);

        user.Update(new Name("Jane"), null);

        Assert.Equal(UserStatus.Active, user.Status);
    }

    [Fact]
    public void Update_OnDeactivatedUser_UpdatesNameWithoutReactivating()
    {
        var user = new User(new Name("Fagner"), null);
        user.Deactivate();

        user.Update(new Name("Jane"), null);

        Assert.Equal("Jane", user.Name.Value);
        Assert.Equal(UserStatus.Deactivated, user.Status);
    }
}
