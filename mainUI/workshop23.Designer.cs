using System.Windows.Forms;

namespace MESSystem.mainUI
{
    partial class workshop23 : Form
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(workshop23));
            this.SuspendLayout();
            // 
            // workshop23
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1002);
            this.Name = "workshop23";
            this.Text = "车间生产状况一览";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.workshop23_FormClosing);
            this.Load += new System.EventHandler(this.workshop23_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.workshop23_MouseDown);
            this.ResumeLayout(false);

        }
    }
}