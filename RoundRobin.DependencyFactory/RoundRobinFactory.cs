using System.Collections.Generic;
using System.Linq;

namespace RoundRobin.DependencyFactory
{
    internal class RoundRobinFactory<TService> where TService : class
    {
        private readonly RoundRobinTracker<TService> tracker;
        private readonly IEnumerable<IRoundRobinService<TService>> services;

        public RoundRobinFactory(RoundRobinTracker<TService> tracker, IEnumerable<IRoundRobinService<TService>> services)
        {
            this.tracker = tracker;
            this.services = services;
        }

        public TService NextService()
        {
            var type = tracker.NextType();
            return services.Single(service => service.ImplementationType == type).Service;
        }
    }
}