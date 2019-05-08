namespace VATSolution
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
            this.cmdsms = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdsms
            // 
            this.cmdsms.Location = new System.Drawing.Point(556, 71);
            this.cmdsms.Name = "cmdsms";
            this.cmdsms.Size = new System.Drawing.Size(144, 30);
            this.cmdsms.TabIndex = 0;
            this.cmdsms.Text = "Download data";
            this.cmdsms.UseVisualStyleBackColor = true;
            this.cmdsms.Click += new System.EventHandler(this.cmdsms_Click);
            // 
            // button2
            // 
            this.button2.ForeColor = System.Drawing.Color.Crimson;
            this.button2.Location = new System.Drawing.Point(556, 128);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(144, 30);
            this.button2.TabIndex = 6;
            this.button2.Text = "Failed ";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 179);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cmdsms);
            this.Name = "Form1";
            this.Text = "VAT test pad";
            this.ResumeLayout(false);
        }
        #endregion
        private System.Windows.Forms.Button cmdsms;
        private System.Windows.Forms.Button button2;
    }
}
