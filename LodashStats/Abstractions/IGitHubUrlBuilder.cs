namespace LodashStats.Abstractions;

public interface IGitHubUrlBuilder
{
    string BuildUrl(Dictionary<string, string>? paramMap = null);
}