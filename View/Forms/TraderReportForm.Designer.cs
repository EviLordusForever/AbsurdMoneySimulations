namespace AbsurdMoneySimulations
{
	partial class TraderReportForm
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
			this.rtb1 = new System.Windows.Forms.RichTextBox();
			this.rtb2 = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// rtb1
			// 
			this.rtb1.BackColor = System.Drawing.SystemColors.InfoText;
			this.rtb1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtb1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.rtb1.ForeColor = System.Drawing.Color.Lime;
			this.rtb1.Location = new System.Drawing.Point(0, 0);
			this.rtb1.Name = "rtb1";
			this.rtb1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.rtb1.Size = new System.Drawing.Size(119, 176);
			this.rtb1.TabIndex = 0;
			this.rtb1.Text = "";
			this.rtb1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.rtb1_MouseDoubleClick);
			this.rtb1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtb1_MouseDown);
			// 
			// rtb2
			// 
			this.rtb2.BackColor = System.Drawing.SystemColors.InfoText;
			this.rtb2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtb2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.rtb2.ForeColor = System.Drawing.Color.Lime;
			this.rtb2.Location = new System.Drawing.Point(0, 182);
			this.rtb2.Name = "rtb2";
			this.rtb2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.rtb2.Size = new System.Drawing.Size(119, 117);
			this.rtb2.TabIndex = 1;
			this.rtb2.Text = "";
			this.rtb2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.rtb2_MouseDoubleClick);
			this.rtb2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtb2_MouseDown);
			// 
			// TraderReportForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(119, 296);
			this.Controls.Add(this.rtb2);
			this.Controls.Add(this.rtb1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "TraderReportForm";
			this.Text = "TraderReportForm";
			this.Load += new System.EventHandler(this.TraderReportForm_Load);
			this.DoubleClick += new System.EventHandler(this.TraderReportForm_DoubleClick);
			this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TraderReportForm_MouseDoubleClick);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TraderReportForm_MouseDown);
			this.ResumeLayout(false);

		}

		#endregion

		public RichTextBox rtb1;
		public RichTextBox rtb2;
	}
}