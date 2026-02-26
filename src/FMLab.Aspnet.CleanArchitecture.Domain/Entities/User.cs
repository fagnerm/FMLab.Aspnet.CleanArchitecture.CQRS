// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Domain.Enums;
using FMLab.Aspnet.CleanArchitecture.Domain.Exceptions;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;

namespace FMLab.Aspnet.CleanArchitecture.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public Name Name { get; private set; }
    public Email? Email { get; private set; }
    public UserStatus Status { get; private set; }

    private User()
    {
        Name = null!;
        Email = null!;
    }

    public User(Name name, Email? email)
    {
        Name = name;
        Status = UserStatus.Active;
        Email = email;
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Deactivated)
            DomainGuard.Throw("User already deactivated");

        Status = UserStatus.Deactivated;
    }

    public void Update(Name name, Email? email)
    {
        Name = name;
        Email = email;
    }
}
