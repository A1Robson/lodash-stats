namespace LodashStats.Config;

public record LodashRunnerOptions
{
    public bool UseContentApi { get; init; }
    public int ProcessingPageSize { get; init; }
    public bool DeleteDownloadedZip { get; init; }
    public List<string>? FileExtensionsIncPeriod { get; init; }
}