using Dynamo.ORM.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.ORM.Exceptions
{
    public class TableKeyAttributeException : Exception
    {
        public TableKeyAttributeException() 
            : base("Type does not have any key attributes defined")
        { }

        public TableKeyAttributeException(Type type) 
            : base($"Type '{type.ToString()}' does not have any key attributes defined")
        { }

        public TableKeyAttributeException(Type type, KeyEnum keys) 
            : base($"Type '{type.ToString()}' can only have 1 {keys} defined")
        { }
    }
}
