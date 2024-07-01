using LodashStats.Models;

namespace LodashStats.Abstractions;

public interface IGitHubApiClient
{
    Task<bool> DownloadRepositoryZipAsync(string zipDstFilePath);
    Task<List<GitHubContent>> RetrieveRepoContentsAsync(string path = "");
    Task<RetrievedFileInfo> RetrieveFileAsync(string downloadUrl);
}