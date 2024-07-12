using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Dynamo.ORM.Models;
using Dynamo.ORM.Services;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Dynamo.ORM.UnitTests.Services
{
    public class RealWorldTests : IClassFixture<SharedFixture>
    {
        private readonly AmazonDynamoDBClient client;
        private readonly SharedFixture sharedFixture;
        private readonly string tableName;

        public RealWorldTests(SharedFixture sharedFixture)
        {
            tableName = DateTime.UtcNow.Ticks + "-Resources";

            client = AmazonDynamoDBClientTestExtensions.InitializeTestDynamoDbClient();
            client.CreateTestTableIfNotExists<Resource>(tableName).Wait();

            this.sharedFixture = sharedFixture;
        }

        [Fact]
        public async Task TestDictionaryProperty_ExpectPropertyReturned()
        {
            var repository = new Repository(client);

            var randomIndex = sharedFixture.RandomInt();

            var resource = new Resource
            {
                Author = "author" + randomIndex,
                Body = "body" + randomIndex,
                Id = randomIndex,
                Name = "name" + randomIndex,
                SEO = new SEO
                {
                    Description = "Description",
                    Title = "Title",
                    Metadata = new Dictionary<string, string>
                    {
                        { "key1", "value1" },
                        { "key2", "value2" }
                    }
                },
                Thumbnail = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAIAAADTED8xAAADMElEQVR4nOzVwQnAIBQFQYXff81RUkQCOyDj1YOPnbXWPmeTRef+/3O/OyBjzh3CD95BfqICMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMO0TAAD//2Anhf4QtqobAAAAAElFTkSuQmCC"
            };

            await repository.Add(resource, tableName: tableName);

            var newModel = await repository.Get<Resource>(resource.Id, tableName: tableName);

            Assert.NotNull(newModel);
        }

        [Fact]
        public async Task TestGetTableSize()
        {
            var repository = new Repository(client);

            for (var i = 0; i < 5; i++)
            {
                var randomIndex = sharedFixture.RandomInt();

                var resource = new Resource
                {
                    Author = "author" + randomIndex,
                    Body = "body" + randomIndex,
                    Id = randomIndex,
                    Name = "name" + randomIndex,
                    SEO = new SEO
                    {
                        Description = "Description",
                        Title = "Title",
                        Metadata = new Dictionary<string, string>
                    {
                        { "key1", "value1" },
                        { "key2", "value2" }
                    }
                    },
                    Thumbnail = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAIAAADTED8xAAADMElEQVR4nOzVwQnAIBQFQYXff81RUkQCOyDj1YOPnbXWPmeTRef+/3O/OyBjzh3CD95BfqICMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMO0TAAD//2Anhf4QtqobAAAAAElFTkSuQmCC"
                };

                await repository.Add(resource, tableName: tableName);
            }

            var itemCount = await repository.GetTableSize<Resource>(tableName: tableName);

            Assert.Equal(5, itemCount);
        }

        [Fact]
        public async Task TestPagination()
        {
            var repository = new Repository(client);

            for (var i = 0; i < 5; i++)
            {
                var randomIndex = sharedFixture.RandomInt();

                var resource = new Resource
                {
                    Author = "author" + randomIndex,
                    Body = "body" + randomIndex,
                    Id = randomIndex,
                    Name = "name" + randomIndex,
                    SEO = new SEO
                    {
                        Description = "Description",
                        Title = "Title",
                        Metadata = new Dictionary<string, string>
                    {
                        { "key1", "value1" },
                        { "key2", "value2" }
                    }
                    },
                    Thumbnail = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAIAAADTED8xAAADMElEQVR4nOzVwQnAIBQFQYXff81RUkQCOyDj1YOPnbXWPmeTRef+/3O/OyBjzh3CD95BfqICMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMO0TAAD//2Anhf4QtqobAAAAAElFTkSuQmCC"
                };

                await repository.Add(resource, tableName: tableName);
            }

            var newModel = await repository.List<Resource>(null, tableName: tableName, 2, 4);

            Assert.NotNull(newModel);
            Assert.Single(newModel);
        }
    }

    [DynamoDBTable("ResourceTests")]
    public class Resource : Base
    {
        public string Author { get; set; } = string.Empty;

        public string? Body { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [DynamoDBHashKey]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("seo")]
        public SEO? SEO { get; set; }

        public string? Thumbnail { get; set; }
    }

    public class SEO
    {
        public string? Description { get; set; }
        public IDictionary<string, string>? Metadata { get; set; }
        public string? Title { get; set; }
    }
}