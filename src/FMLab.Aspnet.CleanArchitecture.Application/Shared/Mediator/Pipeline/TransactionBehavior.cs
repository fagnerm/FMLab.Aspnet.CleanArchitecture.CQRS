// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Interfaces;
using FMLab.Aspnet.CleanArchitecture.Application.Shared.Mediator.Request;
using MediatR;

namespace FMLab.Aspnet.CleanArchitecture.Application.Shared.Mediator.Pipeline;
public class TransactionBehavior<TInput, TResponse> : IPipelineBehavior<TInput, TResponse>
    where TInput : ICommand<TResponse>
    where TResponse : Result.Result
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TInput request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();

            var result = response as Result.Result;

            if (!result.IsSuccess)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return response;
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return response;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
