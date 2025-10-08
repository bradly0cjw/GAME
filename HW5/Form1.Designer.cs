namespace HW5
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
            this.button_circle = new System.Windows.Forms.Button();
            this.button_Rec = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // button_circle
            // 
            this.button_circle.Location = new System.Drawing.Point(12, 12);
            this.button_circle.Name = "button_circle";
            this.button_circle.Size = new System.Drawing.Size(100, 47);
            this.button_circle.TabIndex = 0;
            this.button_circle.Text = "Ten Circles";
            this.button_circle.UseVisualStyleBackColor = true;
            this.button_circle.Click += new System.EventHandler(this.button_circle_Click);
            // 
            // button_Rec
            // 
            this.button_Rec.Location = new System.Drawing.Point(118, 12);
            this.button_Rec.Name = "button_Rec";
            this.button_Rec.Size = new System.Drawing.Size(100, 47);
            this.button_Rec.TabIndex = 1;
            this.button_Rec.Text = "Ten Rectangles";
            this.button_Rec.UseVisualStyleBackColor = true;
            this.button_Rec.Click += new System.EventHandler(this.button_Rec_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightBlue;
            this.panel1.Location = new System.Drawing.Point(14, 77);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(317, 386);
            this.panel1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 474);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button_Rec);
            this.Controls.Add(this.button_circle);
            this.Name = "Form1";
            this.Text = "資工四 111590453 張竣崴";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_circle;
        private System.Windows.Forms.Button button_Rec;
        private System.Windows.Forms.Panel panel1;
    }
}

