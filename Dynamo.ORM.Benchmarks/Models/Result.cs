using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.ORM.Benchmarks.Models
{
    public class Result
    {
        public string Method { get; set; }
        public int Count { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public bool? Simple { get; set; }
        public string Service { get; set; }
        public string Description { get; set; }
    }
}
