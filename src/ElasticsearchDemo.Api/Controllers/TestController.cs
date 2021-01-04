using ElasticsearchDemo.Api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticsearchDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IElasticClient _elasticClient;

        public TestController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public IActionResult Get()
        {
            var result = _elasticClient.Search<Employee>(r => r.
                  MatchAll());
            return Ok(result.Documents.ToList());
        }
    }
}
