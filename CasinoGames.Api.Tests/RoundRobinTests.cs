using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RoundRobin;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CasinoGames.Api.Tests
{
    public class RoundRobinTests
    {
        [Fact]
        public void TestRoundRobin_Does_Rotate()
        {
            var roundRobin = new RoundRobinList<int>(Enumerable.Range(0, 5));

            for (int i = 0; i < 20; i++)
            {
                roundRobin.Next().Should().Be(i % 5);
            }
        }

        [Fact]
        public async Task TestRoundRobin_Does_Rotate_Concurrently()
        {
            var range = Enumerable.Range(0, 5);
            var roundRobin = new RoundRobinList<int>(range);

            var results = await Task.WhenAll(range.Select(i => Task.Factory.StartNew(() => roundRobin.Next())));
            results.Should().BeEquivalentTo(range);
        }

        [Fact]
        public void TestRoundRobinServiceCollection_Does_Rotate_Transiently()
        {
            var services = new ServiceCollection();
            services
                .AddRoundRobin<IService>(ServiceLifetime.Singleton, ServiceLifetime.Transient)
                .AddImplementation<ConcreteServiceA>()
                .AddImplementation<ConcreteServiceB>();

            var provider = services.BuildServiceProvider();

            using var scope = provider.CreateScope();
            var serviceA = scope.ServiceProvider.GetRequiredService<IService>();
            var serviceB = scope.ServiceProvider.GetRequiredService<IService>();

            serviceA.Should().BeOfType(typeof(ConcreteServiceA));
            serviceB.Should().BeOfType(typeof(ConcreteServiceB));

            serviceA = scope.ServiceProvider.GetRequiredService<IService>();
            serviceB = scope.ServiceProvider.GetRequiredService<IService>();

            serviceA.Should().BeOfType(typeof(ConcreteServiceA));
            serviceB.Should().BeOfType(typeof(ConcreteServiceB));
        }

        [Fact]
        public void TestRoundRobinServiceCollection_Does_Rotate_Across_Scopes()
        {
            var services = new ServiceCollection();
            services.AddRoundRobin<IService>(ServiceLifetime.Scoped, ServiceLifetime.Transient)
                .AddImplementation<ConcreteServiceA>()
                .AddImplementation<ConcreteServiceB>();

            var provider = services.BuildServiceProvider();

            using (var scope = provider.CreateScope())
            {
                var serviceA = scope.ServiceProvider.GetRequiredService<IService>();
                serviceA.Should().BeOfType(typeof(ConcreteServiceA));
            }
            using (var scope = provider.CreateScope())
            {
                var serviceB = scope.ServiceProvider.GetRequiredService<IService>();
                serviceB.Should().BeOfType(typeof(ConcreteServiceB));
            }
        }

        [Fact]
        public void TestRoundRobinServiceCollection_RoundRobinServiceLifetime_Transient()
        {
            var services = new ServiceCollection();
            services.AddRoundRobin<IService>(ServiceLifetime.Transient, ServiceLifetime.Transient)
                .AddImplementation<ConcreteServiceA>()
                .AddImplementation<ConcreteServiceB>();

            var provider = services.BuildServiceProvider();

            using var scope = provider.CreateScope();
            scope.ServiceProvider.GetRequiredService<IService>().Should().BeOfType(typeof(ConcreteServiceA));
            scope.ServiceProvider.GetRequiredService<IService>().Should().BeOfType(typeof(ConcreteServiceB));
        }

        [Fact]
        public void TestRoundRobinServiceCollection_RoundRobinServiceLifetime_Scoped()
        {
            var services = new ServiceCollection();
            services.AddRoundRobin<IService>(ServiceLifetime.Transient, ServiceLifetime.Scoped)
                .AddImplementation<ConcreteServiceA>()
                .AddImplementation<ConcreteServiceB>();

            var provider = services.BuildServiceProvider();

            using (var scope = provider.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<IService>().Should().BeOfType(typeof(ConcreteServiceA));
                scope.ServiceProvider.GetRequiredService<IService>().Should().BeOfType(typeof(ConcreteServiceA));
            }
            using (var scope = provider.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<IService>().Should().BeOfType(typeof(ConcreteServiceB));
                scope.ServiceProvider.GetRequiredService<IService>().Should().BeOfType(typeof(ConcreteServiceB));
            }
        }

        [Fact]
        public void TestRoundRobinServiceCollection_RoundRobinServiceLifetime_Singleton_ThrowException()
        {
            var services = new ServiceCollection();
            Action act = () => services.AddRoundRobin<IService>(ServiceLifetime.Transient, ServiceLifetime.Singleton);

            act.Should().Throw<InvalidOperationException>("singleton lifetime is not allowed");
        }

        [Fact]
        public void TestRoundRobinServiceCollection_Service_Injected_After_Dispose()
        {
            var services = new ServiceCollection();
            services.AddRoundRobin<IService>(ServiceLifetime.Transient, ServiceLifetime.Transient)
                .AddImplementation<ConcreteServiceA>()
                .AddImplementation<ConcreteServiceB>();

            var provider = services.BuildServiceProvider();

            using (var scope = provider.CreateScope())
            {
                using (var service = scope.ServiceProvider.GetRequiredService<IService>())
                {
                    service.Should().BeOfType(typeof(ConcreteServiceA));
                    service.Name.Should().Be(nameof(ConcreteServiceA));
                    service.Disposed.Should().BeFalse();
                }
                using (var service = scope.ServiceProvider.GetRequiredService<IService>())
                {
                    service.Should().BeOfType(typeof(ConcreteServiceB));
                    service.Name.Should().Be(nameof(ConcreteServiceB));
                    service.Disposed.Should().BeFalse();
                }
                using (var service = scope.ServiceProvider.GetRequiredService<IService>())
                {
                    service.Should().BeOfType(typeof(ConcreteServiceA));
                    service.Name.Should().Be(nameof(ConcreteServiceA));
                    service.Disposed.Should().BeFalse();
                }
                using (var service = scope.ServiceProvider.GetRequiredService<IService>())
                {
                    service.Should().BeOfType(typeof(ConcreteServiceB));
                    service.Name.Should().Be(nameof(ConcreteServiceB));
                    service.Disposed.Should().BeFalse();
                }
            }
        }

        public interface IService : IDisposable
        {
            string Name { get; }
            bool Disposed { get; }
        }

        public abstract class BaseService : IService
        {
            string IService.Name => GetType().Name;

            public bool Disposed { get; set; } = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!Disposed)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects)
                    }
                    Disposed = true;
                }
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        public class ConcreteServiceA : BaseService { }

        public class ConcreteServiceB : BaseService { }
    }
}