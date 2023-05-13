using Model;
using DAL;
using BLL;

namespace Meeteam
{
    partial class FirstForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.logbut = new System.Windows.Forms.Button();
            this.registerbut = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.idbox = new System.Windows.Forms.TextBox();
            this.pwdbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 25.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(87, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(598, 57);
            this.label1.TabIndex = 0;
            this.label1.Text = "This is the Meeteam System";
            // 
            // logbut
            // 
            this.logbut.Location = new System.Drawing.Point(245, 360);
            this.logbut.Name = "logbut";
            this.logbut.Size = new System.Drawing.Size(115, 48);
            this.logbut.TabIndex = 1;
            this.logbut.Text = "Login";
            this.logbut.UseVisualStyleBackColor = true;
            this.logbut.Click += new System.EventHandler(this.logbut_Click);
            // 
            // registerbut
            // 
            this.registerbut.Location = new System.Drawing.Point(441, 360);
            this.registerbut.Name = "registerbut";
            this.registerbut.Size = new System.Drawing.Size(115, 48);
            this.registerbut.TabIndex = 2;
            this.registerbut.Text = "Register";
            this.registerbut.UseVisualStyleBackColor = true;
            this.registerbut.Click += new System.EventHandler(this.registerbut_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(231, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 27);
            this.label2.TabIndex = 3;
            this.label2.Text = "User ID : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(206, 229);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 27);
            this.label3.TabIndex = 4;
            this.label3.Text = "PassWord : ";
            // 
            // idbox
            // 
            this.idbox.Location = new System.Drawing.Point(357, 138);
            this.idbox.Name = "idbox";
            this.idbox.Size = new System.Drawing.Size(222, 27);
            this.idbox.TabIndex = 5;
            // 
            // pwdbox
            // 
            this.pwdbox.Location = new System.Drawing.Point(357, 231);
            this.pwdbox.Name = "pwdbox";
            this.pwdbox.Size = new System.Drawing.Size(222, 27);
            this.pwdbox.TabIndex = 6;
            // 
            // FirstForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pwdbox);
            this.Controls.Add(this.idbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.registerbut);
            this.Controls.Add(this.logbut);
            this.Controls.Add(this.label1);
            this.Name = "FirstForm";
            this.Text = "Welcome to Meeteam!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button logbut;
        private System.Windows.Forms.Button registerbut;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox idbox;
        private System.Windows.Forms.TextBox pwdbox;
    }
}

