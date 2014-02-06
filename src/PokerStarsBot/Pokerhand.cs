using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace pokermaster
{
	/*
	 * ==============================================================
	 * ||						Poker Hands							||
	 * ==============================================================
	 *
	 * NEVER use suits to evaluate hands (break ties in hand value)
	 * 
	 * Two straights are ranked by comparing the highest card of each.
	 * Two straights with the same high card are of equal value, suits are not used to separate them.
	 *
	 * Two flushes are compared as if they were high card hands; the highest ranking card of each is compared to determine the winner
	 * If both hands have the same highest card, then the 2nd-highest ranking card is compared, and so on until a difference is found
	 *
	 * Suits are irrelevant for Straight Flush 
	 * Aces can play low in straights and straight flushes: 5? 4? 3? 2? A? is a 5-high straight flush, also known as a "steel wheel".
	 * An ace-high straight flush such as A? K? Q? J? 10? is known as a royal flush, and is the highest ranking standard poker hand.
	 *
	 * For Pairs, 2-Pairs and Trios, the highest single cards (kickers) are used as a tiebreak, sucessively one by one
	*/

	static public class PokerHand
	{
		private const int N = PokerApp.SUIT_CARDS+1;		// 0 == no card
		private const int N1 = N;
		private const int N2 = N * N;
		private const int N3 = N * N * N;
		private const int N4 = N * N * N * N;
		private const int N5 = N * N * N * N * N;

		public const int HAND_CARDS = PokerApp.TAKEN_CARDS;


/*
		--------------------------------------------------------------
		| Hand			| Description			| Value				 |
		--------------------------------------------------------------
		Highest Card							type1*N^4 + type2*N^3 + type3*N^2 + type4*N + type5
		Pair									N^5 + Type*N^3 + kicker1*N^2 + kicker2*N + kicker3
		Two Pairs								N^5 + N^4 + Type1*N*N + Type2*N + kicker
		Trio									N^5 + N^4 + N^3 + Type*N^2 + kicker1*N + kicker2

		Straight		5 sucessive, diff suits N^5 + N^4 + 2*N^3 + highest Type
		Flush			5 increasing, same suit	N^5 + N^4 + 3*N^3 + type1*N^4 + type2*N^3 + type3*N^2 + type4*N + type5
 
		Full House		trio + pair				2*N^5 + N^4 + 3*N^3 + N * Trio CardType + Pair CardType
		Four of a Kind							2*N^5 + N^4 + 4*N^3 + type
		StraightFlush	5 sucessive, same suit  2*N^5 + N^4 + 5*N^3 + highest cardtype

		CardType = card w/o suit     (max = SUIT_CARDS)
*/

		public enum Hand: int
		{
			// max values for each hand
			HighCard	= N5,
			OnePair		= N5 + N4,
			TwoPairs	= N5 + N4 + 1*N3,
			OneTrio		= N5 + N4 + 2*N3,
			Straight	= N5 + N4 + 3*N3,
			Flushie		= 2*N5 + N4 + 3*N3,
			FullHaus	= 2*N5 + N4 + 4*N3,
			FourKind	= 2*N5 + N4 + 5*N3,
			StrFlush	= 2*N5 + N4 + 6*N3,
			ERROR
		}

		static int[] deck;

		private static ReserveComp qcmp = new ReserveComp();

		static public Hand getHand(int val)
		{
			if (val <= 0)
				IO.WriteLine("Invalid Hand from outer Space!! " + val);

			if (val <= (int) Hand.HighCard) return Hand.HighCard;
			if (val <= (int) Hand.OnePair)	return Hand.OnePair;
			if (val <= (int) Hand.TwoPairs)	return Hand.TwoPairs;
			if (val <= (int) Hand.OneTrio)	return Hand.OneTrio;
			if (val <= (int) Hand.Straight)	return Hand.Straight;
			if (val <= (int) Hand.Flushie)	return Hand.Flushie;
			if (val <= (int) Hand.FullHaus)	return Hand.FullHaus;
			if (val <= (int) Hand.FourKind)	return Hand.FourKind;
			if (val <= (int) Hand.StrFlush)	return Hand.StrFlush;
			
			IO.WriteLine("Unknown Hand from outer Space!! " + val);
			return Hand.ERROR;
		}


		static public int HandValue(int[] odeck)
		{
			PrepareCards(odeck);
			return getHandValue();
		}

		static public void PrepareCards(int[] odeck)
		{
			deck = odeck;
			Utils.BinaryInsertionSort(deck);	// faster by large
			//			Utils.SelectionSort(deck);
			//			Array.Sort(deck, qcmp);	// a LOT slower
		}

		static public int getHandValue()
		{
			int value, valueK;

			if ((value = checkStrFlush()) > 0)
				return value;
			
			// 4kind or fullhouse
			if ((valueK = checkKinds()) > (int) Hand.Flushie)
				return valueK;
				
			if ((value = checkFlush()) > 0)
				return value;
				
			if ((value = checkStraight()) > 0)
				return value;

			return valueK;
		}
		

/*		delegate bool CheckKickerDel(int val, int f1, int f2);
		CheckKickerDel checkKicker;
		bool CheckKicker0(int val, int f1, int f2) { return true; }
		bool CheckKicker1(int val, int f1, int f2) { return (val != f1); }
		bool CheckKicker2(int val, int f1, int f2) { return (val != f1 && val != f2); }

		//	factors only the 5 highest letters
		int getKickersValue(int num, int found1, int found2)
		{
			int val = 0;

			for (int i = 0; num != 0 && i < HAND_CARDS; i++)
				if (checkKicker(Card.idType(deck[i]), found1, found2))
					val += Card.idType(deck[i]) * Utils.pow(N, --num);

			return val;
		}
*/
		// extended
		static int getKickersValue0(int num)
		{
			int val = 0;
			for (int i = 0; num != 0 && i < HAND_CARDS; i++)
				val += Card.idType(deck[i]) * Utils.pow(N, --num);
			return val;
		}

		static int getKickersValue1(int num, int found1)
		{
			int val = 0;
			for (int i = 0; num != 0 && i < HAND_CARDS; i++)
				if (Card.idType(deck[i]) != found1)
					val += Card.idType(deck[i]) * Utils.pow(N, --num);
			return val;
		}

		static int getKickersValue2(int num, int found1, int found2)
		{
			int val = 0;
			for (int i = 0; num != 0 && i < HAND_CARDS; i++)
			{
				int tt = Card.idType(deck[i]);
				if (tt != found1 && tt != found2)
					val += Card.idType(deck[i]) * Utils.pow(N, --num);
			}
			return val;
		}




/*		Highest Card							type1*N^4 + type2*N^3 + type3*N^2 + type4*N + type5
		Pair									N^5 + Type*N^3 + kicker1*N^2 + kicker2*N + kicker3
		Two Pairs								N^5 + N^4 + Type1*N*N + Type2*N + kicker
		Trio									N^5 + N^4 + N^3 + Type*N^2 + kicker1*N + kicker2
		Full House		trio + pair				2*N^5 + N^4 + 3*N^3 + N * Trio CardType + Pair CardType
		Four of a Kind							2*N^5 + N^4 + 4*N^3 + type
*/
		static int checkKinds()
		{
			int i, idf, idt=-1, idp1=-1, idp2=-1;	// four, trio, pair1, pair2

			for (int j = 0; j <= HAND_CARDS-2; j += i)
			{
				idf = Card.idType(deck[j]);
				for (i = 1; i + j < HAND_CARDS; i++)
					if (idf != Card.idType(deck[i+j]))
						break;
				
				if (i == 4)		// four of a kind
					return 2*N5 + N4 + 4*N3 + idf;
				
				if (i == 3 && idt < 0)// trio
					idt = idf;

				else if (i > 1)	// single pair or pair from trio
				{
					if (idp1 < 0)
						idp1 = idf;
					else if (idp2 < 0)
						idp2 = idf;
				}
			}
			
			if (idt >= 0)
			{
				if (idp1 >= 0)	// fullhouse
					return 2 * N5 + N4 + 3 * N3 + N * idt + idp1;
	
				else			// trio
					return N5 + N4 + N3 + N2*idt + getKickersValue1(2, idt);
			}
			
			if (idp1 >= 0)
			{
				if (idp2 < 0)	// pair
					return N5 + N3 * idp1 + getKickersValue1(3, idp1);
				else			// two pairs
					return N5 + N4 + N2 * idp1 + N * idp2 + getKickersValue2(1, idp1, idp2);
			}

			return getKickersValue0(5);		// highest card
		}


		// 	StraightFlush	5 sucessive, same suit  2*N^5 + N^4 + 5*N^3 + highest cardtype
		static int checkStrFlush()
		{
			int i, id = 0, mcount = 0, suit = 0, HandCards = 5;
			int ttj = HAND_CARDS - HandCards + 1;
			
			for (int j = 0; j <= ttj; j++)
			{
				id = Card.idType(deck[j]);
				suit = Card.idSuit(deck[j]);
				mcount = HandCards-1;

				for (i = j + 1; mcount != 0 && i < HAND_CARDS && mcount <= HAND_CARDS - i +1; i++)
					if (id	== Card.idType(deck[i]) +1 && suit== Card.idSuit(deck[i]))
					{	mcount--;
						id = Card.idType(deck[i]);
					}
				
				if (mcount == 0)
					return 2 * N5 + N4 + 5 * N3 + Card.idType(deck[j]);
			}

			// check for low-Ace straight flush
			if (mcount == 1 && id == (int) CardType.Two
			&&	Card.idType(deck[0]) == (int)CardType.A
			&&	Card.idSuit(deck[0]) == suit)
				return 2 * N5 + N4 + 5 * N3 + Card.idType(deck[HAND_CARDS-HandCards]);
			
			return -1;
		}
		
		
        

        // Straight		5 sucessive, diff suits N^5 + N^4 + 2*N^3 + highest Type
		// the Aces may be higher or lower
		static int checkStraight()
		{
			int id = 0, j, mcount = 0, HandCards = 5;
			
			for (j = 0; j <= HAND_CARDS-HandCards+1; j++)
			{
				id = Card.idType(deck[j]);
				mcount = HandCards - 1;

				for (int i = j + 1; mcount != 0 && i < HAND_CARDS && mcount <= HAND_CARDS - i + 1; i++)
					if (id == Card.idType(deck[i]) +1)
					{	mcount--;
						id = Card.idType(deck[i]);
					}
				
				if (mcount == 0)
					return N5 + N4 + 2 * N3 + Card.idType(deck[j]);
			}


			// check for low-Ace straight
			if (mcount == 1 && id == (int) CardType.Two
			&&	Card.idType(deck[0]) == (int) CardType.A)
				return N5 + N4 + 2 * N3 + (int) Card.idType(deck[j-1]);
			
			return -1;
		}
		
        
		// 	Flush			5 increasing, same suit	N^5 + N^4 + 3*N^3 + type1*N^4 + type2*N^3 + type3*N^2 + type4*N + type5
		static int checkFlush()
		{
			int i;
			int HandCards = 5;
			int[] sol = new int[HandCards];
			
			for (int j = 0; j <= HAND_CARDS-HandCards; j++)
			{
				int count = 1;
				int id = sol[HandCards-count] = deck[j];
				
				for (i = j+1; count < HandCards && i < HAND_CARDS; i++)
					if (Card.idType(id) >  Card.idType(deck[i])
					&&	Card.idSuit(id) == Card.idSuit(deck[i]))
						id = sol[HandCards - ++count] = deck[i];
				
				if (count == HandCards)
				{
					int val = N5 + N4 + 3 * N3, exp = 4;

					for (int t = 0; t < HandCards && exp >= 0; t++)
						val += Card.idType(sol[t]) * Utils.pow(N, exp--);

					return val;
				}
			}
			
			return -1;
		}
	
	}
}

