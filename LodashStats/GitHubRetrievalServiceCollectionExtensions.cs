using LodashStats.Abstractions;
using LodashStats.Config;
using LodashStats.Services;
using LodashStats.UrlBuilders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LodashStats;

public static class GitHubRetrievalServiceCollectionExtensions
{
    public static IServiceCollection AddGitHubRetrievalService(this IServiceCollection services)
    {
        services.AddSingleton<IStatisticsAggregator, StatisticsAggregator>();
        services.AddSingleton<GitHubUrlBuilderRepoContent>();
        services.AddSingleton<GitHubUrlBuilderRepoZip>();
        services.AddSingleton<IGitHubApiClient, GitHubApiClient>();
        services.AddSingleton<IZipFileExtractor, ZipFileExtractor>();
        services.AddSingleton<IGitHubFileRetrievalService>(provider =>
        { 
            LodashRunnerOptions options = provider.GetRequiredService<LodashRunnerOptions>();

            IGitHubApiClient gitHubApiClient = provider.GetRequiredService<IGitHubApiClient>();

            if (options.UseContentApi)
            {
                return new GitHubFileRetrievalServiceContentApi(provider.GetRequiredService<ILogger<GitHubFileRetrievalServiceContentApi>>(), gitHubApiClient, options);
            }
            else
            {
                return new GitHubFileRetrievalServiceZip(provider.GetRequiredService<ILogger<GitHubFileRetrievalServiceZip>>(), gitHubApiClient, provider.GetRequiredService<IZipFileExtractor>(), options);
            }
        });

        return services;
    }
}