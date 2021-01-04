using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchDemo
{
    public class Test
    {
        public static void Insert()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.21:9200"))
                .DefaultIndex("test");
            var client = new ElasticClient(settings);

            var employee = new Employee
            {
                Id = 3,
                FirstName = "Douglas",
                LastName = "Fir",
                Age = 35,
                About = "I like to build cabinets",
                Interests = new string[]
                {
                    "forestry"
                }
            };
            client.IndexDocument(employee);
        }

        public static List<Employee> GetList()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.21:9200"))
                .DefaultIndex("test");
            var client = new ElasticClient(settings);

            var searchResponse = client.Search<Employee>(s => s
                                    .From(0)
                                    .Size(10)
                                    .MatchAll()
                                );
            return searchResponse.Documents.ToList();
        }

        public static async Task<TermsAggregate<string>> Aggregations()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.21:9200"))
                .DefaultIndex("test");
            var client = new ElasticClient(settings);

            var searchResponse = await client.SearchAsync<Employee>(s => s
                            .Size(0)
                            .Aggregations(aggs => aggs
                                //.Average("average_age", avg => avg.Field(f => f.Age))
                                //.Max("max_age", max => max.Field(f => f.Age))
                                //.Min("min_age", min => min.Field(f => f.Age))
                                .Terms("all_interests",t=>t.Field("interests")))
                            );
            var termsAggregation = searchResponse.Aggregations.Terms("all_interests");
            return termsAggregation;
        }

        public static Employee Get(int id)
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.21:9200"))
                .DefaultIndex("test");
            var client = new ElasticClient(settings);

            var response = client.Search<Employee>(r => r
                                  .Query(s => s.Ids(t => t.Values(id))));
            return response.Documents.FirstOrDefault();
        }

        public static List<Employee> Get(string name)
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.21:9200"))
                .DefaultIndex("test");
            var client = new ElasticClient(settings);

            var response = client.Search<Employee>(r => r
                                  .Query(s => s.Match(t => t.Field(x => x.LastName).Query(name)))
                                  .Query(s => s.Range(t => t.Field(x => x.Age).GreaterThanOrEquals(35))));
            return response.Documents.ToList();
        }

        public static List<Employee> Get()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.21:9200"))
                .DefaultIndex("test");
            var client = new ElasticClient(settings);

            var response = client.Search<Employee>(r => r
                                  //.Query(s => s.Match(t => t.Field(x => x.About).Query("rock climbing"))));
                                  .Query(s => s.MatchPhrase(t => t.Field(x => x.About).Query("rock climbing"))));
            return response.Documents.ToList();
        }
    }
}
