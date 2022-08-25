namespace Jenny
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.fastTextBox1 = new Jenny.FastTextBox();
            this.SuspendLayout();
            // 
            // fastTextBox1
            // 
            this.fastTextBox1.AutoScroll = true;
            this.fastTextBox1.BackColor = System.Drawing.SystemColors.Window;
            this.fastTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastTextBox1.Location = new System.Drawing.Point(0, 0);
            this.fastTextBox1.Name = "fastTextBox1";
            this.fastTextBox1.Size = new System.Drawing.Size(584, 561);
            this.fastTextBox1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.fastTextBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Jenny";
            this.ResumeLayout(false);

		}

		#endregion
		private FastTextBox fastTextBox1;
	}
}
