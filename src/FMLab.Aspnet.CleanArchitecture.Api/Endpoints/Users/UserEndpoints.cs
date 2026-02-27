// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Api.Endpoints.Helpers;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.CreateUser;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.DeleteUser;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.DisableUser;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.GetUser;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.ListUsers;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.UpdateUser;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FMLab.Aspnet.CleanArchitecture.Api.Endpoints.Users;

internal static class UserEndpoints
{
    internal static void MapUser(WebApplication app)
    {
        app.MapGet("/users", ListAllUsersEndpoint)
            .WithTags("Users")
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapGet("/users/{id}", ListUserEndpoint)
            .WithTags("Users")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();

        app.MapPost("/users/{id}/deactivate", DisableUserEndpoint)
            .WithTags("Users")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();

        app.MapPost("/users", PostUserEndpoint)
            .WithTags("Users")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status409Conflict)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithOpenApi();

        app.MapPatch("/users/{id}", PatchUserEndpoint)
            .WithTags("Users")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithOpenApi();

        app.MapPut("/users/{id}", PutUserEndpoint)
            .WithTags("Users")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithOpenApi();

        app.MapDelete("/users/{id}", DeleteUserEndpoint)
            .WithTags("Users")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }

    private static async Task<IResult> ListAllUsersEndpoint([FromServices] IMediator mediator, [AsParameters] ListUsersQuery input, CancellationToken cancellationToken)
    {
        var output = await mediator.Send(input, cancellationToken);

        return output.ToProblemResult();
    }

    private static async Task<IResult> ListUserEndpoint([FromServices] IMediator mediator, [FromRoute] int id, CancellationToken cancellationToken)
    {
        var input = new GetUserQuery(id);
        var output = await mediator.Send(input, cancellationToken);

        return output.ToProblemResult();
    }

    private static async Task<IResult> PostUserEndpoint([FromServices] IMediator mediator, [FromBody] CreateUserInputRequest body, CancellationToken cancellationToken)
    {
        var input = new CreateUserCommand(body.Name, body.Email);
        var output = await mediator.Send(input, cancellationToken);

        if (!output.IsSuccess) return output.ToProblemResult();

        var result = output.Data<CreateUserOutputDTO>();
        return Results.Created($"/users/{result?.Id}", result);
    }

    private static async Task<IResult> DisableUserEndpoint([FromServices] IMediator mediator, [FromRoute] int id, CancellationToken cancellationToken)
    {
        var input = new DisableUserCommand(id);
        var output = await mediator.Send(input, cancellationToken);

        return output.ToProblemResult(ResultType.NoContent);
    }

    private static async Task<IResult> PutUserEndpoint([FromServices] IMediator mediator, [FromRoute] int id, [FromBody] UpdateUserInputRequest body, CancellationToken cancellationToken)
    {
        var input = new UpdateUserCommand(id, body.Name, body.Email);
        var output = await mediator.Send(input, cancellationToken);

        return output.ToProblemResult();
    }

    private static async Task<IResult> PatchUserEndpoint([FromServices] IMediator mediator, [FromRoute] int id, [FromBody] UpdateUserInputRequest body, CancellationToken cancellationToken)
    {
        var input = new PatchUserCommand(id, body.Name, body.Email);
        var output = await mediator.Send(input, cancellationToken);

        return output.ToProblemResult();
    }

    private static async Task<IResult> DeleteUserEndpoint([FromServices] IMediator mediator, [FromRoute] int id, CancellationToken cancellationToken)
    {
        var input = new DeleteUserCommand(id);
        var output = await mediator.Send(input, cancellationToken);

        return output.ToProblemResult(ResultType.NoContent);
    }
}
