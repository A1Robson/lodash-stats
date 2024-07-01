using LodashStats.Abstractions;
using LodashStats.Config;

namespace LodashStats.UrlBuilders;

public class GitHubUrlBuilderRepoZip(GitHubRepoOptions options) : IGitHubUrlBuilder
{
    public string BuildUrl(Dictionary<string, string>? paramMap = null)
    {
        return $"https://api.github.com/repos/{options.GitHubRepoOwner}/{options.GitHubRepo}/zipball/"; //ref 
    }
}