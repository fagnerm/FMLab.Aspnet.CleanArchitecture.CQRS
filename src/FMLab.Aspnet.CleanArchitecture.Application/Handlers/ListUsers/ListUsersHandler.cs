// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Gateways;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using MediatR;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.ListUsers;

public class ListUsersHandler : IRequestHandler<ListUsersQuery, Result>
{
    private readonly IUserGateway _gateway;

    public ListUsersHandler(IUserGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task<Result> Handle(ListUsersQuery input, CancellationToken cancellationToken)
    {
        var filter = new ListUsersFilter(input.Status, input.Page, input.PageSize);
        var result = await _gateway.ListAsync(filter, cancellationToken);

        var output = new ListUsersOutputDTO(result.Items, result.Page, result.PageSize, result.TotalItems);
        return Result.Success(output);
    }
}
