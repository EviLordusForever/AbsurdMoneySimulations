namespace AbsurdMoneySimulations
{
	partial class FittingParamsForm
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
			this.useDropout = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.statisticsRecalculatePeriod = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.validationRecalculatePeriod = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.batchSize = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.validationTestsCount = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.trainingTestsCount = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.MOMENTUM = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.LEARNING_RATE = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(9, 241);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(460, 23);
			this.button1.TabIndex = 52;
			this.button1.Text = "Fit!";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// useDropout
			// 
			this.useDropout.Location = new System.Drawing.Point(244, 212);
			this.useDropout.Name = "useDropout";
			this.useDropout.Size = new System.Drawing.Size(225, 23);
			this.useDropout.TabIndex = 51;
			this.useDropout.Text = "false";
			this.useDropout.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label7.Location = new System.Drawing.Point(9, 212);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(229, 23);
			this.label7.TabIndex = 50;
			this.label7.Text = "Use dropout:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// statisticsRecalculatePeriod
			// 
			this.statisticsRecalculatePeriod.Location = new System.Drawing.Point(244, 183);
			this.statisticsRecalculatePeriod.Name = "statisticsRecalculatePeriod";
			this.statisticsRecalculatePeriod.Size = new System.Drawing.Size(225, 23);
			this.statisticsRecalculatePeriod.TabIndex = 49;
			this.statisticsRecalculatePeriod.Text = "50";
			this.statisticsRecalculatePeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label6.Location = new System.Drawing.Point(9, 183);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(229, 23);
			this.label6.TabIndex = 48;
			this.label6.Text = "Statistics recalculate period:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// validationRecalculatePeriod
			// 
			this.validationRecalculatePeriod.Location = new System.Drawing.Point(244, 154);
			this.validationRecalculatePeriod.Name = "validationRecalculatePeriod";
			this.validationRecalculatePeriod.Size = new System.Drawing.Size(225, 23);
			this.validationRecalculatePeriod.TabIndex = 47;
			this.validationRecalculatePeriod.Text = "30";
			this.validationRecalculatePeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label5.Location = new System.Drawing.Point(9, 154);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(229, 23);
			this.label5.TabIndex = 46;
			this.label5.Text = "Validation recalculate period:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// batchSize
			// 
			this.batchSize.Location = new System.Drawing.Point(244, 96);
			this.batchSize.Name = "batchSize";
			this.batchSize.Size = new System.Drawing.Size(225, 23);
			this.batchSize.TabIndex = 45;
			this.batchSize.Text = "21000";
			this.batchSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label4.Location = new System.Drawing.Point(9, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(229, 23);
			this.label4.TabIndex = 44;
			this.label4.Text = "Batch size:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// validationTestsCount
			// 
			this.validationTestsCount.Location = new System.Drawing.Point(244, 125);
			this.validationTestsCount.Name = "validationTestsCount";
			this.validationTestsCount.Size = new System.Drawing.Size(225, 23);
			this.validationTestsCount.TabIndex = 43;
			this.validationTestsCount.Text = "8000";
			this.validationTestsCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label3.Location = new System.Drawing.Point(9, 125);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(229, 23);
			this.label3.TabIndex = 42;
			this.label3.Text = "Validation tests count:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// trainingTestsCount
			// 
			this.trainingTestsCount.Location = new System.Drawing.Point(244, 67);
			this.trainingTestsCount.Name = "trainingTestsCount";
			this.trainingTestsCount.Size = new System.Drawing.Size(225, 23);
			this.trainingTestsCount.TabIndex = 41;
			this.trainingTestsCount.Text = "21000";
			this.trainingTestsCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label2.Location = new System.Drawing.Point(9, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(229, 23);
			this.label2.TabIndex = 40;
			this.label2.Text = "Training tests count:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MOMENTUM
			// 
			this.MOMENTUM.Location = new System.Drawing.Point(244, 38);
			this.MOMENTUM.Name = "MOMENTUM";
			this.MOMENTUM.Size = new System.Drawing.Size(225, 23);
			this.MOMENTUM.TabIndex = 39;
			this.MOMENTUM.Text = "0.8f";
			this.MOMENTUM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label1.Location = new System.Drawing.Point(9, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(229, 23);
			this.label1.TabIndex = 38;
			this.label1.Text = "Momentum:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LEARNING_RATE
			// 
			this.LEARNING_RATE.Location = new System.Drawing.Point(244, 9);
			this.LEARNING_RATE.Name = "LEARNING_RATE";
			this.LEARNING_RATE.Size = new System.Drawing.Size(225, 23);
			this.LEARNING_RATE.TabIndex = 37;
			this.LEARNING_RATE.Text = "0.01f";
			this.LEARNING_RATE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label8
			// 
			this.label8.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label8.Location = new System.Drawing.Point(9, 9);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(229, 23);
			this.label8.TabIndex = 36;
			this.label8.Text = "Learning rate:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// FittingParamsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(479, 275);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.useDropout);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.statisticsRecalculatePeriod);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.validationRecalculatePeriod);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.batchSize);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.validationTestsCount);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.trainingTestsCount);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.MOMENTUM);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.LEARNING_RATE);
			this.Controls.Add(this.label8);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "FittingParamsForm";
			this.Text = "Fittin Params";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FittingParamsForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FittingParamsForm_FormClosed);
			this.Load += new System.EventHandler(this.FittingParamsForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Button button1;
		public TextBox useDropout;
		private Label label7;
		public TextBox statisticsRecalculatePeriod;
		private Label label6;
		public TextBox validationRecalculatePeriod;
		private Label label5;
		public TextBox batchSize;
		private Label label4;
		public TextBox validationTestsCount;
		private Label label3;
		public TextBox trainingTestsCount;
		private Label label2;
		public TextBox MOMENTUM;
		private Label label1;
		public TextBox LEARNING_RATE;
		private Label label8;
	}
}