// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.CleanArchitecture.Application.Shared.Result;

public class Result
{

    private IResultData? _data;

    public bool IsSuccess { get; protected set; }
    public string? Error { get; protected set; }
    public ResultType Type { get; protected set; }

    private Result(ResultType type = ResultType.Success)
    {
        Type = type;
        _data = default!;
    }

    private Result(string? error, ResultType type)
    {
        IsSuccess = false;
        Error = error;
        Type = type;
        _data = default!;
    }

    public static Result Success(IResultData? data = default)
    {
        return new Result(ResultType.Success)
        {
            IsSuccess = true,
            _data = data
        };
    }

    public static Result NotFound(string? error)
    {
        return new Result(error, ResultType.NotFound);
    }

    public static Result Validation(string? error)
    {
        return new Result(error, ResultType.Validation);
    }

    public static Result Domain(string? error)
    {
        return new Result(error, ResultType.Domain);
    }

    public static Result Conflict(string? error)
    {
        return new Result(error, ResultType.Conflict);
    }

    public TOutput Data<TOutput>()
        where TOutput : IResultData
    {
        try
        {
            return (TOutput)_data!;
        }
        catch
        {
            return default!;
        }
    }

}

public interface IResultData { }


public enum ResultType
{
    Success,
    NoContent,
    NotFound,
    Validation,
    Domain,
    Conflict
}