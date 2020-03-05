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
            Console.WriteLine("1: Create New Tournament");
            Console.WriteLine("2: Load Tournament");
            Console.WriteLine("3: Update Player File");
            int choice = Listen(3);
            switch (choice)
            {
              case 1: ; break;
              case 2: ; break;
              case 3: ; break;
            }
            
            /*
            NimMain();
            IntPtr strPtr = HelloNim(1);
            var str = Marshal.PtrToStringUTF8(strPtr);
            Console.WriteLine(str);
            HelloDim(1);
            ZipFile.ExtractToDirectory("allworld_lp.zip","Data");
            */
        }


        [DllImport("HelloNim.dll")]
        public static extern void NimMain();
        
        [DllImport("HelloNim.dll")]
        public static extern IntPtr HelloNim(int a);
        [DllImport("HelloNim.dll")]
        public static extern void HelloDim(int a);

        
    static private int Listen(int choices = 1)
    {
        bool loop = true;
        while(loop)
        {
            try{int i = int.Parse(Console.ReadLine());
            if(i>0 && i<=choices) return i; } catch{}
        }
        return 0;
    }
}

}
