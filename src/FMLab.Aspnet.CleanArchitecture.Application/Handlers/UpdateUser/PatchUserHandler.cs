// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using MediatR;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.UpdateUser;
public class PatchUserHandler : IRequestHandler<PatchUserCommand, Result<UpdateUserOutputDTO>>
{
    private readonly IUserRepository _repository;

    public PatchUserHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UpdateUserOutputDTO>> Handle(PatchUserCommand input, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(input.Id, cancellationToken);

        if (user is null) return Result<UpdateUserOutputDTO>.NotFound("User not found");

        var name = string.IsNullOrEmpty(input.Name) ? user.Name : new Name(input.Name!);
        var email = string.IsNullOrEmpty(input.Email) ? user.Email : new Email(input.Email!);
        user.Update(name, email);

        _repository.Update(user);

        var result = new UpdateUserOutputDTO(user.Id, user.Name.Value, user.Email?.Value, user.Status.ToString());
        return Result<UpdateUserOutputDTO>.Success(result);
    }
}
