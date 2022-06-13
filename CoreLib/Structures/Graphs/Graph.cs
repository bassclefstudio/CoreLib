using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Core.Structures.Graphs
{
    /// <summary>
    /// Provides helper methods and operations that can be executed on a desired <see cref="Graph"/>.
    /// </summary>
    public static class Graph
    {
        public static IEnumerable<T> Nodes<T>(this IEnumerable<Edge<T>> graph)
            => graph.SelectMany(e => new[] { e.Start, e.End }).Distinct();

        public static IEnumerable<Edge<T>> To<T>(this IEnumerable<Edge<T>> graph, T node)
            => graph.Where(e => Equals(node, e.End));

        public static IEnumerable<Edge<T>> From<T>(this IEnumerable<Edge<T>> graph, T node)
            => graph.Where(e => Equals(node, e.Start));

        public static IEnumerable<Edge<T>> Transitive<T>(this IEnumerable<Edge<T>> graph, Func<Edge<T>, Edge<T>, Edge<T>> newEdge)
            => graph.SelectMany(e => graph.From(e.Start).Select(t => newEdge(e, t)));
    }
}
