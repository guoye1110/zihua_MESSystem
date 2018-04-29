namespace MESSystem.OEEManagement
{
    partial class OEESummaryManager
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
            this.TlpSummary = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.PnlMachineType = new System.Windows.Forms.Panel();
            this.CmbMachineType = new System.Windows.Forms.ComboBox();
            this.LblMachineType = new System.Windows.Forms.Label();
            this.PnlQueryPeriod = new System.Windows.Forms.Panel();
            this.CmbQueryPeriod = new System.Windows.Forms.ComboBox();
            this.LblQueryType = new System.Windows.Forms.Label();
            this.PnlQueryDate = new System.Windows.Forms.Panel();
            this.BtnQuery = new System.Windows.Forms.Button();
            this.LblMonth = new System.Windows.Forms.Label();
            this.CmbMonth = new System.Windows.Forms.ComboBox();
            this.LblYear = new System.Windows.Forms.Label();
            this.CmbYear = new System.Windows.Forms.ComboBox();
            this.LblQueryDate = new System.Windows.Forms.Label();
            this.TabSummary = new System.Windows.Forms.TabControl();
            this.PageOEE = new System.Windows.Forms.TabPage();
            this.PageTimeRate = new System.Windows.Forms.TabPage();
            this.PagePerformanceRate = new System.Windows.Forms.TabPage();
            this.TlpPerformance = new System.Windows.Forms.TableLayoutPanel();
            this.LblPerformanceTitle = new System.Windows.Forms.Label();
            this.TabQualityRate = new System.Windows.Forms.TabPage();
            this.TlpQualityRate = new System.Windows.Forms.TableLayoutPanel();
            this.LblQualityTitle = new System.Windows.Forms.Label();
            this.PnlQualityPercentage = new System.Windows.Forms.Panel();
            this.PnlPerformancePercentage = new System.Windows.Forms.Panel();
            this.TlpHoursRate = new System.Windows.Forms.TableLayoutPanel();
            this.PnlHoursRatePercentage = new System.Windows.Forms.Panel();
            this.LblHoursTitle = new System.Windows.Forms.Label();
            this.TlpOEESummary = new System.Windows.Forms.TableLayoutPanel();
            this.LblOEESummary = new System.Windows.Forms.Label();
            this.PnlOEESummaryPercentage = new System.Windows.Forms.Panel();
            this.TlpSummary.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.PnlMachineType.SuspendLayout();
            this.PnlQueryPeriod.SuspendLayout();
            this.PnlQueryDate.SuspendLayout();
            this.TabSummary.SuspendLayout();
            this.PageOEE.SuspendLayout();
            this.PageTimeRate.SuspendLayout();
            this.PagePerformanceRate.SuspendLayout();
            this.TlpPerformance.SuspendLayout();
            this.TabQualityRate.SuspendLayout();
            this.TlpQualityRate.SuspendLayout();
            this.TlpHoursRate.SuspendLayout();
            this.TlpOEESummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // TlpSummary
            // 
            this.TlpSummary.AutoScroll = true;
            this.TlpSummary.ColumnCount = 1;
            this.TlpSummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpSummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpSummary.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.TlpSummary.Controls.Add(this.TabSummary, 0, 1);
            this.TlpSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpSummary.Location = new System.Drawing.Point(0, 0);
            this.TlpSummary.Name = "TlpSummary";
            this.TlpSummary.RowCount = 2;
            this.TlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.055363F));
            this.TlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.94463F));
            this.TlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpSummary.Size = new System.Drawing.Size(1029, 578);
            this.TlpSummary.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.PnlMachineType);
            this.flowLayoutPanel1.Controls.Add(this.PnlQueryPeriod);
            this.flowLayoutPanel1.Controls.Add(this.PnlQueryDate);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1023, 28);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // PnlMachineType
            // 
            this.PnlMachineType.Controls.Add(this.CmbMachineType);
            this.PnlMachineType.Controls.Add(this.LblMachineType);
            this.PnlMachineType.Location = new System.Drawing.Point(3, 3);
            this.PnlMachineType.Name = "PnlMachineType";
            this.PnlMachineType.Size = new System.Drawing.Size(158, 25);
            this.PnlMachineType.TabIndex = 0;
            // 
            // CmbMachineType
            // 
            this.CmbMachineType.FormattingEnabled = true;
            this.CmbMachineType.Location = new System.Drawing.Point(65, 2);
            this.CmbMachineType.Name = "CmbMachineType";
            this.CmbMachineType.Size = new System.Drawing.Size(80, 20);
            this.CmbMachineType.TabIndex = 1;
            this.CmbMachineType.SelectedIndexChanged += new System.EventHandler(this.CmbMachineType_SelectedIndexChanged);
            // 
            // LblMachineType
            // 
            this.LblMachineType.AutoSize = true;
            this.LblMachineType.Location = new System.Drawing.Point(3, 5);
            this.LblMachineType.Name = "LblMachineType";
            this.LblMachineType.Size = new System.Drawing.Size(65, 12);
            this.LblMachineType.TabIndex = 0;
            this.LblMachineType.Text = "设备类型：";
            // 
            // PnlQueryPeriod
            // 
            this.PnlQueryPeriod.Controls.Add(this.CmbQueryPeriod);
            this.PnlQueryPeriod.Controls.Add(this.LblQueryType);
            this.PnlQueryPeriod.Location = new System.Drawing.Point(167, 3);
            this.PnlQueryPeriod.Name = "PnlQueryPeriod";
            this.PnlQueryPeriod.Size = new System.Drawing.Size(115, 25);
            this.PnlQueryPeriod.TabIndex = 1;
            // 
            // CmbQueryPeriod
            // 
            this.CmbQueryPeriod.FormattingEnabled = true;
            this.CmbQueryPeriod.Location = new System.Drawing.Point(64, 2);
            this.CmbQueryPeriod.Name = "CmbQueryPeriod";
            this.CmbQueryPeriod.Size = new System.Drawing.Size(39, 20);
            this.CmbQueryPeriod.TabIndex = 1;
            this.CmbQueryPeriod.SelectedIndexChanged += new System.EventHandler(this.CmbQueryPeriod_SelectedIndexChanged);
            // 
            // LblQueryType
            // 
            this.LblQueryType.AutoSize = true;
            this.LblQueryType.Location = new System.Drawing.Point(4, 7);
            this.LblQueryType.Name = "LblQueryType";
            this.LblQueryType.Size = new System.Drawing.Size(65, 12);
            this.LblQueryType.TabIndex = 0;
            this.LblQueryType.Text = "查询周期：";
            // 
            // PnlQueryDate
            // 
            this.PnlQueryDate.Controls.Add(this.BtnQuery);
            this.PnlQueryDate.Controls.Add(this.LblMonth);
            this.PnlQueryDate.Controls.Add(this.CmbMonth);
            this.PnlQueryDate.Controls.Add(this.LblYear);
            this.PnlQueryDate.Controls.Add(this.CmbYear);
            this.PnlQueryDate.Controls.Add(this.LblQueryDate);
            this.PnlQueryDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlQueryDate.Location = new System.Drawing.Point(288, 3);
            this.PnlQueryDate.Name = "PnlQueryDate";
            this.PnlQueryDate.Size = new System.Drawing.Size(322, 25);
            this.PnlQueryDate.TabIndex = 2;
            // 
            // BtnQuery
            // 
            this.BtnQuery.Location = new System.Drawing.Point(201, 1);
            this.BtnQuery.Name = "BtnQuery";
            this.BtnQuery.Size = new System.Drawing.Size(75, 23);
            this.BtnQuery.TabIndex = 5;
            this.BtnQuery.Text = "查询";
            this.BtnQuery.UseVisualStyleBackColor = true;
            this.BtnQuery.Click += new System.EventHandler(this.BtnQuery_Click);
            // 
            // LblMonth
            // 
            this.LblMonth.AutoSize = true;
            this.LblMonth.Location = new System.Drawing.Point(177, 6);
            this.LblMonth.Name = "LblMonth";
            this.LblMonth.Size = new System.Drawing.Size(17, 12);
            this.LblMonth.TabIndex = 4;
            this.LblMonth.Text = "月";
            // 
            // CmbMonth
            // 
            this.CmbMonth.FormattingEnabled = true;
            this.CmbMonth.Location = new System.Drawing.Point(145, 3);
            this.CmbMonth.Name = "CmbMonth";
            this.CmbMonth.Size = new System.Drawing.Size(29, 20);
            this.CmbMonth.TabIndex = 3;
            // 
            // LblYear
            // 
            this.LblYear.AutoSize = true;
            this.LblYear.Location = new System.Drawing.Point(126, 6);
            this.LblYear.Name = "LblYear";
            this.LblYear.Size = new System.Drawing.Size(17, 12);
            this.LblYear.TabIndex = 2;
            this.LblYear.Text = "年";
            // 
            // CmbYear
            // 
            this.CmbYear.FormattingEnabled = true;
            this.CmbYear.Location = new System.Drawing.Point(66, 3);
            this.CmbYear.Name = "CmbYear";
            this.CmbYear.Size = new System.Drawing.Size(59, 20);
            this.CmbYear.TabIndex = 1;
            // 
            // LblQueryDate
            // 
            this.LblQueryDate.AutoSize = true;
            this.LblQueryDate.Location = new System.Drawing.Point(4, 6);
            this.LblQueryDate.Name = "LblQueryDate";
            this.LblQueryDate.Size = new System.Drawing.Size(65, 12);
            this.LblQueryDate.TabIndex = 0;
            this.LblQueryDate.Text = "查询时间：";
            // 
            // TabSummary
            // 
            this.TabSummary.Controls.Add(this.PageOEE);
            this.TabSummary.Controls.Add(this.PageTimeRate);
            this.TabSummary.Controls.Add(this.PagePerformanceRate);
            this.TabSummary.Controls.Add(this.TabQualityRate);
            this.TabSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabSummary.Location = new System.Drawing.Point(3, 37);
            this.TabSummary.Name = "TabSummary";
            this.TabSummary.SelectedIndex = 0;
            this.TabSummary.Size = new System.Drawing.Size(1023, 538);
            this.TabSummary.TabIndex = 1;
            this.TabSummary.SelectedIndexChanged += new System.EventHandler(this.TabSummary_SelectedIndexChanged);
            // 
            // PageOEE
            // 
            this.PageOEE.Controls.Add(this.TlpOEESummary);
            this.PageOEE.Location = new System.Drawing.Point(4, 22);
            this.PageOEE.Name = "PageOEE";
            this.PageOEE.Padding = new System.Windows.Forms.Padding(3);
            this.PageOEE.Size = new System.Drawing.Size(1015, 512);
            this.PageOEE.TabIndex = 0;
            this.PageOEE.Text = "设备OEE";
            this.PageOEE.UseVisualStyleBackColor = true;
            // 
            // PageTimeRate
            // 
            this.PageTimeRate.Controls.Add(this.TlpHoursRate);
            this.PageTimeRate.Location = new System.Drawing.Point(4, 22);
            this.PageTimeRate.Name = "PageTimeRate";
            this.PageTimeRate.Padding = new System.Windows.Forms.Padding(3);
            this.PageTimeRate.Size = new System.Drawing.Size(1015, 512);
            this.PageTimeRate.TabIndex = 1;
            this.PageTimeRate.Text = "时间开动率";
            this.PageTimeRate.UseVisualStyleBackColor = true;
            // 
            // PagePerformanceRate
            // 
            this.PagePerformanceRate.Controls.Add(this.TlpPerformance);
            this.PagePerformanceRate.Location = new System.Drawing.Point(4, 22);
            this.PagePerformanceRate.Name = "PagePerformanceRate";
            this.PagePerformanceRate.Padding = new System.Windows.Forms.Padding(3);
            this.PagePerformanceRate.Size = new System.Drawing.Size(1015, 512);
            this.PagePerformanceRate.TabIndex = 2;
            this.PagePerformanceRate.Text = "性能开动率";
            this.PagePerformanceRate.UseVisualStyleBackColor = true;
            // 
            // TlpPerformance
            // 
            this.TlpPerformance.ColumnCount = 1;
            this.TlpPerformance.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpPerformance.Controls.Add(this.LblPerformanceTitle, 0, 0);
            this.TlpPerformance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpPerformance.Location = new System.Drawing.Point(3, 3);
            this.TlpPerformance.Name = "TlpPerformance";
            this.TlpPerformance.RowCount = 3;
            this.TlpPerformance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.TlpPerformance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TlpPerformance.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.TlpPerformance.Size = new System.Drawing.Size(1009, 506);
            this.TlpPerformance.TabIndex = 0;
            // 
            // LblPerformanceTitle
            // 
            this.LblPerformanceTitle.AutoSize = true;
            this.LblPerformanceTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblPerformanceTitle.Location = new System.Drawing.Point(3, 0);
            this.LblPerformanceTitle.Name = "LblPerformanceTitle";
            this.LblPerformanceTitle.Size = new System.Drawing.Size(1003, 75);
            this.LblPerformanceTitle.TabIndex = 0;
            this.LblPerformanceTitle.Text = "label1";
            this.LblPerformanceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TabQualityRate
            // 
            this.TabQualityRate.Controls.Add(this.TlpQualityRate);
            this.TabQualityRate.Location = new System.Drawing.Point(4, 22);
            this.TabQualityRate.Name = "TabQualityRate";
            this.TabQualityRate.Padding = new System.Windows.Forms.Padding(3);
            this.TabQualityRate.Size = new System.Drawing.Size(1015, 512);
            this.TabQualityRate.TabIndex = 3;
            this.TabQualityRate.Text = "产品合格率";
            this.TabQualityRate.UseVisualStyleBackColor = true;
            // 
            // TlpQualityRate
            // 
            this.TlpQualityRate.ColumnCount = 1;
            this.TlpQualityRate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpQualityRate.Controls.Add(this.LblQualityTitle, 0, 0);
            this.TlpQualityRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpQualityRate.Location = new System.Drawing.Point(3, 3);
            this.TlpQualityRate.Name = "TlpQualityRate";
            this.TlpQualityRate.RowCount = 3;
            this.TlpQualityRate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.TlpQualityRate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TlpQualityRate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.TlpQualityRate.Size = new System.Drawing.Size(1009, 506);
            this.TlpQualityRate.TabIndex = 0;
            // 
            // LblQualityTitle
            // 
            this.LblQualityTitle.AutoSize = true;
            this.LblQualityTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblQualityTitle.Location = new System.Drawing.Point(3, 0);
            this.LblQualityTitle.Name = "LblQualityTitle";
            this.LblQualityTitle.Size = new System.Drawing.Size(1003, 75);
            this.LblQualityTitle.TabIndex = 0;
            this.LblQualityTitle.Text = "label1";
            this.LblQualityTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PnlQualityPercentage
            // 
            this.PnlQualityPercentage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlQualityPercentage.Location = new System.Drawing.Point(3, 78);
            this.PnlQualityPercentage.Name = "PnlQualityPercentage";
            this.PnlQualityPercentage.Size = new System.Drawing.Size(1003, 44);
            this.PnlQualityPercentage.TabIndex = 1;
            // 
            // PnlPerformancePercentage
            // 
            this.PnlPerformancePercentage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlPerformancePercentage.Location = new System.Drawing.Point(3, 78);
            this.PnlPerformancePercentage.Name = "PnlPerformancePercentage";
            this.PnlPerformancePercentage.Size = new System.Drawing.Size(1003, 44);
            this.PnlPerformancePercentage.TabIndex = 1;
            // 
            // TlpHoursRate
            // 
            this.TlpHoursRate.ColumnCount = 1;
            this.TlpHoursRate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpHoursRate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpHoursRate.Controls.Add(this.PnlHoursRatePercentage, 0, 1);
            this.TlpHoursRate.Controls.Add(this.LblHoursTitle, 0, 0);
            this.TlpHoursRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpHoursRate.Location = new System.Drawing.Point(3, 3);
            this.TlpHoursRate.Name = "TlpHoursRate";
            this.TlpHoursRate.RowCount = 3;
            this.TlpHoursRate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.TlpHoursRate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TlpHoursRate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.TlpHoursRate.Size = new System.Drawing.Size(1009, 506);
            this.TlpHoursRate.TabIndex = 0;
            // 
            // PnlHoursRatePercentage
            // 
            this.PnlHoursRatePercentage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlHoursRatePercentage.Location = new System.Drawing.Point(3, 78);
            this.PnlHoursRatePercentage.Name = "PnlHoursRatePercentage";
            this.PnlHoursRatePercentage.Size = new System.Drawing.Size(1003, 44);
            this.PnlHoursRatePercentage.TabIndex = 0;
            // 
            // LblHoursTitle
            // 
            this.LblHoursTitle.AutoSize = true;
            this.LblHoursTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblHoursTitle.Location = new System.Drawing.Point(3, 0);
            this.LblHoursTitle.Name = "LblHoursTitle";
            this.LblHoursTitle.Size = new System.Drawing.Size(1003, 75);
            this.LblHoursTitle.TabIndex = 1;
            this.LblHoursTitle.Text = "label1";
            this.LblHoursTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TlpOEESummary
            // 
            this.TlpOEESummary.ColumnCount = 1;
            this.TlpOEESummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpOEESummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlpOEESummary.Controls.Add(this.LblOEESummary, 0, 0);
            this.TlpOEESummary.Controls.Add(this.PnlOEESummaryPercentage, 0, 1);
            this.TlpOEESummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpOEESummary.Location = new System.Drawing.Point(3, 3);
            this.TlpOEESummary.Name = "TlpOEESummary";
            this.TlpOEESummary.RowCount = 3;
            this.TlpOEESummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.TlpOEESummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TlpOEESummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.TlpOEESummary.Size = new System.Drawing.Size(1009, 506);
            this.TlpOEESummary.TabIndex = 0;
            // 
            // LblOEESummary
            // 
            this.LblOEESummary.AutoSize = true;
            this.LblOEESummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblOEESummary.Location = new System.Drawing.Point(3, 0);
            this.LblOEESummary.Name = "LblOEESummary";
            this.LblOEESummary.Size = new System.Drawing.Size(1003, 75);
            this.LblOEESummary.TabIndex = 0;
            this.LblOEESummary.Text = "label1";
            this.LblOEESummary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PnlOEESummaryPercentage
            // 
            this.PnlOEESummaryPercentage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlOEESummaryPercentage.Location = new System.Drawing.Point(3, 78);
            this.PnlOEESummaryPercentage.Name = "PnlOEESummaryPercentage";
            this.PnlOEESummaryPercentage.Size = new System.Drawing.Size(1003, 44);
            this.PnlOEESummaryPercentage.TabIndex = 1;
            // 
            // OEESummaryManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 578);
            this.Controls.Add(this.TlpSummary);
            this.Name = "OEESummaryManager";
            this.Text = "OEESummary";
            this.TlpSummary.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.PnlMachineType.ResumeLayout(false);
            this.PnlMachineType.PerformLayout();
            this.PnlQueryPeriod.ResumeLayout(false);
            this.PnlQueryPeriod.PerformLayout();
            this.PnlQueryDate.ResumeLayout(false);
            this.PnlQueryDate.PerformLayout();
            this.TabSummary.ResumeLayout(false);
            this.PageOEE.ResumeLayout(false);
            this.PageTimeRate.ResumeLayout(false);
            this.PagePerformanceRate.ResumeLayout(false);
            this.TlpPerformance.ResumeLayout(false);
            this.TlpPerformance.PerformLayout();
            this.TabQualityRate.ResumeLayout(false);
            this.TlpQualityRate.ResumeLayout(false);
            this.TlpQualityRate.PerformLayout();
            this.TlpHoursRate.ResumeLayout(false);
            this.TlpHoursRate.PerformLayout();
            this.TlpOEESummary.ResumeLayout(false);
            this.TlpOEESummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpSummary;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel PnlMachineType;
        private System.Windows.Forms.ComboBox CmbMachineType;
        private System.Windows.Forms.Label LblMachineType;
        private System.Windows.Forms.Panel PnlQueryPeriod;
        private System.Windows.Forms.ComboBox CmbQueryPeriod;
        private System.Windows.Forms.Label LblQueryType;
        private System.Windows.Forms.Panel PnlQueryDate;
        private System.Windows.Forms.Label LblQueryDate;
        private System.Windows.Forms.Label LblMonth;
        private System.Windows.Forms.ComboBox CmbMonth;
        private System.Windows.Forms.Label LblYear;
        private System.Windows.Forms.ComboBox CmbYear;
        private System.Windows.Forms.TabControl TabSummary;
        private System.Windows.Forms.TabPage PageOEE;
        private System.Windows.Forms.TabPage PageTimeRate;
        private System.Windows.Forms.TabPage PagePerformanceRate;
        private System.Windows.Forms.TabPage TabQualityRate;
        private System.Windows.Forms.Button BtnQuery;
        private System.Windows.Forms.TableLayoutPanel TlpQualityRate;
        private System.Windows.Forms.Label LblQualityTitle;
        private System.Windows.Forms.Panel PnlQualityPercentage;
        private System.Windows.Forms.TableLayoutPanel TlpPerformance;
        private System.Windows.Forms.Label LblPerformanceTitle;
        private System.Windows.Forms.Panel PnlPerformancePercentage;
        private System.Windows.Forms.TableLayoutPanel TlpHoursRate;
        private System.Windows.Forms.Panel PnlHoursRatePercentage;
        private System.Windows.Forms.Label LblHoursTitle;
        private System.Windows.Forms.TableLayoutPanel TlpOEESummary;
        private System.Windows.Forms.Label LblOEESummary;
        private System.Windows.Forms.Panel PnlOEESummaryPercentage;
    }
}