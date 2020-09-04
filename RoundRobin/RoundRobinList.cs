using System.Collections.Generic;

namespace RoundRobin
{
    /// <summary>
    /// Accepts a collection of <typeparamref name="T"/> and resolves every element when requested in a round robin
    /// </summary>
    /// <typeparam name="T">The type of collection to iterate through</typeparam>
    public class RoundRobinList<T>
    {
        private readonly LinkedList<T> linkedList;
        private readonly object lockNext = new object();

        private LinkedListNode<T> node;

        /// <summary>
        /// Accepts a collection of <typeparamref name="T"/> and resolves every element when requested in a round robin
        /// </summary>
        /// <param name="collection">The collection of <typeparamref name="T"/> that will be used to iterate over</param>
        public RoundRobinList(IEnumerable<T> collection)
        {
            if (collection is null)
            {
                throw new System.ArgumentNullException(nameof(collection));
            }

            linkedList = new LinkedList<T>(collection);
        }

        /// <summary>
        /// Resolve the next element in the collection, or start over if end is reached.
        /// </summary>
        /// <returns>The next <typeparamref name="T"/> in line.</returns>
        public T Next()
        {
            lock (lockNext)
            {
                // set node to next in line or go back to first
                node = node?.Next ?? linkedList.First;
                return node.Value;
            }
        }
    }
}