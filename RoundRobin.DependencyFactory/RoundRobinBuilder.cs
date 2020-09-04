using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace RoundRobin.DependencyFactory
{
    internal class RoundRobinBuilder<TService> : IRoundRobinBuilder<TService> where TService : class
    {
        private readonly IServiceCollection services;
        private readonly ServiceLifetime lifetime;
        private readonly HashSet<Type> implementationTypes;

        public RoundRobinBuilder(IServiceCollection services, ServiceLifetime lifetime)
        {
            this.services = services;
            this.lifetime = lifetime;

            implementationTypes = new HashSet<Type>();
        }

        public IEnumerable<Type> ImplementationTypes => implementationTypes;

        IRoundRobinBuilder<TService> IRoundRobinBuilder<TService>.AddImplementation<TImplementation>()
        {
            services.Add(ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime));
            services.Add(ServiceDescriptor.Describe(typeof(IRoundRobinService<TService>), typeof(RoundRobinService<TService, TImplementation>), lifetime));

            if (!implementationTypes.Add(typeof(TImplementation)))
            {
                throw new InvalidOperationException($"\"{typeof(TImplementation).Name}\" has already been registered");
            }

            return this;
        }
    }
}