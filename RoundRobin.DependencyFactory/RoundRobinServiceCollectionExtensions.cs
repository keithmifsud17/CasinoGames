using RoundRobin.DependencyFactory;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RoundRobinServiceCollectionExtensions
    {
        private static readonly MethodInfo implementationFactoryMethodInfo = typeof(RoundRobinServiceCollectionExtensions).GetMethod(nameof(ImplementationFactory), BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// Adds a round robin service resolver of the type specified in <typeparamref name="TService"/> with implementations specified 
        /// in <typeparamref name="TImplementation1"/> and <typeparamref name="TImplementation2"/> to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation1">The type of the implementation to use.</typeparam>
        /// <typeparam name="TImplementation2">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddRoundRobin<TService, TImplementation1, TImplementation2>(this IServiceCollection services)
            where TService : class
            where TImplementation1 : class, TService
            where TImplementation2 : class, TService
        {
            return services.AddRoundRobin(typeof(TService), typeof(TImplementation1), typeof(TImplementation2));
        }

        /// <summary>
        /// Adds a round robin service resolver of the type specified in <typeparamref name="TService"/> with implementations specified 
        /// in <typeparamref name="TImplementation1"/>, <typeparamref name="TImplementation2"/> and <typeparamref name="TImplementation3"/> 
        /// to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation1">The type of the implementation to use.</typeparam>
        /// <typeparam name="TImplementation2">The type of the implementation to use.</typeparam>
        /// <typeparam name="TImplementation3">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddRoundRobin<TService, TImplementation1, TImplementation2, TImplementation3>(this IServiceCollection services)
            where TService : class
            where TImplementation1 : class, TService
            where TImplementation2 : class, TService
            where TImplementation3 : class, TService
        {
            return services.AddRoundRobin(typeof(TService), typeof(TImplementation1), typeof(TImplementation2), typeof(TImplementation3));
        }

        /// <summary>
        /// Adds a round robin service resolver of the type specified in <typeparamref name="TService"/> with implementations specified 
        /// in <typeparamref name="TImplementation1"/>, <typeparamref name="TImplementation2"/>, <typeparamref name="TImplementation3"/> and <typeparamref name="TImplementation4"/>
        /// to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation1">The type of the implementation to use.</typeparam>
        /// <typeparam name="TImplementation2">The type of the implementation to use.</typeparam>
        /// <typeparam name="TImplementation3">The type of the implementation to use.</typeparam>
        /// <typeparam name="TImplementation4">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddRoundRobin<TService, TImplementation1, TImplementation2, TImplementation3, TImplementation4>(this IServiceCollection services)
            where TService : class
            where TImplementation1 : class, TService
            where TImplementation2 : class, TService
            where TImplementation3 : class, TService
            where TImplementation4 : class, TService
        {
            return services.AddRoundRobin(typeof(TService), typeof(TImplementation1), typeof(TImplementation2), typeof(TImplementation3), typeof(TImplementation4));
        }

        /// <summary>
        /// Adds a round robin service resolver of the type specified in <typeparamref name="TService"/> with implementations specified 
        /// in <typeparamref name="TImplementation1"/>, <typeparamref name="TImplementation2"/>, <typeparamref name="TImplementation3"/>, 
        /// <typeparamref name="TImplementation4"/> and <typeparamref name="TImplementation5"/> to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation1">The type of the implementation to use.</typeparam>
        /// <typeparam name="TImplementation2">The type of the implementation to use.</typeparam>
        /// <typeparam name="TImplementation3">The type of the implementation to use.</typeparam>
        /// <typeparam name="TImplementation4">The type of the implementation to use.</typeparam>
        /// <typeparam name="TImplementation5">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddRoundRobin<TService, TImplementation1, TImplementation2, TImplementation3, TImplementation4, TImplementation5>(this IServiceCollection services)
            where TService : class
            where TImplementation1 : class, TService
            where TImplementation2 : class, TService
            where TImplementation3 : class, TService
            where TImplementation4 : class, TService
            where TImplementation5 : class, TService
        {
            return services.AddRoundRobin(typeof(TService), typeof(TImplementation1), typeof(TImplementation2), typeof(TImplementation3), typeof(TImplementation4), typeof(TImplementation5));
        }

        private static IServiceCollection AddRoundRobin(this IServiceCollection services, Type serviceType, params Type[] implementationTypes)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var roundRobinImplementation = typeof(IRoundRobinImplementation<>).MakeGenericType(serviceType);
            var roundRobinGenericService = typeof(RoundRobinImplementation<object, object>).GetGenericTypeDefinition();

            foreach (var type in implementationTypes)
            {
                if (!serviceType.IsAssignableFrom(type))
                {
                    throw new InvalidOperationException($"\"{type.Name}\" is not an implementation of \"{serviceType.Name}\"");
                }

                var func = typeof(Func<>).MakeGenericType(type);
                var method = implementationFactoryMethodInfo.MakeGenericMethod(type);

                services.AddTransient(type); // In case of IDisposable
                services.AddSingleton(func, provider => method.Invoke(null, new[] { provider }));
                services.AddSingleton(roundRobinImplementation, roundRobinGenericService.MakeGenericType(serviceType, type));
            }

            var roundRobinFactory = typeof(RoundRobinFactory<>).MakeGenericType(serviceType);
            var resolveNext = roundRobinFactory.GetMethod(nameof(RoundRobinFactory<object>.Resolve));

            return services
                .AddSingleton(roundRobinFactory)
                .AddTransient(serviceType, provider =>
                {
                    var factory = provider.GetRequiredService(roundRobinFactory);
                    return resolveNext.Invoke(factory, new object[] { });
                });
        }

        private static Func<TResult> ImplementationFactory<TResult>(IServiceProvider provider) => () => provider.GetRequiredService<TResult>();

        // some beautiful generics
        //public static IServiceCollection AddRoundRobin<TService, TImplementation1, TImplementation2>(this IServiceCollection services)
        //    where TService : class
        //    where TImplementation1 : class, TService
        //    where TImplementation2 : class, TService
        //{
        //    if (services is null)
        //    {
        //        throw new ArgumentNullException(nameof(services));
        //    }

        //    services
        //        .AddTransient<TImplementation1>()
        //        .AddSingleton<Func<TImplementation1>>(provider => () => provider.GetRequiredService<TImplementation1>())
        //        .AddSingleton<IRoundRobinImplementation<TService>, RoundRobinImplementation<TService, TImplementation1>>()

        //        .AddTransient<TImplementation2>()
        //        .AddSingleton<Func<TImplementation2>>(provider => () => provider.GetRequiredService<TImplementation2>())
        //        .AddSingleton<IRoundRobinImplementation<TService>, RoundRobinImplementation<TService, TImplementation2>>()

        //        .AddSingleton<RoundRobinFactory<TService>>()
        //        .AddTransient(provider => provider.GetRequiredService<RoundRobinFactory<TService>>().Resolve());

        //    return services;
        //}
    }
}