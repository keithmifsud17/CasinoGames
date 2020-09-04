using System;

namespace RoundRobin.DependencyFactory
{
    internal interface IRoundRobinImplementation<TService> where TService : class
    {
        Func<TService> Service { get; }
    }
}