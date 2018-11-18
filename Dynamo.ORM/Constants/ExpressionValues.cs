using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.ORM.Constants
{
    internal class ExpressionValues
    {
        internal static readonly IDictionary<string, string> Comparitives = new Dictionary<string, string>
        {
            { "LessThanOrEqual", "<=" },
            { "GreaterThanOrEqual", ">=" },
            { "AndAlso", "&&" },
            { "OrElse", "OR" },
            { "GreaterThan", ">" },
            { "NotEqual", "!=" },
            { "LessThan", "<" },
            { "Equal", "=" }
        };
    }
}
