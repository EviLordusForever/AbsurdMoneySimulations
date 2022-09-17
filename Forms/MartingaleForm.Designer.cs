namespace AbsurdMoneySimulations
{
	partial class MartingaleForm
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
			this.button1 = new System.Windows.Forms.Button();
			this.money = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.bet = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.prize = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.winrate = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(12, 193);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(384, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Simulate!";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// money
			// 
			this.money.Location = new System.Drawing.Point(171, 12);
			this.money.Name = "money";
			this.money.Size = new System.Drawing.Size(225, 23);
			this.money.TabIndex = 1;
			this.money.Text = "10";
			this.money.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(153, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "money:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label2.Location = new System.Drawing.Point(12, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(153, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "bet, %";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// bet
			// 
			this.bet.Location = new System.Drawing.Point(171, 38);
			this.bet.Name = "bet";
			this.bet.Size = new System.Drawing.Size(225, 23);
			this.bet.TabIndex = 4;
			this.bet.Text = "1";
			this.bet.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label3.Location = new System.Drawing.Point(12, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(153, 23);
			this.label3.TabIndex = 5;
			this.label3.Text = "prize, %";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// prize
			// 
			this.prize.Location = new System.Drawing.Point(171, 64);
			this.prize.Name = "prize";
			this.prize.Size = new System.Drawing.Size(225, 23);
			this.prize.TabIndex = 6;
			this.prize.Text = "70";
			this.prize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label5.Location = new System.Drawing.Point(12, 90);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(153, 23);
			this.label5.TabIndex = 7;
			this.label5.Text = "win rate, %";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// winrate
			// 
			this.winrate.Location = new System.Drawing.Point(171, 90);
			this.winrate.Name = "winrate";
			this.winrate.Size = new System.Drawing.Size(225, 23);
			this.winrate.TabIndex = 8;
			this.winrate.Text = "50";
			this.winrate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// MartingaleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(408, 228);
			this.Controls.Add(this.winrate);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.prize);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.bet);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.money);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MartingaleForm";
			this.Text = "Martingale";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Button button1;
		public TextBox money;
		private Label label1;
		private Label label2;
		public TextBox bet;
		private Label label3;
		public TextBox prize;
		private Label label5;
		public TextBox winrate;
	}
}