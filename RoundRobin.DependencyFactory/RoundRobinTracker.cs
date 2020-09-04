using System;
using System.Collections.Generic;

namespace RoundRobin.DependencyFactory
{
    internal class RoundRobinTracker<TService> where TService : class
    {
        private readonly RoundRobinList<Type> roundRobin;

        public RoundRobinTracker(IEnumerable<Type> types)
        {
            roundRobin = new RoundRobinList<Type>(types);
        }

        public Type NextType() => roundRobin.Next();
    }
}