using LodashStats.Abstractions;
using LodashStats.Config;

namespace LodashStats.UrlBuilders;

public class GitHubUrlBuilderRepoContent(GitHubRepoOptions options) : IGitHubUrlBuilder
{
    public string BuildUrl(Dictionary<string, string>? paramMap = null)
    {
        paramMap ??= [];

        if (!paramMap.TryGetValue("path", out string? path))
        {
            throw new ArgumentException("paramMap must contain a 'path' entry");
        }

        return $"https://api.github.com/repos/{options.GitHubRepoOwner}/{options.GitHubRepo}/contents/{path}";
    }
}