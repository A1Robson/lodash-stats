using LodashStats;
using LodashStats.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LodashStatsRunner
{
    // FUTURE: Add option to use api auth
    // FUTURE: accept sha1 to select which branch

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
