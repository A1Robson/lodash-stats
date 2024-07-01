using LodashStats;
using LodashStats.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LodashStatsRunner
{
    // FUTURE: Add option to use api auth
    // FUTURE: accept sha1 to select which branch
    // TODO: have 4 testfiles to use to confirm counts
    // TODO: tidy api version - esp handling retry
    // TODO: change tmp path storage location to current directory
    // TODO: interface for ZipFile

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder
                .ConfigureOptions<LodashRunnerOptions>()
                .ConfigureOptions<GitHubRepoOptions>()
                .ConfigureOptions<GitHubApiClientOptions>();
            
            builder.Services
                .AddGitHubRetrievalService()
                .AddHostedService<LodashRunner>();
            
            using IHost host = builder.Build();

            await host.RunAsync();
        }
    }
}
