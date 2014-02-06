
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using Microsoft.Win32;


namespace pokermaster
{
	static public class Keys
	{
		const string regkeyName = "Software\\Microsoft\\PokerSync";
		
		static public bool ValidateKey(string str)
		{
			if (str == null && str.Trim() == "")
				return false;

			const int SecsPerDay = 60 * 60 * 24;

			int secs = ReadReqKey();

			int day = secs / SecsPerDay;

			return str.Trim().Equals(Utils.hash((uint) day).ToString());
		}

		static public int ReadReqKey()
		{
			RegistryKey key = Registry.CurrentUser.CreateSubKey(regkeyName);
			int val;

			if (key.ValueCount == 0)
				val = 1;
			else
				val = (int) key.GetValue("count");

			key.Close();
			return val;
		}

		static public void UpdateReqKey(int spentSecs)
		{
			RegistryKey key = Registry.CurrentUser.CreateSubKey(regkeyName);

			if (key.ValueCount == 0)
				key.SetValue("count", spentSecs);
			else
				key.SetValue("count", spentSecs + (int)key.GetValue("count") + 1);

			key.Close();
		}


		static public void RequireKey()
		{
			string choice = null;

			while (true)
			{

//				if (ValidateKey(Microsoft.VisualBasic.Interaction.InputBox("Enter program key:", "Poker Symulator", null, 100, 100))) break;

				DialogResult res = IO.InputBox("Poker Symulator", "Enter program key:", ref choice);

				if (res == DialogResult.OK && ValidateKey(choice))
					break;

				if (res == DialogResult.Abort
				||	res == DialogResult.Cancel
				||	res == DialogResult.Ignore
				||	res == DialogResult.No)
					Environment.Exit(0);

				MessageBox.Show("Invalid Key!", "Poker Symulator");
			}
		}

	}

}