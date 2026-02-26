// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Shared.Mediator.Request;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.DisableUser;

public record DisableUserCommand(int Id) : ICommand<Result>;
