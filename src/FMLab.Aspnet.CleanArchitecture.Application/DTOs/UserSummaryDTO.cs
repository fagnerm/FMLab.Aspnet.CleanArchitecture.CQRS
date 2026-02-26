// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;

namespace FMLab.Aspnet.CleanArchitecture.Application.DTOs;

public record UserSummaryDTO(
    int Id,
    string Name,
    string? Email,
    string Status
    ) : IResultData;
