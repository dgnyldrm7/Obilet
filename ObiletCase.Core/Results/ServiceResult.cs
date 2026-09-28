using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ObiletCase.Core.Result;

public class ServiceResult<T>
{
    private ServiceResult(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; init; }

    public T? Data { get; init; }

    public ProblemDetails? ProblemDetails { get; init; }

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>(true)
        {
            Data = data
        };
    }

    public static ServiceResult<T> Fail(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ServiceResult<T>(false)
        {
            ProblemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = "Operation Error",
                Detail = message
            }
        };
    }

    public static ServiceResult<T> Exception(ProblemDetails problemDetails)
    {
        return new ServiceResult<T>(false)
        {
            ProblemDetails = problemDetails
        };
    }
}