using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Core.Structures.Graphs
{
    /// <summary>
    /// Represents an abstract connection between two <typeparamref name="T"/> nodes.
    /// </summary>
    /// <typeparam name="T">The type of nodes that this <see cref="Edge{T}"/> connects.</typeparam>
    /// <param name="Start">The starting node of this <see cref="Edge{T}"/> connection.</param>
    /// <param name="End">The ending node of this <see cref="Edge{T}"/> connection.</param>
    /// <param name="Directional">A <see cref="bool"/> indicating whether the directionality of this <see cref="Edge{T}"/> is relevant (if <c>false</c>, the <see cref="Edge{T}"/> with reversed <see cref="Start"/> and <see cref="End"/> nodes is identical).</param>
    public record Edge<T>(T Start, T End, bool Directional = false)
    { }
}
