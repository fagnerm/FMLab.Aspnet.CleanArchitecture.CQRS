// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Gateways;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using MediatR;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.GetUser;

public class GetUserHandler : IRequestHandler<GetUserQuery, Result>
{
    private readonly IUserGateway _gateway;

    public GetUserHandler(IUserGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task<Result> Handle(GetUserQuery input, CancellationToken cancellationToken)
    {
        var user = await _gateway.ListUserByIdAsync(input.Id, cancellationToken);

        if (user == null)
        {
            return Result.NotFound("User not found");
        }

        return Result.Success(user);
    }
}
