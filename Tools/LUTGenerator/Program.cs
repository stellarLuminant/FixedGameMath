using FixedGameMath;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace LUTGenerator
{
    // Excluded from code coverage because:
    // - this is not part of the user api
    // - this is a part of implementation of already tested functions
    [ExcludeFromCodeCoverage]
    internal class Program
    {
        internal static void Main()
        {
            WriteSinLutToFile();
            WriteTanLutToFile();
        }

        internal const string SinLutHeading = 
@"namespace FixedGameMath
{
    partial struct Fix64 
    {
        public static readonly long[] SinLut = new[] 
        {";

        internal const string TanLutHeading = 
@"namespace FixedGameMath
{
    partial struct Fix64 
    {
        public static readonly long[] TanLut = new[] 
        {";

        internal const string LutFooter =
@"
        };
    }
}";
        internal const string FourSpaceTab = "    ";
        internal const string ThreeTabs = FourSpaceTab + FourSpaceTab + FourSpaceTab;

        internal static void WriteSinLutToFile()
        {
            Console.WriteLine("Generating Sine Lookup Table...");
            var sinLut = Fix64.GenerateSinLut();

            Console.WriteLine($"Writing {sinLut.Length} entries to file...");

            using var writer = new StreamWriter("Fix64SinLut.cs");

            writer.Write(SinLutHeading);

            var lineCounter = 0;
            foreach (var value in sinLut)
            {
                if (lineCounter++ % 8 == 0)
                {
                    writer.WriteLine();
                    writer.Write(ThreeTabs);
                }

                writer.Write($"0x{value:X}L, ");
            }

            writer.Write(LutFooter);
        }

        internal static void WriteTanLutToFile()
        {
            Console.WriteLine("Generating Tangent Lookup Table...");
            var tanLut = Fix64.GenerateTanLut();

            Console.WriteLine($"Writing {tanLut.Length} entries to file...");

            using var writer = new StreamWriter("Fix64TanLut.cs");

            writer.Write(TanLutHeading);

            var lineCounter = 0;
            foreach (var value in tanLut)
            {
                if (lineCounter++ % 8 == 0)
                {
                    writer.WriteLine();
                    writer.Write(ThreeTabs);
                }
                
                writer.Write($"0x{value:X}L, ");
            }

            writer.Write(LutFooter);
        }
    }
}
