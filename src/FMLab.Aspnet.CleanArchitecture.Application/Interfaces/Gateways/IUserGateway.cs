// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.DTOs;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.ListUsers;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;

namespace FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Gateways;

public interface IUserGateway
{
    Task<CollectionResult<UserSummaryDTO>> ListAsync(ListUsersFilter filter, CancellationToken cancellationToken);
    Task<UserSummaryDTO?> ListUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsByKeyAsync(string? name, string? email, CancellationToken cancellationToken);
}
