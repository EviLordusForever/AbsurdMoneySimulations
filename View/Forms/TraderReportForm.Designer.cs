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
			this.rtb = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// rtb
			// 
			this.rtb.BackColor = System.Drawing.SystemColors.InfoText;
			this.rtb.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtb.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.rtb.ForeColor = System.Drawing.Color.Lime;
			this.rtb.Location = new System.Drawing.Point(0, 0);
			this.rtb.Name = "rtb";
			this.rtb.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.rtb.Size = new System.Drawing.Size(119, 568);
			this.rtb.TabIndex = 0;
			this.rtb.Text = "";
			// 
			// TraderReportForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(119, 568);
			this.Controls.Add(this.rtb);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "TraderReportForm";
			this.Text = "TraderReportForm";
			this.Load += new System.EventHandler(this.TraderReportForm_Load);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TraderReportForm_MouseDown);
			this.ResumeLayout(false);

		}

		#endregion

		public RichTextBox rtb;
	}
}