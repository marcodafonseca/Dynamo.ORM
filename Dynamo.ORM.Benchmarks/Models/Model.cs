using Amazon.DynamoDBv2.DataModel;
using Dynamo.ORM.Models;

namespace Dynamo.ORM.Benchmarks.Models
{
    [DynamoDBTable("Benchmarking")]
    public class Model : Base
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        public string Property1 { get; set; } = "TEST";
        public string Property2 { get; set; } = "TEST";
        public string Property3 { get; set; } = "TEST";
        public string Property4 { get; set; } = "TEST";
    }
}