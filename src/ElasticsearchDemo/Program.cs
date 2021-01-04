using Nest;
using System;

namespace ElasticsearchDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var result=Test.Aggregations().Result;
            Console.WriteLine("Hello World!");
        }
    }
}
