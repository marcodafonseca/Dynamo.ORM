using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Models;
using Dynamo.ORM.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using Dynamo.ORM.Converters;

namespace Dynamo.ORM.Services
{
    public class Repository : IRepository
    {
        private readonly IAmazonDynamoDB amazonDynamoDB;

        public Repository(IAmazonDynamoDB amazonDynamoDB)
        {
            this.amazonDynamoDB = amazonDynamoDB;
        }

        public async Task<List<T>> List<T>(Expression<Func<T, bool>> expression = null) where T : Base, new()
        {
            var generic = new T();

            var tableName = generic.GetTableName();
            var projectExpressionAttributes = generic.GetExpressionAttributes();

            var dynamoDbRequest = new ScanRequest
            {
                TableName = tableName,
                ProjectionExpression = string.Join(", ", projectExpressionAttributes.Keys),
                ExpressionAttributeNames = projectExpressionAttributes
            };

            if (expression == null)
                expression = (x) => 1 == 1;

            var expressionString = new StringBuilder();

            dynamoDbRequest.ExpressionAttributeValues = ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            dynamoDbRequest.FilterExpression = expressionString.ToString();

            return (await amazonDynamoDB.ScanAsync(dynamoDbRequest))
                .Items
                .Select(x => x.Map<T>())
                .ToList();
        }
    }
}