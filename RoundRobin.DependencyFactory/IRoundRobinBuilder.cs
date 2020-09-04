namespace RoundRobin.DependencyFactory
{
    public interface IRoundRobinBuilder<TService> where TService : class
    {
        /// <summary>
        /// Adds an implementation <typeparamref name="TImplementation"/> in the round robin resolver for <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TImplementation"><typeparamref name="TService"/> implementation</typeparam>
        /// <returns>The round robin builder</returns>
        IRoundRobinBuilder<TService> AddImplementation<TImplementation>() where TImplementation : class, TService;
    }
}
