// Much of this code is derived from poker.eval (look for it on sourceforge.net).
// This library is covered by the LGPL Gnu license. See http://www.gnu.org/copyleft/lesser.html 
// for more information on this license.

// This code is a very fast, native C# Texas Holdem mask evaluator (containing no interop or unsafe code). 
// This code can enumarate 35 million 5 card hands per second and 29 million 7 card hands per second on my desktop machine.
// That's not nearly as fast as the heavily macro-ed poker.eval C library. However, this implementation is
// in roughly the same ballpark for speed and is quite usable in C#.

// The speed ups are mostly table driven. That means that there are several very large tables included in this file. 
// The code is divided up into several files they are:
//      HandEvaluator.cs - base mask evaluator
//      HandIterator.cs - methods that support IEnumerable and methods that validate the mask evaluator
//      HandAnalysis.cs - methods to aid in analysis of Texas Holdem Hands.
//      PocketHands.cs - a class to manipulate pocket hands.
//      PocketQueryParser - a parser used to interprete pocket mask query language statements.
//                          The runtime portion of the query parser is copyrighted by Malcolm Crowe (but is freely distributable)

// Written (ported) by Keith Rule - Sept 2005, updated May 2007

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tools;

namespace HoldemHand
{
    /// <summary>
    /// Represents a set of pocket hands and operations that
    /// can be applied to them.
    /// </summary>
    public partial class PocketHands 
    {
        /// <summary>
        /// This function allows a text description (or query) of set 
        /// of pocket hands to be specified such that all of the hands that match
        /// this query (minus the hands containing any of the cards in the 
        /// dead mask) will be returned.
        /// </summary>
        /// <param name="dead">card mask for dead cards</param>
        /// <param name="s">string defining PocketHands query</param>
        /// <returns></returns>
        static public PocketHands Query(string s, ulong dead)
        {
            return Query(s) - dead;
        }

        
       /// <summary>
       /// 
       /// </summary>
       /// <param name="s"></param>
       /// <param name="dead"></param>
       /// <returns></returns>
        static public bool ValidateQuery(string s, ulong dead)
        {
            try
            {
                return Query(s, dead).Count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public bool ValidateQuery(string s)
        {
            return ValidateQuery(s, 0UL);
        }

        /// <summary>
        /// This function allows a text description (or query) of set 
        /// of pocket hands to be specified such that all of the hands that match
        /// this query will be returned.
        /// </summary>
        /// <param name="s">Query String</param>
        /// <returns></returns>
        static public PocketHands Query(string s)
        {
            try
            {
                syntax p = new syntax();
                // For debugging the parser
                //p.m_debug = true;
                object obj = p.Parse(s);
                return (PocketHands)((Expr)((SpecDoc)obj).yylval).yylval;
            }
            catch
            {
                throw new ArgumentException("Syntax Error");
            }
        }
    }
}
