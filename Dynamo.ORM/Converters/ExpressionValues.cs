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

            return result;
        }

        private static Dictionary<string, AttributeValue> ConvertExpressionValues(BinaryExpression binaryExpression, ref int valueIndex, ref StringBuilder expressionString)
        {
            var expressionValues = GetExpressionValues(binaryExpression, ref valueIndex, ref expressionString);

            SanitizeComparitives(ref expressionString);

            return ConvertExpressionValues(expressionValues);
        }

        private static Dictionary<string, AttributeValue> ConvertExpressionValues(IDictionary<string, object> expressionValues)
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

        private static IDictionary<string, object> GetExpressionValues(BinaryExpression binaryExpression, ref int valueIndex, ref StringBuilder expressionString)
        {
            var variableName = $":val{valueIndex}";
            IEnumerable<KeyValuePair<string, object>> expressionValues = new Dictionary<string, object>();

            if (IsExpressionNodeType(binaryExpression.Left.NodeType))
                expressionValues = expressionValues.Union(GetExpressionValues(binaryExpression.Left as BinaryExpression, ref valueIndex, ref expressionString));
            else if (IsMemberNode(binaryExpression.Left as Expression))
            {
                var propertyName = GetPropertyReference(binaryExpression.Left);

                expressionValues = expressionValues.Append(new KeyValuePair<string, object>(variableName, GetValue(binaryExpression.Right)));

                expressionString.Replace(binaryExpression.ToString(), $"{propertyName} {binaryExpression.NodeType} {variableName}");
                valueIndex++;
            }

            if (IsExpressionNodeType(binaryExpression.Right.NodeType))
                expressionValues = expressionValues.Union(GetExpressionValues(binaryExpression.Right as BinaryExpression, ref valueIndex, ref expressionString));
            else if (IsMemberNode(binaryExpression.Right as Expression))
            {
                var propertyName = GetPropertyReference(binaryExpression.Right);

                expressionValues = expressionValues.Append(new KeyValuePair<string, object>(variableName, GetValue(binaryExpression.Left)));
                expressionString.Replace(binaryExpression.ToString(), $"{variableName} {binaryExpression.NodeType} {propertyName}");
                valueIndex++;
            }

            return expressionValues.ToDictionary(s => s.Key, s => s.Value);
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
            if (nodeType != ExpressionType.MemberAccess && nodeType != ExpressionType.Constant && nodeType != ExpressionType.New)
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