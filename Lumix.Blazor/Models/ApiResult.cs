namespace Lumix.Blazor.Models;

public class ApiResult<T>
{
    public T? Value { get; private set; }
    public bool IsFailed => !IsSuccess;
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public static ApiResult<T> Success(T value) => new ApiResult<T> { Value = value, IsSuccess = true };
    public static ApiResult<T> Failure(string errorMessage) => new ApiResult<T> { IsSuccess = false, ErrorMessage = errorMessage };

    private ApiResult() { }
}