namespace pokermaster
{
	partial class PokerSimulatorUI
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.PlayerCardsBox = new System.Windows.Forms.TextBox();
			this.SharedCardsBox = new System.Windows.Forms.TextBox();
			this.WinProbBox = new System.Windows.Forms.TextBox();
			this.PlayerCardsDesc = new System.Windows.Forms.Label();
			this.SharedCardsDesc = new System.Windows.Forms.Label();
			this.WinProbDesc = new System.Windows.Forms.Label();
			this.UITitle = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.CalculateButton = new System.Windows.Forms.Button();
			this.NumPlayers = new System.Windows.Forms.TextBox();
			this.NumPlayersDesc = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// PlayerCardsBox
			// 
			this.PlayerCardsBox.Location = new System.Drawing.Point(91, 42);
			this.PlayerCardsBox.MaxLength = 100;
			this.PlayerCardsBox.Name = "PlayerCardsBox";
			this.PlayerCardsBox.Size = new System.Drawing.Size(218, 20);
			this.PlayerCardsBox.TabIndex = 0;
			// 
			// SharedCardsBox
			// 
			this.SharedCardsBox.Location = new System.Drawing.Point(91, 80);
			this.SharedCardsBox.MaxLength = 1000;
			this.SharedCardsBox.Name = "SharedCardsBox";
			this.SharedCardsBox.Size = new System.Drawing.Size(218, 20);
			this.SharedCardsBox.TabIndex = 1;
			// 
			// WinProbBox
			// 
			this.WinProbBox.Location = new System.Drawing.Point(91, 139);
			this.WinProbBox.MaxLength = 100;
			this.WinProbBox.Name = "WinProbBox";
			this.WinProbBox.ReadOnly = true;
			this.WinProbBox.Size = new System.Drawing.Size(218, 20);
			this.WinProbBox.TabIndex = 2;
			// 
			// PlayerCardsDesc
			// 
			this.PlayerCardsDesc.AutoSize = true;
			this.PlayerCardsDesc.Location = new System.Drawing.Point(12, 45);
			this.PlayerCardsDesc.Name = "PlayerCardsDesc";
			this.PlayerCardsDesc.Size = new System.Drawing.Size(64, 13);
			this.PlayerCardsDesc.TabIndex = 3;
			this.PlayerCardsDesc.Text = "Your Cards:";
			// 
			// SharedCardsDesc
			// 
			this.SharedCardsDesc.AutoSize = true;
			this.SharedCardsDesc.Location = new System.Drawing.Point(12, 80);
			this.SharedCardsDesc.Name = "SharedCardsDesc";
			this.SharedCardsDesc.Size = new System.Drawing.Size(76, 13);
			this.SharedCardsDesc.TabIndex = 4;
			this.SharedCardsDesc.Text = "Shared Cards:";
			// 
			// WinProbDesc
			// 
			this.WinProbDesc.AutoSize = true;
			this.WinProbDesc.Location = new System.Drawing.Point(6, 142);
			this.WinProbDesc.Name = "WinProbDesc";
			this.WinProbDesc.Size = new System.Drawing.Size(82, 13);
			this.WinProbDesc.TabIndex = 5;
			this.WinProbDesc.Text = "Win Probability:";
			// 
			// UITitle
			// 
			this.UITitle.AutoSize = true;
			this.UITitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.UITitle.ForeColor = System.Drawing.Color.Crimson;
			this.UITitle.Location = new System.Drawing.Point(87, 9);
			this.UITitle.MinimumSize = new System.Drawing.Size(50, 10);
			this.UITitle.Name = "UITitle";
			this.UITitle.Size = new System.Drawing.Size(136, 20);
			this.UITitle.TabIndex = 6;
			this.UITitle.Text = "Poker Simulator";
			this.UITitle.Click += new System.EventHandler(this.UITitle_Click);
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(12, 115);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(297, 10);
			this.progressBar.TabIndex = 7;
			// 
			// CalculateButton
			// 
			this.CalculateButton.Location = new System.Drawing.Point(328, 115);
			this.CalculateButton.Name = "CalculateButton";
			this.CalculateButton.Size = new System.Drawing.Size(75, 23);
			this.CalculateButton.TabIndex = 8;
			this.CalculateButton.Text = "Calculate";
			this.CalculateButton.UseVisualStyleBackColor = true;
			this.CalculateButton.Click += new System.EventHandler(this.Calculate_Click);
			// 
			// NumPlayers
			// 
			this.NumPlayers.Location = new System.Drawing.Point(328, 65);
			this.NumPlayers.MaxLength = 2;
			this.NumPlayers.Name = "NumPlayers";
			this.NumPlayers.Size = new System.Drawing.Size(100, 20);
			this.NumPlayers.TabIndex = 9;
			// 
			// NumPlayersDesc
			// 
			this.NumPlayersDesc.AutoSize = true;
			this.NumPlayersDesc.Location = new System.Drawing.Point(325, 45);
			this.NumPlayersDesc.Name = "NumPlayersDesc";
			this.NumPlayersDesc.Size = new System.Drawing.Size(118, 13);
			this.NumPlayersDesc.TabIndex = 10;
			this.NumPlayersDesc.Text = "How many Opponents?";
			// 
			// PokerSimulatorUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(457, 177);
			this.Controls.Add(this.NumPlayersDesc);
			this.Controls.Add(this.NumPlayers);
			this.Controls.Add(this.CalculateButton);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.UITitle);
			this.Controls.Add(this.WinProbDesc);
			this.Controls.Add(this.SharedCardsDesc);
			this.Controls.Add(this.PlayerCardsDesc);
			this.Controls.Add(this.WinProbBox);
			this.Controls.Add(this.SharedCardsBox);
			this.Controls.Add(this.PlayerCardsBox);
			this.Name = "PokerSimulatorUI";
			this.Text = "PokerSim";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PokerUI_Closed);
			this.Load += new System.EventHandler(this.PokerUI_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox PlayerCardsBox;
		private System.Windows.Forms.TextBox SharedCardsBox;
		private System.Windows.Forms.TextBox WinProbBox;
		private System.Windows.Forms.Label PlayerCardsDesc;
		private System.Windows.Forms.Label SharedCardsDesc;
		private System.Windows.Forms.Label WinProbDesc;
		private System.Windows.Forms.Label UITitle;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Button CalculateButton;
		private System.Windows.Forms.TextBox NumPlayers;
		private System.Windows.Forms.Label NumPlayersDesc;
	}
}

