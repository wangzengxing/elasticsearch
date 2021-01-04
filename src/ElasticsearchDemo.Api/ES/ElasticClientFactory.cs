using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticsearchDemo.Api.ES
{
    public class ElasticClientFactory : IElasticClientFactory
    {
        private readonly ElasticClientOptions _options;

        public ElasticClientFactory(IOptions<ElasticClientOptions> optionsAccesser)
        {
            _options = optionsAccesser.Value;
        }

        public ElasticClient CreateClient()
        {
            var settings = new ConnectionSettings(new Uri(_options.Url))
                .DefaultIndex(_options.DefaultIndex);
            var client = new ElasticClient(settings);

            return client;
        }
    }
}
