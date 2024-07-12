using System;

namespace Dynamo.ORM.Exceptions
{
    public class PageNotFoundException : Exception
    {
        public PageNotFoundException(string message)
            : base(message)
        { }
    }
}
