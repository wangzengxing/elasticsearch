using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticsearchDemo.Api.ES
{
    public class ElasticClientOptions
    {
        public string Url { get; set; }
        public string DefaultIndex { get; set; }
    }
}
