// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Gateways;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using MediatR;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result>
{
    private readonly IUserRepository _repository;
    private readonly IUserGateway _gateway;

    public CreateUserHandler(IUserRepository userRepository, IUserGateway gateway)
    {
        _repository = userRepository;
        _gateway = gateway;
    }

    public async Task<Result> Handle(CreateUserCommand input, CancellationToken cancellationToken)
    {
        var name = new Name(input.Name);
        var email = input.Email is null ? null : new Email(input.Email);

        var found = await _gateway.ExistsByKeyAsync(name.Value, email?.Value, cancellationToken);

        if (found) return Result.Conflict("User already exists");

        var user = new User(name, email);
        await _repository.AddAsync(user, cancellationToken);

        var result = new CreateUserOutputDTO(user.Id, user.Name.Value, user.Email?.Value, user.Status.ToString());

        return Result.Success(result);
    }
}
