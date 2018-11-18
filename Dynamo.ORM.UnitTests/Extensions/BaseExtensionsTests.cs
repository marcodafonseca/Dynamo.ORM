using Amazon.DynamoDBv2.DataModel;
using Dynamo.ORM.Extensions;
using Dynamo.ORM.Models;
using Dynamo.ORM.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Amazon.DynamoDBv2.Model;

namespace Dynamo.ORM.UnitTests.Extensions
{
    public class BaseExtensionsTests
    {
        /// <summary>
        /// Test that table name is returned when DynamoDBTable is assigned
        /// </summary>
        [Fact]
        public void TestGetTableName_ExpectTestTableName()
        {
            var entity = new MockTable();
            var tableName = entity.GetTableName();

            Assert.Equal("TestTableName", tableName);
        }

        /// <summary>
        /// Test that exception is thrown when DynamoDBTable attribute is not assigned
        /// </summary>
        [Fact]
        public void TestGetTableName_ExpectException()
        {
            var entity = new MockNoNameTable();

            try
            {
                var tableName = entity.GetTableName();
            }
            catch (TableAttributeException)
            {
                Assert.True(true);
                return;
            }

            Assert.True(false, "Test Class has DynamoDBTable attribute");
        }

        /// <summary>
        /// Test that dictionary is populated with ExpressionAttributes based on class setup
        /// private property is to be ignored
        /// </summary>
        [Fact]
        public void TestExpressionAttributes_ExpectDictionary()
        {
            var entity = new MockTable();

            var dictionary = entity.GetExpressionAttributes();

            Assert.Equal(3, dictionary.Count);
            Assert.True(dictionary.ContainsKey("#id"));
            Assert.True(dictionary.ContainsKey("#property1"));
            Assert.True(dictionary.ContainsKey("#property2"));
            Assert.False(dictionary.ContainsKey("#property3"));
        }

        /// <summary>
        /// Test that AttributeValues are correctly mapped to Object
        /// </summary>
        [Fact]
        public void TestMapping_ExpectPopulatedObject()
        {
            var dictionary = new Dictionary<string, AttributeValue>
            {
                { "Property1", new AttributeValue("TEST") },
                { "Property2", new AttributeValue{ BOOL = true } },
                { "Id", new AttributeValue{ N = "1" } }
            };

            var entity = dictionary.Map<MockTable>();

            Assert.Equal(1, entity.Id);
            Assert.Equal("TEST", entity.Property1);
            Assert.True(entity.Property2);
        }
    }

    [DynamoDBTable("TestTableName")]
    public class MockTable : Base
    {
        public string Property1 { get; set; }
        public bool Property2 { get; set; }

        /// <summary>
        /// To be ignored during reflection
        /// </summary>
        private string Property3 { get; set; }
    }

    public class MockNoNameTable : Base
    { }
}
