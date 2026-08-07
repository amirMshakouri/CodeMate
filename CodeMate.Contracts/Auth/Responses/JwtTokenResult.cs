namespace CodeMate.Contracts.Auth.Responses;

public sealed class JwtTokenResult
{
    public string Token { get; init; } = string.Empty;

    public DateTimeOffset Expiration { get; init; }
}