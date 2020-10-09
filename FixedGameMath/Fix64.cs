using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FixedGameMath
{
    /// <summary>
    /// Represents a Q31.32 fixed-point number.
    /// </summary>
    public partial struct Fix64 : IEquatable<Fix64>, IComparable<Fix64>
    {
        readonly long _rawValue;

        /// <summary>
        /// Smallest absolute value of this type
        /// </summary>
        public static readonly Fix64 PrecisionUnit = new Fix64(1L);

        /// <summary>
        /// Smallest absolute value of this type in decimal.
        /// It is 2^-32, or 0.00000000023283064365386962890625
        /// </summary>
        public static readonly decimal PrecisionDecimal = (decimal)PrecisionUnit;

        /// <summary>
        /// Greatest value of this type
        /// </summary>
        public static readonly Fix64 MaxValue = new Fix64(MAX_VALUE);
        
        /// <summary>
        /// Lowest value of this type
        /// </summary>
        public static readonly Fix64 MinValue = new Fix64(MIN_VALUE);

        public static readonly Fix64 One = new Fix64(ONE);
        public static readonly Fix64 MinusOne = new Fix64(-ONE);
        public static readonly Fix64 Two = new Fix64(TWO);

        /// <summary>
        /// 1 / 2 or 0.5
        /// </summary>
        public static readonly Fix64 Half = new Fix64(HALF);
        public static readonly Fix64 Zero = new Fix64();

        /// <summary>
        /// Value of Pi
        /// </summary>
        public static readonly Fix64 Pi = new Fix64(PI);

        /// <summary>
        /// Equivalent to Pi / 2
        /// </summary>
        public static readonly Fix64 PiOver2 = new Fix64(PI_OVER_2);

        /// <summary>
        /// Equivalent to Pi * 2
        /// </summary>
        public static readonly Fix64 PiTimes2 = new Fix64(PI_TIMES_2);

        /// <summary>
        /// Equivalent to 1 / Pi
        /// </summary>
        public static readonly Fix64 PiInv = new Fix64(PI_INV);

        /// <summary>
        /// Equivalent to 1 / (Pi / 2)
        /// </summary>
        public static readonly Fix64 PiOver2Inv = new Fix64(PI_OVER_2_INV);

        /// <summary>
        /// 360 degrees representing one full rotation.
        /// </summary>
        public static readonly Fix64 FullRot = new Fix64((int)360);

        /// <summary>
        /// 180 degrees representing one half rotation.
        /// </summary>
        public static readonly Fix64 HalfRot = new Fix64((int)180);

        /// <summary>
        /// 90 degrees representing one quarter rotation.
        /// </summary>
        public static readonly Fix64 QuarterRot = new Fix64((int)90);

        /// <summary>
        /// 45 degrees representing 1/8 rotation.
        /// </summary>
        public static readonly Fix64 EighthRot = new Fix64((int)45);

        /// <summary>
        /// Value of E
        /// </summary>
        public static readonly Fix64 E = new Fix64(E_CONST);

        // Equal to 31. 2^31 is the largest representable power of two.
        static readonly Fix64 Log2Max = new Fix64(LOG2MAX);

        // Equal to 32. 2^32 is the smallest representable power of 2.
        static readonly Fix64 Log2Min = new Fix64(LOG2MIN);

        // Equivalent to ln(2)
        static readonly Fix64 Ln2 = new Fix64(LN2);

        const long MAX_VALUE = Int64.MaxValue;
        const long MIN_VALUE = Int64.MinValue;
        const int NUM_BITS = 64;
        const int FRACTIONAL_PLACES = 32;
        const long ONE = 1L << FRACTIONAL_PLACES;
        const long TWO = 2L << FRACTIONAL_PLACES;
        const long HALF = 1L << (FRACTIONAL_PLACES - 1);
        const long PI_TIMES_2 = 0x6487ED511;
        const long PI = 0x3243F6A88;
        const long PI_OVER_2 = 0x1921FB544;
        const long PI_INV = 0x517CC1B7;
        const long PI_OVER_2_INV = 0xA2F9836E;
        const long LN2 = 0xB17217F7;
        const long LOG2MAX =  0x1F00000000;
        const long LOG2MIN = -0x2000000000;
        const long E_CONST = 0x2B7E15162;

        const int LUT_FRACTION_BITS = 10;
        const int LUT_ANGLE_TO_INDEX_BITS = FRACTIONAL_PLACES - LUT_FRACTION_BITS;
        static readonly long LutInputPrecision = ONE >> LUT_FRACTION_BITS;
        static readonly uint LutSize = (uint)(int)(QuarterRot._rawValue >> LUT_ANGLE_TO_INDEX_BITS);

        /// <summary>
        /// Converts an angle measure in radians to degrees.
        /// </summary>
        public static Fix64 ToDegrees(Fix64 radians)
        {
            return HalfRot * radians / Pi;
        }

        /// <summary>
        /// Converts an angle measure in degrees to radians.
        /// </summary>
        public static Fix64 ToRadians(Fix64 degrees)
        {
            return Pi * degrees / HalfRot;
        }

        /// <summary>
        /// Returns a number indicating the sign of a Fix64 number.
        /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
        /// </summary>
        public static int Sign(Fix64 value)
        {
            return
                value._rawValue < 0 ? -1 :
                value._rawValue > 0 ? 1 :
                0;
        }


        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
        /// </summary>
        public static Fix64 Abs(Fix64 value)
        {
            if (value._rawValue == MIN_VALUE)
            {
                return MaxValue;
            }

            // branchless implementation, see http://www.strchr.com/optimized_abs_function
            var mask = value._rawValue >> 63;
            return new Fix64((value._rawValue + mask) ^ mask);
        }

        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// FastAbs(Fix64.MinValue) == Fix64.MinValue
        /// </summary>
        public static Fix64 FastAbs(Fix64 value)
        {
            // branchless implementation, see http://www.strchr.com/optimized_abs_function
            var mask = value._rawValue >> 63;
            return new Fix64((value._rawValue + mask) ^ mask);
        }


        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        public static Fix64 Floor(Fix64 value)
        {
            // Just zero out the fractional part
            return new Fix64((long)((ulong)value._rawValue & 0xFFFFFFFF00000000));
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified number.
        /// </summary>
        public static Fix64 Ceiling(Fix64 value)
        {
            var hasFractionalPart = (value._rawValue & 0x00000000FFFFFFFF) != 0;
            return hasFractionalPart ? Floor(value) + One : value;
        }

        /// <summary>
        /// Rounds a value to the nearest integral value.
        /// If the value is halfway between an even and an uneven value, returns the even value.
        /// </summary>
        public static Fix64 Round(Fix64 value)
        {
            var fractionalPart = value._rawValue & 0x00000000FFFFFFFF;
            var integralPart = Floor(value);
            if (fractionalPart < 0x80000000)
            {
                return integralPart;
            }
            if (fractionalPart > 0x80000000)
            {
                return integralPart + One;
            }
            // if number is halfway between two values, round to the nearest even number
            // this is the method used by System.Math.Round().
            return (integralPart._rawValue & ONE) == 0
                       ? integralPart
                       : integralPart + One;
        }

        /// <summary>
        /// Checks if a value has a fractional component.
        /// </summary>
        public static bool HasFraction(Fix64 value)
        {
            return ((ulong)value._rawValue & 0x00000000FFFFFFFFUL) != 0;
        }

        /// <summary>
        /// Removes the integral portion of the number and returns only the fractional component.
        /// </summary>
        public static Fix64 Fraction(Fix64 value)
        {
            return new Fix64(value._rawValue % ONE);
        }

        /// <summary>
        /// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        public static Fix64 operator +(Fix64 x, Fix64 y)
        {
            var xl = x._rawValue;
            var yl = y._rawValue;
            var sum = xl + yl;
            // if signs of operands are equal and signs of sum and x are different
            if (((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0)
            {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
            }
            return new Fix64(sum);
        }

        /// <summary>
        /// Adds x and y without performing overflow checking. Should be inlined by the CLR.
        /// </summary>
        public static Fix64 FastAdd(Fix64 x, Fix64 y)
        {
            return new Fix64(x._rawValue + y._rawValue);
        }

        /// <summary>
        /// Subtracts y from x. Performs saturating subtraction, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        public static Fix64 operator -(Fix64 x, Fix64 y)
        {
            var xl = x._rawValue;
            var yl = y._rawValue;
            var diff = xl - yl;
            // if signs of operands are different and signs of sum and x are different
            if ((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0)
            {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
            }
            return new Fix64(diff);
        }

        /// <summary>
        /// Subtracts y from x without performing overflow checking. Should be inlined by the CLR.
        /// </summary>
        public static Fix64 FastSub(Fix64 x, Fix64 y)
        {
            return new Fix64(x._rawValue - y._rawValue);
        }

        static long AddOverflowHelper(long x, long y, ref bool overflow)
        {
            var sum = x + y;
            // x + y overflows if sign(x) ^ sign(y) != sign(sum)
            overflow |= ((x ^ y ^ sum) & MIN_VALUE) != 0;
            return sum;
        }

        /// <summary>
        /// Multiplies x and y and uses saturating multiplication,
        /// using MinValue or MaxValue in case of overflow.
        /// </summary>
        public static Fix64 operator *(Fix64 x, Fix64 y)
        {

            var xl = x._rawValue;
            var yl = y._rawValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            bool overflow = false;
            var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
            sum = AddOverflowHelper(sum, midResult2, ref overflow);
            sum = AddOverflowHelper(sum, hiResult, ref overflow);

            bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

            // if signs of operands are equal and sign of result is negative,
            // then multiplication overflowed positively
            // the reverse is also true
            if (opSignsEqual)
            {
                if (sum < 0 || (overflow && xl > 0))
                {
                    return MaxValue;
                }
            }
            else
            {
                if (sum > 0)
                {
                    return MinValue;
                }
            }

            // if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
            // then this means the result overflowed.
            var topCarry = hihi >> FRACTIONAL_PLACES;
            if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/)
            {
                return opSignsEqual ? MaxValue : MinValue;
            }

            // If signs differ, both operands' magnitudes are greater than 1,
            // and the result is greater than the negative operand, then there was negative overflow.
            if (!opSignsEqual)
            {
                long posOp, negOp;
                if (xl > yl)
                {
                    posOp = xl;
                    negOp = yl;
                }
                else
                {
                    posOp = yl;
                    negOp = xl;
                }
                if (sum > negOp && negOp < -ONE && posOp > ONE)
                {
                    return MinValue;
                }
            }

            return new Fix64(sum);
        }

        /// <summary>
        /// Performs multiplication without checking for overflow.
        /// Useful for performance-critical code where the values are guaranteed not to cause overflow
        /// </summary>
        public static Fix64 FastMul(Fix64 x, Fix64 y)
        {

            var xl = x._rawValue;
            var yl = y._rawValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            var sum = (long)loResult + midResult1 + midResult2 + hiResult;
            return new Fix64(sum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CountLeadingZeroes(ulong x)
        {
            int result = 0;
            while ((x & 0xF000000000000000) == 0) { result += 4; x <<= 4; }
            while ((x & 0x8000000000000000) == 0) { result += 1; x <<= 1; }
            return result;
        }

        /// <summary>
        /// Divides x by y using saturating division.
        /// MinValue and MaxValue are used in case of overflow.
        /// </summary>
        public static Fix64 operator /(Fix64 x, Fix64 y)
        {
            var xl = x._rawValue;
            var yl = y._rawValue;

            if (yl == 0)
            {
                throw new DivideByZeroException();
            }

            var remainder = (ulong)(xl >= 0 ? xl : -xl);
            var divider = (ulong)(yl >= 0 ? yl : -yl);
            var quotient = 0UL;
            var bitPos = NUM_BITS / 2 + 1;


            // If the divider is divisible by 2^n, take advantage of it.
            while ((divider & 0xF) == 0 && bitPos >= 4)
            {
                divider >>= 4;
                bitPos -= 4;
            }

            while (remainder != 0 && bitPos >= 0)
            {
                int shift = CountLeadingZeroes(remainder);
                if (shift > bitPos)
                {
                    shift = bitPos;
                }
                remainder <<= shift;
                bitPos -= shift;

                var div = remainder / divider;
                remainder = remainder % divider;
                quotient += div << bitPos;

                // Detect overflow
                if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
                {
                    return ((xl ^ yl) & MIN_VALUE) == 0 ? MaxValue : MinValue;
                }

                remainder <<= 1;
                --bitPos;
            }

            // rounding
            ++quotient;
            var result = (long)(quotient >> 1);
            if (((xl ^ yl) & MIN_VALUE) != 0)
            {
                result = -result;
            }

            return new Fix64(result);
        }

        public static Fix64 operator %(Fix64 x, Fix64 y)
        {
            return new Fix64(
                x._rawValue == MIN_VALUE & y._rawValue == -1 ?
                0 :
                x._rawValue % y._rawValue);
        }

        /// <summary>
        /// Performs modulo as fast as possible; throws if x == MinValue and y == -1.
        /// Use the operator (%) for a more reliable but slower modulo.
        /// </summary>
        public static Fix64 FastMod(Fix64 x, Fix64 y)
        {
            return new Fix64(x._rawValue % y._rawValue);
        }

        /// <summary>
        /// Negates the current value.
        /// MinValue is negated to MaxValue.
        /// </summary>
        public static Fix64 operator -(Fix64 x)
        {
            return x._rawValue == MIN_VALUE ? MaxValue : new Fix64(-x._rawValue);
        }

        public static bool operator ==(Fix64 x, Fix64 y)
        {
            return x._rawValue == y._rawValue;
        }

        public static bool operator !=(Fix64 x, Fix64 y)
        {
            return x._rawValue != y._rawValue;
        }

        public static bool operator >(Fix64 x, Fix64 y)
        {
            return x._rawValue > y._rawValue;
        }

        public static bool operator <(Fix64 x, Fix64 y)
        {
            return x._rawValue < y._rawValue;
        }

        public static bool operator >=(Fix64 x, Fix64 y)
        {
            return x._rawValue >= y._rawValue;
        }

        public static bool operator <=(Fix64 x, Fix64 y)
        {
            return x._rawValue <= y._rawValue;
        }

        /// <summary>
        /// Returns 2 raised to the specified power.
        /// Provides at least 6 decimals of accuracy.
        /// Values beyond (-31, 31) will return PrecisionUnit or MaxValue respectively.
        /// </summary>
        public static Fix64 Pow2(Fix64 x)
        {
            // Constant inputs to be optimized.
            if (x._rawValue == 0)
            {
                return One;
            }

            if (x == One)
            {
                return Two;
            }

            if (x == MinusOne)
            {
                return Half;
            }

            // If the power is greater than 31 (more bits than we can store)
            // return the highest value we can.
            if (x >= Log2Max)
            {
                return MaxValue;
            }

            // if the power is lower than -31 (we can store 1 more bit, but by then the decimal doesn't matter)
            // return the lowest absolute value we can.
            if (x < Log2Min + One)
            {
                return PrecisionUnit;
            }

            // Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
            bool neg = x._rawValue < 0;
            if (neg)
            {
                x = -x;
            }

            /* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */

            int integerPart = (int)Floor(x);
            // Take fractional part of exponent
            x = new Fix64(x._rawValue & 0x00000000FFFFFFFF);

            var result = One;
            var term = One;
            int i = 1;
            while (term._rawValue != 0)
            {
                term = FastMul(FastMul(x, term), Ln2) / (Fix64)i;
                result += term;
                i++;
            }

            result = FromRaw(result._rawValue << integerPart);
            if (neg)
            {
                result = One / result;
            }

            return result;
        }

        /// <summary>
        /// Returns the base-2 logarithm of a specified number.
        /// Provides at least 9 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive.
        /// </exception>
        public static Fix64 Log2(Fix64 x)
        {
            if (x._rawValue <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "Non-positive value passed to Ln");
            }

            // This implementation is based on Clay. S. Turner's fast binary logarithm
            // algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
            //     Processing Mag., pp. 124,140, Sep. 2010.)

            long b = 1U << (FRACTIONAL_PLACES - 1);
            long y = 0;

            long rawX = x._rawValue;
            while (rawX < ONE)
            {
                rawX <<= 1;
                y -= ONE;
            }

            while (rawX >= (ONE << 1))
            {
                rawX >>= 1;
                y += ONE;
            }

            var z = new Fix64(rawX);

            for (int i = 0; i < FRACTIONAL_PLACES; i++)
            {
                z = FastMul(z, z);
                if (z._rawValue >= (ONE << 1))
                {
                    z = new Fix64(z._rawValue >> 1);
                    y += b;
                }
                b >>= 1;
            }

            return new Fix64(y);
        }

        /// <summary>
        /// Returns the natural logarithm of a specified number.
        /// Provides at least 7 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        public static Fix64 Ln(Fix64 x)
        {
            return FastMul(Log2(x), Ln2);
        }

        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// Provides about 5 digits of accuracy for the result.
        /// </summary>
        /// <exception cref="DivideByZeroException">
        /// The base was zero with a negative exponent
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The base was negative, with a non-zero exponent
        /// </exception>
        public static Fix64 Pow(Fix64 b, Fix64 exp)
        {
            if (b == One)
            {
                return One;
            }
            if (exp._rawValue == 0)
            {
                return One;
            }
            if (b._rawValue == 0)
            {
                if (exp._rawValue < 0)
                {
                    throw new DivideByZeroException("The base was zero with a negative exponent");
                }
                return Zero;
            }

            Fix64 log2 = Log2(b);
            return Pow2(exp * log2);
        }

        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was negative.
        /// </exception>
        public static Fix64 Sqrt(Fix64 x)
        {
            var xl = x._rawValue;
            if (xl < 0)
            {
                // We cannot represent infinities like Single and Double, and Sqrt is
                // mathematically undefined for x < 0. So we just throw an exception.
                throw new ArgumentOutOfRangeException(nameof(x), "Negative value passed to Sqrt");
            }

            var num = (ulong)xl;
            var result = 0UL;

            // second-to-top bit
            var bit = 1UL << (NUM_BITS - 2);

            while (bit > num)
            {
                bit >>= 2;
            }

            // The main part is executed twice, in order to avoid
            // using 128 bit values in computations.
            for (var i = 0; i < 2; ++i)
            {
                // First we get the top 48 bits of the answer.
                while (bit != 0)
                {
                    if (num >= result + bit)
                    {
                        num -= result + bit;
                        result = (result >> 1) + bit;
                    }
                    else
                    {
                        result = result >> 1;
                    }
                    bit >>= 2;
                }

                if (i == 0)
                {
                    // Then process it again to get the lowest 16 bits.
                    if (num > (1UL << (NUM_BITS / 2)) - 1)
                    {
                        // The remainder 'num' is too large to be shifted left
                        // by 32, so we have to add 1 to result manually and
                        // adjust 'num' accordingly.
                        // num = a - (result + 0.5)^2
                        //       = num + result^2 - (result + 0.5)^2
                        //       = num - result - 0.5
                        num -= result;
                        num = (num << (NUM_BITS / 2)) - 0x80000000UL;
                        result = (result << (NUM_BITS / 2)) + 0x80000000UL;
                    }
                    else
                    {
                        num <<= (NUM_BITS / 2);
                        result <<= (NUM_BITS / 2);
                    }

                    bit = 1UL << (NUM_BITS / 2 - 2);
                }
            }
            // Finally, if next bit would have been 1, round the result upwards.
            if (num > result)
            {
                ++result;
            }
            return new Fix64((long)result);
        }

        /// <summary>
        /// Returns the Sine of x.
        /// </summary>
        public static Fix64 Sin(Fix64 x)
        {
            var clampedL = ClampSinValue(x._rawValue, out bool flipHorizontal, out bool flipVertical);

            // Here we use the fact that the SinLut table has a number of entries
            // equal to (90 << 10) to use the angle to index directly into it
            var rawIndex = (uint)(clampedL >> LUT_ANGLE_TO_INDEX_BITS);

            // Special case handling since sin(90) isn't stored in the table itself.
            if (rawIndex == 0 && flipHorizontal)
                return flipVertical ? -One : One;

            var nearestValue = SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)rawIndex :
                (int)rawIndex];
            return new Fix64(flipVertical ? -nearestValue : nearestValue);
        }

        /// <summary>
        /// Clamps an angle to [0-90] in raw fixed point and outputs needed flip operations for sin(x).
        /// </summary>
        static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical)
        {
            var clampedFullRot = angle;

            clampedFullRot %= FullRot._rawValue;

            if (angle < 0)
                clampedFullRot += FullRot._rawValue;

            // The LUT contains values for 0 - 90; every other value must be obtained by
            // vertical or horizontal mirroring
            flipVertical = clampedFullRot >= HalfRot._rawValue;
            // obtain (angle % 180) from (angle % 360) - much faster than doing another modulo
            var clampedHalfRot = clampedFullRot;
            while (clampedHalfRot >= HalfRot._rawValue)
            {
                clampedHalfRot -= HalfRot._rawValue;
            }
            flipHorizontal = clampedHalfRot >= QuarterRot._rawValue;
            // obtain (angle % 90) from (angle % 180) - much faster than doing another modulo
            var clampedQuarterRot = clampedHalfRot;
            if (clampedQuarterRot >= QuarterRot._rawValue)
            {
                clampedQuarterRot -= QuarterRot._rawValue;
            }
            return clampedQuarterRot;
        }

        /// <summary>
        /// Clamps an angle to [0, 90] in raw fixed point and outputs if a flip is needed for tan(x).
        /// </summary>
        static long ClampTanValue(long angle, out bool flip)
        {
            var clampedHalfRot = angle % HalfRot._rawValue;
            flip = false;

            if (clampedHalfRot < 0)
            {
                clampedHalfRot = -clampedHalfRot;
                flip = true;
            }

            if (clampedHalfRot > QuarterRot._rawValue)
            {
                flip = !flip;
                clampedHalfRot = QuarterRot._rawValue - (clampedHalfRot - QuarterRot._rawValue);
            }

            return clampedHalfRot;
        }

        /// <summary>
        /// Returns the cosine of x.
        /// </summary>
        public static Fix64 Cos(Fix64 x)
        {
            var xl = x._rawValue;
            var rawAngle = xl + (xl > 0 ? -HalfRot._rawValue - QuarterRot._rawValue : QuarterRot._rawValue);
            return Sin(new Fix64(rawAngle));
        }

        /// <summary>
        /// Returns the tangent of x.
        /// </summary>
        public static Fix64 Tan(Fix64 x)
        {
            var clampedL = ClampTanValue(x._rawValue, out bool flip);

            // Here we use the fact that the TanLut table has a number of entries
            // equal to (90 << 10) to use the angle to index directly into it
            var rawIndex = (uint)(clampedL >> LUT_ANGLE_TO_INDEX_BITS);

            // If the value of Tan overflows past our table
            // (i.e. 90 degrees) we return the largest value we can.
            if (rawIndex >= LutSize)
                return flip ? MinValue : MaxValue;

            var nearestValue = TanLut[(int)rawIndex];
            return new Fix64(flip ? -nearestValue : nearestValue);
        }

        /// <summary>
        /// Returns the arccos of of the specified number, calculated using Atan and Sqrt
        /// This function has at least 7 decimals of accuracy.
        /// </summary>
        public static Fix64 Acos(Fix64 x)
        {
            if (x < -One || x > One)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (x._rawValue == 0) return QuarterRot;

            var result = Atan(Sqrt(One - x * x) / x);
            return x._rawValue < 0 ? result + HalfRot : result;
        }

        /// <summary>
        /// Returns the arctangent (degrees) of the specified number, calculated using Euler series
        /// This function has at least 7 decimals of accuracy.
        /// </summary>
        public static Fix64 Atan(Fix64 z)
        {
            if (z._rawValue == 0) return Zero;

            // Force positive values for argument
            // Atan(-z) = -Atan(z).
            var neg = z._rawValue < 0;
            if (neg)
            {
                z = -z;
            }

            Fix64 result;
            var two = (Fix64)2;
            var three = (Fix64)3;

            bool invert = z > One;
            if (invert) z = One / z;

            result = One;
            var term = One;

            var zSq = z * z;
            var zSq2 = zSq * two;
            var zSqPlusOne = zSq + One;
            var zSq12 = zSqPlusOne * two;
            var dividend = zSq2;
            var divisor = zSqPlusOne * three;

            for (var i = 2; i < 30; ++i)
            {
                term *= dividend / divisor;
                result += term;

                dividend += zSq2;
                divisor += zSq12;

                if (term._rawValue == 0) break;
            }

            result = ToDegrees(result * z / zSqPlusOne);

            if (invert)
            {
                result = QuarterRot - result;
            }

            if (neg)
            {
                result = -result;
            }
            return result;
        }

        public static Fix64 Atan2(Fix64 y, Fix64 x)
        {
            var yl = y._rawValue;
            var xl = x._rawValue;
            if (xl == 0)
            {
                if (yl == 0)
                    return Zero;

                return yl > 0 ? QuarterRot : -QuarterRot;
            }

            var atan = Atan(y / x);

            // Atan(z) already maps to quadrants I and IV
            if (xl >= 0)
                return atan;
            
            // Check the sign of y to determine quadrants II and III
            return atan + (yl < 0 ? -HalfRot: HalfRot);
        }

        public static explicit operator Fix64(long value)
        {
            return new Fix64(value * ONE);
        }
        public static explicit operator long(Fix64 value)
        {
            return value._rawValue >> FRACTIONAL_PLACES;
        }

        public static explicit operator int(Fix64 value)
        {
            return (int)(value._rawValue >> FRACTIONAL_PLACES);
        }

        public static explicit operator Fix64(float value)
        {
            return new Fix64(ConvertToRaw(value));
        }
        public static explicit operator float(Fix64 value)
        {
            return (float)value._rawValue / ONE;
        }
        public static explicit operator Fix64(double value)
        {
            return new Fix64(ConvertToRaw(value));
        }
        public static explicit operator double(Fix64 value)
        {
            return (double)value._rawValue / ONE;
        }
        public static explicit operator Fix64(decimal value)
        {
            return new Fix64(ConvertToRaw(value));
        }
        public static explicit operator decimal(Fix64 value)
        {
            return (decimal)value._rawValue / ONE;
        }

        /// <summary>
        /// Converts floating point values to raw values for Fix64.
        /// Since floating points have very wide dynamic ranges,
        /// high/low values beyond the range of Int64 will be
        /// capped to Int64.MaxValue or Int64.MinValue.
        /// </summary>
        static long ConvertToRaw(float value)
        {
            var product = value * ONE;

            if (product >= long.MaxValue)
                return long.MaxValue;
            if (product <= long.MinValue)
                return long.MinValue;
            return (long)product;
        }

        /// <summary>
        /// Converts floating point values to raw values for Fix64.
        /// Since floating points have very wide dynamic ranges,
        /// high/low values beyond the range of Int64 will be
        /// capped to Int64.MaxValue or Int64.MinValue.
        /// </summary>
        static long ConvertToRaw(double value)
        {
            var product = value * ONE;

            if (product >= long.MaxValue)
                return long.MaxValue;
            if (product <= long.MinValue)
                return long.MinValue;
            return (long)product;
        }

        /// <summary>
        /// Converts floating point values to raw values for Fix64.
        /// Since floating points have very wide dynamic ranges,
        /// high/low values beyond the range of Int64 will be
        /// capped to Int64.MaxValue or Int64.MinValue.
        /// </summary>
        static long ConvertToRaw(decimal value)
        {
            var product = value * ONE;

            if (product >= long.MaxValue)
                return long.MaxValue;
            if (product <= long.MinValue)
                return long.MinValue;
            return (long)product;
        }

        public override bool Equals(object obj)
        {
            return obj is Fix64 && ((Fix64)obj)._rawValue == _rawValue;
        }

        public override int GetHashCode()
        {
            return _rawValue.GetHashCode();
        }

        public bool Equals(Fix64 other)
        {
            return _rawValue == other._rawValue;
        }

        public int CompareTo(Fix64 other)
        {
            return _rawValue.CompareTo(other._rawValue);
        }

        public override string ToString()
        {
            // Up to 10 decimal places
            return ((decimal)this).ToString("0.##########");
        }

        public static Fix64 FromRaw(long rawValue)
        {
            return new Fix64(rawValue);
        }

        // Excluded from code coverage because:
        // - this is not part of the user api
        // - this is a part of implementation of already tested functions
        [ExcludeFromCodeCoverage]
        public static long[] GenerateSinLut()
        {
            var table = new long[LutSize];

            for (var i = 0; i < LutSize; i++)
            {
                var angle = new Fix64(i * LutInputPrecision);
                var sin = Math.Sin((double)angle * Math.PI / 180.0);
                var rawValue = ((Fix64)sin).RawValue;
                
                table[i] = rawValue;
            }

            return table;
        }

        // Excluded from code coverage because:
        // - this is not part of the user api
        // - this is a part of implementation of already tested functions
        [ExcludeFromCodeCoverage]
        public static long[] GenerateTanLut()
        {
            var maxValue = (double)MaxValue;

            var table = new long[LutSize];

            for (var i = 0; i < LutSize; i++)
            {
                var angle = new Fix64(i * LutInputPrecision);
                var tan = Math.Tan((double)angle * Math.PI / 180.0);
                
                var fixedNum = tan > maxValue || tan < 0 ? MaxValue : (Fix64)tan;
                var rawValue = fixedNum.RawValue;

                table[i] = rawValue;
            }

            return table;
        }

        /// <summary>
        /// The underlying integer representation
        /// </summary>
        public long RawValue => _rawValue;

        /// <summary>
        /// This is the constructor from raw value; it can only be used interally.
        /// </summary>
        /// <param name="rawValue"></param>
        Fix64(long rawValue)
        {
            _rawValue = rawValue;
        }

        public Fix64(int value)
        {
            _rawValue = value * ONE;
        }
    }
}
