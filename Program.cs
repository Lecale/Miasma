using System;
using System.Runtime.InteropServices;
using System.IO.Compression;

namespace Miasma
{
    class Program
    {
        public static TournamentBoss tb;
        public static int startRound = 1;

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Miasma has loaded");
            Console.WriteLine("1: Create New Tournament");
            Console.WriteLine("2: Load Tournament");
            Console.WriteLine("3: Update Registered Players File");
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
            */
        }


        [DllImport("HelloNim.dll")]
        public static extern void NimMain();
        
        [DllImport("HelloNim.dll")]
        public static extern IntPtr HelloNim(int a);
        [DllImport("HelloNim.dll")]
        public static extern void HelloDim(int a);

        static void newTournament()
        {
            tb = new TournamentBoss();
            bool newT = tb.GenerateTemplateInputFile();
            if(newT == false)   RestoreTournament(true);
            Console.WriteLine("Do you need to download the EGF Rating List? (yes / no)");
            if (Console.ReadLine().ToUpper().StartsWith("Y"))
            tb.DownloadAllWorld();           
            // ZipFile.ExtractToDirectory("allworld_lp.zip","Data");
    
            tb.GeneratePlayers(); 
            tb.ReadPlayers(true,true);
            tb.SortField(true);
        }

        static void RestoreTournament(bool redirection=false)
        {
            if(redirection==false)  tb = new TournamentBoss();
        }


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
