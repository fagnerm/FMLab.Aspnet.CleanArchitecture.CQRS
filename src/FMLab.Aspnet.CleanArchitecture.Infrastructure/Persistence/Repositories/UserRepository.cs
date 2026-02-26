// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;
using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public async Task AddAsync(User User, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(User, cancellationToken)
                        .ConfigureAwait(false);
    }

    public void Delete(User user)
    {
        _dbContext.Remove(user);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _dbContext
                            .Users
                            .AsTracking()
                            .SingleOrDefaultAsync(_ => _.Id == id, cancellationToken);

        return user;
    }

    public User Update(User user)
    {
        _dbContext.Update(user);
        return user;
    }
}
