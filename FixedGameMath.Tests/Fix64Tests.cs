using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FixedGameMath.Tests
{
    [TestClass]
    public class Fix64Tests
    {
        private readonly long[] _testCases = {
            // Small numbers
            0L, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
            -1, -2, -3, -4, -5, -6, -7, -8, -9, -10,
  
            // Integer numbers
            0x100000000, -0x100000000, 0x200000000, -0x200000000, 0x300000000, -0x300000000,
            0x400000000, -0x400000000, 0x500000000, -0x500000000, 0x600000000, -0x600000000,
  
            // Fractions (1/2, 1/4, 1/8)
            0x80000000, -0x80000000, 0x40000000, -0x40000000, 0x20000000, -0x20000000,
  
            // Problematic carry
            0xFFFFFFFF, -0xFFFFFFFF, 0x1FFFFFFFF, -0x1FFFFFFFF, 0x3FFFFFFFF, -0x3FFFFFFFF,
  
            // Smallest and largest values
            long.MaxValue, long.MinValue,
  
            // Large random numbers
            6791302811978701836, -8192141831180282065, 6222617001063736300, -7871200276881732034,
            8249382838880205112, -7679310892959748444, 7708113189940799513, -5281862979887936768,
            8220231180772321456, -5204203381295869580, 6860614387764479339, -9080626825133349457,
            6658610233456189347, -6558014273345705245, 6700571222183426493,
  
            // Small random numbers
            -436730658, -2259913246, 329347474, 2565801981, 3398143698, 137497017, 1060347500,
            -3457686027, 1923669753, 2891618613, 2418874813, 2899594950, 2265950765, -1962365447,
            3077934393

            // Tiny random numbers
            - 171,
            -359, 491, 844, 158, -413, -422, -737, -575, -330,
            -376, 435, -311, 116, 715, -1024, -487, 59, 724, 993
        };

        private readonly List<(double radians, double degrees)> radiansToDegreesTable = new List<(double radians, double degrees)> 
        {
            (0, 0),
            (0.785398163397, 45),
            (1.57079632679, 90),
            (3.14159265359, 180),
            (4.71238898038, 270),
            (6.28318530718, 360),
            (-0.785398163397, -45),
            (-1.57079632679, -90),
            (-3.14159265359, -180),
            (-4.71238898038, -270),
            (-6.28318530718, -360)
        };

        [TestMethod]
        public void Precision()
        {
            Assert.AreEqual(0.00000000023283064365386962890625m, Fix64.PrecisionDecimal);
        }

        [TestMethod]
        public void ToDegrees()
        {
            const double maxDelta = 0.0000001;
            foreach (var item in radiansToDegreesTable)
            {
                var actualDegrees = (double)Fix64.ToDegrees((Fix64)item.radians);
                var delta = Math.Abs(item.degrees - actualDegrees);

                Assert.IsTrue(delta <= maxDelta, $"ToDegrees({item.radians}) Precision: expected <{maxDelta} but got {delta}");
            }
        }

        [TestMethod]
        public void ToRadians()
        {
            const double maxDelta = 0.0000001;
            foreach (var item in radiansToDegreesTable)
            {
                var actualRadians = (double)Fix64.ToRadians((Fix64)item.degrees);
                var delta = Math.Abs(item.radians - actualRadians);

                Assert.IsTrue(delta <= maxDelta, $"ToRadians({item.degrees}) Precision: expected <{maxDelta} but got {delta}");
            }
        }

        [TestMethod]
        public void Equals()
        {
            foreach (long testCase in _testCases)
            {
                var fixedNum = Fix64.FromRaw(testCase);
                var identical = Fix64.FromRaw(testCase);

                Assert.IsTrue(fixedNum.Equals(identical));
            }
        }

        [TestMethod]
        public void EqualsInvalid()
        {
            object[] testCases =
            {
                null,
                "",
                "a",
                "1.20",
                1,
                0.24
            };

            var fixedNum = new Fix64(1);
            Fix64? nullableFixedNum = null;

            foreach (var testCase in testCases)
            {
                Assert.IsFalse(fixedNum.Equals(testCase));
            }

            Assert.IsFalse(fixedNum.Equals(nullableFixedNum));
        }

        [TestMethod]
        public void HashCode()
        {
            foreach (long testCase in _testCases)
            {
                var expectedHashCode = testCase.GetHashCode();
                var fixedNum = Fix64.FromRaw(testCase);
                var hashCode = fixedNum.GetHashCode();

                Assert.AreEqual(expectedHashCode, hashCode);
            }
        }

        private readonly int[] _intTestCases =
        {
            // Simple values
            0, 1, -1,

            // Boundary values
            Int32.MaxValue, Int32.MaxValue - 1, Int32.MinValue, Int32.MinValue + 1,

            // Small values
            324, -324, 9545, -9545, 16, -16,

            // Big values
            9784556, -9784556, 1826483, -1826483, 67839374, -67839374
        };

        [TestMethod]
        public void IntToFix64AndBack()
        {
            foreach (int testCase in _intTestCases)
            {
                var fixedNum = new Fix64(testCase);
                var intNum = (int)fixedNum;
                var newFixedNum = new Fix64(intNum);

                Assert.AreEqual(fixedNum, newFixedNum);
            }
        }

        [TestMethod]
        public void LongToFix64AndBack()
        {
            foreach (long testCase in _testCases)
            {
                var fixedNum = Fix64.FromRaw(testCase);
                var longNum = fixedNum.RawValue;
                var newFixedNum = Fix64.FromRaw(longNum);

                Assert.AreEqual(fixedNum, newFixedNum);
            }
        }

        [TestMethod]
        public void FloatToFix64AndBack()
        {
            foreach (long testCase in _testCases)
            {
                var fixedNum = Fix64.FromRaw(testCase);
                var floatNum = (float)fixedNum;
                var newFixedNum = (Fix64)floatNum;

                // 23 bits of precision in a Single.
                AreEqualWithinBitPrecision(fixedNum, newFixedNum, 23);
            }
        }

        [TestMethod]
        public void DoubleToFix64AndBack()
        {
            foreach (long testCase in _testCases)
            {
                var fixedNum = Fix64.FromRaw(testCase);
                var doubleNum = (double)fixedNum;
                var newFixedNum = (Fix64)doubleNum;

                // 52 bits of precision in a Double.
                AreEqualWithinBitPrecision(fixedNum, newFixedNum, 52);
            }
        }

        [TestMethod]
        public void DecimalToFix64AndBack()
        {
            foreach (long testCase in _testCases)
            {
                var fixedNum = Fix64.FromRaw(testCase);
                var decimalNum = (decimal)fixedNum;
                var newFixedNum = (Fix64)decimalNum;

                // There are more bits in a decimal than a Fix64 so technically it should be lossless.
                // Decided to still use this method since it's a change in number format.
                AreEqualWithinBitPrecision(fixedNum, newFixedNum, 64);
            }
        }

        static void AreEqualWithinBitPrecision(Fix64 original, Fix64 converted, int floatPrecisionBits)
        {
            // Counts unused bits in order of most significance to least.
            // Floating point values usually can't store all 64 bits of precision, so they cut off the least significant part.
            // In order to see how precise this conversion is, we need to check how many significant bits are being used by the source value itself.
            // Once a used bit is found, it is considered the first significant bit and all other lower precision bits are considered "used".
            int unusedBits = 0;

            // Check the sign bit to see if the value is negative
            // Unused bits from most to least significant are the same bit as the sign bit.
            var signIsNegative = (original.RawValue & (1L << 63)) != 0;

            // To shift 1 up to the top of a 64 bit int we'd shift up 63 times.
            // But we want to avoid the sign bit since floats account for that in a separate bit.
            // So we're shifting up 62 times.
            for (int bitPos = 62; bitPos >= 0; bitPos--)
            {
                var bit = 1L << bitPos;
                
                // If the bit is the same as the sign bit (negative is 1)
                // then it is empty.
                var bitIsOne = (original.RawValue & bit) != 0;
                var bitIsEmpty = bitIsOne == signIsNegative;

                if (bitIsEmpty)
                    unusedBits++;
                else
                    break;
            }

            // largest value below the sign bit
            // all bits are 1 except for sign bit
            var largestValue = 0x7FFFFFFFFFFFFFFF;

            // Add the unused bits to the float's precision bits to get how much we should shift down.
            // If the sum is more than 62 then we have more than enough precision to cover the entire value.
            var downShift = Math.Min(unusedBits + floatPrecisionBits, 62);

            // Shift down the largest value to make up the largest allowed error between conversion.
            var precisionRaw = largestValue >> downShift;
            var precision = Fix64.FromRaw(precisionRaw);

            // Check the values against the newly computed precision.
            var difference = Fix64.Abs(converted - original);

            Assert.IsTrue(difference <= precision, $"Floating Point Precision Check({original}, {converted}, {floatPrecisionBits}) = expected <{precision} but got {difference}");
        }

        [TestMethod]
        public void RawValue()
        {
            foreach (long testCase in _testCases)
            {
                var fixedNum = Fix64.FromRaw(testCase);
                Assert.AreEqual(fixedNum.RawValue, testCase);
            }
        }

        [TestMethod]
        public void Addition()
        {
            var terms1 = new[] { Fix64.MinValue, (Fix64)(-1), Fix64.Zero, Fix64.One, Fix64.MaxValue };
            var terms2 = new[] { (Fix64)(-1), (Fix64)2, (Fix64)(-1.5m), (Fix64)(-2), Fix64.One };
            var expecteds = new[] { Fix64.MinValue, Fix64.One, (Fix64)(-1.5m), (Fix64)(-1), Fix64.MaxValue };
            for (int i = 0; i < terms1.Length; ++i)
            {
                var actual = terms1[i] + terms2[i];
                var expected = expecteds[i];
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Subtraction()
        {
            var terms1 = new[] { Fix64.MinValue, (Fix64)(-1), Fix64.Zero, Fix64.One, Fix64.MaxValue };
            var terms2 = new[] { Fix64.One, (Fix64)(-2), (Fix64)(1.5m), (Fix64)(2), (Fix64)(-1) };
            var expecteds = new[] { Fix64.MinValue, Fix64.One, (Fix64)(-1.5m), (Fix64)(-1), Fix64.MaxValue };
            for (int i = 0; i < terms1.Length; ++i)
            {
                var actual = terms1[i] - terms2[i];
                var expected = expecteds[i];
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void BasicMultiplication()
        {
            var term1s = new[] { 0m, 1m, -1m, 5m, -5m, 0.5m, -0.5m, -1.0m };
            var term2s = new[] { 16m, 16m, 16m, 16m, 16m, 16m, 16m, -1.0m };
            var expecteds = new[] { 0L, 16, -16, 80, -80, 8, -8, 1 };
            for (int i = 0; i < term1s.Length; ++i)
            {
                var expected = expecteds[i];
                var actual = (long)((Fix64)term1s[i] * (Fix64)term2s[i]);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void MultiplicationTestCases()
        {
            var sw = new Stopwatch();
            int failures = 0;
            for (int i = 0; i < _testCases.Length; ++i)
            {
                for (int j = 0; j < _testCases.Length; ++j)
                {
                    var x = Fix64.FromRaw(_testCases[i]);
                    var y = Fix64.FromRaw(_testCases[j]);
                    var xM = (decimal)x;
                    var yM = (decimal)y;
                    var expected = xM * yM;
                    expected =
                        expected > (decimal)Fix64.MaxValue
                            ? (decimal)Fix64.MaxValue
                            : expected < (decimal)Fix64.MinValue
                                  ? (decimal)Fix64.MinValue
                                  : expected;
                    sw.Start();
                    var actual = x * y;
                    sw.Stop();
                    var actualM = (decimal)actual;
                    var maxDelta = (decimal)Fix64.FromRaw(1);
                    if (Math.Abs(actualM - expected) > maxDelta)
                    {
                        Console.WriteLine("Failed for FromRaw({0}) * FromRaw({1}): expected {2} but got {3}",
                                          _testCases[i],
                                          _testCases[j],
                                          (Fix64)expected,
                                          actualM);
                        ++failures;
                    }
                }
            }
            Console.WriteLine("{0} total, {1} per multiplication", sw.ElapsedMilliseconds, (double)sw.Elapsed.Milliseconds / (_testCases.Length * _testCases.Length));
            Assert.IsTrue(failures < 1);
        }


        static void Ignore<T>(T value) { }

        [TestMethod]
        public void DivisionTestCases()
        {
            var sw = new Stopwatch();
            int failures = 0;
            for (int i = 0; i < _testCases.Length; ++i)
            {
                for (int j = 0; j < _testCases.Length; ++j)
                {
                    var x = Fix64.FromRaw(_testCases[i]);
                    var y = Fix64.FromRaw(_testCases[j]);
                    var xM = (decimal)x;
                    var yM = (decimal)y;

                    if (_testCases[j] == 0)
                    {
                        Assert.ThrowsException<DivideByZeroException>(() => Ignore(x / y));
                    }
                    else
                    {
                        var expected = xM / yM;
                        expected =
                            expected > (decimal)Fix64.MaxValue
                                ? (decimal)Fix64.MaxValue
                                : expected < (decimal)Fix64.MinValue
                                      ? (decimal)Fix64.MinValue
                                      : expected;
                        sw.Start();
                        var actual = x / y;
                        sw.Stop();
                        var actualM = (decimal)actual;
                        var maxDelta = (decimal)Fix64.FromRaw(1);
                        if (Math.Abs(actualM - expected) > maxDelta)
                        {
                            Console.WriteLine("Failed for FromRaw({0}) / FromRaw({1}): expected {2} but got {3}",
                                              _testCases[i],
                                              _testCases[j],
                                              (Fix64)expected,
                                              actualM);
                            ++failures;
                        }
                    }
                }
            }
            Console.WriteLine("{0} total, {1} per division", sw.ElapsedMilliseconds, (double)sw.Elapsed.Milliseconds / (_testCases.Length * _testCases.Length));
            Assert.IsTrue(failures < 1);
        }



        [TestMethod]
        public void Sign()
        {
            var sources = new[] { Fix64.MinValue, (Fix64)(-1), Fix64.Zero, Fix64.One, Fix64.MaxValue };
            var expecteds = new[] { -1, -1, 0, 1, 1 };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = Fix64.Sign(sources[i]);
                var expected = expecteds[i];
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Abs()
        {
            Assert.AreEqual(Fix64.MaxValue, Fix64.Abs(Fix64.MinValue));
            var sources = new[] { -1, 0, 1, int.MaxValue };
            var expecteds = new[] { 1, 0, 1, int.MaxValue };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = Fix64.Abs((Fix64)sources[i]);
                var expected = (Fix64)expecteds[i];
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void FastAbs()
        {
            Assert.AreEqual(Fix64.MinValue, Fix64.FastAbs(Fix64.MinValue));
            var sources = new[] { -1, 0, 1, int.MaxValue };
            var expecteds = new[] { 1, 0, 1, int.MaxValue };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = Fix64.FastAbs((Fix64)sources[i]);
                var expected = (Fix64)expecteds[i];
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Floor()
        {
            var sources = new[] { -5.1m, -1, 0, 1, 5.1m };
            var expecteds = new[] { -6m, -1, 0, 1, 5m };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = (decimal)Fix64.Floor((Fix64)sources[i]);
                var expected = expecteds[i];
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Ceiling()
        {
            var sources = new[] { -5.1m, -1, 0, 1, 5.1m };
            var expecteds = new[] { -5m, -1, 0, 1, 6m };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = (decimal)Fix64.Ceiling((Fix64)sources[i]);
                var expected = expecteds[i];
                Assert.AreEqual(expected, actual);
            }

            Assert.AreEqual(Fix64.MaxValue, Fix64.Ceiling(Fix64.MaxValue));
        }

        [TestMethod]
        public void Round()
        {
            var sources = new[] { -5.5m, -5.1m, -4.5m, -4.4m, -1, 0, 1, 4.5m, 4.6m, 5.4m, 5.5m };
            var expecteds = new[] { -6m, -5m, -4m, -4m, -1, 0, 1, 4m, 5m, 5m, 6m };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = (decimal)Fix64.Round((Fix64)sources[i]);
                var expected = expecteds[i];
                Assert.AreEqual(expected, actual);
            }
            Assert.AreEqual(Fix64.MaxValue, Fix64.Round(Fix64.MaxValue));
        }


        [TestMethod]
        public void Sqrt()
        {
            for (int i = 0; i < _testCases.Length; ++i)
            {
                var f = Fix64.FromRaw(_testCases[i]);
                if (Fix64.Sign(f) < 0)
                {
                    Assert.ThrowsException<ArgumentOutOfRangeException>(() => Fix64.Sqrt(f));
                }
                else
                {
                    var expected = Math.Sqrt((double)f);
                    var actual = (double)Fix64.Sqrt(f);
                    var delta = (decimal)Math.Abs(expected - actual);
                    Assert.IsTrue(delta <= Fix64.PrecisionDecimal);
                }
            }
        }

        [TestMethod]
        public void Log2()
        {
            double maxDelta = (double)(Fix64.PrecisionDecimal * 4);

            for (int j = 0; j < _testCases.Length; ++j)
            {
                var b = Fix64.FromRaw(_testCases[j]);

                if (b <= Fix64.Zero)
                {
                    Assert.ThrowsException<ArgumentOutOfRangeException>(() => Fix64.Log2(b));
                }
                else
                {
                    var expected = Math.Log((double)b) / Math.Log(2);
                    var actual = (double)Fix64.Log2(b);
                    var delta = Math.Abs(expected - actual);

                    Assert.IsTrue(delta <= maxDelta, string.Format("Ln({0}) = expected {1} but got {2}", b, expected, actual));
                }
            }
        }

        [TestMethod]
        public void Ln()
        {
            double maxDelta = 0.00000001;

            for (int j = 0; j < _testCases.Length; ++j)
            {
                var b = Fix64.FromRaw(_testCases[j]);

                if (b <= Fix64.Zero)
                {
                    Assert.ThrowsException<ArgumentOutOfRangeException>(() => Fix64.Ln(b));
                }
                else
                {
                    var expected = Math.Log((double)b);
                    var actual = (double)Fix64.Ln(b);
                    var delta = Math.Abs(expected - actual);

                    Assert.IsTrue(delta <= maxDelta, string.Format("Ln({0}) = expected {1} but got {2}", b, expected, actual));
                }
            }
        }

        [TestMethod]
        public void Pow2()
        {
            double maxDelta = 0.0000001;
            for (int i = 0; i < _testCases.Length; ++i)
            {
                var e = Fix64.FromRaw(_testCases[i]);

                var expected = Math.Min(Math.Pow(2, (double)e), (double)Fix64.MaxValue);
                var actual = (double)Fix64.Pow2(e);
                var delta = Math.Abs(expected - actual);

                Assert.IsTrue(delta <= maxDelta, string.Format("Pow2({0}) = expected {1} but got {2}", e, expected, actual));
            }
        }

        [TestMethod]
        public void Pow()
        {
            for (int i = 0; i < _testCases.Length; ++i)
            {
                var b = Fix64.FromRaw(_testCases[i]);

                for (int j = 0; j < _testCases.Length; ++j)
                {
                    var e = Fix64.FromRaw(_testCases[j]);

                    if (b == Fix64.Zero && e < Fix64.Zero)
                    {
                        Assert.ThrowsException<DivideByZeroException>(() => Fix64.Pow(b, e));
                    }
                    else if (b < Fix64.Zero && e != Fix64.Zero)
                    {
                        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Fix64.Pow(b, e));
                    }
                    else
                    {
                        var expected = e == Fix64.Zero ? 1 : b == Fix64.Zero ? 0 : Math.Min(Math.Pow((double)b, (double)e), (double)Fix64.MaxValue);

                        // Absolute precision deteriorates with large result values, take this into account
                        // Similarly, large exponents reduce precision, even if result is small.
                        double maxDelta = Math.Abs((double)e) > 100000000 ? 0.5 : expected > 100000000 ? 10 : expected > 1000 ? 0.5 : 0.00001;

                        var actual = (double)Fix64.Pow(b, e);
                        var delta = Math.Abs(expected - actual);

                        Assert.IsTrue(delta <= maxDelta, string.Format("Pow({0}, {1}) = expected {2} but got {3}", b, e, expected, actual));
                    }
                }
            }
        }

        [TestMethod]
        public void Modulus()
        {
            var deltas = new List<decimal>();
            foreach (var operand1 in _testCases)
            {
                foreach (var operand2 in _testCases)
                {
                    var f1 = Fix64.FromRaw(operand1);
                    var f2 = Fix64.FromRaw(operand2);

                    if (operand2 == 0)
                    {
                        Assert.ThrowsException<DivideByZeroException>(() => Ignore(f1 / f2));
                    }
                    else
                    {
                        var d1 = (decimal)f1;
                        var d2 = (decimal)f2;
                        var actual = (decimal)(f1 % f2);
                        var expected = d1 % d2;
                        var delta = Math.Abs(expected - actual);
                        deltas.Add(delta);
                        Assert.IsTrue(delta <= 60 * Fix64.PrecisionDecimal, string.Format("{0} % {1} = expected {2} but got {3}", f1, f2, expected, actual));
                    }
                }
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / Fix64.PrecisionDecimal);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / Fix64.PrecisionDecimal);
            Console.WriteLine("failed: {0}%", deltas.Count(d => d > Fix64.PrecisionDecimal) * 100.0 / deltas.Count);
        }

        [TestMethod]
        public void FastModZeroFailure()
        {
            Assert.ThrowsException<DivideByZeroException>(() => Fix64.FastMod(Fix64.One, Fix64.Zero));
        }

        [TestMethod]
        public void FastModMinValueFailure()
        {
            Assert.ThrowsException<OverflowException>(() => Fix64.FastMod(Fix64.MinValue, -Fix64.PrecisionUnit));
        }

        [TestMethod]
        public void FastMod()
        {
            int[] testInputs = { 0, -1, 1, int.MaxValue, int.MinValue, -1234, 1234, 1234567, -1234567 };
            int[] testDivisors = { 1, 2, 4, 8, 16, 32, 64, -2, -4, -8, -16, -32, -64 };

            foreach (var input in testInputs)
            {
                var fixedInput = new Fix64(input);
                foreach (var divisor in testDivisors)
                {
                    var fixedDivisor = new Fix64(divisor);
                    var fixedMod = Fix64.FastMod(fixedInput, fixedDivisor);

                    var expectedMod = input % divisor;
                    var actualMod = (int)fixedMod;

                    Assert.AreEqual(expectedMod, actualMod);
                }
            }
        }

        [TestMethod]
        public void FastAdd()
        {
            foreach (var inputA in _testCases)
            {
                var fixedInputA = Fix64.FromRaw(inputA);
                foreach (var inputB in _testCases)
                {
                    var fixedInputB = Fix64.FromRaw(inputB);

                    var fixedSum = Fix64.FastAdd(fixedInputA, fixedInputB);
                    
                    var expectedSum = inputA + inputB;

                    var actualSum = fixedSum.RawValue;

                    Assert.AreEqual(expectedSum, actualSum);
                }
            }
        }

        [TestMethod]
        public void FastSub()
        {
            foreach (var inputA in _testCases)
            {
                var fixedInputA = Fix64.FromRaw(inputA);
                foreach (var inputB in _testCases)
                {
                    var fixedInputB = Fix64.FromRaw(inputB);

                    var fixedDiff = Fix64.FastSub(fixedInputA, fixedInputB);
                    
                    var expectedDiff = inputA - inputB;

                    var actualDiff = fixedDiff.RawValue;

                    Assert.AreEqual(expectedDiff, actualDiff);
                }
            }
        }

        [TestMethod]
        public void HasFraction()
        {
            foreach (var testCase in _testCases)
            {
                var fixedNum = Fix64.FromRaw(testCase);

                var expected = Fix64.Fraction(fixedNum) != Fix64.Zero;
                var actual = Fix64.HasFraction(fixedNum);

                Assert.AreEqual(expected, actual);
            }
        }

        // Test depends on Double conversion working properly.
        [TestMethod]
        public void Fraction()
        {
            foreach (var testCase in _testCases)
            {
                var fixedNum = Fix64.FromRaw(testCase);

                // Get the fraction in decimal, then convert back.
                var decimalNum = (decimal)fixedNum;
                var decimalFraction = decimalNum % 1;
                var expectedFraction = (Fix64)decimalFraction;
                var actualFraction = Fix64.Fraction(fixedNum);

                AreEqualWithinBitPrecision(actualFraction, expectedFraction, 64);
            }
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        private double ToRadians(double degrees)
        {
            return Math.PI * degrees / 180.0;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        private double ToDegrees(double radians)
        {
            return 180.0 * radians / Math.PI;
        }

        // TODO: Figure out what to do with these benchmarks.
        public void SinBenchmark()
        {
            var deltas = new List<double>();

            var swf = new Stopwatch();
            var swd = new Stopwatch();

            // Restricting the range to from 0 to Pi/2
            for (var angle = 0.0; angle <= 360.0; angle += 0.001)
            {
                var f = (Fix64)angle;
                var radians = angle * Math.PI / 180.0;

                swf.Start();
                var actualF = Fix64.Sin(f);
                swf.Stop();
                var actual = (double)actualF;

                swd.Start();
                var expected = Math.Sin(radians);
                swd.Stop();

                var delta = Math.Abs(expected - actual);
                deltas.Add(delta);
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / (double)Fix64.PrecisionDecimal);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / (double)Fix64.PrecisionDecimal);
            Console.WriteLine("Fix64.Sin time = {0}ms, Math.Sin time = {1}ms", swf.ElapsedMilliseconds, swd.ElapsedMilliseconds);
        }

        [TestMethod]
        public void Sin()
        {
            Assert.IsTrue(Fix64.Sin(Fix64.Zero) == Fix64.Zero);

            Assert.IsTrue(Fix64.Sin(Fix64.QuarterRot) == Fix64.One);
            Assert.IsTrue(Fix64.Sin(Fix64.HalfRot) == Fix64.Zero);
            Assert.IsTrue(Fix64.Sin(Fix64.HalfRot + Fix64.QuarterRot) == -Fix64.One);
            Assert.IsTrue(Fix64.Sin(Fix64.FullRot) == Fix64.Zero);

            Assert.IsTrue(Fix64.Sin(-Fix64.QuarterRot) == -Fix64.One);
            Assert.IsTrue(Fix64.Sin(-Fix64.HalfRot) == Fix64.Zero);
            Assert.IsTrue(Fix64.Sin(-Fix64.HalfRot - Fix64.QuarterRot) == Fix64.One);
            Assert.IsTrue(Fix64.Sin(-Fix64.FullRot) == Fix64.Zero);


            for (double angle = -360.0; angle <= 360.0; angle += 0.001)
            {
                var f = (Fix64)angle;
                var actualF = Fix64.Sin(f);
                var expected = (decimal)Math.Sin(ToRadians(angle));
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.IsTrue(delta <= 0.0001M, $"Sin({angle}): expected {expected} but got {actualF}");
            }

            var deltas = new List<decimal>();
            foreach (var val in _testCases)
            {
                var f = Fix64.FromRaw(val);
                var actualF = Fix64.Sin(f);
                var expected = (decimal)Math.Sin(ToRadians((double)f));
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.IsTrue(delta <= 0.0001M, $"Sin({f}): expected {expected} but got {actualF}");
            }
        }

        [TestMethod]
        public void Acos()
        {
            var maxDelta = 0.0000003m;
            var deltas = new List<decimal>();

            Assert.AreEqual(Fix64.Zero, Fix64.Acos(Fix64.One));
            Assert.AreEqual(Fix64.QuarterRot, Fix64.Acos(Fix64.Zero));
            Assert.AreEqual(Fix64.HalfRot, Fix64.Acos(-Fix64.One));

            // Precision
            for (double x = -1.0; x < 1.0; x += 0.001)
            {
                var xf = (Fix64)x;
                var actual = (decimal)Fix64.Acos(xf);
                var expected = (decimal)ToDegrees(Math.Acos((double)xf));
                var delta = Math.Abs(actual - expected);
                deltas.Add(delta);
                Assert.IsTrue(delta <= maxDelta, $"Precision: Acos({xf}): expected {expected} but got {actual}");
            }

            for (int i = 0; i < _testCases.Length; ++i)
            {
                var b = Fix64.FromRaw(_testCases[i]);

                if (b < -Fix64.One || b > Fix64.One)
                {
                    Assert.ThrowsException<ArgumentOutOfRangeException>(() => Fix64.Acos(b));
                }
                else
                {
                    var expected = (decimal)ToDegrees(Math.Acos((double)b));
                    var actual = (decimal)Fix64.Acos(b);
                    var delta = Math.Abs(expected - actual);
                    deltas.Add(delta);
                    Assert.IsTrue(delta <= maxDelta, $"Acos({b}) = expected {expected} but got {actual}");
                }
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / Fix64.PrecisionDecimal);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / Fix64.PrecisionDecimal);
        }

        [TestMethod]
        public void Cos()
        {
            Assert.IsTrue(Fix64.Cos(Fix64.Zero) == Fix64.One);

            Assert.IsTrue(Fix64.Cos(Fix64.QuarterRot) == Fix64.Zero);
            Assert.IsTrue(Fix64.Cos(Fix64.HalfRot) == -Fix64.One);
            Assert.IsTrue(Fix64.Cos(Fix64.HalfRot + Fix64.QuarterRot) == Fix64.Zero);
            Assert.IsTrue(Fix64.Cos(Fix64.FullRot) == Fix64.One);

            Assert.IsTrue(Fix64.Cos(-Fix64.QuarterRot) == -Fix64.Zero);
            Assert.IsTrue(Fix64.Cos(-Fix64.HalfRot) == -Fix64.One);
            Assert.IsTrue(Fix64.Cos(-Fix64.HalfRot - Fix64.QuarterRot) == Fix64.Zero);
            Assert.IsTrue(Fix64.Cos(-Fix64.FullRot) == Fix64.One);


            for (double angle = -360.0; angle <= 360.0; angle += 0.001)
            {
                var f = (Fix64)angle;
                var actualF = Fix64.Cos(f);
                var expected = (decimal)Math.Cos(ToRadians(angle));
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.IsTrue(delta <= 0.0001M, $"Cos({angle}): expected {expected} but got {actualF}");
            }

            foreach (var val in _testCases)
            {
                var f = Fix64.FromRaw(val);
                var actualF = Fix64.Cos(f);
                var expected = (decimal)Math.Cos(ToRadians((double)f));
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.IsTrue(delta <= 0.0001M, $"Cos({f}): expected {expected} but got {actualF}");
            }
        }

        [TestMethod]
        public void Tan()
        {
            Assert.IsTrue(Fix64.Tan(Fix64.Zero) == Fix64.Zero);
            Assert.IsTrue(Fix64.Tan(Fix64.HalfRot) == Fix64.Zero);
            Assert.IsTrue(Fix64.Tan(-Fix64.HalfRot) == Fix64.Zero);

            Assert.IsTrue(Fix64.Tan(Fix64.QuarterRot) == Fix64.MaxValue);
            Assert.IsTrue(Fix64.Tan(-Fix64.QuarterRot) == Fix64.MinValue);

            Assert.IsTrue(Fix64.Tan(Fix64.QuarterRot - Fix64.PrecisionUnit) > Fix64.Zero);
            Assert.IsTrue(Fix64.Tan(Fix64.QuarterRot + Fix64.PrecisionUnit) < Fix64.Zero);
            Assert.IsTrue(Fix64.Tan(-Fix64.QuarterRot - Fix64.PrecisionUnit) > Fix64.Zero);
            Assert.IsTrue(Fix64.Tan(-Fix64.QuarterRot + Fix64.PrecisionUnit) < Fix64.Zero);

            int inputBitPrecision = 10;
            int expectedFunctionPrecision = 5;
            double startRange = -90.0;
            double endRange = 90.0;
            double increment = Math.Pow(2, -inputBitPrecision);
            for (double angle = startRange; angle <= endRange; angle += increment)
            {
                var f = (Fix64)angle;
                var actualF = Fix64.Tan(f);
                var expected = (Fix64)Math.Tan(ToRadians(angle));

                AreEqualWithinBitPrecision(actualF, expected, expectedFunctionPrecision);
            }
        }

        [TestMethod]
        public void Atan()
        {
            var maxDelta = 0.0000003m;
            var deltas = new List<decimal>();

            Assert.AreEqual(Fix64.Zero, Fix64.Atan(Fix64.Zero));

            // Precision
            for (var x = -1.0; x < 1.0; x += 0.0001)
            {
                var xf = (Fix64)x;
                var actual = (decimal)Fix64.Atan(xf);
                var expected = (decimal)ToDegrees(Math.Atan((double)xf));
                var delta = Math.Abs(actual - expected);
                deltas.Add(delta);
                Assert.IsTrue(delta <= maxDelta, $"Precision: Atan({xf}): expected {expected} but got {actual}");
            }

            // Scalability and edge cases
            foreach (var x in _testCases)
            {
                var xf = (Fix64)x;
                var actual = (decimal)Fix64.Atan(xf);
                var expected = (decimal)ToDegrees(Math.Atan((double)xf));
                var delta = Math.Abs(actual - expected);
                deltas.Add(delta);
                Assert.IsTrue(delta <= maxDelta, $"Scalability: Atan({xf}): expected {expected} but got {actual}");
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / Fix64.PrecisionDecimal);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / Fix64.PrecisionDecimal);
        }
        
        // TODO: Figure out what to do with these benchmarks.
        public void AtanBenchmark()
        {
            var deltas = new List<decimal>();

            var swf = new Stopwatch();
            var swd = new Stopwatch();

            for (var x = -1.0; x < 1.0; x += 0.001)
            {
                for (int k = 0; k < 1000; ++k)
                {
                    var xf = (Fix64)x;

                    swf.Start();
                    var actualF = Fix64.Atan(xf);
                    swf.Stop();

                    swd.Start();
                    var expectedRadians = Math.Atan((double)xf);
                    swd.Stop();
                    var expected = expectedRadians * 180.0 / Math.PI;

                    deltas.Add(Math.Abs((decimal)actualF - (decimal)expected));
                }
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / Fix64.PrecisionDecimal);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / Fix64.PrecisionDecimal);
            Console.WriteLine("Fix64.Atan time = {0}ms, Math.Atan time = {1}ms", swf.ElapsedMilliseconds, swd.ElapsedMilliseconds);
        }

        private void AssertEqualWithinPrecision(Fix64 expected, Fix64 actual, Fix64 precision)
        {
            var delta = Fix64.Abs(expected - actual);

            Assert.IsTrue(precision >= delta, $"Precision: (expected: {expected}, actual: {actual}, expectedDelta: {precision}, actualDelta: {delta}");
        }

        [TestMethod]
        public void Atan2()
        {
            var deltas = new List<decimal>();

            var precision = (Fix64)0.000001;

            // Identities
            AssertEqualWithinPrecision(Fix64.Zero, Fix64.Atan2(Fix64.Zero, Fix64.Zero), precision);
            AssertEqualWithinPrecision(Fix64.Zero, Fix64.Atan2(Fix64.Zero, Fix64.One), precision);
            AssertEqualWithinPrecision(Fix64.EighthRot, Fix64.Atan2(Fix64.One, Fix64.One), precision);
            AssertEqualWithinPrecision(Fix64.QuarterRot, Fix64.Atan2(Fix64.One, Fix64.Zero), precision);
            AssertEqualWithinPrecision(Fix64.QuarterRot + Fix64.EighthRot, Fix64.Atan2(Fix64.One, -Fix64.One), precision);
            AssertEqualWithinPrecision(Fix64.HalfRot, Fix64.Atan2(Fix64.Zero, -Fix64.One), precision);
            AssertEqualWithinPrecision(-Fix64.QuarterRot - Fix64.EighthRot, Fix64.Atan2(-Fix64.One, -Fix64.One), precision);
            AssertEqualWithinPrecision(-Fix64.QuarterRot, Fix64.Atan2(-Fix64.One, Fix64.Zero), precision);
            AssertEqualWithinPrecision(-Fix64.EighthRot, Fix64.Atan2(-Fix64.One, Fix64.One), precision);

            // Precision
            for (var y = -1.0; y < 1.0; y += 0.01)
            {
                for (var x = -1.0; x < 1.0; x += 0.01)
                {
                    var yf = (Fix64)y;
                    var xf = (Fix64)x;
                    var actual = Fix64.Atan2(yf, xf);
                    var expected = (decimal)ToDegrees(Math.Atan2((double)yf, (double)xf));
                    var delta = Math.Abs((decimal)actual - expected);
                    deltas.Add(delta);
                    Assert.IsTrue(delta <= 0.005M, $"Precision: Atan2({yf}, {xf}): expected {expected} but got {actual}");
                }
            }

            // Scalability and edge cases
            foreach (var y in _testCases)
            {
                foreach (var x in _testCases)
                {
                    var yf = (Fix64)y;
                    var xf = (Fix64)x;
                    var actual = (decimal)Fix64.Atan2(yf, xf);
                    var expected = (decimal)ToDegrees(Math.Atan2((double)yf, (double)xf));
                    var delta = Math.Abs(actual - expected);
                    deltas.Add(delta);
                    Assert.IsTrue(delta <= 0.005M, $"Scalability: Atan2({yf}, {xf}): expected {expected} but got {actual}");
                }
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / Fix64.PrecisionDecimal);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / Fix64.PrecisionDecimal);
        }

        // TODO: Figure out what to do with these benchmarks.
        public void Atan2Benchmark()
        {
            var deltas = new List<decimal>();

            var swf = new Stopwatch();
            var swd = new Stopwatch();

            foreach (var y in _testCases)
            {
                foreach (var x in _testCases)
                {
                    for (int k = 0; k < 1000; ++k)
                    {
                        var yf = (Fix64)y;
                        var xf = (Fix64)x;

                        swf.Start();
                        var actualF = Fix64.Atan2(yf, xf);
                        swf.Stop();

                        swd.Start();
                        var expectedRadians = Math.Atan2((double)yf, (double)xf);
                        swd.Stop();
                        var expected = expectedRadians * 180.0 / Math.PI;

                        deltas.Add(Math.Abs((decimal)actualF - (decimal)expected));
                    }
                }
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / Fix64.PrecisionDecimal);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / Fix64.PrecisionDecimal);
            Console.WriteLine("Fix64.Atan2 time = {0}ms, Math.Atan2 time = {1}ms", swf.ElapsedMilliseconds, swd.ElapsedMilliseconds);
        }

        [TestMethod]
        public void Negation()
        {
            foreach (var operand1 in _testCases)
            {
                var f = Fix64.FromRaw(operand1);
                if (f == Fix64.MinValue)
                {
                    Assert.AreEqual(-f, Fix64.MaxValue);
                }
                else
                {
                    var expected = -((decimal)f);
                    var actual = (decimal)(-f);
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        [TestMethod]
        public void EqualsTests()
        {
            foreach (var op1 in _testCases)
            {
                foreach (var op2 in _testCases)
                {
                    var d1 = (decimal)op1;
                    var d2 = (decimal)op2;
                    Assert.IsTrue(op1.Equals(op2) == d1.Equals(d2));
                }
            }
        }

        [TestMethod]
        public void EqualityAndInequalityOperators()
        {
            var sources = _testCases.Select(Fix64.FromRaw).ToList();
            foreach (var op1 in sources)
            {
                foreach (var op2 in sources)
                {
                    var d1 = (double)op1;
                    var d2 = (double)op2;
                    Assert.IsTrue((op1 == op2) == (d1 == d2));
                    Assert.IsTrue((op1 != op2) == (d1 != d2));
                    Assert.IsFalse((op1 == op2) && (op1 != op2));
                }
            }
        }

        [TestMethod]
        public void CompareTo()
        {
            var nums = _testCases.Select(Fix64.FromRaw).ToArray();
            var numsDecimal = nums.Select(t => (decimal)t).ToArray();
            Array.Sort(nums);
            Array.Sort(numsDecimal);
            Assert.IsTrue(nums.Select(t => (decimal)t).SequenceEqual(numsDecimal));
        }
    }
}
