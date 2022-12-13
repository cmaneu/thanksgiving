namespace api.Services;

public record AuthToken
{
    public required TokenType Type { get; init; }
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required TimeSpan Validity { get; set; }
    public DateTime ValidUntil => CreatedAt + Validity;

    public string UserName { get; internal set; }
}
