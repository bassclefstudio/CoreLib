using System;

namespace BassClefStudio.Core.Primitives
{
    /// <summary>
    /// Represents a contiguous block of time between two <see cref="DateTimeOffset"/>s.
    /// </summary>
    /// <param name="StartDate">Represents the beginning <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> starts.</param>
    /// <param name="EndDate">Represents the final <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> ends.</param>
    public readonly record struct DateTimeSpan(DateTimeOffset StartDate, DateTimeOffset EndDate) : IComparable<DateTimeSpan>
    {
        /// <summary>
        /// A <see cref="DateTimeSpan"/> that spans every point in time.
        /// </summary>
        public static readonly DateTimeSpan All = new DateTimeSpan(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);

        /// <summary>
        /// Creates a new <see cref="DateTimeSpan"/> between two dates.
        /// </summary>
        /// <param name="start">The <see cref="DateTime"/> where this <see cref="DateTimeSpan"/> starts.</param>
        /// <param name="end">The <see cref="DateTime"/> where this <see cref="DateTimeSpan"/> ends.</param>
        public DateTimeSpan(DateTime start, DateTime end) : this(new DateTimeOffset(start), new DateTimeOffset(end))
        { }

        /// <summary>
        /// Creates a new <see cref="DateTimeSpan"/> from a <see cref="DateTimeOffset"/> and a length.
        /// </summary>
        /// <param name="start">The <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> starts.</param>
        /// <param name="duration">The <see cref="TimeSpan"/> length of this <see cref="DateTimeSpan"/>.</param>
        public DateTimeSpan(DateTimeOffset start, TimeSpan duration) : this(start, start + duration)
        { }

        /// <summary>
        /// Creates a new <see cref="DateTimeSpan"/> from a <see cref="DateTime"/> and a length.
        /// </summary>
        /// <param name="start">The <see cref="DateTime"/> where this <see cref="DateTimeSpan"/> starts.</param>
        /// <param name="duration">The <see cref="TimeSpan"/> length of this <see cref="DateTimeSpan"/>.</param>
        public DateTimeSpan(DateTime start, TimeSpan duration) : this(new DateTimeOffset(start), duration)
        { }

        /// <summary>
        /// Gets the <see cref="TimeSpan"/> duration between the two events this <see cref="DateTimeSpan"/> encapsulates.
        /// </summary>
        public TimeSpan Duration => EndDate - StartDate;

        /// <summary>
        /// Gets a <see cref="bool"/> value indicating whether this <see cref="DateTimeSpan"/> represents a period which is reversed (the <see cref="StartDate"/> is before the <see cref="EndDate"/>).
        /// </summary>
        public bool IsReversed => StartDate > EndDate;

        /// <summary>
        /// Checks if this <see cref="DateTimeSpan"/> occurs wholly within a given <see cref="DateTimeSpan"/>. Assumes <see cref="IsReversed"/> is false.
        /// </summary>
        /// <param name="span">The larger, potentially encapsulating <see cref="DateTimeSpan"/>.</param>
        /// <returns>A <see cref="bool"/> indicating whether <c>this</c> occurs wholly inside of <paramref name="span"/>.</returns>
        public bool IsWithin(DateTimeSpan span)
            => this.StartDate >= span.StartDate && this.EndDate <= span.EndDate;

        /// <summary>
        /// Checks if this <see cref="DateTimeSpan"/> shares any non-zero period with a given <see cref="DateTimeSpan"/>. Assumes <see cref="IsReversed"/> is false.
        /// This operation is commutative - if <c>a</c> intersects <c>b</c>, then <c>b</c> must also intersect <c>a</c>.
        /// </summary>
        /// <param name="span">The potentially intersecting other <see cref="DateTimeSpan"/>.</param>
        /// <returns>A <see cref="bool"/> indicating whether <c>this</c> intersects with <paramref name="span"/>.</returns>
        public bool Intersects(DateTimeSpan span)
            => (this.StartDate < span.EndDate && this.EndDate > span.StartDate) || (span.StartDate < this.EndDate && span.EndDate > this.StartDate);

        /// <summary>
        /// Calculates the pair of <see cref="DateTimeSpan"/>s which represent all time except for that which is defined by this <see cref="DateTimeSpan"/>.
        /// Note that if <see cref="StartDate"/> or <see cref="EndDate"/> are at the DateTime bounds (<see cref="DateTime.MaxValue"/> and <see cref="DateTime.MinValue"/>), the result may include zero or one elements instead.
        /// </summary>
        /// <returns>An array of <see cref="DateTimeSpan"/>s (with a length of usually two, but always between zero and two) covering all area except for that which is covered in <see cref="DateTimeSpan"/>). The endpoints are included in this result.</returns>
        public DateTimeSpan[] Inverse()
        {
            if (this.StartDate == DateTimeOffset.MinValue && this.EndDate == DateTimeOffset.MaxValue) return Array.Empty<DateTimeSpan>();
            else if (this.StartDate == DateTimeOffset.MinValue) return new[] { new DateTimeSpan(this.EndDate, DateTimeOffset.MaxValue) };
            else if (this.EndDate == DateTimeOffset.MaxValue) return new[] { new DateTimeSpan(DateTimeOffset.MinValue, this.StartDate) };
            else
            {
                return new[]
                {
                    new DateTimeSpan(DateTimeOffset.MinValue, this.StartDate),
                    new DateTimeSpan(this.EndDate, DateTimeOffset.MaxValue)
                };
            }
        }

        /// <summary>
        /// Checks whether a given <see cref="DateTimeOffset"/> is included in the <see cref="DateTimeSpan"/>.
        /// </summary>
        /// <param name="point">The <see cref="DateTimeOffset"/> point being queried.</param>
        /// <returns>A <see cref="bool"/> indicating whether <paramref name="point"/> is within the bounds of this <see cref="DateTimeSpan"/>.</returns>
        public bool Includes(DateTimeOffset point)
            => this.StartDate >= point && this.EndDate <= point;

        /// <inheritdoc/>
        public int CompareTo(DateTimeSpan other)
        {
            var startComp = this.StartDate.CompareTo(other.StartDate);
            return startComp == 0 ? this.EndDate.CompareTo(other.EndDate) : startComp;
        }
    }

    /// <summary>
    /// Provides extension methods for dealing with complex <see cref="DateTimeSpan"/> operations.
    /// </summary>
    public static class DateTimeSpanExtensions
    {
        /// <summary>
        /// Normalizes a collection of <see cref="DateTimeSpan"/>s by removing all duplicate periods within the collection, as well as merging any adjacent <see cref="DateTimeSpan"/>s into a single value.
        /// </summary>
        /// <param name="spans">The unnormalized collection of <see cref="DateTimeSpan"/>s.</param>
        /// <returns>A collection of <see cref="DateTimeSpan"/>s equivalent to <paramref name="spans"/>, but ordered by <see cref="DateTimeSpan.StartDate"/> and without any duplicate or adjacent items. The length of the result must be equal to or less than the length of <paramref name="spans"/>.</returns>
        public static IEnumerable<DateTimeSpan> Normalize(this IEnumerable<DateTimeSpan> spans)
        {
            DateTimeSpan? previous = null;
            foreach(var span in spans.Distinct()
                .Where(s => s.StartDate < s.EndDate)
                .OrderBy(s => s))
            {
                if (previous == null) previous = span;
                else if (previous.Value.EndDate >= span.StartDate)
                {
                    if (span.EndDate > previous.Value.EndDate)
                    {
                        previous = previous.Value with
                        {
                            EndDate = span.EndDate
                        };
                    }
                }
                else
                {
                    yield return previous.Value;
                    previous = span;
                }
            }

            if (previous != null) yield return previous.Value;
        }

        /// <summary>
        /// Splits this given <see cref="DateTimeSpan"/> into component <see cref="DateTimeSpan"/>s by day.
        /// </summary>
        /// <param name="span">The <see cref="DateTimeSpan"/> being queried.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="DateTimeSpan"/> periods which together are equivalent to <paramref name="span"/>. Each one is wholly contained within a single day.</returns>
        public static IEnumerable<DateTimeSpan> Days(this DateTimeSpan span)
        {
            var currentDate = span.StartDate;
            var nextDate = currentDate.AddDays(1);
            while (nextDate < span.EndDate)
            {
                yield return new DateTimeSpan(currentDate, nextDate);
                currentDate = nextDate;
                nextDate = nextDate.AddDays(1);
            }

            if (currentDate < span.EndDate)
            {
                yield return new DateTimeSpan(currentDate, span.EndDate);
            }
        }
        
        /// <summary>
        /// Splits this given <see cref="DateTimeSpan"/> into component <see cref="DateTimeSpan"/>s by calendar month.
        /// </summary>
        /// <param name="span">The <see cref="DateTimeSpan"/> being queried.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="DateTimeSpan"/> periods which together are equivalent to <paramref name="span"/>. Each one is wholly contained within a single month.</returns>
        public static IEnumerable<DateTimeSpan> Months(this DateTimeSpan span)
        {
            var currentDate = span.StartDate;
            var nextDate = currentDate.AddMonths(1);
            while (nextDate < span.EndDate)
            {
                yield return new DateTimeSpan(currentDate, nextDate);
                currentDate = nextDate;
                nextDate = nextDate.AddMonths(1);
            }

            if (currentDate < span.EndDate)
            {
                yield return new DateTimeSpan(currentDate, span.EndDate);
            }
        }
        
        /// <summary>
        /// Splits this given <see cref="DateTimeSpan"/> into component <see cref="DateTimeSpan"/>s by calendar year.
        /// </summary>
        /// <param name="span">The <see cref="DateTimeSpan"/> being queried.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="DateTimeSpan"/> periods which together are equivalent to <paramref name="span"/>. Each one is wholly contained within a single year.</returns>
        public static IEnumerable<DateTimeSpan> Years(this DateTimeSpan span)
        {
            var currentDate = span.StartDate;
            var nextDate = currentDate.AddYears(1);
            while (nextDate < span.EndDate)
            {
                yield return new DateTimeSpan(currentDate, nextDate);
                currentDate = nextDate;
                nextDate = nextDate.AddYears(1);
            }
            
            if (currentDate < span.EndDate)
            {
                yield return new DateTimeSpan(currentDate, span.EndDate);
            }
        }
    }
}
