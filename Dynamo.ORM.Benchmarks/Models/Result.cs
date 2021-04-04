using System;

namespace Dynamo.ORM.Benchmarks.Models
{
    public class Result
    {
        public int Count { get; set; }
        public string Description { get; set; }
        public string Method { get; set; }
        public string Service { get; set; }
        public bool? Simple { get; set; }
        public TimeSpan TimeTaken { get; set; }
    }
}