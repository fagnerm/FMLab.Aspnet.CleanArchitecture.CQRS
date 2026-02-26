// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FluentValidation;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;
using MediatR;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUserRepository _repository;
    private readonly IValidator<UpdateUserCommand> _validator;

    public UpdateUserHandler(IUserRepository repository, IValidator<UpdateUserCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result> Handle(UpdateUserCommand input, CancellationToken cancellationToken)
    {
        var validation = _validator.Validate(input);
        if (!validation.IsValid)
            return Result.Validation(error: validation.Errors[0].ErrorMessage);

        var user = await _repository.GetByIdAsync(input.Id, cancellationToken);

        if (user is null) return Result.NotFound("User not found");

        var name = new Name(input.Name!);
        var email = string.IsNullOrEmpty(input.Email) ? null : new Email(input.Email!);
        user.Update(name, email);

        _repository.Update(user);

        return Result.Success();
    }
}
