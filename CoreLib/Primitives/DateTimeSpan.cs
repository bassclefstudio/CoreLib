namespace BassClefStudio.CoreLib.Primitives
{
    /// <summary>
    /// Represents a contiguous block of time between two <see cref="DateTimeOffset"/>s.
    /// </summary>
    /// <param name="StartDate">Represents the beginning <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> starts.</param>
    /// <param name="EndDate">Represents the final <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> ends.</param>
    public readonly record struct DateTimeSpan(DateTimeOffset StartDate, DateTimeOffset EndDate) : IComparable<DateTimeSpan>
    {
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
        /// Intersects this <see cref="DateTimeSpan"/> with another, returning the shared region of time (if any) between them. This is a commutative operation. Assumes <see cref="IsReversed"/> is false.
        /// </summary>
        /// <param name="other">The other <see cref="DateTimeSpan"/> being intersected.</param>
        /// <returns>A <see cref="DateTimeSpan"/> representing the period(s) of time shared by the two <see cref="DateTimeSpan"/>s. If <c>null</c>, the two regions did not intersect at all.</returns>
        public DateTimeSpan? Intersect(DateTimeSpan other)
        {
            if (this.IsWithin(other)) return this;
            else if (other.IsWithin(this)) return other;
            else if (this.StartDate < other.StartDate && other.StartDate < this.EndDate)
            { 
                return new DateTimeSpan(other.StartDate, this.EndDate);
            }
            else if (this.StartDate < other.EndDate && other.EndDate < this.EndDate)
            {
                return new DateTimeSpan(this.StartDate, other.EndDate);
            }
            else return null;
        }

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
        /// Returns the collection of <see cref="DateTimeSpan"/>s representing all contiguous periods of time during which at least one <see cref="DateTimeSpan"/> from <paramref name="events"/> and one <see cref="DateTimeSpan"/> from <paramref name="others"/> simultaneously occur. 
        /// </summary>
        /// <param name="events">The first (main) set of <see cref="DateTimeSpan"/>s.</param>
        /// <param name="others">The second set of <see cref="DateTimeSpan"/>s to intersect <paramref name="events"/> with.</param>
        /// <returns>The resulting set of <see cref="DateTimeSpan"/>s.</returns>
        public static IEnumerable<DateTimeSpan> Intersect(
            this IEnumerable<DateTimeSpan> events,
            IEnumerable<DateTimeSpan> others)
        {
            DateTimeSpan[] othersArray = others.ToArray();
            foreach (var spanA in events)
            {
                foreach (var spanB in othersArray)
                {
                    DateTimeSpan? shared = spanA.Intersect(spanB);
                    if (shared.HasValue)
                    {
                        yield return shared.Value;
                    }
                }
            }
        }
        
        /// <summary>
        /// Returns the collection of <see cref="DateTimeSpan"/>s representing all contiguous periods of time during which one <see cref="DateTimeSpan"/> in <paramref name="events"/> occurs and no <see cref="DateTimeSpan"/> from <paramref name="others"/> simultaneously occur. 
        /// </summary>
        /// <param name="events">The first (main) set of <see cref="DateTimeSpan"/>s.</param>
        /// <param name="others">The second set of <see cref="DateTimeSpan"/>s to subtract from <paramref name="events"/>.</param>
        /// <returns>The resulting set of <see cref="DateTimeSpan"/>s.</returns>
        public static IEnumerable<DateTimeSpan> Difference(
            this IEnumerable<DateTimeSpan> events,
            IEnumerable<DateTimeSpan> others)
        {
            DateTimeSpan[] othersArray = others.ToArray();
            foreach (var spanA in events)
            {
                foreach (var spanB in othersArray)
                {
                    DateTimeSpan? shared = spanA.Intersect(spanB);
                    if (shared.HasValue)
                    {
                        yield return shared.Value;
                    }
                }
            }
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
