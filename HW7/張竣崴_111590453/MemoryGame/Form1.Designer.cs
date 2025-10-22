namespace MemoryGame
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
            this.button1 = new System.Windows.Forms.Button();
            this.panel_tool = new System.Windows.Forms.Panel();
            this.panel_board = new System.Windows.Forms.Panel();
            this.panel_tool.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 61);
            this.button1.TabIndex = 0;
            this.button1.Text = "Memory Game";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel_tool
            // 
            this.panel_tool.Controls.Add(this.button1);
            this.panel_tool.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_tool.Location = new System.Drawing.Point(0, 0);
            this.panel_tool.Name = "panel_tool";
            this.panel_tool.Size = new System.Drawing.Size(800, 88);
            this.panel_tool.TabIndex = 1;
            // 
            // panel_board
            // 
            this.panel_board.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_board.Location = new System.Drawing.Point(0, 88);
            this.panel_board.Name = "panel_board";
            this.panel_board.Size = new System.Drawing.Size(800, 362);
            this.panel_board.TabIndex = 2;
            this.panel_board.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel_board_MouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel_board);
            this.Controls.Add(this.panel_tool);
            this.Name = "Form1";
            this.Text = "111590453 張竣崴";
            this.panel_tool.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel_tool;
        private System.Windows.Forms.Panel panel_board;
    }
}

