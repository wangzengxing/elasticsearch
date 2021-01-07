using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchDemo
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
