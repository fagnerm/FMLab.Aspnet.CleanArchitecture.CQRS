// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Domain.Entities;

namespace FMLab.Aspnet.CleanArchitecture.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    void Delete(User user);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
    User Update(User user);
}
