using LodashStats.Abstractions;
using LodashStats.Config;
using LodashStats.Models;
using Microsoft.Extensions.Logging;

namespace LodashStats.Services;

public class GitHubFileRetrievalServiceZip(ILogger<GitHubFileRetrievalServiceZip> log, IGitHubApiClient gitHubApiClient, IZipFileExtractor zipFileExtractor, LodashRunnerOptions options) : IGitHubFileRetrievalService
{
    public async IAsyncEnumerable<RetrievedFileInfo> RetrieveFilesAsync(List<string>? fileExtensionsIncPeriod)
    {
        string zipFileName = Path.ChangeExtension(Path.GetRandomFileName(), ".zip");
        string zipFilePath = Path.Combine(Path.GetTempPath(), zipFileName);
        string extractPath = Path.Combine(Path.GetDirectoryName(zipFilePath)!, $"{Path.GetFileNameWithoutExtension(zipFilePath)}_extracted");

        if (await gitHubApiClient.DownloadRepositoryZipAsync(zipFilePath))
        {
            try
            {
                zipFileExtractor.ExtractToDirectory(zipFilePath, extractPath);

                Func<string, bool> validExtension = fileExtensionsIncPeriod == null || fileExtensionsIncPeriod.Count == 0 ? _ => true : fileName => fileExtensionsIncPeriod.Contains(Path.GetExtension(fileName));

                foreach (var filePath in Directory.EnumerateFiles(extractPath, "*", SearchOption.AllDirectories))
                {
                    if (validExtension(filePath))
                    {
                        var fileName = Path.GetFileName(filePath);

                        var content = await File.ReadAllTextAsync(filePath);

                        yield return new RetrievedFileInfo(fileName, content);
                    }
                }
            }
            finally
            {
                await CleanupAsync(zipFilePath, extractPath);
            }
        }
    }

    private async Task CleanupAsync(string zipPath, string extractPath)
    {
        await Task.Run(() =>
        {
            try
            {
                if (options.DeleteDownloadedZip && File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, recursive: true);
                }
            }
            catch (Exception e)
            {
                log.LogError(e, "Directory Cleanup failed");
            }
        });
    }
}
