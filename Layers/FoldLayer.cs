using System;
using System.Collections.Generic;

namespace Miasma
{
	public class FoldLayer //Should extend Generic Layer
	{
		public float MMSKey;
		private List<int> population;
    private List<int> stack;

		public FoldLayer (float MMS, int Seed)
		{
			MMSKey = MMS;
      stack = new List<int>();
      population = new List<int> ();
			population.Add (Seed);
      stack.Add(Seed);
		}

        // 1 2 3 (4)
		public void Add(int _seed)
		{
			if (population.Contains (_seed) == false) {
				population.Add (_seed);
				stack.Add (_seed);
			}
		}

		//_Seed is not used here ??
		public int Pop(int _Seed, int[] opp)
		{
            bool popIt = true;
            int hold = -1;
            for (int i = stack.Count - 1; i > -1; i--)
                for (int o = 0; o < opp.Length; o++) {
                    if (opp[o] == population[i])
                        popIt = false;
                    if (popIt)
                    {
                        hold =  population[i];
                        stack.RemoveAt(i); //population always static
                        return hold;
                    }
                    
                }                     
            return -1; //NoMatch
		}

		//use this after Offer
		// stack is available 
		//  population is original
		public int Eject(int Request)
		{
			for (int i = stack.Count-1; i > -1; i--)
				if (stack [i] == Request) {
					stack.RemoveAt (i);
					return Request;
				}
			Console.WriteLine ("Horrid error in FoldLayer:Eject()"); 
			return -1;
		}

		//this is an ordered list of available opponents in the Fold Layer
		//can only return opponents in the Stack (the active list)
		public List<int> Offer(int _Target , int []opp)
		{
			//string dbg = " ";
			List<int> Offrage = new List<int> ();
			for (int i = stack.Count-1; i > -1; i--)
                if (opp == null || opp.Length == 0)
                {
                    if (stack[i] != _Target)
                        Offrage.Add(stack[i]);
                }
                else
                {
					bool good = true;
                    for (int o = 0; o < opp.Length; o++)	
						if (opp [o] == stack [i])
							good = false;
					if (stack [i] == _Target)
						good = false;
					if(good)
                       Offrage.Add(stack[i]);
                }
		//	foreach (int eye in Offrage)
		//		dbg += eye + " ";
		//	Console.WriteLine ("Offrage" + dbg); 
			return Offrage;
		}

        //Push is only used for re-injection
    public void Push(int _Seed)
    {
      int origin = -1; 
      bool found = false;
      for (int i = population.Count-1; i > -1; i--)
        if (population[i] == _Seed)
        {
          origin = i;
          break;
        }
        int nigiro = 99999;
        for (int j = stack.Count-1; j > -1; j--)
        {
          for (int i = population.Count-1; i > -1; i--)
            if (population[i] == stack[j])
            {
              nigiro = i; break;
            }
            if (nigiro < origin)
            {
              stack.Insert(j + 1, _Seed);
              found = true;
              j = -2;
            }
          }
          if(found==false) stack.Add(_Seed);
        }


        public int StackSize()
        { return stack.Count;  }


		public bool Contains(int i)
		{
      if(stack.Contains(i)) return true;
			return false;
		}

		public bool Contained(int i)
		{
      if(population.Contains(i)) return true;
			return false;
		}

		public override string ToString()
		{
			string s = "MMS:" + MMSKey + ":";
			foreach (int i in stack) {
				s += i + ",";
			}
			return s;
		}
	}
}