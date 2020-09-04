using System;

namespace RoundRobin.DependencyFactory
{
    internal class RoundRobinImplementation<TService, TImplementation> : IRoundRobinImplementation<TService>
        where TService : class
        where TImplementation : class, TService
    {
        private readonly Func<TImplementation> implementation;

        public RoundRobinImplementation(Func<TImplementation> implementation)
        {
            this.implementation = implementation;
        }

        public Func<TService> Service => implementation;
    }
}