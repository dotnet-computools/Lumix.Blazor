namespace Lumix.Blazor.Data;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}