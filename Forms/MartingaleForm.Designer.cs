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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MartingaleForm));
			this.button1 = new System.Windows.Forms.Button();
			this.money = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.bet = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.prize = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.winrate = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.simulationsCount = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.martingaleChain = new System.Windows.Forms.TextBox();
			this.antimartingaleChain = new System.Windows.Forms.TextBox();
			this.simulationTime = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.height = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(12, 245);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(384, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Simulate!";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// money
			// 
			this.money.Location = new System.Drawing.Point(171, 87);
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
			this.label1.Location = new System.Drawing.Point(12, 87);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(153, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "start depo, $:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label2.Location = new System.Drawing.Point(12, 113);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(153, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "bet, (% of depo):";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// bet
			// 
			this.bet.Location = new System.Drawing.Point(171, 113);
			this.bet.Name = "bet";
			this.bet.Size = new System.Drawing.Size(225, 23);
			this.bet.TabIndex = 4;
			this.bet.Text = "4";
			this.bet.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label3.Location = new System.Drawing.Point(12, 165);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(153, 23);
			this.label3.TabIndex = 5;
			this.label3.Text = "prize, %:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// prize
			// 
			this.prize.Location = new System.Drawing.Point(171, 165);
			this.prize.Name = "prize";
			this.prize.Size = new System.Drawing.Size(225, 23);
			this.prize.TabIndex = 6;
			this.prize.Text = "75";
			this.prize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label5.Location = new System.Drawing.Point(12, 139);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(153, 23);
			this.label5.TabIndex = 7;
			this.label5.Text = "your win rate, %:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// winrate
			// 
			this.winrate.Location = new System.Drawing.Point(171, 139);
			this.winrate.Name = "winrate";
			this.winrate.Size = new System.Drawing.Size(225, 23);
			this.winrate.TabIndex = 8;
			this.winrate.Text = "59";
			this.winrate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label4.Location = new System.Drawing.Point(12, 61);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(153, 23);
			this.label4.TabIndex = 9;
			this.label4.Text = "simulations count:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// simulationsCount
			// 
			this.simulationsCount.Location = new System.Drawing.Point(171, 61);
			this.simulationsCount.Name = "simulationsCount";
			this.simulationsCount.Size = new System.Drawing.Size(225, 23);
			this.simulationsCount.TabIndex = 10;
			this.simulationsCount.Text = "100";
			this.simulationsCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label6.Location = new System.Drawing.Point(12, 191);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(153, 23);
			this.label6.TabIndex = 11;
			this.label6.Text = "martingale chain:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label7.Location = new System.Drawing.Point(12, 217);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(153, 23);
			this.label7.TabIndex = 12;
			this.label7.Text = "antimartingale chain:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// martingaleChain
			// 
			this.martingaleChain.Location = new System.Drawing.Point(171, 191);
			this.martingaleChain.Name = "martingaleChain";
			this.martingaleChain.Size = new System.Drawing.Size(225, 23);
			this.martingaleChain.TabIndex = 13;
			this.martingaleChain.Text = "0";
			this.martingaleChain.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// antimartingaleChain
			// 
			this.antimartingaleChain.Location = new System.Drawing.Point(171, 217);
			this.antimartingaleChain.Name = "antimartingaleChain";
			this.antimartingaleChain.Size = new System.Drawing.Size(225, 23);
			this.antimartingaleChain.TabIndex = 14;
			this.antimartingaleChain.Text = "0";
			this.antimartingaleChain.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// simulationTime
			// 
			this.simulationTime.Location = new System.Drawing.Point(171, 9);
			this.simulationTime.Name = "simulationTime";
			this.simulationTime.Size = new System.Drawing.Size(225, 23);
			this.simulationTime.TabIndex = 18;
			this.simulationTime.Text = "1920";
			this.simulationTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label8
			// 
			this.label8.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label8.Location = new System.Drawing.Point(12, 9);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(153, 23);
			this.label8.TabIndex = 17;
			this.label8.Text = "bets count:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label9
			// 
			this.label9.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label9.Location = new System.Drawing.Point(12, 35);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(153, 23);
			this.label9.TabIndex = 16;
			this.label9.Text = "height:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// height
			// 
			this.height.Location = new System.Drawing.Point(171, 35);
			this.height.Name = "height";
			this.height.Size = new System.Drawing.Size(225, 23);
			this.height.TabIndex = 15;
			this.height.Text = "950";
			this.height.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label10
			// 
			this.label10.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label10.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label10.Location = new System.Drawing.Point(12, 287);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(384, 231);
			this.label10.TabIndex = 19;
			this.label10.Text = resources.GetString("label10.Text");
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MartingaleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(408, 516);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.simulationTime);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.height);
			this.Controls.Add(this.antimartingaleChain);
			this.Controls.Add(this.martingaleChain);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.simulationsCount);
			this.Controls.Add(this.label4);
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
			this.Text = "Binary options simulation";
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
		private Label label4;
		public TextBox simulationsCount;
		private Label label6;
		private Label label7;
		public TextBox martingaleChain;
		public TextBox antimartingaleChain;
		public TextBox simulationTime;
		private Label label8;
		private Label label9;
		public TextBox height;
		private Label label10;
	}
}