using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Core.Structures.Streams
{
    /// <summary>
    /// Represents a streaming interface that is used to stream discrete objects or events (as opposed to other <see cref="Stream"/>s which are used for streaming continuous data from storage or memory).
    /// </summary>
    /// <typeparam name="T">The type of objects this <see cref="IDiscreteStream{T}"/> produces or manages.</typeparam>
    public interface IDiscreteStream<T>
    {
        /// <summary>
        /// Awaits and consumes the next <typeparamref name="T"/> data from this stream asynchronously.
        /// </summary>
        /// <returns>The <typeparamref name="T"/> data returned from the stream.</returns>
        Task<T> Consume();

        /// <summary>
        /// Pushes a new <typeparamref name="T"/> data object onto the stream asynchronously.
        /// </summary>
        /// <param name="data">A unit of <typeparamref name="T"/> data that can be read by the <see cref="Consume"/> operation.</param>
        Task Push(T data);
    }
}
