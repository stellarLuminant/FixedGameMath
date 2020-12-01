using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FixedGameMath.Tests
{
    // Excluded from coverage since these are tests.
    [TestClass, ExcludeFromCodeCoverage]
    public class FixIntervalTests
    {
        private static readonly List<long> TestValues = new List<long>
        {
            // bounds, near boundaries, zero
            long.MinValue, long.MinValue + 1, long.MaxValue, long.MaxValue - 1, 0,

            // bit range
            1L << 0, 1L << 1, 1L << 2, 1L << 3,
            1L << 4, 1L << 5, 1L << 6, 1L << 7,
            1L << 8, 1L << 9, 1L << 0, 1L << 0,
            1L << 12, 1L << 13, 1L << 14, 1L << 15,
            1L << 16, 1L << 17, 1L << 18, 1L << 19,
            1L << 20, 1L << 21, 1L << 22, 1L << 23,
            1L << 24, 1L << 25, 1L << 26, 1L << 27,
            1L << 28, 1L << 29, 1L << 30, 1L << 31,
            1L << 32, 1L << 33, 1L << 34, 1L << 35,
            1L << 36, 1L << 37, 1L << 38, 1L << 39,
            1L << 40, 1L << 41, 1L << 42, 1L << 43,
            1L << 44, 1L << 45, 1L << 46, 1L << 47,
            1L << 48, 1L << 49, 1L << 50, 1L << 51,
            1L << 52, 1L << 53, 1L << 54, 1L << 55,
            1L << 56, 1L << 57, 1L << 58, 1L << 59,
            1L << 60, 1L << 61, 1L << 62, 

            // negative bit range
            -1L << 0, -1L << 1, -1L << 2, -1L << 3,
            -1L << 4, -1L << 5, -1L << 6, -1L << 7,
            -1L << 8, -1L << 9, -1L << 0, -1L << 0,
            -1L << 12, -1L << 13, -1L << 14, -1L << 15,
            -1L << 16, -1L << 17, -1L << 18, -1L << 19,
            -1L << 20, -1L << 21, -1L << 22, -1L << 23,
            -1L << 24, -1L << 25, -1L << 26, -1L << 27,
            -1L << 28, -1L << 29, -1L << 30, -1L << 31,
            -1L << 32, -1L << 33, -1L << 34, -1L << 35,
            -1L << 36, -1L << 37, -1L << 38, -1L << 39,
            -1L << 40, -1L << 41, -1L << 42, -1L << 43,
            -1L << 44, -1L << 45, -1L << 46, -1L << 47,
            -1L << 48, -1L << 49, -1L << 50, -1L << 51,
            -1L << 52, -1L << 53, -1L << 54, -1L << 55,
            -1L << 56, -1L << 57, -1L << 58, -1L << 59,
            -1L << 60, -1L << 61, -1L << 62, 

            // carry bit range
            (1L << 0) - 1L, (1L << 1) - 1L, (1L << 2) - 1L, (1L << 3) - 1L,
            (1L << 4) - 1L, (1L << 5) - 1L, (1L << 6) - 1L, (1L << 7) - 1L,
            (1L << 8) - 1L, (1L << 9) - 1L, (1L << 0) - 1L, (1L << 0) - 1L,
            (1L << 12) - 1L, (1L << 13) - 1L, (1L << 14) - 1L, (1L << 15) - 1L,
            (1L << 16) - 1L, (1L << 17) - 1L, (1L << 18) - 1L, (1L << 19) - 1L,
            (1L << 20) - 1L, (1L << 21) - 1L, (1L << 22) - 1L, (1L << 23) - 1L,
            (1L << 24) - 1L, (1L << 25) - 1L, (1L << 26) - 1L, (1L << 27) - 1L,
            (1L << 28) - 1L, (1L << 29) - 1L, (1L << 30) - 1L, (1L << 31) - 1L,
            (1L << 32) - 1L, (1L << 33) - 1L, (1L << 34) - 1L, (1L << 35) - 1L,
            (1L << 36) - 1L, (1L << 37) - 1L, (1L << 38) - 1L, (1L << 39) - 1L,
            (1L << 40) - 1L, (1L << 41) - 1L, (1L << 42) - 1L, (1L << 43) - 1L,
            (1L << 44) - 1L, (1L << 45) - 1L, (1L << 46) - 1L, (1L << 47) - 1L,
            (1L << 48) - 1L, (1L << 49) - 1L, (1L << 50) - 1L, (1L << 51) - 1L,
            (1L << 52) - 1L, (1L << 53) - 1L, (1L << 54) - 1L, (1L << 55) - 1L,
            (1L << 56) - 1L, (1L << 57) - 1L, (1L << 58) - 1L, (1L << 59) - 1L,
            (1L << 60) - 1L, (1L << 61) - 1L, (1L << 62) - 1L, 

            // negative carry bit range
            (-1L << 0) + 1L, (-1L << 1) + 1L, (-1L << 2) + 1L, (-1L << 3) + 1L,
            (-1L << 4) + 1L, (-1L << 5) + 1L, (-1L << 6) + 1L, (-1L << 7) + 1L,
            (-1L << 8) + 1L, (-1L << 9) + 1L, (-1L << 0) + 1L, (-1L << 0) + 1L,
            (-1L << 12) + 1L, (-1L << 13) + 1L, (-1L << 14) + 1L, (-1L << 15) + 1L,
            (-1L << 16) + 1L, (-1L << 17) + 1L, (-1L << 18) + 1L, (-1L << 19) + 1L,
            (-1L << 20) + 1L, (-1L << 21) + 1L, (-1L << 22) + 1L, (-1L << 23) + 1L,
            (-1L << 24) + 1L, (-1L << 25) + 1L, (-1L << 26) + 1L, (-1L << 27) + 1L,
            (-1L << 28) + 1L, (-1L << 29) + 1L, (-1L << 30) + 1L, (-1L << 31) + 1L,
            (-1L << 32) + 1L, (-1L << 33) + 1L, (-1L << 34) + 1L, (-1L << 35) + 1L,
            (-1L << 36) + 1L, (-1L << 37) + 1L, (-1L << 38) + 1L, (-1L << 39) + 1L,
            (-1L << 40) + 1L, (-1L << 41) + 1L, (-1L << 42) + 1L, (-1L << 43) + 1L,
            (-1L << 44) + 1L, (-1L << 45) + 1L, (-1L << 46) + 1L, (-1L << 47) + 1L,
            (-1L << 48) + 1L, (-1L << 49) + 1L, (-1L << 50) + 1L, (-1L << 51) + 1L,
            (-1L << 52) + 1L, (-1L << 53) + 1L, (-1L << 54) + 1L, (-1L << 55) + 1L,
            (-1L << 56) + 1L, (-1L << 57) + 1L, (-1L << 58) + 1L, (-1L << 59) + 1L,
            (-1L << 60) + 1L, (-1L << 61) + 1L, (-1L << 62) + 1L, 
        };

        [TestMethod]
        public void ConstZero()
        {
            Assert.AreEqual(Fix64.Zero, FixInterval.Zero.Lower);
            Assert.AreEqual(Fix64.Zero, FixInterval.Zero.Upper);
        }

        [TestMethod]
        public void ConstMinBound()
        {
            Assert.AreEqual(Fix64.MinValue, FixInterval.MinimumBound.Lower);
            Assert.AreEqual(Fix64.MinValue, FixInterval.MinimumBound.Upper);
        }

        [TestMethod]
        public void ConstMaxBound()
        {
            Assert.AreEqual(Fix64.MaxValue, FixInterval.MaximumBound.Lower);
            Assert.AreEqual(Fix64.MaxValue, FixInterval.MaximumBound.Upper);
        }

        [TestMethod]
        public void ConstFullRange()
        {
            Assert.AreEqual(Fix64.MinValue, FixInterval.FullRange.Lower);
            Assert.AreEqual(Fix64.MaxValue, FixInterval.FullRange.Upper);
        }

        [TestMethod]
        public void CreateUnchecked()
        {
            foreach (long lowerRaw in TestValues)
            {
                var lower = Fix64.FromRaw(lowerRaw);
                foreach (long upperRaw in TestValues)
                {
                    var upper = Fix64.FromRaw(upperRaw);

                    var interval = FixInterval.CreateUnchecked(lower, upper);

                    Assert.AreEqual(lower, interval.Lower);
                    Assert.AreEqual(upper, interval.Upper);
                }
            }
        }

        [TestMethod]
        public void Create()
        {
            foreach (long aRaw in TestValues)
            {
                var a = Fix64.FromRaw(aRaw);
                foreach (long bRaw in TestValues)
                {
                    var aIsLower = aRaw < bRaw;
                    var b = Fix64.FromRaw(bRaw);

                    var interval = FixInterval.Create(a, b);

                    var lower = aIsLower ? a : b;
                    var upper = aIsLower ? b : a;

                    Assert.AreEqual(lower, interval.Lower);
                    Assert.AreEqual(upper, interval.Upper);
                }
            }
        }

        [TestMethod]
        public void OperatorEquality()
        {
            foreach (long aRaw in TestValues)
            {
                var a = Fix64.FromRaw(aRaw);
                foreach (long bRaw in TestValues)
                {
                    var aIsLower = aRaw < bRaw;
                    var b = Fix64.FromRaw(bRaw);

                    // Test value equality
                    var interval1 = FixInterval.Create(a, b);
                    var interval2 = FixInterval.Create(a, b);

                    // Test Create invariant, values are always sorted.
                    var interval3 = FixInterval.Create(b, a);

                    var lower = aIsLower ? a : b;
                    var upper = aIsLower ? b : a;

                    var valueEqual = interval1 == interval2;
                    var createEqual = interval1 == interval3;

                    Assert.IsTrue(valueEqual);
                    Assert.IsTrue(createEqual);

                    if (lower != upper)
                    {
                        // Test inequality after breaking invariant
                        var interval4 = FixInterval.CreateUnchecked(upper, lower);

                        var uncheckedEqual = interval1 == interval4;

                        Assert.IsFalse(uncheckedEqual);
                    }
                }
            }
        }

        [TestMethod]
        public void OperatorInequality()
        {
            foreach (long aRaw in TestValues)
            {
                var a = Fix64.FromRaw(aRaw);
                foreach (long bRaw in TestValues)
                {
                    var aIsLower = aRaw < bRaw;
                    var b = Fix64.FromRaw(bRaw);

                    // Test value equality
                    var interval1 = FixInterval.Create(a, b);
                    var interval2 = FixInterval.Create(a, b);

                    // Test Create invariant, values are always sorted.
                    var interval3 = FixInterval.Create(b, a);

                    var lower = aIsLower ? a : b;
                    var upper = aIsLower ? b : a;

                    var valueEqual = interval1 != interval2;
                    var createEqual = interval1 != interval3;

                    Assert.IsFalse(valueEqual);
                    Assert.IsFalse(createEqual);

                    if (lower != upper)
                    {
                        // Test inequality after breaking invariant
                        var interval4 = FixInterval.CreateUnchecked(upper, lower);

                        var uncheckedEqual = interval1 != interval4;

                        Assert.IsTrue(uncheckedEqual);
                    }
                }
            }
        }

        [TestMethod]
        public void OverlapsValue()
        {
            var interval = FixInterval.Create(Fix64.Zero, Fix64.One);

            // Passing: Inside interval
            Assert.IsTrue(FixInterval.Overlaps(interval, Fix64.Parse("0.5")));

            // Passing: Lower bound inclusive
            Assert.IsTrue(FixInterval.Overlaps(interval, Fix64.Zero));

            // Passing: Upper bound inclusive
            Assert.IsTrue(FixInterval.Overlaps(interval, Fix64.One));

            // Failing: Below lower bound
            Assert.IsFalse(FixInterval.Overlaps(interval, Fix64.MinusOne));

            // Failing: Above upper bound
            Assert.IsFalse(FixInterval.Overlaps(interval, Fix64.Two));
        }

        [TestMethod]
        public void OverlapsInterval()
        {
            var baseInterval = FixInterval.Create(Fix64.Zero, Fix64.One);

            // Passing: Base interval overlapping base interval.
            var newBaseInterval = FixInterval.Create(baseInterval.Lower, baseInterval.Upper);
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, newBaseInterval));
            Assert.IsTrue(FixInterval.Overlaps(newBaseInterval, baseInterval));

            // Passing: Point interval overlapping point interval.
            var pointInterval = FixInterval.Create(Fix64.Zero, Fix64.Zero);
            var newPointInterval = FixInterval.Create(Fix64.Zero, Fix64.Zero);
            Assert.IsTrue(FixInterval.Overlaps(pointInterval, newPointInterval));
            Assert.IsTrue(FixInterval.Overlaps(newPointInterval, pointInterval));

            // Passing: Normal interval inside base
            var normalInsideBase = FixInterval.Create(Fix64.Parse("0.25"), Fix64.Parse("0.75"));
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, normalInsideBase));
            Assert.IsTrue(FixInterval.Overlaps(normalInsideBase, baseInterval));

            // Passing: Normal interval inside base, overlaps lower bound
            var normalInsideBaseLower = FixInterval.Create(Fix64.Zero, Fix64.Parse("0.5"));
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, normalInsideBaseLower));
            Assert.IsTrue(FixInterval.Overlaps(normalInsideBaseLower, baseInterval));

            // Passing: Normal interval inside base, overlaps upper bound
            var normalInsideBaseUpper = FixInterval.Create(Fix64.Parse("0.5"), Fix64.One);
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, normalInsideBaseUpper));
            Assert.IsTrue(FixInterval.Overlaps(normalInsideBaseUpper, baseInterval));

            // Passing: Single-Point interval inside base
            var pointInsideBase = FixInterval.Create(Fix64.Parse("0.5"), Fix64.Parse("0.5"));
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, pointInsideBase));
            Assert.IsTrue(FixInterval.Overlaps(pointInsideBase, baseInterval));

            // Passing: Single-Point interval inside base, overlaps lower bound.
            var pointInsideBaseLower = FixInterval.Create(Fix64.Zero, Fix64.Zero);
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, pointInsideBaseLower));
            Assert.IsTrue(FixInterval.Overlaps(pointInsideBaseLower, baseInterval));

            // Passing: Single-Point interval inside base, overlaps upper bound.
            var pointInsideBaseUpper = FixInterval.Create(Fix64.One, Fix64.One);
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, pointInsideBaseUpper));
            Assert.IsTrue(FixInterval.Overlaps(pointInsideBaseUpper, baseInterval));

            // Passing: Normal interval partially overlaps base from below.
            var normalPartialLower = FixInterval.Create(Fix64.Parse("-0.5"), Fix64.Parse("0.5"));
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, normalPartialLower));
            Assert.IsTrue(FixInterval.Overlaps(normalPartialLower, baseInterval));

            // Passing: Normal interval partially overlaps base from above.
            var normalPartialUpper = FixInterval.Create(Fix64.Parse("0.5"), Fix64.Parse("1.5"));
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, normalPartialUpper));
            Assert.IsTrue(FixInterval.Overlaps(normalPartialUpper, baseInterval));

            // Passing: Normal interval overlaps base at lower bound point.
            var normalLowerPoint = FixInterval.Create(Fix64.MinusOne, Fix64.Zero);
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, normalLowerPoint));
            Assert.IsTrue(FixInterval.Overlaps(normalLowerPoint, baseInterval));

            // Passing: Normal interval overlaps base at upper bound point.
            var normalUpperPoint = FixInterval.Create(Fix64.One, Fix64.Two);
            Assert.IsTrue(FixInterval.Overlaps(baseInterval, normalUpperPoint));
            Assert.IsTrue(FixInterval.Overlaps(normalUpperPoint, baseInterval));

            // Failing: Normal interval below base.
            var normalBelow = FixInterval.Create(Fix64.Parse("-2"), Fix64.MinusOne);
            Assert.IsFalse(FixInterval.Overlaps(baseInterval, normalBelow));
            Assert.IsFalse(FixInterval.Overlaps(normalBelow, baseInterval));

            // Failing: Normal interval above base.
            var normalAbove = FixInterval.Create(Fix64.Parse("3"), Fix64.Parse("4"));
            Assert.IsFalse(FixInterval.Overlaps(baseInterval, normalAbove));
            Assert.IsFalse(FixInterval.Overlaps(normalAbove, baseInterval));

            // Failing: Point interval below base.
            var pointBelow = FixInterval.Create(Fix64.MinusOne, Fix64.MinusOne);
            Assert.IsFalse(FixInterval.Overlaps(baseInterval, pointBelow));
            Assert.IsFalse(FixInterval.Overlaps(pointBelow, baseInterval));

            // Failing: Point interval above base.
            var pointAbove = FixInterval.Create(Fix64.Two, Fix64.Two);
            Assert.IsFalse(FixInterval.Overlaps(baseInterval, pointAbove));
            Assert.IsFalse(FixInterval.Overlaps(pointAbove, baseInterval));

            // Failing: Point interval below point.
            Assert.IsFalse(FixInterval.Overlaps(pointInterval, pointBelow));
            Assert.IsFalse(FixInterval.Overlaps(pointBelow, pointInterval));

            // Failing: Point interval above point.
            Assert.IsFalse(FixInterval.Overlaps(pointInterval, pointAbove));
            Assert.IsFalse(FixInterval.Overlaps(pointAbove, pointInterval));
        }

        [TestMethod]
        public void Intersection()
        {
            var baseInterval = FixInterval.Create(Fix64.Zero, Fix64.One);

            // Passing: Base interval overlapping base interval.
            var newBaseInterval = FixInterval.Create(baseInterval.Lower, baseInterval.Upper);
            var newBaseExpected = FixInterval.Create(baseInterval.Lower, baseInterval.Upper);

            Assert.IsTrue(FixInterval.Intersection(baseInterval, newBaseInterval, out var newBaseActual1));
            Assert.IsTrue(FixInterval.Intersection(newBaseInterval, baseInterval, out var newBaseActual2));

            Assert.AreEqual(newBaseExpected, newBaseActual1);
            Assert.AreEqual(newBaseExpected, newBaseActual2);

            // Passing: Point interval overlapping point interval.
            var pointInterval = FixInterval.Create(Fix64.Zero, Fix64.Zero);
            var newPointInterval = FixInterval.Create(Fix64.Zero, Fix64.Zero);
            var newPointExpected = FixInterval.Create(Fix64.Zero, Fix64.Zero);

            Assert.IsTrue(FixInterval.Intersection(pointInterval, newPointInterval, out var newPointActual1));
            Assert.IsTrue(FixInterval.Intersection(newPointInterval, pointInterval, out var newPointActual2));

            Assert.AreEqual(newPointExpected, newPointActual1);
            Assert.AreEqual(newPointExpected, newPointActual2);

            // Passing: Normal interval inside base
            var normalInsideBase = FixInterval.Create(Fix64.Parse("0.25"), Fix64.Parse("0.75"));
            var normalInsideBaseExpected = FixInterval.Create(Fix64.Parse("0.25"), Fix64.Parse("0.75"));
            Assert.IsTrue(FixInterval.Intersection(baseInterval, normalInsideBase, out var normalInsideBaseActual1));
            Assert.IsTrue(FixInterval.Intersection(normalInsideBase, baseInterval, out var normalInsideBaseActual2));

            Assert.AreEqual(normalInsideBaseExpected, normalInsideBaseActual1);
            Assert.AreEqual(normalInsideBaseExpected, normalInsideBaseActual2);

            // Passing: Normal interval inside base, overlaps lower bound
            var normalInsideBaseLower = FixInterval.Create(Fix64.Zero, Fix64.Parse("0.5"));
            var normalInsideBaseLowerExpected = FixInterval.Create(Fix64.Zero, Fix64.Parse("0.5"));

            Assert.IsTrue(FixInterval.Intersection(baseInterval, normalInsideBaseLower, out var normalInsideBaseLowerActual1));
            Assert.IsTrue(FixInterval.Intersection(normalInsideBaseLower, baseInterval, out var normalInsideBaseLowerActual2));

            Assert.AreEqual(normalInsideBaseLowerExpected, normalInsideBaseLowerActual1);
            Assert.AreEqual(normalInsideBaseLowerExpected, normalInsideBaseLowerActual2);

            // Passing: Normal interval inside base, overlaps upper bound
            var normalInsideBaseUpper = FixInterval.Create(Fix64.Parse("0.5"), Fix64.One);
            var normalInsideBaseUpperExpected = FixInterval.Create(Fix64.Parse("0.5"), Fix64.One);

            Assert.IsTrue(FixInterval.Intersection(baseInterval, normalInsideBaseUpper, out var normalInsideBaseUpperActual1));
            Assert.IsTrue(FixInterval.Intersection(normalInsideBaseUpper, baseInterval, out var normalInsideBaseUpperActual2));

            Assert.AreEqual(normalInsideBaseUpperExpected, normalInsideBaseUpperActual1);
            Assert.AreEqual(normalInsideBaseUpperExpected, normalInsideBaseUpperActual2);

            // Passing: Single-Point interval inside base
            var pointInsideBase = FixInterval.Create(Fix64.Parse("0.5"), Fix64.Parse("0.5"));
            var pointInsideBaseExpected = FixInterval.Create(Fix64.Parse("0.5"), Fix64.Parse("0.5"));

            Assert.IsTrue(FixInterval.Intersection(baseInterval, pointInsideBase, out var pointInsideBaseActual1));
            Assert.IsTrue(FixInterval.Intersection(pointInsideBase, baseInterval, out var pointInsideBaseActual2));

            Assert.AreEqual(pointInsideBaseExpected, pointInsideBaseActual1);
            Assert.AreEqual(pointInsideBaseExpected, pointInsideBaseActual2);

            // Passing: Single-Point interval inside base, overlaps lower bound.
            var pointInsideBaseLower = FixInterval.Create(Fix64.Zero, Fix64.Zero);
            var pointInsideBaseLowerExpected = FixInterval.Create(Fix64.Zero, Fix64.Zero);

            Assert.IsTrue(FixInterval.Intersection(baseInterval, pointInsideBaseLower, out var pointInsideBaseLowerActual1));
            Assert.IsTrue(FixInterval.Intersection(pointInsideBaseLower, baseInterval, out var pointInsideBaseLowerActual2));

            Assert.AreEqual(pointInsideBaseLowerExpected, pointInsideBaseLowerActual1);
            Assert.AreEqual(pointInsideBaseLowerExpected, pointInsideBaseLowerActual2);

            // Passing: Single-Point interval inside base, overlaps upper bound.
            var pointInsideBaseUpper = FixInterval.Create(Fix64.One, Fix64.One);
            var pointInsideBaseUpperExpected = FixInterval.Create(Fix64.One, Fix64.One);

            Assert.IsTrue(FixInterval.Intersection(baseInterval, pointInsideBaseUpper, out var pointInsideBaseUpperActual1));
            Assert.IsTrue(FixInterval.Intersection(pointInsideBaseUpper, baseInterval, out var pointInsideBaseUpperActual2));

            Assert.AreEqual(pointInsideBaseUpperExpected, pointInsideBaseUpperActual1);
            Assert.AreEqual(pointInsideBaseUpperExpected, pointInsideBaseUpperActual2);

            // Passing: Normal interval partially overlaps base from below.
            var normalPartialLower = FixInterval.Create(Fix64.Parse("-0.5"), Fix64.Parse("0.5"));
            var normalPartialLowerExpected = FixInterval.Create(Fix64.Zero, Fix64.Parse("0.5"));

            Assert.IsTrue(FixInterval.Intersection(baseInterval, normalPartialLower, out var normalPartialLowerActual1));
            Assert.IsTrue(FixInterval.Intersection(normalPartialLower, baseInterval, out var normalPartialLowerActual2));

            Assert.AreEqual(normalPartialLowerExpected, normalPartialLowerActual1);
            Assert.AreEqual(normalPartialLowerExpected, normalPartialLowerActual2);

            // Passing: Normal interval partially overlaps base from above.
            var normalPartialUpper = FixInterval.Create(Fix64.Parse("0.5"), Fix64.Parse("1.5"));
            var normalPartialUpperExpected = FixInterval.Create(Fix64.Parse("0.5"), Fix64.One);

            Assert.IsTrue(FixInterval.Intersection(baseInterval, normalPartialUpper, out var normalPartialUpperActual1));
            Assert.IsTrue(FixInterval.Intersection(normalPartialUpper, baseInterval, out var normalPartialUpperActual2));

            Assert.AreEqual(normalPartialUpperExpected, normalPartialUpperActual1);
            Assert.AreEqual(normalPartialUpperExpected, normalPartialUpperActual2);

            // Passing: Normal interval overlaps base at lower bound point.
            var normalLowerPoint = FixInterval.Create(Fix64.MinusOne, Fix64.Zero);
            var normalLowerPointExpected = FixInterval.Create(Fix64.Zero, Fix64.Zero);

            Assert.IsTrue(FixInterval.Intersection(baseInterval, normalLowerPoint, out var normalLowerPointActual1));
            Assert.IsTrue(FixInterval.Intersection(normalLowerPoint, baseInterval, out var normalLowerPointActual2));

            Assert.AreEqual(normalLowerPointExpected, normalLowerPointActual1);
            Assert.AreEqual(normalLowerPointExpected, normalLowerPointActual2);

            // Passing: Normal interval overlaps base at upper bound point.
            var normalUpperPoint = FixInterval.Create(Fix64.One, Fix64.Two);
            var normalUpperPointExpected = FixInterval.Create(Fix64.One, Fix64.One);

            Assert.IsTrue(FixInterval.Intersection(baseInterval, normalUpperPoint, out var normalUpperPointActual1));
            Assert.IsTrue(FixInterval.Intersection(normalUpperPoint, baseInterval, out var normalUpperPointActual2));

            Assert.AreEqual(normalUpperPointExpected, normalUpperPointActual1);
            Assert.AreEqual(normalUpperPointExpected, normalUpperPointActual2);

            // Failing: Normal interval below base.
            var normalBelow = FixInterval.Create(Fix64.Parse("-2"), Fix64.MinusOne);

            Assert.IsFalse(FixInterval.Intersection(baseInterval, normalBelow, out var normalBelowActual1));
            Assert.IsFalse(FixInterval.Intersection(normalBelow, baseInterval, out var normalBelowActual2));
            
            Assert.AreEqual(FixInterval.Zero, normalBelowActual1);
            Assert.AreEqual(FixInterval.Zero, normalBelowActual2);

            // Failing: Normal interval above base.
            var normalAbove = FixInterval.Create(Fix64.Parse("3"), Fix64.Parse("4"));

            Assert.IsFalse(FixInterval.Intersection(baseInterval, normalAbove, out var normalAboveActual1));
            Assert.IsFalse(FixInterval.Intersection(normalAbove, baseInterval, out var normalAboveActual2));

            Assert.AreEqual(FixInterval.Zero, normalAboveActual1);
            Assert.AreEqual(FixInterval.Zero, normalAboveActual2);

            // Failing: Point interval below base.
            var pointBelow = FixInterval.Create(Fix64.MinusOne, Fix64.MinusOne);

            Assert.IsFalse(FixInterval.Intersection(baseInterval, pointBelow, out var pointBelowActual1));
            Assert.IsFalse(FixInterval.Intersection(pointBelow, baseInterval, out var pointBelowActual2));

            Assert.AreEqual(FixInterval.Zero, pointBelowActual1);
            Assert.AreEqual(FixInterval.Zero, pointBelowActual2);

            // Failing: Point interval above base.
            var pointAbove = FixInterval.Create(Fix64.Two, Fix64.Two);

            Assert.IsFalse(FixInterval.Intersection(baseInterval, pointAbove, out var pointAboveActual1));
            Assert.IsFalse(FixInterval.Intersection(pointAbove, baseInterval, out var pointAboveActual2));

            Assert.AreEqual(FixInterval.Zero, pointAboveActual1);
            Assert.AreEqual(FixInterval.Zero, pointAboveActual2);

            // Failing: Point interval below point.
            Assert.IsFalse(FixInterval.Intersection(pointInterval, pointBelow, out var pointBelowPointActual1));
            Assert.IsFalse(FixInterval.Intersection(pointBelow, pointInterval, out var pointBelowPointActual2));

            Assert.AreEqual(FixInterval.Zero, pointBelowPointActual1);
            Assert.AreEqual(FixInterval.Zero, pointBelowPointActual2);

            // Failing: Point interval above point.
            Assert.IsFalse(FixInterval.Intersection(pointInterval, pointAbove, out var pointAbovePointActual1));
            Assert.IsFalse(FixInterval.Intersection(pointAbove, pointInterval, out var pointAbovePointActual2));

            Assert.AreEqual(FixInterval.Zero, pointAbovePointActual1);
            Assert.AreEqual(FixInterval.Zero, pointAbovePointActual2);
        }

        [TestMethod]
        public void Union()
        {
            throw new NotImplementedException();


        }

        [TestMethod]
        public void MinimumTranslation()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void SetLowerUnchecked()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void SetUpperUnchecked()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void SetLower()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void SetUpper()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void AddLowerUnchecked()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void AddUpperUnchecked()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void AddLower()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void AddUpper()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Translate()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ObjectEquals()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ObjectHashCode()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ObjectToString()
        {
            throw new NotImplementedException();
        }
    }
}
