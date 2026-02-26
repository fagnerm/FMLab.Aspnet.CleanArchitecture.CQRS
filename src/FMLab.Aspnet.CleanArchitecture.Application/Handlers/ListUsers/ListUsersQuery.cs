// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Shared.Filter;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Mediator.Request;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using FMLab.Aspnet.CleanArchitecture.Domain.Enums;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.ListUsers;

public record ListUsersQuery(UserStatus? Status, int Page = 1, int PageSize = 20)
    : PaginationFilter(Page, PageSize), IQuery<Result>;