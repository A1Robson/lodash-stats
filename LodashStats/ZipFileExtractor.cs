using System.IO.Compression;
using LodashStats.Abstractions;
using Microsoft.Extensions.Logging;

namespace LodashStats;

public class ZipFileExtractor(ILogger<ZipFileExtractor> log) : IZipFileExtractor
{
    public void ExtractToDirectory(string srcArchiveName, string dstDirectoryName)
    {
        ZipFile.ExtractToDirectory(srcArchiveName, dstDirectoryName);

        log.LogInformation("Repo Zip extracted to {extractPath}", dstDirectoryName);
    }
}