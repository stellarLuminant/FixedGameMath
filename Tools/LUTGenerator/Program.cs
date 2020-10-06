using FixedGameMath;
using System.IO;

namespace LUTGenerator
{
    internal class Program
    {
        internal static void Main()
        {
            WriteSinLutToFile();
            WriteTanLutToFile();
        }

        internal const string SinLutHeading = 
@"namespace FixedMath.NET 
{
    partial struct Fix64 
    {
        public static readonly long[] SinLut = new[] 
        {";

        internal const string TanLutHeading = 
@"namespace FixedMath.NET 
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
            var sinLut = Fix64.GenerateSinLut();

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
            var tanLut = Fix64.GenerateTanLut();

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
