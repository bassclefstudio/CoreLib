using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Core.Primitives;

/// <summary>
/// A set of <see cref="DateTimeOffset"/>s, represented by a collection of <see cref="DateTimeSpan"/>s from which the set is constructed.
/// </summary>
/// <param name="Spans">The collection of normalized (see <see cref="DateTimeSpanExtensions.Normalize(IEnumerable{DateTimeSpan})"/>) <see cref="DateTimeSpan"/>s which make up every interval of time which is included in this <see cref="DateTimeSet"/>.</param>
public readonly record struct DateTimeSet(params DateTimeSpan[] Spans) : IEquatable<DateTimeSet>
{
    /// <summary>
    /// Represents an empty <see cref="DateTimeSet"/> which includes no points in time.
    /// </summary>
    public static readonly DateTimeSet Empty = new DateTimeSet();

    /// <summary>
    /// Creates a <see cref="DateTimeSet"/> from a collection of <see cref="DateTimeSpan"/>s.
    /// </summary>
    /// <param name="spans">A collection of unordered <see cref="DateTimeSpan"/> spans which define the periods of time included in this set.</param>
    /// <returns>A new <see cref="DateTimeSet"/> constructed from the normalized (see <see cref="DateTimeSpanExtensions.Normalize(IEnumerable{DateTimeSpan})"/>) <see cref="DateTimeSpan"/>s included in <paramref name="spans"/>.</returns>
    public static DateTimeSet CreateSet(IEnumerable<DateTimeSpan> spans)
        => new DateTimeSet(spans.Normalize().ToArray());

    /// <summary>
    /// Enumerates over <see cref="Spans"/> and returns a flat collection of all of the transition points between the starts and ends of each <see cref="DateTimeSpan"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="DateTimeSpan.StartDate"/>s and <see cref="DateTimeSpan.EndDate"/>s.</returns>
    private IEnumerable<DateTimeOffset> Points()
    {
        foreach(var sp in Spans)
        {
            yield return sp.StartDate;
            yield return sp.EndDate;
        }
    }

    /// <summary>
    /// Checks if a given <see cref="DateTimeOffset"/> is present in the set.
    /// </summary>
    /// <param name="point">The <see cref="DateTimeOffset"/> point in time being checked.</param>
    /// <returns>A <see cref="bool"/> indicating whether this <see cref="DateTimeSet"/> includes <paramref name="point"/>.</returns>
    public bool Includes(DateTimeOffset point)
        => Spans.Any(s => s.Includes(point));

    /// <summary>
    /// Calculates the inverse <see cref="DateTimeSet"/> to the current set.
    /// </summary>
    /// <returns>A <see cref="DateTimeSet"/> which contains all points in time which are not present in the current set. Note that the endpoints of component <see cref="DateTimeSpan"/>s are preserved in both sets.</returns>
    public DateTimeSet Inverse()
    {
        DateTimeOffset? current = DateTimeOffset.MinValue;
        List<DateTimeSpan> spans = new List<DateTimeSpan>();
        foreach (var p in this.Points())
        {
            if (current == null) current = p;
            else
            {
                spans.Add(new DateTimeSpan(current.Value, p)); current = null;
            }
        }
        return new DateTimeSet(spans.ToArray());
    }

    /// <summary>
    /// Calculates the union of this set with another set.
    /// </summary>
    /// <param name="other">The second <see cref="DateTimeSet"/> to combine.</param>
    /// <returns>The resulting single <see cref="DateTimeSet"/> which contains all points in time contained within the current set and/or <paramref name="other"/>.</returns>
    public DateTimeSet Union(DateTimeSet other)
        => new DateTimeSet(this.Spans.Concat(other.Spans).Normalize().ToArray());

    /// <summary>
    /// Finds the intersection of this set with another set.
    /// </summary>
    /// <param name="other">The second <see cref="DateTimeSet"/> to intersect.</param>
    /// <returns>The resulting single <see cref="DateTimeSet"/> which contains all points in time contained within both the current set and <paramref name="other"/>.</returns>
    public DateTimeSet Intersection(DateTimeSet other)
    {
        int i = 0;
        int j = 0;
        List<DateTimeSpan> spans = new List<DateTimeSpan>();
        while (i < this.Spans.Length && j < other.Spans.Length)
        {
            var a = this.Spans[i];
            var b = other.Spans[j];
            if (a.StartDate > b.EndDate) i++;
            else if (b.StartDate > a.EndDate) j++;
            else
            {
                spans.Add(new DateTimeSpan(
                    (a.StartDate < b.StartDate) ? b.StartDate : a.StartDate,
                    (a.EndDate > b.EndDate) ? b.EndDate : a.EndDate));
            }
        }
        return new DateTimeSet(spans.ToArray());
    }
}
