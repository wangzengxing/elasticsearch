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
        #region 基础
        public static void Index()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Age=20,
                    CreateTime=DateTime.Now,
                    Name="jack"
                },
                new Person
                {
                    Age=30,
                    CreateTime=DateTime.Now,
                    Name="alice"
                },
                new Person
                {
                    Age=27,
                    CreateTime=DateTime.Now,
                    Name="bob"
                }
            };

            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            foreach (var person in persons)
            {
                var response = client.IndexDocument(person);
                if (response.Result != Result.Created)
                {
                    throw new Exception();
                }
            }
        }

        public static void Delete()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            var response = client.DeleteByQuery<Person>(r => r.MatchAll());
        }
        #endregion

        #region 查找
        /// <summary>
        /// 精确查找
        /// </summary>
        public static void Term()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            //SELECT * FROM Person WHERE Introduce = "hello";
            var response = client.Search<Person>(r => r
                .Query(s => s
                    //.Term(t => t.Introduce, "hello")//带评分
                    .ConstantScore(t => t.Filter(x => x//不带评分
                                                       //.Term(y => y.Age, 25)))//数字
                                                       //.Term(y => y.IsDeleted, false)))//bool
                                                       //.Term(y=>y.CreateTime,DateTime.Now)//datetime
                        .Term(y => y.Name, "hello")))//text
                    ));
            var result = response.Documents;
        }

        /// <summary>
        /// 过滤查找
        /// </summary>
        public static void Bool()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            //SELECT * FROM Person WHERE (Age = 25 OR Introduce = "hello") AND Age != 30;
            //var response = client.Search<Person>(r => r
            //      .Query(s => s.Bool(t => t.Filter(x => x.Bool(y => y
            //                                          .Should(z => z.Term(f => f.Age, 25))
            //                                          .Should(z => z.Term(f => f.Introduce, "hello"))))
            //                                .MustNot(x => x.Term(f => f.Age, 30)))));

            //SELECT * FROM Person WHERE Introduce = "hello" OR (Introduce = "world AND Age = 30");
            var response = client.Search<Person>(r => r
                      .Query(s => s.Bool(t => t.Should(x => x.Term(f => f.Name, "hello"))
                                            .Should(x => x.Bool(y => y.Must(z => z.Term(f => f.Name, "world"))
                                                                  .Must(z => z.Term(f => f.Age, 30))))
                                          )));
            var result = response.Documents;
        }

        /// <summary>
        /// 多值查找
        /// </summary>
        public static void Terms()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            //SELECT * FROM Person WHERE Age IN (25,26,27);
            var response = client.Search<Person>(r => r
                .Query(s => s
                    //.Terms(t => t.Field(f => f.Age)
                    //            .Terms(new int[] { 25, 26, 27 }))//带评分
                    .ConstantScore(t => t.Filter(x => x//不带评分
                        .Terms(y => y.Field(f => f.Age)
                                    .Terms(new int[] { 25, 26, 27 }))))
                    ));
            var result = response.Documents;
        }

        /// <summary>
        /// 范围查找
        /// </summary>
        public static void Range()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            //SELECT * FROM Person WHERE Age BETWEEN 20 AND 40;
            var response = client.Search<Person>(r => r
                .Query(s => s
                    .ConstantScore(t => t.Filter(x => x//不带评分
                        .Range(y => y.Field(f => f.Age)
                                    .GreaterThanOrEquals(20)
                                    .LessThanOrEquals(40))))
                    ));
            var result = response.Documents;
        }

        /// <summary>
        /// 非空查找
        /// </summary>
        public static void Exists()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            //SELECT * FROM Person WHERE Introduce IS NOT NULL;
            //var response = client.Search<Person>(r => r
            //    .Query(s => s
            //        .ConstantScore(t => t.Filter(x => x//不带评分
            //            .Exists(y => y.Field(f => f.Introduce))))
            //        ));

            //SELECT * FROM Person WHERE Introduce IS NULL;
            var response = client.Search<Person>(r => r
                .Query(s => s
                    .Bool(t => t.MustNot(x => x
                              .Exists(y => y.Field(f => f.Name))))
                    ));
            var result = response.Documents;
        }

        /// <summary>
        /// 词语查询
        /// </summary>
        public static void Match()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            //底层使用Term查询
            var response = client.Search<Person>(r => r
                .Query(s => s
                    .Match(t => t.Field(f => f.Name).Query("hello world")
                                                        //.Operator(Operator.And))//多词精确查找
                                                        //.MinimumShouldMatch(MinimumShouldMatch.Percentage(75)))//75%匹配
                                                        .Boost(3))//权重
                    ));
            var result = response.Documents;
        }

        /// <summary>
        /// 短语匹配
        /// </summary>
        public static void MatchPhrase()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            var response = client.Search<Person>(r => r
                .Query(s => s
                    .MatchPhrase(t => t.Field(f => f.Name).Query("hello world"))
                    ));
            var result = response.Documents;
        }
        #endregion

        #region 聚合
        /// <summary>
        /// 聚合分组
        /// </summary>
        public static void AggregationsTerms()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            //PUT person/_mapping
            //  {
            //      "properties": 
            //      {
            //          "name": 
            //          {
            //              "type":     "text",
            //              "fielddata": true
            //          }
            //      }
            //  }

            var response = client.Search<Person>(r => r
                .Aggregations(s => s
                    .Terms("igroup", t => t.Field(f => f.Name))
                    ));
            var result = response.Aggregations.Terms("igroup");
        }

        /// <summary>
        /// 条形图
        /// </summary>
        public static void AggregationsHistogram()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            var response = client.Search<Person>(r => r
                .Aggregations(s => s
                    .Histogram("igroup", t => t.Field(f => f.Age).Interval(10)
                        .Aggregations(x => x
                            .Average("avg_age", t => t.Field(f => f.Age))))
                    ));
            var result = response.Aggregations.Histogram("igroup");
        }

        /// <summary>
        /// 时间统计
        /// </summary>
        public static void AggregationsDataHistogram()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            var response = client.Search<Person>(r => r
                .Aggregations(s => s
                    .DateHistogram("igroup", t => t.Field(f => f.CreateTime).CalendarInterval(DateInterval.Day)
                                                                        .Format("yyyy-MM-dd")
                                                                        .MinimumDocumentCount(0)
                                                                        .ExtendedBounds("2021-01-01", "2021-01-31")
                        .Aggregations(x => x
                            .Average("avg_age", t => t.Field(f => f.Age))))
                    ));
            var result = response.Aggregations.DateHistogram("igroup");
        }

        /// <summary>
        /// 去重统计
        /// </summary>
        public static void AggregationsCardinality()
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.60.20:9200"))
                .DefaultIndex("person");
            var client = new ElasticClient(settings);

            var response = client.Search<Person>(r => r
                .Size(0)
                .Aggregations(s => s
                    .Cardinality("distinct_ages", t => t.Field(f => f.Age))
                    ));
            var result = response.Aggregations.Cardinality("distinct_ages");
        }
        #endregion
    }
}
