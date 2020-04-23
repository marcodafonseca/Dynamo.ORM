using System;

namespace Dynamo.ORM.Exceptions
{
    public class RepositoryException : Exception
    {
        public RepositoryException(string message)
            : base(message)
        { }
    }
}