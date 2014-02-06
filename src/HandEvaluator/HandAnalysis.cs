// Much of this code is derived from poker.eval (look for it on sourceforge.net).
// This library is covered by the GPL Gnu license. See http://www.gnu.org/licenses/old-licenses/gpl-2.0.html 
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
using System.Diagnostics;

namespace HoldemHand
{
    public partial class Hand : IComparable
    {
        #region Analysis Functions

        #region DefaultTimeDuration
        /// <exclude/>
        private const double DefaultTimeDuration = 0.25;
        #endregion

        #region HandStrength
        /// <summary>
        /// The classic HandStrengh Calculation from page 21 of Aaron Davidson's
        /// Masters Thesis.
        /// </summary>
        /// <param name="pocket">Pocket cards</param>
        /// <param name="board">Current Board</param>
        /// <returns>Hand strength as a percentage of hands won.</returns>
        public static double HandStrength(ulong pocket, ulong board)
        {
            double win = 0.0, count = 0.0;
#if DEBUG
            if (BitCount(pocket) != 2) 
                throw new ArgumentException("pocket must have exactly two cards");
            if (BitCount(board) < 3 || BitCount(board) > 5)
                throw new ArgumentException("board must have 3, 4 or 5 cards for this calculation");
#endif
            uint ourrank = Evaluate(pocket | board);
            foreach (ulong oppcards in Hand.Hands(0UL, pocket | board, 2))
            {
                uint opprank = Evaluate(oppcards | board);
                if (ourrank > opprank)
                {
                    win += 1.0;
                }
                else if (ourrank == opprank)
                {
                    win += 0.5;
                }
                count += 1.0;
            }
            return win / count;
        }

        /// <summary>
        /// A HandStrength function that accounts for multiple opponents. This method uses the
        /// Monte Carlo menthod to calculate a value. It takes surprisingly little time to get
        /// good results. A duration of 0.01 seconds (10mS) returns good results, and a duration of 0.1 (100mS) returns
        /// even better results.
        /// </summary>
        /// <param name="pocketquery">pocket mask query</param>
        /// <param name="board">current board</param>
        /// <param name="NOpponnents">the number of opponents (must be 1-9)</param>
        /// <param name="duration">time in seconds to perform this calculation</param>
        /// <returns>returns percentage of hands won</returns>
        public static double HandStrength(string pocketquery, string board, int NOpponnents, double duration)
        {
            double win = 0.0, count = 0.0;
            double starttime = CurrentTime;
            ulong[] list = PocketHands.Query(pocketquery);
            ulong boardmask = ParseHand(board);
            Random rand = new Random();
 
#if DEBUG
            if (!PocketHands.ValidateQuery(pocketquery, Hand.ParseHand(board)))
                throw new ArgumentException("pocketquery and/or board is invalid");
            if (BitCount(boardmask) > 5)
                throw new ArgumentException("board must have 5 or less cards");
            if (NOpponnents < 1 || NOpponnents > 9)
                throw new ArgumentException("may only select 1-9 opponents");
#endif
            
            switch (NOpponnents)
            {
                case 1:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, 0UL, 2);
                        uint ourrank = Evaluate(pocketmask | boardmask);
                        ulong oppcards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask, 2);
                        uint opprank = Evaluate(oppcards | boardmask);
                        if (ourrank > opprank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank == opprank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;
                    }
                    break;
                case 2:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, 0UL, 2);
                        uint ourrank = Evaluate(pocketmask | boardmask);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards, 2);
                        uint opp1rank = Evaluate(opp1cards | boardmask);
                        uint opp2rank = Evaluate(opp2cards | boardmask);
                       
                        if (ourrank > opp1rank && ourrank > opp2rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0; 

                    }
                    break;
                case 3:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, 0UL, 2);
                        uint ourrank = Evaluate(pocketmask | boardmask);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards, 2);
                        uint opp1rank = Evaluate(opp1cards | boardmask);
                        uint opp2rank = Evaluate(opp2cards | boardmask);
                        uint opp3rank = Evaluate(opp3cards | boardmask);

                        if (ourrank > opp1rank && ourrank > opp2rank && ourrank > opp3rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank && ourrank >= opp3rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0; 

                    }
                    break;
                case 4:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, 0UL, 2);
                        uint ourrank = Evaluate(pocketmask | boardmask);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards, 2);
                        uint opp1rank = Evaluate(opp1cards | boardmask);
                        uint opp2rank = Evaluate(opp2cards | boardmask);
                        uint opp3rank = Evaluate(opp3cards | boardmask);
                        uint opp4rank = Evaluate(opp4cards | boardmask);

                        if (ourrank > opp1rank && ourrank > opp2rank && 
                            ourrank > opp3rank && ourrank > opp4rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank && 
                            ourrank >= opp3rank && ourrank >= opp4rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;

                    }
                    break;
                case 5:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, 0UL, 2);
                        uint ourrank = Evaluate(pocketmask | boardmask);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        uint opp1rank = Evaluate(opp1cards | boardmask);
                        uint opp2rank = Evaluate(opp2cards | boardmask);
                        uint opp3rank = Evaluate(opp3cards | boardmask);
                        uint opp4rank = Evaluate(opp4cards | boardmask);
                        uint opp5rank = Evaluate(opp5cards | boardmask);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;

                    }
                    break;
                case 6:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, 0UL, 2);
                        uint ourrank = Evaluate(pocketmask | boardmask);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        uint opp1rank = Evaluate(opp1cards | boardmask);
                        uint opp2rank = Evaluate(opp2cards | boardmask);
                        uint opp3rank = Evaluate(opp3cards | boardmask);
                        uint opp4rank = Evaluate(opp4cards | boardmask);
                        uint opp5rank = Evaluate(opp5cards | boardmask);
                        uint opp6rank = Evaluate(opp6cards | boardmask);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;
                    }
                    break;
                case 7:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, 0UL, 2);
                        uint ourrank = Evaluate(pocketmask | boardmask);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        uint opp1rank = Evaluate(opp1cards | boardmask);
                        uint opp2rank = Evaluate(opp2cards | boardmask);
                        uint opp3rank = Evaluate(opp3cards | boardmask);
                        uint opp4rank = Evaluate(opp4cards | boardmask);
                        uint opp5rank = Evaluate(opp5cards | boardmask);
                        uint opp6rank = Evaluate(opp6cards | boardmask);
                        uint opp7rank = Evaluate(opp7cards | boardmask);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;
                    }
                    break;
                case 8:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, 0UL, 2);
                        uint ourrank = Evaluate(pocketmask | boardmask);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        ulong opp8cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards, 2);
                        uint opp1rank = Evaluate(opp1cards | boardmask);
                        uint opp2rank = Evaluate(opp2cards | boardmask);
                        uint opp3rank = Evaluate(opp3cards | boardmask);
                        uint opp4rank = Evaluate(opp4cards | boardmask);
                        uint opp5rank = Evaluate(opp5cards | boardmask);
                        uint opp6rank = Evaluate(opp6cards | boardmask);
                        uint opp7rank = Evaluate(opp7cards | boardmask);
                        uint opp8rank = Evaluate(opp8cards | boardmask);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank && ourrank > opp8rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank && ourrank >= opp8rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;
                    }
                    break;
                case 9:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, 0UL, 2);
                        uint ourrank = Evaluate(pocketmask | boardmask);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        ulong opp8cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards, 2);
                        ulong opp9cards = Hand.RandomHand(rand, 0UL, pocketmask | boardmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards | opp8cards, 2);
                        uint opp1rank = Evaluate(opp1cards | boardmask);
                        uint opp2rank = Evaluate(opp2cards | boardmask);
                        uint opp3rank = Evaluate(opp3cards | boardmask);
                        uint opp4rank = Evaluate(opp4cards | boardmask);
                        uint opp5rank = Evaluate(opp5cards | boardmask);
                        uint opp6rank = Evaluate(opp6cards | boardmask);
                        uint opp7rank = Evaluate(opp7cards | boardmask);
                        uint opp8rank = Evaluate(opp8cards | boardmask);
                        uint opp9rank = Evaluate(opp9cards | boardmask);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank && ourrank > opp8rank && 
                            ourrank > opp9rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank && ourrank >= opp8rank &&
                            ourrank >= opp9rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;
                    }
                    break;
            }
           
            return win / count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pocket"></param>
        /// <param name="board"></param>
        /// <param name="NOpponnents"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static double HandStrength(ulong pocket, ulong board, int NOpponnents, double duration)
        {
            double win = 0.0, count = 0.0;
            double starttime = CurrentTime;
            Random rand = new Random();
#if DEBUG
            if (BitCount(pocket) != 2)
                throw new ArgumentException("pocket must have exactly two cards");
            if (BitCount(board) > 5)
                throw new ArgumentException("board must have 5 or less cards");
            if (NOpponnents < 1 || NOpponnents > 9)
                throw new ArgumentException("may only select 1-9 opponents");
#endif
            uint ourrank = Evaluate(pocket | board);

            switch (NOpponnents)
            {
                case 1:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong oppcards = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        uint opprank = Evaluate(oppcards | board);
                        if (ourrank > opprank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank == opprank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;
                    }
                    break;
                case 2:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards, 2);
                        uint opp1rank = Evaluate(opp1cards | board);
                        uint opp2rank = Evaluate(opp2cards | board);

                        if (ourrank > opp1rank && ourrank > opp2rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;

                    }
                    break;
                case 3:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards, 2);
                        uint opp1rank = Evaluate(opp1cards | board);
                        uint opp2rank = Evaluate(opp2cards | board);
                        uint opp3rank = Evaluate(opp3cards | board);

                        if (ourrank > opp1rank && ourrank > opp2rank && ourrank > opp3rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank && ourrank >= opp3rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;

                    }
                    break;
                case 4:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards, 2);
                        uint opp1rank = Evaluate(opp1cards | board);
                        uint opp2rank = Evaluate(opp2cards | board);
                        uint opp3rank = Evaluate(opp3cards | board);
                        uint opp4rank = Evaluate(opp4cards | board);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;

                    }
                    break;
                case 5:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        uint opp1rank = Evaluate(opp1cards | board);
                        uint opp2rank = Evaluate(opp2cards | board);
                        uint opp3rank = Evaluate(opp3cards | board);
                        uint opp4rank = Evaluate(opp4cards | board);
                        uint opp5rank = Evaluate(opp5cards | board);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;

                    }
                    break;
                case 6:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        uint opp1rank = Evaluate(opp1cards | board);
                        uint opp2rank = Evaluate(opp2cards | board);
                        uint opp3rank = Evaluate(opp3cards | board);
                        uint opp4rank = Evaluate(opp4cards | board);
                        uint opp5rank = Evaluate(opp5cards | board);
                        uint opp6rank = Evaluate(opp6cards | board);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;
                    }
                    break;
                case 7:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        uint opp1rank = Evaluate(opp1cards | board);
                        uint opp2rank = Evaluate(opp2cards | board);
                        uint opp3rank = Evaluate(opp3cards | board);
                        uint opp4rank = Evaluate(opp4cards | board);
                        uint opp5rank = Evaluate(opp5cards | board);
                        uint opp6rank = Evaluate(opp6cards | board);
                        uint opp7rank = Evaluate(opp7cards | board);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;
                    }
                    break;
                case 8:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        ulong opp8cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards, 2);
                        uint opp1rank = Evaluate(opp1cards | board);
                        uint opp2rank = Evaluate(opp2cards | board);
                        uint opp3rank = Evaluate(opp3cards | board);
                        uint opp4rank = Evaluate(opp4cards | board);
                        uint opp5rank = Evaluate(opp5cards | board);
                        uint opp6rank = Evaluate(opp6cards | board);
                        uint opp7rank = Evaluate(opp7cards | board);
                        uint opp8rank = Evaluate(opp8cards | board);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank && ourrank > opp8rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank && ourrank >= opp8rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;
                    }
                    break;
                case 9:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        ulong opp8cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards, 2);
                        ulong opp9cards = Hand.RandomHand(rand, 0UL, pocket | board | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards | opp8cards, 2);
                        uint opp1rank = Evaluate(opp1cards | board);
                        uint opp2rank = Evaluate(opp2cards | board);
                        uint opp3rank = Evaluate(opp3cards | board);
                        uint opp4rank = Evaluate(opp4cards | board);
                        uint opp5rank = Evaluate(opp5cards | board);
                        uint opp6rank = Evaluate(opp6cards | board);
                        uint opp7rank = Evaluate(opp7cards | board);
                        uint opp8rank = Evaluate(opp8cards | board);
                        uint opp9rank = Evaluate(opp9cards | board);

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank && ourrank > opp8rank &&
                            ourrank > opp9rank)
                        {
                            win += 1.0;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank && ourrank >= opp8rank &&
                            ourrank >= opp9rank)
                        {
                            win += 0.5;
                        }
                        count += 1.0;
                    }
                    break;
            }

            return win / count;
        }
        #endregion

        #region Outs/Draws

        /// <summary>
        /// The method returns the number of straight draws that are possible for the current mask.      
        /// </summary>
        /// <param name="mask">Current Hand</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>The number of straight draws for this mask type.</returns>
        public static int StraightDrawCount(ulong mask, ulong dead)
        {
            int retval = 0;

            // Get original mask value
            Hand.HandTypes origtype = Hand.EvaluateType(mask);

            // If current mask better than a straight return then 0 outs
            if (origtype >= Hand.HandTypes.Straight)
                return retval;

            // Look ahead one card
            foreach (ulong card in Hand.Hands(0UL, mask | dead, 1))
            {
                // Get new mask value
                Hand.HandTypes newHandType = Hand.EvaluateType(mask | card);

                // Include straight flush as this will ensure outs is always the maximum
                if (newHandType == Hand.HandTypes.Straight || newHandType == Hand.HandTypes.StraightFlush)
                {
                    retval++;
                }
            }

            return retval;
        }

        /// <summary>
        /// The method returns the number of straight draws that are possible for the player and board configuration.   
        /// This method filters the results so only player hand improvements are counted.
        /// </summary>
        /// <param name="player">Two card mask making up the players pocket cards</param>
        /// <param name="board">The community cards</param>  
        /// <param name="dead">Dead cards</param>  
        /// <returns>The number of straight draws for this mask type.</returns>
        public static int StraightDrawCount(ulong player, ulong board, ulong dead)
        {
            int retval = 0;
            int ncards = BitCount(player | board);
#if DEBUG
            if (BitCount(player) != 2) throw new ArgumentException("player must have exactly 2 cards");
            if (BitCount(board) != 3 && BitCount(board) != 4) throw new ArgumentException("board must contain 3 or 4 cards");
#endif
            // Get original mask value
            uint playerOrigHandVal = Hand.Evaluate(player | board, ncards);

            // If current mask better than a straight return then 0 outs
            if (Hand.HandType(playerOrigHandVal) >= (uint)Hand.HandTypes.Straight)
                return retval;

            // Look ahead one card
            foreach (ulong card in Hand.Hands(0UL, board | player | dead, 1))
            {
                // Get new mask value
                uint playerNewHandVal = Hand.Evaluate(player | board | card, ncards+1);
                uint playerHandType = Hand.HandType(playerNewHandVal);
                
                // Include straight flush as this will ensure outs is always the maximum
                if (playerHandType == (uint)Hand.HandTypes.Straight ||
                    playerHandType == (uint)Hand.HandTypes.StraightFlush)
                {
                    // Get new board value
                    uint boardHandVal = Hand.Evaluate(board | card);

                    // If the mask improved, increment out
                    if (Hand.HandType(playerNewHandVal) > Hand.HandType(boardHandVal) ||
                        (Hand.HandType(playerNewHandVal) == Hand.HandType(boardHandVal) && 
                         Hand.TopCard(playerNewHandVal) > Hand.TopCard(boardHandVal)))
                    {
                        retval++;
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// Returns true if the combined mask is an open ended straight draw.
        /// </summary>
        /// <param name="mask">Players pocket cards mask</param>
        /// <param name="dead">Community card mask</param>
        /// <returns>Returns true if the combined mask is an open ended straight draw</returns>
        public static bool IsOpenEndedStraightDraw(ulong mask, ulong dead)
        {
#if DEBUG
            if ((mask & dead) != 0) throw new ArgumentException("mask and dead cards must not have any cards in common");
            if (BitCount(mask) < 4 || BitCount(mask) > 6) throw new ArgumentException("mask must have 4-6 cards");
#endif
            return StraightDrawCount(mask, 0UL) > 4 && StraightDrawCount(mask, dead) > 0;
        }

        /// <summary>
        /// Returns true if the mask is an open ended straight draw.
        /// </summary>
        /// <param name="mask">Players pocket cards mask</param>
        /// <param name="dead">Community card mask</param>
        /// <returns>Returns true if the combined mask is an open ended straight draw</returns>
        public static bool IsOpenEndedStraightDraw(string mask, string dead)
        {
            return IsOpenEndedStraightDraw(Hand.ParseHand(mask), Hand.ParseHand(dead));
        }

        /// <summary>
        /// Returns true if the combined mask is an open ended straight draw. Only straight possiblities that
        /// improve the player's mask are considered in this method.
        /// </summary>
        /// <param name="pocket">Players pocket cards mask</param>
        /// <param name="board">Community card mask</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true if the combined mask is an open ended straight draw</returns>
        public static bool IsOpenEndedStraightDraw(ulong pocket, ulong board, ulong dead)
        {
#if DEBUG
            if (BitCount(pocket) != 2)
                throw new ArgumentException("pocket must have exactly two cards");
            if (BitCount(board) != 3 && BitCount(board) != 4)
                throw new ArgumentException("board must have 3 or 4 cards for this calculation");
#endif
            return IsOpenEndedStraightDraw(pocket | board, 0UL) && StraightDrawCount(pocket, board, dead) > 0;
        }

        /// <summary>
        /// Return true if the combined cards contain a gut shot straight.
        /// </summary>
        /// <param name="pocket">Players pocket cards mask</param>
        /// <param name="board">Community card mask</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true of the combined mask is a gut shot straight draw</returns>
        public static bool IsGutShotStraightDraw(ulong pocket, ulong board, ulong dead)
        {
#if DEBUG
            if (BitCount(pocket) != 2)
                throw new ArgumentException("pocket must have exactly two cards");
            if (BitCount(board) != 3 && BitCount(board) != 4)
                throw new ArgumentException("board must have 3 or 4 cards for this calculation");
#endif
            return IsGutShotStraightDraw(pocket | board, dead) && StraightDrawCount(pocket, board, dead) > 0;
        }

        /// <summary>
        /// Return true if the combined cards contain a gut shot straight.
        /// </summary>
        /// <param name="mask">Current mask</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true of the combined mask is a gut shot straight draw</returns>
        public static bool IsGutShotStraightDraw(ulong mask, ulong dead)
        {
#if DEBUG
            if ((mask & dead) != 0) throw new ArgumentException("mask and dead cards must not have any cards in common");
            if (BitCount(mask) < 4 || BitCount(mask) > 6) throw new ArgumentException("mask must have 4-6 cards");
#endif
            return StraightDrawCount(mask, 0UL) <= 4 && StraightDrawCount(mask, dead) > 0;
        }

        /// <summary>
        /// Return true if the combined cards contain a gut shot straight.
        /// </summary>
        /// <param name="mask">Current mask</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true of the combined mask is a gut shot straight draw</returns>
        public static bool IsGutShotStraightDraw(string mask, string dead)
        {
            return IsGutShotStraightDraw(Hand.ParseHand(mask), Hand.ParseHand(dead));
        }

        /// <summary>
        /// Returns true if the passed mask only needs one card to make a straight.
        /// Note that the pocket cards must contains at least one card in the 
        /// combined straight.
        /// </summary>
        /// <param name="pocket">Players pocket mask</param>
        /// <param name="board">Community board</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true if mask has a straight draw.</returns>
        public static bool IsStraightDraw(ulong pocket, ulong board, ulong dead)
        {
            return StraightDrawCount(pocket, board, dead) > 0;
        }

        /// <summary>
        /// Returns true if the passed mask only needs one card to make a straight.
        /// Note that the pocket cards must contains at least one card in the 
        /// combined straight.
        /// </summary>
        /// <param name="mask">Current Hand</param>
        /// <param name="dead">Dead Cards</param>
        /// <returns>Returns true if mask has a straight draw.</returns>
        public static bool IsStraightDraw(ulong mask, ulong dead)
        {
            return StraightDrawCount(mask, dead) > 0;
        }

        /// <summary>
        /// Returns true if the passed mask only needs one card to make a straight.
        /// Note that the pocket cards must contains at least one card in the 
        /// combined straight.
        /// </summary>
        /// <param name="mask">Current Hand</param>
        /// <param name="dead">Dead Cards</param>
        /// <returns>Returns true if mask has a straight draw.</returns>
        public static bool IsStraightDraw(string mask, string dead)
        {
            return IsStraightDraw(Hand.ParseHand(mask), Hand.ParseHand(dead));
        }

        /// <summary>
        /// Returns true if the passed mask only needs one card to make a straight.
        /// </summary>
        /// <param name="pocket">Players pocket mask</param>
        /// <param name="board">Community board</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true if mask has a straight draw.</returns>
        public static bool IsStraightDraw(string pocket, string board, string dead)
        {
#if DEBUG
            if (!Hand.ValidateHand(pocket)) throw new ArgumentException("pocket fails to Validate");
            if (!Hand.ValidateHand(board)) throw new ArgumentException("board fails to Validate");
#endif
            return IsStraightDraw(Hand.ParseHand(pocket), Hand.ParseHand(board), Hand.ParseHand(dead));
        }

        /// <summary>
        /// Returns the count of adjacent cards
        /// </summary>
        /// <param name="pocket">Players pocket cards mask</param>
        /// <param name="board">Community card mask</param>
        /// <returns>Return the number of contigous cards</returns>
        public static int CountContiguous(ulong pocket, ulong board)
        {
            ulong mask = pocket | board;
#if DEBUG
            if (BitCount(pocket) != 2)
                throw new ArgumentException("pocket must have exactly two cards");
            if (BitCount(board) != 3 && BitCount(board) != 4)
                throw new ArgumentException("board must have 3 or 4 cards for this calculation");

            uint bf = CardMask(mask, Clubs) | CardMask(mask, Diamonds) | 
                CardMask(mask, Hearts) | CardMask(mask, Spades);

            uint[] masks = {0x7fU, 0x3fU, 0x1fU, 0xfU, 0x7U, 0x3U};

            for (int i = 0; i < masks.Length; i++)
            {
                int count = BitCount(masks[i]);
                uint contmask = 0U;
                for (int offset = 13 - count; offset >= 0; offset--)
                {
                    contmask = masks[i] << offset;
                    if ((bf & contmask) == contmask)
                        return count;
                }

                contmask = 0x1000U | (masks[i] >> 1);
                if ((bf & contmask) == contmask)
                    return count;
            }

            return 0;
#else
            return contiguousCountTable[CardMask(mask, Clubs) | CardMask(mask, Diamonds) | CardMask(mask, Hearts) | CardMask(mask, Spades)];
#endif
        }

        /// <summary>
        /// Returns the count of adjacent cards
        /// </summary>
        /// <param name="mask">Current Hand</param>
        /// <returns>Return the number of contigous cards</returns>
        public static int CountContiguous(ulong mask)
        {
            return contiguousCountTable[CardMask(mask, Clubs) | CardMask(mask, Diamonds) | CardMask(mask, Hearts) |  CardMask(mask, Spades)];
        }

        /// <summary>
        /// Return true if the combined cards contain a gut shot straight.
        /// </summary>
        /// <param name="pocket">Players pocket cards</param>
        /// <param name="board">Community cards</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true of the combined mask is a gut shot straight draw</returns>
        public static bool IsGutShotStraightDraw(string pocket, string board, string dead)
        {
#if DEBUG
            if (!Hand.ValidateHand(pocket)) throw new ArgumentException("pocket fails to Validate");
            if (!Hand.ValidateHand(board)) throw new ArgumentException("board fails to Validate");
#endif
            return IsGutShotStraightDraw(Hand.ParseHand(pocket), Hand.ParseHand(board), Hand.ParseHand(dead));
        }

        /// <summary>
        /// Returns true if the combined mask is an open ended straight draw.
        /// </summary>
        /// <param name="pocket">Players pocket cards</param>
        /// <param name="board">Community cards</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true if the combined mask is an open ended straight draw</returns>
        public static bool IsOpenEndedStraightDraw(string pocket, string board, string dead)
        {
#if DEBUG
            if (!Hand.ValidateHand(pocket)) throw new ArgumentException("pocket fails to Validate");
            if (!Hand.ValidateHand(board)) throw new ArgumentException("board fails to Validate");
#endif
            return IsOpenEndedStraightDraw(Hand.ParseHand(pocket), Hand.ParseHand(board), Hand.ParseHand(dead));
        }

        /// <summary>
        /// Counts the number of hands that are a flush with one more drawn card.
        /// </summary>
        /// <param name="mask">Hand</param>
        /// <param name="dead">Cards not allowed to be drawn</param>
        /// <returns></returns>
        public static int FlushDrawCount(ulong mask, ulong dead)
        {
            int retval = 0;
      
            // Get original mask value
            Hand.HandTypes handtype = Hand.EvaluateType(mask);

            // If current mask better than a straight return then 0 outs
            if (handtype >= Hand.HandTypes.Flush)
                return retval;

            // Look ahead one card
            foreach (ulong card in Hand.Hands(0UL, mask | dead, 1))
            {
                // Get new mask value
                Hand.HandTypes handType = Hand.EvaluateType(mask | card);
           
                // Include straight flush as this will ensure outs is always the maximum
                if (handType == Hand.HandTypes.Flush || handType == Hand.HandTypes.StraightFlush)
                {
                    retval++;
                }
            }

            return retval;
        }

        /// <summary>
        /// Counts the number of hands that are a flush with one more drawn card. However, 
        /// Flush hands that only improve the board are not considered.
        /// </summary>
        /// <param name="player">Players two card hand</param>
        /// <param name="board">Board cards</param>
        /// <param name="dead">Dead cards</param>
        /// <returns></returns>
        public static int FlushDrawCount(ulong player, ulong board, ulong dead)
        {
            int retval = 0;
#if DEBUG
            if (BitCount(player) != 2) throw new ArgumentException("player must have exactly 2 cards");
            if (BitCount(board) != 3 && BitCount(board) != 4) throw new ArgumentException("board must contain 3 or 4 cards");
#endif
            // Get original mask value
            Hand.HandTypes playerOrigHandType = Hand.EvaluateType(player | board);

            // If current mask better than a straight return then 0 outs
            if (playerOrigHandType == Hand.HandTypes.Flush || 
                playerOrigHandType == Hand.HandTypes.StraightFlush)
                return retval;

            // Look ahead one card
            foreach (ulong card in Hand.Hands(0UL, board | player | dead, 1))
            {
                // Get new mask value
                uint playerNewHandValue = Hand.Evaluate(player | board | card);
                uint boardNewHandValue = Hand.Evaluate(board | card);

                // Include straight flush as this will ensure outs is always the maximum
                if (HandType(playerNewHandValue) == (uint)Hand.HandTypes.Flush ||
                        HandType(playerNewHandValue) == (uint)Hand.HandTypes.StraightFlush)
                {
                    // If the mask improved, increment out
                    if (HandType(playerNewHandValue) > HandType(boardNewHandValue) ||
                        (HandType(playerNewHandValue) == HandType(boardNewHandValue) && TopCard(playerNewHandValue) > TopCard(boardNewHandValue)))
                    {
                        retval++;
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// Returns true if there are 4 cards of the same suit.
        /// </summary>
        /// <param name="pocket">Players pocket cards mask</param>
        /// <param name="board">Community card mask</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true if there are 4 cards of the same suit</returns>
        public static bool IsFlushDraw(ulong pocket, ulong board, ulong dead)
        {
#if DEBUG
            if (BitCount(pocket) != 2)
                throw new ArgumentException("pocket must have exactly two cards");
            if (BitCount(board) != 3 && BitCount(board) != 4)
                throw new ArgumentException("board must have 3 or 4 cards for this calculation");
#endif
            return FlushDrawCount(pocket, board, dead) > 0;
        }

        /// <summary>
        /// Returns true if the hand is a flush draw.
        /// </summary>
        /// <param name="mask">Cards</param>
        /// <param name="dead">Dead cards</param>
        /// <returns></returns>
        public static bool IsFlushDraw(ulong mask, ulong dead)
        {
            return FlushDrawCount(mask, dead) > 0;
        }

        /// <summary>
        /// Returns true if there are 4 cards of the same suit.
        /// </summary>
        /// <param name="pocket">Players pocket cards</param>
        /// <param name="board">Community cards</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true if there are 4 cards of the same suit</returns>
        public static bool IsFlushDraw(string pocket, string board, string dead)
        {
#if DEBUG
            if (!Hand.ValidateHand(pocket)) throw new ArgumentException("pocket fails to Validate");
            if (!Hand.ValidateHand(board)) throw new ArgumentException("board fails to Validate");
#endif
            return FlushDrawCount(Hand.ParseHand(pocket), Hand.ParseHand(board), Hand.ParseHand(dead)) > 0;
        }

        /// <summary>
        /// Returns true if there are three cards of the same suit. 
        /// The pocket cards must have at least one card in that suit.
        /// </summary>
        /// <param name="pocket">Players pocket cards mask</param>
        /// <param name="board">Community card mask</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true if there are three cards of the same suit</returns>
        public static bool IsBackdoorFlushDraw(ulong pocket, ulong board, ulong dead)
        {
#if DEBUG
            if (BitCount(pocket) != 2)
                throw new ArgumentException("pocket must have exactly two cards");
            if (BitCount(board) != 3 && BitCount(board) != 4)
                throw new ArgumentException("board must have 3 or 4 cards for this calculation");
#endif
            ulong mask = pocket | board;

            Hand.HandTypes currentType = EvaluateType(pocket | board);
            if (currentType >= Hand.HandTypes.Flush) return false;

            uint ss = (uint)((mask >> (SPADE_OFFSET)) & 0x1fffUL);
            uint sc = (uint)((mask >> (CLUB_OFFSET)) & 0x1fffUL);
            uint sd = (uint)((mask >> (DIAMOND_OFFSET)) & 0x1fffUL);
            uint sh = (uint)((mask >> (HEART_OFFSET)) & 0x1fffUL);

            if (BitCount(ss) == 3)
            {
                uint ps = (uint)((pocket >> (SPADE_OFFSET)) & 0x1fffUL);
                return ps != 0;
            }
            else if (BitCount(sc) == 3)
            {
                uint pc = (uint)((pocket >> (CLUB_OFFSET)) & 0x1fffUL);
                return pc != 0;
            }
            else if (BitCount(sd) == 3)
            {
                uint pd = (uint)((pocket >> (DIAMOND_OFFSET)) & 0x1fffUL);
                return pd != 0;
            }
            else if (BitCount(sh) == 3)
            {
                uint ph = (uint)((pocket >> (HEART_OFFSET)) & 0x1fffUL);
                return ph != 0;
            }

            return false;
        }

        /// <summary>
        /// Returns true if there are three cards of the same suit. 
        /// The pocket cards must have at least one card in that suit.
        /// </summary>
        /// <param name="pocket">Players pocket cards</param>
        /// <param name="board">Community cards</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>Returns true if there are three cards of the same suit</returns>
        public static bool IsBackdoorFlushDraw(string pocket, string board, string dead)
        {
#if DEBUG
            if (!Hand.ValidateHand(pocket)) throw new ArgumentException("pocket fails to Validate");
            if (!Hand.ValidateHand(board)) throw new ArgumentException("board fails to Validate");
#endif
            return IsBackdoorFlushDraw(Hand.ParseHand(pocket), Hand.ParseHand(board), Hand.ParseHand(dead));
        }

        /// <summary>
        /// The method returns the number of draws that are possible for the
        /// specified HandType. This method only returns the counts that improve the 
        /// player's mask rather than just the board. Because of this subtle distinction,
        /// DrawCount(player, board, dead, type) doesn't necessarily return the same value as
        /// DrawCount(player | board, dead, type).
        /// </summary>
        /// <param name="player">Two card mask making up the players pocket cards</param>
        /// <param name="board">The community cards</param>
        /// <param name="dead">Dead cards</param>
        /// <param name="type">They type of mask to count draws for.</param>
        /// <returns>The number of draws for this mask type.</returns>
        public static int DrawCount(ulong player, ulong board, ulong dead, Hand.HandTypes type)
        {
            int retval = 0;
#if DEBUG
            if (BitCount(player) != 2) throw new ArgumentException("player must have exactly 2 cards");
            if (BitCount(board) != 3 && BitCount(board) != 4) throw new ArgumentException("board must contain 3 or 4 cards");
            if (((board | player) & dead) != 0UL) throw new ArgumentException("player and board must not contain dead cards");
#endif
            // Get original mask value
            uint playerOrigHandVal = Hand.Evaluate(player | board);

            if (Hand.HandType(playerOrigHandVal) > (uint)type)
                return 0;

            // Look ahead one card
            foreach (ulong card in Hand.Hands(0UL, board | player | dead, 1))
            {
                // Get new mask value
                uint playerNewHandVal = Hand.Evaluate(player | board | card);

                // Get new board value
                uint boardHandVal = Hand.Evaluate(board | card);

                // Is the new mask better than the old one? We don't 
                // want to know about supersizing the kickers so this
                // ensures that mask moved up in mask type.
                bool handImproved = Hand.HandType(playerNewHandVal) > Hand.HandType(playerOrigHandVal);

                // This compare ensures we aren't just making the board stronger
                bool handStrongerThanBoard = playerNewHandVal > boardHandVal;

                // If the mask improved and it matches the specified type, return true
                if (handImproved && handStrongerThanBoard && Hand.HandType(playerNewHandVal) == (int)type)
                {
                    retval++;
                }
            }

            return retval;
        }

        /// <summary>
        /// The method returns the number of draws that are possible for the
        /// specified HandType. Note that DrawCount(pocket, board, dead, type) is not
        /// necessarily the same as DrawCount(pocket | board, dead, type). 
        /// 
        /// This method returns all possible draws that are the same as the requested type.
        /// </summary>
        /// <param name="mask">Hand</param>
        /// <param name="dead">Dead cards</param>
        /// <param name="type">They type of mask to count draws for.</param>
        /// <returns>The number of draws for this mask type.</returns>
        public static int DrawCount(ulong mask, ulong dead, Hand.HandTypes type)
        {
            int retval = 0;
#if DEBUG
            if (BitCount(mask) >= 7) throw new ArgumentException("mask must contain less than 7 cards");
            if ((mask & dead) != 0UL) throw new ArgumentException("mask must not contain dead cards");
#endif
            // Get original mask type
            Hand.HandTypes playerOrigHandType = Hand.EvaluateType(mask);

            if (playerOrigHandType >= type) return 0;

            // Look ahead one card
            foreach (ulong card in Hand.Hands(0UL, mask | dead, 1))
            {
                // Get new mask value
                Hand.HandTypes playerNewHandType = Hand.EvaluateType(mask | card);

                // If the mask improved and it matches the specified type, count it.
                if (playerNewHandType > playerOrigHandType && playerNewHandType == type)
                {
                    retval++;
                }
            }

            return retval;
        }

        /// <summary>
        /// The method returns the number of draws that are possible for the
        /// specified HandType.
        /// </summary>
        /// <param name="player">Two card mask making up the players pocket cards</param>
        /// <param name="board">The community cards</param>
        /// <param name="dead">Dead cards</param>
        /// <param name="type">They type of mask to count draws for.</param>
        /// <returns>The number of draws for this mask type.</returns>
        public static int DrawCount(string player, string board, string dead, Hand.HandTypes type)
        {
#if DEBUG
            if (!Hand.ValidateHand(player)) throw new ArgumentException("player fails to Validate");
            if (!Hand.ValidateHand(board)) throw new ArgumentException("board fails to Validate");
#endif
            return DrawCount(Hand.ParseHand(player), Hand.ParseHand(board), Hand.ParseHand(dead), type);
        }

        /// <summary>
        /// This method returns the number of unique mask values that can 
        /// be made with ncards, where all of the hands contain the shared cards.
        /// </summary>
        /// <param name="shared">Cards required in all of the hands</param>
        /// <param name="ncards">The number of cards in the mask</param>
        /// <returns>The total number of unique mask values</returns>
        static public long UniqueHands(ulong shared, int ncards)
        {
#if DEBUG
            if (BitCount(shared) > ncards) throw new ArgumentException("mask must contain less cards than ncards");
#endif
            uint hv = 0U;
            Dictionary<uint, long> dict = new Dictionary<uint, long>();
            foreach (ulong mask in Hand.Hands(shared, 0UL, ncards))
            {
                hv = Hand.Evaluate(mask);
                if (!dict.ContainsKey(hv))
                    dict.Add(hv, 1);
            }
            return dict.Count;
        }

        /// <summary>
        /// This method returns the mask distance from the best possible
        /// mask given this board (no draws are considered). The value 0 is the 
        /// best possible mask. The value 1 is the next best mask and so on.
        /// </summary>
        /// <param name="pocket">The players pocket mask</param>
        /// <param name="board">The board mask</param>
        /// <returns>An integer representing the distance from the best possible mask</returns>
        static public long HandDistance(ulong pocket, ulong board)
        {
#if DEBUG
            if (BitCount(pocket) != 2) throw new ArgumentException("player must have exactly 2 cards");
            if (BitCount(board) != 3 && BitCount(board) != 4) throw new ArgumentException("board must contain 3 or 4 cards");
#endif
            uint hv = 0U;
            SortedDictionary<uint, long> dict = new SortedDictionary<uint, long>();

            // Current Hand
            uint pocketHandVal = Hand.Evaluate(pocket | board);

            // Build a List of unique hands
            foreach (ulong p in Hand.Hands(0UL, board, 2))
            {
                hv = Hand.Evaluate(p | board);
                if (!dict.ContainsKey(hv))
                    dict.Add(hv, 1);
            }

            // Find current mask in the sorted list
            long count = dict.Count-1;
            foreach (uint handval in dict.Keys)
            {
                if (handval == pocketHandVal)
                    return count;
                count--;
            }
            return -1;
        }

        /// <summary>
        /// Returns the number of discouted outs possible with the next card.
        /// 
        /// Players pocket cards
        /// The board (must contain either 3 or 4 cards)
        /// A list of zero or more opponent cards.
        /// The count of the number of single cards that improve the current hand applying the following discouting rules:
        /// 1) Drawing to a pair must use an over card (ie a card higher than all those on the board)
        /// 2) Drawing to 2 pair / pairing your hand is discounted if the board is paired (ie your 2 pair drawing deat to trips)
        /// 3) Drawing to a hand lower than a flush must not make a 3 suited board or the board be 3 suited.
        /// 4) Drawing to a hand lower than a stright, the flop must not be 3card connected or on the turn
        /// allow a straight to be made with only 1 pocket card or the out make a straight using only 1 card.
        /// 
        /// Function provided by Matt Baker.
        /// </summary>
        /// <param name="player">Players pocket hand</param>
        /// <param name="board">Board cards so far</param>
        /// <param name="opponents">Opponents pocket hands</param>
        /// <returns>Number of outs</returns>
        public static int OutsDiscounted(ulong player, ulong board, params ulong[] opponents)
        {
            return BitCount(OutsMaskDiscounted(player, board, opponents));
        }

        /// <summary>
        /// 
        /// Creates a Hand mask with the cards that will improve the specified players hand
        /// against a list of opponents or if no opponents are list just the cards that improve the 
        /// players current hand. 
        /// 
        /// This implements the concept of 'discounted outs'. That is outs that will improve the
        /// players hand, but not potentially improve an opponents hand to an even better one. For
        /// example drawing to a straight that could end up loosing to a flush.
        /// 
        /// Please note that this only looks at single cards that improve the hand and will not specifically
        /// look at runner-runner possiblities.
        /// 
        /// Players pocket cards
        /// The board (must contain either 3 or 4 cards)
        /// A list of zero or more opponent pocket cards
        /// A mask of all of the cards that improve the hand applying the following discouting rules: 
        /// 1) Drawing to a pair must use an over card (ie a card higher than all those on the board)
        /// 2) Drawing to 2 pair / pairing your hand is discounted if the board is paired (ie your 2 pair drawing deat to trips)
        /// 3) Drawing to a hand lower than a flush must not make a 3 suited board or the board be 3 suited.
        /// 4) Drawing to a hand lower than a stright, the flop must not be 3card connected or on the turn
        /// allow a straight to be made with only 1 pocket card or the out make a straight using only 1 card. 
        /// 
        /// 
        /// Function provided by Matt Baker.
        /// </summary>
        /// <param name="player">Players pocket hand</param>
        /// <param name="board">Board mask</param>
        /// <param name="opponents">Opponent pocket hand</param>
        /// <returns>Mask of cards that are probable outs.</returns>
        public static ulong OutsMaskDiscounted(ulong player, ulong board, params ulong[] opponents)
        {
            ulong retval = 0UL, dead = 0UL;
            int ncards = Hand.BitCount(player | board);
#if DEBUG
            Debug.Assert(Hand.BitCount(player) == 2); // Must have two cards for a legit set of pocket cards
            if (ncards != 5 && ncards != 6)
                throw new ArgumentException("Outs only make sense after the flop and before the river");
#endif

            if (opponents.Length > 0)
            {
                // Check opportunities to improve against one or more opponents
                foreach (ulong opp in opponents)
                {
                    Debug.Assert(Hand.BitCount(opp) == 2); // Must have two cards for a legit set of pocket cards
                    dead |= opp;
                }

                uint playerOrigHandVal = Hand.Evaluate(player | board, ncards);
                uint playerOrigHandType = Hand.HandType(playerOrigHandVal);
                uint playerOrigTopCard = Hand.TopCard(playerOrigHandVal);

                foreach (ulong card in Hand.Hands(0UL, dead | board | player, 1))
                {
                    bool bWinFlag = true;
                    uint playerHandVal = Hand.Evaluate(player | board | card, ncards + 1);
                    uint playerNewHandType = Hand.HandType(playerHandVal);
                    uint playerNewTopCard = Hand.TopCard(playerHandVal);
                    foreach (ulong oppmask in opponents)
                    {
                        uint oppHandVal = Hand.Evaluate(oppmask | board | card, ncards + 1);

                        bWinFlag = oppHandVal < playerHandVal && (playerNewHandType > playerOrigHandType || (playerNewHandType == playerOrigHandType && playerNewTopCard > playerOrigTopCard));
                        if (!bWinFlag)
                            break;
                    }
                    if (bWinFlag)
                        retval |= card;
                }
            }
            else
            {
                // Look at the cards that improve the hand.
                uint playerOrigHandVal = Hand.Evaluate(player | board, ncards);
                uint playerOrigHandType = Hand.HandType(playerOrigHandVal);
                uint playerOrigTopCard = Hand.TopCard(playerOrigHandVal);
                uint boardOrigHandVal = Hand.Evaluate(board);
                uint boardOrigHandType = Hand.HandType(boardOrigHandVal);
                uint boardOrigTopCard = Hand.TopCard(boardOrigHandVal);

                // Look at players pocket cards for special cases.
                uint playerPocketHandVal = Hand.Evaluate(player);
                uint playerPocketHandType = Hand.HandType(playerPocketHandVal);

                // Seperate out by suit
                uint sc = (uint)((board >> (CLUB_OFFSET)) & 0x1fffUL);
                uint sd = (uint)((board >> (DIAMOND_OFFSET)) & 0x1fffUL);
                uint sh = (uint)((board >> (HEART_OFFSET)) & 0x1fffUL);
                uint ss = (uint)((board >> (SPADE_OFFSET)) & 0x1fffUL);

                // Check if board is 3 suited. 
                bool discountSuitedBoard = ((nBitsTable[sc] > 2) || (nBitsTable[sd] > 2) || (nBitsTable[sh] > 2) || (nBitsTable[ss] > 2));

                // Check if board is 3 Conected on the flop. A dangerous board:
                // 3 possible straights using 2 pocket cards and a higher chance 
                // of 2 pair; players often play 2 connected cards which can hit. 
                int CountContiguous = 0;
                int boardCardCount = BitCount(board);

                if (boardCardCount == 3)
                {
                    uint bf = CardMask(board, Clubs) | CardMask(board, Diamonds) | CardMask(board, Hearts) | CardMask(board, Spades);

                    if (BitCount(0x1800 & bf) == 2) CountContiguous++;
                    if (BitCount(0xc00 & bf) == 2) CountContiguous++;
                    if (BitCount(0x600 & bf) == 2) CountContiguous++;
                    if (BitCount(0x300 & bf) == 2) CountContiguous++;
                    if (BitCount(0x180 & bf) == 2) CountContiguous++;
                    if (BitCount(0xc0 & bf) == 2) CountContiguous++;
                    if (BitCount(0x60 & bf) == 2) CountContiguous++;
                    if (BitCount(0x30 & bf) == 2) CountContiguous++;
                    if (BitCount(0x18 & bf) == 2) CountContiguous++;
                    if (BitCount(0xc & bf) == 2) CountContiguous++;
                    if (BitCount(0x6 & bf) == 2) CountContiguous++;
                    if (BitCount(0x3 & bf) == 2) CountContiguous++;
                    if (BitCount(0x1001 & bf) == 2) CountContiguous++;

                }
                bool discountStraight = (CountContiguous > 1);

                // Look ahead one card
                foreach (ulong card in Hand.Hands(0UL, dead | board | player, 1))
                {
                    uint boardNewHandval = Hand.Evaluate(board | card);
                    uint boardNewHandType = Hand.HandType(boardNewHandval);
                    uint boardNewTopCard = Hand.TopCard(boardNewHandval);
                    uint playerNewHandVal = Hand.Evaluate(player | board | card, ncards + 1);
                    uint playerNewHandType = Hand.HandType(playerNewHandVal);
                    uint playerNewTopCard = Hand.TopCard(playerNewHandVal);
                    bool playerImproved = (playerNewHandType > playerOrigHandType || (playerNewHandType == playerOrigHandType && playerNewTopCard > playerOrigTopCard) || (playerNewHandType == playerOrigHandType && playerNewHandType == (uint)HandTypes.TwoPair && playerNewHandVal > playerOrigHandVal));
                    bool playerStrongerThanBoard = (playerNewHandType > boardNewHandType || (playerNewHandType == boardNewHandType && playerNewTopCard > boardNewTopCard));

                    if (playerImproved && playerStrongerThanBoard)
                    {
                        bool isOut = false;

                        bool discountSuitedOut = false;
                        if (!discountSuitedBoard)
                        {
                            // Get suit of card.
                            uint cc = (uint)((card >> (CLUB_OFFSET)) & 0x1fffUL);
                            uint cd = (uint)((card >> (DIAMOND_OFFSET)) & 0x1fffUL);
                            uint ch = (uint)((card >> (HEART_OFFSET)) & 0x1fffUL);
                            uint cs = (uint)((card >> (SPADE_OFFSET)) & 0x1fffUL);

                            // Check if card will make a 3 suited board. 
                            discountSuitedOut = ((nBitsTable[sc] > 1 && nBitsTable[cc] == 1) || (nBitsTable[sd] > 1 && nBitsTable[cd] == 1) || (nBitsTable[sh] > 1 && nBitsTable[ch] == 1) || (nBitsTable[ss] > 1 && nBitsTable[cs] == 1));
                        }

                        // Check if board is 4 connected or card + board is 4 connected. 
                        // Dangerous board: straight using 1 pocket card only.
                        if (boardCardCount == 4)
                        {
                            // We need to check for the following:
                            // 9x,8x,7x,6x (4 in a row)
                            // 9x,8x,7x,5x (3 in a row with a 1 gap connected card)
                            // 9x,8x,6x,5x (2 connected with a 1 gap connected in the middle)
                            CountContiguous = 0;
                            uint bf = CardMask(board | card, Clubs) | CardMask(board | card, Diamonds) | CardMask(board | card, Hearts) | CardMask(board | card, Spades);

                            // AxKx
                            if (BitCount(0x1800 & bf) == 2)
                            {
                                CountContiguous++;
                            }

                            // KxQx
                            if (BitCount(0xc00 & bf) == 2)
                            {
                                CountContiguous++;
                            }
                            else
                            {
                                if (CountContiguous == 1 && BitCount(0x300 & bf) == 2)
                                    // 2 connected with a 1 gap connected in the middle
                                    discountStraight = true;

                                CountContiguous = 0;
                            }

                            // QxJx
                            if (BitCount(0x600 & bf) == 2)
                            {
                                CountContiguous++;
                            }
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 1:
                                        if (BitCount(0x180 & bf) == 2)
                                            // 2 connected with a 1 gap in the middle
                                            discountStraight = true;
                                        break;
                                    case 2:
                                        // test for a T
                                        if (BitCount(0x100 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                }
                                CountContiguous = 0;
                            }

                            // JxTx
                            if (BitCount(0x300 & bf) == 2)
                                CountContiguous++;
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 1:
                                        if (BitCount(0xc0 & bf) == 2)
                                            // 2 connected with a 1 gap in the middle
                                            discountStraight = true;
                                        break;
                                    case 2:
                                        // test for 9x
                                        if (BitCount(0x80 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                                CountContiguous = 0;
                            }

                            // Tx9x
                            if (BitCount(0x180 & bf) == 2)
                                CountContiguous++;
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 1:
                                        if (BitCount(0x60 & bf) == 2)
                                            // 2 connected with a 1 gap in the middle
                                            discountStraight = true;
                                        break;
                                    case 2:
                                        // test for 8x or Ax
                                        if (BitCount(0x1040 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                                CountContiguous = 0;
                            }

                            // 9x8x
                            if (BitCount(0xc0 & bf) == 2)
                                CountContiguous++;
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 1:
                                        if (BitCount(0x30 & bf) == 2)
                                            // 2 connected with a 1 gap in the middle
                                            discountStraight = true;
                                        break;
                                    case 2:
                                        // test for 7x or Kx
                                        if (BitCount(0x820 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                                CountContiguous = 0;
                            }

                            // 8x7x
                            if (BitCount(0x60 & bf) == 2)
                                CountContiguous++;
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 1:
                                        if (BitCount(0x18 & bf) == 2)
                                            // 2 connected with a 1 gap in the middle
                                            discountStraight = true;
                                        break;
                                    case 2:
                                        // test for 6x or Qx
                                        if (BitCount(0x410 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                                CountContiguous = 0;
                            }

                            // 7x6x
                            if (BitCount(0x30 & bf) == 2)
                                CountContiguous++;
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 1:
                                        if (BitCount(0xc & bf) == 2)
                                            // 2 connected with a 1 gap in the middle
                                            discountStraight = true;
                                        break;
                                    case 2:
                                        // test for 5x or Jx
                                        if (BitCount(0x208 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                                CountContiguous = 0;
                            }

                            // 6x5x
                            if (BitCount(0x18 & bf) == 2)
                                CountContiguous++;
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 1:
                                        if (BitCount(0x6 & bf) == 2)
                                            // 2 connected with a 1 gap in the middle
                                            discountStraight = true;
                                        break;
                                    case 2:
                                        // test for 4x or Tx
                                        if (BitCount(0x104 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                                CountContiguous = 0;
                            }

                            // 5x4x
                            if (BitCount(0xc & bf) == 2)
                                CountContiguous++;
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 1:
                                        if (BitCount(0x3 & bf) == 2)
                                            // 2 connected with a 1 gap in the middle
                                            discountStraight = true;
                                        break;
                                    case 2:
                                        // test for 3x or 9x
                                        if (BitCount(0x82 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                                CountContiguous = 0;
                            }

                            // 4x3x
                            if (BitCount(0x6 & bf) == 2)
                                CountContiguous++;
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 1:
                                        if (BitCount(0x1001 & bf) == 2)
                                            // 2 connected with a 1 gap in the middle
                                            discountStraight = true;
                                        break;
                                    case 2:
                                        // test for 2x or 8x
                                        if (BitCount(0x41 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                                CountContiguous = 0;
                            }

                            // 3x2x
                            if (BitCount(0x3 & bf) == 2)
                                CountContiguous++;
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 2:
                                        // test for Ax or 7x
                                        if (BitCount(0x1020 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                                CountContiguous = 0;
                            }

                            // 2xAx
                            if (BitCount(0x1001 & bf) == 2)
                            {
                                CountContiguous++;

                                // Check one last time.
                                switch (CountContiguous)
                                {
                                    case 2:
                                        // test for 5x
                                        if (BitCount(0x8 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                            }
                            else
                            {
                                switch (CountContiguous)
                                {
                                    case 2:
                                        // test for 6x
                                        if (BitCount(0x10 & bf) == 1)
                                            // 3 in a row with a 1 gap connected
                                            discountStraight = true;
                                        break;
                                    case 3: // 4 in a row 
                                        discountStraight = true;
                                        break;
                                }
                            }
                        }

                        // Hand imporving to a pair, must use overcards and not make a 3 suited board. 
                        if (playerNewHandType == (uint)HandTypes.Pair)
                        {
                            uint newCardVal = Hand.Evaluate(card);
                            uint newTopCard = Hand.TopCard(newCardVal);

                            if (boardOrigTopCard < newTopCard && !(discountSuitedBoard || discountSuitedOut) && !(discountStraight))
                                isOut = true;
                        }

                                // Hand imporving to two pair, must use one of the players pocket cards and 
                        // the player already has a pair, either a pocket pair or a pair using the board. 
                        // ie: not drawing to two pair when trips is out - drawing dead.
                        // And not make a 3 suited board and not discounting for a straight. 
                        else if (playerNewHandType == (uint)HandTypes.TwoPair)
                        {
                            // Special case pair improving to two pair must use one of the players cards. 
                            uint playerPocketHandNewCardVal = Hand.Evaluate(player | card);
                            uint playerPocketHandNewCardType = Hand.HandType(playerPocketHandNewCardVal);

                            if ((playerPocketHandNewCardType == (uint)HandTypes.Pair && playerPocketHandType != (uint)HandTypes.Pair) && (boardOrigHandType != (uint)HandTypes.Pair || playerOrigHandType == (uint)HandTypes.TwoPair))
                            {
                                if (!(discountSuitedBoard || discountSuitedOut) && !(discountStraight))
                                    isOut = true;
                            }
                        }
                        // New hand better than two pair.
                        else if (playerNewHandType > (uint)HandTypes.TwoPair)
                        {
                            // Hand imporving trips, must not make a 3 suited board and not discounting for a straight. 
                            if (playerNewHandType == (uint)HandTypes.Trips)
                            {
                                if (!(discountSuitedBoard || discountSuitedOut) && !(discountStraight))
                                    isOut = true;
                            }
                            // Hand imporving to a straight, must not make a 3 suited board.
                            else if (playerNewHandType == (uint)HandTypes.Straight)
                            {
                                if (!(discountSuitedBoard || discountSuitedOut))
                                    isOut = true;
                            }
                            else
                                // No discounting for a Flush (should we consider a straight Flush?),
                                // Full House, Four of a Kind and Straight Flush.
                                isOut = true;
                        }
                        if (isOut)
                            retval |= card;
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// Returns the number of outs possible with the next card. Note that cards that only
        /// help the board will be ignored.
        /// </summary>
        /// <param name="player">Players pocket cards</param>
        /// <param name="board">The board (must contain either 3 or 4 cards)</param>
        /// <param name="opponents">A list of zero or more opponent cards.</param>
        /// <returns></returns>
        public static int Outs(string player, string board, params string[] opponents)
        {
            if (!Hand.ValidateHand(player) && Hand.BitCount(Hand.ParseHand(player)) != 2) throw new ArgumentException("player");
            if (!Hand.ValidateHand(board) && Hand.BitCount(Hand.ParseHand(board)) != 3 && Hand.BitCount(Hand.ParseHand(board)) != 4) throw new ArgumentException("board");

            ulong[] opponentsMask = new ulong[opponents.Length];
            for (int i = 0; i < opponents.Length; i++)
            {
                if (!Hand.ValidateHand(opponents[i]) && Hand.BitCount(Hand.ParseHand(opponents[i])) != 2) throw new ArgumentException("opponents");
                opponentsMask[i] = Hand.ParseHand(opponents[i]);
            }
            return Outs(Hand.ParseHand(player), Hand.ParseHand(board), opponentsMask);
        }

        /// <summary>
        /// Returns a string of the possible outs for the next card. Note that cards that only
        /// help the board will be ignored.
        /// </summary>
        /// <param name="player">Player pocket card string</param>
        /// <param name="board">Board card string</param>
        /// <param name="opponents">Opponent card strings</param>
        /// <returns>String containing the cards that will improve the players mask</returns>
        public static string OutCards(string player, string board, params string[] opponents)
        {
            if (!Hand.ValidateHand(player) && Hand.BitCount(Hand.ParseHand(player)) != 2) throw new ArgumentException("player");
            if (!Hand.ValidateHand(board) && Hand.BitCount(Hand.ParseHand(board)) != 3 && Hand.BitCount(Hand.ParseHand(board)) != 4) throw new ArgumentException("board");

            ulong[] opponentsMask = new ulong[opponents.Length];
            for (int i = 0; i < opponents.Length; i++)
            {
                if (!Hand.ValidateHand(opponents[i]) && Hand.BitCount(Hand.ParseHand(opponents[i])) != 2) throw new ArgumentException("opponents");
                opponentsMask[i] = Hand.ParseHand(opponents[i]);
            }

            ulong mask = OutsMask(Hand.ParseHand(player), Hand.ParseHand(board), opponentsMask);

            string retval = "";
            foreach (string s in Cards(mask))
            {
                if (retval.Length == 0)
                    retval = s;
                else
                    retval += " " + s;
            }
  
            return retval;
        }

        /// <summary>
        /// Returns the number of outs possible with the next card.
        /// </summary>
        /// <param name="player">Players pocket cards</param>
        /// <param name="board">The board (must contain either 3 or 4 cards)</param>
        /// <param name="opponents">A list of zero or more opponent cards.</param>
        /// <returns>The count of the number of single cards that improve the current mask.</returns>
        public static int Outs(ulong player, ulong board, params ulong[] opponents)
        {
            return BitCount(OutsMask(player, board, opponents));
        }

        /// <summary>
        /// Creates a Hand mask with the cards that will improve the specified players mask
        /// against a list of opponents or if no opponents are list just the cards that improve the 
        /// players current had.
        /// 
        /// Please note that this only looks at single cards that improve the mask and will not specifically
        /// look at runner-runner possiblities.
        /// </summary>
        /// <param name="player">Players pocket cards</param>
        /// <param name="board">The board (must contain either 3 or 4 cards)</param>
        /// <param name="opponents">A list of zero or more opponent pocket cards</param>
        /// <returns>A mask of all of the cards taht improve the mask.</returns>
        public static ulong OutsMask(ulong player, ulong board, params ulong[] opponents)
        {
            ulong retval = 0UL;

#if DEBUG
            if (BitCount(player) != 2) throw new ArgumentException("player must have exactly 2 cards");
            if (BitCount(board) != 3 && BitCount(board) != 4) throw new ArgumentException("board must contain 3 or 4 cards");
#endif

            // Get original mask value
            uint playerOrigHandVal = Hand.Evaluate(player | board);

            // Look ahead one card
            foreach (ulong card in Hand.Hands(0UL, board | player, 1))
            {
                // Get new mask value
                uint playerNewHandVal = Hand.Evaluate(player | board | card);
                // Get new board value
                uint boardHandVal = Hand.Evaluate(board | card);

                // Is the new mask better than the old one?
                bool handImproved = playerNewHandVal > playerOrigHandVal;

                // This compare ensures we move up in mask type.
                bool handStrongerThanBoard = Hand.HandType(playerNewHandVal) > Hand.HandType(boardHandVal) ||
                                            (Hand.HandType(playerNewHandVal) == Hand.HandType(boardHandVal) && 
                                            Hand.TopCard(playerNewHandVal) > Hand.TopCard(boardHandVal));

                // Check against opponents cards
                bool handBeatAllOpponents = true;
                if (handImproved && handStrongerThanBoard && opponents != null && opponents.Length > 0)
                {
                    foreach (ulong opponent in opponents)
                    {
                        uint opponentHandVal = Hand.Evaluate(opponent | board | card);
                        if (opponentHandVal > playerNewHandVal)
                        {
                            handBeatAllOpponents = false;
                            break;
                        }
                    }
                }

                // If the mask improved then we have an out
                if (handImproved && handStrongerThanBoard && handBeatAllOpponents)
                {
                    // Add card to outs mask
                    retval |= card;
                }
            }

            // return outs as a mask mask
            return retval;
        }

        /// <summary>
        /// Creates a Hand mask with the cards that will improve the specified players mask.
        ///
        /// Please note that this only looks at single cards that improve the mask and will not specifically
        /// look at runner-runner possiblities.
        /// 
        /// This version of outs contributed by Matt Baker
        /// </summary>
        /// <param name="player">Players pocket cards</param>
        /// <param name="board">The board (must contain either 3 or 4 cards)</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>A mask of all of the cards taht improve the mask.</returns>
        public static ulong OutsMaskEx(ulong player, ulong board, ulong dead)
        {
            ulong retval = 0UL;
            int ncards = Hand.BitCount(player | board);
#if DEBUG
            Debug.Assert(Hand.BitCount(player) == 2); // Must have two cards for a legit set of pocket cards
            if (ncards != 5 && ncards != 6)
                throw new ArgumentException("Outs only make sense after the flop and before the river");
#endif
            // Look at the cards that improve the mask.
            uint playerOrigHandVal = Hand.Evaluate(player | board, ncards);
            uint playerOrigHandType = Hand.HandType(playerOrigHandVal);
            uint playerOrigTopCard = Hand.TopCard(playerOrigHandVal);
            uint boardOrigHandVal = Hand.Evaluate(board);
            uint boardOrigHandType = Hand.HandType(boardOrigHandVal);
            uint boardOrigTopCard = Hand.TopCard(boardOrigHandVal);

            // Look at players pocket cards for special cases.
            uint playerPocketHandVal = Hand.Evaluate(player);
            uint playerPocketHandType = Hand.HandType(playerPocketHandVal);

            // Look ahead one card
            foreach (ulong card in Hand.Hands(0UL, dead | board | player, 1))
            {
                uint boardNewHandval = Hand.Evaluate(board | card);
                uint boardNewHandType = Hand.HandType(boardNewHandval);
                uint boardNewTopCard = Hand.TopCard(boardNewHandval);
                uint playerNewHandVal = Hand.Evaluate(player | board | card, ncards + 1);
                uint playerNewHandType = Hand.HandType(playerNewHandVal);
                uint playerNewTopCard = Hand.TopCard(playerNewHandVal);
                bool playerImproved = (playerNewHandType > playerOrigHandType || (playerNewHandType == playerOrigHandType && playerNewTopCard > playerOrigTopCard) || (playerNewHandType == playerOrigHandType && playerNewHandType == (uint)HandTypes.TwoPair && playerNewHandVal > playerOrigHandVal));
                bool playerStrongerThanBoard = (playerNewHandType > boardNewHandType || (playerNewHandType == boardNewHandType && playerNewTopCard > boardNewTopCard));

                if (playerImproved && playerStrongerThanBoard)
                {
                    // New mask better than two pair and special case pocket pair improving to full house.
                    if (playerNewHandType > (uint)HandTypes.TwoPair || (playerPocketHandType == (uint)HandTypes.Pair && playerNewHandType > (uint)HandTypes.TwoPair))
                        retval |= card;
                    else
                    {
                        // Special case pair improving to two pair must use one of the players cards.
                        uint playerPocketHandNewCardVal = Hand.Evaluate(player | card);
                        uint playerPocketHandNewCardType = Hand.HandType(playerPocketHandNewCardVal);

                        if (playerPocketHandNewCardType == (uint)HandTypes.Pair && playerPocketHandType != (uint)HandTypes.Pair)
                            retval |= card;
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pocket"></param>
        /// <param name="board"></param>
        /// <param name="dead"></param>
        /// <returns></returns>
        public static ulong OutsMaskEx(string pocket, string board, string dead)
        {
            return OutsMaskEx(Hand.ParseHand(pocket), Hand.ParseHand(board), Hand.ParseHand(dead));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pocket"></param>
        /// <param name="board"></param>
        /// <param name="dead"></param>
        /// <returns></returns>
        public static int OutsEx(ulong pocket, ulong board, ulong dead)
        {
            return BitCount(OutsMaskEx(pocket, board, dead));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pocket"></param>
        /// <param name="board"></param>
        /// <param name="dead"></param>
        /// <returns></returns>
        public static int OutsEx(string pocket, string board, string dead)
        {
            return OutsEx(Hand.ParseHand(pocket), Hand.ParseHand(board), Hand.ParseHand(dead));
        }
        #endregion

        #region IsSuited

        /// <summary>
        /// This function returns true if the cards in the mask are all one suit. This method
        /// calculates the results. Because of the equivelent call in PocketHands is preferred
        /// because it uses a lookup table and is faster. This function remains to allow for automated
        /// testing.
        /// </summary>
        /// <param name="mask">mask to check for "suited-ness"</param>
        /// <returns>true if all hands are of the same suit, false otherwise.</returns>
        public static bool IsSuited(ulong mask)
        {
            int cards = BitCount(mask);
           
            uint sc = CardMask(mask, Clubs);
            uint sd = CardMask(mask, Diamonds);
            uint sh = CardMask(mask, Hearts);
            uint ss = CardMask(mask, Spades);

            return  BitCount(sc) == cards || BitCount(sd) == cards ||
                    BitCount(sh) == cards || BitCount(ss) == cards;
        }
        #endregion

        #region IsConnected

        /// <summary>
        /// Returns true if the cards in the two card mask are connected. This method
        /// calculates the results. Because of that equivelent call in PocketHands is preferred
        /// because it uses a lookup table and is faster. This function remains to allow for automated
        /// testing.
        /// </summary>
        /// <param name="mask">the mask to check</param>
        /// <returns>true of all of the cards are next to each other.</returns>
        public static bool IsConnected(ulong mask)
        {
            return GapCount(mask) == 0;
        }
        #endregion

        #region GapCount
        /// <summary>
        /// Counts the number of empty space between adjacent cards. 0 means connected, 1 means a gap
        /// of one, 2 means a gap of two and 3 means a gap of three. This method
        /// calculates the results. Because of that equivelent call in PocketHands is preferred
        /// because it uses a lookup table and is faster. This function remains to allow for automated
        /// testing.
        /// </summary>
        /// <param name="mask">two card mask mask</param>
        /// <returns>number of spaces between two cards</returns>
        static public int GapCount(ulong mask)
        {
            int start, end;

            if (BitCount(mask) != 2) return -1;

            uint bf = CardMask(mask, Clubs) |
                        CardMask(mask, Diamonds) |
                        CardMask(mask, Hearts) |
                        CardMask(mask, Spades);

            if (BitCount(bf) != 2) return -1;

            for (start = 12; start >= 0; start--)
            {
                if ((bf & (1UL << start)) != 0)
                    break;
            }

            for (end = start - 1; end >= 0; end--)
            {
                if ((bf & (1UL << end)) != 0)
                    break;
            }

            // Handle wrap
            if (start == 12 && end == 0) return 0;
            if (start == 12 && end == 1) return 1;
            if (start == 12 && end == 2) return 2;
            if (start == 12 && end == 3) return 3;

            return (start-end-1 > 3 ? -1 : start-end-1);
        }
        #endregion

        #region HandWinOdds

        /// <summary>
        /// Cacluates the approxamate odds that each player and opponent mask
        /// type has to win. This method uses Monte Carlo Analysis to determine
        /// a results. The quality of the result will depend on the number of trials.
        /// </summary>
        /// <param name="pocket">Players Hand</param>
        /// <param name="board">Current Board</param>
        /// <param name="dead">Dead Cards</param>
        /// <param name="playerCount">the number of players in the mask.</param>
        /// <param name="playerodds">The returned odds array for the player</param>
        /// <param name="oppodds">The returned odds array for the opponents</param>
        /// <param name="trials">The number of simulations to run.</param>
        public static void DetailedOddsWithMultiplePlayers(string pocket, string board, ulong dead, int playerCount, out double[] playerodds, out double[] oppodds, int trials)
        {
            // Holds total count.
            int count = 0;
            ulong deadmask = 0UL, finalboard = 0UL, pocketmask = 0UL;
            uint best;
            Random rand = new Random();

            // Intermediate Results
            ulong[] opponent = new ulong[playerCount];
            uint[] oppHandVal = new uint[playerCount];

            // Storage for results
            double[] podds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] oodds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

#if DEBUG
            if (!Hand.ValidateHand(board)) throw new ArgumentException();
            if (playerCount < 1 || playerCount > 9) throw new ArgumentException();
            if (Hand.BitCount(Hand.ParseHand(board)) > 5) throw new ArgumentException();
            if (trials <= 0) throw new ArgumentException();
            if (!PocketHands.ValidateQuery(pocket)) throw new ArgumentException();
#endif

            ulong boardmask = Hand.ParseHand(board);
            ulong[] pocketlist = PocketHands.Query(pocket, boardmask | dead);

            // Loop until count equals trials
            while (count < trials)
            {
                pocketmask = Hand.RandomHand(rand, pocketlist, boardmask | dead, 2);

                // The order cards are dealt doesn't effect the resulting
                // odds in a simulation like this. So for coding convienence
                // we will draw the board cards before the opponents cards.
                finalboard = Hand.RandomHand(rand, boardmask, dead | pocketmask, 5);
                deadmask = pocketmask | dead | finalboard;

                // Assign opponents their hands and calculate their
                // mask value.
                for (int i = 0; i < playerCount; i++)
                {
                    deadmask |= opponent[i] = Hand.RandomHand(rand, 0UL, deadmask, 2);
                    oppHandVal[i] = Hand.Evaluate(opponent[i] | finalboard, 7);
                }

                // Get players mask values
                uint handval = Hand.Evaluate(pocketmask | finalboard, 7);

                // Sort the opponent mask values. This way we know 
                // the the best opponent mask will be last in the array.
                Array.Sort<uint>(oppHandVal);

                // Get the best opponent mask.
                best = oppHandVal[playerCount - 1];

                // Update the results
                if (handval > best)
                {
                    podds[Hand.HandType(handval)] += 1.0;
                }
                else if (handval == best)
                {
                    podds[Hand.HandType(handval)] += 0.5;
                    oodds[Hand.HandType(best)] += 0.5;
                }
                else
                {
                    oodds[Hand.HandType(best)] += 1.0;
                }

                // Update count
                count++;
            }

            // Normalize the results.
            if (count > 0)
            {
                for (int i = 0; i < podds.Length; i++)
                {
                    podds[i] /= count;
                    oodds[i] /= count;
                }
            }

            // returned odds
            playerodds = podds;
            oppodds = oodds;
        }

        /// <summary>
        /// Cacluates the approxamate odds that each player and opponent mask
        /// type has to win. This method uses Monte Carlo Analysis to determine
        /// a results. The quality of the result will depend on the amount of time
        /// allowed for the simulation.
        /// </summary>
        /// <param name="pocket">Players Hand</param>
        /// <param name="board">Current Board</param>
        /// <param name="dead">Dead Cards</param>
        /// <param name="playerCount">the number of players in the mask.</param>
        /// <param name="playerodds">The returned odds array for the player</param>
        /// <param name="oppodds">The returned odds array for the opponents</param>
        /// <param name="duration">The time allowed to run the simulation</param>
        public static void DetailedOddsWithMultiplePlayers(string pocket, string board, ulong dead, int playerCount, out double[] playerodds, out double[] oppodds, double duration)
        {
#if DEBUG
            if (!Hand.ValidateHand(board)) throw new ArgumentException();
            if (playerCount < 1 || playerCount > 9) throw new ArgumentException();
            if (Hand.BitCount(Hand.ParseHand(board)) > 5) throw new ArgumentException();
            if (duration < 0.0) throw new ArgumentException();
#endif
            Random rand = new Random();
            ulong boardmask = Hand.ParseHand(board);
            ulong[] pocketlist = PocketHands.Query(pocket, boardmask | dead);
            double[] podds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] oodds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            ulong[] opponent = new ulong[playerCount];
            uint[] oppHandVal = new uint[playerCount];
            double start = Hand.CurrentTime;
            int count = 0;
            ulong deadmask, finalboard, pocketmask;
            uint best;

            while ((Hand.CurrentTime - start) < duration)
            {
                pocketmask = Hand.RandomHand(rand, pocketlist, boardmask, 2);

                // The order cards are dealt doesn't effect the resulting
                // odds in a simulation like this. So for coding convienence
                // we will draw the board cards before the opponents cards.
                finalboard = Hand.RandomHand(rand, boardmask, pocketmask | dead, 5);
                deadmask = pocketmask | dead | finalboard;

                for (int i = 0; i < playerCount; i++)
                {
                    deadmask |= opponent[i] = Hand.RandomHand(rand, 0UL, deadmask, 2);
                    oppHandVal[i] = Hand.Evaluate(opponent[i] | finalboard, 7);
                }

                uint handval = Hand.Evaluate(pocketmask | finalboard, 7);

                Array.Sort<uint>(oppHandVal);
                best = oppHandVal[playerCount - 1];

                if (handval > best)
                {
                    podds[Hand.HandType(handval)] += 1.0;
                }
                else if (handval == best)
                {
                    podds[Hand.HandType(handval)] += 0.5;
                    oodds[Hand.HandType(best)] += 0.5;
                }
                else
                {
                    oodds[Hand.HandType(best)] += 1.0;
                }
                count++;
            }

            if (count > 0)
            {
                for (int i = 0; i < podds.Length; i++)
                {
                    podds[i] /= count;
                    oodds[i] /= count;
                }
            }

            // The returned results
            playerodds = podds;
            oppodds = oodds;
        }

        /// <summary>
        /// Cacluates the approxamate odds that each player and opponent mask
        /// type has to win. This method uses Monte Carlo Analysis to determine
        /// a results. The quality of the result will depend on the number of trials.
        /// </summary>
        /// <param name="pocket">Players Hand</param>
        /// <param name="board">Current Board</param>
        /// <param name="dead">Dead Cards</param>
        /// <param name="playerCount">the number of players in the mask.</param>
        /// <param name="playerodds">The returned odds array for the player</param>
        /// <param name="oppodds">The returned odds array for the opponents</param>
        /// <param name="trials">The number of simulations to run.</param>
        public static void DetailedOddsWithMultiplePlayers(ulong pocket, ulong board, ulong dead, int playerCount, out double[] playerodds, out double[] oppodds, int trials)
        {
#if DEBUG
            if (playerCount < 1 || playerCount > 9) throw new ArgumentException();
            if (Hand.BitCount(pocket) != 2) throw new ArgumentException();
            if (Hand.BitCount(board) > 5) throw new ArgumentException();
            if (trials <= 0) throw new ArgumentException();
#endif
            // Storage for results
            double[] podds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] oodds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

            // Intermediate Results
            ulong[] opponent = new ulong[playerCount];
            uint[] oppHandVal = new uint[playerCount];

            // Holds total count.
            int count = 0;
            // Intermediate Masks
            ulong deadmask, boardmask;
            uint best;
            Random rand = new Random();

            // Loop until count equals trials
            while (count < trials)
            {
                // The order cards are dealt doesn't effect the resulting
                // odds in a simulation like this. So for coding convienence
                // we will draw the board cards before the opponents cards.
                boardmask = Hand.RandomHand(rand, board, pocket | dead, 5);
                deadmask = pocket | dead | boardmask;

                // Assign opponents their hands and calculate their
                // mask value.
                for (int i = 0; i < playerCount; i++)
                {
                    deadmask |= opponent[i] = Hand.RandomHand(rand, 0UL, deadmask, 2);
                    oppHandVal[i] = Hand.Evaluate(opponent[i] | boardmask, 7);
                }

                // Get players mask values
                uint handval = Hand.Evaluate(pocket | boardmask, 7);

                // Sort the opponent mask values. This way we know 
                // the the best opponent mask will be last in the array.
                Array.Sort<uint>(oppHandVal);

                // Get the best opponent mask.
                best = oppHandVal[playerCount - 1];

                // Update the results
                if (handval > best)
                {
                    podds[Hand.HandType(handval)] += 1.0;
                }
                else if (handval == best)
                {
                    podds[Hand.HandType(handval)] += 0.5;
                    oodds[Hand.HandType(best)] += 0.5;
                }
                else
                {
                    oodds[Hand.HandType(best)] += 1.0;
                }

                // Update count
                count++;
            }

            // Normalize the results.
            if (count > 0)
            {
                for (int i = 0; i < podds.Length; i++)
                {
                    podds[i] /= count;
                    oodds[i] /= count;
                }
            }

            // returned odds
            playerodds = podds;
            oppodds = oodds;
        }

        /// <summary>
        /// Cacluates the approxamate odds that each player and opponent mask
        /// type has to win. This method uses Monte Carlo Analysis to determine
        /// a results. The quality of the result will depend on the amount of time
        /// allowed for the simulation.
        /// </summary>
        /// <param name="pocket">Players Hand</param>
        /// <param name="board">Current Board</param>
        /// <param name="dead">Dead Cards</param>
        /// <param name="playerCount">the number of players in the mask.</param>
        /// <param name="playerodds">The returned odds array for the player</param>
        /// <param name="oppodds">The returned odds array for the opponents</param>
        /// <param name="duration">The time allowed to run the simulation</param>
        public static void DetailedOddsWithMultiplePlayers(ulong pocket, ulong board, ulong dead, int playerCount, out double[] playerodds, out double[] oppodds, double duration)
        {
#if DEBUG
            if (playerCount < 1 || playerCount > 9) throw new ArgumentException();
            if (Hand.BitCount(pocket) != 2) throw new ArgumentException();
            if (Hand.BitCount(board) > 5) throw new ArgumentException();
            if (duration < 0.0) throw new ArgumentException();
#endif
            double[] podds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] oodds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            ulong[] opponent = new ulong[playerCount];
            uint[] oppHandVal = new uint[playerCount];
            double start = Hand.CurrentTime;
            int count = 0;
            ulong deadmask, boardmask;
            uint best;
            Random rand = new Random();

            while ((Hand.CurrentTime - start) < duration)
            {
                // The order cards are dealt doesn't effect the resulting
                // odds in a simulation like this. So for coding convienence
                // we will draw the board cards before the opponents cards.
                boardmask = Hand.RandomHand(rand, board, pocket | dead, 5);
                deadmask = pocket | dead | boardmask;

                for (int i = 0; i < playerCount; i++)
                {
                    deadmask |= opponent[i] = Hand.RandomHand(rand, 0UL, deadmask, 2);
                    oppHandVal[i] = Hand.Evaluate(opponent[i] | boardmask, 7);
                }

                uint handval = Hand.Evaluate(pocket | boardmask, 7);

                Array.Sort<uint>(oppHandVal);
                best = oppHandVal[playerCount - 1];

                if (handval > best)
                {
                    podds[Hand.HandType(handval)] += 1.0;
                }
                else if (handval == best)
                {
                    podds[Hand.HandType(handval)] += 0.5;
                    oodds[Hand.HandType(best)] += 0.5;
                }
                else
                {
                    oodds[Hand.HandType(best)] += 1.0;
                }
                count++;
            }

            if (count > 0)
            {
                for (int i = 0; i < podds.Length; i++)
                {
                    podds[i] /= count;
                    oodds[i] /= count;
                }
            }

            // The returned results
            playerodds = podds;
            oppodds = oodds;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="ourcards"></param>
       /// <param name="oppcards"></param>
       /// <param name="board"></param>
       /// <param name="player"></param>
       /// <param name="opponent"></param>
       /// <returns>True means approximate results</returns>
        public static bool HandWinOdds(ulong[] ourcards, ulong[] oppcards, ulong board, out double[] player, out double[] opponent)
        {
            uint ourbest, oppbest;
            int count = 0;
            int boardcount = BitCount(board);
            int cards = boardcount + 2;
            double[] podds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] oodds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
#if DEBUG
            // Preconditions
            foreach (ulong pocketcards in ourcards)
            {
                if (BitCount(pocketcards) != 2) throw new ArgumentOutOfRangeException("pocketcards");
            }

            foreach (ulong pocketcards in oppcards)
            {
                if (BitCount(pocketcards) != 2) throw new ArgumentOutOfRangeException("pocketcards");
            }
            if (boardcount > 5) throw new ArgumentOutOfRangeException("boardcards");
#endif

            player = podds;
            opponent = oodds;

            if (boardcount == 0)
            {
                // Calculate monte carlo results
                foreach (ulong pocketcards in ourcards)
                {
                    if ((pocketcards & board) != 0UL) continue;

                    foreach (ulong opphand in oppcards)
                    {
                        if (((opphand & pocketcards) != 0UL) || (opphand & board) != 0UL) continue;

                        foreach (ulong handmask in Hand.RandomHands(board, pocketcards | opphand, 5, (5.0 / (ourcards.Length * oppcards.Length))))
                        {
                            ourbest = Evaluate(pocketcards | handmask, 7);
                            oppbest = Evaluate(opphand | handmask, 7);
                            if (ourbest > oppbest)
                            {
                                player[(uint)HandType(ourbest)] += 1.0;
                                count++;
                            }
                            else if (ourbest == oppbest)
                            {
                                player[(uint)HandType(ourbest)] += 0.5;
                                opponent[(uint)HandType(oppbest)] += 0.5;
                                count++;
                            }
                            else
                            {
                                opponent[(uint)HandType(oppbest)] += 1.0;
                                count++;
                            }
                        }
                    }
                }
                for (int i = 0; i < 9; i++)
                {
                    player[i] = player[i] / count;
                    opponent[i] = opponent[i] / count;
                }

                return true;
            }
            else
            {
                // Calculate results
                foreach (ulong pocketcards in ourcards)
                {
                    if ((pocketcards & board) != 0UL) continue;

                    foreach (ulong opphand in oppcards)
                    {
                        if (((opphand & pocketcards) != 0UL) || (opphand & board) != 0UL) continue;

                        foreach (ulong handmask in Hands(board, pocketcards | opphand, 5))
                        {
                            ourbest = Evaluate(pocketcards | handmask, 7);
                            oppbest = Evaluate(opphand | handmask, 7);
                            if (ourbest > oppbest)
                            {
                                player[(uint)HandType(ourbest)] += 1.0;
                                count++;
                            }
                            else if (ourbest == oppbest)
                            {
                                player[(uint)HandType(ourbest)] += 0.5;
                                opponent[(uint)HandType(oppbest)] += 0.5;
                                count++;
                            }
                            else
                            {
                                opponent[(uint)HandType(oppbest)] += 1.0;
                                count++;
                            }
                        }
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    player[i] = player[i] / count;
                    opponent[i] = opponent[i] / count;
                }

                return false;
            }
        }

        /// <summary>
        /// Given a set of pocket cards and a set of board cards this function returns the odds of winning or tying for a player and a random opponent.
        /// </summary>
        /// <param name="ourcards">Pocket mask for the mask.</param>
        /// <param name="board">Board mask for mask</param>
        /// <param name="player">Player odds as doubles</param>
        /// <param name="opponent">Opponent odds as doubles</param>
        public static void HandWinOdds(ulong ourcards, ulong board, out double[] player, out double[] opponent)
        {
            uint ourbest, oppbest;
            int count = 0;
            int cards = BitCount(ourcards | board);
            int boardcount = BitCount(board);

            double[] podds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] oodds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
#if DEBUG
            // Preconditions
            if (BitCount(ourcards) != 2) throw new ArgumentOutOfRangeException("pocketcards");
            if (boardcount > 5) throw new ArgumentOutOfRangeException("boardcards");
            //if (player.Length != opponent.Length || player.Length != 9) throw new ArgumentOutOfRangeException();
#endif
            // Use precalcuated results for pocket cards
            if (boardcount == 0)
            {
                int index = (int)Hand.PocketHand169Type(ourcards);
                player = Hand.PreCalcPlayerOdds[index];
                opponent = Hand.PreCalcOppOdds[index];
                return;
            }

            player = podds;
            opponent = oodds;

            // Calculate results
            foreach (ulong oppcards in Hands(0UL, ourcards | board, 2))
            {
                foreach (ulong handmask in Hands(board, ourcards | oppcards, 5))
                {
                    ourbest = Evaluate(ourcards | handmask, 7);
                    oppbest = Evaluate(oppcards | handmask, 7);
                    if (ourbest > oppbest)
                    {
                        player[(uint)HandType(ourbest)] += 1.0;
                        count++;
                    }
                    else if (ourbest == oppbest)
                    {
                        player[(uint)HandType(ourbest)] += 0.5;
                        opponent[(uint)HandType(oppbest)] += 0.5;
                        count++;
                    }
                    else
                    {
                        opponent[(uint)HandType(oppbest)] += 1.0;
                        count++;
                    }
                }
            }

            for (int i = 0; i < 9; i++)
            {
                player[i] = player[i] / count;
                opponent[i] = opponent[i] / count;
            }
        }

        /// <summary>
        /// Given a set of pocket cards and a set of board cards this function returns the odds of winning or tying for a player and a random opponent.
        /// </summary>
        /// <param name="pocketcards">Pocket cards in ASCII</param>
        /// <param name="boardcards">Board cards in ASCII</param>
        /// <param name="player">Player odds as doubles</param>
        /// <param name="opponent">Opponent odds as doubles</param>
        public static void HandWinOdds(string pocketcards, string boardcards, out double[] player, out double[] opponent)
        {
            HandWinOdds(Hand.ParseHand(pocketcards), Hand.ParseHand(boardcards), out player, out opponent);
        }

        /// <summary>
        /// This method calculates the probablity of a player winning with specific hands and 
        /// opponents winning with specific hands.
        /// </summary>
        /// <param name="ourcards">pocket card mask</param>
        /// <param name="board">board cards mask</param>
        /// <param name="player">player win odds</param>
        /// <param name="opponent">opponent(s) win odds</param>
        /// <param name="NOpponents">The number of opponents</param>
        /// <param name="duration">The amount of time in seconds to calculate samples</param>
        public static void HandWinOdds(ulong ourcards, ulong board, out double[] player, out double[] opponent, int NOpponents, double duration)
        {
            long count = 0;

            double[] podds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] oodds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            Random rand = new Random();

            player = podds;
            opponent = oodds;

            switch (NOpponents)
            {
                case 1:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong oppcards = Hand.RandomHand(rand, 0UL, boardmask | ourcards, 2);
                        uint playerHandVal = Evaluate(ourcards | boardmask, 7);
                        uint oppHandVal = Evaluate(oppcards | boardmask, 7);

                        if (playerHandVal > oppHandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal == oppHandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(oppHandVal)] += 0.5;
                        }
                        else
                        {
                            opponent[HandType(oppHandVal)] += 1.0;
                        }

                        count++;
                    }
                    break;
                case 2:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards, 2);
                        uint playerHandVal = Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal > opp2HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 3:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards, 2);
                        uint playerHandVal = Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal
                            && playerHandVal > opp3HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 4:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards, 2);
                        uint playerHandVal = Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal
                            && playerHandVal > opp3HandVal && playerHandVal > opp4HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal &&
                                opp2HandVal >= opp4HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                                    opp3HandVal >= opp4HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                               opp4HandVal >= opp3HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 5:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        uint playerHandVal = Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);
                        uint opp5HandVal = Evaluate(opp5cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal
                            && playerHandVal > opp3HandVal && playerHandVal > opp4HandVal &&
                            playerHandVal > opp5HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal && opp1HandVal >= opp5HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal &&
                                opp2HandVal >= opp4HandVal && opp2HandVal >= opp5HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                                opp3HandVal >= opp4HandVal && opp3HandVal >= opp5HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                                opp4HandVal >= opp3HandVal && opp4HandVal >= opp5HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                            else if (opp5HandVal >= opp1HandVal && opp5HandVal >= opp2HandVal &&
                                opp5HandVal >= opp3HandVal && opp5HandVal >= opp4HandVal)
                            {
                                opponent[HandType(opp5HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 6:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        uint playerHandVal = Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);
                        uint opp5HandVal = Evaluate(opp5cards | boardmask, 7);
                        uint opp6HandVal = Evaluate(opp6cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal
                            && playerHandVal > opp3HandVal && playerHandVal > opp4HandVal &&
                            playerHandVal > opp5HandVal && playerHandVal > opp6HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal && opp1HandVal >= opp5HandVal &&
                                opp1HandVal >= opp6HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal > opp1HandVal && opp2HandVal > opp3HandVal &&
                                opp2HandVal > opp4HandVal && opp2HandVal > opp5HandVal &&
                                opp2HandVal > opp6HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                                opp3HandVal >= opp4HandVal && opp3HandVal >= opp5HandVal &&
                                opp3HandVal >= opp6HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                                opp4HandVal >= opp3HandVal && opp4HandVal >= opp5HandVal &&
                                opp4HandVal >= opp6HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                            else if (opp5HandVal >= opp1HandVal && opp5HandVal >= opp2HandVal &&
                               opp5HandVal >= opp3HandVal && opp5HandVal >= opp4HandVal &&
                               opp5HandVal >= opp6HandVal)
                            {
                                opponent[HandType(opp5HandVal)] += 1.0;
                            }
                            else if (opp6HandVal >= opp2HandVal && opp6HandVal >= opp2HandVal &&
                               opp6HandVal >= opp3HandVal && opp6HandVal >= opp4HandVal &&
                               opp6HandVal >= opp5HandVal)
                            {
                                opponent[HandType(opp6HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 7:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        uint playerHandVal = Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);
                        uint opp5HandVal = Evaluate(opp5cards | boardmask, 7);
                        uint opp6HandVal = Evaluate(opp6cards | boardmask, 7);
                        uint opp7HandVal = Evaluate(opp7cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal &&
                            playerHandVal > opp3HandVal && playerHandVal > opp4HandVal &&
                            playerHandVal > opp5HandVal && playerHandVal > opp6HandVal &&
                            playerHandVal > opp7HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal &&
                            playerHandVal >= opp7HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal && opp1HandVal >= opp5HandVal &&
                                opp1HandVal >= opp6HandVal && opp1HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal &&
                                opp2HandVal >= opp4HandVal && opp2HandVal >= opp5HandVal &&
                                opp2HandVal >= opp6HandVal && opp2HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                                opp3HandVal >= opp4HandVal && opp3HandVal >= opp5HandVal &&
                                opp3HandVal >= opp6HandVal && opp3HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                                opp4HandVal >= opp3HandVal && opp4HandVal >= opp5HandVal &&
                                opp4HandVal >= opp6HandVal && opp4HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                            else if (opp5HandVal >= opp1HandVal && opp5HandVal >= opp2HandVal &&
                               opp5HandVal >= opp3HandVal && opp5HandVal >= opp4HandVal &&
                               opp5HandVal >= opp6HandVal && opp5HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp5HandVal)] += 1.0;
                            }
                            else if (opp6HandVal >= opp1HandVal && opp6HandVal >= opp2HandVal &&
                               opp6HandVal >= opp3HandVal && opp6HandVal >= opp4HandVal &&
                               opp6HandVal >= opp5HandVal && opp6HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp6HandVal)] += 1.0;
                            }
                            else if (opp7HandVal >= opp1HandVal && opp7HandVal >= opp2HandVal &&
                               opp7HandVal >= opp3HandVal && opp7HandVal >= opp4HandVal &&
                               opp7HandVal >= opp5HandVal && opp7HandVal >= opp6HandVal)
                            {
                                opponent[HandType(opp7HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 8:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        ulong opp8cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards, 2);
                        uint playerHandVal = Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);
                        uint opp5HandVal = Evaluate(opp5cards | boardmask, 7);
                        uint opp6HandVal = Evaluate(opp6cards | boardmask, 7);
                        uint opp7HandVal = Evaluate(opp7cards | boardmask, 7);
                        uint opp8HandVal = Evaluate(opp8cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal &&
                            playerHandVal > opp3HandVal && playerHandVal > opp4HandVal &&
                            playerHandVal > opp5HandVal && playerHandVal > opp6HandVal &&
                            playerHandVal > opp7HandVal && playerHandVal > opp8HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal &&
                            playerHandVal >= opp7HandVal && playerHandVal >= opp8HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal && opp1HandVal >= opp5HandVal &&
                                opp1HandVal >= opp6HandVal && opp1HandVal >= opp7HandVal &&
                                opp1HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal &&
                                opp2HandVal >= opp4HandVal && opp2HandVal >= opp5HandVal &&
                                opp2HandVal >= opp6HandVal && opp2HandVal >= opp7HandVal &&
                                opp2HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                               opp3HandVal >= opp4HandVal && opp3HandVal >= opp5HandVal &&
                               opp3HandVal >= opp6HandVal && opp3HandVal >= opp7HandVal &&
                               opp3HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                               opp4HandVal >= opp3HandVal && opp4HandVal >= opp5HandVal &&
                               opp4HandVal >= opp6HandVal && opp4HandVal >= opp7HandVal &&
                               opp4HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                            else if (opp5HandVal >= opp1HandVal && opp5HandVal >= opp2HandVal &&
                               opp5HandVal >= opp3HandVal && opp5HandVal >= opp4HandVal &&
                               opp5HandVal >= opp6HandVal && opp5HandVal >= opp7HandVal &&
                               opp5HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp5HandVal)] += 1.0;
                            }
                            else if (opp6HandVal >= opp1HandVal && opp6HandVal >= opp2HandVal &&
                               opp6HandVal >= opp3HandVal && opp6HandVal >= opp4HandVal &&
                               opp6HandVal >= opp5HandVal && opp6HandVal >= opp7HandVal &&
                               opp6HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp6HandVal)] += 1.0;
                            }
                            else if (opp7HandVal >= opp1HandVal && opp7HandVal >= opp2HandVal &&
                                  opp7HandVal >= opp3HandVal && opp7HandVal >= opp4HandVal &&
                                  opp7HandVal >= opp5HandVal && opp7HandVal >= opp6HandVal &&
                                  opp7HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp7HandVal)] += 1.0;
                            }
                            else if (opp8HandVal >= opp1HandVal && opp8HandVal >= opp2HandVal &&
                                 opp8HandVal >= opp3HandVal && opp8HandVal >= opp4HandVal &&
                                 opp8HandVal >= opp5HandVal && opp8HandVal >= opp6HandVal &&
                                 opp8HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp8HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 9:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        ulong opp8cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards, 2);
                        ulong opp9cards = Hand.RandomHand(rand, 0UL, boardmask | ourcards | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards | opp8cards, 2);
                        uint playerHandVal = Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);
                        uint opp5HandVal = Evaluate(opp5cards | boardmask, 7);
                        uint opp6HandVal = Evaluate(opp6cards | boardmask, 7);
                        uint opp7HandVal = Evaluate(opp7cards | boardmask, 7);
                        uint opp8HandVal = Evaluate(opp8cards | boardmask, 7);
                        uint opp9HandVal = Evaluate(opp9cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal &&
                            playerHandVal > opp3HandVal && playerHandVal > opp4HandVal &&
                            playerHandVal > opp5HandVal && playerHandVal > opp6HandVal &&
                            playerHandVal > opp7HandVal && playerHandVal > opp8HandVal &&
                            playerHandVal > opp9HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal &&
                            playerHandVal >= opp7HandVal && playerHandVal >= opp8HandVal &&
                            playerHandVal >= opp9HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal && opp1HandVal >= opp5HandVal &&
                                opp1HandVal >= opp6HandVal && opp1HandVal >= opp7HandVal &&
                                opp1HandVal >= opp8HandVal && opp1HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal &&
                                opp2HandVal >= opp4HandVal && opp2HandVal >= opp5HandVal &&
                                opp2HandVal >= opp6HandVal && opp2HandVal >= opp7HandVal &&
                                opp2HandVal >= opp8HandVal && opp2HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                               opp3HandVal >= opp4HandVal && opp3HandVal >= opp5HandVal &&
                               opp3HandVal >= opp6HandVal && opp3HandVal >= opp7HandVal &&
                               opp3HandVal >= opp8HandVal && opp3HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                              opp4HandVal >= opp3HandVal && opp4HandVal >= opp5HandVal &&
                              opp4HandVal >= opp6HandVal && opp4HandVal >= opp7HandVal &&
                              opp4HandVal >= opp8HandVal && opp4HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                            else if (opp5HandVal >= opp1HandVal && opp5HandVal >= opp2HandVal &&
                              opp5HandVal >= opp3HandVal && opp5HandVal >= opp4HandVal &&
                              opp5HandVal >= opp6HandVal && opp5HandVal >= opp7HandVal &&
                              opp5HandVal >= opp8HandVal && opp5HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp5HandVal)] += 1.0;
                            }
                            else if (opp6HandVal >= opp1HandVal && opp6HandVal >= opp2HandVal &&
                                 opp6HandVal >= opp3HandVal && opp6HandVal >= opp4HandVal &&
                                 opp6HandVal >= opp5HandVal && opp6HandVal >= opp7HandVal &&
                                 opp6HandVal >= opp8HandVal && opp6HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp6HandVal)] += 1.0;
                            }
                            else if (opp7HandVal >= opp1HandVal && opp7HandVal >= opp2HandVal &&
                                opp7HandVal >= opp3HandVal && opp7HandVal >= opp4HandVal &&
                                opp7HandVal >= opp5HandVal && opp7HandVal >= opp6HandVal &&
                                opp7HandVal >= opp8HandVal && opp7HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp7HandVal)] += 1.0;
                            }
                            else if (opp8HandVal >= opp1HandVal && opp8HandVal >= opp2HandVal &&
                               opp8HandVal >= opp3HandVal && opp8HandVal >= opp4HandVal &&
                               opp8HandVal >= opp5HandVal && opp8HandVal >= opp6HandVal &&
                               opp8HandVal >= opp7HandVal && opp8HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp8HandVal)] += 1.0;
                            }
                            else if (opp9HandVal >= opp1HandVal && opp9HandVal >= opp2HandVal &&
                              opp9HandVal >= opp3HandVal && opp9HandVal >= opp4HandVal &&
                              opp9HandVal >= opp5HandVal && opp9HandVal >= opp6HandVal &&
                              opp9HandVal >= opp7HandVal && opp9HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp9HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
            }

            for (int i = 0; i < player.Length; i++)
            {
                player[i] /= (double)count;
                opponent[i] /= (double)count;
            }
        }

        /// <summary>
        /// This method calculates the probablity of a player winning with specific hands and 
        /// opponents winning with specific hands.
        /// </summary>
        /// <param name="ourcards">pocket cards</param>
        /// <param name="board">board cards</param>
        /// <param name="player">player win odds</param>
        /// <param name="opponent">opponent(s) win odds</param>
        /// <param name="NOpponents">The number of opponents</param>
        /// <param name="duration">The amount of time in seconds to calculate samples</param>
        public static void HandWinOdds(string ourcards, string board, out double[] player, out double[] opponent, int NOpponents, double duration)
        {
            long count = 0;
            ulong[] list = PocketHands.Query(ourcards);
            ulong boardhand = ParseHand(board);
            Random rand = new Random();
            double starttime = CurrentTime;

            double[] podds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            double[] oodds = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

            player = podds;
            opponent = oodds;

            switch (NOpponents)
            {
                case 1:
                    while ((CurrentTime - starttime) < duration) {
                        ulong pocketmask = RandomHand(rand, list, boardhand, 2);
                        ulong boardmask = Hand.RandomHand(rand, boardhand, pocketmask, 5);
                        ulong oppcards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask, 2);
                        uint playerHandVal = Evaluate(pocketmask | boardmask, 7);
                        uint oppHandVal = Evaluate(oppcards | boardmask, 7);

                        if (playerHandVal > oppHandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal == oppHandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(oppHandVal)] += 0.5;
                        }
                        else
                        {
                            opponent[HandType(oppHandVal)] += 1.0;
                        }

                        count++;
                    }
                    break;
                case 2:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, boardhand, 2);
                        ulong boardmask = Hand.RandomHand(rand, boardhand, pocketmask, 5);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards, 2);
                        uint playerHandVal = Evaluate(pocketmask | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal > opp2HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 3:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, boardhand, 2);
                        ulong boardmask = Hand.RandomHand(rand, boardhand, pocketmask, 5);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards, 2);
                        uint playerHandVal = Evaluate(pocketmask | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal
                            && playerHandVal > opp3HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 4:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, boardhand, 2);
                        ulong boardmask = Hand.RandomHand(rand, boardhand, pocketmask, 5);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards, 2);
                        uint playerHandVal = Evaluate(pocketmask | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal
                            && playerHandVal > opp3HandVal && playerHandVal > opp4HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal &&
                                opp2HandVal >= opp4HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                                    opp3HandVal >= opp4HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                               opp4HandVal >= opp3HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 5:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, boardhand, 2);
                        ulong boardmask = Hand.RandomHand(rand, boardhand, pocketmask, 5);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        uint playerHandVal = Evaluate(pocketmask | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);
                        uint opp5HandVal = Evaluate(opp5cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal
                            && playerHandVal > opp3HandVal && playerHandVal > opp4HandVal &&
                            playerHandVal > opp5HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal && opp1HandVal >= opp5HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal &&
                                opp2HandVal >= opp4HandVal && opp2HandVal >= opp5HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                                opp3HandVal >= opp4HandVal && opp3HandVal >= opp5HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                                opp4HandVal >= opp3HandVal && opp4HandVal >= opp5HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                            else if (opp5HandVal >= opp1HandVal && opp5HandVal >= opp2HandVal &&
                                opp5HandVal >= opp3HandVal && opp5HandVal >= opp4HandVal)
                            {
                                opponent[HandType(opp5HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 6:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, boardhand, 2);
                        ulong boardmask = Hand.RandomHand(rand, boardhand, pocketmask, 5);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        uint playerHandVal = Evaluate(pocketmask | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);
                        uint opp5HandVal = Evaluate(opp5cards | boardmask, 7);
                        uint opp6HandVal = Evaluate(opp6cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal
                            && playerHandVal > opp3HandVal && playerHandVal > opp4HandVal &&
                            playerHandVal > opp5HandVal && playerHandVal > opp6HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal && opp1HandVal >= opp5HandVal &&
                                opp1HandVal >= opp6HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal > opp1HandVal && opp2HandVal > opp3HandVal &&
                                opp2HandVal > opp4HandVal && opp2HandVal > opp5HandVal &&
                                opp2HandVal > opp6HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                                opp3HandVal >= opp4HandVal && opp3HandVal >= opp5HandVal &&
                                opp3HandVal >= opp6HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                                opp4HandVal >= opp3HandVal && opp4HandVal >= opp5HandVal &&
                                opp4HandVal >= opp6HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                            else if (opp5HandVal >= opp1HandVal && opp5HandVal >= opp2HandVal &&
                               opp5HandVal >= opp3HandVal && opp5HandVal >= opp4HandVal &&
                               opp5HandVal >= opp6HandVal)
                            {
                                opponent[HandType(opp5HandVal)] += 1.0;
                            }
                            else if (opp6HandVal >= opp2HandVal && opp6HandVal >= opp2HandVal &&
                               opp6HandVal >= opp3HandVal && opp6HandVal >= opp4HandVal &&
                               opp6HandVal >= opp5HandVal)
                            {
                                opponent[HandType(opp6HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 7:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, boardhand, 2);
                        ulong boardmask = Hand.RandomHand(rand, boardhand, pocketmask, 5);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        uint playerHandVal = Evaluate(pocketmask | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);
                        uint opp5HandVal = Evaluate(opp5cards | boardmask, 7);
                        uint opp6HandVal = Evaluate(opp6cards | boardmask, 7);
                        uint opp7HandVal = Evaluate(opp7cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal &&
                            playerHandVal > opp3HandVal && playerHandVal > opp4HandVal &&
                            playerHandVal > opp5HandVal && playerHandVal > opp6HandVal &&
                            playerHandVal > opp7HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal &&
                            playerHandVal >= opp7HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal && opp1HandVal >= opp5HandVal &&
                                opp1HandVal >= opp6HandVal && opp1HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal &&
                                opp2HandVal >= opp4HandVal && opp2HandVal >= opp5HandVal &&
                                opp2HandVal >= opp6HandVal && opp2HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                                opp3HandVal >= opp4HandVal && opp3HandVal >= opp5HandVal &&
                                opp3HandVal >= opp6HandVal && opp3HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                                opp4HandVal >= opp3HandVal && opp4HandVal >= opp5HandVal &&
                                opp4HandVal >= opp6HandVal && opp4HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                            else if (opp5HandVal >= opp1HandVal && opp5HandVal >= opp2HandVal &&
                               opp5HandVal >= opp3HandVal && opp5HandVal >= opp4HandVal &&
                               opp5HandVal >= opp6HandVal && opp5HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp5HandVal)] += 1.0;
                            }
                            else if (opp6HandVal >= opp1HandVal && opp6HandVal >= opp2HandVal &&
                               opp6HandVal >= opp3HandVal && opp6HandVal >= opp4HandVal &&
                               opp6HandVal >= opp5HandVal && opp6HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp6HandVal)] += 1.0;
                            }
                            else if (opp7HandVal >= opp1HandVal && opp7HandVal >= opp2HandVal &&
                               opp7HandVal >= opp3HandVal && opp7HandVal >= opp4HandVal &&
                               opp7HandVal >= opp5HandVal && opp7HandVal >= opp6HandVal)
                            {
                                opponent[HandType(opp7HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 8:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, boardhand, 2);
                        ulong boardmask = Hand.RandomHand(rand, boardhand, pocketmask, 5);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        ulong opp8cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards, 2);
                        uint playerHandVal = Evaluate(pocketmask | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);
                        uint opp5HandVal = Evaluate(opp5cards | boardmask, 7);
                        uint opp6HandVal = Evaluate(opp6cards | boardmask, 7);
                        uint opp7HandVal = Evaluate(opp7cards | boardmask, 7);
                        uint opp8HandVal = Evaluate(opp8cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal &&
                            playerHandVal > opp3HandVal && playerHandVal > opp4HandVal &&
                            playerHandVal > opp5HandVal && playerHandVal > opp6HandVal &&
                            playerHandVal > opp7HandVal && playerHandVal > opp8HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal &&
                            playerHandVal >= opp7HandVal && playerHandVal >= opp8HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal && opp1HandVal >= opp5HandVal &&
                                opp1HandVal >= opp6HandVal && opp1HandVal >= opp7HandVal &&
                                opp1HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal &&
                                opp2HandVal >= opp4HandVal && opp2HandVal >= opp5HandVal &&
                                opp2HandVal >= opp6HandVal && opp2HandVal >= opp7HandVal &&
                                opp2HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                               opp3HandVal >= opp4HandVal && opp3HandVal >= opp5HandVal &&
                               opp3HandVal >= opp6HandVal && opp3HandVal >= opp7HandVal &&
                               opp3HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                               opp4HandVal >= opp3HandVal && opp4HandVal >= opp5HandVal &&
                               opp4HandVal >= opp6HandVal && opp4HandVal >= opp7HandVal &&
                               opp4HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                            else if (opp5HandVal >= opp1HandVal && opp5HandVal >= opp2HandVal &&
                               opp5HandVal >= opp3HandVal && opp5HandVal >= opp4HandVal &&
                               opp5HandVal >= opp6HandVal && opp5HandVal >= opp7HandVal &&
                               opp5HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp5HandVal)] += 1.0;
                            }
                            else if (opp6HandVal >= opp1HandVal && opp6HandVal >= opp2HandVal &&
                               opp6HandVal >= opp3HandVal && opp6HandVal >= opp4HandVal &&
                               opp6HandVal >= opp5HandVal && opp6HandVal >= opp7HandVal &&
                               opp6HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp6HandVal)] += 1.0;
                            }
                            else if (opp7HandVal >= opp1HandVal && opp7HandVal >= opp2HandVal &&
                                  opp7HandVal >= opp3HandVal && opp7HandVal >= opp4HandVal &&
                                  opp7HandVal >= opp5HandVal && opp7HandVal >= opp6HandVal &&
                                  opp7HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp7HandVal)] += 1.0;
                            }
                            else if (opp8HandVal >= opp1HandVal && opp8HandVal >= opp2HandVal &&
                                 opp8HandVal >= opp3HandVal && opp8HandVal >= opp4HandVal &&
                                 opp8HandVal >= opp5HandVal && opp8HandVal >= opp6HandVal &&
                                 opp8HandVal >= opp7HandVal)
                            {
                                opponent[HandType(opp8HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
                case 9:
                    while ((CurrentTime - starttime) < duration)
                    {
                        ulong pocketmask = RandomHand(rand, list, boardhand, 2);
                        ulong boardmask = Hand.RandomHand(rand, boardhand, pocketmask, 5);
                        ulong opp1cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask, 2);
                        ulong opp2cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards, 2);
                        ulong opp3cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards, 2);
                        ulong opp4cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards, 2);
                        ulong opp5cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards, 2);
                        ulong opp6cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards, 2);
                        ulong opp7cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards, 2);
                        ulong opp8cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards, 2);
                        ulong opp9cards = Hand.RandomHand(rand, 0UL, boardmask | pocketmask | opp1cards | opp2cards | opp3cards | opp4cards | opp5cards | opp6cards | opp7cards | opp8cards, 2);
                        uint playerHandVal = Evaluate(pocketmask | boardmask, 7);
                        uint opp1HandVal = Evaluate(opp1cards | boardmask, 7);
                        uint opp2HandVal = Evaluate(opp2cards | boardmask, 7);
                        uint opp3HandVal = Evaluate(opp3cards | boardmask, 7);
                        uint opp4HandVal = Evaluate(opp4cards | boardmask, 7);
                        uint opp5HandVal = Evaluate(opp5cards | boardmask, 7);
                        uint opp6HandVal = Evaluate(opp6cards | boardmask, 7);
                        uint opp7HandVal = Evaluate(opp7cards | boardmask, 7);
                        uint opp8HandVal = Evaluate(opp8cards | boardmask, 7);
                        uint opp9HandVal = Evaluate(opp9cards | boardmask, 7);

                        if (playerHandVal > opp1HandVal && playerHandVal > opp2HandVal &&
                            playerHandVal > opp3HandVal && playerHandVal > opp4HandVal &&
                            playerHandVal > opp5HandVal && playerHandVal > opp6HandVal &&
                            playerHandVal > opp7HandVal && playerHandVal > opp8HandVal &&
                            playerHandVal > opp9HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal &&
                            playerHandVal >= opp7HandVal && playerHandVal >= opp8HandVal &&
                            playerHandVal >= opp9HandVal)
                        {
                            player[HandType(playerHandVal)] += 0.5;
                            opponent[HandType(playerHandVal)] += 0.5;
                        }
                        else
                        {
                            if (opp1HandVal >= opp2HandVal && opp1HandVal >= opp3HandVal &&
                                opp1HandVal >= opp4HandVal && opp1HandVal >= opp5HandVal &&
                                opp1HandVal >= opp6HandVal && opp1HandVal >= opp7HandVal &&
                                opp1HandVal >= opp8HandVal && opp1HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp1HandVal)] += 1.0;
                            }
                            else if (opp2HandVal >= opp1HandVal && opp2HandVal >= opp3HandVal &&
                                opp2HandVal >= opp4HandVal && opp2HandVal >= opp5HandVal &&
                                opp2HandVal >= opp6HandVal && opp2HandVal >= opp7HandVal &&
                                opp2HandVal >= opp8HandVal && opp2HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp2HandVal)] += 1.0;
                            }
                            else if (opp3HandVal >= opp1HandVal && opp3HandVal >= opp2HandVal &&
                               opp3HandVal >= opp4HandVal && opp3HandVal >= opp5HandVal &&
                               opp3HandVal >= opp6HandVal && opp3HandVal >= opp7HandVal &&
                               opp3HandVal >= opp8HandVal && opp3HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp3HandVal)] += 1.0;
                            }
                            else if (opp4HandVal >= opp1HandVal && opp4HandVal >= opp2HandVal &&
                              opp4HandVal >= opp3HandVal && opp4HandVal >= opp5HandVal &&
                              opp4HandVal >= opp6HandVal && opp4HandVal >= opp7HandVal &&
                              opp4HandVal >= opp8HandVal && opp4HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp4HandVal)] += 1.0;
                            }
                            else if (opp5HandVal >= opp1HandVal && opp5HandVal >= opp2HandVal &&
                              opp5HandVal >= opp3HandVal && opp5HandVal >= opp4HandVal &&
                              opp5HandVal >= opp6HandVal && opp5HandVal >= opp7HandVal &&
                              opp5HandVal >= opp8HandVal && opp5HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp5HandVal)] += 1.0;
                            }
                            else if (opp6HandVal >= opp1HandVal && opp6HandVal >= opp2HandVal &&
                                 opp6HandVal >= opp3HandVal && opp6HandVal >= opp4HandVal &&
                                 opp6HandVal >= opp5HandVal && opp6HandVal >= opp7HandVal &&
                                 opp6HandVal >= opp8HandVal && opp6HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp6HandVal)] += 1.0;
                            }
                            else if (opp7HandVal >= opp1HandVal && opp7HandVal >= opp2HandVal &&
                                opp7HandVal >= opp3HandVal && opp7HandVal >= opp4HandVal &&
                                opp7HandVal >= opp5HandVal && opp7HandVal >= opp6HandVal &&
                                opp7HandVal >= opp8HandVal && opp7HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp7HandVal)] += 1.0;
                            }
                            else if (opp8HandVal >= opp1HandVal && opp8HandVal >= opp2HandVal &&
                               opp8HandVal >= opp3HandVal && opp8HandVal >= opp4HandVal &&
                               opp8HandVal >= opp5HandVal && opp8HandVal >= opp6HandVal &&
                               opp8HandVal >= opp7HandVal && opp8HandVal >= opp9HandVal)
                            {
                                opponent[HandType(opp8HandVal)] += 1.0;
                            }
                            else if (opp9HandVal >= opp1HandVal && opp9HandVal >= opp2HandVal &&
                              opp9HandVal >= opp3HandVal && opp9HandVal >= opp4HandVal &&
                              opp9HandVal >= opp5HandVal && opp9HandVal >= opp6HandVal &&
                              opp9HandVal >= opp7HandVal && opp9HandVal >= opp8HandVal)
                            {
                                opponent[HandType(opp9HandVal)] += 1.0;
                            }
                        }
                        count++;
                    }
                    break;
            }

            for (int i = 0; i < player.Length; i++)
            {
                player[i] /= (double)count;
                opponent[i] /= (double)count;
            }
        }

        /// <summary>
        /// Used to calculate the wining information about each players mask. This function enumerates all 
        /// possible remaining hands and tallies win, tie and losses for each player. This function typically takes
        /// well less than a second regardless of the number of players.
        /// </summary>
        /// <param name="pockets">Array of pocket mask string, one for each player</param>
        /// <param name="board">the board cards</param>
        /// <param name="dead">the dead cards</param>
        /// <param name="wins">An array of win tallies, one for each player</param>
        /// <param name="ties">An array of tie tallies, one for each player</param>
        /// <param name="losses">An array of losses tallies, one for each player</param>
        /// <param name="totalHands">The total number of hands enumarated.</param>
        public static void HandWinOdds(string[] pockets, string board, string dead, long[] wins, long[] ties, long[] losses, ref long totalHands)
        {
            ulong[] pocketmasks = new ulong[pockets.Length];
            ulong[] pockethands = new ulong[pockets.Length];
            int count = 0, bestcount;
            ulong boardmask = 0UL, deadcards_mask = 0UL, deadcards = Hand.ParseHand(dead, ref count);

            totalHands = 0;
            deadcards_mask |= deadcards;

            // Read pocket cards
            for (int i = 0; i < pockets.Length; i++)
            {
                count = 0;
                pocketmasks[i] = Hand.ParseHand(pockets[i], "", ref count);
                if (count != 2)
                    throw new ArgumentException("There must be two pocket cards."); // Must have 2 cards in each pocket card set.
                deadcards_mask |= pocketmasks[i];
                wins[i] = ties[i] = losses[i] = 0;
            }

            // Read board cards
            count = 0;
            boardmask = Hand.ParseHand("", board, ref count);


#if DEBUG
            Debug.Assert(count >= 0 && count <= 5); // The board must have zero or more cards but no more than a total of 5

            // Check pocket cards, board, and dead cards for duplicates
            if ((boardmask & deadcards) != 0)
                throw new ArgumentException("Duplicate between cards dead cards and board");

            // Validate the input
            for (int i = 0; i < pockets.Length; i++)
            {
                for (int j = i + 1; j < pockets.Length; j++)
                {
                    if ((pocketmasks[i] & pocketmasks[j]) != 0)
                        throw new ArgumentException("Duplicate pocket cards");
                }

                if ((pocketmasks[i] & boardmask) != 0)
                    throw new ArgumentException("Duplicate between cards pocket and board");

                if ((pocketmasks[i] & deadcards) != 0)
                    throw new ArgumentException("Duplicate between cards pocket and dead cards");
            }
#endif

            // Iterate through all board possiblities that doesn't include any pocket cards.
            foreach (ulong boardhand in Hands(boardmask, deadcards_mask, 5))
            {
                // Evaluate all hands and determine the best mask
                ulong bestpocket = Evaluate(pocketmasks[0] | boardhand, 7);
                pockethands[0] = bestpocket;
                bestcount = 1;
                for (int i = 1; i < pockets.Length; i++)
                {
                    pockethands[i] = Evaluate(pocketmasks[i] | boardhand, 7);
                    if (pockethands[i] > bestpocket)
                    {
                        bestpocket = pockethands[i];
                        bestcount = 1;
                    }
                    else if (pockethands[i] == bestpocket)
                    {
                        bestcount++;
                    }
                }

                // Calculate wins/ties/loses for each pocket + board combination.
                for (int i = 0; i < pockets.Length; i++)
                {
                    if (pockethands[i] == bestpocket)
                    {
                        if (bestcount > 1)
                        {
                            ties[i]++;
                        }
                        else
                        {
                            wins[i]++;
                        }
                    }
                    else if (pockethands[i] < bestpocket)
                    {
                        losses[i]++;
                    }
                }

                totalHands++;
            }
        }
        #endregion

        #region Hand Potential
        /// <summary>
        /// Returns the normalized, positive and negative potential of the current mask. This funciton
        /// is described in Aaron Davidson's masters thesis on page 23.
        /// </summary>
        /// <param name="pocket">Hold Cards</param>
        /// <param name="board">Community cards</param>
        /// <param name="ppot">Positive Potential</param>
        /// <param name="npot">Negative Potential</param>
        public static void HandPotential(ulong pocket, ulong board, out double ppot, out double npot)
        {
            const int ahead = 2;
            const int tied = 1;
            const int behind = 0;

#if DEBUG
            if (BitCount(pocket) != 2)
                throw new ArgumentException("pocket must contain exactly two cards");
            if (BitCount(board) != 3 && BitCount(board) != 4)
                throw new ArgumentException("board must contain only 3 or 4 cards");
#endif

            int[,]  HP = new int[3, 3];
            int[]   HPTotal = new int[3];
            int     ncards = BitCount(pocket | board);
            double  mult = (ncards == 5 ? 990.0 : 45.0);
            uint    ourbest, oppbest;
#if DEBUG
            if (ncards < 5 || ncards > 7)
                throw new ArgumentOutOfRangeException();
#endif
            // Rank our mask
            uint ourrank = Evaluate(pocket | board, ncards);

            // Iterate through all possible opponent pocket cards
            foreach (ulong oppPocket in Hands(0UL, pocket | board, 2))
            {
                uint opprank = Evaluate(oppPocket | board, ncards);
                int index = (ourrank > opprank ? ahead : (ourrank == opprank ? tied : behind));
                foreach (ulong boardmask in Hands(board, pocket | oppPocket, 5))
                {
                    ourbest = Evaluate(pocket | boardmask, 7);
                    oppbest = Evaluate(oppPocket | boardmask, 7);
                    if (ourbest > oppbest)
                        HP[index, ahead]++;
                    else if (ourbest == oppbest)
                        HP[index, tied]++;
                    else
                        HP[index, behind]++;
                }
                HPTotal[index]++;
            }

            double den1 = (mult * (HPTotal[behind] + (HPTotal[tied] / 2.0)));
            double den2 = (mult * (HPTotal[ahead] + (HPTotal[tied] / 2.0)));
            if (den1 > 0)
                ppot = (HP[behind, ahead] + (HP[behind, tied] / 2) + (HP[tied, ahead] / 2)) / den1;
            else
                ppot = 0;
            if (den2 > 0)
                npot = (HP[ahead, behind] + (HP[ahead, tied] / 2) + (HP[tied, behind] / 2)) / den2;
            else
                npot = 0;
        }

        /// <summary>
        /// This method is similar to the HandPotential algorithm described in Aaron Davidson's
        /// masters thesis, however if differs in several significant ways. First, it makes the calculation
        /// while accounting for one or more opponents. Second, it uses the Monte Carlo technique to get 
        /// answers in a reasonable amount of time (a duration of 0.1 seems to give reasonable results). And
        /// finally, the results are not normalized; the positive and negative potential is the actual odds of improvement
        /// or worsening of a mask.
        /// </summary>
        /// <param name="pocket">Players pocket card mask</param>
        /// <param name="board">The current board mask</param>
        /// <param name="ppot">The resultant positive potential</param>
        /// <param name="npot">The resultant negative potential</param>
        /// <param name="NOpponents">The number of opponents</param>
        /// <param name="duration">The length of time (in seconds) to spend on this calculation</param>
        public static void HandPotential(ulong pocket, ulong board, out double ppot, out double npot, int NOpponents, double duration)
        {
            const int ahead = 2;
            const int tied = 1;
            const int behind = 0;
            Random rand = new Random();

#if DEBUG
            if (BitCount(pocket) != 2)
                throw new ArgumentException("pocket must contain exactly two cards");
            if (BitCount(board) != 3 && BitCount(board) != 4)
                throw new ArgumentException("board must contain only 3 or 4 cards");
#endif
            ppot = npot = 0.0;

            int[,] HP = new int[3, 3];
            int count = 0;
            int ncards = BitCount(pocket | board);
            
            uint ourbest;

            // Rank our mask
            uint ourrank = Evaluate(pocket | board);
            double start = CurrentTime;

            switch (NOpponents)
            {
                case 1:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board, ncards);
                        int index;

                        if (ourrank > opp1rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        if (ourbest > opp1best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 2:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 3:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                             //   count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //   count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 4:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 5:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        ulong opp5Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        uint opp5rank = Evaluate(opp5Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        uint opp5best = Evaluate(opp5Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best &&
                            ourbest > opp5best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best &&
                            ourbest >= opp5best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 6:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        ulong opp5Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 2);
                        ulong opp6Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        uint opp5rank = Evaluate(opp5Pocket | board);
                        uint opp6rank = Evaluate(opp6Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        uint opp5best = Evaluate(opp5Pocket | boardmask, 7);
                        uint opp6best = Evaluate(opp6Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best &&
                            ourbest > opp5best && ourbest > opp6best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best &&
                            ourbest >= opp5best && ourbest >= opp6best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //   count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 7:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        ulong opp5Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 2);
                        ulong opp6Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket, 2);
                        ulong opp7Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        uint opp5rank = Evaluate(opp5Pocket | board);
                        uint opp6rank = Evaluate(opp6Pocket | board);
                        uint opp7rank = Evaluate(opp7Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        uint opp5best = Evaluate(opp5Pocket | boardmask, 7);
                        uint opp6best = Evaluate(opp6Pocket | boardmask, 7);
                        uint opp7best = Evaluate(opp7Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best &&
                            ourbest > opp5best && ourbest > opp6best &&
                            ourbest > opp7best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best &&
                            ourbest >= opp5best && ourbest >= opp6best &&
                            ourbest >= opp7best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 8:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        ulong opp5Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 2);
                        ulong opp6Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket, 2);
                        ulong opp7Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket, 2);
                        ulong opp8Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        uint opp5rank = Evaluate(opp5Pocket | board);
                        uint opp6rank = Evaluate(opp6Pocket | board);
                        uint opp7rank = Evaluate(opp7Pocket | board);
                        uint opp8rank = Evaluate(opp8Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank && ourrank > opp8rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank && ourrank >= opp8rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket | opp8Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        uint opp5best = Evaluate(opp5Pocket | boardmask, 7);
                        uint opp6best = Evaluate(opp6Pocket | boardmask, 7);
                        uint opp7best = Evaluate(opp7Pocket | boardmask, 7);
                        uint opp8best = Evaluate(opp8Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best &&
                            ourbest > opp5best && ourbest > opp6best &&
                            ourbest > opp7best && ourbest > opp8best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best &&
                            ourbest >= opp5best && ourbest >= opp6best &&
                            ourbest >= opp7best && ourbest >= opp8best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 9:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        ulong opp5Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 2);
                        ulong opp6Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket, 2);
                        ulong opp7Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket, 2);
                        ulong opp8Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket, 2);
                        ulong opp9Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket | opp8Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        uint opp5rank = Evaluate(opp5Pocket | board);
                        uint opp6rank = Evaluate(opp6Pocket | board);
                        uint opp7rank = Evaluate(opp7Pocket | board);
                        uint opp8rank = Evaluate(opp8Pocket | board);
                        uint opp9rank = Evaluate(opp9Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank && ourrank > opp8rank &&
                            ourrank > opp9rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank && ourrank >= opp8rank &&
                            ourrank >= opp9rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket | opp8Pocket | opp9Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        uint opp5best = Evaluate(opp5Pocket | boardmask, 7);
                        uint opp6best = Evaluate(opp6Pocket | boardmask, 7);
                        uint opp7best = Evaluate(opp7Pocket | boardmask, 7);
                        uint opp8best = Evaluate(opp8Pocket | boardmask, 7);
                        uint opp9best = Evaluate(opp9Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best &&
                            ourbest > opp5best && ourbest > opp6best &&
                            ourbest > opp7best && ourbest > opp8best &&
                            ourbest > opp9best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best &&
                            ourbest >= opp5best && ourbest >= opp6best &&
                            ourbest >= opp7best && ourbest >= opp8best &&
                            ourbest >= opp9best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                
            }
            if (count != 0)
            {
                ppot = (HP[behind, ahead] + (HP[behind, tied] / 2.0) + (HP[tied, ahead] / 2.0)) / ((double)count);
                npot = (HP[ahead, behind] + (HP[ahead, tied] / 2.0) + (HP[tied, behind] / 2.0)) / ((double)count);
            }
        }
       
        /// <summary>
        /// This method is similar to the HandPotential algorithm described in Aaron Davidson's
        /// masters thesis, however if differs in several significant ways. First, it makes the calculation
        /// while accounting for one or more opponents. Second, it uses the Monte Carlo technique to get 
        /// answers in a reasonable amount of time (a duration of 0.1 seems to give reasonable results). And
        /// finally, the results are not normalized; the positive and negative potential is the actual odds of improvement
        /// or worsening of a mask.
        /// </summary>
        /// <param name="pockethand">Players pocket card query string</param>
        /// <param name="boardhand">The current board string</param>
        /// <param name="ppot">The resultant positive potential</param>
        /// <param name="npot">The resultant negative potential</param>
        /// <param name="NOpponents">The number of opponents</param>
        /// <param name="duration">The length of time (in seconds) to spend on this calculation</param>
        public static void HandPotential(string pockethand, string boardhand, out double ppot, out double npot, int NOpponents, double duration)
        {
            const int ahead = 2;
            const int tied = 1;
            const int behind = 0;

            ulong board = ParseHand(boardhand);
            ulong[] list = PocketHands.Query(pockethand);

#if DEBUG
            if (BitCount(board) != 3 && BitCount(board) != 4)
                throw new ArgumentException("board must contain only 3 or 4 cards");
#endif
            ppot = npot = 0.0;

            int[,]  HP = new int[3, 3];
            int     count = 0;
            int     ncards = BitCount(board)+2;
            uint    ourbest;
            Random rand = new Random();
            double  start = CurrentTime;

            switch (NOpponents)
            {
                case 1:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong pocket = RandomHand(rand, list, board, 2);
                        uint ourrank = Evaluate(pocket | board);
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board, ncards);
                        int index;

                        if (ourrank > opp1rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        if (ourbest > opp1best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 2:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong pocket = RandomHand(rand, list, board, 2);
                        uint ourrank = Evaluate(pocket | board);
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 3:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong pocket = RandomHand(rand, list, board, 2);
                        uint ourrank = Evaluate(pocket | board);
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //   count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //   count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 4:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong pocket = RandomHand(rand, list, board, 2);
                        uint ourrank = Evaluate(pocket | board);
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 5:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong pocket = RandomHand(rand, list, board, 2);
                        uint ourrank = Evaluate(pocket | board);
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        ulong opp5Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        uint opp5rank = Evaluate(opp5Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        uint opp5best = Evaluate(opp5Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best &&
                            ourbest > opp5best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best &&
                            ourbest >= opp5best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 6:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong pocket = RandomHand(rand, list, board, 2);
                        uint ourrank = Evaluate(pocket | board);
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        ulong opp5Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 2);
                        ulong opp6Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        uint opp5rank = Evaluate(opp5Pocket | board);
                        uint opp6rank = Evaluate(opp6Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        uint opp5best = Evaluate(opp5Pocket | boardmask, 7);
                        uint opp6best = Evaluate(opp6Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best &&
                            ourbest > opp5best && ourbest > opp6best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best &&
                            ourbest >= opp5best && ourbest >= opp6best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //   count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 7:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong pocket = RandomHand(rand, list, board, 2);
                        uint ourrank = Evaluate(pocket | board);
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        ulong opp5Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 2);
                        ulong opp6Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket, 2);
                        ulong opp7Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        uint opp5rank = Evaluate(opp5Pocket | board);
                        uint opp6rank = Evaluate(opp6Pocket | board);
                        uint opp7rank = Evaluate(opp7Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        uint opp5best = Evaluate(opp5Pocket | boardmask, 7);
                        uint opp6best = Evaluate(opp6Pocket | boardmask, 7);
                        uint opp7best = Evaluate(opp7Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best &&
                            ourbest > opp5best && ourbest > opp6best &&
                            ourbest > opp7best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best &&
                            ourbest >= opp5best && ourbest >= opp6best &&
                            ourbest >= opp7best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 8:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong pocket = RandomHand(rand, list, board, 2);
                        uint ourrank = Evaluate(pocket | board);
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        ulong opp5Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 2);
                        ulong opp6Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket, 2);
                        ulong opp7Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket, 2);
                        ulong opp8Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        uint opp5rank = Evaluate(opp5Pocket | board);
                        uint opp6rank = Evaluate(opp6Pocket | board);
                        uint opp7rank = Evaluate(opp7Pocket | board);
                        uint opp8rank = Evaluate(opp8Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank && ourrank > opp8rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank && ourrank >= opp8rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket | opp8Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        uint opp5best = Evaluate(opp5Pocket | boardmask, 7);
                        uint opp6best = Evaluate(opp6Pocket | boardmask, 7);
                        uint opp7best = Evaluate(opp7Pocket | boardmask, 7);
                        uint opp8best = Evaluate(opp8Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best &&
                            ourbest > opp5best && ourbest > opp6best &&
                            ourbest > opp7best && ourbest > opp8best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best &&
                            ourbest >= opp5best && ourbest >= opp6best &&
                            ourbest >= opp7best && ourbest >= opp8best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;
                case 9:
                    while ((CurrentTime - start) < duration)
                    {
                        ulong pocket = RandomHand(rand, list, board, 2);
                        uint ourrank = Evaluate(pocket | board);
                        ulong opp1Pocket = Hand.RandomHand(rand, 0UL, pocket | board, 2);
                        ulong opp2Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket, 2);
                        ulong opp3Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket, 2);
                        ulong opp4Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket, 2);
                        ulong opp5Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket, 2);
                        ulong opp6Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket, 2);
                        ulong opp7Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket, 2);
                        ulong opp8Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket, 2);
                        ulong opp9Pocket = Hand.RandomHand(rand, 0UL, pocket | board | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket | opp8Pocket, 2);
                        uint opp1rank = Evaluate(opp1Pocket | board);
                        uint opp2rank = Evaluate(opp2Pocket | board);
                        uint opp3rank = Evaluate(opp3Pocket | board);
                        uint opp4rank = Evaluate(opp4Pocket | board);
                        uint opp5rank = Evaluate(opp5Pocket | board);
                        uint opp6rank = Evaluate(opp6Pocket | board);
                        uint opp7rank = Evaluate(opp7Pocket | board);
                        uint opp8rank = Evaluate(opp8Pocket | board);
                        uint opp9rank = Evaluate(opp9Pocket | board);
                        int index;

                        if (ourrank > opp1rank && ourrank > opp2rank &&
                            ourrank > opp3rank && ourrank > opp4rank &&
                            ourrank > opp5rank && ourrank > opp6rank &&
                            ourrank > opp7rank && ourrank > opp8rank &&
                            ourrank > opp9rank)
                        {
                            index = ahead;
                        }
                        else if (ourrank >= opp1rank && ourrank >= opp2rank &&
                            ourrank >= opp3rank && ourrank >= opp4rank &&
                            ourrank >= opp5rank && ourrank >= opp6rank &&
                            ourrank >= opp7rank && ourrank >= opp8rank &&
                            ourrank >= opp9rank)
                        {
                            index = tied;
                        }
                        else
                        {
                            index = behind;
                        }

                        ulong boardmask = Hand.RandomHand(rand, board, pocket | opp1Pocket | opp2Pocket | opp3Pocket | opp4Pocket | opp5Pocket | opp6Pocket | opp7Pocket | opp8Pocket | opp9Pocket, 5);
                        ourbest = Evaluate(pocket | boardmask, 7);
                        uint opp1best = Evaluate(opp1Pocket | boardmask, 7);
                        uint opp2best = Evaluate(opp2Pocket | boardmask, 7);
                        uint opp3best = Evaluate(opp3Pocket | boardmask, 7);
                        uint opp4best = Evaluate(opp4Pocket | boardmask, 7);
                        uint opp5best = Evaluate(opp5Pocket | boardmask, 7);
                        uint opp6best = Evaluate(opp6Pocket | boardmask, 7);
                        uint opp7best = Evaluate(opp7Pocket | boardmask, 7);
                        uint opp8best = Evaluate(opp8Pocket | boardmask, 7);
                        uint opp9best = Evaluate(opp9Pocket | boardmask, 7);
                        if (ourbest > opp1best && ourbest > opp2best &&
                            ourbest > opp3best && ourbest > opp4best &&
                            ourbest > opp5best && ourbest > opp6best &&
                            ourbest > opp7best && ourbest > opp8best &&
                            ourbest > opp9best)
                        {
                            HP[index, ahead]++;
                            //if (index == behind || index == tied)
                            //    count++;
                        }
                        else if (ourbest >= opp1best && ourbest >= opp2best &&
                            ourbest >= opp3best && ourbest >= opp4best &&
                            ourbest >= opp5best && ourbest >= opp6best &&
                            ourbest >= opp7best && ourbest >= opp8best &&
                            ourbest >= opp9best)
                        {
                            HP[index, tied]++;
                            //if (index == behind || index == ahead)
                            //    count++;
                        }
                        else
                        {
                            HP[index, behind]++;
                            //if (index == ahead || index == tied)
                            //    count++;
                        }
                        count++;
                    }
                    break;

            }
            if (count != 0)
            {
                ppot = (HP[behind, ahead] + (HP[behind, tied] / 2.0) + (HP[tied, ahead] / 2.0)) / ((double)count);
                npot = (HP[ahead, behind] + (HP[ahead, tied] / 2.0) + (HP[tied, behind] / 2.0)) / ((double)count);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ourcards"></param>
        /// <param name="board"></param>
        /// <param name="numOpponents"></param>
        /// <param name="duration"></param>
        /// <param name="player"></param>
        /// <param name="opponent"></param>
        public static void HandPlayerMultiOpponentOdds(ulong ourcards, ulong board, int numOpponents, double duration, ref double[] player, ref double[] opponent)
        {
            long playerCount = 0, OpponentCount = 0;
            Random rand = new Random();

            for (int i = 0; i < 9; i++)
            {
                player[i] = 0.0; opponent[i] = 0.0;
            }
#if DEBUG
            if (BitCount(ourcards) != 2) throw new ArgumentException("pocket must contain exactly 2 cards");
            if (numOpponents < 1 || numOpponents > 9) throw new ArgumentException("numOpponents must be 1-9");
            if (BitCount(board) > 5) throw new ArgumentException("board must contain 0-5 cards");
            if (duration <= 0.0) throw new ArgumentException("duration musn't be 0.0 or negative");
#endif
            switch (numOpponents)
            {
                case 1:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1 = Hand.RandomHand(rand, 0UL, ourcards | boardmask, 2);
                        uint playerHandVal = Hand.Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Hand.Evaluate(opp1 | boardmask, 7);

                        if (playerHandVal >= opp1HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        else
                        {
                            opponent[HandType(opp1HandVal)] += 1.0;
                        }
                        playerCount++;
                        OpponentCount++;
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        player[i] = player[i] / playerCount;
                        opponent[i] = opponent[i] / OpponentCount;
                    }

                    return;
                case 2:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1 = Hand.RandomHand(rand, 0UL, ourcards | boardmask, 2);
                        ulong opp2 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1, 2);
                        uint playerHandVal = Hand.Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Hand.Evaluate(opp1 | boardmask, 7);
                        uint opp2HandVal = Hand.Evaluate(opp2 | boardmask, 7);

                        if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        playerCount++;

                        if (opp1HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp1HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp2HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp2HandVal)] += 1.0;
                        }
                        OpponentCount++;
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        player[i] = player[i] / playerCount;
                        opponent[i] = opponent[i] / OpponentCount;
                    }

                    return;
                case 3:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1 = Hand.RandomHand(rand, 0UL, ourcards | boardmask, 2);
                        ulong opp2 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1, 2);
                        ulong opp3 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2, 2);
                        uint playerHandVal = Hand.Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Hand.Evaluate(opp1 | boardmask, 7);
                        uint opp2HandVal = Hand.Evaluate(opp2 | boardmask, 7);
                        uint opp3HandVal = Hand.Evaluate(opp3 | boardmask, 7);

                        if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal && playerHandVal >= opp3HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        playerCount++;

                        if (opp1HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp1HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp2HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp2HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp3HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp3HandVal)] += 1.0;
                        }
                        OpponentCount++;
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        player[i] = player[i] / playerCount;
                        opponent[i] = opponent[i] / OpponentCount;
                    }

                    return;
                case 4:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1 = Hand.RandomHand(rand, 0UL, ourcards | boardmask, 2);
                        ulong opp2 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1, 2);
                        ulong opp3 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2, 2);
                        ulong opp4 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3, 2);
                        uint playerHandVal = Hand.Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Hand.Evaluate(opp1 | boardmask, 7);
                        uint opp2HandVal = Hand.Evaluate(opp2 | boardmask, 7);
                        uint opp3HandVal = Hand.Evaluate(opp3 | boardmask, 7);
                        uint opp4HandVal = Hand.Evaluate(opp4 | boardmask, 7);

                        if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        playerCount++;

                        if (opp1HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp1HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp2HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp2HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp3HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp3HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp4HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp4HandVal)] += 1.0;
                        }
                        OpponentCount++;
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        player[i] = player[i] / playerCount;
                        opponent[i] = opponent[i] / OpponentCount;
                    }

                    return;
                case 5:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1 = Hand.RandomHand(rand, 0UL, ourcards | boardmask, 2);
                        ulong opp2 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1, 2);
                        ulong opp3 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2, 2);
                        ulong opp4 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3, 2);
                        ulong opp5 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4, 2);
                        uint playerHandVal = Hand.Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Hand.Evaluate(opp1 | boardmask, 7);
                        uint opp2HandVal = Hand.Evaluate(opp2 | boardmask, 7);
                        uint opp3HandVal = Hand.Evaluate(opp3 | boardmask, 7);
                        uint opp4HandVal = Hand.Evaluate(opp4 | boardmask, 7);
                        uint opp5HandVal = Hand.Evaluate(opp5 | boardmask, 7);

                        if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        playerCount++;

                        if (opp1HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp1HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp2HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp2HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp3HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp3HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp4HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp4HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp5HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp5HandVal)] += 1.0;
                        }
                        OpponentCount++;
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        player[i] = player[i] / playerCount;
                        opponent[i] = opponent[i] / OpponentCount;
                    }

                    return;
                case 6:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1 = Hand.RandomHand(rand, 0UL, ourcards | boardmask, 2);
                        ulong opp2 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1, 2);
                        ulong opp3 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2, 2);
                        ulong opp4 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3, 2);
                        ulong opp5 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4, 2);
                        ulong opp6 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4 | opp5, 2);
                        uint playerHandVal = Hand.Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Hand.Evaluate(opp1 | boardmask, 7);
                        uint opp2HandVal = Hand.Evaluate(opp2 | boardmask, 7);
                        uint opp3HandVal = Hand.Evaluate(opp3 | boardmask, 7);
                        uint opp4HandVal = Hand.Evaluate(opp4 | boardmask, 7);
                        uint opp5HandVal = Hand.Evaluate(opp5 | boardmask, 7);
                        uint opp6HandVal = Hand.Evaluate(opp6 | boardmask, 7);

                        if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        playerCount++;

                        if (opp1HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp1HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp2HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp2HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp3HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp3HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp4HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp4HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp5HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp5HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp6HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp6HandVal)] += 1.0;
                        }
                        OpponentCount++;
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        player[i] = player[i] / playerCount;
                        opponent[i] = opponent[i] / OpponentCount;
                    }

                    return;
                case 7:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1 = Hand.RandomHand(rand, 0UL, ourcards | boardmask, 2);
                        ulong opp2 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1, 2);
                        ulong opp3 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2, 2);
                        ulong opp4 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3, 2);
                        ulong opp5 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4, 2);
                        ulong opp6 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4 | opp5, 2);
                        ulong opp7 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4 | opp5 | opp6, 2);
                        uint playerHandVal = Hand.Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Hand.Evaluate(opp1 | boardmask, 7);
                        uint opp2HandVal = Hand.Evaluate(opp2 | boardmask, 7);
                        uint opp3HandVal = Hand.Evaluate(opp3 | boardmask, 7);
                        uint opp4HandVal = Hand.Evaluate(opp4 | boardmask, 7);
                        uint opp5HandVal = Hand.Evaluate(opp5 | boardmask, 7);
                        uint opp6HandVal = Hand.Evaluate(opp6 | boardmask, 7);
                        uint opp7HandVal = Hand.Evaluate(opp7 | boardmask, 7);

                        if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal &&
                            playerHandVal >= opp7HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        playerCount++;

                        if (opp1HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp1HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp2HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp2HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp3HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp3HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp4HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp4HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp5HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp5HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp6HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp6HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp7HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp7HandVal)] += 1.0;
                        }
                        OpponentCount++;
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        player[i] = player[i] / playerCount;
                        opponent[i] = opponent[i] / OpponentCount;
                    }

                    return;
                case 8:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1 = Hand.RandomHand(rand, 0UL, ourcards | boardmask, 2);
                        ulong opp2 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1, 2);
                        ulong opp3 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2, 2);
                        ulong opp4 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3, 2);
                        ulong opp5 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4, 2);
                        ulong opp6 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4 | opp5, 2);
                        ulong opp7 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4 | opp5 | opp6, 2);
                        ulong opp8 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4 | opp5 | opp6 | opp7, 2);
                        uint playerHandVal = Hand.Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Hand.Evaluate(opp1 | boardmask, 7);
                        uint opp2HandVal = Hand.Evaluate(opp2 | boardmask, 7);
                        uint opp3HandVal = Hand.Evaluate(opp3 | boardmask, 7);
                        uint opp4HandVal = Hand.Evaluate(opp4 | boardmask, 7);
                        uint opp5HandVal = Hand.Evaluate(opp5 | boardmask, 7);
                        uint opp6HandVal = Hand.Evaluate(opp6 | boardmask, 7);
                        uint opp7HandVal = Hand.Evaluate(opp7 | boardmask, 7);
                        uint opp8HandVal = Hand.Evaluate(opp8 | boardmask, 7);

                        if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal &&
                            playerHandVal >= opp7HandVal && playerHandVal >= opp8HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        playerCount++;

                        if (opp1HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp1HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp2HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp2HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp3HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp3HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp4HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp4HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp5HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp5HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp6HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp6HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp7HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp7HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp8HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp7HandVal)] += 1.0;
                        }
                        OpponentCount++;
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        player[i] = player[i] / playerCount;
                        opponent[i] = opponent[i] / OpponentCount;
                    }

                    return;
                case 9:
                    foreach (ulong boardmask in Hand.RandomHands(board, ourcards, 5, duration))
                    {
                        ulong opp1 = Hand.RandomHand(rand, 0UL, ourcards | boardmask, 2);
                        ulong opp2 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1, 2);
                        ulong opp3 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2, 2);
                        ulong opp4 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3, 2);
                        ulong opp5 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4, 2);
                        ulong opp6 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4 | opp5, 2);
                        ulong opp7 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4 | opp5 | opp6, 2);
                        ulong opp8 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4 | opp5 | opp6 | opp7, 2);
                        ulong opp9 = Hand.RandomHand(rand, 0UL, ourcards | boardmask | opp1 | opp2 | opp3 | opp4 | opp5 | opp6 | opp7 | opp8, 2);
                        uint playerHandVal = Hand.Evaluate(ourcards | boardmask, 7);
                        uint opp1HandVal = Hand.Evaluate(opp1 | boardmask, 7);
                        uint opp2HandVal = Hand.Evaluate(opp2 | boardmask, 7);
                        uint opp3HandVal = Hand.Evaluate(opp3 | boardmask, 7);
                        uint opp4HandVal = Hand.Evaluate(opp4 | boardmask, 7);
                        uint opp5HandVal = Hand.Evaluate(opp5 | boardmask, 7);
                        uint opp6HandVal = Hand.Evaluate(opp6 | boardmask, 7);
                        uint opp7HandVal = Hand.Evaluate(opp7 | boardmask, 7);
                        uint opp8HandVal = Hand.Evaluate(opp8 | boardmask, 7);
                        uint opp9HandVal = Hand.Evaluate(opp9 | boardmask, 7);

                        if (playerHandVal >= opp1HandVal && playerHandVal >= opp2HandVal &&
                            playerHandVal >= opp3HandVal && playerHandVal >= opp4HandVal &&
                            playerHandVal >= opp5HandVal && playerHandVal >= opp6HandVal &&
                            playerHandVal >= opp7HandVal && playerHandVal >= opp8HandVal &&
                            playerHandVal >= opp9HandVal)
                        {
                            player[HandType(playerHandVal)] += 1.0;
                        }
                        playerCount++;

                        if (opp1HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp1HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp2HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp2HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp3HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp3HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp4HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp4HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp5HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp5HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp6HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp6HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp7HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp7HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp8HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp8HandVal)] += 1.0;
                        }
                        OpponentCount++;

                        if (opp9HandVal >= playerHandVal)
                        {
                            opponent[HandType(opp8HandVal)] += 1.0;
                        }
                        OpponentCount++;
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        player[i] = player[i] / playerCount;
                        opponent[i] = opponent[i] / OpponentCount;
                    }

                    return;
                default:
                    Debug.Assert(false); // Should never get here
                    return;
            }
        }
        #endregion

        #region WinOdds
        /// <summary>
        /// This method returns the approximate odd for the players mask winning against multiple opponents.
        /// This uses a default time duration of 0.25S (or 250mS) for the time allotment for Monte Carlo analysis.
        /// </summary>
        /// <param name="pocket">The pocket mask of the player.</param>
        /// <param name="board">The current board cards</param>
        /// <param name="dead">Dead cards</param>
        /// <param name="numOpponents">The number of oppoents 1-9 are legal values</param>
        /// <returns>The approximate odds of winning the passed mask against the number of opponents specified.</returns>
        /// <example>
        /// <code>
        /// using System;
        /// using HoldemHand;
        ///
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///    {
        ///        static void Main(string[] args)
        ///        {
        ///            // Outputs approximately the following: 50.82%
        ///            Console.WriteLine("{0:#.00}%", Hand.WinOdds("As Ks", "Ts Qs 4h", 0UL, 5) * 100.0);
        ///        }
        ///    }
        /// }
        /// </code>
        /// </example>
        static public double WinOdds(string pocket, string board, ulong dead, int numOpponents)
        {
#if DEBUG
            if (!Hand.ValidateHand(pocket) || BitCount(Hand.ParseHand(pocket)) != 2) throw new ArgumentException("pocket must contain exactly 2 cards");
            if ((board != "" && !Hand.ValidateHand(board)) || BitCount(Hand.ParseHand(board)) > 5) throw new ArgumentException("board must have 0-5 cards");
            if (numOpponents < 0 || numOpponents > 9) throw new ArgumentException("numOpponents must be 1-9");
#endif
            return WinOdds(ParseHand(pocket), ParseHand(board), dead, numOpponents);
        }

        /// <summary>
        /// This method returns the approximate odd for the players mask winning against multiple opponents.
        /// This uses a default time duration of 0.25S (or 250mS) for the time allotment for Monte Carlo analysis.
        /// </summary>
        /// <param name="pocket">The pocket mask of the player.</param>
        /// <param name="board">The current board cards</param>
        /// <param name="numOpponents">The number of oppoents 1-9 are legal values</param>
        /// <returns>The approximate odds of winning the passed mask against the number of opponents specified.</returns>
        static public double WinOdds(string pocket, string board, int numOpponents)
        {
#if DEBUG
            if (!Hand.ValidateHand(pocket) || BitCount(Hand.ParseHand(pocket)) != 2) throw new ArgumentException("pocket must contain exactly 2 cards");
            if ((board != "" && !Hand.ValidateHand(board)) || BitCount(Hand.ParseHand(board)) > 5) throw new ArgumentException("board must have 0-5 cards");
            if (numOpponents < 0 || numOpponents > 9) throw new ArgumentException("numOpponents must be 1-9");
#endif
            return WinOdds(ParseHand(pocket), ParseHand(board), 0UL, numOpponents);
        }

        /// <summary>
        /// This method returns the exact odds of the specified mask mask
        /// winning against an average player. It's reasonably fast because it
        /// uses a lookup table when possible.
        /// </summary>
        /// <param name="pocket">The pocket mask</param>
        /// <param name="board">The board mask</param>
        /// <param name="dead">Dead cards</param>
        /// <returns>The Win odds</returns>
        static public double WinOdds(ulong pocket, ulong board, ulong dead)
        {
            // For one player we can lookup the value if the board is empty
            // and if it's not empty it's probably just faster to calculate the
            // results exhaustively.
            if (board == 0UL && dead == 0UL)
            {
                // Use precalculate values
                double retval = 0.0;
                int index = (int)Hand.PocketHand169Type(pocket);
                foreach (double value in Hand.PreCalcPlayerOdds[index])
                    retval += value;
                return retval;
            }
            else
            {
                // Calculate the results exhaustively
                long win = 0, lose = 0, tie = 0;
                foreach (ulong mask in Hand.Hands(board, pocket | dead, 5))
                {
                    foreach (ulong opp1 in Hand.Hands(0UL, pocket | board | dead, 2))
                    {
                        uint playerHandVal = Hand.Evaluate(mask | pocket);
                        uint oppHandVal = Hand.Evaluate(mask | opp1);

                        if (playerHandVal > oppHandVal)
                            win++;
                        else if (playerHandVal == oppHandVal)
                            tie++;
                        else
                            lose++;
                    }
                }
                return (win + tie / 2.0) / (win + tie + lose);
            }
        }

        /// <summary>
        /// This method returns the approximate odd for the players mask winning against multiple opponents.
        /// This uses a default time duration of 0.1S (or 100mS) for the time allotment for Monte Carlo analysis.
        /// </summary>
        /// <param name="pocket">The pocket mask of the player.</param>
        /// <param name="board">The current board cards</param>
        /// <param name="dead">Dead cards</param>
        /// <param name="numOpponents">The number of oppoents 1-9 are legal values</param>
        /// <returns>The approximate odds of winning the passed mask against the number of opponents specified.</returns>
        /// <example>
        /// <code>
        /// using System;
        /// using HoldemHand;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             ulong pocket = Hand.ParseHand("As Ks");
        ///             ulong board = Hand.ParseHand("Ac 2d 8c");
        ///             // Outputs approximately the following: 49.33%
        ///             Console.WriteLine("{0:#.00}%", Hand.WinOdds(pocket, board, 5) * 100.0);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        static public double WinOdds(ulong pocket, ulong board, ulong dead, int numOpponents)
        {
            return WinOdds(pocket, board, dead, numOpponents, DefaultTimeDuration); 
        }

        /// <summary>
        /// This method returns the approximate odd for the players mask winning against multiple opponents.
        /// </summary>
        /// <param name="pocket">The pocket mask of the player.</param>
        /// <param name="board">The current board cards</param>
        /// <param name="dead">Dead cards</param>
        /// <param name="numOpponents">The approximate odds of winning the passed mask against the number of opponents specified.</param>
        /// <param name="duration">The period of time (in seconds) to run trials. On my 2.8Ghz laptop 0.1 seconds seems adequate.</param>
        /// <returns>The approximate odds of winning the passed mask against the number of opponents specified.</returns>
        /// <example>
        /// <code>
        /// using System;
        /// using HoldemHand;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             ulong pocket = Hand.ParseHand("As Ks");
        ///             ulong board = Hand.ParseHand("Ac 2d 8c");
        ///             // Outputs approximately the following: 49.77%
        ///             Console.WriteLine("{0:#.00}%", Hand.WinOdds(pocket, board, 5, 1.5) * 100.0);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        static public double WinOdds(ulong pocket, ulong board, ulong dead, int numOpponents, double duration)
        {
#if DEBUG
            if (BitCount(pocket) != 2) throw new ArgumentException("pocket must contain exactly 2 cards");
            if (numOpponents < 1 || numOpponents > 9) throw new ArgumentException("numOpponents must be 1-9");
            if (BitCount(board) > 5) throw new ArgumentException("board must contain 0-5 cards");
            if (duration <= 0.0) throw new ArgumentException("duration musn't be 0.0 or negative");
#endif
            // Keep track of stats
            double win = 0.0, count = 0.0;

            // Loop through random boards
            foreach (ulong boardmask in Hand.RandomHands(board, dead | pocket, 5, duration))
            {
                ulong deadmask = dead | boardmask | pocket;
                uint playerHandVal = Hand.Evaluate(pocket | boardmask);

                // Comparison Results
                bool greaterthan = true;
                bool greaterthanequal = true;

                // Get random opponent hand values
                for (int i = 0; i < numOpponents; i++)
                {
                    ulong oppmask = Hand.RandomHand(deadmask, 2);
                    uint oppHandVal = Hand.Evaluate(oppmask | boardmask);
                    deadmask |= oppmask;

                    if (playerHandVal < oppHandVal)
                    {
                        greaterthan = greaterthanequal = false;
                        break;
                    }
                    else if (playerHandVal <= oppHandVal)
                    {
                        greaterthan = false;
                    }
                }

                if (greaterthan)
                    win += 1.0;
                else if (greaterthanequal)
                    win += 0.5;

                count += 1.0;
            }

            // Return stats
            return (count == 0.0 ? 0.0 : win / count);
        }
 
        /// <summary>
        /// This method returns the approximate odd for the players mask winning against multiple opponents.
        /// </summary>
        /// <param name="pocketquery">The pocket mask of the player.</param>
        /// <param name="boardhand">The current board cards</param>
        /// <param name="deadcards">dead cards</param>
        /// <param name="numOpponents">The approximate odds of winning the passed mask against the number of opponents specified.</param>
        /// <param name="duration">The period of time (in seconds) to run trials. On my 2.8Ghz laptop 0.1 seconds seems adequate.</param>
        /// <returns>The approximate odds of winning the passed mask against the number of opponents specified.</returns>
        /// <example>
        /// <code>
        /// using System;
        /// using HoldemHand;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             // Outputs approximately the following: 31.06%
        ///             Console.WriteLine("{0:#.00}%", Hand.WinOdds("AKs", "", 5, 1.5) * 100.0);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        static public double WinOdds(string pocketquery, string boardhand, string deadcards, int numOpponents, double duration)
        {
            ulong board = ParseHand(boardhand);
            ulong dead = ParseHand(deadcards);
            ulong[] list = PocketHands.Query(pocketquery);

#if DEBUG
            if (!PocketHands.ValidateQuery(pocketquery, Hand.ParseHand(boardhand))) throw new ArgumentException("pocketquery and/or board is invalid");
            if (numOpponents < 1 || numOpponents > 9) throw new ArgumentException("numOpponents must be 1-9");
            if (BitCount(board) > 5) throw new ArgumentException("board must contain 0-5 cards");
            if (duration <= 0.0) throw new ArgumentException("duration musn't be 0.0 or negative");
#endif
          
            double start = Hand.CurrentTime;

            // Keep track of stats
            double win = 0.0, count = 0.0;

            // Loop through random boards
            while ((Hand.CurrentTime - start) < duration)
            {
                ulong pocket = Hand.RandomHand(list, dead | board, 2);
                ulong boardmask = Hand.RandomHand(board, dead | pocket, 5);
                ulong deadmask = dead | boardmask | pocket;
                uint playerHandVal = Hand.Evaluate(pocket | boardmask);

                // Comparison Results
                bool greaterthan = true;
                bool greaterthanequal = true;

                // Get random opponent hand values
                for (int i = 0; i < numOpponents; i++)
                {
                    ulong oppmask = Hand.RandomHand(deadmask, 2);
                    uint oppHandVal = Hand.Evaluate(oppmask | boardmask);
                    deadmask |= oppmask;

                    if (playerHandVal < oppHandVal)
                    {
                        greaterthan = greaterthanequal = false;
                        break;
                    }
                    else if (playerHandVal <= oppHandVal)
                    {
                        greaterthan = false;
                    }
                }

                if (greaterthan)
                    win += 1.0;
                else if (greaterthanequal)
                    win += 0.5;

                count += 1.0;
            }

            // Return stats
            return (count == 0.0 ? 0.0 : win / count);
        }
        #endregion

        #region Contiguous Count Table
        /// <summary>
        /// Contains the count of the maximum number of contiguous bits
        /// in a 13 bit word (where bits 1 and bits 13 can be considered
        /// adjacent)
        /// </summary>
        static private readonly int[] contiguousCountTable =
        {
            0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2,
            0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 0, 0, 0, 2, 0, 0, 2, 3,
            0, 0, 0, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3,
            4, 4, 5, 6, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4,
            0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3,
            4, 4, 4, 4, 5, 5, 6, 7, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2,
            2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5,
            0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 2, 2, 2, 2,
            2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            3, 3, 4, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 8, 0, 0, 0, 2,
            0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3,
            2, 2, 2, 2, 3, 3, 4, 5, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2,
            2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6,
            0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2,
            0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
            5, 5, 6, 7, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3,
            3, 3, 3, 3, 4, 4, 5, 6, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 5,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5,
            5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 8, 9, 0, 0, 0, 2, 0, 0, 2, 3,
            0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2,
            3, 3, 4, 5, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4,
            2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6, 0, 0, 0, 2,
            0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3,
            2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 6, 7,
            0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2,
            0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 0, 0, 0, 2, 0, 0, 2, 3,
            0, 0, 0, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3,
            4, 4, 5, 6, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            5, 5, 5, 5, 6, 6, 7, 8, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2,
            2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 6, 7, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 4, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 5, 6,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7,
            8, 8, 9, 10, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4,
            0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 0, 0, 0, 2,
            0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3,
            3, 3, 3, 3, 4, 4, 5, 6, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2,
            2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3,
            3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 6, 7, 0, 0, 0, 2, 0, 0, 2, 3,
            0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2,
            3, 3, 4, 5, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4,
            2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 3, 3, 4, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 8,
            0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2,
            0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 0, 0, 0, 2, 0, 0, 2, 3,
            0, 0, 0, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3,
            4, 4, 5, 6, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4,
            0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3,
            4, 4, 4, 4, 5, 5, 6, 7, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2,
            2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 8, 9, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
            5, 5, 6, 7, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3,
            3, 3, 3, 3, 4, 4, 5, 6, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 8, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 4, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 5, 6, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 4, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 6, 7,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 5, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6,
            6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7,
            8, 8, 8, 8, 9, 9, 10, 11, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2,
            2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5,
            0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 2, 2, 2, 2,
            2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6, 0, 0, 0, 2, 0, 0, 2, 3,
            0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2,
            3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 6, 7, 0, 0, 0, 2,
            0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3,
            2, 2, 2, 2, 3, 3, 4, 5, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2,
            2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5,
            6, 6, 7, 8, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4,
            0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 0, 0, 0, 2,
            0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3,
            3, 3, 3, 3, 4, 4, 5, 6, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2,
            2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3,
            3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 6, 7, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4,
            2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 8, 9,
            0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 0, 0, 0, 2,
            0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 0, 0, 0, 2, 0, 0, 2, 3,
            0, 0, 0, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3,
            4, 4, 5, 6, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4,
            0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3,
            4, 4, 4, 4, 5, 5, 6, 7, 0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2,
            2, 2, 3, 4, 0, 0, 0, 2, 0, 0, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5,
            0, 0, 0, 2, 0, 0, 2, 3, 0, 0, 0, 2, 2, 2, 3, 4, 2, 2, 2, 2,
            2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            3, 3, 4, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 8, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
            5, 5, 6, 7, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 5, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 4, 4, 5, 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6,
            6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 9, 10, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4,
            2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            2, 2, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 6, 7,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3,
            4, 4, 5, 6, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            5, 5, 5, 5, 6, 6, 7, 8, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 3, 3, 4, 5,
            2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2,
            2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 6, 7, 2, 2, 2, 2,
            2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3,
            2, 2, 2, 2, 3, 3, 4, 5, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2,
            2, 2, 3, 4, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 5, 6,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6,
            7, 7, 8, 9, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 5, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 4, 4, 5, 6, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 5,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 6, 7, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 4, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 5, 6, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 4, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 8,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 5, 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 5, 5, 6, 7, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            8, 8, 8, 8, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 11, 12, 0, 2, 0, 3,
            0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5, 0, 2, 0, 3, 0, 2, 2, 4,
            2, 2, 2, 3, 3, 3, 4, 6, 0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3,
            2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7,
            0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5, 0, 2, 0, 3,
            0, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 2, 2, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4,
            5, 5, 6, 8, 0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5,
            0, 2, 0, 3, 0, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 0, 2, 0, 3,
            0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4,
            3, 3, 3, 3, 4, 4, 5, 7, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4,
            4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 9, 0, 2, 0, 3, 0, 2, 2, 4,
            0, 2, 0, 3, 2, 2, 3, 5, 0, 2, 0, 3, 0, 2, 2, 4, 2, 2, 2, 3,
            3, 3, 4, 6, 0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5,
            2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7, 0, 2, 0, 3,
            0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5, 0, 2, 0, 3, 0, 2, 2, 4,
            2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            2, 2, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 8,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3,
            4, 4, 5, 7, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 4, 6, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            6, 6, 6, 6, 7, 7, 8, 10, 0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3,
            2, 2, 3, 5, 0, 2, 0, 3, 0, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6,
            0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5, 2, 2, 2, 3,
            2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7, 0, 2, 0, 3, 0, 2, 2, 4,
            0, 2, 0, 3, 2, 2, 3, 5, 0, 2, 0, 3, 0, 2, 2, 4, 2, 2, 2, 3,
            3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5,
            3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 8, 0, 2, 0, 3,
            0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5, 0, 2, 0, 3, 0, 2, 2, 4,
            2, 2, 2, 3, 3, 3, 4, 6, 0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3,
            2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5,
            6, 6, 7, 9, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4,
            3, 3, 3, 3, 4, 4, 5, 7, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 3, 3, 3, 3,
            3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 8, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 4, 6, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 4, 4, 5, 7, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 9, 11,
            0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5, 0, 2, 0, 3,
            0, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 0, 2, 0, 3, 0, 2, 2, 4,
            0, 2, 0, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3,
            4, 4, 5, 7, 0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5,
            0, 2, 0, 3, 0, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4,
            4, 4, 4, 4, 5, 5, 6, 8, 0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3,
            2, 2, 3, 5, 0, 2, 0, 3, 0, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6,
            0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5, 2, 2, 2, 3,
            2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            3, 3, 4, 6, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5,
            4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 9, 0, 2, 0, 3,
            0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5, 0, 2, 0, 3, 0, 2, 2, 4,
            2, 2, 2, 3, 3, 3, 4, 6, 0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3,
            2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7,
            0, 2, 0, 3, 0, 2, 2, 4, 0, 2, 0, 3, 2, 2, 3, 5, 0, 2, 0, 3,
            0, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 2, 2, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4,
            5, 5, 6, 8, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4,
            3, 3, 3, 3, 4, 4, 5, 7, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 4, 6,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 8, 10, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5,
            2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            2, 2, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 8,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3,
            4, 4, 5, 7, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 4,
            5, 5, 5, 5, 6, 6, 7, 9, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 4, 6,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 4, 4, 5, 7, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 4, 6, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5,
            3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 8, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 7,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 8, 8, 8, 8,
            9, 9, 10, 12, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4,
            3, 3, 3, 3, 4, 4, 5, 7, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 3, 3, 3, 3,
            3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 8, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5,
            2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 3, 3, 4, 6, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 9,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3,
            4, 4, 5, 7, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4,
            4, 4, 4, 4, 5, 5, 6, 8, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3,
            2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 8, 10, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 2, 2, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4,
            5, 5, 6, 8, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5,
            2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4,
            3, 3, 3, 3, 4, 4, 5, 7, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 3, 3, 4, 6,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4,
            4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 9, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5,
            2, 2, 2, 3, 2, 2, 2, 4, 3, 3, 3, 3, 4, 4, 5, 7, 2, 2, 2, 3,
            2, 2, 2, 4, 2, 2, 2, 3, 2, 2, 3, 5, 2, 2, 2, 3, 2, 2, 2, 4,
            2, 2, 2, 3, 3, 3, 4, 6, 2, 2, 2, 3, 2, 2, 2, 4, 2, 2, 2, 3,
            2, 2, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 8,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 4, 6, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            4, 4, 5, 7, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 6, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6,
            7, 7, 7, 7, 8, 8, 9, 11, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 4, 6,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 4, 4, 5, 7, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 4, 6, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5,
            3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 8, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 4, 6, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 4, 4, 5, 7,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 4, 6, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 5, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5,
            6, 6, 7, 9, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 4, 6, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 4, 4, 5, 7, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 4, 6,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3,
            3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 6, 8, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3,
            3, 3, 4, 6, 3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5,
            3, 3, 3, 3, 3, 3, 3, 4, 3, 3, 3, 3, 4, 4, 5, 7, 3, 3, 3, 3,
            3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 4,
            3, 3, 3, 3, 3, 3, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 8, 10,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 5, 7, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 6, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 5, 5, 6, 8, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 6,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 7, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5,
            4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 9, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 7,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 6, 8, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 9, 9, 9, 9,
            9, 9, 9, 9, 10, 10, 10, 10, 11, 11, 12
        };
        #endregion

        #region Precalc Odds Table
        /// <summary>
        /// This table is used by HandPlayerOpponentOdds and contains the odds of each type of 
        /// mask occuring against a random player when the board is currently empty. This calculation
        /// normally takes about 5 minutes, so the values are precalculated to save time.
        /// </summary>
        internal static readonly double[][] PreCalcPlayerOdds = {
	         new double[] {0, 0.286740271754148, 0.337632177082422, 0.107630068931113, 0.00871248305898762, 0.0186005527151292, 0.0842236711352609, 0.00840620900618257, 9.16993377677929E-05},
	         new double[] {0, 0.27207410337779, 0.324989138873109, 0.10751232138638, 0.00934600684105111, 0.0180595349176028, 0.0835212896584642, 0.0083608155789998, 9.35872344620858E-05},
	         new double[] {0, 0.258379200641656, 0.313030451773679, 0.106138870820383, 0.0129346953649848, 0.0174970341905719, 0.0828132101661902, 0.00832256374082725, 0.000135613912540039},
	         new double[] {0, 0.244957911345515, 0.300932738245412, 0.104766138227219, 0.0165233838889184, 0.0169436487627316, 0.0821021100392053, 0.00829115791187947, 0.000177640590617992},
	         new double[] {0, 0.231824795177511, 0.28851389778012, 0.10339412360689, 0.0201120724128521, 0.0163993424017212, 0.0813879892775096, 0.0082659115842676, 0.000219667268695946},
	         new double[] {0, 0.217844636971768, 0.275105943899719, 0.103675534632321, 0.0189032492990468, 0.0159062342734868, 0.0806736110753555, 0.00824653871303799, 0.000216770586798339},
	         new double[] {0, 0.204018493950435, 0.261328674519173, 0.103557069114754, 0.0189032492990468, 0.0154180280022754, 0.0799561264250045, 0.00823194279253484, 0.000216770586798339},
	         new double[] {0, 0.190192350929103, 0.247219242587288, 0.103438603597187, 0.0189032492990468, 0.0149329429582502, 0.0792356211399425, 0.00822144684970111, 0.000216770586798339},
	         new double[] {0, 0.176338424361419, 0.232896092168261, 0.10332013807962, 0.0189032492990468, 0.0144463480736112, 0.0785120952201697, 0.00821436437664798, 0.000216770586798339},
	         new double[] {0, 0.162635821295131, 0.218553432053168, 0.102802513991889, 0.0190919245504947, 0.0139528294708683, 0.0777854628522, 0.00820999933065481, 0.000217221584341976},
	         new double[] {0, 0.148162103963611, 0.203711469029627, 0.10394046946842, 0.015503236026561, 0.0134690902683502, 0.0770585730437719, 0.00820807520159972, 0.000175194906264022},
	         new double[] {0, 0.133400213503953, 0.188900201490065, 0.105077706972117, 0.0119145475026273, 0.0129687585515523, 0.076328662600633, 0.00820750501865871, 0.000133168228186069},
	         new double[] {0, 0.118330046676816, 0.174113077574819, 0.10621422650298, 0.00834041866683601, 0.0124479441091044, 0.0755957315227832, 0.00820760227394296, 9.11434570744733E-05},
	         new double[] {0.0571760455086079, 0.280205634856752, 0.184453924450951, 0.0365310084171588, 0.0266728173482832, 0.0627649090920533, 0.0208664745016668, 0.00123686052505268, 0.000538648391826666},
	         new double[] {0.0623297388924454, 0.295079403218692, 0.189393299129985, 0.037229782390348, 0.0286287805846416, 0.018343179000639, 0.0208670251382026, 0.00123686624595175, 9.26432861149393E-05},
	         new double[] {0.0563138631114711, 0.273071728060495, 0.181336609406188, 0.0361693789449175, 0.0298615120507879, 0.0627519903484619, 0.020787031236681, 0.00123684908325453, 0.000559661730865643},
	         new double[] {0.0613800553439776, 0.287610629316061, 0.186203309597323, 0.0368551455005796, 0.0320692320322293, 0.0180619286371236, 0.0207875818732169, 0.0012368548041536, 0.000113656625153916},
	         new double[] {0.0555485498378983, 0.265836711524236, 0.178378615679726, 0.0358473867219077, 0.0330502067532925, 0.0627403220980596, 0.020707484995512, 0.00123683764145638, 0.000580675069904619},
	         new double[] {0.0605358651744274, 0.280022782050336, 0.183176929196818, 0.036522534335406, 0.035509683479817, 0.0177852359232034, 0.0207080356320478, 0.00123684336235545, 0.000134669964192893},
	         new double[] {0.0548500495143815, 0.258622320259363, 0.175555095976663, 0.0355624373203995, 0.0362389014557972, 0.0627287129636145, 0.0206278357781596, 0.00123682619965823, 0.000601688408943596},
	         new double[] {0.0597644305388458, 0.272450396944582, 0.180288551184217, 0.0362290483989969, 0.0389501349274047, 0.0175130827426982, 0.0206283864146954, 0.00123683192055731, 0.000155683303231869},
	         new double[] {0.0574857463799581, 0.254825609833539, 0.174627747533291, 0.0354514275645503, 0.0203041237098657, 0.0631969142042487, 0.0205495381232133, 0.0012368233392087, 0.000134208478334288},
	         new double[] {0.0626260385577156, 0.268517188727312, 0.17933947071386, 0.0361150332641677, 0.0219231388628111, 0.017266608771168, 0.0205495266814151, 0.0012368233392087, 0.000154234962283066},
	         new double[] {0.055426437247172, 0.248354728065644, 0.172370931511113, 0.0353384398078464, 0.0229088361860597, 0.0631777978199942, 0.0204698803245123, 0.00123682047875916, 0.000154234962283066},
	         new double[] {0.0603773891189644, 0.261706200939715, 0.177031322017776, 0.0359976308803453, 0.0247300665283353, 0.0170225056355623, 0.0204698803245123, 0.00123682047875916, 0.000154234962283066},
	         new double[] {0.0540477577794216, 0.242280815193793, 0.170399055593981, 0.0352441164843702, 0.0229088361860597, 0.0631777978199942, 0.0203901538750224, 0.00123681761830962, 0.000154234962283066},
	         new double[] {0.0588679728051342, 0.255339080548543, 0.175013417892036, 0.0359002468758647, 0.0247300665283353, 0.0167799631135497, 0.0203901538750224, 0.00123681761830962, 0.000154234962283066},
	         new double[] {0.053377082478774, 0.236664279621528, 0.168698908795711, 0.035164367151284, 0.0202753890640437, 0.0631969142042487, 0.0203103587747436, 0.00123681475786009, 0.000134165571591236},
	         new double[] {0.0581277599762468, 0.249478345538871, 0.173272169771113, 0.0358185276465308, 0.0218896573009828, 0.0165366656712302, 0.0203103473329455, 0.00123681475786009, 0.000154234962283066},
	         new double[] {0.0495020672468802, 0.229125260229397, 0.165634580718167, 0.0350797550539853, 0.0350912190206164, 0.0627287129636145, 0.0202292035307101, 0.00123681189741055, 0.000601645502200544},
	         new double[] {0.0539093120218401, 0.241543500000286, 0.17013673854595, 0.0357301101978649, 0.0377348243140499, 0.0162898262772718, 0.0202297541672459, 0.00123681761830962, 0.000154460461054884},
	         new double[] {0.0484983712600337, 0.22563169023391, 0.164477461659965, 0.0351392438230023, 0.0318824225089918, 0.0627403220980596, 0.0201493998490827, 0.00123681761830962, 0.000580630732936799},
	         new double[] {0.0528171351796963, 0.2378983233189, 0.168950961120579, 0.0357896456875577, 0.0342725333342487, 0.0160479566760127, 0.0201499504856185, 0.0012368233392087, 0.000133447122015908},
	         new double[] {0.0480691608070358, 0.222398602784819, 0.163345378686333, 0.0352099455542035, 0.0285622543946516, 0.0627519903484619, 0.020069493191272, 0.0012368233392087, 0.00055955684771596},
	         new double[] {0.0523535278210182, 0.23453412144439, 0.167790650277435, 0.0358607145097828, 0.0306899118237826, 0.0157977908176137, 0.0200700438278078, 0.00123682906010777, 0.000112433782976931},
	         new double[] {0.0476945134289524, 0.219158324165593, 0.162231942983231, 0.035283424781905, 0.0248923190922993, 0.0627649090920533, 0.0199894835572779, 0.00123682906010777, 0.000537232469305946},
	         new double[] {0.0519490960121329, 0.231165252746461, 0.166649352842362, 0.0359346380606457, 0.0267315731271064, 0.0155373835963898, 0.0199900341938138, 0.00123683478100684, 9.14213974211331E-05},
	         new double[] {0.0463257883255901, 0.252002054374857, 0.173669051423445, 0.0347785797524796, 0.0424756597197789, 0.0618434967965826, 0.0207056271335378, 0.00119692864951884, 0.0010068467720113},
	         new double[] {0.0505172550897409, 0.265106691907273, 0.178202746899225, 0.0354012233379882, 0.0455204745256946, 0.0177913396457734, 0.0207067284066095, 0.00119694009131699, 0.000114600573501062},
	         new double[] {0.0454199554685216, 0.244720638486662, 0.170730035349435, 0.0344562228221538, 0.0456643544222836, 0.06183133654886, 0.0206260808923687, 0.0011969172077207, 0.00102786011105028},
	         new double[] {0.049520231101439, 0.257467379433482, 0.175195653079722, 0.0350682903722417, 0.0489609259732823, 0.0175146469318532, 0.0206271821654404, 0.00119692864951884, 0.000135613912540039},
	         new double[] {0.044662873138491, 0.237657768094203, 0.167925486862813, 0.0341737262561235, 0.0488530491247883, 0.0618204673173617, 0.0205464316750163, 0.00119690576592255, 0.00104887345008926},
	         new double[] {0.0486851085569204, 0.250054959247175, 0.172326807646783, 0.034777327352324, 0.05240137742087, 0.017242493751348, 0.020547532948088, 0.0011969172077207, 0.000156627251579016},
	         new double[] {0.0473196229126585, 0.234490950109755, 0.167045325110113, 0.0340896833882826, 0.0324032789046995, 0.0622894642397087, 0.0204681340200701, 0.00119690290547301, 0.000581393519479947},
	         new double[] {0.0515702151687351, 0.246780948776786, 0.171425909303536, 0.034690563720232, 0.0348348290623961, 0.0169960197798179, 0.0204686732148077, 0.00119690862637209, 0.000155178910630212},
	         new double[] {0.0482008058458435, 0.229799316581397, 0.165812924025888, 0.0339996392973134, 0.0208593712903545, 0.0627296907606145, 0.020389699063546, 0.00119690004502348, 0.000135152426681434},
	         new double[] {0.0525199916818127, 0.241900469800232, 0.170165775207569, 0.0345985444888577, 0.0225231810830463, 0.0167519967367992, 0.0203896876217479, 0.00119690004502348, 0.000155178910630212},
	         new double[] {0.0461820936431086, 0.223813894576416, 0.163618899876829, 0.0339249100531643, 0.0234640837665484, 0.0627114787551552, 0.0203099382886617, 0.00119689718457394, 0.000155178910630212},
	         new double[] {0.0503143848574667, 0.235614019806897, 0.167922660261929, 0.0345208594468539, 0.0253301087485705, 0.0165094542147866, 0.0203099382886617, 0.00119689718457394, 0.000155178910630212},
	         new double[] {0.0448215994832884, 0.218273938959151, 0.161690117347082, 0.0338647547994053, 0.0234640837665484, 0.0627106787827681, 0.0202301088629885, 0.0011968943241244, 0.000155178910630212},
	         new double[] {0.0488249344814034, 0.229820854336184, 0.165949713821559, 0.0344588391799968, 0.0253301087485705, 0.0162661567724671, 0.0202301088629885, 0.0011968943241244, 0.000155178910630212},
	         new double[] {0.0437027561003377, 0.212843816022751, 0.159720843723916, 0.0338148099202678, 0.0236374842174697, 0.0627069940470231, 0.0201501764611319, 0.00119689146367487, 0.000155404409402031},
	         new double[] {0.0476003617324484, 0.224155905655509, 0.163935191462283, 0.0344076156799165, 0.0255187840000183, 0.0160193974710956, 0.0201501764611319, 0.00119689146367487, 0.000155404409402031},
	         new double[] {0.0426523489725551, 0.209250891173053, 0.158500574521289, 0.0338742986892848, 0.020448789514965, 0.0627178017788564, 0.0200703727795045, 0.00119689718457394, 0.000134391070363054},
	         new double[] {0.0464572688885495, 0.220406914679083, 0.162685864859778, 0.0344671511696092, 0.0220783325524306, 0.0157775278698366, 0.0200703727795045, 0.00119689718457394, 0.000134391070363054},
	         new double[] {0.0421295231573413, 0.205887041133836, 0.157298748782164, 0.0339450004204861, 0.0172600948124603, 0.0627286095106896, 0.0199904661216938, 0.00119690290547301, 0.000113377731324077},
	         new double[] {0.045891786619618, 0.216905482738045, 0.161455417224216, 0.0345382199918344, 0.0186378811048429, 0.0155273620114376, 0.0199904661216938, 0.00119690290547301, 0.000113377731324077},
	         new double[] {0.0415875537836024, 0.202387923296474, 0.156102155520353, 0.0340157021516874, 0.0140848013160356, 0.0627394167657812, 0.0199104564876998, 0.00119690862637209, 9.23653457682796E-05},
	         new double[] {0.0453054016156963, 0.213263612736323, 0.160230259990072, 0.0346092888140595, 0.0152119893453976, 0.0152669547902137, 0.0199104564876998, 0.00119690862637209, 9.23653457682796E-05},
	         new double[] {0.0367350371314954, 0.224383730449543, 0.163381827249443, 0.0330285805629403, 0.0609119492132906, 0.0609478056633468, 0.0205443421166297, 0.00116366448185531, 0.00149511454288777},
	         new double[] {0.040070950590311, 0.23579145206144, 0.167553169559249, 0.0335791274713569, 0.0652525776941001, 0.0172333164757507, 0.0205459940262372, 0.00116368164455253, 0.000156627251579016},
	         new double[] {0.0358573439467453, 0.217302590365892, 0.160588721991193, 0.0327453746054248, 0.0641006439157952, 0.0609364444345282, 0.0204646928992773, 0.00116365304005716, 0.00151612788192675},
	         new double[] {0.0391047908525112, 0.228357655735745, 0.164696050539185, 0.0332874808039999, 0.0686930291416878, 0.0169611632952455, 0.0204663448088848, 0.00116367020275438, 0.000177640590617992},
	         new double[] {0.0384869051480655, 0.214279256820885, 0.159683215225372, 0.0326573104222767, 0.0475316744728335, 0.0614061617134169, 0.020386395244331, 0.00116365017960763, 0.00104864795131744},
	         new double[] {0.0419606922745551, 0.225235224776985, 0.16376882104284, 0.0331964183929956, 0.0510011644890064, 0.0167146893237154, 0.0203874850756045, 0.00116366162140577, 0.000176192249669189},
	         new double[] {0.0394664803941928, 0.210109579531081, 0.158495276253635, 0.0325928387501666, 0.0355919736072042, 0.0618471667533383, 0.020307960287807, 0.00116364731915809, 0.000602406858518924},
	         new double[] {0.0430184722110188, 0.220900556757898, 0.162554356884177, 0.0331302433231864, 0.0382752805099838, 0.0164706662806967, 0.0203084994825447, 0.00116365304005716, 0.000176192249669189},
	         new double[] {0.0398099131167058, 0.205514361745034, 0.15728853769243, 0.0325306697399336, 0.0240480659928592, 0.0622818511532665, 0.0202294223550996, 0.00116364445870855, 0.000156165765720411},
	         new double[] {0.0433818637201748, 0.21612625766815, 0.161320363721414, 0.0330670903183127, 0.025963632530634, 0.0162282038512711, 0.0202294109133015, 0.00116364445870855, 0.000176192249669189},
	         new double[] {0.0379638791013841, 0.200131440516666, 0.15513436318098, 0.0324901085655017, 0.0266527784690531, 0.0622637020776971, 0.020149558604032, 0.00116364159825902, 0.000176192249669189},
	         new double[] {0.0413644673242268, 0.21048746493804, 0.159118962711371, 0.0330247690139325, 0.0287705601961582, 0.0159849064089516, 0.020149558604032, 0.00116364159825902, 0.000176192249669189},
	         new double[] {0.0369036177249472, 0.194724169711615, 0.153130219247736, 0.0324401636863643, 0.0268261789199743, 0.0622600769346507, 0.0200696262021754, 0.00116363873780948, 0.000176417748441007},
	         new double[] {0.0402035824842089, 0.20484706463529, 0.157069570042016, 0.0329735455138521, 0.028959235447606, 0.0157381471075802, 0.0200696262021754, 0.00116363873780948, 0.000176417748441007},
	         new double[] {0.0358467030744684, 0.191120801360659, 0.151869403172925, 0.0324996524553813, 0.0236374842174697, 0.0622701047172436, 0.019989822520548, 0.00116364445870855, 0.000155404409402031},
	         new double[] {0.0390532527029818, 0.201086877859377, 0.155779696567327, 0.0330330810035449, 0.0255187840000183, 0.0154962775063211, 0.019989822520548, 0.00116364445870855, 0.000155404409402031},
	         new double[] {0.0353173697365583, 0.187746507820183, 0.150621353999509, 0.0325703541865825, 0.020448789514965, 0.0622801324998365, 0.0199099158627373, 0.00116365017960763, 0.000134391070363054},
	         new double[] {0.0384805334967222, 0.197574250118852, 0.154503025497475, 0.03310414982577, 0.0220783325524306, 0.0152461116479221, 0.0199099158627373, 0.00116365017960763, 0.000134391070363054},
	         new double[] {0.0347688928401232, 0.184236946481561, 0.149372860741303, 0.0326410559177838, 0.0172734960185403, 0.0622901598056878, 0.0198299062287433, 0.0011636559005067, 0.000113378684807256},
	         new double[] {0.0378869115554724, 0.193921184317643, 0.153225968266936, 0.0331752186479952, 0.0186524407929853, 0.0149857044266982, 0.0198299062287433, 0.0011636559005067, 0.000113378684807256},
	         new double[] {0.0280928896661684, 0.198748847953949, 0.154065147167268, 0.0314300314496892, 0.0793482387068022, 0.0600907162966103, 0.0203828511473549, 0.00113646637417617, 0.00198338231376424},
	         new double[] {0.030656295820826, 0.208590982604462, 0.157924536001713, 0.0319159481694172, 0.0849846808625056, 0.0166843904887383, 0.0203850536934983, 0.00113648925777246, 0.000198653929656969},
	         new double[] {0.0306068171949631, 0.195292900736108, 0.153118814635433, 0.031325592146426, 0.0627637198601583, 0.0605599415781787, 0.0203045534924087, 0.00113646351372663, 0.00151590238315493},
	         new double[] {0.0333866092059564, 0.205008572767262, 0.156954791643902, 0.0318077983863632, 0.0672764000899325, 0.0164379165172082, 0.020306193960218, 0.00113648067642385, 0.000197205588708166},
	         new double[] {0.0315532684354542, 0.191281454933332, 0.151908748656304, 0.0312587126909183, 0.0507203691753381, 0.0610016474282366, 0.0202261185358846, 0.00113646065327709, 0.00106966129035641},
	         new double[] {0.0344087979990583, 0.2008398680303, 0.155717555446477, 0.0317390131563516, 0.0544416159365941, 0.0161938934741895, 0.0202272083671581, 0.00113647209507524, 0.000197205588708166},
	         new double[] {0.0319924117994688, 0.187160167868342, 0.150747898856793, 0.0312221160995444, 0.0387806683097089, 0.0614370903240336, 0.0201475806031773, 0.00113645779282756, 0.000623420197557901},
	         new double[] {0.0348774254466735, 0.196560338036484, 0.154530916310684, 0.031701704313043, 0.0417157319575715, 0.0159514310447639, 0.020148119797915, 0.00113646351372663, 0.000197205588708166},
	         new double[] {0.0320433206501001, 0.182796865319166, 0.149548553127415, 0.031185114754561, 0.0272367606953638, 0.0618658831513992, 0.0200689396942866, 0.00113645493237802, 0.000177179104759388},
	         new double[] {0.0349224632246305, 0.192036259630418, 0.153304609890939, 0.0316645113179407, 0.0294040839782217, 0.0157082136950315, 0.0200689282524884, 0.00113645493237802, 0.000197205588708166},
	         new double[] {0.0306152555210967, 0.177597580660386, 0.147317395337582, 0.0311547639547507, 0.030014873622479, 0.0618457699004812, 0.0199889729670356, 0.00113645207192848, 0.000197431087479984},
	         new double[] {0.0333618591663391, 0.186604898119369, 0.151025140777024, 0.0316329867803371, 0.0323996868951937, 0.01546145439366, 0.0199889729670356, 0.00113645207192848, 0.000197431087479984},
	         new double[] {0.0295719351570415, 0.173994186565384, 0.146020898015248, 0.0312142527237677, 0.0268261789199743, 0.0618550377569804, 0.0199091692854082, 0.00113645779282756, 0.000176417748441007},
	         new double[] {0.0322261319799974, 0.182844739947951, 0.149699586054813, 0.0316925222700299, 0.028959235447606, 0.015219584792401, 0.0199091692854082, 0.00113645779282756, 0.000176417748441007},
	         new double[] {0.0290561961055552, 0.170619867280862, 0.144732301969648, 0.031284954454969, 0.0236374842174697, 0.0618643056134797, 0.0198292626275975, 0.00113646351372663, 0.000155404409402031},
	         new double[] {0.0316680153686233, 0.179332140811921, 0.148382368112776, 0.031763591092255, 0.0255187840000183, 0.014969418934002, 0.0198292626275975, 0.00113646351372663, 0.000155404409402031},
	         new double[] {0.0285213134955437, 0.167110280198195, 0.143438396214596, 0.0313556561861703, 0.020462190721045, 0.0618735729932373, 0.0197492529936035, 0.0011364692346257, 0.000134392023846233},
	         new double[] {0.0310889960222589, 0.175679103615208, 0.147059898385391, 0.0318346599144802, 0.022092892240573, 0.0147090117127781, 0.0197492529936035, 0.0011364692346257, 0.000134392023846233},
	         new double[] {0.0237410017408696, 0.178033017596913, 0.147263559531962, 0.030154308142117, 0.0780113146511653, 0.0597515866436839, 0.020222608764303, 0.0011147326785955, 0.00198315681499242},
	         new double[] {0.0259152294337969, 0.186632688816844, 0.150890405975975, 0.0305868903500065, 0.0835680518107504, 0.0161656832441159, 0.0202247998686482, 0.0011147555621918, 0.000218218927747142},
	         new double[] {0.0245718193088353, 0.17374053691782, 0.146016035966148, 0.0300742806303134, 0.065952414562663, 0.0601928004964215, 0.0201441738077789, 0.00111472981814597, 0.0015369157221939},
	         new double[] {0.0268118301899853, 0.18216485543002, 0.149614364205021, 0.030504394985365, 0.0707168515375202, 0.0159216602010972, 0.0201458142755883, 0.00111474698084319, 0.000218218927747142},
	         new double[] {0.0249818575988128, 0.169728582670138, 0.144834249821365, 0.0300366592828929, 0.0539090638778428, 0.0606289246559499, 0.0200656358750716, 0.00111472695769643, 0.00109067462939539},
	         new double[] {0.0272493788533831, 0.177998729388316, 0.148406375388997, 0.0304659233693197, 0.0578820673841818, 0.0156791977716717, 0.0200667257063451, 0.00111473839949458, 0.000218218927747142},
	         new double[] {0.0251160341354606, 0.165808167813421, 0.14368333293287, 0.0300252303567686, 0.0419693630122135, 0.0610584759791843, 0.0199869949661809, 0.00111472409724689, 0.000644433536596877},
	         new double[] {0.0273860296788802, 0.173936593559297, 0.147230188335811, 0.0304545745357824, 0.0451561834051592, 0.0154359804219392, 0.0199875341609186, 0.00111472981814597, 0.000218218927747142},
	         new double[] {0.0251982005484054, 0.161294301212201, 0.142379219663645, 0.0299908220092904, 0.0305988558487898, 0.0614798001728093, 0.0199082510811069, 0.00111472123679736, 0.000198417942570183},
	         new double[] {0.0274667801693043, 0.169261923926917, 0.14589665224428, 0.0304202219670701, 0.0330332106772572, 0.0151893012131548, 0.0199082396393088, 0.00111472123679736, 0.000218444426518961},
	         new double[] {0.0238347458233146, 0.157916549388236, 0.140855708484723, 0.0300699048576345, 0.030014873622479, 0.0614725827818863, 0.0198284130740851, 0.00111472695769643, 0.000197431087479984},
	         new double[] {0.0259831865636676, 0.165729726420885, 0.144341021077508, 0.0304994564192397, 0.0323996868951937, 0.0149474316118957, 0.0198284130740851, 0.00111472695769643, 0.000197431087479984},
	         new double[] {0.0233527028673718, 0.154552622116881, 0.139532242129044, 0.0301406065888357, 0.0268261789199743, 0.0614811107354387, 0.0197485064162744, 0.0011147326785955, 0.000176417748441007},
	         new double[] {0.0254615120793923, 0.162228380293333, 0.142988932825394, 0.0305705252414648, 0.028959235447606, 0.0146972657534968, 0.0197485064162744, 0.0011147326785955, 0.000176417748441007},
	         new double[] {0.022851516352904, 0.151053427047381, 0.138199411376694, 0.030211308320037, 0.0236508854235496, 0.0614896382122496, 0.0196684967822803, 0.00111473839949458, 0.00015540536288521},
	         new double[] {0.0249189348601269, 0.158586596105097, 0.141627538100711, 0.03064159406369, 0.0255333436881607, 0.0144368585322728, 0.0196684967822803, 0.00111473839949458, 0.00015540536288521},
	         new double[] {0.0194458007742665, 0.159299809150807, 0.141398242082133, 0.0291764153170589, 0.0761043366131248, 0.0594424678738145, 0.0200623577999024, 0.0010978531658788, 0.00198027109815137},
	         new double[] {0.0212338105707341, 0.166773736153279, 0.144835725336584, 0.0295658605157085, 0.0815069734899258, 0.015675026044393, 0.0200645489042476, 0.0010978760494751, 0.000216770586798339},
	         new double[] {0.0197402053917185, 0.154949099730717, 0.140174160615386, 0.0291270174988954, 0.0646916535515055, 0.0598781000360226, 0.019983819867195, 0.00109785030542927, 0.0015354039746137},
	         new double[] {0.0215457711972183, 0.162248064476821, 0.14358417044389, 0.0295151661987925, 0.0693522297490184, 0.0154325636149675, 0.0199854603350044, 0.00109786746812649, 0.000216770586798339},
	         new double[] {0.0198602084009115, 0.151101909521693, 0.139004611473721, 0.0291157163395171, 0.0527998780876407, 0.0603083326229884, 0.0199051789583044, 0.00109784744497973, 0.00108922485822182},
	         new double[] {0.0216676907076008, 0.158261064075786, 0.142389137318931, 0.0295038607487398, 0.056681455190772, 0.015189346265235, 0.0199062687895779, 0.00109785888677788, 0.000216770586798339},
	         new double[] {0.0200032451799995, 0.147004435699097, 0.13775258055455, 0.0291068804108979, 0.0410536794820527, 0.0607304339054042, 0.0198264350732304, 0.00109784458453019, 0.000643210694419892},
	         new double[] {0.0218155330419107, 0.154020401870276, 0.141109567183474, 0.0294953523415926, 0.0441660859954107, 0.0149426670564506, 0.0198269742679681, 0.00109785030542927, 0.000216996085570157},
	         new double[] {0.0198044725416868, 0.144005095604805, 0.13713300217909, 0.0291756713618085, 0.026321077165203, 0.0611587628632032, 0.0197478199083855, 0.00109785030542927, 0.000175956262582402},
	         new double[] {0.0215968302214503, 0.150916376474061, 0.140475573095832, 0.0295652498097324, 0.0284139865684732, 0.0147008775477786, 0.0197478084665874, 0.00109785030542927, 0.000195982746531181},
	         new double[] {0.0190681499241695, 0.140894910230512, 0.13558414384171, 0.0292659671723369, 0.0257370949388922, 0.0611516074486869, 0.0196678789251804, 0.00109785602632834, 0.000174969407492204},
	         new double[] {0.0207991080546254, 0.147673108685068, 0.138895030274044, 0.0296560175944344, 0.0277804627864097, 0.0144507116893796, 0.0196678789251804, 0.00109785602632834, 0.000174969407492204},
	         new double[] {0.0186207613143651, 0.137414508314469, 0.134218875591612, 0.0293366689035382, 0.0225618014424675, 0.0611594150456976, 0.0195878692911863, 0.00109786174722741, 0.000153957021936406},
	         new double[] {0.0203148124946724, 0.144051702339333, 0.137501198051614, 0.0297270864166596, 0.0243545710269643, 0.0141903044681557, 0.0195878692911863, 0.00109786174722741, 0.000153957021936406},
	         new double[] {0.0150189762222272, 0.141771374852186, 0.135925600708705, 0.0284132764618756, 0.0761043366131248, 0.0591624565616901, 0.0199019008831352, 0.00108520902544294, 0.00198027109815137},
	         new double[] {0.0164071380801921, 0.148228832530405, 0.139201259036398, 0.0287694446208388, 0.0815069734899258, 0.0151883803867747, 0.0199040919874804, 0.00108523190903923, 0.000216770586798339},
	         new double[] {0.0149798881793067, 0.13761659478357, 0.134716210510779, 0.0283925038773393, 0.0646916535515055, 0.0595919716525637, 0.0198232599742445, 0.0010852061649934, 0.0015354039746137},
	         new double[] {0.0163561720205701, 0.143914917072707, 0.137965313616827, 0.0287483287823581, 0.0693522297490184, 0.0149451630370422, 0.0198249004420539, 0.00108522332769062, 0.000216770586798339},
	         new double[] {0.015118705795328, 0.133590298957023, 0.133448964860522, 0.0283847177337002, 0.052973278538562, 0.0600147913845548, 0.0197445160891705, 0.00108520330454386, 0.00108945035699364},
	         new double[] {0.0165001813525006, 0.139747336492414, 0.136670482267978, 0.0287408286836726, 0.0568701304422198, 0.0146984838282578, 0.019745605920444, 0.00108521474634201, 0.000216996085570157},
	         new double[] {0.0149821479344408, 0.130987395715161, 0.132886235774269, 0.0284790811034699, 0.037864984779548, 0.0604431203423539, 0.0196659009243257, 0.00108520902544294, 0.000622197355380916},
	         new double[] {0.0163495929866354, 0.137056402916057, 0.13609538197585, 0.0288365703133775, 0.040725634547823, 0.0144566943195858, 0.0196664401190634, 0.00108521474634201, 0.000195982746531181},
	         new double[] {0.0151266006360496, 0.128004763506614, 0.132222871067525, 0.0285542336941504, 0.0231323824626983, 0.0608668492205561, 0.0195871827832975, 0.00108521474634201, 0.000154942923543426},
	         new double[] {0.0165075613123056, 0.133973080023364, 0.135417045914601, 0.0289129395485944, 0.0249735351208855, 0.0142066085537739, 0.0195871713414993, 0.00108521474634201, 0.000174969407492204},
	         new double[] {0.0144706852550119, 0.124797801973367, 0.130635454585501, 0.0286445295046788, 0.0225618014424675, 0.0608597553057048, 0.019507138823909, 0.00108522046724108, 0.000153957021936406},
	         new double[] {0.0157971972743348, 0.130630744855338, 0.133798002872273, 0.0290037073332963, 0.0243545710269643, 0.01394620133255, 0.019507138823909, 0.00108522046724108, 0.000153957021936406},
	         new double[] {0.0107493405233593, 0.126422920610511, 0.130855868669897, 0.0278780000633113, 0.0761043366131248, 0.0589095008115095, 0.0197412380140013, 0.00107619002805338, 0.00198027109815137},
	         new double[] {0.0117500330381922, 0.132026187987599, 0.133994633033882, 0.0282106882222516, 0.0815069734899258, 0.0147025404224426, 0.0197434291183465, 0.00107621291164968, 0.000216770586798339},
	         new double[] {0.0107099235287421, 0.122155096291313, 0.129555400328494, 0.0278639695583332, 0.0648650540024268, 0.0593316118671279, 0.0196624941289273, 0.00107618716760385, 0.00153562947338552},
	         new double[] {0.0117002540651279, 0.127602710161518, 0.132665868172178, 0.0281967549725578, 0.0695409050004663, 0.0144558612136582, 0.0196641345967367, 0.00107620433030107, 0.000216996085570157},
	         new double[] {0.0106306032630864, 0.119647420274981, 0.128981987463222, 0.0279600742267585, 0.0497845838360573, 0.059759940824927, 0.0195838789640825, 0.00107619288850292, 0.00106843701795466},
	         new double[] {0.0116123309974902, 0.125010639442052, 0.132080172774966, 0.0282942286044572, 0.0534296789946321, 0.0142140717049862, 0.019584968795356, 0.00107620433030107, 0.000195982746531181},
	         new double[] {0.0108248277866356, 0.117049876085326, 0.12838135241482, 0.0280607992362981, 0.0346762900770433, 0.0601836697031292, 0.0195051608230543, 0.00107619860940199, 0.000601184016341939},
	         new double[] {0.0118247908868366, 0.122328554666337, 0.131466739598595, 0.0283964420012391, 0.0372851831002353, 0.0139639859391743, 0.019505700017792, 0.00107620433030107, 0.000174969407492204},
	         new double[] {0.0108151737694489, 0.113751960838157, 0.127665662934924, 0.0281324835319153, 0.0199539510531317, 0.06060322899939, 0.0194263397058428, 0.00107620433030107, 0.000133929584504449},
	         new double[] {0.0118146005353617, 0.11891317219849, 0.130735338622877, 0.0284691970584663, 0.0215440163114274, 0.0137036588105374, 0.0194263282640447, 0.00107620433030107, 0.000153957021936406},
	         new double[] {0.00715480667079716, 0.11305582682152, 0.126000009582506, 0.0275335502126172, 0.076277737064046, 0.0586808800973926, 0.0195803691925008, 0.00107018594447562, 0.00198049659692319},
	         new double[] {0.00782691219621311, 0.117967235838915, 0.129019536822662, 0.0278514405509912, 0.0816956487413736, 0.0142124836787517, 0.019582560296846, 0.00107020882807192, 0.000216996085570157},
	         new double[] {0.00699029506681152, 0.110441574746121, 0.125404233722755, 0.0276261880162039, 0.0616763592999221, 0.059107977631666, 0.019501754027656, 0.0010701916653747, 0.00151461613434654},
	         new double[] {0.00764622475009683, 0.11526153090115, 0.128411057229777, 0.0279453357605201, 0.0661004535528786, 0.0139706941700797, 0.0195033944954653, 0.00107020882807192, 0.000195982746531181},
	         new double[] {0.00720442831913692, 0.107935243856183, 0.124798559277382, 0.027729115333516, 0.0465958891335527, 0.0595317065098683, 0.0194230358866278, 0.00107019738627377, 0.00104742367891568},
	         new double[] {0.00788048126491367, 0.112674148458475, 0.127792776306553, 0.0280497636219851, 0.0499892275470444, 0.0137206084042677, 0.0194241257179013, 0.00107020882807192, 0.000174969407492204},
	         new double[] {0.00724454612389065, 0.105022416627908, 0.124152593254946, 0.0278263720479922, 0.0314978586674767, 0.059951265806129, 0.0193442147694163, 0.00107020310717284, 0.000580170677302962},
	         new double[] {0.00792478247711497, 0.109660004107606, 0.127133368555002, 0.0281483628407773, 0.0338556642907773, 0.0134602812756308, 0.019344753964154, 0.00107020882807192, 0.000153957021936406},
	         new double[] {0.0046199728314503, 0.103372264051529, 0.121754777808861, 0.0274419562347407, 0.0764109338967275, 0.0584674927072839, 0.0194194917896517, 0.00106659512682375, 0.00198071923524547},
	         new double[] {0.00506002081263083, 0.107812188032222, 0.124678615860888, 0.0277530258311942, 0.0818406449283944, 0.0137238547761212, 0.019421694335795, 0.00106661801042005, 0.000196208245302999},
	         new double[] {0.00478069433979967, 0.100848382158346, 0.121136827744301, 0.0275426833896174, 0.0618095561326036, 0.0588892669449693, 0.0193407736486235, 0.00106660084772282, 0.00151483877266882},
	         new double[] {0.00523608148162133, 0.105205131417633, 0.124047839063863, 0.0278552020421321, 0.0662454497398993, 0.0134737690103093, 0.019342425558231, 0.00106661801042005, 0.000175194906264022},
	         new double[] {0.00484569805552361, 0.0980976094079041, 0.120493131488572, 0.027642723321493, 0.0467290859662341, 0.0593088262412301, 0.019261952531412, 0.0010666065686219, 0.00104764631723797},
	         new double[] {0.00530762847566072, 0.102359674927073, 0.123391014298243, 0.0279566173734933, 0.0501342237340651, 0.0132134418816724, 0.0192630538044837, 0.00106661801042005, 0.000154182520708224},
	         new double[] {0.00362257698470861, 0.0972710110030052, 0.119458007742665, 0.0275736522848985, 0.0579746444032158, 0.0587024695786424, 0.0192598629730254, 0.00106480734586325, 0.001492451464369},
	         new double[] {0.00396843751376591, 0.101451643814535, 0.122333384058638, 0.0278845063941535, 0.0621085417599888, 0.0132318994090502, 0.0192615148826329, 0.00106482450856047, 0.000154181567225045},
	         new double[] {0.00368758070043256, 0.0943639204062754, 0.11878586646163, 0.0276722977476248, 0.043388816042774, 0.0591192399366048, 0.0191810418558139, 0.00106481306676232, 0.00102657100179236},
	         new double[] {0.00403998450780531, 0.0984440885091737, 0.121647442062071, 0.0279845143843426, 0.0465297626913855, 0.0129715722804133, 0.0191821431288856, 0.00106482450856047, 0.000133169181669248},
	         new double[] {0.00368818854595913, 0.0914915070392803, 0.117212702169422, 0.0277525564790994, 0.0395740061225062, 0.0589569284950546, 0.0191000282040324, 0.00106420379101098, 0.0010041851237173},
	         new double[] {0.00404105717638161, 0.0954661989259584, 0.120045861110682, 0.0280655628382601, 0.0424146942436886, 0.0127214064220143, 0.0191011294771041, 0.00106421523280913, 0.000112155842630271},
        };
        /// This table is used by HandPlayerOpponentOdds and contains the odds of each type of 
        /// mask occuring for a random player when the board is currently empty. This calculation
        /// normally takes about 5 minutes, so the values are precalculated to save time.
        private static readonly double[][] PreCalcOppOdds = {
	         new double[] {0, 0.000176203691467336, 0.0326410244528389, 0.0291796688400362, 0.0451538349760895, 0.021233850140286, 0.0176688366036853, 0.0015772518745956, 0.00033219639999077},
	         new double[] {0, 0.0144409365798291, 0.045186784494304, 0.0292916954856958, 0.0450247457489429, 0.0217730639476378, 0.018371218080482, 0.00162264530177838, 0.000332112493471024},
	         new double[] {0, 0.0285658649970795, 0.0573239407612343, 0.0293464073039863, 0.0421463011240995, 0.0223154194820641, 0.0190763112634396, 0.00166049477004942, 0.000313619687215564},
	         new double[] {0, 0.0424026202861937, 0.069600123457002, 0.029400401149443, 0.0392824161873983, 0.0228486578103335, 0.019784425081108, 0.00169149822909569, 0.000295128787926462},
	         new double[] {0, 0.0559512024471718, 0.0821974330897947, 0.0294536770220661, 0.0364185312506972, 0.0233728170717731, 0.0204955595334874, 0.00171634218680604, 0.00027663788863736},
	         new double[] {0, 0.0699936898483218, 0.095626857504418, 0.0295849144468148, 0.0371309061846924, 0.0238678664917597, 0.0212100235491276, 0.00173572459286745, 0.00027749793046476},
	         new double[] {0, 0.0838198328696545, 0.109404126884965, 0.0297033799643817, 0.0371309061846924, 0.0243560727629711, 0.0219275081994786, 0.0017503205133706, 0.00027749793046476},
	         new double[] {0, 0.0976459758909871, 0.123513558816849, 0.0298218454819486, 0.0371309061846924, 0.0248411578069963, 0.0226480134845405, 0.00176081645620432, 0.00027749793046476},
	         new double[] {0, 0.111499902458671, 0.137836709235877, 0.0299403109995154, 0.0371309061846924, 0.0253277526916353, 0.0233715394043133, 0.00176789892925746, 0.00027749793046476},
	         new double[] {0, 0.125140176329551, 0.152157898816746, 0.0300452866370667, 0.0374386791130547, 0.0258193300026259, 0.0240980859587969, 0.00177225444041884, 0.00027908357299133},
	         new double[] {0, 0.139198427668099, 0.166821392672787, 0.0302260699082425, 0.0403025640497558, 0.0263232163047149, 0.0248279620765414, 0.00177458093937544, 0.000297574472280432},
	         new double[] {0, 0.153544852134782, 0.181454191044848, 0.030407571152252, 0.0431664489864569, 0.0268436951210838, 0.0255608588289968, 0.00177555349221796, 0.000316065371569534},
	         new double[] {0, 0.168184993280804, 0.196062845792593, 0.0305897903690952, 0.046030333923158, 0.0273846547561362, 0.026296776216163, 0.00177585860683522, 0.000334556270858636},
	         new double[] {0.000778585759423608, 0.0857216227673476, 0.120254482753492, 0.0303105118087938, 0.0415814374273803, 0.0263834292442063, 0.0227276245625658, 0.00145477672189051, 0.000341205862548535},
	         new double[] {0.000863083438740899, 0.0931761058640932, 0.128900488011761, 0.0320056885759938, 0.0442699160229225, 0.0230610004212489, 0.0227356824489109, 0.00145516288257797, 0.000332154446730897},
	         new double[] {0.001943975807462, 0.0912532888018549, 0.12236401422902, 0.0305207820240198, 0.0408509186619732, 0.0263836695219674, 0.022806681666864, 0.00145477386144097, 0.00033327145227502},
	         new double[] {0.00213885823440469, 0.0989645801975655, 0.131040601983512, 0.0322232686700111, 0.0433906052539593, 0.0233308843117882, 0.0228147395532092, 0.00145516002212844, 0.000322908043603167},
	         new double[] {0.00309443430891825, 0.0971088320956168, 0.124351205708084, 0.0306994957599557, 0.0397706327085539, 0.0263839097997285, 0.0228858417473456, 0.00145477100099143, 0.000324086548812332},
	         new double[] {0.00339828556096562, 0.105110615013813, 0.133056615352109, 0.0324073986671449, 0.042135577298786, 0.0235962095992491, 0.0228938996336908, 0.0014551571616789, 0.000313662593958616},
	         new double[] {0.00422996126379237, 0.103003240317235, 0.126203923163749, 0.0308411666743899, 0.0385789751524191, 0.0263841500774896, 0.0229651048040106, 0.00145476814054189, 0.000314842529392549},
	         new double[] {0.0046413654184237, 0.111305110135889, 0.134934626332803, 0.0325524034355143, 0.0407602188129478, 0.0238569953532951, 0.0229731626903558, 0.00145515430122936, 0.000304417144314065},
	         new double[] {0.00500605843211896, 0.114119995572024, 0.131034367872117, 0.0315056648342627, 0.0393253293664619, 0.0263829272353126, 0.0230527604196165, 0.00145516002212844, 0.000305597079747998},
	         new double[] {0.00549146241626749, 0.123138150559189, 0.140011536192982, 0.033258019127254, 0.0414562477080648, 0.0241039126945034, 0.0230526059553415, 0.00145515430122936, 0.000304847165227765},
	         new double[] {0.00629338944391145, 0.119787641656612, 0.132650986683463, 0.0315623746765547, 0.0389923058198134, 0.026382961560707, 0.0231322294286481, 0.0014551571616789, 0.000304847165227765},
	         new double[] {0.00690074154293792, 0.129060170700187, 0.141642831970901, 0.033313468941525, 0.0411164634889361, 0.0243480387137054, 0.0231322294286481, 0.0014551571616789, 0.000304847165227765},
	         new double[] {0.00767206891166188, 0.125861554528463, 0.134622862600595, 0.0316566980000309, 0.0389923058198134, 0.026382961560707, 0.023211955878138, 0.00145516002212844, 0.000304847165227765},
	         new double[] {0.00841015785676814, 0.135427291091359, 0.143660736096642, 0.0334108529460056, 0.0411164634889361, 0.024590581235718, 0.023211955878138, 0.00145516002212844, 0.000304847165227765},
	         new double[] {0.00911472233330301, 0.132281325784035, 0.136963206609698, 0.031792725247529, 0.0393540640122839, 0.0263829272353126, 0.0232919397680862, 0.00145516860347705, 0.00030563998649105},
	         new double[] {0.00998974099773624, 0.142176993747629, 0.146078837135729, 0.0335545247448908, 0.0414897292698931, 0.0248338557944412, 0.0232917853038112, 0.00145516288257797, 0.000304847165227765},
	         new double[] {0.0095779435312936, 0.132500300347201, 0.136124438422245, 0.0313238489408041, 0.0397266575875998, 0.0263841500774896, 0.0233637370514601, 0.00145478244278958, 0.000314885436135601},
	         new double[] {0.0104964839354294, 0.142212007080185, 0.14508643897107, 0.0330513416366462, 0.0419755294263025, 0.0250802518187215, 0.0233717949378052, 0.00145516860347705, 0.00030563998649105},
	         new double[] {0.0101446128867828, 0.137313853385943, 0.138252359727845, 0.0314076386588611, 0.0409384169528546, 0.0263839097997285, 0.0234439268937749, 0.00145479102413819, 0.000324130885780152},
	         new double[] {0.0111170155556967, 0.147235073745249, 0.147282583428348, 0.0331402873149933, 0.0433727274443542, 0.0253334888464398, 0.0234519847801201, 0.00145517718482566, 0.000314885436135601},
	         new double[] {0.0101886781118974, 0.141926414077531, 0.140355244948875, 0.0314802154147337, 0.0421501763181094, 0.0263836695219674, 0.0235242197122731, 0.0014547996054868, 0.000333376335424703},
	         new double[] {0.0111653857573641, 0.152041088069237, 0.1494532613034, 0.0332176996608079, 0.044769925462406, 0.0255950221312981, 0.0235322775986183, 0.00145518576617427, 0.000324130885780152},
	         new double[] {0.0102601178390791, 0.146768933458507, 0.142476464221211, 0.0315580954440476, 0.0433619356833643, 0.0263834292442063, 0.0236046155069546, 0.00145480818683541, 0.000342621785069254},
	         new double[] {0.0112437263190534, 0.157090256336325, 0.151644434299383, 0.0333008329056961, 0.0461671234804577, 0.0258667958254981, 0.0236126733932998, 0.00145519434752288, 0.000333376335424703},
	         new double[] {0.00938340435829533, 0.106536486178022, 0.126920558737329, 0.0314315648890117, 0.0401629233393803, 0.026846661883995, 0.022878916599017, 0.00149431099493872, 0.000341140072209188},
	         new double[] {0.0102295825402737, 0.115235952761392, 0.135750875392907, 0.0331660885698153, 0.0427453951053132, 0.0236011233748118, 0.0228950323717074, 0.00149508331631366, 0.000322866090343294},
	         new double[] {0.0105070604475917, 0.112156171105226, 0.128851791242104, 0.031602562562322, 0.0395772791918887, 0.0268460821662222, 0.0229580766794986, 0.00149430813448918, 0.000333267161600715},
	         new double[] {0.0114598904905499, 0.121131043200225, 0.137708093651499, 0.0333419647398107, 0.0420228140873707, 0.0238664486622726, 0.022974192452189, 0.00149508045586412, 0.000313620640698743},
	         new double[] {0.0116291861963859, 0.117839568255189, 0.130685537481328, 0.0317417806412785, 0.0385170950475893, 0.0268455219948546, 0.0230373397361636, 0.00149430527403965, 0.000324083688362795},
	         new double[] {0.0126884106598657, 0.127101650937055, 0.139566572052531, 0.0334844465916886, 0.0407896256644109, 0.0241272344163186, 0.0230534555088539, 0.00149507759541459, 0.000304375191054192},
	         new double[] {0.0124732404945832, 0.129018431020545, 0.13558351096725, 0.0324139762708548, 0.0388480519194475, 0.0268434991802905, 0.0231249953517695, 0.00149469715562619, 0.000314842529392549},
	         new double[] {0.0136129651591526, 0.138997545448252, 0.144713078795278, 0.034197968565948, 0.0410522621293072, 0.0243741517575269, 0.0231328987738397, 0.00149507759541459, 0.000304805211967892},
	         new double[] {0.0140899808750344, 0.138915287500922, 0.139821439298114, 0.0329521498280584, 0.0392351565552636, 0.0268492067305996, 0.0232125994792838, 0.00149508331631366, 0.000305597079747998},
	         new double[] {0.0153791139700351, 0.149504482896514, 0.14915529995532, 0.0347687870034903, 0.0413606400427466, 0.024617622733785, 0.0232124450150088, 0.00149507759541459, 0.000304805211967892},
	         new double[] {0.0153788350762052, 0.144080950435847, 0.141375266236341, 0.0329706011577956, 0.038876536275935, 0.0268483786304587, 0.0232921714644987, 0.00149508045586412, 0.000304805211967892},
	         new double[] {0.0167909031411741, 0.154886267096192, 0.150721561982795, 0.0347845194759428, 0.0409910013118022, 0.0248601881393939, 0.0232921714644987, 0.00149508045586412, 0.000304805211967892},
	         new double[] {0.0167393292360254, 0.149620906053112, 0.143304048766088, 0.0330307564115546, 0.038876536275935, 0.0268491786028458, 0.0233720008901719, 0.00149508331631366, 0.000304805211967892},
	         new double[] {0.0182803535172374, 0.160679432566904, 0.152694508423166, 0.0348465397427998, 0.0409910013118022, 0.0251034855817134, 0.0233720008901719, 0.00149508331631366, 0.000304805211967892},
	         new double[] {0.0179919558438126, 0.15433230242732, 0.145104149682748, 0.0330549529541865, 0.0394830002053803, 0.0268518450185557, 0.0234519332920284, 0.0014950861767632, 0.000305598033231177},
	         new double[] {0.0196486495531692, 0.165587838112286, 0.154532129141287, 0.0348709179239773, 0.0416188928687277, 0.0253492265630497, 0.0234519332920284, 0.0014950861767632, 0.000305598033231177},
	         new double[] {0.018585234531118, 0.159245210320273, 0.147295221132772, 0.0331387426722434, 0.0406947595706351, 0.0268524047131818, 0.0235321231343433, 0.00149509475811181, 0.000314843482875728},
	         new double[] {0.0202982576429781, 0.170714719072391, 0.1567918227757, 0.0349598636023243, 0.0430160908867794, 0.0256024635907681, 0.0235321231343433, 0.00149509475811181, 0.000314843482875728},
	         new double[] {0.0186509319058546, 0.163929043402745, 0.149467849119296, 0.033211319428116, 0.0419065189358899, 0.0268529644078078, 0.0236124159528415, 0.00149510333946042, 0.000324088932520279},
	         new double[] {0.0203702551578196, 0.175594040997107, 0.15903263744317, 0.0350372759481389, 0.0444132889048311, 0.0258639968756263, 0.0236124159528415, 0.00149510333946042, 0.000324088932520279},
	         new double[] {0.0187223716330364, 0.168748144283363, 0.151635244628505, 0.0332838961839887, 0.0431182783011447, 0.0268535236256923, 0.023692811747523, 0.00149511192080903, 0.00033333438216483},
	         new double[] {0.0204485957195089, 0.180613800982507, 0.161268161709222, 0.0351146882939535, 0.0458104869228829, 0.0261357705698263, 0.023692811747523, 0.00149511192080903, 0.00033333438216483},
	         new double[] {0.0172662526452007, 0.125596476669888, 0.132139757130672, 0.0324108188113078, 0.0378316412344098, 0.0272736855233221, 0.02303026012356, 0.00152716325786895, 0.000331893192339869},
	         new double[] {0.018810745221476, 0.135424968406335, 0.14107351956004, 0.03417585776777, 0.0401439177975454, 0.0241369647121596, 0.0230544337825955, 0.00152832173993136, 0.000304374237571013},
	         new double[] {0.0183416672530588, 0.131015885792548, 0.133925078819687, 0.0325426655118078, 0.0372660988960381, 0.0272723053564206, 0.0231095231802249, 0.00152716039741942, 0.000324021711956164},
	         new double[] {0.0199883493890366, 0.141114542697072, 0.142880763019193, 0.0343104476393759, 0.0394431763118165, 0.0243977504662056, 0.0231336968392605, 0.00152831887948182, 0.000295128787926462},
	         new double[] {0.0191906987334502, 0.142159968351986, 0.138860754460728, 0.0332216713949898, 0.0376146160676027, 0.0272695016391329, 0.0231971787958308, 0.00152755227900596, 0.000314841099167781},
	         new double[] {0.0209183530446911, 0.152970112020925, 0.148066266985588, 0.0350310830748917, 0.0397281500271457, 0.0246446678074139, 0.0232131401042462, 0.00152831887948182, 0.000295558808840162},
	         new double[] {0.02078848863572, 0.152085104666709, 0.14316275757633, 0.0337675424218969, 0.0376260292612546, 0.0272744292402017, 0.0232847829233451, 0.00152793843969343, 0.000305597079747998},
	         new double[] {0.0226636420273264, 0.163505530965224, 0.152574264182729, 0.0356098077949538, 0.0396441314731258, 0.024888138783672, 0.0232926863454153, 0.00152831887948182, 0.000295558808840162},
	         new double[] {0.0229429792268434, 0.161885545404774, 0.147375023384175, 0.0342778408983642, 0.0380131338970707, 0.0272856789114883, 0.0233724900270427, 0.0015283246003809, 0.000296351630103447},
	         new double[] {0.0250161758421306, 0.17390628852668, 0.156990344409566, 0.0361517600059955, 0.0399525093865652, 0.025130049146337, 0.0233723355627677, 0.00152831887948182, 0.000295558808840162},
	         new double[] {0.0240675792644869, 0.166443421452342, 0.148889000684792, 0.0342621241583842, 0.0376513757046002, 0.0272847888349408, 0.0234521649884409, 0.00152832173993136, 0.000295558808840162},
	         new double[] {0.0262488651166463, 0.17863493198137, 0.158514892501446, 0.0361321287408244, 0.0395792436056081, 0.0253733694722528, 0.0234521649884409, 0.00152832173993136, 0.000295558808840162},
	         new double[] {0.0252616238657602, 0.171131965695201, 0.15072397191153, 0.0342863207010161, 0.0382578396340455, 0.027287395657952, 0.0235320973902975, 0.0015283246003809, 0.000296351630103447},
	         new double[] {0.0275534732436411, 0.183518789148827, 0.160387383529646, 0.0361565069220018, 0.0402071351625336, 0.0256191104535891, 0.0235320973902975, 0.0015283246003809, 0.000296351630103447},
	         new double[] {0.0258614100757619, 0.176055317089412, 0.152955590233739, 0.034370110419073, 0.0394695989993003, 0.0272887353018184, 0.0236122872326123, 0.00152833318172951, 0.000305597079747998},
	         new double[] {0.0282103182707782, 0.188656865908419, 0.162687624036243, 0.0362454526003489, 0.0416043331805853, 0.0258723474813074, 0.0236122872326123, 0.00152833318172951, 0.000305597079747998},
	         new double[] {0.0259336149731947, 0.180749593673143, 0.155174441654553, 0.0344426871749457, 0.0406813583645551, 0.0272900749456848, 0.0236925800511105, 0.00152834176307812, 0.000314842529392549},
	         new double[] {0.0282895527229477, 0.193547383632622, 0.164974662138003, 0.0363228649461635, 0.0430015311986371, 0.0261338807661657, 0.0236925800511105, 0.00152834176307812, 0.000314842529392549},
	         new double[] {0.0260115622230727, 0.18557913805502, 0.157393737160157, 0.0345152639308183, 0.0418931177298099, 0.0272914141128096, 0.023772975845792, 0.00152835034442673, 0.0003240879790371},
	         new double[] {0.0283751302219652, 0.198578339417509, 0.16726208640045, 0.0364002772919781, 0.0443987292166888, 0.0264056544603657, 0.023772975845792, 0.00152835034442673, 0.0003240879790371},
	         new double[] {0.0241754182120245, 0.142662453510544, 0.136388411432187, 0.0332386226573157, 0.0355360103422413, 0.0276621059659252, 0.0231818096004696, 0.0015539494608148, 0.000322647742695318},
	         new double[] {0.0263329694841523, 0.153488246698898, 0.145375219944732, 0.0350267104010331, 0.0375806961418829, 0.0246637079130141, 0.0232140411458503, 0.00155549410356467, 0.000285883338281911},
	         new double[] {0.0249827157336738, 0.153978036467299, 0.141327931517406, 0.0339259228906711, 0.0363636199160515, 0.0276584822531036, 0.0232694652160755, 0.00155434134240134, 0.00031477912276115},
	         new double[] {0.0272175062944192, 0.165523319242759, 0.15056373024359, 0.0357558575808873, 0.0383817006745512, 0.0249106252542225, 0.0232934844108361, 0.00155549410356467, 0.000286313359195611},
	         new double[] {0.0265814638865386, 0.163837787196285, 0.145664418782398, 0.0344769906392742, 0.0364028567023479, 0.027662648497854, 0.0233570693435898, 0.00155472750308881, 0.00030559564952323},
	         new double[] {0.0289637320742779, 0.175987578783931, 0.155107165550043, 0.036340007143496, 0.0383309520090939, 0.0251540962304805, 0.0233730306520051, 0.00155549410356467, 0.000286313359195611},
	         new double[] {0.0287196856709213, 0.173714533286193, 0.149939332725774, 0.0349949865854452, 0.0364142698959998, 0.0276731382430471, 0.0234447764472874, 0.00155511366377628, 0.000296351630103447},
	         new double[] {0.0312981735457618, 0.186467859702959, 0.159587337724314, 0.0368898656370574, 0.0382469334550741, 0.0253960065931455, 0.0234526798693575, 0.00155549410356467, 0.000286313359195611},
	         new double[] {0.0311667001339262, 0.183283058787387, 0.154144205701791, 0.0354801173966629, 0.0368013745318159, 0.0276902794868964, 0.0235325865271683, 0.00155549982446375, 0.000287106180458896},
	         new double[] {0.0339690610917649, 0.196618396580733, 0.163995731208134, 0.0374058578383278, 0.0385553113685134, 0.0256386718761174, 0.0235324320628933, 0.00155549410356467, 0.000286313359195611},
	         new double[] {0.0320071145100879, 0.186938571703175, 0.155565993574286, 0.0354284419455557, 0.0370460802687907, 0.0276903352656623, 0.0236123644647498, 0.00155549696401421, 0.000287106180458896},
	         new double[] {0.0348886813156008, 0.20038306568107, 0.16542144576273, 0.037348584487477, 0.0388099371444819, 0.02588443574105, 0.0236123644647498, 0.00155549696401421, 0.000287106180458896},
	         new double[] {0.0325933064336659, 0.191861948841432, 0.157833293144017, 0.0355122316636127, 0.0382578396340455, 0.0276924348356224, 0.0236925543070647, 0.00155550554536282, 0.000296351630103447},
	         new double[] {0.0355309237478525, 0.205521113836166, 0.167757367516849, 0.0374375301658241, 0.0402071351625336, 0.0261376727687683, 0.0236925543070647, 0.00155550554536282, 0.000296351630103447},
	         new double[] {0.0326519170446751, 0.196556251169209, 0.160092691437015, 0.0355848084194853, 0.0394695989993003, 0.0276945344055824, 0.0237728471255629, 0.00155551412671143, 0.000305597079747998},
	         new double[] {0.0355955556051367, 0.210411602955874, 0.170084952490794, 0.0375149425116387, 0.0416043331805853, 0.0263992060536266, 0.0237728471255629, 0.00155551412671143, 0.000305597079747998},
	         new double[] {0.0327162700081294, 0.201385821295131, 0.162357399439466, 0.035657385175358, 0.0406813583645551, 0.0276966334988008, 0.0238532429202444, 0.00155552270806004, 0.000314842529392549},
	         new double[] {0.0356665305092687, 0.215442530136266, 0.172417789250087, 0.0375923548574533, 0.0430015311986371, 0.0266709797478266, 0.0238532429202444, 0.00155552270806004, 0.000314842529392549},
	         new double[] {0.0299178278661561, 0.164330744912547, 0.143122944409452, 0.0344778208847523, 0.0346335313622548, 0.0280109096591851, 0.0233418546125035, 0.00157567457504685, 0.000313405153500304},
	         new double[] {0.0325850111300091, 0.176506234063721, 0.152351058299585, 0.0363214957443185, 0.0365192205046176, 0.0251720431676161, 0.0233739316936092, 0.00157721349689765, 0.00027706790955106},
	         new double[] {0.0314748420602788, 0.174210417242332, 0.147459907939292, 0.0350339559197098, 0.0351518605507967, 0.0280142559084015, 0.0234294587400177, 0.00157606073573432, 0.000305533673116599},
	         new double[] {0.0342857700644803, 0.186989146596322, 0.156893790650563, 0.0369107798138458, 0.0369845026564995, 0.0254155141438741, 0.0234534779347783, 0.00157721349689765, 0.00027706790955106},
	         new double[] {0.0336100031636572, 0.184070676416223, 0.151768115369939, 0.0355557655602257, 0.0351910973370931, 0.0280240038436814, 0.0235171658437153, 0.00157644689642179, 0.000296350199878679},
	         new double[] {0.0366166359740431, 0.197450827442238, 0.161407978575614, 0.0374646157624881, 0.0369337539910422, 0.0256574245065391, 0.0235331271521307, 0.00157721349689765, 0.00027706790955106},
	         new double[] {0.0360531917754067, 0.193746550297859, 0.156033096402298, 0.036048593841147, 0.035202510530745, 0.0280403851614371, 0.0236049759235962, 0.00157683305710926, 0.000287106180458896},
	         new double[] {0.0392830540676451, 0.207713714196468, 0.165877698667278, 0.0379885142462782, 0.0368497354370223, 0.025900089789511, 0.0236128793456664, 0.00157721349689765, 0.00027706790955106},
	         new double[] {0.0386027319009346, 0.202746913288905, 0.160173564211657, 0.0365053833183541, 0.0361960790960064, 0.0280639767189919, 0.0236928889796605, 0.00157721921779673, 0.00027865355207763},
	         new double[] {0.0420619521881581, 0.217258299165264, 0.17021642018173, 0.0384748207022556, 0.0377860049073872, 0.0261451986114997, 0.0236927345153855, 0.00157721349689765, 0.000277860730814345},
	         new double[] {0.03878762420787, 0.206619602975325, 0.162027680427145, 0.036513301042672, 0.0370460802687907, 0.0280635223842572, 0.0237729243577004, 0.00157722207824626, 0.000287106180458896},
	         new double[] {0.0422673539182724, 0.221258237379554, 0.172105565462246, 0.0384821148485745, 0.0388099371444819, 0.0263984585228143, 0.0237729243577004, 0.00157722207824626, 0.000287106180458896},
	         new double[] {0.0388125387233356, 0.211303513289935, 0.164321949030222, 0.0365858777985446, 0.0382578396340455, 0.028066361857164, 0.0238532171761985, 0.00157723065959487, 0.000296351630103447},
	         new double[] {0.0422955436484576, 0.226137473490784, 0.174468020746268, 0.0385595271943891, 0.0402071351625336, 0.0266599918076725, 0.0238532171761985, 0.00157723065959487, 0.000296351630103447},
	         new double[] {0.0388431955912463, 0.21612269140269, 0.166625582029969, 0.0366584545544173, 0.0394695989993003, 0.0280692008533293, 0.0239336129708801, 0.00157723924094348, 0.000305597079747998},
	         new double[] {0.0423300764254907, 0.231157147662698, 0.176839782502859, 0.0386369395402037, 0.0416043331805853, 0.0269317655018725, 0.0239336129708801, 0.00157723924094348, 0.000305597079747998},
	         new double[] {0.0344508394561256, 0.184183918037823, 0.149194415887623, 0.0354895428162575, 0.0349427509629703, 0.0283224841249818, 0.023502105576904, 0.00159255408776355, 0.000313835174414004},
	         new double[] {0.0375266736919307, 0.197551984379657, 0.158622149109132, 0.0373779465252308, 0.0368814263574406, 0.025663718687374, 0.0235341826580098, 0.00159409300961435, 0.00027749793046476},
	         new double[] {0.0362848595833927, 0.193780071190868, 0.153470955996561, 0.0360069673876334, 0.0354610801515123, 0.0283300380954669, 0.0235898126806016, 0.00159294024845102, 0.000305963694030299},
	         new double[] {0.0395300324317769, 0.207726403150614, 0.163100886052849, 0.0379268539193212, 0.0373467085093225, 0.025905629050039, 0.0236138318753622, 0.00159409300961435, 0.00027749793046476},
	         new double[] {0.038617970945842, 0.20341607612686, 0.157766926424089, 0.0365024568401072, 0.0355003169378087, 0.0283456156269028, 0.0236776227604825, 0.00159332640913849, 0.000296780220792379},
	         new double[] {0.042076440365062, 0.21794503589006, 0.167602118286835, 0.0384535237019709, 0.0372959598438652, 0.0261482943330109, 0.0236935840688979, 0.00159409300961435, 0.00027749793046476},
	         new double[] {0.0411659807308677, 0.212550282412183, 0.161963848780619, 0.0369669437870178, 0.0361181940609058, 0.0283684272352172, 0.0237655358165468, 0.00159371256982596, 0.000288329022635881},
	         new double[] {0.0448535507046145, 0.227629905885489, 0.171998319819616, 0.038947736440468, 0.0378398328467709, 0.0263934031549996, 0.023773439238617, 0.00159409300961435, 0.000278290751728045},
	         new double[] {0.0434055482423396, 0.222074828501748, 0.166559756650116, 0.0374895607894154, 0.0377170580619768, 0.0283973997750924, 0.0238537063130693, 0.0015941044514125, 0.000288329022635881},
	         new double[] {0.0472946940949452, 0.23773827973709, 0.17682476800324, 0.039505119346536, 0.039545408778262, 0.0266460080233703, 0.0238535518487943, 0.00159409873051343, 0.000287536201372596},
	         new double[] {0.0429633084417015, 0.225679951738495, 0.168439220024062, 0.037486265551549, 0.0385670592347611, 0.028396883463951, 0.0239338446672925, 0.00159410731186204, 0.000296781651017147},
	         new double[] {0.0468142243862476, 0.241449288234342, 0.178738824938772, 0.0395008801603225, 0.0405693410153566, 0.0269075641918248, 0.0239338446672925, 0.00159410731186204, 0.000296781651017147},
	         new double[] {0.0429401674049487, 0.230480336697794, 0.170775290521557, 0.0375588423074217, 0.0397788186000159, 0.0284004423399164, 0.024014240461974, 0.00159411589321065, 0.000306027100661698},
	         new double[] {0.0467904755039683, 0.246448584563756, 0.18114302419311, 0.0395782925061371, 0.0419665390334083, 0.0271793378860248, 0.024014240461974, 0.00159411589321065, 0.000306027100661698},
	         new double[] {0.0388776640081649, 0.201712352336444, 0.154667057261051, 0.0362526816714408, 0.0349427509629703, 0.0286024954371062, 0.0236625624936713, 0.00160519822819942, 0.000313835174414004},
	         new double[] {0.0423533461824727, 0.216096888002531, 0.164256615409318, 0.0381743624201005, 0.0368814263574406, 0.0261503643449923, 0.023694639574777, 0.00160673715005022, 0.00027749793046476},
	         new double[] {0.0410451767958045, 0.211112576138016, 0.158928906101167, 0.0367414810091895, 0.0354610801515123, 0.0286161664789258, 0.0237503725735522, 0.00160558438888689, 0.000305963694030299},
	         new double[] {0.0447196316084251, 0.226059550554727, 0.168719742879912, 0.0386936913357556, 0.0373467085093225, 0.0263930296279642, 0.0237743917683127, 0.00160673715005022, 0.00027749793046476},
	         new double[] {0.043473154967142, 0.220208960129338, 0.163153400330782, 0.0372077071094185, 0.0361268826763739, 0.0286381371150765, 0.0238382856296164, 0.00160597054957436, 0.000297574472280432},
	         new double[] {0.0473658334749256, 0.235702220338139, 0.173143871696634, 0.0391897104481352, 0.0379456909330043, 0.0266381384499529, 0.0238542469380318, 0.00160673715005022, 0.000278290751728045},
	         new double[] {0.0457299495359493, 0.229887305439374, 0.167800995808297, 0.0377380215815197, 0.0373299534261606, 0.0286671082247268, 0.023926456126139, 0.0016063624311609, 0.000297574472280432},
	         new double[] {0.0498260060057998, 0.245971794823387, 0.178022872059148, 0.0397549996367229, 0.0392370308648226, 0.0268907433183236, 0.0239343595482092, 0.00160674287094929, 0.000287536201372596},
	         new double[] {0.0476262917074996, 0.239395143643194, 0.172440690009079, 0.0382542769441474, 0.0389288174272316, 0.0287006808441988, 0.0240147295988448, 0.00160675431274744, 0.000297574472280432},
	         new double[] {0.0518904782499999, 0.256059466171466, 0.182893662216379, 0.0403059107757139, 0.0409426067963137, 0.0271516444438342, 0.0240145751345698, 0.00160674859184837, 0.000296781651017147},
	         new double[] {0.0470902434643019, 0.243097043038896, 0.174358711527669, 0.038250981706281, 0.0397788186000159, 0.0287001020799091, 0.0240949709292514, 0.00160675717319698, 0.000306027100661698},
	         new double[] {0.0513080907243059, 0.25986954204775, 0.184846219372452, 0.0403016715895003, 0.0419665390334083, 0.0274234410216305, 0.0240949709292514, 0.00160675717319698, 0.000306027100661698},
	         new double[] {0.0431472997070328, 0.217060806578119, 0.159736789299859, 0.0367879580700051, 0.0349427509629703, 0.0288554511872868, 0.0238232253628051, 0.00161421722558897, 0.000313835174414004},
	         new double[] {0.0470104512244726, 0.232299532545337, 0.169463241411834, 0.0387331188186877, 0.0368814263574406, 0.0266362043093244, 0.0238553024439109, 0.00161575614743977, 0.00027749793046476},
	         new double[] {0.045336737840372, 0.225795857868839, 0.163920543576946, 0.03724426699169, 0.0362392211110329, 0.0288754445376951, 0.0239111384188694, 0.00161460338627644, 0.000306819921924983},
	         new double[] {0.0493973461893377, 0.241551291864824, 0.173842286683406, 0.0392184198266529, 0.0381604491935535, 0.0268813131313131, 0.0239351576136299, 0.00161575614743977, 0.000278290751728045},
	         new double[] {0.0475041290589064, 0.235471821854635, 0.16859117997548, 0.037775629103434, 0.0373386420416287, 0.0289043551011636, 0.0239993089153919, 0.00161499526786298, 0.000306819921924983},
	         new double[] {0.051760199075846, 0.251816807372179, 0.178744548221554, 0.0397847916953903, 0.039342888951056, 0.0271339179996838, 0.0240152702238073, 0.00161576186833885, 0.000287536201372596},
	         new double[] {0.0494301412432772, 0.245144808112464, 0.173276681415144, 0.0382995819357654, 0.0385417127914154, 0.0289379262904108, 0.0240875823880978, 0.00161538714944953, 0.000306819921924983},
	         new double[] {0.0538573233515086, 0.262077533056785, 0.183661881468311, 0.040343609116901, 0.0406342288828743, 0.0273948191251944, 0.024095485810168, 0.00161576758923792, 0.000296781651017147},
	         new double[] {0.051475612951429, 0.254962643244162, 0.177968700389078, 0.0388193055934565, 0.0401405767924864, 0.0289756684918242, 0.024175958836987, 0.00161577903103607, 0.000306819921924983},
	         new double[] {0.0560845051164861, 0.272491780498256, 0.188585736540012, 0.0408981344338818, 0.0423398048143654, 0.0276659606600468, 0.024175804372712, 0.00161577331013699, 0.000306027100661698},
	         new double[] {0.0465040229362286, 0.229307935687941, 0.164386494358907, 0.0370985788142521, 0.036367108949374, 0.0290816162054764, 0.0239840941843056, 0.00162022130916673, 0.000316065371569534},
	         new double[] {0.0506733283675929, 0.245171687041649, 0.174221927452897, 0.0390569455433338, 0.0383916235739944, 0.0271252427329803, 0.0240161712654114, 0.00162176023101753, 0.000278290751728045},
	         new double[] {0.0485992378618254, 0.238829362457286, 0.169042512430083, 0.0376253270208933, 0.0374509804762877, 0.0291104461996163, 0.0240722646808282, 0.00162061319075327, 0.000316065371569534},
	         new double[] {0.0529578907502787, 0.25527036110887, 0.179107464657716, 0.0396183202067304, 0.0395576472116052, 0.027377847601351, 0.0240962838755888, 0.00162176595191661, 0.000287536201372596},
	         new double[] {0.0504731755623787, 0.248503981316688, 0.173745410408718, 0.0381498664837505, 0.0385504014068835, 0.0291439568426816, 0.0241605381535341, 0.00162100507233982, 0.000316065371569534},
	         new double[] {0.0549985640543325, 0.265531188339435, 0.184042311721874, 0.0401777378459022, 0.0407400869691077, 0.0276387487268616, 0.0241764994619494, 0.00162177167281568, 0.000296781651017147},
	         new double[] {0.052548317283351, 0.258486964502393, 0.178476242822417, 0.0386772876111452, 0.0397534721566703, 0.0291816976138702, 0.0242489146024233, 0.00162139695392636, 0.000316065371569534},
	         new double[] {0.0572583978507726, 0.276118490117433, 0.189005619543812, 0.0407401694454027, 0.042031426900926, 0.027909890261714, 0.0242568180244935, 0.00162177739371475, 0.000306027100661698},
	         new double[] {0.0476483350467426, 0.238039037412964, 0.168698780790594, 0.0372266978722641, 0.0384733151523161, 0.0292853295552516, 0.0241451689581728, 0.00162382070816721, 0.000325310821214085},
	         new double[] {0.0519292444923474, 0.254267041271138, 0.178621140085558, 0.0391896327392561, 0.0407247320759941, 0.0276242436256312, 0.0241774005035535, 0.00162536535091709, 0.000288329022635881},
	         new double[] {0.0494183168600045, 0.247470094000093, 0.173376973066579, 0.0377453567276152, 0.0395571866792298, 0.0293194828459795, 0.0242334424308787, 0.00162421258975376, 0.000325310821214085},
	         new double[] {0.0538570587599265, 0.264267067015184, 0.183528974494516, 0.0397427264012436, 0.0418907557136049, 0.0278851447511418, 0.0242576160899142, 0.00162537107181616, 0.000297574472280432},
	         new double[] {0.0514463612793532, 0.257399726941487, 0.17811789285557, 0.038272783575909, 0.0406410582061434, 0.0293571630709863, 0.0243218188797679, 0.0016246044713403, 0.000325310821214085},
	         new double[] {0.0560658907411253, 0.274796935257157, 0.18850236540107, 0.0403051565705193, 0.0430567793512157, 0.0281562862859942, 0.0243379346524582, 0.00162537679271524, 0.000306819921924983},
	         new double[] {0.0503787127919875, 0.252709196116425, 0.17606357663745, 0.0378657470893496, 0.0407689460444846, 0.0295190216080265, 0.0243147392671643, 0.00162602039386102, 0.000334556270858636},
	         new double[] {0.0549132582980211, 0.26976477665324, 0.18629330506065, 0.0398704788449734, 0.0432879537316566, 0.0281383817788602, 0.0243389129261998, 0.00162717887592342, 0.000306819921924983},
	         new double[] {0.0522394352156808, 0.262512889185613, 0.18079596012991, 0.0383864876368511, 0.0418528175713982, 0.0295581787784774, 0.0244031157160535, 0.00162641227544756, 0.000334556270858636},
	         new double[] {0.0569401370841836, 0.280154334124534, 0.19125630466915, 0.0404257407277098, 0.0444539773692674, 0.0284095233137126, 0.0244192314887438, 0.0016271845968225, 0.000316065371569534},
	         new double[] {0.0520210041379263, 0.267047033513599, 0.183376907991352, 0.0384575881623919, 0.0430645769366531, 0.029733230185523, 0.0244845155285224, 0.00162703585344658, 0.000343801720503187},
	         new double[] {0.056705780453633, 0.284876445742707, 0.19390776118145, 0.0405017490695434, 0.0458511753873192, 0.0286710565985708, 0.0245006313012128, 0.00162780817482152, 0.000325310821214085},
        };
        #endregion

        #endregion
    }
}
