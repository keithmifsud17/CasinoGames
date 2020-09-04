using System;

namespace RoundRobin.DependencyFactory
{
    internal interface IRoundRobinService<TService> where TService : class
    {
        TService Service { get; }

        Type ImplementationType { get; }
    }

    internal class RoundRobinService<TService, TImplementation> : IRoundRobinService<TService> where TService : class where TImplementation : class, TService
    {
        public RoundRobinService(TImplementation implementation)
        {
            Service = implementation;
            ImplementationType = typeof(TImplementation);
        }

        public TService Service { get; }

        public Type ImplementationType { get; }
    }
}
