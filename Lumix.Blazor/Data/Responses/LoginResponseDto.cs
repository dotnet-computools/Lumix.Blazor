namespace Lumix.Blazor.Data.Responses;

public class LoginResponseDto
{
    public string accessToken { get; set; } = string.Empty;
    public string refreshToken { get; set; } = string.Empty;
}