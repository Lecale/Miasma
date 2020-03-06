using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.IO.Compression;

namespace Miasma
{ 
    public class TournamentBoss
    {
    #region Things
      public int nRounds = 0;
      public int currentRound = 0;
      public string exeDirectory;
      public string dataDirectory;
      public string tourDirectory;
	    public string TournamentName;
	  	bool TopBar=false;
		  bool RatingFloor=false;
      bool HandiAboveBar = false;
      bool Verbose = false;
      bool TeaBreak = false; //EndTiebreakers
  		int HandiAdjust=1;
	  	int nMaxHandicap = 9;
		  int nTopBar = 5000;
		  int nRatingFloor = 100;
		  int nGradeWidth = 100; //to take from Settings
		  string PairingStrategy = "None";
    /* List<Player> AllPlayers = new List<Player>();
		List<Player> RoundPlayers;
		List<Pairing> AllPairings = new List<Pairing> ();
		List<Pairing> RoundPairings = new List<Pairing> ();
    */
    List<string> Tiebreakers = new List<string>(); //to take from Settings
    List<string> EndTiebreakers = new List<string>(); //Final round only
      public TournamentBoss()
      {
        exeDirectory = Directory.GetCurrentDirectory();
        dataDirectory = exeDirectory + @"\Data\";
      }
    #endregion
  public void DownloadAllWorld()
	{
			Console.WriteLine ("Downloading data file allworld_lp.zip ...");
			string uri = "http://www.europeangodatabase.eu/EGD/EGD_2_0/downloads/allworld_lp.zip";
			if( File.Exists(dataDirectory + "allworld_lp.zip")) File.Delete(dataDirectory + "allworld_lp.zip");
			if( File.Exists(dataDirectory + "allworld_lp.html")) File.Delete(dataDirectory + "allworld_lp.html");
			FileInfo fi;
      WebClient client = new WebClient ();
			client.DownloadFile (uri, dataDirectory + "egzipdata.zip");
			Console.WriteLine ("File downloaded");
		  fi = new FileInfo (dataDirectory + "egzipdata.zip");
      ZipFile.ExtractToDirectory(dataDirectory + "egzipdata.zip", dataDirectory);
  }

   public bool GenerateTemplateInputFile()
   {
      try
      {
        Console.WriteLine("Please enter the name of the working folder for the tournament.");
        Console.WriteLine("This path should be relative to the exe file.");
        tourDirectory = exeDirectory + "\\"+ Console.ReadLine().Trim();
      } catch { GenerateTemplateInputFile(); }
      if (Directory.Exists (tourDirectory) == false)
				Directory.CreateDirectory (tourDirectory);
      tourDirectory += "\\";
			Console.WriteLine("Working directory is: "+tourDirectory);
      // Does tournament data already exist here?
      if (File.Exists(tourDirectory + "settings.txt") &&
                File.Exists(tourDirectory + "players.txt") &&
                File.Exists(tourDirectory + "Init.txt"))
      {
        Console.WriteLine("Would you like to restore saved tournament data? (yes / no) ");
        string _answer = Console.ReadLine();
        if (_answer.ToUpper().StartsWith("Y"))
          return false; // Enter load stored tournament mode from Program.cs
      }
      string fOut = tourDirectory + "players.txt";
			Console.WriteLine("Template file created at: "+fOut);
      Console.WriteLine("Setting Tournament Information...");
      Console.WriteLine("Please enter the Tournament Name:");
      string name = Console.ReadLine();
      Console.WriteLine("Please enter number of Rounds:");
			string round = Console.ReadLine();
      try {
        nRounds = int.Parse(round);
      } catch {
        Console.WriteLine("Number of rounds set to 3 as it could not be read.");
        nRounds = 3; 
      }
      Console.WriteLine ("Top Group ? (yes / no )");
			string top  = Console.ReadLine();
			if (top.ToUpper ().StartsWith ("Y")) {
				TopBar = true; Console.WriteLine ("Noted. Top Group Rating can be entered in Settings");
			}
      Console.WriteLine ("Rating Floor ? (yes / no )");
      string bot  = Console.ReadLine ();
			if (bot.ToUpper ().StartsWith ("Y")) {
				RatingFloor = true; Console.WriteLine ("Noted. Rating Floor can be entered in Settings");
			}
      Console.WriteLine ("Handicap Adjustment ? (0 for none)");
      try {
        HandiAdjust = int.Parse(Console.ReadLine());
      }  catch {
        HandiAdjust = 1; Console.WriteLine("Handicap Adjustment set to 1 as it could not be read");
      }
      Console.WriteLine("Maximum Handicap Allowed ?");
			try {
        nMaxHandicap = int.Parse(Console.ReadLine());
      } catch {
        Console.WriteLine("Maximum Handicap set to 9 as it could not be read."); nMaxHandicap = 9;
      }
      Console.WriteLine ("Grade Width (default 100)");
			try{
				nGradeWidth = int.Parse(Console.ReadLine()); 
			} catch {
				Console.WriteLine("Grade width set to 100 as it could not be read."); nGradeWidth = 100;
			}
      Console.WriteLine ("Pairing Strategy (Fold / Split / Adjacent / None)");
			string pst = Console.ReadLine().ToUpper().Trim();
			if(pst.Equals("FOLD")) 	PairingStrategy = "Fold";
      if (pst.Equals("SPLIT"))  PairingStrategy = "Split";
      if (pst.Equals("ADJACENT")) PairingStrategy = "Adjacent";

      bool overwritePlayers = true;
      if (File.Exists(fOut))
      {
        Console.WriteLine("Players file already exists - Overwrite it? (yes / no)");
				if (Console.ReadLine().ToUpper().StartsWith ("N")) overwritePlayers = false;
        else File.Delete(fOut);
			}

      return true;
   }

  }
}