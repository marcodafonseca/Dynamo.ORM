using Amazon.DynamoDBv2.DataModel;
using Dynamo.ORM.Models;
using System;

namespace Dynamo.ORM.UnitTests.Services
{
    [DynamoDBTable("TESTS")]
    class TestModel : Base
    {
        private DateTime property2;
        private DateTime? property11;

        [DynamoDBHashKey]
        public int Id { get; set; }
        public string Property1 { get; set; }
        public DateTime Property2
        {
            get
            {
                return property2.ToUniversalTime();
            }
            set
            {
                property2 = value;
            }
        }
        public int Property3 { get; set; }
        public bool Property4 { get; set; }
        public byte Property5 { get; set; }
        public byte? Property6 { get; set; }
        public byte[] Property7 { get; set; }
        public bool? Property8 { get; set; }
        public char Property9 { get; set; }
        public char? Property10 { get; set; }
        public DateTime? Property11
        {
            get
            {
                return property11?.ToUniversalTime();
            }
            set
            {
                property11 = value;
            }
        }
        public decimal Property12 { get; set; }
        public decimal? Property13 { get; set; }
        public double Property14 { get; set; }
        public double? Property15 { get; set; }
        public float Property16 { get; set; }
        public float? Property17 { get; set; }
        public short Property18 { get; set; }
        public short? Property19 { get; set; }
        public long Property20 { get; set; }
        public long? Property21 { get; set; }
        public ushort Property22 { get; set; }
        public ushort? Property23 { get; set; }
        public ulong Property24 { get; set; }
        public ulong? Property25 { get; set; }
        public uint Property26 { get; set; }
        public uint? Property27 { get; set; }
    }
}
