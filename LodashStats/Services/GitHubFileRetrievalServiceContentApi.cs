using LodashStats.Abstractions;
using LodashStats.Config;
using LodashStats.Models;
using Microsoft.Extensions.Logging;

namespace LodashStats.Services;

public class GitHubFileRetrievalServiceContentApi(ILogger<GitHubFileRetrievalServiceContentApi> log, IGitHubApiClient gitHubApiClient, LodashRunnerOptions options) : IGitHubFileRetrievalService
{

    public async IAsyncEnumerable<RetrievedFileInfo> RetrieveFilesAsync(List<string>? fileExtensionsIncPeriod)
    {
        Func<string, bool> validExtension = fileExtensionsIncPeriod == null || fileExtensionsIncPeriod.Count == 0 ? _ => true : fileName => fileExtensionsIncPeriod.Contains(Path.GetExtension(fileName));

        List<GitHubContent> gitHubContents = await gitHubApiClient.RetrieveRepoContentsAsync();

        int totalItems = gitHubContents.Count;

        var pageSize = options.ProcessingPageSize == 0 ? totalItems : options.ProcessingPageSize;

        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        for (int page = 0; page < totalPages; page++)
        {
            log.LogInformation("Processing Page: {pagePlusOne} of {totalPages}", page + 1, totalPages);

            var currentPageItems = gitHubContents.Skip(page * pageSize).Take(pageSize).ToList();

            List<Task<RetrievedFileInfo>> tasks = [];

            foreach (var gitHubContent in currentPageItems.Where(c => validExtension(c.Name!)))
            {
                Task<RetrievedFileInfo> retrieveFileAsync = gitHubApiClient.RetrieveFileAsync(gitHubContent.Download_Url!);

                tasks.Add(retrieveFileAsync);
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            foreach (RetrievedFileInfo result in results)
            {
                yield return new RetrievedFileInfo(result.FileName, result.Content);
            }
        }
    }
}