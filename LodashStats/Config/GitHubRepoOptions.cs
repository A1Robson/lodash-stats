namespace LodashStats.Config;

public record GitHubRepoOptions
{
    public string? GitHubRepoOwner { get; init; }
    public string? GitHubRepo { get; init; }
}
