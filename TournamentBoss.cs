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
		  string PairingStrategy = "Simple";
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

  }
}