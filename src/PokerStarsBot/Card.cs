using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace pokermaster
{
	public enum CardType : int
	{
		Two		= 2,
		Three	= 3,
		Four	= 4,
		Five	= 5,
		Six		= 6,
		Seven	= 7,
		Eight	= 8,
		Nine	= 9,
		T		= 10,
		J		= 11,
		Q		= 12,
		K		= 13,
		A		= 14
	}

	public enum CardSuit : int
	{
		c = 0,
		d = 1,
		h = 2,
		s = 3
	}

	// Ids from 1 to 52
	// Type from 2 to 14

	public class Card
	{
		public CardType type;
		public CardSuit suit;
		
		public Card(CardType t, CardSuit s) { type = t; suit = s;  }
		public Card(int suit, int type) : this((CardType)suit, (CardSuit)type) { }
		public Card(int id) : this((CardType) idType(id), (CardSuit) idSuit(id)) { }
		
		// convert the Card struct to a Card ID
		public		  int cardToId() { return cardToId(suit, type); }
		public static int cardToId(Card card) { return cardToId(card.suit, card.type); }
		public static int cardToId(int suit, int type) { return cardToId((CardSuit) suit, (CardType) type); }
		public static int cardToId(CardSuit suit, CardType type)
		{
			return PokerApp.TOTAL_SUITS * ((int)type-2) + (int)suit +1;
		}
		
		// convert the Card ID to a struct
		public static int idSuit(int id) { return (id-1) % PokerApp.TOTAL_SUITS; }
		public static int idType(int id) { return (id-1) / PokerApp.TOTAL_SUITS +2; }
		
		public override string ToString(	  ) { return ToString(type, suit); }
		public static	string ToString(int id) { return new Card(id).ToString(); }
		public static	string ToString(Card c) { return c.ToString(); }

		public static	string ToString(CardType type, CardSuit suit)
		{
			int t = (int)type;
			if (t < 10)
				return t.ToString() + suit.ToString();
			else
				return type.ToString() + suit.ToString();
		}


		public static int parseCard(string str)
		{
			string c = str[str.Length-1].ToString().ToLower();
			string t = str.Substring(0, str.Length-1).ToUpper();

			if (!Enum.IsDefined(typeof(CardSuit), c))
			{
				c = str[0].ToString().ToLower();
				t = str.Substring(1, str.Length-0).ToUpper();

				if (!Enum.IsDefined(typeof(CardSuit), c))
					return 0;
			}

			try {
				CardSuit suit = (CardSuit) Enum.Parse(typeof(CardSuit), c, true);

				if (t == "T")
					return cardToId((int)suit, 10);

				if (!Enum.IsDefined(typeof(CardType), t)
				&&	!Enum.IsDefined(typeof(CardType),
					   Enum.GetName(typeof(CardType), Int32.Parse(t))))
					return 0;
			
				CardType type = (CardType) Enum.Parse(typeof(CardType), t, true);
				return cardToId(suit, type);
			}
			catch (Exception) {
				return 0;
			}
		}

	}

}


//	MAYBE TODO
		/*
		 * Input format for cards
		 * 
		 * suits:
		 *		ouros	diamond	- o d
		 *		copas	hearts	- h co
		 *		paus	clubs	- p cl
		 *		spades	espadas	- s e
		 *	
		 * court:
		 *		jack knave valete	- j n v
		 *		queen rainha		- q ra
		 *		king rei			- k re

		static readonly string[] diamons= { "o", "d"  };
		static readonly string[] hearts	= { "h", "do" };
		static readonly string[] clubs	= { "p", "cl" };
		static readonly string[] spades	= { "s", "e"  };

		static readonly string[] knave	= { "j", "n", "v" };
		static readonly string[] queen	= { "q", "ra" };
		static readonly string[] king	= { "k", "re" };
*/
