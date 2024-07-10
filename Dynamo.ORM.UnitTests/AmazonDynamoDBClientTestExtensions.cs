using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dynamo.ORM.UnitTests
{
    public static class AmazonDynamoDBClientTestExtensions
    {
        public static async Task CreateTestTableIfNotExists(this AmazonDynamoDBClient client, string tableName)
        {
            var tables = await client.ListTablesAsync();
            if (!tables.TableNames.Contains(tableName))
            {
                await client.CreateTableAsync(tableName, new List<KeySchemaElement> {
                    new KeySchemaElement("Id", KeyType.HASH)
                }, new List<AttributeDefinition> {
                    new AttributeDefinition("Id", ScalarAttributeType.N)
                }, new ProvisionedThroughput(1, 1));
            }
        }

        public static AmazonDynamoDBClient InitializeTestDynamoDbClient()
        {
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000/"
            };
            return new AmazonDynamoDBClient(config);// If you don't have environment credentials setup provide dummy variables here.
        }
    }
}