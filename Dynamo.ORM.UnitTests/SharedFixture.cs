using System;

namespace Dynamo.ORM.UnitTests
{
    public class SharedFixture : IDisposable
    {
        private readonly Random random = new();

        public void Dispose()
        {
        }

        public int RandomInt() => random.Next(10000);
    }
}