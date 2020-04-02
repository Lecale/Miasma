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
     List<Player> AllPlayers = new List<Player>();
		List<Player> RoundPlayers;
		List<Pairing> AllPairings = new List<Pairing> ();
		List<Pairing> RoundPairings = new List<Pairing> ();
    
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
			if (pst.Equals("FOLD")) 	PairingStrategy = "Fold";
      if (pst.Equals("SPLIT"))  PairingStrategy = "Split";
      if (pst.Equals("ADJACENT")) PairingStrategy = "Adjacent";

      bool overwritePlayers = true;
      if (File.Exists(fOut))
      {
        Console.WriteLine("Players file already exists - Overwrite it? (yes / no)");
				if (Console.ReadLine().ToUpper().StartsWith ("N")) overwritePlayers = false;
        else File.Delete(fOut);
			}

      if(overwritePlayers)
        using (StreamWriter riter = new StreamWriter(fOut))
        {
          riter.WriteLine("= Tournament Name:\t" + name + ",");
          riter.WriteLine("= Copy Paste the Player information into the sheet,");
          riter.WriteLine("= PIN , Name, Rating, Club, Country,");
          riter.WriteLine("= type the number of the round to denote an allocated Bye,");
          riter.WriteLine("=======================================================");
          string hdr = "Pin,Name,Rating,Club,Country";
          string bdy = ",,,,";
          for (int i = 0; i < nRounds; i++)
          {
            hdr = hdr + ",R" + (i + 1);
            bdy = bdy + ",";
          }
          riter.WriteLine(hdr);
          riter.WriteLine(bdy);
        }
      //write or update tournament settings
      bool overwriteSettings = true;
			string fSettings = tourDirectory + "settings.txt";
			if (File.Exists(fSettings))
			{
				Console.WriteLine("Settings file already exists - Overwrite it? (yes / no)");
				if (Console.ReadLine().ToUpper().StartsWith("N"))	overwriteSettings = false;
				else File.Delete(fSettings);
			}
      if(overwriteSettings)
			  using (StreamWriter riter = new StreamWriter (fSettings)) {
          riter.WriteLine("Tournament Name:\t" + name);
          riter.WriteLine("Rounds:\t" + nRounds);
          riter.WriteLine("Pairing Strategy:\t" + PairingStrategy);
				  if (TopBar) {
					  riter.WriteLine ("Top Bar Rating:\t");
					  riter.WriteLine ("Permit handicap above bar:\tNo");
				  }
				  if(RatingFloor) riter.WriteLine ("Rating Floor:\t");
				  riter.WriteLine ("Handicap Policy:\t"+HandiAdjust);
				  riter.WriteLine ("Max Handicap:\t"+nMaxHandicap);		
				  riter.WriteLine ("Grade Width:\t"+nGradeWidth);		
				  riter.WriteLine ("Tiebreak 1:\tSOS");		
				  riter.WriteLine ("Tiebreak 2:\t");		
				  riter.WriteLine ("Tiebreak 3:\t");		
			  }

      //
      return true;
   }

   public void ReadPlayers(bool Supression=false, bool Initial = false)
   {
     if (Initial) awaitText("Please type 'done' when you have finished editing players.txt", true);
   }

   public void awaitText(string instruction, bool clearConsole=true, string keyPhrase="done")
   {
    bool awaiting = true;
    while (awaiting)
    {
      Console.WriteLine(instruction);
      if (keyPhrase.ToLower().Equals(Console.ReadLine())) awaiting = false;
    }
    if (clearConsole) Console.Clear();
   }
    public void ReadSettings() 
    {
      char[] ch = { '\t'};
			string[] s;
			string dbg="";
			try {
			using (StreamReader sr = new StreamReader (tourDirectory + "settings.txt"))
      {
        while(sr.EndOfStream == false)
        {
          dbg = sr.ReadLine ();
						if(dbg!=null & dbg.Length > 2)
            {
              s =dbg.Split(ch);
              if(s[0].Contains("Tournament Name")) TournamentName = s[1];
              if(s[0].Contains("Rounds")) nRounds = int.Parse(s[1].Trim());
              if(s[0].Contains("Pairing Strategy")) PairingStrategy = s[1].Trim();
              if(s[0].Contains("Handicap Policy")) HandiAdjust = int.Parse(s[1].Trim());
							if(s[0].Contains("Max Handicap")) nMaxHandicap = int.Parse(s[1].Trim());
              if(s[0].Contains("Grade Width")) nGradeWidth = int.Parse(s[1].Trim());
              if(s[0].Contains("Debug") && s.Length > 1) Verbose = true;
              if (s[0].Contains("Top Bar Rating"))
              {
                nTopBar = int.Parse(s[1].Trim());
                TopBar = true;
              }
              if (s[0].Contains("Rating Floor"))
              {
                nRatingFloor = int.Parse(s[1].Trim());
                RatingFloor = true;
              }
              if(s[0].Contains("Permit handicap above bar")){
	              if (s[1].ToUpper().StartsWith("Y")) HandiAboveBar = true;
              }
              if (s[0].Contains("Tiebreak ") && s.Length > 1) 
              {
                if (s[1].Trim() != "")
                {
                  if (s.Length == 3)  //Tiebreakers applying to END or Pairing
                  {
                    TeaBreak = true;    //There is a difference in 2 lists
                    if(s[2].ToUpper().Equals("END")) EndTiebreakers.Add(s[1].ToUpper());
                    if (s[2].ToUpper().Equals("PAIR")) Tiebreakers.Add(s[1].ToUpper());
                  }
                  else //ALL
                  {
                    Tiebreakers.Add(s[1].ToUpper());
                    EndTiebreakers.Add(s[1].ToUpper());
                  }
                }
              }
            }
        }
      }
      } catch(Exception e) {
        Console.WriteLine ("ReadSettings() exception: {0} {1}", dbg, e.InnerException);
    }
       //Player.SetTiebreakers(Tiebreakers);
    }
    public void GeneratePlayers(int nPlayers=17, int midpoint=1500, int spread=500 )
		{
      Console.WriteLine("Generate dummy players for Test Tournament? ( yes / no)");
      if (Console.ReadLine().ToUpper().StartsWith("Y"))
      {
        Console.WriteLine("Enter 3 comma separated params (nPlayers,Midpoint,Spread)");
        string auto = Console.ReadLine();
        string[] split = auto.Split(new char[] {','});
        if (split.Length == 3)
        {
          nPlayers = int.Parse(split[0]);
          midpoint = int.Parse(split[1]);
          spread = int.Parse(split[2]);
          Utility u = new Utility();
          string end = "";
          string fn = tourDirectory + "players.txt";
          int i = 999100; //fake EGD pin
          using (StreamWriter sw = new StreamWriter(fn, true))
          {
            for (int np = 0; np < nPlayers; np++)
              sw.WriteLine(i++ + "," + u.ProvideName() + "," + u.RatingByBox(midpoint, spread) + ",BLF,IE,1k" + end);
          }
        }
        else
          Console.WriteLine("Data entry error, players were not autogenerated");
      }
  }

  public void SortField(bool init=false)
	{
		if (init) Player.SortByRating = true;
		AllPlayers.Sort ();
		int i = 1;
		if (init) { //late players get a different Seed because they were late and that is how it goes
				//top group but for the bye will then be seeded as if in top group
				//this should not have any negative effect on the pairings though
			Player.SortByRating = false;
			foreach (Player p in AllPlayers) 
				p.Seed = i++;
		}
	}
  
  
  }
}