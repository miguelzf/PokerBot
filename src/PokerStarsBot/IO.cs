using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Drawing;


namespace pokermaster
{
	static class IO
	{
		public static TextWriter log = new StreamWriter("poker-sym-log.txt");

		public static void Write	(string str) { log.Write	(str); log.Flush(); }

		public static void WriteLine(string str) { log.WriteLine(str); log.Flush(); }

		public static void Message	(string str) { MessageBox.Show(str); }

		public static string stringIds	(int[] bb) { return stringIds  (bb, 0, bb.Length); }
		public static string stringCards(int[] bb) { return stringCards(bb, 0, bb.Length); }
		public static string stringCards(Card[]bb) { return stringCards(bb, 0, bb.Length); }

		public static string stringIds	(int[] bb, int i) { return stringIds  (bb, i, bb.Length); }
		public static string stringCards(int[] bb, int i) { return stringCards(bb, i, bb.Length); }
		public static string stringCards(Card[]bb, int i) { return stringCards(bb, i, bb.Length); }

		public static string stringIds(int[] bb, int i, int j)
		{
			string str = "";
			for (; i < j; i++)
				str += String.Format("{0,3}", bb[i]);
			return str;
		}

		public static string stringCards(Card[] cards, int i, int j)
		{
			string str = "";
			for (; i < j; i++)
				str += String.Format("{0,3}", Card.ToString(cards[i]));
			return str;
		}

		public static string stringCards(int[] cards, int i, int j)
		{
			string str = "";
			for (; i < j; i++)
				str += String.Format("{0,3}", Card.ToString(cards[i]));
			return str;
		}

		public static void printDeck(string cards)
		{
			Write("Deck: " + cards + "\n");
		}

		public static void printHand(string cards)
		{
			Write("Hand: " + cards + "\n");
		}


		public static int[] parseCards(string input)
		{
			string[] cards = input.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);
			int[] ids = new int[PokerApp.TAKEN_CARDS]; //cards.Length];
			int i = 0;
			
			foreach (string s in cards)
				if ((ids[i++] = Card.parseCard(s)) == 0)
				{	IO.Message("[ERROR] Wrong Format of value: " + s);
					return null;
				}

			return ids;
		}


		public static DialogResult InputBox(string title, string promptText, ref string value)
		{
			Form form = new Form();
			Label label = new Label();
			TextBox textBox = new TextBox();
			Button buttonOk = new Button();
			Button buttonCancel = new Button();

			form.Text = title;
			label.Text = promptText;
			textBox.Text = value;

			buttonOk.Text = "OK";
			buttonCancel.Text = "Cancel";
			buttonOk.DialogResult = DialogResult.OK;
			buttonCancel.DialogResult = DialogResult.Cancel;

			label.SetBounds(9, 20, 372, 13);
			textBox.SetBounds(12, 36, 372, 20);
			buttonOk.SetBounds(228, 72, 75, 23);
			buttonCancel.SetBounds(309, 72, 75, 23);

			label.AutoSize = true;
			textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
			buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			form.ClientSize = new Size(396, 107);
			form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
			form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.AcceptButton = buttonOk;
			form.CancelButton = buttonCancel;

			DialogResult dialogResult = form.ShowDialog();
			value = textBox.Text;
			return dialogResult;
		}


	}	
}
