namespace WindowsFormsApplication1
{
    partial class dialogbox
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtWidth = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtHeight = new System.Windows.Forms.TextBox();
            this.BtnOK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(315, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Height";
            // 
            // TxtWidth
            // 
            this.TxtWidth.Location = new System.Drawing.Point(12, 58);
            this.TxtWidth.Name = "TxtWidth";
            this.TxtWidth.Size = new System.Drawing.Size(116, 20);
            this.TxtWidth.TabIndex = 2;
            this.TxtWidth.Text = "0";
            this.TxtWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtWidth_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(173, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "by";
            // 
            // TxtHeight
            // 
            this.TxtHeight.Location = new System.Drawing.Point(236, 58);
            this.TxtHeight.Name = "TxtHeight";
            this.TxtHeight.Size = new System.Drawing.Size(117, 20);
            this.TxtHeight.TabIndex = 4;
            this.TxtHeight.Text = "0";
            this.TxtHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtWidth_KeyPress);
            // 
            // BtnOK
            // 
            this.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnOK.Location = new System.Drawing.Point(142, 112);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 5;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "(tiles)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(244, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "*You cannot change this without reseting the level";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(322, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "(tiles)";
            // 
            // dialogbox
            // 
            this.AcceptButton = this.BtnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 162);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.TxtHeight);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TxtWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "dialogbox";
            this.Text = "Size?";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtHeight;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}