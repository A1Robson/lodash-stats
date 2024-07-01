using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using LodashStats.Abstractions;
using LodashStats.Config;
using LodashStats.Models;
using LodashStats.UrlBuilders;
using Microsoft.Extensions.Logging;

namespace LodashStats;

public class GitHubApiClient : IGitHubApiClient
{
    private readonly ILogger<GitHubApiClient> _log;
    private readonly GitHubUrlBuilderRepoContent _urlBuilderRepoContent;
    private readonly GitHubUrlBuilderRepoZip _urlBuilderRepoZip;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public GitHubApiClient(ILogger<GitHubApiClient> log, GitHubUrlBuilderRepoContent urlBuilderRepoContent, GitHubUrlBuilderRepoZip urlBuilderRepoZip, GitHubApiClientOptions options)
    {
        _log = log;
        _urlBuilderRepoContent = urlBuilderRepoContent;
        _urlBuilderRepoZip = urlBuilderRepoZip;
        _httpClient = new HttpClient(new SocketsHttpHandler { MaxConnectionsPerServer = options.MaxConcurrentRequests })
        {
            Timeout = TimeSpan.FromSeconds(options.RequestTimeout)
        };

        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("LodashStatsClient", "1.0"));
        _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", [options.ApiVersion]);
    }

    /// <summary>
    /// This API has an upper limit of 1,000 files for a directory. If you need to retrieve more files, use the Git Trees API (or zip)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public async Task<List<GitHubContent>> RetrieveRepoContentsAsync(string path = "")
    {
        List<GitHubContent> fileContent = [];

        string requestUrl = _urlBuilderRepoContent.BuildUrl(new() { { "path", path } });

        HttpResponseMessage response = await _httpClient.GetAsync(requestUrl).ConfigureAwait(false);

        CheckRemainingRateLimits(response.Headers);

        if (response.StatusCode is not (HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests))
        {
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            List<GitHubContent> contents = JsonSerializer.Deserialize<List<GitHubContent>>(responseContent, options: _options) ?? [];

            foreach (var content in contents)
            {
                if (content.Type == "file")
                {
                    fileContent.Add(content);
                }
                else if (content.Type == "dir")
                {
                    fileContent.AddRange(await RetrieveRepoContentsAsync(content.Path!).ConfigureAwait(false));
                }
            }
        }
        else
        {
            _log.LogInformation("Unable to RetrieveRepoContentsAsync - Reason Phrase: {reasonPhrase}", response.ReasonPhrase);

            if (int.TryParse(response.Headers.GetValues("X-RateLimit-Reset").FirstOrDefault(), out int reset))
            {
                var resetTime = DateTimeOffset.FromUnixTimeSeconds(reset).DateTime;

                var sleepDuration = resetTime - DateTime.UtcNow;

                if (sleepDuration > TimeSpan.Zero)
                {
                    _log.LogInformation("Rate limit exceeded. Retry in {totalMinutes:F} minutes.", sleepDuration.TotalMinutes);
                }
            }
        }

        return fileContent;
    }

    public async Task<RetrievedFileInfo> RetrieveFileAsync(string downloadUrl)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(downloadUrl).ConfigureAwait(false);

        string? result = null;

        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        else
        {
            _log.LogWarning("RetrieveFileAsync was not successful for {downloadUrl}", downloadUrl);
        }

        return new RetrievedFileInfo(Path.GetFileName(downloadUrl), result);
    }

    public async Task<bool> DownloadRepositoryZipAsync(string zipDstFilePath)
    {
        bool result = false;

        try
        {
            string srcUrl = _urlBuilderRepoZip.BuildUrl();

            var request = new HttpRequestMessage(HttpMethod.Get, srcUrl); // request.Headers.Add("Authorization", $"token {accessToken}");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                await using var fileStream = new FileStream(zipDstFilePath, FileMode.Create, FileAccess.Write);
                await response.Content.CopyToAsync(fileStream);

                _log.LogInformation("Repo Zip for {zipSrcUrl} download to {zipFilePath}", srcUrl, zipDstFilePath);

                result = true;
            }
            else
            {
                _log.LogWarning("Failed to download repository zip. Status code: {statusCode}", response.StatusCode);
            }
        }
        catch (Exception e)
        {
            _log.LogError(e, "Exception while downloading repository zip");
        }

        return result;
    }

    private void CheckRemainingRateLimits(HttpResponseHeaders headers)
    {
        if (int.TryParse(headers.GetValues("X-RateLimit-Remaining").FirstOrDefault(), out int remaining))
        { 
            _log.LogInformation("Remaining Rate limit {remaining}", remaining);
        }
    }
}