using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Constants;
using Dynamo.ORM.Extensions;
using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Dynamo.ORM.Converters
{
    public class ExpressionValues
    {
        public static Dictionary<string, AttributeValue> ConvertExpressionValues<T>(Expression<Func<T, bool>> expression, ref StringBuilder expressionString) where T : Base, new()
        {
            var valueIndex = 0;

            if (string.IsNullOrWhiteSpace(expressionString.ToString()))
                expressionString = new StringBuilder(expression.Body.ToString());

            var result = ConvertExpressionValues(expression.Body as BinaryExpression, ref valueIndex, ref expressionString);

            SanitizeExpression(expression, ref expressionString);

            return result.ToDictionary(s => s.Key, s => s.Value);
        }

        private static IDictionary<string, AttributeValue> ConvertExpressionValues(BinaryExpression binaryExpression, ref int valueIndex, ref StringBuilder expressionString)
        {
            var expressionValues = GetExpressionValues(binaryExpression, ref valueIndex, ref expressionString);

            SanitizeComparitives(ref expressionString);

            return ConvertExpressionValues(expressionValues);
        }

        private static IDictionary<string, AttributeValue> ConvertExpressionValues(IEnumerable<KeyValuePair<string, object>> expressionValues)
        {
            var convertedExpressionValues = new Dictionary<string, AttributeValue>();

            foreach (var item in expressionValues)
            {
                if (item.Value == null)
                    convertedExpressionValues.Add(item.Key, new AttributeValue { NULL = true });
                else
                    convertedExpressionValues.Add(item.Key, AttributeValueConverter.ConvertToAttributeValue[item.Value.GetType()](item.Value));
            }

            return convertedExpressionValues;
        }

        private static IEnumerable<KeyValuePair<string, object>> GetExpressionValues(BinaryExpression binaryExpression, ref int valueIndex, ref StringBuilder expressionString)
        {
            var variableName = $":val{valueIndex}";
            IEnumerable<KeyValuePair<string, object>> expressionValues = new Dictionary<string, object>();

            expressionValues = expressionValues.Union(GetExpressionValues(binaryExpression, binaryExpression.Left, binaryExpression.Right, ref valueIndex, ref expressionString));
            expressionValues = expressionValues.Union(GetExpressionValues(binaryExpression, binaryExpression.Right, binaryExpression.Left, ref valueIndex, ref expressionString));

            return expressionValues;
        }

        private static IEnumerable<KeyValuePair<string, object>> GetExpressionValues(BinaryExpression binaryExpression, Expression primaryExpression, Expression secondaryExpression, ref int valueIndex, ref StringBuilder expressionString)
        {
            var variableName = $":val{valueIndex}";
            IEnumerable<KeyValuePair<string, object>> expressionValues = new Dictionary<string, object>();

            if (primaryExpression is UnaryExpression)
            {
                expressionValues = expressionValues.Union(GetExpressionValues(primaryExpression as UnaryExpression, ref valueIndex, ref expressionString));
            }
            else if (IsExpressionNodeType(primaryExpression.NodeType))
                expressionValues = expressionValues.Union(GetExpressionValues(primaryExpression as BinaryExpression, ref valueIndex, ref expressionString));
            else if (IsMemberNode(primaryExpression))
            {
                AppedExpressionValue(binaryExpression, primaryExpression, secondaryExpression, variableName, ref expressionValues, ref expressionString);
                valueIndex++;
            }

            return expressionValues;
        }

        private static IEnumerable<KeyValuePair<string, object>> GetExpressionValues(UnaryExpression unaryExpression, ref int valueIndex, ref StringBuilder expressionString)
        {
            var variableName = $":val{valueIndex}";
            IEnumerable<KeyValuePair<string, object>> expressionValues = new Dictionary<string, object>();

            if (!IsMemberNode(unaryExpression.Operand))
            {
                expressionValues = expressionValues.Append(new KeyValuePair<string, object>(variableName, GetValue(unaryExpression)));

                expressionString.Replace(unaryExpression.ToString(), variableName);
                valueIndex++;
            }
            else
            {
                var propertyName = GetPropertyReference(unaryExpression.Operand);

                expressionString.Replace(unaryExpression.ToString(), propertyName);
            }

            return expressionValues;
        }

        private static void AppedExpressionValue(BinaryExpression binaryExpression, Expression propertyExpression, Expression valueExpression, string variableName, ref IEnumerable<KeyValuePair<string, object>> expressionValues, ref StringBuilder expressionString)
        {
            var propertyName = GetPropertyReference(propertyExpression);

            expressionValues = expressionValues.Append(new KeyValuePair<string, object>(variableName, GetValue(valueExpression)));

            expressionString.Replace(binaryExpression.ToString(), $"{propertyName} {binaryExpression.NodeType} {variableName}");
        }

        private static string GetPropertyReference(Expression expression) => Base.GetPropertyReference(expression.ToString());

        // Needs revising for a more generic approach
        private static object GetValue(Expression expression)
        {
            if (Constants.ExpressionValues.GetExpressionValues.ContainsKey(expression.Type))
                return Constants.ExpressionValues.GetExpressionValues[expression.Type](expression);
            else
                return Constants.ExpressionValues.GetExpressionValues[typeof(object)](expression);
        }

        private static bool IsMemberNode(Expression expression)
        {
            if (expression.NodeType != ExpressionType.MemberAccess)
                return false;
            if (expression is MemberExpression)
                if (!(((MemberExpression)expression).Expression is ParameterExpression))
                    return false;
            return true;
        }

        private static bool IsExpressionNodeType(ExpressionType nodeType)
        {
            if (nodeType != ExpressionType.MemberAccess &&
                nodeType != ExpressionType.Constant &&
                nodeType != ExpressionType.New &&
                nodeType != ExpressionType.Call)
                return true;
            return false;
        }

        private static void SanitizeExpression<T>(Expression<Func<T, bool>> expression, ref StringBuilder expressionString) where T : Base, new()
        {
            foreach (var parameter in expression.Parameters)
                expressionString.Replace($"{parameter.Name}.", "");

            foreach (Match match in Regex.Matches(expressionString.ToString(), @"(#\w+)+"))
                expressionString.Replace(match.Value, Base.GetPropertyReference(match.Value.Substring(1)));
        }

        private static void SanitizeComparitives(ref StringBuilder expressionString)
        {
            foreach (var comparitive in Constants.ExpressionValues.Comparitives)
                expressionString.Replace(comparitive.Key, comparitive.Value);
        }
    }
}