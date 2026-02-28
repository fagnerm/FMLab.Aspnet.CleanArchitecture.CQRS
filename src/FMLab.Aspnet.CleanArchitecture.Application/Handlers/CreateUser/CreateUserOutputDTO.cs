// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.CreateUser;

public record CreateUserOutputDTO(int Id, string Name, string? Email, string Status);
