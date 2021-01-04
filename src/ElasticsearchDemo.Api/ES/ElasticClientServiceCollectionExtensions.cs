using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticsearchDemo.Api.ES
{
    public static class ElasticClientServiceCollectionExtensions
    {
        public static IServiceCollection AddElasticClient(this IServiceCollection services, Action<ElasticClientOptions> optionsBuilder)
        {
            services.Configure(optionsBuilder);

            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            services.AddScoped<IElasticClient, ElasticClient>(provider => provider.GetService<IElasticClientFactory>().CreateClient());

            return services;
        }

        public static IServiceCollection AddElasticClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ElasticClientOptions>(configuration.GetSection(nameof(ElasticClient)));

            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            services.AddScoped<IElasticClient, ElasticClient>(provider => provider.GetService<IElasticClientFactory>().CreateClient());

            return services;
        }
    }
}
