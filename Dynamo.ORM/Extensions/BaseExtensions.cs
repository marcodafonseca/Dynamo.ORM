using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Constants;
using Dynamo.ORM.Exceptions;
using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (values.ContainsKey(property.Name))
                {
                    if (AttributeValueConverter.ConvertToValue.ContainsKey(property.PropertyType))
                        property.SetValue(entity, AttributeValueConverter.ConvertToValue[property.PropertyType](values[property.Name]));
                    else if (property.PropertyType.IsClass)
                    {
                        var value = (Dictionary<string, AttributeValue>)AttributeValueConverter.ConvertToValue[typeof(object)](values[property.Name]);

                        property.SetValue(entity, AttributeValueConverter.FromDictionary(property.PropertyType, value));
                    }
                }
            }

            return entity;
        }

        public static Dictionary<string, AttributeValue> GetKey(this Base entity, object hashKeyValue = null, object rangeKeyValue = null)
        {
            var keys = new Dictionary<string, AttributeValue>();

            var type = entity.GetType();
            var properties = type.GetProperties();
            var hashKeys = properties.Where(property => Attribute.IsDefined(property, typeof(DynamoDBHashKeyAttribute)));
            var rangeKeys = properties.Where(property => Attribute.IsDefined(property, typeof(DynamoDBRangeKeyAttribute)));

            if ((hashKeys.Count() + rangeKeys.Count()) == 0)
                throw new TableKeyAttributeException(type);
            else if (hashKeys.Count() != 1)
                throw new TableKeyAttributeException(type, KeyEnum.Hash);
            else if (rangeKeys.Count() > 1)
                throw new TableKeyAttributeException(type, KeyEnum.Range);

            if (hashKeys.Count() == 1)
            {
                var propertyInfo = hashKeys.First();
                var attributeName = propertyInfo.Name;

                if (hashKeyValue != null)
                    keys.Add(attributeName, AttributeValueConverter.ConvertToAttributeValue[propertyInfo.PropertyType](hashKeyValue));
                else
                    keys.Add(attributeName, AttributeValueConverter.ConvertToAttributeValue[propertyInfo.PropertyType](propertyInfo.GetValue(entity)));
            }

            if (rangeKeys.Count() == 1)
            {
                var propertyInfo = rangeKeys.First();
                var attributeName = propertyInfo.Name;

                if (rangeKeyValue != null)
                    keys.Add(attributeName, AttributeValueConverter.ConvertToAttributeValue[propertyInfo.PropertyType](rangeKeyValue));
                else
                    keys.Add(attributeName, AttributeValueConverter.ConvertToAttributeValue[propertyInfo.PropertyType](propertyInfo.GetValue(entity)));
            }

            return keys;
        }

        public static Dictionary<string, AttributeValue> Map(this Base entity, bool includeKeys = false)
        {
            var results = new Dictionary<string, AttributeValue>();

            var type = entity.GetType();
            var properties = type.GetProperties()
                .Where(x => includeKeys || !IsKeyProperty(x));

            foreach (var property in properties)
            {
                if (AttributeValueConverter.ConvertToAttributeValue.ContainsKey(property.PropertyType))
                    results.Add(property.Name, AttributeValueConverter.ConvertToAttributeValue[property.PropertyType](property.GetValue(entity)));
                else if (property.PropertyType.IsClass)
                    results.Add(property.Name, AttributeValueConverter.ConvertToAttributeValue[typeof(object)](property.GetValue(entity)));
            }

            return results;
        }

        private static bool IsKeyProperty(PropertyInfo propertyInfo)
        {
            var result = false;
            if (Attribute.IsDefined(propertyInfo, typeof(DynamoDBHashKeyAttribute)) ||
                Attribute.IsDefined(propertyInfo, typeof(DynamoDBRangeKeyAttribute)))
                result = true;
            return result;
        }
    }
}
