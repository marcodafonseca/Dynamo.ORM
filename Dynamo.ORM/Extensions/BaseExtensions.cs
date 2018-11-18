using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Constants;
using Dynamo.ORM.Exceptions;
using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dynamo.ORM.Extensions
{
    public static class BaseExtensions
    {
        public static string GetTableName(this Base entity)
        {
            var type = entity.GetType();
            var attributes = type.GetCustomAttributes(typeof(DynamoDBTableAttribute), true);

            if (attributes.Length != 1)
                throw new TableAttributeException(type);

            return ((DynamoDBTableAttribute)attributes[0]).TableName;
        }

        public static Dictionary<string, string> GetExpressionAttributes(this Base entity)
        {
            var type = entity.GetType();
            var properties = type.GetProperties();

            var attributes = new Dictionary<string, string>();

            foreach (var property in properties)
                attributes.Add(Base.GetPropertyReference(property.Name), property.Name);

            return attributes;
        }

        public static T Map<T>(this Dictionary<string, AttributeValue> values) where T : Base, new()
        {
            var entity = new T();

            foreach (var property in typeof(T).GetProperties())
            {
                property.SetValue(entity, AttributeValueConverter.ConvertToValue[property.PropertyType](values[property.Name]));
            }

            return entity;
        }
    }
}
