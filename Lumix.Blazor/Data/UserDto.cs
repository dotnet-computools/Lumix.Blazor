namespace Lumix.Blazor.Data;

public class UserDto
{
    public Guid id { get; set; }
    public string username { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? profilePictureUrl { get; set; }
    public string? bio { get; set; }
    public DateTime createdAt { get; set; }
}
public class RefreshTokenDto
{
    public string refreshToken { get; set; } = string.Empty;
}