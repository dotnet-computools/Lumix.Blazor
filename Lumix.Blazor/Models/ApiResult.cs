namespace Lumix.Blazor.Models;

public class ApiResult<T>
{
    public T? Value { get; set; } 
    public bool IsFailed => !IsSuccess;
    public bool IsSuccess { get; set; } 
    public string ErrorMessage { get; set; } = string.Empty;

    public static ApiResult<T> Success(T value) => new() { Value = value, IsSuccess = true };
    public static ApiResult<T> Failure(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };

    public ApiResult()
    {
    }
}