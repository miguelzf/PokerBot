using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pokermaster
{
	public partial class PokerSimulatorUI : Form
	{
		PokerApp calc;
		DateTime startTime = DateTime.Now;


		public PokerSimulatorUI() : this(null)
		{
		}

		public PokerSimulatorUI(PokerApp pc)
		{
			calc = pc;
			InitializeComponent();
		}

		private void PokerUI_Load(object sender, EventArgs e)
		{

		}

		Card[] parseCards(string str)
		{


			return null;
		}


		// only action function, calls the poker calc class
		private void Calculate_Click(object sender, EventArgs e)
		{
			
			


		}


		private void PokerUI_Closed(object sender, FormClosedEventArgs e)
		{
			TimeSpan ts = DateTime.Now - startTime;
			Keys.UpdateReqKey((int) ts.TotalSeconds);

//			MessageBox.Show("See you next time young Pokeron");
		}

		private void UITitle_Click(object sender, EventArgs e)
		{

		}



/*		private System.Windows.Forms.TextBox PlayerCardsBox;
		private System.Windows.Forms.TextBox SharedCardsBox;
		private System.Windows.Forms.TextBox WinProbBox;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Button CalculateButton;
		private System.Windows.Forms.TextBox NumPlayers;
*/


	}
}
