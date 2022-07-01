using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Core.Structures.Graphs
{
    /// <summary>
    /// Provides helper methods and operations that can be executed on a desired graph.
    /// </summary>
    public static class Graph
    {
        /// <summary>
        /// Gets the collection of all <typeparamref name="T"/> nodes in the graph.
        /// </summary>
        /// <typeparam name="T">The type of nodes that make up this graph.</typeparam>
        /// <param name="graph">The collection of <see cref="Edge{T}"/>s describing the graph.</param>
        /// <returns>A collection of <typeparamref name="T"/> nodes.</returns>
        public static IEnumerable<T> Nodes<T>(this IEnumerable<Edge<T>> graph)
            => graph.SelectMany(e => new[] { e.Start, e.End }).Distinct();

        /// <summary>
        /// Gets the collection of all <see cref="Edge{T}"/>s in a graph that travel to a given node.
        /// </summary>
        /// <typeparam name="T">The type of nodes that make up this graph.</typeparam>
        /// <param name="graph">The collection of <see cref="Edge{T}"/>s describing the graph.</param>
        /// <param name="node">The <typeparamref name="T"/> node desired edges must end on.</param>
        /// <returns>A collection of <see cref="Edge{T}"/> edges which have a <see cref="Edge{T}.End"/> of <paramref name="node"/>.</returns>
        public static IEnumerable<Edge<T>> To<T>(this IEnumerable<Edge<T>> graph, T node)
            => graph.Where(e => Equals(node, e.End));

        /// <summary>
        /// Gets the collection of all <see cref="Edge{T}"/>s in a graph that travel from a given node.
        /// </summary>
        /// <typeparam name="T">The type of nodes that make up this graph.</typeparam>
        /// <param name="graph">The collection of <see cref="Edge{T}"/>s describing the graph.</param>
        /// <param name="node">The <typeparamref name="T"/> node desired edges must start on.</param>
        /// <returns>A collection of <see cref="Edge{T}"/> edges which have a <see cref="Edge{T}.Start"/> of <paramref name="node"/>.</returns>
        public static IEnumerable<Edge<T>> From<T>(this IEnumerable<Edge<T>> graph, T node)
            => graph.Where(e => Equals(node, e.Start));

        /// <summary>
        /// Performs a transitive operation on the given graph, returning a new graph of all <see cref="Edge{T}"/>s representing every two-edge connection in the original graph.
        /// </summary>
        /// <typeparam name="T">The type of nodes that make up this graph.</typeparam>
        /// <param name="graph">The collection of <see cref="Edge{T}"/>s describing the graph.</param>
        /// <param name="newEdge">A function which takes in two component <see cref="Edge{T}"/>s and returns the resulting <see cref="Edge{T}"/>.</param>
        /// <returns>A new graph made up of a collection of <see cref="Edge{T}"/>s which are each the product (using <paramref name="newEdge"/>) of two edges in the original <paramref name="graph"/>.</returns>
        public static IEnumerable<Edge<T>> Transitive<T>(this IEnumerable<Edge<T>> graph, Func<Edge<T>, Edge<T>, Edge<T>> newEdge)
            => graph.SelectMany(e => graph.From(e.Start).Select(t => newEdge(e, t)));
    }
}
