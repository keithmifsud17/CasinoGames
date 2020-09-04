using System;
using System.Collections.Generic;
using System.Linq;

namespace RoundRobin.DependencyFactory
{
    internal class RoundRobinFactory<TService> where TService : class
    {
        private readonly RoundRobinList<Func<TService>> services;

        public RoundRobinFactory(IEnumerable<IRoundRobinImplementation<TService>> services)
        {
            this.services = new RoundRobinList<Func<TService>>(services.Select(s => s.Service));
        }

        public TService Resolve() => services.Next().Invoke();
    }
}