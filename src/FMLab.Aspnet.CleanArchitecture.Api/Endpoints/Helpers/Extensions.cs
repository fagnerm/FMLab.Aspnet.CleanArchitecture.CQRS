// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;

namespace FMLab.Aspnet.CleanArchitecture.Api.Endpoints.Helpers;

public static class Extensions
{
    public static IResult ToProblemResult(this Result result, ResultType? @default = null)
    {
        var type = result.IsSuccess && @default.HasValue ? @default : result.Type;

        return type switch
        {
            ResultType.Success => Results.Ok(result.Data<IResultData>()),
            ResultType.NoContent => Results.NoContent(),
            ResultType.NotFound => Results.Problem(result.Error, statusCode: StatusCodes.Status404NotFound, type: "about:blank"),
            ResultType.Validation => Results.Problem(result.Error, statusCode: StatusCodes.Status422UnprocessableEntity, type: "about:blank"),
            ResultType.Domain => Results.Problem(result.Error, statusCode: StatusCodes.Status422UnprocessableEntity, type: "about:blank"),
            ResultType.Conflict => Results.Problem(result.Error, statusCode: StatusCodes.Status409Conflict, type: "about:blank"),
            _ => Results.Problem(statusCode: 500, type: "about:blank")
        };
    }
}
