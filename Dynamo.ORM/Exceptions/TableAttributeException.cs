using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.ORM.Exceptions
{
    public class TableAttributeException : Exception
    {
        public TableAttributeException() : base("Type does not have attribute DynamoDBTableAttribute")
        { }

        public TableAttributeException(Type type) : base($"Type '{type.ToString()}' does not have attribute DynamoDBTableAttribute")
        { }
    }
}
