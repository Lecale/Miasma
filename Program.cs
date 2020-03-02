using System;
using System.Runtime.InteropServices;
using System.IO.Compression;

namespace Miasma
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Miasma has loaded");

            NimMain();
            IntPtr strPtr = HelloNim(1);
            var str = Marshal.PtrToStringUTF8(strPtr);
            Console.WriteLine(str);
            HelloDim(1);
            // ZipFile.ExtractToDirectory("allworld_lp.zip","Data");
        }

        [DllImport("HelloNim.dll")]
        public static extern void NimMain();
        
        [DllImport("HelloNim.dll")]
        public static extern IntPtr HelloNim(int a);
        [DllImport("HelloNim.dll")]
        public static extern void HelloDim(int a);
    }
}
