using LodashStats.Config;
using LodashStats.UrlBuilders;

namespace LodashStatsTests;

public class GitHubUrlBuilderRepoContentTests
{
    private GitHubUrlBuilderRepoContent _urlBuilder;

    [SetUp]
    public void Setup()
    {
        _urlBuilder = new GitHubUrlBuilderRepoContent(new GitHubRepoOptions { GitHubRepoOwner = "owner", GitHubRepo = "repo" });
    }

    [Test]
    public void WhenOwnerAndRepoAreProvidedWithoutAPathAnExceptionIsRaised()
    {
        var exc = Assert.Throws(typeof(ArgumentException), () => _urlBuilder.BuildUrl());

        Assert.That(exc.Message, Is.EqualTo("paramMap must contain a 'path' entry"));
    }

    [Test]
    public void WhenOwnerAndRepoAreProvidedWithAPathAValidUrlIsBuilt()
    {
        var url = _urlBuilder.BuildUrl(new() {{"path", "thePath"}});

        Assert.That(url, Is.EqualTo("https://api.github.com/repos/owner/repo/contents/thePath"));
    }
}