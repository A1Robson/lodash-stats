using LodashStats.Config;
using LodashStats.UrlBuilders;

namespace LodashStatsTests;

public class GitHubUrlBuilderRepoZipTests
{
    private GitHubUrlBuilderRepoZip _urlBuilder;

    [SetUp]
    public void Setup()
    {
        _urlBuilder = new GitHubUrlBuilderRepoZip(new GitHubRepoOptions { GitHubRepoOwner = "owner", GitHubRepo = "repo" });
    }

    [Test]
    public void WhenOwnerAndRepoAreProvidedAValidUrlIsBuilt()
    {
        var url = _urlBuilder.BuildUrl();

        Assert.That(url, Is.EqualTo("https://api.github.com/repos/owner/repo/zipball/"));
    }
}