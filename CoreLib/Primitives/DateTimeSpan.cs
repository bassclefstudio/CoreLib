namespace BassClefStudio.CoreLib.Primitives
{
    /// <summary>
    /// Represents a contiguous block of time between two <see cref="DateTimeOffset"/>s.
    /// </summary>
    /// <param name="StartDate">Represents the beginning <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> starts.</param>
    /// <param name="EndDate">Represents the final <see cref="DateTimeOffset"/> where this <see cref="DateTimeSpan"/> ends.</param>
    public record struct DateTimeSpan(DateTimeOffset StartDate, DateTimeOffset EndDate)
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
    }
}
