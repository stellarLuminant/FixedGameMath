namespace FixedGameMath
{
    /// <summary>
    ///     An interval between two Fix64 values, with inclusive bounds.
    ///     Offers two sets of transformations- normal and unchecked.
    ///     Normal ensures that invariants are respected with any input, while
    ///     Unchecked eliminates an extra branch per function assuming the user
    ///     will always provide correct inputs.
    /// </summary>
    public readonly struct FixInterval
    {
        /// <summary>
        ///     A zero-width interval at Fix64.Zero.
        /// </summary>
        public static readonly FixInterval Zero = new FixInterval(Fix64.Zero, Fix64.Zero);

        /// <summary>
        ///     A zero-width interval at Fix64.MinValue.
        /// </summary>
        public static readonly FixInterval MinimumBound = new FixInterval(Fix64.MinValue, Fix64.MinValue);

        /// <summary>
        ///     A zero-width interval at Fix64.MaxValue.
        /// </summary>
        public static readonly FixInterval MaximumBound = new FixInterval(Fix64.MaxValue, Fix64.MaxValue);

        /// <summary>
        ///     An interval that spans the entire range of Fix64.
        /// </summary>
        public static readonly FixInterval FullRange = new FixInterval(Fix64.MinValue, Fix64.MaxValue);

        /// <summary>
        ///     Gets the lower value of the interval.
        /// </summary>
        public readonly Fix64 Lower;

        /// <summary>
        ///     Gets the upper value of the interval.
        /// </summary>
        public readonly Fix64 Upper;

        private FixInterval(Fix64 lower, Fix64 upper)
        {
            Lower = lower;
            Upper = upper;
        }

        #region Create functions

        /// <summary>
        ///     Constructs an interval without checking bounds.
        ///     This can be useful in performance-heavy contexts where
        ///     (upper > lower) is guaranteed by the calling code.
        /// </summary>
        public static FixInterval CreateUnchecked(Fix64 lower, Fix64 upper)
        {
            return new FixInterval(lower, upper);
        }

        /// <summary>
        ///     Constructs an interval.
        /// </summary>
        public static FixInterval Create(Fix64 value1, Fix64 value2)
        {
            return value1 > value2 ? new FixInterval(value2, value1) : new FixInterval(value1, value2);
        }

        #endregion

        #region Operators

        public static bool operator ==(FixInterval lhs, FixInterval rhs)
        {
            return lhs.Upper == rhs.Upper && lhs.Lower == rhs.Lower;
        }

        public static bool operator !=(FixInterval lhs, FixInterval rhs)
        {
            return lhs.Upper != rhs.Upper || lhs.Lower != rhs.Lower;
        }

        #endregion

        #region Binary operations

        /// <summary>
        ///     Checks if an interval and a value overlap.
        /// </summary>
        public static bool Overlaps(FixInterval interval, Fix64 value)
        {
            return value >= interval.Lower && value <= interval.Upper;
        }

        /// <summary>
        ///     Checks if this FixInterval overlaps with another.
        /// </summary>
        public static bool Overlaps(FixInterval x, FixInterval y)
        {
            return x.Lower <= y.Upper && y.Lower <= x.Upper;
        }

        /// <summary>
        ///     Computes the intersection of lhs and rhs and if true,
        ///     returns true and stores it in the out param.
        ///     If no intersection is found, returns false.
        /// </summary>
        public static bool Intersection(FixInterval lhs, FixInterval rhs, out FixInterval intersection)
        {
            var max = Fix64.Min(lhs.Upper, rhs.Upper);
            var min = Fix64.Max(lhs.Lower, rhs.Lower);
            if (min > max)
            {
                intersection = Zero;
                return false;
            }

            intersection = new FixInterval(min, max);
            return true;
        }

        /// <summary>
        ///     Computes the smallest interval that fully contains both intervals.
        /// </summary>
        public static FixInterval Union(FixInterval lhs, FixInterval rhs)
        {
            return new FixInterval(Fix64.Max(lhs.Upper, rhs.Upper), Fix64.Min(lhs.Lower, rhs.Lower));
        }

        /// <summary>
        ///     Gets the minimum translation that moves x to the nearest bound of y.
        ///     If x and y do not overlap, returns zero.
        /// </summary>
        public Fix64 MinimumTranslation(FixInterval x, FixInterval y)
        {
            if (!Overlaps(x, y))
                return Fix64.Zero;

            var a = y.Upper - x.Lower;
            var b = x.Upper - y.Lower;

            return a < b ? a : -b;
        }

        #endregion

        #region Transformation

        /// <summary>
        ///     Sets the lower bound of the interval. Useful in performance-heavy contexts
        ///     where (input &lt;= upper) is guaranteed.
        /// </summary>
        public FixInterval SetLowerUnchecked(Fix64 x)
        {
            return new FixInterval(x, Upper);
        }

        /// <summary>
        ///     Sets the upper bound of the interval. Useful in performance-heavy contexts
        ///     where (input >= lower) is guaranteed.
        /// </summary>
        public FixInterval SetUpperUnchecked(Fix64 x)
        {
            return new FixInterval(Lower, x);
        }

        /// <summary>
        ///     Sets the lower bound of the interval. If this value is
        ///     greater than Upper, the existing Upper bound becomes the new Lower bound,
        ///     and the input is set to the Upper bound.
        /// </summary>
        public FixInterval SetLower(Fix64 x)
        {
            return x > Upper ? new FixInterval(Upper, x) : new FixInterval(x, Upper);
        }

        /// <summary>
        ///     Sets the upper bound of the interval. If this value is
        ///     lesser than Lower, the existing Lower bound becomes the new Upper bound,
        ///     and the input is set to the Lower bound.
        /// </summary>
        public FixInterval SetUpper(Fix64 x)
        {
            return x < Lower ? new FixInterval(x, Lower) : new FixInterval(Lower, x);
        }

        /// <summary>
        ///     Adds to the lower bound of the interval. Useful in performance-heavy contexts
        ///     where (lower + value &lt;= upper) is guaranteed.
        /// </summary>
        public FixInterval AddLowerUnchecked(Fix64 value)
        {
            return SetLowerUnchecked(Lower + value);
        }

        /// <summary>
        ///     Adds to the upper bound of the interval. Useful in performance-heavy contexts
        ///     where (upper + value >= lower) is guaranteed.
        /// </summary>
        public FixInterval AddUpperUnchecked(Fix64 value)
        {
            return SetUpperUnchecked(Upper + value);
        }

        /// <summary>
        ///     Adds to the lower bound of the interval.
        ///     The bounds are properly reoriented if necessary.
        /// </summary>
        public FixInterval AddLower(Fix64 value)
        {
            return SetLower(Lower + value);
        }

        /// <summary>
        ///     Adds to the upper bound of the interval.
        ///     The bounds are properly reoriented if necessary.
        /// </summary>
        public FixInterval AddUpper(Fix64 value)
        {
            return SetUpper(Upper + value);
        }

        /// <summary>
        ///     Adds to both bounds of the interval.
        /// </summary>
        public FixInterval Translate(Fix64 value)
        {
            return new FixInterval(Lower + value, Upper + value);
        }

        #endregion

        #region Object overrides

        public override bool Equals (object obj)
        {
            return obj is FixInterval interval && this == interval;
        }

        public override int GetHashCode ()
        {
            unchecked
            {
                var hashCode = -1959444751;
                hashCode = hashCode * -1521134295 + Lower.GetHashCode();
                hashCode = hashCode * -1521134295 + Upper.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"[{Lower}, {Upper}]";
        }

        #endregion
    }
}
