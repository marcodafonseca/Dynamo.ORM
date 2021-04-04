using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Constants;
using Dynamo.ORM.Converters;
using Dynamo.ORM.Exceptions;
using Dynamo.ORM.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dynamo.ORM.Extensions
{
    public static class BaseExtensions
    {
        public static object CreateInstance(this Type type)
        {
            var parameterTypes = type.GetGenericArguments();

            if (type.GetInterfaces().Any(x => x == typeof(IEnumerable)))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    var baseType = typeof(Dictionary<,>);

                    var genericType = baseType.MakeGenericType(parameterTypes);

                    return Activator.CreateInstance(genericType);
                }
                else
                {
                    var baseType = typeof(List<>);

                    var genericType = baseType.MakeGenericType(parameterTypes);

                    return Activator.CreateInstance(genericType);
                }
            }
            else
            {
                var parameters = new List<object>();

                foreach (var parameterType in parameterTypes)
                    parameters.Add(CreateInstance(parameterType));

                if (parameters.Count > 0)
                    return Activator.CreateInstance(type, parameters);
                else
                    return Activator.CreateInstance(type);
            }
        }

        public static Array CreateInstance(this Type type, int size)
        {
            var typeArgument = type.GetDeclaringType();

            return Array.CreateInstance(typeArgument, size);
        }

        public static Type GetDeclaringType(this Type type)
        {
            return type.DeclaringType ?? type.GetElementType() ?? type.GenericTypeArguments.FirstOrDefault();
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

        public static string GetTableName(this Base entity)
        {
            var type = entity.GetType();
            var attributes = type.GetCustomAttributes(typeof(DynamoDBTableAttribute), true);

            if (attributes.Length != 1)
                throw new TableAttributeException(type);

            return ((DynamoDBTableAttribute)attributes[0]).TableName;
        }

        public static T Map<T>(this Dictionary<string, AttributeValue> values) where T : Base, new()
        {
            var entity = new T();

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                if (values.ContainsKey(property.Name))
                {
                    if (AttributeValueConverter.ConvertToValue.ContainsKey(propertyType) && propertyType != typeof(object))
                        property.SetValue(entity, AttributeValueConverter.ConvertToValue[propertyType](values[property.Name]));
                    else if (propertyType.IsArray || (propertyType.IsGenericType && propertyType.GetInterfaces().Contains(typeof(IEnumerable))))
                    {
                        var declaredType = propertyType.GetDeclaringType();

                        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                        {
                            var dictionary = AttributeValueConverter.ConvertToDictionary(propertyType, values[property.Name].M);

                            property.SetValue(entity, dictionary);
                        }
                        else if (ListAttributeValueConverter.AllTypes.Contains(declaredType))
                        {
                            var value = AttributeValueConverter.ConvertToArrayValue(declaredType, values[property.Name]);

                            property.SetValue(entity, AttributeValueConverter.FromList(propertyType, value));
                        }
                        else if (declaredType.IsClass && !values[property.Name].NULL)
                        {
                            var value = values[property.Name].L;

                            property.SetValue(entity, AttributeValueConverter.FromList(propertyType, value));
                        }
                    }
                    else if (propertyType.IsClass)
                    {
                        var value = (Dictionary<string, AttributeValue>)AttributeValueConverter.ConvertToValue[typeof(object)](values[property.Name]);

                        property.SetValue(entity, AttributeValueConverter.FromDictionary(propertyType, value));
                    }
                }
            }

            return entity;
        }

        public static Dictionary<string, AttributeValue> Map(this Base entity, bool includeKeys = false)
        {
            var results = new Dictionary<string, AttributeValue>();

            var type = entity.GetType();
            var properties = type.GetProperties()
                .Where(x => includeKeys || !IsKeyProperty(x));

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                if (AttributeValueConverter.ConvertToAttributeValue.ContainsKey(propertyType))
                    results.Add(property.Name, AttributeValueConverter.ConvertToAttributeValue[propertyType](property.GetValue(entity)));
                else if (propertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    {
                        var model = new Dictionary<string, AttributeValue>();
                        var dictionary = (IDictionary)property.GetValue(entity);

                        foreach (var key in dictionary.Keys)
                        {
                            var value = dictionary[key];
                            var valueType = value.GetType();

                            model.Add(key.ToString(), AttributeValueConverter.ConvertToAttributeValue[valueType](value));
                        }

                        results.Add(property.Name, new AttributeValue
                        {
                            M = model
                        });
                    }
                    else
                    {
                        var elementType = propertyType.GetDeclaringType();

                        results.Add(property.Name, ListAttributeValueConverter.ConvertToAttributeValue(elementType, ((IEnumerable)property.GetValue(entity)).GetEnumerator()));
                    }
                }
                else if (propertyType.IsClass)
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