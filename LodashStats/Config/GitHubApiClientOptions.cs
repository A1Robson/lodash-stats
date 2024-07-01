namespace LodashStats.Config;

public record GitHubApiClientOptions
{
    public int RequestTimeout { get; init; }
    public string? ApiVersion { get; init; }
    public int MaxConcurrentRequests { get; init; }
}