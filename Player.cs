﻿﻿using System;
using System.Collections.Generic;

namespace Miasma
{
	public class Player : Person, IComparable<Player> //Inheritance probably useless
    {
		#region variable
    public float initMMS = -1;
		protected float MMS = -1;  
    protected float[] score;
		protected bool[] participation;
		protected int[] opponent;
		protected int[] handi;
		protected int[] BlackWhite; //Black0White1
    public bool topBar = false; //potentially no handicap above bar
    public float SOS = -1; //Solkoff
		public float MOS = -1; //Median
    public float SODOS = -1; //Sonnenborg-Bergen
    public float MDOS = -1; //Tampere
    public float SOSOS = -1; //
    protected int eRating; // effective rating, used for lower bar
		public int EGDPin;
		public int Deed = -1; //Deed is the draw seeding for a particular round

		public float firstRating = -1; //new tiebreak
    public float secondRating = -1;
    public int SODOR = 0; //New tiebreaker
    public int SOR = 0; //New tiebreaker

		private static List<string> Tiebreaker = new List<string> ();
		public static bool SortByRating = false;
		#endregion

        //the bool[] par must be changed to a short in order to indicate 0 point byes
		public Player(int _seed, string _nom , int _rat, string _ctry, string _club, bool[] par, string _grd) 
			: base (_nom, _rat, _club, _ctry )
        {
			Seed = -1; //When we first read Players in they are not sorted
			EGDPin = _seed; 
			eRating = _rat;
      Rank = _grd;
			//Take care, participation does not exist
			participation = new bool[par.Length];
			score = new float[par.Length];
			BlackWhite = new int[par.Length];
			handi = new int[par.Length];
			opponent = new int[par.Length];
			for(int i=0; i<par.Length; i++)
				participation[i]=par[i];
		}
			
		#region ResultsByesMMS

		public void setResult(int rnd, int op, float _score, int _handicap=0, int BW =1)
		{
      rnd--; //0 based arrary as always
      participation[rnd] = true; //manually removed byes must be erased
			score[rnd] = _score;
			opponent[rnd] = op;
			handi[rnd] = _handicap;
			BlackWhite[rnd] = BW;
			//setMMS(getMMS (rnd));
			setMMS(getMMS ());
		}

		public void SetParticipation(int rnd, bool play=true)
		{
			participation[rnd] = play;
		}

        //Might be possible to use negative round numbers to create a 0 point bye instead of normal 0.5 points
		public void AssignBye(int rnd)
		{
      rnd--; //0 based arrary as always
			participation[rnd] = false;
      score[rnd] = 0.5f;
      setMMS(getMMS(rnd));
		}

    public float getMMS()
    {
      float f = initMMS;
			if(opponent!=null)
            for (int i = 0; i < opponent.Length; i++)
                f += score[i];
           // f += MMS;
           return f;
    }

    public float getResult(int rnd)
    {
      return score[rnd];
    }
    public float getScore(int rnd)
    {
      float f = 0;
      for (int i = 0; i < rnd; i++)
        f += score[i] ;
      return f;
    }
    public float getMMS(int rnd)
    {
      return initMMS + getScore(rnd);
    }
    public void setMMS(float s)
    {
      MMS = s;
    }
    public void setInitMMS(float s)
    {
      initMMS = s;
    }
    #endregion

		#region Tiebreak Calculation
		public int getOpponent(int i)
		{
			return opponent[i];
		}

		public int getAdjHandi(int i)
		{
			//if black substract handicap , if White add handicap to SOS
			try{
				if(BlackWhite[i]==1)
					return handi[i] * BlackWhite[i];
				else
					return handi[i] * -1;
			}
			catch(Exception e) {
				Console.WriteLine ("EXCEPTION in getAdjHandi rnd " + i);
				Console.WriteLine (e.Message);
				Console.WriteLine ("handi " + handi.Length);
				Console.WriteLine ("BlackWhite " + BlackWhite.Length);
				return 0;
			}
		}
		#endregion

		public bool getParticipation(int i)
		{
			try{
			  return participation[i];}
			catch(Exception e) {
				Console.WriteLine ("EXCEPTION in getParticipation rnd " + i);
				Console.WriteLine (e.Message);
				Console.WriteLine ("par " + participation.Length);
				return false;
			}
		}
    public void setOpponent(int i, int rnd)
    {
      opponent[rnd] = i;
    }
    public int nBye()
    {
      int n =0;
      for (int i = 0; i < participation.Length; i++)
        if (!participation[i]) n++;
      return n;
    }
        public string getName()
        {
            return Name;
        }
        public int getRating()
        {
            return Rating;
        }
		public int getERating()
		{
			return eRating;
		}
		public void setERating(int _rating)
		{
			eRating = _rating;
		}

        public int getSeed()
        { return Seed; }
        public void setTop()
        { topBar = true; }

        public int[] GetOpposition() 
        {
           return opponent;
        }

		#region Override Methods

		public override bool Equals(System.Object obj)
		{
			if (obj == null)
				return false;
			try{
				Player p = (Player) obj;

			if(Seed>0)
			{
				if(Seed==p.Seed)
					return true;
			}
			if(EGDPin==p.EGDPin)
					return true;
			}
			catch {
				return false;
			}
			return false;
		}
		#endregion

		public static void SetTiebreakers(List<string> _tie)
		{
			Tiebreaker = _tie;
		}
		public float qSc()
		{
			return MMS - initMMS;
		}

		public int CompareTo(Player p)
        {
			if (SortByRating) {
				if (p.eRating > eRating)
					return 1;
				if (p.eRating == eRating)
					return 0;
				return -1;
			}
			//Sort by MMS
            if (p.MMS > MMS)
                return 1;
			if (p.MMS == MMS) {
				foreach(string tie in Tiebreaker)
				{
                    if (tie.Equals("SOS"))
                    {
                        if (p.SOS > SOS)
                            return 1;
                        if (p.SOS < SOS)
                            return -1;
                    }if (tie.Equals("SOSOS"))
                    {
                        if (p.SOSOS > SOSOS)
                            return 1;
                        if (p.SOSOS < SOSOS)
                            return -1;
                    }
                    if (tie.Equals("MOS"))
					{
						if(p.MOS > MOS)
							return 1;
						if(p.MOS < MOS)
							return -1;
                    } if (tie.Equals("SODOS"))  
                    {
						//SODOS should be split by Wins first
						if (p.qSc() > qSc())
							return 1;
						if (p.qSc() < qSc())
							return -1;
						//wins are even
						if (p.SODOS > SODOS)
                            return 1;
                        if (p.SODOS < SODOS)
                            return -1;
                    } if (tie.Equals("MDOS"))
					{
						if (p.MDOS > MDOS)
							return 1;
						if (p.MDOS < MDOS)
							return -1;
					}if (tie.Equals("SODOR"))
					{
						if (p.SODOR > SODOR)
							return 1;
						if (p.SODOR < SODOR)
							return -1;
					}if (tie.Equals("SOR"))
					{
						if (p.SOR > SOR)
							return 1;
						if (p.SOR < SOR)
							return -1;
					}
				}
				return 0;
			}
            return -1;
        }

		#region output
        public override string ToString()
        {
            return Name + " " + eRating + "(" + MMS + ")";
        }

        public string ToDebug()
        {
			return ToString() + ".IM."+ initMMS + " S(" + Seed + ")";
        }

		public string ToStore()
		{
      return EGDPin + "\t" + Seed + "\t" + initMMS + "\t" + Name;
		}

		public string ToFile()
		{
			char[] c = { ' '};
			string[] split = Name.Split (c);
			if(split.Length ==2)
				return split[0]+"."+split[1].Substring(0,1).ToUpper() + "(" + Seed + ")";
			else
				return split[0] + "(" + Seed + ")";
		}

		public string ToStanding(int rnd)
		{
			string s = ToFile() ;
			s = s + "\t(" + Rank + ")\t(" + Rating + ")\t";
			s = s + getMMS() + "\t" + getScore(rnd) + "\t";
			for(int i=0; i<rnd; i++)
			{
				s = s + opponent[i];
				if (score[i] == 0) s = s + "-\t";
				if (score[i] == 1) s = s + "+\t";
				if (score[i] == 0.5) s = s + "=\t";
			}
			return s;
		}
		public string ToStandingVerbose(int rnd)
		{
			string s = ToFile() ;
			s = s + "\t(" + Rank + ")\t(" + Rating + ")\t";
			s = s + getMMS() + "\t" + getScore(rnd) + "\t";
			for(int i=0; i<rnd; i++)
			{
				s = s + opponent[i];
				if (score[i] == 0) s = s + "-";
				if (score[i] == 1) s = s + "+";
				if (score[i] == 0.5) s = s + "=";
				if(BlackWhite[i]==0)	s+="b";
				if(BlackWhite[i]==1)	s+="w";//bye mightbe2
				s+=getAdjHandi(i);
				s+="\t";
			}
					
			return s;
		}

    public string ToEGF()
    {
      return  Name + " " + Rank + " " + Country + " " + Club + " "; //EGD identifiers
    }

		public string EGFColour(int rnd){
			if(BlackWhite[rnd]==0)
				return "/b";
			return "/w";
		}

		#endregion
  }

}
