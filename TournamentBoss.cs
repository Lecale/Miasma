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
		if (init) { //late players get a different Seeding because they were late, and that is how it goes
				//top group but for the bye will then be seeded as if in top group
				//this should not have any negative effect on the pairings though
			Player.SortByRating = false;
			foreach (Player p in AllPlayers) 
				p.Seed = i++;
		}
	}
  
  public void MakeDraw(int currentRound = 1)
	{
    Console.Clear();
		Console.WriteLine ("We are ready to make the draw for Round "+currentRound);
		if (currentRound > 1) 
    {
			Console.WriteLine ("Do you want to add a new player to the players list (yes / no)");
		  string s = Console.ReadLine ();
		  if (s.ToUpper ().Trim ().StartsWith ("Y")) HandleLatePlayers (currentRound);
		  Console.WriteLine ("Do you want to update player participation (byes) in the players list (yes / no)");
		  s = Console.ReadLine ();
		  if (s.ToUpper ().Trim ().StartsWith ("Y")) {
        awaitText("After updating the players file type 'done' to continue",true);
			  ReadByesFromFile (currentRound);
		  }
		}
		int i = -1;	//Handle this later
		UpdateParticipation (currentRound);
		if (currentRound == 1)  InitMMS ();
		else                  SortField ();
		if (RoundPlayers.Count % 2 == 1) 
    {
			i = AssignBye (currentRound);
			while (i == -1) 
      {
				int retry = 2;
				Console.WriteLine ("No player was found who already had less than " + (retry - 1) + "bye");
				if (currentRound <= retry)
					i = AssignBye (currentRound, retry++);
				else 
        {
					i = 100;
					Console.WriteLine ("Fatal error encountered. Tournament cannot proceed");
				}
			}
    }
    List<Pairing> RndPairings = new List<Pairing>();
		Console.WriteLine ("The draw is being made ...");
    if (PairingStrategy.ToUpper().Equals("SIMPLE"))
    {
			DrawMachine1 dm1 = new DrawMachine1(RoundPlayers, AllPairings, nMaxHandicap, HandiAdjust, HandiAboveBar, Verbose);
      RndPairings = dm1.GetCurrentPairings();
    }
    if (PairingStrategy.ToUpper().Equals("FOLD"))
    {
			DrawMachine2 dm2 = new DrawMachine2(AllPlayers, AllPairings, currentRound, nMaxHandicap, HandiAdjust, HandiAboveBar, Verbose);
      RndPairings = dm2.GetCurrentPairings();
    }
    if (PairingStrategy.ToUpper().Equals("SPLIT"))
    {
      DrawMachine3 dm3 = new DrawMachine3(AllPlayers, AllPairings, currentRound, nMaxHandicap, HandiAdjust, HandiAboveBar, Verbose);
      RndPairings = dm3.GetCurrentPairings();
    }
    if (PairingStrategy.ToUpper().Equals("ADJACENT"))
    {
      DrawMachine4 dm4 = new DrawMachine4(AllPlayers, AllPairings, currentRound, nMaxHandicap, HandiAdjust, HandiAboveBar, Verbose);
      RndPairings = dm4.GetCurrentPairings();
    }
		if(Verbose)
			foreach (Pairing rp in RndPairings)
        Console.WriteLine(rp.BasicOutput());
    GenerateRoundResultsFile(currentRound, RndPairings);
    Console.WriteLine();
		Console.WriteLine ("The draw is available at Round"+currentRound+"Results.txt");
    Console.WriteLine("Remember that the draw can be overwritten in the input file");
    Console.WriteLine("When you are ready to read in the results press Return");
    string anyKey = Console.ReadLine();
    if (anyKey.ToUpper().Equals("AUTO"))
    {
      Console.WriteLine("Result autogeneration was selected");
      GenerateResultsForRound(currentRound);
    }
  }

  public void InitMMS() 
	{
		SortField (true);
		foreach (Player ap in AllPlayers)
    {
			int gap = nTopBar - ap.getERating ();
		  gap = gap / nGradeWidth;
			if (gap >= 0 && ap.topBar == false) gap++;
      ap.setMMS(100 - gap); 
      ap.setInitMMS(100 - gap);
		}
	}

  public void UpdateParticipation(int _rnd)
	{
		RoundPlayers = new List<Player> ();
		foreach (Player p in AllPlayers) {
			if (p.getParticipation (_rnd - 1))
				RoundPlayers.Add (p);
			else
				p.AssignBye (_rnd);
		}
		Console.WriteLine ("The number of players competing in round " + _rnd + " is " + RoundPlayers.Count);
	}
  public int AssignBye(int _rnd, int ByeLevel=1)
	{ 
		for (int i = RoundPlayers.Count - 1; i >-1; i--) {
			if (RoundPlayers [i].nBye() < ByeLevel) {
				Console.WriteLine ("A bye will be assigned to:");
				Console.WriteLine (RoundPlayers [i].ToString ());
				RoundPlayers [i].AssignBye (_rnd);
				RoundPlayers.RemoveAt (i);
				return i;
			}
		}
			//emergency
		Console.WriteLine("Strangely, no Candidate for the Bye was found...");
		return -1;
	}

public void HandleLatePlayers(int rnd)
{
	Console.WriteLine ("Late entrants must be added to the file players.txt");
	Console.WriteLine ("When ready, press return to proceed");
	string s = Console.ReadLine ();
	int before = AllPlayers.Count;
	ReadPlayers(false); //later true
	int after = AllPlayers.Count;
	//init mms and then give byes
  for (int i = after; i > before; i--)
  {
    AllPlayers[i - 1].SetSeed(i);
		AllPlayers [i - 1].topBar = false; //should already be false?
    if(TopBar)
      if (AllPlayers[i - 1].getERating() > nTopBar)
      {
        AllPlayers[i - 1].setERating(nTopBar+1);
      }
    if(RatingFloor)
    	if (AllPlayers [i - 1].getERating () < nRatingFloor)
	    	AllPlayers [i - 1].setERating (nRatingFloor);
        //set initial mms
    int gap = nTopBar - AllPlayers[i-1].getERating();
    gap = gap / nGradeWidth;
    if (gap >= 0 && AllPlayers[i - 1].topBar == false)  gap++;
    AllPlayers[i - 1].setInitMMS(100 - gap);
    //assign bye
		for (int j = 1; j < rnd; j++) 
			AllPlayers [i - 1].AssignBye (j);
		string hlpDebug = "";
		for (int k = 0; k < nRounds; k++)
			if (AllPlayers [i - 1].getParticipation (k))
				hlpDebug += (k + 1) + " ";
		Console.WriteLine (AllPlayers [i - 1].ToString () + " plays in "+ hlpDebug);
  }
	GenerateStore (); //Else seeding is not recorded and a bug appears
	SortField ();
}

public void ReadByesFromFile(int nextRound)
{
			//read players file
			//if player already registered
			//check if his participation changed
	string tLn = "";
	string fin = tourDirectory + "players.txt";
	using (StreamReader reader = new StreamReader (fin)) {
		for (int i = 0; i < 6; i++) tLn = reader.ReadLine(); //trip through headers
		while ((tLn = reader.ReadLine ()) != null) 
    {
		  String[] split = tLn.Split(new char[] {',','\t'});
			try {
				int pine = int.Parse (split [0]);
				int rats = int.Parse (split [2]);
				bool[] bull = new bool[nRounds]; //not set via input file
				for(int k=0; k<bull.Length; k++)  bull[k]=true;
        if(split.Length > 6)
				  for (int i = 6; i < split.Length; i++) {
						if (split [i].Equals ("") == false) {
							try{
								int byeRound = int.Parse(split[i].Trim());
								if(byeRound > nRounds){
									Console.WriteLine("A bye cannot be allocated for a round which does not exist");
									Console.WriteLine(tLn);
								}
								else
									bull[byeRound-1]=false; //0 based
						  }
							catch(Exception e){Console.WriteLine(e.Message);}
						}
					}
					Player j = new Player (pine, split [1], rats, split [3], split [4],  bull, split[5]);
					if (AllPlayers.Contains (j) == true) {
						for(int ap = 0; ap<AllPlayers.Count; ap++)
							if(j.Equals(AllPlayers[ap]))
								for(int i2=nextRound-1; i2<nRounds; i2++) //0 based
									AllPlayers[ap].SetParticipation(i2, bull[i2]);
					}
				} catch (Exception e) {
					Console.WriteLine ("An exception was encountered in ReadByesFromFile" + e.Message);
					Console.WriteLine (tLn);
				}
			}
		}
  }

public void GenerateResultsForRound(int rnd)
{ 
  Utility u = new Utility();
  u.EnterResults(tourDirectory, rnd);
}

public void GenerateRoundResultsFile(int rnd, List<Pairing> ps)
{
	string fOut = tourDirectory + "Round" + rnd + "Results.txt";
  using (StreamWriter riter = new StreamWriter(fOut))
  {
    riter.WriteLine("Results of Round " + rnd);
    riter.WriteLine("Bd\twhite\t:\tblack");
		int count = 0;
    foreach (Pairing p in ps)
      riter.WriteLine(++count + "\t" + p.ToFile());
  }
}

public void GenerateStore()
{	
  string fOut = tourDirectory + "Init.txt";
	using (StreamWriter riter = new StreamWriter(fOut))
		foreach (Player p in AllPlayers)
			riter.WriteLine(p.ToStore());
}

public void previewFloor(bool SetFloor =  false)
{
  if (RatingFloor)
  {
    int tCount = 0;
    foreach (Player p in AllPlayers)
      if (p.getRating() < nRatingFloor)
      {
        Console.WriteLine(p.getName() + " " + p.getRating());
        tCount++;
      }
    Console.WriteLine("Provisionally " + tCount + " in bottom group");
  if (SetFloor)
  {
    Console.WriteLine("Apply this setting (yes / no )");
    if (Console.ReadLine().ToUpper().StartsWith("Y"))
      foreach (Player pete in AllPlayers)
        if (nRatingFloor > pete.getRating())
          pete.setERating(nRatingFloor);
    else
    {
      Console.WriteLine("Enter a new value for the Rating Floor");
      nRatingFloor = int.Parse(Console.ReadLine().Trim());
      previewFloor(true);
    }
  }
}
}
public void previewTopBar(bool SetBar = false)
{
  if (TopBar)
  {
    int tCount = 0;
    foreach (Player peter in AllPlayers)
    {
      if (peter.getRating() > nTopBar && peter.nBye() == 0)
      {
        Console.WriteLine(peter.getName() + " " + peter.getRating());
                        tCount++;
                    }
                }
                Console.WriteLine("Provisionally " + tCount + " in top group");
                if (SetBar)
                {
                    Console.WriteLine("Apply this setting (yes / no )");
                    if (Console.ReadLine().ToUpper().StartsWith("Y"))
                    {
                        foreach (Player pete in AllPlayers)
                        {
                            if (nTopBar < pete.getRating() && pete.nBye() == 0)
                            {
                                pete.topBar = true;
                                pete.setERating(nTopBar+1);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Enter a new value for the Rating of the Top Bar");
                        nTopBar = int.Parse(Console.ReadLine().Trim());
                        previewTopBar(true);
                    }
                }
            }
		}

public void ReadResults(int rnd)
{
	List<Pairing> actualPairs = new List<Pairing> ();
	char[] c = { '\t','\v'};
	char[] c1 = { '(',')','?'};
	int[] LUT = new int[AllPlayers.Count];
	int[] CNT = new int[AllPlayers.Count];
  for (int i = 0; i < AllPlayers.Count; i++){
    if(Verbose) Console.WriteLine("ReadResults()i:" + i + ":S:" + AllPlayers[i].Seed);
    LUT[AllPlayers[i].Seed - 1] = i;
  }
  using (StreamReader sr = new StreamReader(tourDirectory + "Round" + rnd + "Results.txt"))
  {
		sr.ReadLine (); sr.ReadLine ();
		string s;
		while (sr.EndOfStream == false) {
			try{
				s = sr.ReadLine().Trim (); }
			catch{ s = "";
			}
			if (s.Length >2 ) {	
				string[] split = s.Split (c);
				string[] split2 = split [1].Split (c1);
				int white = int.Parse (split2 [1]) -1;
				split2 = split [3].Split (c1);
				int black = int.Parse (split2 [1]) -1;
				int handicap = int.Parse (split [4].Replace ("h", ""));
				int result = 0;
        bool validResult = false;
        if (split[2].Equals("1:0")) { result = 1; validResult = true; }
        if (split [2].Equals ("0:1")) { result = 2; validResult = true; }
        if (split [2].Equals ("0.5:0.5")) { result = 3; validResult = true; }
        if (split [2].Equals ("0:0")) { result = 7; validResult = true; }
        if (validResult == false)
        {
          Console.WriteLine("The following result could not be processed.");
          Console.WriteLine(s);
          Console.WriteLine("You MUST choose one of these 4 options");
          Console.WriteLine("Type '1' for  1:0");
          Console.WriteLine("Type '2' for  0:1");
          Console.WriteLine("Type '3' for  0.5:0.5");
          Console.WriteLine("Type '7' for  0:0");
          result = int.Parse(Console.ReadLine());
        }
				if(Verbose) Console.WriteLine("Read Pairing: " + AllPlayers[LUT[white]].Seed + ":" + AllPlayers[LUT[black]].Seed);
				Pairing p = new Pairing (AllPlayers [LUT [white]], AllPlayers [LUT [black]], handicap, result);
				CNT [LUT [white]]++;
				CNT [LUT [black]]++;
				if (actualPairs.Contains (p) == false)
					if (CNT [LUT [white]] == 1 && CNT [LUT [black]] == 1)
						actualPairs.Add (p);
					else
						throw new Exception ("A player played more than one game in this round.");
				else
					Console.WriteLine ("A duplicate pairing result was detected " + p.ToFile ());
			}
		}
				//and if we did not hit an exception
		Console.WriteLine("Finished reading in results for Round " + rnd);
		RoundPairings = actualPairs;
    AllPairings.AddRange(actualPairs);
  }
}

//It should be possible to reverse Byes using this method
	public void ProcessResults(int rnd)
  {
    if(rnd == 1) GenerateStore();
		Console.WriteLine ("Processing Results: rnd " + rnd + " pairings " + RoundPairings.Count);
		//lookuptable
		foreach (Pairing p in RoundPairings) {
			if(p.white.getParticipation(rnd-1)==false) 
				Console.WriteLine("The following was originally assigned a bye:" + p.white.getName());
			if(p.black.getParticipation(rnd-1)==false) 
				Console.WriteLine("The following was originally assigned a bye:" + p.black.getName());
			p.white.setResult (rnd, p.black.Seed, p.WhiteScore (), p.GetHandi (), 1);
			p.black.setResult (rnd, p.white.Seed, p.BlackScore (), p.GetHandi (), 0);
			if(Verbose)	Console.WriteLine("W" + p.white.Seed + ": B " + p.black.Seed);
		}
		//now check for maladjusted bye
		foreach (Player ap in AllPlayers) {
      if (ap.getOpponent(rnd - 1) == 0)
        if (ap.getResult(rnd - 1) != 0.5)  
        {
          Console.WriteLine("Assigning a bye to " + ap.getName());
          ap.AssignBye(rnd);//AssignBye adjusts the rnd number
        }
			}
		}


//Should have some if SOS if MOS if SODOS logic
public void UpdateTiebreaks(int rnd)
{
  int[] lookUp = new int[AllPlayers.Count]; //yuck
  for (int i = 0; i < AllPlayers.Count; i++)
    lookUp[AllPlayers[i].Seed - 1] = i;
  foreach (Player ap in AllPlayers)
  {
    float _SOS = 0;
    float _MOS = 0; //do not calculate if rnd <3
    float _SODOS = 0;
    float maxSOS = -999;
    float minSOS = 999;
    //to add _SOR , SODOR

    //find actual games played from participation
    int nGame = 0;
		int dbg = -1;
    for (int i = 0; i < rnd; i++)
    {
			dbg = i;
      try
      {
        if (ap.getParticipation(i) == true)
        {
          nGame++;
          int op = ap.getOpponent(i) -1; 
					float f = AllPlayers[lookUp[op]].getMMS() ;
					f = f + ap.getAdjHandi(i);
          _SOS += f;
          _SODOS += (f * ap.getResult(i));
          if (f > maxSOS)
            maxSOS = f;
          if (f < minSOS)
            minSOS = f;
        }
        else
        {
					if(Verbose) Console.WriteLine("a bye detected for " +ap.Seed + " in round "+i);
        }
      }
      catch (Exception e) 
      {
				Console.WriteLine("rnd:" + dbg + " " + e.Message);
        Console.WriteLine("Seed was " + ap.Seed);
        for (int ie = 0; ie < rnd; ie++ )
          Console.WriteLine("Opponent " + ie + " was " + ap.getOpponent(ie));
      }
    }
		float _score = ap.getScore (rnd);

		if (nGame == 0)
			_SOS = ap.getMMS() * rnd; //used to be initMMS();
    if (nGame < rnd)
    {
			if (nGame != 0) 
      {
			  _SOS = _SOS * rnd / nGame;
        if (_score != 0.5f * (float)(rnd - nGame))
        {
          _SODOS = _SODOS * (_score / (_score - (0.5f * (float)(rnd - nGame))));
        }
        else
        {
          _SODOS = 0.5f * _SOS / (rnd); // assign half mean SOS
        }
			}
      else
      {
				_SODOS = 0.5f * _SOS / (rnd);
			}
    }
            
		if (rnd > 2)
			_MOS = _SOS - maxSOS - minSOS;
		else
			_MOS = _SOS; //maybe this is normal ??
    
    ap.SOS = _SOS;
    ap.MOS = _MOS;
    ap.SODOS = _SODOS;
    if (_score == 0 || _SODOS == 0)
    { ap.MDOS = 0;}
    else
    { ap.MDOS = _SODOS / _score; }                                    
  }
  // now that all players are updated, we can calculate SOSOS 
  if (Tiebreakers.Contains("SOSOS") || EndTiebreakers.Contains("SOSOS"))
  {//if we have to             
    foreach (Player ap in AllPlayers)
    {
      float _SOSOS = 0;
      int nGame = 0;
      for (int i = 0; i < rnd; i++)
      {
        try
        {
          if (ap.getParticipation(i) == true)
          {
            nGame++;
            int op = ap.getOpponent(i) - 1;
                                //float f = AllPlayers[lookUp[op]].getMMS();
                                //f = f + ap.getAdjHandi(i);
                                //_SOS += f;
            _SOSOS += AllPlayers[lookUp[op]].SOS;
          }
        }
      catch {} //would be really odd to have an exception here
    }
    if (nGame < rnd)
    {
      if (nGame != 0)
      {
        _SOSOS = _SOSOS * rnd / nGame;
      }
      else
      {
        _SOSOS = rnd * ap.SOS; 
      }
    }
    ap.SOSOS = _SOSOS;
  }
}
}

public void GenerateStandingsfile(int rnd)
{
  if (File.Exists(tourDirectory + "Round" + rnd + "Standings.txt"))
  {
    Console.WriteLine("Warning - Overwriting Round" + rnd + "Standings.txt");
    File.Delete(tourDirectory + "Round" + rnd + "Standings.txt");
  }
  string hdr = "Pl\tName\tRank\tRating\tMMS\tWins\t";
  for (int i = 0; i < rnd; i++)
    hdr = hdr + (i+1) + "\t";
  foreach(string tb in Tiebreakers)
    hdr = hdr + tb + "\t";

  using (StreamWriter sw = new StreamWriter(tourDirectory + "Round" + rnd + "Standings.txt"))
  {
    sw.WriteLine("Tournament: " + TournamentName + " Round: " + rnd);
    sw.WriteLine("");
    sw.WriteLine(hdr);
    int cnt = 1; 
    string t = "\t";
    foreach (Player ap in AllPlayers)
    {
			sw.WriteLine(cnt++ + t + ap.ToStanding(rnd) + TiebreakerOut(ap)); 
    }
  }
	VerboseStandings (rnd); //To be made optional ?
}

private string TiebreakerOut(Player p)
{
	string s = "";
	foreach (string tb in Tiebreakers) {
		if (tb.Equals ("SOS")) s += (p.SOS + "\t");
		if (tb.Equals ("SODOS")) s += (p.SODOS + "\t");
    if (tb.Equals("MOS")) s += (p.MOS + "\t");
    if (tb.Equals("MDOS"))  s += (p.MDOS + "\t");
    if (tb.Equals("SOSOS"))  s += (p.SOSOS + "\t");
    if (tb.Equals("SOR"))  s += (p.SOR + "\t");
    if (tb.Equals("SODOR"))  s += (p.SODOR + "\t");
  }
return s;
}

//Can this method just be junked ?
public void VerboseStandings(int rnd)
		{
			string hdr = "Pl\tName\tRank\tRating\tMMS\tWins\t";
			for (int i = 0; i < rnd; i++)
				hdr = hdr + (i+1) + "\t";
			foreach(string tb in Tiebreakers)
				hdr = hdr + tb + "\t";
			if (rnd == 0)
			    if (File.Exists (tourDirectory + "RoundStandings.txt"))
				    File.Delete (tourDirectory + "RoundStandings.txt");
			using (StreamWriter sw = new StreamWriter (tourDirectory + "RoundStandings.txt", true)) {
				sw.WriteLine ("Standings for Round " + rnd);
				sw.WriteLine("");
				sw.WriteLine(hdr);
				int cnt = 1;
				foreach (Player ap in AllPlayers)
				{
					sw.WriteLine(cnt++ + "\t" + ap.ToStandingVerbose(rnd) + TiebreakerOut(ap)); 
				}
				sw.WriteLine("");
			}

		}



  }
}