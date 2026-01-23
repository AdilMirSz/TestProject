namespace TestProject.Shared.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "TestProject";
    public string Audience { get; set; } = "TestProject";
    public string Key { get; set; } = null!;
    public int AccessTokenMinutes { get; set; } = 60;
}