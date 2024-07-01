using LodashStats.Abstractions;
using LodashStats.Config;
using LodashStats.Services;
using Moq;

namespace LodashStatsTests;

public class GitHubFileRetrievalServiceZipTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test1()
    {
        var mockHttpClient = new Mock<IGitHubApiClient>();
        mockHttpClient.Setup(client => client.DownloadRepositoryZipAsync(It.IsAny<string>()));

        var mockZipFileExtractor = new Mock<IZipFileExtractor>();
        mockZipFileExtractor.Setup(zfe => zfe.ExtractToDirectory(It.IsAny<string>(), It.IsAny<string>()));

        LodashRunnerOptions lodashRunnerOptions = new() { DeleteDownloadedZip = true };
            
        var gitHubFileRetrievalServiceZip = new GitHubFileRetrievalServiceZip(new TestLogger<GitHubFileRetrievalServiceZip>(), mockHttpClient.Object, mockZipFileExtractor.Object, lodashRunnerOptions);

        await foreach (var (fileName, content) in gitHubFileRetrievalServiceZip.RetrieveFilesAsync(null))
        {
                
        }

        mockHttpClient.Verify();
        mockZipFileExtractor.Verify();
    }
}