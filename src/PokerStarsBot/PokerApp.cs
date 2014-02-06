#define REQUIRE_KEY
#undef	REQUIRE_KEY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;
using HoldemHand;


namespace pokermaster
{
	public class PokerApp
	{
		public enum ProbMode
		{
			MonteCarlo,
			Enumerate
		};


		// global game constants

		public const int TOTAL_HANDS= 2598960;	// 2,6 million games

		public const int TOTAL_SYMS = 100000000;
		
		public const int DECK_CARDS	= 52;
		
		public const int TAKEN_CARDS= 7;
		
		public const int SUIT_CARDS	= 14;
		
		public const int TOTAL_SUITS= 4;

		int CFourKind	= 0;
		int CFullHaus	= 0;
		int CStrFlush	= 0;
		int CFlushie	= 0;
		int CStraight	= 0;
		int COneTrio	= 0;
		int CTwoPairs	= 0;
		int COnePair	= 0;
		int CHighCard	= 0;


		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			PokerApp poker = new PokerApp();
			
#if REQUIRE_KEY	
			Keys.RequireKey();
#endif		


			int[] cards = IO.parseCards("7s 6d");
//			if (!Hand.ValidateHand(str))
//				IO.Message("invalid formart");

			Process pp = Process.GetCurrentProcess();
			Stopwatch sw = new Stopwatch();
			sw.Start();
			TimeSpan tb = pp.TotalProcessorTime;
			//			poker.testHands();
//			poker.testHand(IO.parseCards("As 2s 3s 4s 5s"));
//			Application.Run(new PokerUI(poker));

//			double prob = poker.MyComputeProb(cards, new int[TAKEN_CARDS], 0, 1, ProbMode.Enumerate);
			double prob = poker.HandEvalComputeProb("7s 6d",  "", 0, 1);

			TimeSpan te = pp.TotalProcessorTime;
			sw.Stop();

			IO.Message("Computed " + TOTAL_SYMS + " hands in " + (te-tb).TotalMilliseconds + " millis: "
						+ (double) TOTAL_SYMS / (te - tb).TotalSeconds + " games/sec");

			IO.Message("Win Prob: " + prob * 100 + "%");
//			IO.Message("Computed " + TOTAL_SYMS + " hands in " + sw.ElapsedMilliseconds + " millis");
//			poker.printCounts();

		}

		void printCounts()
		{
			IO.Message(
			"CStrFlush " + CStrFlush + "\r\n" +
			"CFourKind " + CFourKind + "\r\n" +
			"CFullHaus " + CFullHaus + "\r\n" +
			"CFlushie  " + CFlushie + "\r\n" +
			"CStraight " + CStraight + "\r\n" +
			"COneTrio  " + COneTrio + "\r\n" +
			"CTwoPairs " + CTwoPairs + "\r\n" +
			"COnePair  " + COnePair + "\r\n" +
			"CHighCard " + CHighCard + "\r\n");
		}

		void testHand(Card[] cards)
		{
			int[] ids = new int[TAKEN_CARDS];
			int i = 0;
			foreach (Card c in cards)
				ids[i++] = c.cardToId();
			testHand(ids);
		}

		void testHand(int[] cards)
		{
			int val = PokerHand.HandValue(cards);

				 if (val <= (int)PokerHand.Hand.HighCard) CHighCard++;
			else if (val <= (int)PokerHand.Hand.OnePair ) COnePair++;
			else if (val <= (int)PokerHand.Hand.TwoPairs) CTwoPairs++;
			else if (val <= (int)PokerHand.Hand.OneTrio ) COneTrio++;
			else if (val <= (int)PokerHand.Hand.Straight) CStraight++;
			else if (val <= (int)PokerHand.Hand.Flushie ) CFlushie++;
			else if (val <= (int)PokerHand.Hand.FullHaus) CFullHaus++;
			else if (val <= (int)PokerHand.Hand.FourKind) CFourKind++;
			else if (val <= (int)PokerHand.Hand.StrFlush) CStrFlush++;
//			IO.WriteLine(IO.stringIds(ids, 1));
		}

		void testHands()
		{
			const int HAND_CARDS = PokerHand.HAND_CARDS;

			int[] deck = new int[HAND_CARDS];
			int[] ids = new int[HAND_CARDS + 1];
			int i, j;

			for (i = 1; i <= HAND_CARDS; i++) ids[i] = i;

			IO.Write("Generating all possible hands:\n");

			for (int cc = 0; cc < TOTAL_SYMS; cc++)
			{
				// calculate hand value
				Array.Copy(ids, 1, deck, 0, deck.Length);
				testHand(deck);

//				if (cc > TOTAL_SYMS - 10000)
//					IO.WriteLine(IO.stringIds(deck));

				// generate new combination
				for (i = HAND_CARDS; ids[i] == DECK_CARDS - HAND_CARDS + i; i--) ;
				if  (i == 0) break;

				ids[i]++;
				for (j = i + 1; j <= HAND_CARDS; j++)
					ids[j] = ids[i] + j - i;
			}
		}
		

		// using the HandEvaluator lib, ported from PokerEval by Keith Rule

		public double HandEvalComputeProb(int[] myCards, int[] sharedCards, int NsharedCards, int nplayers)
		{
			return 0;
		}

		// number of opponents
		public double HandEvalComputeProb(string myCards, string sharedCards, int NsharedCards, int nplayers)
		{
			uint wins = 0, losses = 0, draws = 0;
			    
            ulong pocketmask = Hand.ParseHand(myCards);
			ulong board = Hand.ParseHand(sharedCards);

			int cc = 0;

            // Iterate through all possible opponent hands
            foreach (ulong oppmask in Hand.Hands(0UL, board | pocketmask, 2))
                // Iterate through all possible board cards                
                foreach (ulong boardmask in Hand.Hands(board, pocketmask | oppmask, 5-NsharedCards))
				{
					if (++cc == TOTAL_SYMS)
						return (double)(wins + draws / 2) / (double)(draws + losses + wins);

					// Evaluate the player and opponent hands and tally the results    
                    uint pocketHandVal = Hand.Evaluate(pocketmask | boardmask, TAKEN_CARDS);
                    uint oppHandVal = Hand.Evaluate(oppmask | boardmask, TAKEN_CARDS);

                    if (pocketHandVal > oppHandVal)
                        wins++;
                    else if (pocketHandVal == oppHandVal)
                        draws++;
					else
						losses++;
                }

			return (double)(wins + draws / 2) / (double)(draws + losses + wins);
		}



		// returns number of selected cards. the bdeck array carries the result
		int selectDeck(int[] bdeck, int[] remove)
		{
			int count = 0;
			bdeck[0] = 0;
			
			for (int n = 1; n <= DECK_CARDS; n++)
				if (Array.IndexOf<int>(remove, n) < 0)
					bdeck[count++] = n;

			for (int n = count+1; n <= DECK_CARDS; n++)
				bdeck[n] = 0;
			
			return count;
		}


		// number of opponents
		public double MyComputeProb(int[] myCards, int[] sharedCards,	int NsharedCards, int nplayers, ProbMode mode)
		{
			if (myCards.Length < 2)
				throw new Exception("Necessario pelo menos duas cartas");

			Array.Copy(sharedCards, 0, myCards, 2, NsharedCards);

			int NmyCards= 2+NsharedCards;
			int p, wins	= 0, losses = 0, draws = 0;
			int myMiss	= TAKEN_CARDS - NmyCards;
			int othersMiss = TAKEN_CARDS - NsharedCards;

			int[] bufCards = new int[TAKEN_CARDS ];
			int[] baseDeck = new int[DECK_CARDS+1];
			int[] gameDeck = new int[DECK_CARDS+1];

			int permLen = selectDeck(baseDeck, myCards);

//			IO.Write("AVAILABLE CARDS " + permLen + ":\n" + IO.stringIds(baseDeck, 1, permLen+1) + "\n");
//			IO.Write(IO.stringCards(myCards) + ": INITIAL\t\t\tX\t" + IO.stringCards(sharedCards) + ": INITIAL\n");

			if (mode == ProbMode.Enumerate)
				Combinatorics.initCombination();

			for (int i = 0; i < TOTAL_SYMS; i++)
			{
				bool draw = false;

				if (mode == ProbMode.Enumerate)
					Combinatorics.nextCombination(gameDeck);
				else
					Combinatorics.selectPermRandom(gameDeck, baseDeck, othersMiss, permLen);

				const bool PHANDS = false;
//				IO.Write(IO.stringIds(gameDeck, 0, permLen) + "\n");
				
				// take the needed cards from the deck and distribute it along the players
				Array.Copy(myCards , 0, bufCards, 0, NmyCards);
				Array.Copy(gameDeck, 0, bufCards, NmyCards, myMiss);

				// check Hand. destructive
				int myHandVal = PokerHand.HandValue(bufCards);
				if (PHANDS) IO.Write(IO.stringCards(bufCards) + ": " + myHandVal + " " + PokerHand.getHand(myHandVal) + "\tX\t");

				for (p = 0; p < nplayers; p++)
				{
					Array.Copy(sharedCards, 0, bufCards, 0, NsharedCards);
					Array.Copy(gameDeck, 0, bufCards, NsharedCards, othersMiss);

					// check hand
					int otherHandVal = PokerHand.HandValue(bufCards);

					if (PHANDS) IO.Write(IO.stringCards(bufCards) + ": " + otherHandVal + " " + PokerHand.getHand(otherHandVal) + "\t");

					if (myHandVal == otherHandVal)
						draw = true;

					if (myHandVal <  otherHandVal)
						break;
				}

				if (p < nplayers)
					losses++;
				else
				if (!draw)
					wins++;
				else
					draws++;

				if (PHANDS)
				{
					if (p < nplayers)
						IO.Write(" => LOSE\n\n");
					else if (!draw)
						IO.Write(" => WIN\n\n");
					else
						IO.Write(" => DRAW\n\n");
				}
			}

			return (double) wins / (double) (losses + wins);
		}
		

	}
}


