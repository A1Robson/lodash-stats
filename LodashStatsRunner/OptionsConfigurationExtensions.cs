using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LodashStatsRunner;

// TODO: Move into separate library
public static class OptionsConfigurationExtensions
{
    public static IHostApplicationBuilder ConfigureOptions<TOptions>(this IHostApplicationBuilder builder) 
        where TOptions : class
    {
        builder.Services
            .Configure<TOptions>(builder.Configuration.GetSection(typeof(TOptions).Name))
            .AddSingleton(provider => provider.GetRequiredService<IOptions<TOptions>>().Value); // Also register the direct object 
            
        return builder;
    }
}