using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Extensions;
using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dynamo.ORM.UnitTests
{
    public static class AmazonDynamoDBClientTestExtensions
    {
        public static async Task CreateTestTableIfNotExists<T>(this AmazonDynamoDBClient client, string tableName = null) where T : Base, new()
        {
            var generic = new T();

            var tables = await client.ListTablesAsync();

            tableName = tableName ?? generic.GetTableName();

            if (!tables.TableNames.Contains(tableName))
            {
                var keys = generic.GetKey();

                var keySchemaElements = new List<KeySchemaElement>
                {
                    new(keys.First().Key, KeyType.HASH)
                };

                var attributeDefinitions = new List<AttributeDefinition>
                {
                    new (keys.First().Key, GetScalarAttributeType(generic.GetType().GetProperty(keys.First().Key).PropertyType))
                };

                for (int i = 1; i < keys.Count; i++)
                {
                    var keyValuePair = keys.ElementAt(i);

                    keySchemaElements.Add(new(keyValuePair.Key, KeyType.RANGE));
                    attributeDefinitions.Add(new(keyValuePair.Key, GetScalarAttributeType(generic.GetType().GetProperty(keyValuePair.Key).PropertyType)));
                }

                await client.CreateTableAsync(tableName, keySchemaElements, attributeDefinitions, new ProvisionedThroughput(1, 1));
            }
        }

        public static AmazonDynamoDBClient InitializeTestDynamoDbClient()
        {
            var config = new AmazonDynamoDBConfig
            {
                // If you don't have environment credentials setup provide dummy variables here.
                ServiceURL = "http://localhost:8000/"
            };

            return new AmazonDynamoDBClient(config);
        }

        private static ScalarAttributeType GetScalarAttributeType(Type propertyType)
        {
            if (propertyType == typeof(int))
                return ScalarAttributeType.N;

            return ScalarAttributeType.S;
        }
    }
}