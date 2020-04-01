using System;
using System.Collections.Generic;
using System.IO;

namespace Miasma
{
	public class Utility
	{
		private Random r;
		private string[] fn = {"Alison","Bob","Calum","Darren","Eoin","Fra","Ginger","Hal","Irina","Jo","Ken","Lucian","Meghan","Nicky","Olli","Peg","Rach","Sue","Tony","Unt"};
		private string[] pre = {"O'","Mc","Ker","Ze","Van","Herr","Mac"};
		private string[] mid = {"Blue","Row","Land","Sea","Flack","Black","Whit","Red","Ash","Round","Tri","Green"};
		private string[] end = {"better","lower","water","later","son","morn","mane","ston","crake","flute","pie","cake","snorkler","nibbler"};

		public Utility ()
		{
			r = new Random();
		}

		public string ProvideName()
		{
			string first = fn [r.Next (fn.Length)];
			string surname = pre [r.Next (pre.Length)] + mid [r.Next (mid.Length)] + end [r.Next (end.Length)];
			return surname + " " + first; 
		}

		public int RatingByBox(int mid, int width)
		{
			double d1 = r.NextDouble ();
            double d2 = r.NextDouble();
            double d = d1 * d2;
            double plusMinus = r.NextDouble();
            if (plusMinus > 0.5)    return (int)((width * d) + mid);
            else   return (int)( mid - (width * d));
		}

        public void EnterResults(string dir, int rnd)
        {
            List<string> allLines = new List<string>();
            using (StreamReader sr = new StreamReader(dir + "Round" + rnd + "Results.txt"))
            {
                while (sr.EndOfStream == false)
                    allLines.Add(sr.ReadLine());
            }
            using (StreamWriter sw = new StreamWriter(dir + "Round" + rnd + "Results.txt",false))
            {
                int counter = 0;
                foreach (string al in allLines)
                {
                    if (counter < 2)
                        sw.WriteLine(al);
                    else
                    {
                        if (r.NextDouble() < 0.45)
                        {
                            sw.WriteLine(al.Replace("?:?", "1:0"));
                        }
                        else
                        {
                            if (r.NextDouble() < 0.9)
                            {
                                sw.WriteLine(al.Replace("?:?", "0:1"));
                            }
                            else 
                            {
                                if (r.NextDouble() < 0.98)
                                    sw.WriteLine(al.Replace("?:?", "0.5:0.5"));
                                else
                                    sw.WriteLine(al.Replace("?:?", "0:0")); 
                            }
                        } 
                    }
                        counter++;
                }
            }

        }

        //Utility::Search by EGD Pin or Country
        public void egfTextSearchResults(List<string> tokens)
        {
            // this now calls nim script
        }
	}
}

