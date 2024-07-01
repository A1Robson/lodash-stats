using System.Diagnostics;
using LodashStats.Abstractions;
using LodashStats.Config;
using LodashStats.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LodashStatsRunner;

public class LodashRunner(ILogger<LodashRunner> log, IHostApplicationLifetime appLifetime, IGitHubFileRetrievalService retrievalService, IStatisticsAggregator statisticsAggregator, LodashRunnerOptions options) : IHostedService
{
    private readonly Dictionary<char, string> _specialCharacterMap = new()
    {
        { '\n', "<NL>" },
        { '\r', "<CR>" },
        { '\t', "<TB>" },
        { ' ', "<SP>" }
    };

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await RetrieveFilesAndCompileStatistics();
        }
        finally
        {
            appLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task RetrieveFilesAndCompileStatistics()
    {
        Stopwatch sw = Stopwatch.StartNew();

        int retrievedFileCount = 0;
        await foreach (RetrievedFileInfo retrievedFileInfo in retrievalService.RetrieveFilesAsync(options.FileExtensionsIncPeriod))
        {
            if (retrievedFileInfo.HasValidContent)
            {
                statisticsAggregator.ExtractStatistics(retrievedFileInfo.Content!);
                retrievedFileCount++;
            }
            else
            {
                log.LogWarning("*** Problem retrieving content for {fileName} - not included in statistics", retrievedFileInfo.FileName);
            }
        }

        sw.Stop();

        DisplayStatistics();   

        log.LogInformation("Processed {fileCount} files in {totalMilliseconds}ms: ", retrievedFileCount, sw.Elapsed.TotalMilliseconds);
    }

    private void DisplayStatistics()
    {
        foreach (var (key, value) in statisticsAggregator.GetOrderedStatistics())
        {
            if (!_specialCharacterMap.TryGetValue(key, out var displayKey))
            {
                displayKey = key.ToString();
            }

            Console.WriteLine($"Char: {displayKey} Frequency: {value}");
        }
    }
}