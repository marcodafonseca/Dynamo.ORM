using Amazon.DynamoDBv2.DataModel;
using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Dynamo.ORM.UnitTests.Services
{
    public class ChildModel
    {
        public string Property1 { get; set; }
    }

    [DynamoDBTable("TESTS")]
    internal class TestModel : Base
    {
        private DateTime? property11;
        private DateTime property2;

        [DefaultValue(null)]
        public string EmptyString1 { get; set; }

        [DefaultValue("")]
        public string EmptyString2 { get; set; }

        [DynamoDBHashKey]
        public int Id { get; set; }

        public string Property1 { get; set; }
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

        public long Property20 { get; set; }
        public long? Property21 { get; set; }
        public ushort Property22 { get; set; }
        public ushort? Property23 { get; set; }
        public ulong Property24 { get; set; }
        public ulong? Property25 { get; set; }
        public uint Property26 { get; set; }
        public uint? Property27 { get; set; }
        public Guid Property28 { get; set; }
        public Guid? Property29 { get; set; }
        public int Property3 { get; set; }
        public string[] Property30 { get; set; }
        public IList<string> Property31 { get; set; }
        public IList<ChildModel> Property32 { get; set; }
        public int[] Property33 { get; set; }
        public IList<int> Property34 { get; set; }
        public bool Property4 { get; set; }
        public byte Property5 { get; set; }
        public byte? Property6 { get; set; }
        public byte[] Property7 { get; set; }
        public bool? Property8 { get; set; }
        public char Property9 { get; set; }
    }
}