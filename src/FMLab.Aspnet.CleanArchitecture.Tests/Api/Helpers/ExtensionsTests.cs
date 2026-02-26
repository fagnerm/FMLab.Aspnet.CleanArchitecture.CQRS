// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Api.Endpoints.Helpers;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Api.Helpers;

public class ExtensionsTests
{
    private sealed record TestData(string Value) : IResultData;

    [Fact]
    public void ToProblemResult_OnSuccess_ReturnsOkWithData()
    {
        var data = new TestData("test");
        var result = Result.Success(data);

        var httpResult = result.ToProblemResult();

        var ok = Assert.IsType<Ok<IResultData>>(httpResult);
        Assert.Equal(data, ok.Value);
    }

    [Fact]
    public void ToProblemResult_OnSuccessWithNoContentDefault_ReturnsNoContent()
    {
        var result = Result.Success();

        var httpResult = result.ToProblemResult(ResultType.NoContent);

        Assert.IsType<NoContent>(httpResult);
    }

    [Fact]
    public void ToProblemResult_OnNotFound_Returns404Problem()
    {
        var result = Result.NotFound("User not found");

        var httpResult = result.ToProblemResult();

        var problem = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, problem.StatusCode);
        Assert.Equal("User not found", problem.ProblemDetails.Detail);
    }

    [Fact]
    public void ToProblemResult_OnValidation_Returns422Problem()
    {
        var result = Result.Validation("Name is required");

        var httpResult = result.ToProblemResult();

        var problem = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, problem.StatusCode);
        Assert.Equal("Name is required", problem.ProblemDetails.Detail);
    }

    [Fact]
    public void ToProblemResult_OnDomain_Returns422Problem()
    {
        var result = Result.Domain("User already deactivated");

        var httpResult = result.ToProblemResult();

        var problem = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, problem.StatusCode);
        Assert.Equal("User already deactivated", problem.ProblemDetails.Detail);
    }

    [Fact]
    public void ToProblemResult_OnConflict_Returns409Problem()
    {
        var result = Result.Conflict("User already exists");

        var httpResult = result.ToProblemResult();

        var problem = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status409Conflict, problem.StatusCode);
        Assert.Equal("User already exists", problem.ProblemDetails.Detail);
    }

    [Fact]
    public void ToProblemResult_OnFailure_DefaultIsIgnored()
    {
        var result = Result.NotFound("not found");

        var httpResult = result.ToProblemResult(ResultType.NoContent);

        var problem = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, problem.StatusCode);
    }
}
