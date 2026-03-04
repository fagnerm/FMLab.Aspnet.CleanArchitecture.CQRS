// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Domain.Exceptions;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using MediatR;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<CreateUserOutputDTO>>
{
    private readonly IUserRepository _repository;

    public CreateUserHandler(IUserRepository userRepository)
    {
        _repository = userRepository;
    }

    public async Task<Result<CreateUserOutputDTO>> Handle(CreateUserCommand input, CancellationToken cancellationToken)
    {
        var name = new Name(input.Name);
        var email = input.Email is null ? null : new Email(input.Email);
        var user = new User(name, email);

        try
        {
            await _repository.AddAsync(user, cancellationToken);
        }
        catch (DomainException ex) when (ex.Message == "User already exists")
        {
            return Result<CreateUserOutputDTO>.Conflict(ex.Message);
        }

        var result = new CreateUserOutputDTO(user.Id, user.Name.Value, user.Email?.Value, user.Status.ToString());

        return Result<CreateUserOutputDTO>.Success(result);
    }
}
