// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Handlers.ListUsers;
using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Gateways;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.DTOs;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Gateways;

public class UserGateway : IUserGateway
{
    private readonly ApplicationDbContext _context;

    public UserGateway(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByKeyAsync(string? name, string? email, CancellationToken cancellationToken)
    {
        return await _context.Users
                             .AsNoTracking()
                             .AsQueryable()
                             .AnyAsync(u => (name != null && u.Name.Value == name) ||
                                            (email != null && u.Email!.Value == email), cancellationToken);
    }

    public async Task<CollectionResult<UserSummaryDTO>> ListAsync(ListUsersFilter filter, CancellationToken cancellationToken)
    {
        var query = _context.Users
                            .AsNoTracking()
                            .AsQueryable();

        if (filter.Status.HasValue)
        {
            query = query.Where(t => t.Status == filter.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.OrderByDescending(t => t.Name)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => new UserSummaryDTO(
                t.Id,
                t.Name.Value,
                t.Email!.Value,
                t.Status.ToString()
                ))
            .ToListAsync(cancellationToken);

        return new CollectionResult<UserSummaryDTO>(
            items, filter.Page, filter.PageSize, totalCount);
    }

    public async Task<UserSummaryDTO?> ListUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        var query = _context.Users
                            .AsNoTracking()
                            .AsQueryable();

        var user = await query.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new UserSummaryDTO(user.Id, user.Name.Value, user.Email?.Value, user.Status.ToString());
    }
}
