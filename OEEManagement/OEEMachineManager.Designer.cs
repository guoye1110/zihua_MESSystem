namespace MESSystem.OEEManagement
{
    partial class OEEMachineManager
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
            this.TlpOEE = new System.Windows.Forms.TableLayoutPanel();
            this.FlpPeriodQuery = new System.Windows.Forms.FlowLayoutPanel();
            this.PnlMachineType = new System.Windows.Forms.Panel();
            this.CmbMachineType = new System.Windows.Forms.ComboBox();
            this.LblMachineType = new System.Windows.Forms.Label();
            this.PnlMachineQuery = new System.Windows.Forms.Panel();
            this.LblCalendar = new System.Windows.Forms.Label();
            this.DtpMachineDateStart = new System.Windows.Forms.DateTimePicker();
            this.BtnMachineQuery = new System.Windows.Forms.Button();
            this.DtpMachineDateEnd = new System.Windows.Forms.DateTimePicker();
            this.LblDash = new System.Windows.Forms.Label();
            this.TabMachineOuputStatus = new System.Windows.Forms.TabControl();
            this.PageOuputCapacity = new System.Windows.Forms.TabPage();
            this.TlpChart = new System.Windows.Forms.TableLayoutPanel();
            this.FlpChartTitle = new System.Windows.Forms.FlowLayoutPanel();
            this.LblChartQueryPeriod = new System.Windows.Forms.Label();
            this.LblChartTitle = new System.Windows.Forms.Label();
            this.PageOutputStatus = new System.Windows.Forms.TabPage();
            this.TlpOutputStatus = new System.Windows.Forms.TableLayoutPanel();
            this.LblOutputStatus = new System.Windows.Forms.Label();
            this.PageOuputHours = new System.Windows.Forms.TabPage();
            this.TlpMachineHours = new System.Windows.Forms.TableLayoutPanel();
            this.HoursDataGridView = new System.Windows.Forms.DataGridView();
            this.TlpPieChart = new System.Windows.Forms.TableLayoutPanel();
            this.PnlPercentage = new System.Windows.Forms.Panel();
            this.TlpOEE.SuspendLayout();
            this.FlpPeriodQuery.SuspendLayout();
            this.PnlMachineType.SuspendLayout();
            this.PnlMachineQuery.SuspendLayout();
            this.TabMachineOuputStatus.SuspendLayout();
            this.PageOuputCapacity.SuspendLayout();
            this.TlpChart.SuspendLayout();
            this.FlpChartTitle.SuspendLayout();
            this.PageOutputStatus.SuspendLayout();
            this.TlpOutputStatus.SuspendLayout();
            this.PageOuputHours.SuspendLayout();
            this.TlpMachineHours.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HoursDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // TlpOEE
            // 
            this.TlpOEE.ColumnCount = 1;
            this.TlpOEE.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpOEE.Controls.Add(this.FlpPeriodQuery, 0, 0);
            this.TlpOEE.Controls.Add(this.TabMachineOuputStatus, 0, 1);
            this.TlpOEE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpOEE.Location = new System.Drawing.Point(0, 0);
            this.TlpOEE.Name = "TlpOEE";
            this.TlpOEE.Padding = new System.Windows.Forms.Padding(30);
            this.TlpOEE.RowCount = 2;
            this.TlpOEE.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpOEE.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TlpOEE.Size = new System.Drawing.Size(1104, 618);
            this.TlpOEE.TabIndex = 0;
            // 
            // FlpPeriodQuery
            // 
            this.FlpPeriodQuery.AutoSize = true;
            this.FlpPeriodQuery.Controls.Add(this.PnlMachineType);
            this.FlpPeriodQuery.Controls.Add(this.PnlMachineQuery);
            this.FlpPeriodQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlpPeriodQuery.Location = new System.Drawing.Point(30, 30);
            this.FlpPeriodQuery.Margin = new System.Windows.Forms.Padding(0);
            this.FlpPeriodQuery.Name = "FlpPeriodQuery";
            this.FlpPeriodQuery.Size = new System.Drawing.Size(1044, 34);
            this.FlpPeriodQuery.TabIndex = 0;
            // 
            // PnlMachineType
            // 
            this.PnlMachineType.Controls.Add(this.CmbMachineType);
            this.PnlMachineType.Controls.Add(this.LblMachineType);
            this.PnlMachineType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlMachineType.Location = new System.Drawing.Point(3, 3);
            this.PnlMachineType.Name = "PnlMachineType";
            this.PnlMachineType.Size = new System.Drawing.Size(150, 28);
            this.PnlMachineType.TabIndex = 22;
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
            this.CmbMachineType.SelectedIndexChanged += new System.EventHandler(this.CmbMachineType_SelectedIndexChanged);
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
            this.PnlMachineQuery.Controls.Add(this.DtpMachineDateStart);
            this.PnlMachineQuery.Controls.Add(this.BtnMachineQuery);
            this.PnlMachineQuery.Controls.Add(this.DtpMachineDateEnd);
            this.PnlMachineQuery.Controls.Add(this.LblDash);
            this.PnlMachineQuery.Location = new System.Drawing.Point(159, 3);
            this.PnlMachineQuery.Name = "PnlMachineQuery";
            this.PnlMachineQuery.Size = new System.Drawing.Size(466, 28);
            this.PnlMachineQuery.TabIndex = 23;
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
            // DtpMachineDateStart
            // 
            this.DtpMachineDateStart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DtpMachineDateStart.Location = new System.Drawing.Point(80, 7);
            this.DtpMachineDateStart.Margin = new System.Windows.Forms.Padding(0);
            this.DtpMachineDateStart.MinDate = new System.DateTime(2018, 1, 28, 0, 0, 0, 0);
            this.DtpMachineDateStart.Name = "DtpMachineDateStart";
            this.DtpMachineDateStart.Size = new System.Drawing.Size(125, 21);
            this.DtpMachineDateStart.TabIndex = 15;
            this.DtpMachineDateStart.Value = new System.DateTime(2018, 1, 29, 0, 0, 0, 0);
            this.DtpMachineDateStart.ValueChanged += new System.EventHandler(this.DtpTimeStart_ValueChanged);
            // 
            // BtnMachineQuery
            // 
            this.BtnMachineQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnMachineQuery.AutoSize = true;
            this.BtnMachineQuery.Location = new System.Drawing.Point(374, 5);
            this.BtnMachineQuery.Margin = new System.Windows.Forms.Padding(0);
            this.BtnMachineQuery.Name = "BtnMachineQuery";
            this.BtnMachineQuery.Size = new System.Drawing.Size(75, 22);
            this.BtnMachineQuery.TabIndex = 19;
            this.BtnMachineQuery.Text = "查询";
            this.BtnMachineQuery.UseVisualStyleBackColor = true;
            this.BtnMachineQuery.Click += new System.EventHandler(this.BtnDeviceQuery_Click);
            // 
            // DtpMachineDateEnd
            // 
            this.DtpMachineDateEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DtpMachineDateEnd.Location = new System.Drawing.Point(234, 6);
            this.DtpMachineDateEnd.Margin = new System.Windows.Forms.Padding(0);
            this.DtpMachineDateEnd.MinDate = new System.DateTime(2018, 1, 28, 0, 0, 0, 0);
            this.DtpMachineDateEnd.Name = "DtpMachineDateEnd";
            this.DtpMachineDateEnd.Size = new System.Drawing.Size(125, 21);
            this.DtpMachineDateEnd.TabIndex = 16;
            this.DtpMachineDateEnd.Value = new System.DateTime(2018, 1, 29, 0, 0, 0, 0);
            this.DtpMachineDateEnd.ValueChanged += new System.EventHandler(this.DtpDateEnd_ValueChanged);
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
            // TabMachineOuputStatus
            // 
            this.TlpOEE.SetColumnSpan(this.TabMachineOuputStatus, 2);
            this.TabMachineOuputStatus.Controls.Add(this.PageOuputCapacity);
            this.TabMachineOuputStatus.Controls.Add(this.PageOutputStatus);
            this.TabMachineOuputStatus.Controls.Add(this.PageOuputHours);
            this.TabMachineOuputStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabMachineOuputStatus.Location = new System.Drawing.Point(30, 74);
            this.TabMachineOuputStatus.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.TabMachineOuputStatus.Name = "TabMachineOuputStatus";
            this.TabMachineOuputStatus.SelectedIndex = 0;
            this.TabMachineOuputStatus.Size = new System.Drawing.Size(1044, 514);
            this.TabMachineOuputStatus.TabIndex = 1;
            this.TabMachineOuputStatus.SelectedIndexChanged += new System.EventHandler(this.TabMachineOuputStatus_SelectedIndexChanged);
            // 
            // PageOuputCapacity
            // 
            this.PageOuputCapacity.Controls.Add(this.TlpChart);
            this.PageOuputCapacity.Location = new System.Drawing.Point(4, 22);
            this.PageOuputCapacity.Margin = new System.Windows.Forms.Padding(0);
            this.PageOuputCapacity.Name = "PageOuputCapacity";
            this.PageOuputCapacity.Padding = new System.Windows.Forms.Padding(3);
            this.PageOuputCapacity.Size = new System.Drawing.Size(1036, 488);
            this.PageOuputCapacity.TabIndex = 0;
            this.PageOuputCapacity.Text = "目标达成率";
            this.PageOuputCapacity.UseVisualStyleBackColor = true;
            // 
            // TlpChart
            // 
            this.TlpChart.ColumnCount = 1;
            this.TlpChart.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpChart.Controls.Add(this.FlpChartTitle, 0, 0);
            this.TlpChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpChart.Location = new System.Drawing.Point(3, 3);
            this.TlpChart.Margin = new System.Windows.Forms.Padding(0);
            this.TlpChart.Name = "TlpChart";
            this.TlpChart.RowCount = 3;
            this.TlpChart.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.TlpChart.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TlpChart.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.TlpChart.Size = new System.Drawing.Size(1030, 482);
            this.TlpChart.TabIndex = 0;
            // 
            // FlpChartTitle
            // 
            this.FlpChartTitle.Controls.Add(this.LblChartQueryPeriod);
            this.FlpChartTitle.Controls.Add(this.LblChartTitle);
            this.FlpChartTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlpChartTitle.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.FlpChartTitle.Location = new System.Drawing.Point(3, 3);
            this.FlpChartTitle.Name = "FlpChartTitle";
            this.FlpChartTitle.Size = new System.Drawing.Size(1024, 66);
            this.FlpChartTitle.TabIndex = 0;
            // 
            // LblChartQueryPeriod
            // 
            this.LblChartQueryPeriod.Dock = System.Windows.Forms.DockStyle.Right;
            this.LblChartQueryPeriod.Location = new System.Drawing.Point(3, 0);
            this.LblChartQueryPeriod.Name = "LblChartQueryPeriod";
            this.LblChartQueryPeriod.Size = new System.Drawing.Size(1040, 16);
            this.LblChartQueryPeriod.TabIndex = 0;
            this.LblChartQueryPeriod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblChartTitle
            // 
            this.LblChartTitle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LblChartTitle.Location = new System.Drawing.Point(3, 16);
            this.LblChartTitle.Name = "LblChartTitle";
            this.LblChartTitle.Size = new System.Drawing.Size(1040, 47);
            this.LblChartTitle.TabIndex = 0;
            this.LblChartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PageOutputStatus
            // 
            this.PageOutputStatus.Controls.Add(this.TlpOutputStatus);
            this.PageOutputStatus.Location = new System.Drawing.Point(4, 22);
            this.PageOutputStatus.Name = "PageOutputStatus";
            this.PageOutputStatus.Size = new System.Drawing.Size(1053, 468);
            this.PageOutputStatus.TabIndex = 1;
            this.PageOutputStatus.Text = "生产状态";
            // 
            // TlpOutputStatus
            // 
            this.TlpOutputStatus.ColumnCount = 1;
            this.TlpOutputStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpOutputStatus.Controls.Add(this.LblOutputStatus, 0, 0);
            this.TlpOutputStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpOutputStatus.Location = new System.Drawing.Point(0, 0);
            this.TlpOutputStatus.Name = "TlpOutputStatus";
            this.TlpOutputStatus.RowCount = 2;
            this.TlpOutputStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.52632F));
            this.TlpOutputStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.47369F));
            this.TlpOutputStatus.Size = new System.Drawing.Size(1053, 468);
            this.TlpOutputStatus.TabIndex = 0;
            // 
            // LblOutputStatus
            // 
            this.LblOutputStatus.AutoSize = true;
            this.LblOutputStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblOutputStatus.Location = new System.Drawing.Point(3, 0);
            this.LblOutputStatus.Name = "LblOutputStatus";
            this.LblOutputStatus.Size = new System.Drawing.Size(1047, 49);
            this.LblOutputStatus.TabIndex = 2;
            this.LblOutputStatus.Text = "label1";
            this.LblOutputStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PageOuputHours
            // 
            this.PageOuputHours.Controls.Add(this.TlpMachineHours);
            this.PageOuputHours.Location = new System.Drawing.Point(4, 22);
            this.PageOuputHours.Name = "PageOuputHours";
            this.PageOuputHours.Padding = new System.Windows.Forms.Padding(3);
            this.PageOuputHours.Size = new System.Drawing.Size(1053, 468);
            this.PageOuputHours.TabIndex = 2;
            this.PageOuputHours.Text = "设备工时";
            this.PageOuputHours.UseVisualStyleBackColor = true;
            // 
            // TlpMachineHours
            // 
            this.TlpMachineHours.ColumnCount = 1;
            this.TlpMachineHours.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpMachineHours.Controls.Add(this.HoursDataGridView, 0, 0);
            this.TlpMachineHours.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpMachineHours.Location = new System.Drawing.Point(3, 3);
            this.TlpMachineHours.Margin = new System.Windows.Forms.Padding(5);
            this.TlpMachineHours.Name = "TlpMachineHours";
            this.TlpMachineHours.RowCount = 1;
            this.TlpMachineHours.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpMachineHours.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 462F));
            this.TlpMachineHours.Size = new System.Drawing.Size(1047, 462);
            this.TlpMachineHours.TabIndex = 0;
            // 
            // HoursDataGridView
            // 
            this.HoursDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.HoursDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HoursDataGridView.Location = new System.Drawing.Point(3, 3);
            this.HoursDataGridView.Name = "HoursDataGridView";
            this.HoursDataGridView.RowTemplate.Height = 23;
            this.HoursDataGridView.Size = new System.Drawing.Size(1041, 456);
            this.HoursDataGridView.TabIndex = 2;
            // 
            // TlpPieChart
            // 
            this.TlpPieChart.ColumnCount = 1;
            this.TlpPieChart.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpPieChart.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpPieChart.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TlpPieChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpPieChart.Location = new System.Drawing.Point(3, 73);
            this.TlpPieChart.Name = "TlpPieChart";
            this.TlpPieChart.RowCount = 1;
            this.TlpPieChart.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpPieChart.Size = new System.Drawing.Size(1047, 40);
            this.TlpPieChart.TabIndex = 1;
            // 
            // PnlPercentage
            // 
            this.PnlPercentage.AutoSize = true;
            this.PnlPercentage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlPercentage.Location = new System.Drawing.Point(3, 72);
            this.PnlPercentage.Name = "PnlPercentage";
            this.PnlPercentage.Size = new System.Drawing.Size(1041, 40);
            this.PnlPercentage.TabIndex = 0;
            // 
            // OEEMachineManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1104, 618);
            this.Controls.Add(this.TlpOEE);
            this.Name = "OEEMachineManager";
            this.Text = "设备工时分析";
            this.Load += new System.EventHandler(this.OEEManager_Load);
            this.TlpOEE.ResumeLayout(false);
            this.TlpOEE.PerformLayout();
            this.FlpPeriodQuery.ResumeLayout(false);
            this.PnlMachineType.ResumeLayout(false);
            this.PnlMachineType.PerformLayout();
            this.PnlMachineQuery.ResumeLayout(false);
            this.PnlMachineQuery.PerformLayout();
            this.TabMachineOuputStatus.ResumeLayout(false);
            this.PageOuputCapacity.ResumeLayout(false);
            this.TlpChart.ResumeLayout(false);
            this.FlpChartTitle.ResumeLayout(false);
            this.PageOutputStatus.ResumeLayout(false);
            this.TlpOutputStatus.ResumeLayout(false);
            this.TlpOutputStatus.PerformLayout();
            this.PageOuputHours.ResumeLayout(false);
            this.TlpMachineHours.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.HoursDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

      

#endregion

        private System.Windows.Forms.TableLayoutPanel TlpOEE;
        private System.Windows.Forms.FlowLayoutPanel FlpPeriodQuery;
        private System.Windows.Forms.Label LblCalendar;
        private System.Windows.Forms.DateTimePicker DtpMachineDateStart;
        private System.Windows.Forms.Label LblDash;
        private System.Windows.Forms.DateTimePicker DtpMachineDateEnd;
        private System.Windows.Forms.Button BtnMachineQuery;
        private System.Windows.Forms.Label LblMachineType;
        private System.Windows.Forms.ComboBox CmbMachineType;
        private System.Windows.Forms.Panel PnlMachineType;
        private System.Windows.Forms.Panel PnlMachineQuery;
        private System.Windows.Forms.TabControl TabMachineOuputStatus;
        private System.Windows.Forms.TabPage PageOutputStatus;
        private System.Windows.Forms.TabPage PageOuputHours;
        public System.Windows.Forms.TableLayoutPanel TlpMachineHours;
        private System.Windows.Forms.Panel PnlPercentage;
        private System.Windows.Forms.TableLayoutPanel TlpPieChart;
        private System.Windows.Forms.TabPage PageOuputCapacity;
        private System.Windows.Forms.TableLayoutPanel TlpChart;
        private System.Windows.Forms.Label LblChartTitle;
        private System.Windows.Forms.FlowLayoutPanel FlpChartTitle;
        private System.Windows.Forms.Label LblChartQueryPeriod;
        private System.Windows.Forms.Label LblOutputStatus;
        private System.Windows.Forms.TableLayoutPanel TlpOutputStatus;
        private System.Windows.Forms.DataGridView HoursDataGridView;
    }
}