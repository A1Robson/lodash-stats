using LodashStats;
using LodashStats.Config;
using LodashStats.Models;
using LodashStats.UrlBuilders;
using Microsoft.Extensions.Logging;

namespace LodashStatsTests;

public class GitHubApiClientTests
{
    // TODO: Some of these tests may trigger Rate Limits - may want to take them out of automated runs.

    private GitHubUrlBuilderRepoContent _urlBuilderRepoContent;
    private GitHubUrlBuilderRepoZip _urlBuilderRepoZip;
    private ILogger<GitHubApiClient> _log;
    private readonly GitHubApiClientOptions _gitHubApiClientOptions = new() { ApiVersion = "", MaxConcurrentRequests = 100, RequestTimeout = 100 };
    private GitHubApiClient _apiClient;

    [SetUp]
    public void Setup()
    {
        var gitHubRepoOptions = new GitHubRepoOptions { GitHubRepoOwner = "lodash", GitHubRepo = "lodash" };

        _urlBuilderRepoContent = new GitHubUrlBuilderRepoContent(gitHubRepoOptions);
        _urlBuilderRepoZip = new GitHubUrlBuilderRepoZip(gitHubRepoOptions);

        _log = new TestLogger<GitHubApiClient>();

        _apiClient = new GitHubApiClient(_log, _urlBuilderRepoContent, _urlBuilderRepoZip, _gitHubApiClientOptions);
    }

    [Test]
    public async Task WhenDownloadingRepositoryZipAZipFileIsSaved()
    {
        string zipDstFilePath = Path.Combine(Directory.GetCurrentDirectory(), "lodashTestArchive.zip");

        File.Delete(zipDstFilePath);
        
        Assert.That(File.Exists(zipDstFilePath), Is.False);

        await _apiClient.DownloadRepositoryZipAsync(zipDstFilePath);

        Assert.That(File.Exists(zipDstFilePath), Is.True);
    }

    [Test]
    public async Task WhenRetrievingLodashContentThereAreOver500Files()
    {
        List<GitHubContent> gitHubContents = await _apiClient.RetrieveRepoContentsAsync();

        Assert.That(gitHubContents, Has.Count.GreaterThan(500));
    }

    [Test]
    public async Task WhenRetrieveIndividualFileRetrievedFileInfoIsValid()
    {
        RetrievedFileInfo retrieveFileInfo = await _apiClient.RetrieveFileAsync("https://raw.githubusercontent.com/lodash/lodash/main/.github/lock.yml");

        Assert.Multiple(() =>
        {
            Assert.That(retrieveFileInfo.HasValidContent, Is.True);
            Assert.That(retrieveFileInfo.FileName, Is.EqualTo("lock.yml"));
            Assert.That(retrieveFileInfo.Content!, Has.Length.GreaterThan(500));
        });
    }

    [Test]
    public async Task WhenRetrieveInvalidIndividualFileRetrievedFileInfoIsNotValid()
    {
        RetrievedFileInfo retrieveFileInfo = await _apiClient.RetrieveFileAsync("https://raw.githubusercontent.com/BAD.XYZ");
        Assert.That(retrieveFileInfo.HasValidContent, Is.False);
    }
}