namespace MESSystem.OEEManagement
{
    partial class MaintainaceCalendar
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
            this.TlpMaintain = new System.Windows.Forms.TableLayoutPanel();
            this.LblMaintain = new System.Windows.Forms.Label();
            this.TlpMachineList = new System.Windows.Forms.TableLayoutPanel();
            this.LblMachineList = new System.Windows.Forms.Label();
            this.LstMachine = new System.Windows.Forms.ListBox();
            this.PnlCalendar = new System.Windows.Forms.Panel();
            this.TlpMaintain.SuspendLayout();
            this.TlpMachineList.SuspendLayout();
            this.SuspendLayout();
            // 
            // TlpMaintain
            // 
            this.TlpMaintain.ColumnCount = 2;
            this.TlpMaintain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.8656F));
            this.TlpMaintain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.1344F));
            this.TlpMaintain.Controls.Add(this.LblMaintain, 0, 0);
            this.TlpMaintain.Controls.Add(this.TlpMachineList, 0, 1);
            this.TlpMaintain.Controls.Add(this.PnlCalendar, 1, 1);
            this.TlpMaintain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpMaintain.Location = new System.Drawing.Point(0, 0);
            this.TlpMaintain.Name = "TlpMaintain";
            this.TlpMaintain.RowCount = 2;
            this.TlpMaintain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TlpMaintain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.TlpMaintain.Size = new System.Drawing.Size(997, 548);
            this.TlpMaintain.TabIndex = 0;
            // 
            // LblMaintain
            // 
            this.LblMaintain.AutoSize = true;
            this.TlpMaintain.SetColumnSpan(this.LblMaintain, 2);
            this.LblMaintain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblMaintain.Location = new System.Drawing.Point(3, 0);
            this.LblMaintain.Name = "LblMaintain";
            this.LblMaintain.Size = new System.Drawing.Size(991, 54);
            this.LblMaintain.TabIndex = 0;
            this.LblMaintain.Text = "label1";
            this.LblMaintain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TlpMachineList
            // 
            this.TlpMachineList.ColumnCount = 1;
            this.TlpMachineList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpMachineList.Controls.Add(this.LblMachineList, 0, 0);
            this.TlpMachineList.Controls.Add(this.LstMachine, 0, 1);
            this.TlpMachineList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpMachineList.Location = new System.Drawing.Point(80, 57);
            this.TlpMachineList.Margin = new System.Windows.Forms.Padding(80, 3, 3, 3);
            this.TlpMachineList.Name = "TlpMachineList";
            this.TlpMachineList.RowCount = 2;
            this.TlpMachineList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.TlpMachineList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95F));
            this.TlpMachineList.Size = new System.Drawing.Size(135, 488);
            this.TlpMachineList.TabIndex = 1;
            // 
            // LblMachineList
            // 
            this.LblMachineList.AutoSize = true;
            this.LblMachineList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblMachineList.Location = new System.Drawing.Point(3, 3);
            this.LblMachineList.Margin = new System.Windows.Forms.Padding(3);
            this.LblMachineList.Name = "LblMachineList";
            this.LblMachineList.Size = new System.Drawing.Size(129, 18);
            this.LblMachineList.TabIndex = 0;
            this.LblMachineList.Text = "label1";
            this.LblMachineList.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LstMachine
            // 
            this.LstMachine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LstMachine.FormattingEnabled = true;
            this.LstMachine.ItemHeight = 12;
            this.LstMachine.Location = new System.Drawing.Point(3, 27);
            this.LstMachine.Name = "LstMachine";
            this.LstMachine.Size = new System.Drawing.Size(129, 458);
            this.LstMachine.TabIndex = 1;
            // 
            // PnlCalendar
            // 
            this.PnlCalendar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlCalendar.Location = new System.Drawing.Point(221, 57);
            this.PnlCalendar.Name = "PnlCalendar";
            this.PnlCalendar.Size = new System.Drawing.Size(773, 488);
            this.PnlCalendar.TabIndex = 2;
            // 
            // MaintainaceCalendar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 548);
            this.Controls.Add(this.TlpMaintain);
            this.Name = "MaintainaceCalendar";
            this.Text = "MaintainaceCalendar";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MaintainaceCalendar_FormClosing);
            this.Load += new System.EventHandler(this.MaintainaceCalendar_Load);
            this.TlpMaintain.ResumeLayout(false);
            this.TlpMaintain.PerformLayout();
            this.TlpMachineList.ResumeLayout(false);
            this.TlpMachineList.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpMaintain;
        private System.Windows.Forms.Label LblMaintain;
        private System.Windows.Forms.TableLayoutPanel TlpMachineList;
        private System.Windows.Forms.Label LblMachineList;
        private System.Windows.Forms.ListBox LstMachine;
        private System.Windows.Forms.Panel PnlCalendar;
    }
}