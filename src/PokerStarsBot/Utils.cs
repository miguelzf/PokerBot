using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Security.Cryptography;


namespace pokermaster
{

	class ReserveComp : IComparer<int>
	{
		int IComparer<int>.Compare (int x, int y)
		{
			return x-y;
		}
	}


	static class Utils
	{
	
		public static void BinaryInsertionSort (int[] a)
		{
			int i, m;
			int hi, lo, tmp;

			for (i = 1; i < a.Length; i++)
			{
				lo = 0;
				hi = i;
				m = i / 2;

				// Find
				do {
					if (a[i] < a[m])
						lo = m + 1;
					else if (a[i] > a[m])
						hi = m;
					else
						break;

					m = lo + ((hi - lo) / 2);
				} while (lo < hi);
				
				// insert
				if (m < i) {
					tmp = a[i];
					Array.Copy(a, m, a, m + 1, i - m);	// copy 1 forward
					a[m] = tmp;
				}
			}
		}

		// a is changed. res has result
		public static void SelectionSort(int[] a)
		{
			int n = a.Length;

			for (int i = 0; i < n; i++)
			{
				int max = i;
				int vmax = a[max];

				for (int j = i + 1; j < n; j++)
					if (a[j] > vmax)
					{
						max = j;
						vmax = a[max];
					}

				if (max != i)
				{	// swap
					int tt = a[i];
					a[i] = a[max];
					a[max] = tt;
				}
			}
		}

	
		// fastest
		static Random rand = new Random(DateTime.UtcNow.Millisecond);
		
		public static int genSimpleRandom(int min, int max)
		{
			return rand.Next(min, max+1);
//			return (rand.Next() % (max-min+1)) + min;	// faster prolly
		}


		static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

		// only generates numbers between 1 and 255. VERY SLOW
		public static int genCryptoRandom(int min, int max)
		{
			byte[] val = new byte[1];
			int range = max - min + 1;
			int limit = range * (Byte.MaxValue / range);

			do
				rng.GetBytes(val);
			while (!(val[0] < limit));	// check for leftovers

			return val[0] % range + min;
		}

		// slower but simpler to test
		// only generates numbers between 1 and 255
		public static int genRandom234(int min, int max)
		{
			byte[] val = new byte[1];

			do
				rng.GetBytes(val);
			while (!(val[0] >= min && val[0] <= max));	// check for leftovers

			return val[0];
		}
		
		public static uint hash(uint a)
		{
			a = (a + 0x7ed55d16) + (a << 12);
			a = (a ^ 0xc761c23c) ^ (a >> 19);
			a = (a + 0x165667b1) + (a << 5 );
			a = (a + 0xd3a2646c) ^ (a << 9 );
			a = (a + 0xfd7046c5) + (a << 3 );
			a = (a ^ 0xb55a4f09) ^ (a >> 16);

			return a;
		}
		
		public static void Test()
		{
			for (uint i = 0; i < 100000; i++)
				IO.WriteLine(i + " : " + hash(i));
		}


		public static int pow(int n, int exp)
		{
			switch (exp)
			{
				case 0: return 1;
				case 1: return n;
				case 2: return n * n;
				case 3: return n * n * n;
				case 4: return n * n * n * n;
				case 5: return n * n * n * n * n;

				default:
					int ret = n;
					for (int i = 1; i < exp; i++)
						ret *= n;
					return ret;
			}
		}

	}	
}
