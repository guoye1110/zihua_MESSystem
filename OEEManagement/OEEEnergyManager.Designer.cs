namespace MESSystem.OEEManagement
{
    partial class OEEEnergyManager
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
            this.TlpEnergyManager = new System.Windows.Forms.TableLayoutPanel();
            this.FlpEnergyQuery = new System.Windows.Forms.FlowLayoutPanel();
            this.PnlMachineType = new System.Windows.Forms.Panel();
            this.CmbMachineType = new System.Windows.Forms.ComboBox();
            this.LblMachineType = new System.Windows.Forms.Label();
            this.PnlMachineQuery = new System.Windows.Forms.Panel();
            this.LblCalendar = new System.Windows.Forms.Label();
            this.DtpStart = new System.Windows.Forms.DateTimePicker();
            this.BtnQuery = new System.Windows.Forms.Button();
            this.DtpEnd = new System.Windows.Forms.DateTimePicker();
            this.LblDash = new System.Windows.Forms.Label();
            this.TabEnenryStatus = new System.Windows.Forms.TabControl();
            this.PageEnergyChart = new System.Windows.Forms.TabPage();
            this.PageEneryList = new System.Windows.Forms.TabPage();
            this.LvwEnergyView = new System.Windows.Forms.ListView();
            this.TlpEnergyManager.SuspendLayout();
            this.FlpEnergyQuery.SuspendLayout();
            this.PnlMachineType.SuspendLayout();
            this.PnlMachineQuery.SuspendLayout();
            this.TabEnenryStatus.SuspendLayout();
            this.PageEneryList.SuspendLayout();
            this.SuspendLayout();
            // 
            // TlpEnergyManager
            // 
            this.TlpEnergyManager.ColumnCount = 1;
            this.TlpEnergyManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpEnergyManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpEnergyManager.Controls.Add(this.FlpEnergyQuery, 0, 0);
            this.TlpEnergyManager.Controls.Add(this.TabEnenryStatus, 0, 1);
            this.TlpEnergyManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpEnergyManager.Location = new System.Drawing.Point(0, 0);
            this.TlpEnergyManager.Name = "TlpEnergyManager";
            this.TlpEnergyManager.RowCount = 2;
            this.TlpEnergyManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.210526F));
            this.TlpEnergyManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.78947F));
            this.TlpEnergyManager.Size = new System.Drawing.Size(989, 475);
            this.TlpEnergyManager.TabIndex = 0;
            // 
            // FlpEnergyQuery
            // 
            this.FlpEnergyQuery.Controls.Add(this.PnlMachineType);
            this.FlpEnergyQuery.Controls.Add(this.PnlMachineQuery);
            this.FlpEnergyQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlpEnergyQuery.Location = new System.Drawing.Point(3, 3);
            this.FlpEnergyQuery.Name = "FlpEnergyQuery";
            this.FlpEnergyQuery.Size = new System.Drawing.Size(983, 33);
            this.FlpEnergyQuery.TabIndex = 0;
            // 
            // PnlMachineType
            // 
            this.PnlMachineType.Controls.Add(this.CmbMachineType);
            this.PnlMachineType.Controls.Add(this.LblMachineType);
            this.PnlMachineType.Location = new System.Drawing.Point(3, 3);
            this.PnlMachineType.Name = "PnlMachineType";
            this.PnlMachineType.Size = new System.Drawing.Size(150, 28);
            this.PnlMachineType.TabIndex = 23;
            // 
            // CmbMachineType
            // 
            this.CmbMachineType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbMachineType.FormattingEnabled = true;
            this.CmbMachineType.Location = new System.Drawing.Point(67, 7);
            this.CmbMachineType.Margin = new System.Windows.Forms.Padding(0);
            this.CmbMachineType.Name = "CmbMachineType";
            this.CmbMachineType.Size = new System.Drawing.Size(68, 20);
            this.CmbMachineType.TabIndex = 21;
            // 
            // LblMachineType
            // 
            this.LblMachineType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblMachineType.AutoSize = true;
            this.LblMachineType.Location = new System.Drawing.Point(0, 10);
            this.LblMachineType.Margin = new System.Windows.Forms.Padding(0);
            this.LblMachineType.Name = "LblMachineType";
            this.LblMachineType.Size = new System.Drawing.Size(65, 12);
            this.LblMachineType.TabIndex = 20;
            this.LblMachineType.Text = "设备类型：";
            this.LblMachineType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PnlMachineQuery
            // 
            this.PnlMachineQuery.Controls.Add(this.LblCalendar);
            this.PnlMachineQuery.Controls.Add(this.DtpStart);
            this.PnlMachineQuery.Controls.Add(this.BtnQuery);
            this.PnlMachineQuery.Controls.Add(this.DtpEnd);
            this.PnlMachineQuery.Controls.Add(this.LblDash);
            this.PnlMachineQuery.Location = new System.Drawing.Point(159, 3);
            this.PnlMachineQuery.Name = "PnlMachineQuery";
            this.PnlMachineQuery.Size = new System.Drawing.Size(466, 28);
            this.PnlMachineQuery.TabIndex = 24;
            // 
            // LblCalendar
            // 
            this.LblCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblCalendar.AutoSize = true;
            this.LblCalendar.Location = new System.Drawing.Point(7, 10);
            this.LblCalendar.Margin = new System.Windows.Forms.Padding(0);
            this.LblCalendar.Name = "LblCalendar";
            this.LblCalendar.Size = new System.Drawing.Size(65, 12);
            this.LblCalendar.TabIndex = 18;
            this.LblCalendar.Text = "查询时间：";
            this.LblCalendar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DtpStart
            // 
            this.DtpStart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DtpStart.Location = new System.Drawing.Point(80, 7);
            this.DtpStart.Margin = new System.Windows.Forms.Padding(0);
            this.DtpStart.MinDate = new System.DateTime(2018, 1, 28, 0, 0, 0, 0);
            this.DtpStart.Name = "DtpStart";
            this.DtpStart.Size = new System.Drawing.Size(125, 21);
            this.DtpStart.TabIndex = 15;
            this.DtpStart.Value = new System.DateTime(2018, 1, 29, 0, 0, 0, 0);
            // 
            // BtnQuery
            // 
            this.BtnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnQuery.AutoSize = true;
            this.BtnQuery.Location = new System.Drawing.Point(374, 5);
            this.BtnQuery.Margin = new System.Windows.Forms.Padding(0);
            this.BtnQuery.Name = "BtnQuery";
            this.BtnQuery.Size = new System.Drawing.Size(75, 22);
            this.BtnQuery.TabIndex = 19;
            this.BtnQuery.Text = "查询";
            this.BtnQuery.UseVisualStyleBackColor = true;
            this.BtnQuery.Click += new System.EventHandler(this.BtnQuery_Click);
            // 
            // DtpEnd
            // 
            this.DtpEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DtpEnd.Location = new System.Drawing.Point(234, 6);
            this.DtpEnd.Margin = new System.Windows.Forms.Padding(0);
            this.DtpEnd.MinDate = new System.DateTime(2018, 1, 28, 0, 0, 0, 0);
            this.DtpEnd.Name = "DtpEnd";
            this.DtpEnd.Size = new System.Drawing.Size(125, 21);
            this.DtpEnd.TabIndex = 16;
            this.DtpEnd.Value = new System.DateTime(2018, 1, 29, 0, 0, 0, 0);
            // 
            // LblDash
            // 
            this.LblDash.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblDash.AutoSize = true;
            this.LblDash.Location = new System.Drawing.Point(214, 10);
            this.LblDash.Margin = new System.Windows.Forms.Padding(0);
            this.LblDash.Name = "LblDash";
            this.LblDash.Size = new System.Drawing.Size(11, 12);
            this.LblDash.TabIndex = 15;
            this.LblDash.Text = "-";
            this.LblDash.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TabEnenryStatus
            // 
            this.TabEnenryStatus.Controls.Add(this.PageEnergyChart);
            this.TabEnenryStatus.Controls.Add(this.PageEneryList);
            this.TabEnenryStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabEnenryStatus.Location = new System.Drawing.Point(3, 42);
            this.TabEnenryStatus.Name = "TabEnenryStatus";
            this.TabEnenryStatus.SelectedIndex = 0;
            this.TabEnenryStatus.Size = new System.Drawing.Size(983, 430);
            this.TabEnenryStatus.TabIndex = 1;
            // 
            // PageEnergyChart
            // 
            this.PageEnergyChart.Location = new System.Drawing.Point(4, 22);
            this.PageEnergyChart.Name = "PageEnergyChart";
            this.PageEnergyChart.Padding = new System.Windows.Forms.Padding(3);
            this.PageEnergyChart.Size = new System.Drawing.Size(975, 404);
            this.PageEnergyChart.TabIndex = 0;
            this.PageEnergyChart.Text = "能耗图表分析";
            this.PageEnergyChart.UseVisualStyleBackColor = true;
            // 
            // PageEneryList
            // 
            this.PageEneryList.Controls.Add(this.LvwEnergyView);
            this.PageEneryList.Location = new System.Drawing.Point(4, 22);
            this.PageEneryList.Name = "PageEneryList";
            this.PageEneryList.Padding = new System.Windows.Forms.Padding(3);
            this.PageEneryList.Size = new System.Drawing.Size(975, 404);
            this.PageEneryList.TabIndex = 1;
            this.PageEneryList.Text = "能耗列表";
            this.PageEneryList.UseVisualStyleBackColor = true;
            // 
            // LvwEnergyView
            // 
            this.LvwEnergyView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LvwEnergyView.Location = new System.Drawing.Point(3, 3);
            this.LvwEnergyView.Name = "LvwEnergyView";
            this.LvwEnergyView.Size = new System.Drawing.Size(969, 398);
            this.LvwEnergyView.TabIndex = 0;
            this.LvwEnergyView.UseCompatibleStateImageBehavior = false;
            // 
            // OEEEnergyManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(989, 475);
            this.Controls.Add(this.TlpEnergyManager);
            this.Name = "OEEEnergyManager";
            this.Text = "OEEEnergyManager";
            this.TlpEnergyManager.ResumeLayout(false);
            this.FlpEnergyQuery.ResumeLayout(false);
            this.PnlMachineType.ResumeLayout(false);
            this.PnlMachineType.PerformLayout();
            this.PnlMachineQuery.ResumeLayout(false);
            this.PnlMachineQuery.PerformLayout();
            this.TabEnenryStatus.ResumeLayout(false);
            this.PageEneryList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpEnergyManager;
        private System.Windows.Forms.FlowLayoutPanel FlpEnergyQuery;
        private System.Windows.Forms.Panel PnlMachineType;
        private System.Windows.Forms.ComboBox CmbMachineType;
        private System.Windows.Forms.Label LblMachineType;
        private System.Windows.Forms.Panel PnlMachineQuery;
        private System.Windows.Forms.Label LblCalendar;
        private System.Windows.Forms.DateTimePicker DtpStart;
        private System.Windows.Forms.Button BtnQuery;
        private System.Windows.Forms.DateTimePicker DtpEnd;
        private System.Windows.Forms.Label LblDash;
        private System.Windows.Forms.TabControl TabEnenryStatus;
        private System.Windows.Forms.TabPage PageEnergyChart;
        private System.Windows.Forms.TabPage PageEneryList;
        private System.Windows.Forms.ListView LvwEnergyView;
    }
}