using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticsearchDemo.Api.ES
{
    public interface IElasticClientFactory
    {
        ElasticClient CreateClient();
    }
}
