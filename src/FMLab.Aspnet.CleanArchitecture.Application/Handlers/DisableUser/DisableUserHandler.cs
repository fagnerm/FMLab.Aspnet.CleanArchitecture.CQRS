// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using MediatR;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.DisableUser;

public class DisableUserHandler : IRequestHandler<DisableUserCommand, Result<NoOutput>>
{
    private readonly IUserRepository _repository;

    public DisableUserHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<NoOutput>> Handle(DisableUserCommand input, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(input.Id, cancellationToken);

        if (user == null)
        {
            return Result<NoOutput>.NotFound("User not found");
        }

        user.Deactivate();
        _repository.Update(user);

        return Result<NoOutput>.NoContent();
    }
}
