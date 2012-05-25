namespace AutoGenPano
{
    partial class Form1
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
            this.buttonGo = new System.Windows.Forms.Button();
            this.sitrep = new System.Windows.Forms.StatusStrip();
            this.sitrepStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.sitrep.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonGo
            // 
            this.buttonGo.Location = new System.Drawing.Point(12, 12);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(571, 225);
            this.buttonGo.TabIndex = 0;
            this.buttonGo.Text = "Go.";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.button1_Click);
            // 
            // sitrep
            // 
            this.sitrep.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sitrepStatusLabel});
            this.sitrep.Location = new System.Drawing.Point(0, 240);
            this.sitrep.Name = "sitrep";
            this.sitrep.Size = new System.Drawing.Size(595, 22);
            this.sitrep.TabIndex = 1;
            this.sitrep.Text = "Sitrep";
            // 
            // sitrepStatusLabel
            // 
            this.sitrepStatusLabel.Name = "sitrepStatusLabel";
            this.sitrepStatusLabel.Size = new System.Drawing.Size(57, 17);
            this.sitrepStatusLabel.Text = "Waiting...";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 262);
            this.Controls.Add(this.sitrep);
            this.Controls.Add(this.buttonGo);
            this.Name = "Form1";
            this.Text = "AutoPano";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.sitrep.ResumeLayout(false);
            this.sitrep.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.StatusStrip sitrep;
        private System.Windows.Forms.ToolStripStatusLabel sitrepStatusLabel;
    }
}

