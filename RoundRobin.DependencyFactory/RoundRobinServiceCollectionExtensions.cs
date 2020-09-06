using Microsoft.Extensions.Logging;
using RoundRobin.DependencyFactory;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RoundRobinServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a round robin service resolver of the type specified in <typeparamref name="TService"/> with implementations specified
        /// with <see cref="IRoundRobinBuilder{TService}.AddImplementation{TImplementation}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IRoundRobinBuilder<TService> AddRoundRobin<TService>(this IServiceCollection services, ServiceLifetime implementationLifetime, ServiceLifetime roundRobinLifetime) where TService : class
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (roundRobinLifetime == ServiceLifetime.Singleton)
            {
                throw new InvalidOperationException("A singleton round robin lifetime will not progress the round robin and always resolve the first instance");
            }

            var builder = new RoundRobinBuilder<TService>(services, implementationLifetime);

            services.Add(ServiceDescriptor.Describe(typeof(RoundRobinFactory<TService>), typeof(RoundRobinFactory<TService>), implementationLifetime));

            services.AddSingleton(_ => new RoundRobinTracker<TService>(builder.ImplementationTypes));

            services.Add(ServiceDescriptor.Describe(typeof(TService), provider => 
            {
                var service = provider.GetRequiredService<RoundRobinFactory<TService>>().NextService();

                var logger = provider.GetService<ILogger<TService>>();
                if (logger != default)
                {
                    logger.LogTrace("Resolving {service} with implementation {implementation}", typeof(TService), service.GetType());
                }

                return service;
            }, roundRobinLifetime));

            return builder;
        }
    }
}