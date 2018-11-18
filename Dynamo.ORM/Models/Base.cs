using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.ORM.Models
{
    public class Base
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        internal static string GetPropertyReference(string propertyName) => $"#{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}";
    }
}
