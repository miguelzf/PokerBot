using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Drawing;


namespace pokermaster
{
	static class Combinatorics
	{

		private static void swap(int a, int b) { int t = a; a = b; b = t; }

		private static void reverse(int[] s, int b, int f)
		{
			for (; b < f; f--, b++)
				swap(s[b], s[f]);
		}


		// best permutation generator
		static public void ReversingQuickPerm(int[] a, int N)
		{
			int[] p = new int[N + 1];
			int i;

			for (i = 0; i <= N; i++)
				p[i] = i;

			for (i = 1; i < N; )
			{
				p[i]--;
				reverse(a, 0, i);
				// print(a);
				for (i = 1; p[i] == 0; i++)
					p[i] = i;		// reset p[i] zero value
			}
		}


		static Random rand = new Random(2346789); // DateTime.UtcNow.Millisecond);
		
		// changes fromDeck
		static public void selectPermRandom(int[] toDeck, int[] ofromDeck, int N, int total)
		{
			int[] fromDeck = new int[ofromDeck.Length];
			Array.Copy(ofromDeck, fromDeck, total);

			for (int r = 0; r < N; )
			{
				int k = rand.Next(total);
				if (fromDeck[k] != 0) {
					toDeck[r++] = fromDeck[k];
					fromDeck[k] = 0;
				}
			}
		}

		const int HAND_CARDS	= PokerHand.HAND_CARDS;
		static private int[] ids	= new int[HAND_CARDS + 1];

		static public void initCombination()
		{
			for (int i = 1; i <= HAND_CARDS; i++)
				ids[i] = i;
		}


		static public void nextCombination(int[] resDeck)
		{
			int i;

			// generate new combination
			for (i = HAND_CARDS; ids[i] == PokerApp.DECK_CARDS - HAND_CARDS + i; i--) ;
			if (i == 0) IO.Message("#4tgh"); // last combination

			ids[i]++;
			for (int j = i + 1; j <= HAND_CARDS; j++)
				ids[j] = ids[i] + j - i;

			Array.Copy(ids, 1, resDeck, 0, HAND_CARDS);
		}

	}

}
