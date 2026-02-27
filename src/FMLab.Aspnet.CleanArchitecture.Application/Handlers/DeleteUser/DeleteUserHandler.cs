// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using MediatR;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUserRepository _repository;

    public DeleteUserHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteUserCommand input, CancellationToken cancellationToken)
    {
        var existingUser = await _repository.GetByIdAsync(input.Id, cancellationToken);

        if (existingUser is null) return Result.NotFound("User not found");

        _repository.Delete(existingUser!);

        return Result.NoContent();
    }
}
