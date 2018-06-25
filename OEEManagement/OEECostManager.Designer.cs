namespace MESSystem.OEEManagement
{
    partial class OEECostManager
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
            this.TlpCostManager = new System.Windows.Forms.TableLayoutPanel();
            this.FlpQuery = new System.Windows.Forms.FlowLayoutPanel();
            this.PnlMachineType = new System.Windows.Forms.Panel();
            this.LblMachineType = new System.Windows.Forms.Label();
            this.CmbMachineType = new System.Windows.Forms.ComboBox();
            this.PnlQueryPeriod = new System.Windows.Forms.Panel();
            this.LblQueryPeriod = new System.Windows.Forms.Label();
            this.CmbQueryPeriod = new System.Windows.Forms.ComboBox();
            this.PnlQueryDate = new System.Windows.Forms.Panel();
            this.LblQueryDate = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.LblYear = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.LblMonth = new System.Windows.Forms.Label();
            this.TabCostResult = new System.Windows.Forms.TabControl();
            this.PageCostChart = new System.Windows.Forms.TabPage();
            this.PageCostSheet = new System.Windows.Forms.TabPage();
            this.TlpCostManager.SuspendLayout();
            this.FlpQuery.SuspendLayout();
            this.PnlMachineType.SuspendLayout();
            this.PnlQueryPeriod.SuspendLayout();
            this.PnlQueryDate.SuspendLayout();
            this.TabCostResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // TlpCostManager
            // 
            this.TlpCostManager.ColumnCount = 1;
            this.TlpCostManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpCostManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpCostManager.Controls.Add(this.FlpQuery, 0, 0);
            this.TlpCostManager.Controls.Add(this.TabCostResult, 0, 1);
            this.TlpCostManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpCostManager.Location = new System.Drawing.Point(0, 0);
            this.TlpCostManager.Name = "TlpCostManager";
            this.TlpCostManager.RowCount = 2;
            this.TlpCostManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.990868F));
            this.TlpCostManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 92.00913F));
            this.TlpCostManager.Size = new System.Drawing.Size(1000, 438);
            this.TlpCostManager.TabIndex = 0;
            // 
            // FlpQuery
            // 
            this.FlpQuery.Controls.Add(this.PnlMachineType);
            this.FlpQuery.Controls.Add(this.PnlQueryPeriod);
            this.FlpQuery.Controls.Add(this.PnlQueryDate);
            this.FlpQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlpQuery.Location = new System.Drawing.Point(3, 3);
            this.FlpQuery.Name = "FlpQuery";
            this.FlpQuery.Size = new System.Drawing.Size(994, 29);
            this.FlpQuery.TabIndex = 0;
            // 
            // PnlMachineType
            // 
            this.PnlMachineType.Controls.Add(this.CmbMachineType);
            this.PnlMachineType.Controls.Add(this.LblMachineType);
            this.PnlMachineType.Location = new System.Drawing.Point(3, 3);
            this.PnlMachineType.Name = "PnlMachineType";
            this.PnlMachineType.Size = new System.Drawing.Size(152, 26);
            this.PnlMachineType.TabIndex = 0;
            // 
            // LblMachineType
            // 
            this.LblMachineType.AutoSize = true;
            this.LblMachineType.Location = new System.Drawing.Point(0, 7);
            this.LblMachineType.Name = "LblMachineType";
            this.LblMachineType.Size = new System.Drawing.Size(65, 12);
            this.LblMachineType.TabIndex = 0;
            this.LblMachineType.Text = "设备类型：";
            // 
            // CmbMachineType
            // 
            this.CmbMachineType.FormattingEnabled = true;
            this.CmbMachineType.Location = new System.Drawing.Point(62, 4);
            this.CmbMachineType.Name = "CmbMachineType";
            this.CmbMachineType.Size = new System.Drawing.Size(80, 20);
            this.CmbMachineType.TabIndex = 1;
            // 
            // PnlQueryPeriod
            // 
            this.PnlQueryPeriod.Controls.Add(this.CmbQueryPeriod);
            this.PnlQueryPeriod.Controls.Add(this.LblQueryPeriod);
            this.PnlQueryPeriod.Location = new System.Drawing.Point(161, 3);
            this.PnlQueryPeriod.Name = "PnlQueryPeriod";
            this.PnlQueryPeriod.Size = new System.Drawing.Size(113, 26);
            this.PnlQueryPeriod.TabIndex = 1;
            // 
            // LblQueryPeriod
            // 
            this.LblQueryPeriod.AutoSize = true;
            this.LblQueryPeriod.Location = new System.Drawing.Point(4, 7);
            this.LblQueryPeriod.Name = "LblQueryPeriod";
            this.LblQueryPeriod.Size = new System.Drawing.Size(65, 12);
            this.LblQueryPeriod.TabIndex = 0;
            this.LblQueryPeriod.Text = "查询周期：";
            // 
            // CmbQueryPeriod
            // 
            this.CmbQueryPeriod.FormattingEnabled = true;
            this.CmbQueryPeriod.Location = new System.Drawing.Point(66, 4);
            this.CmbQueryPeriod.Name = "CmbQueryPeriod";
            this.CmbQueryPeriod.Size = new System.Drawing.Size(35, 20);
            this.CmbQueryPeriod.TabIndex = 1;
            // 
            // PnlQueryDate
            // 
            this.PnlQueryDate.Controls.Add(this.LblMonth);
            this.PnlQueryDate.Controls.Add(this.comboBox2);
            this.PnlQueryDate.Controls.Add(this.LblYear);
            this.PnlQueryDate.Controls.Add(this.comboBox1);
            this.PnlQueryDate.Controls.Add(this.LblQueryDate);
            this.PnlQueryDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlQueryDate.Location = new System.Drawing.Point(280, 3);
            this.PnlQueryDate.Name = "PnlQueryDate";
            this.PnlQueryDate.Size = new System.Drawing.Size(374, 26);
            this.PnlQueryDate.TabIndex = 2;
            // 
            // LblQueryDate
            // 
            this.LblQueryDate.AutoSize = true;
            this.LblQueryDate.Location = new System.Drawing.Point(3, 7);
            this.LblQueryDate.Name = "LblQueryDate";
            this.LblQueryDate.Size = new System.Drawing.Size(65, 12);
            this.LblQueryDate.TabIndex = 0;
            this.LblQueryDate.Text = "查询日期：";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(74, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(49, 20);
            this.comboBox1.TabIndex = 1;
            // 
            // LblYear
            // 
            this.LblYear.AutoSize = true;
            this.LblYear.Location = new System.Drawing.Point(127, 7);
            this.LblYear.Name = "LblYear";
            this.LblYear.Size = new System.Drawing.Size(17, 12);
            this.LblYear.TabIndex = 2;
            this.LblYear.Text = "年";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(150, 3);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(32, 20);
            this.comboBox2.TabIndex = 3;
            // 
            // LblMonth
            // 
            this.LblMonth.AutoSize = true;
            this.LblMonth.Location = new System.Drawing.Point(187, 8);
            this.LblMonth.Name = "LblMonth";
            this.LblMonth.Size = new System.Drawing.Size(17, 12);
            this.LblMonth.TabIndex = 4;
            this.LblMonth.Text = "月";
            // 
            // TabCostResult
            // 
            this.TabCostResult.Controls.Add(this.PageCostChart);
            this.TabCostResult.Controls.Add(this.PageCostSheet);
            this.TabCostResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabCostResult.Location = new System.Drawing.Point(3, 38);
            this.TabCostResult.Name = "TabCostResult";
            this.TabCostResult.SelectedIndex = 0;
            this.TabCostResult.Size = new System.Drawing.Size(994, 397);
            this.TabCostResult.TabIndex = 1;
            // 
            // PageCostChart
            // 
            this.PageCostChart.Location = new System.Drawing.Point(4, 22);
            this.PageCostChart.Name = "PageCostChart";
            this.PageCostChart.Padding = new System.Windows.Forms.Padding(3);
            this.PageCostChart.Size = new System.Drawing.Size(986, 371);
            this.PageCostChart.TabIndex = 0;
            this.PageCostChart.Text = "成本分析图示";
            this.PageCostChart.UseVisualStyleBackColor = true;
            // 
            // PageCostSheet
            // 
            this.PageCostSheet.Location = new System.Drawing.Point(4, 22);
            this.PageCostSheet.Name = "PageCostSheet";
            this.PageCostSheet.Padding = new System.Windows.Forms.Padding(3);
            this.PageCostSheet.Size = new System.Drawing.Size(986, 371);
            this.PageCostSheet.TabIndex = 1;
            this.PageCostSheet.Text = "成本分析表";
            this.PageCostSheet.UseVisualStyleBackColor = true;
            // 
            // OEECostManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 438);
            this.Controls.Add(this.TlpCostManager);
            this.Name = "OEECostManager";
            this.Text = "CmbYear";
            this.TlpCostManager.ResumeLayout(false);
            this.FlpQuery.ResumeLayout(false);
            this.PnlMachineType.ResumeLayout(false);
            this.PnlMachineType.PerformLayout();
            this.PnlQueryPeriod.ResumeLayout(false);
            this.PnlQueryPeriod.PerformLayout();
            this.PnlQueryDate.ResumeLayout(false);
            this.PnlQueryDate.PerformLayout();
            this.TabCostResult.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpCostManager;
        private System.Windows.Forms.FlowLayoutPanel FlpQuery;
        private System.Windows.Forms.Panel PnlMachineType;
        private System.Windows.Forms.Label LblMachineType;
        private System.Windows.Forms.ComboBox CmbMachineType;
        private System.Windows.Forms.Panel PnlQueryPeriod;
        private System.Windows.Forms.ComboBox CmbQueryPeriod;
        private System.Windows.Forms.Label LblQueryPeriod;
        private System.Windows.Forms.Panel PnlQueryDate;
        private System.Windows.Forms.Label LblMonth;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label LblYear;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label LblQueryDate;
        private System.Windows.Forms.TabControl TabCostResult;
        private System.Windows.Forms.TabPage PageCostChart;
        private System.Windows.Forms.TabPage PageCostSheet;
    }
}