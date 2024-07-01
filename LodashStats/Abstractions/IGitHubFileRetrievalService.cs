using LodashStats.Models;

namespace LodashStats.Abstractions;

public interface IGitHubFileRetrievalService
{
    IAsyncEnumerable<RetrievedFileInfo> RetrieveFilesAsync(List<string>? fileExtensionsIncPeriod);
}