namespace LodashStats.Abstractions;

public interface IZipFileExtractor
{
    void ExtractToDirectory(string srcArchiveName, string dstDirectoryName);
}