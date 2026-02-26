// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Shared.Filter;
using FMLab.Aspnet.CleanArchitecture.Domain.Enums;

namespace FMLab.Aspnet.CleanArchitecture.Application.Handlers.ListUsers;

public record ListUsersFilter(UserStatus? Status, int Page, int PageSize)
    : PaginationFilter(Page, PageSize);
