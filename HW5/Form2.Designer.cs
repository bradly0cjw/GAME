namespace HW5
{
    partial class Form2
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_playcard = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.LightBlue;
            this.panel2.Location = new System.Drawing.Point(12, 76);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(317, 386);
            this.panel2.TabIndex = 3;
            // 
            // button_playcard
            // 
            this.button_playcard.Location = new System.Drawing.Point(197, 12);
            this.button_playcard.Name = "button_playcard";
            this.button_playcard.Size = new System.Drawing.Size(131, 53);
            this.button_playcard.TabIndex = 4;
            this.button_playcard.Text = "出牌";
            this.button_playcard.UseVisualStyleBackColor = true;
            this.button_playcard.Click += new System.EventHandler(this.button_playcard_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 474);
            this.Controls.Add(this.button_playcard);
            this.Controls.Add(this.panel2);
            this.Name = "Form2";
            this.Text = "資工四 111590453 張竣崴";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button_playcard;
    }
}