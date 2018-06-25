namespace MESSystem.OEEManagement
{
    partial class OEEStaffManager
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
            this.TlpStaffManager = new System.Windows.Forms.TableLayoutPanel();
            this.FlpQueryPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.PnlStaffName = new System.Windows.Forms.Panel();
            this.CmbWorkshop = new System.Windows.Forms.ComboBox();
            this.LblWorkshop = new System.Windows.Forms.Label();
            this.CmbStaffName = new System.Windows.Forms.ComboBox();
            this.LblQueryKey = new System.Windows.Forms.Label();
            this.PnlStaffQuery = new System.Windows.Forms.Panel();
            this.LblMonthEnd = new System.Windows.Forms.Label();
            this.CmbMonthEnd = new System.Windows.Forms.ComboBox();
            this.LblYearEnd = new System.Windows.Forms.Label();
            this.CmbYearEnd = new System.Windows.Forms.ComboBox();
            this.LblMonthStart = new System.Windows.Forms.Label();
            this.CmbMonthStart = new System.Windows.Forms.ComboBox();
            this.LblYearStart = new System.Windows.Forms.Label();
            this.CmbYearStart = new System.Windows.Forms.ComboBox();
            this.BtnStaffQuery = new System.Windows.Forms.Button();
            this.LblDash = new System.Windows.Forms.Label();
            this.LblDashEnd = new System.Windows.Forms.Label();
            this.LblStaffQueryDate = new System.Windows.Forms.Label();
            this.TlpStaffStatus = new System.Windows.Forms.TableLayoutPanel();
            this.LblMonthView = new System.Windows.Forms.Label();
            this.LblDetailsView = new System.Windows.Forms.Label();
            this.LvwMonthView = new System.Windows.Forms.ListView();
            this.LvwDetailView = new System.Windows.Forms.ListView();
            this.BtnExport = new System.Windows.Forms.Button();
            this.TlpStaffManager.SuspendLayout();
            this.FlpQueryPanel.SuspendLayout();
            this.PnlStaffName.SuspendLayout();
            this.PnlStaffQuery.SuspendLayout();
            this.TlpStaffStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // TlpStaffManager
            // 
            this.TlpStaffManager.AutoSize = true;
            this.TlpStaffManager.ColumnCount = 1;
            this.TlpStaffManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpStaffManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlpStaffManager.Controls.Add(this.FlpQueryPanel, 0, 0);
            this.TlpStaffManager.Controls.Add(this.TlpStaffStatus, 0, 1);
            this.TlpStaffManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpStaffManager.Location = new System.Drawing.Point(0, 0);
            this.TlpStaffManager.Name = "TlpStaffManager";
            this.TlpStaffManager.Padding = new System.Windows.Forms.Padding(30);
            this.TlpStaffManager.RowCount = 2;
            this.TlpStaffManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.294964F));
            this.TlpStaffManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.70503F));
            this.TlpStaffManager.Size = new System.Drawing.Size(1083, 610);
            this.TlpStaffManager.TabIndex = 0;
            // 
            // FlpQueryPanel
            // 
            this.FlpQueryPanel.Controls.Add(this.PnlStaffName);
            this.FlpQueryPanel.Controls.Add(this.PnlStaffQuery);
            this.FlpQueryPanel.Controls.Add(this.BtnExport);
            this.FlpQueryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlpQueryPanel.Location = new System.Drawing.Point(33, 33);
            this.FlpQueryPanel.Name = "FlpQueryPanel";
            this.FlpQueryPanel.Size = new System.Drawing.Size(1017, 28);
            this.FlpQueryPanel.TabIndex = 0;
            // 
            // PnlStaffName
            // 
            this.PnlStaffName.Controls.Add(this.CmbWorkshop);
            this.PnlStaffName.Controls.Add(this.LblWorkshop);
            this.PnlStaffName.Controls.Add(this.CmbStaffName);
            this.PnlStaffName.Controls.Add(this.LblQueryKey);
            this.PnlStaffName.Location = new System.Drawing.Point(3, 3);
            this.PnlStaffName.Name = "PnlStaffName";
            this.PnlStaffName.Size = new System.Drawing.Size(338, 25);
            this.PnlStaffName.TabIndex = 0;
            // 
            // CmbWorkshop
            // 
            this.CmbWorkshop.FormattingEnabled = true;
            this.CmbWorkshop.Location = new System.Drawing.Point(66, 2);
            this.CmbWorkshop.Name = "CmbWorkshop";
            this.CmbWorkshop.Size = new System.Drawing.Size(98, 20);
            this.CmbWorkshop.TabIndex = 5;
            this.CmbWorkshop.SelectedIndexChanged += new System.EventHandler(this.CmbWorkshop_SelectedIndexChanged);
            // 
            // LblWorkshop
            // 
            this.LblWorkshop.AutoSize = true;
            this.LblWorkshop.Location = new System.Drawing.Point(1, 7);
            this.LblWorkshop.Name = "LblWorkshop";
            this.LblWorkshop.Size = new System.Drawing.Size(65, 12);
            this.LblWorkshop.TabIndex = 4;
            this.LblWorkshop.Text = "生产车间：";
            this.LblWorkshop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CmbStaffName
            // 
            this.CmbStaffName.FormattingEnabled = true;
            this.CmbStaffName.Location = new System.Drawing.Point(241, 3);
            this.CmbStaffName.Name = "CmbStaffName";
            this.CmbStaffName.Size = new System.Drawing.Size(94, 20);
            this.CmbStaffName.TabIndex = 3;
            // 
            // LblQueryKey
            // 
            this.LblQueryKey.AutoSize = true;
            this.LblQueryKey.Location = new System.Drawing.Point(170, 6);
            this.LblQueryKey.Name = "LblQueryKey";
            this.LblQueryKey.Size = new System.Drawing.Size(65, 12);
            this.LblQueryKey.TabIndex = 0;
            this.LblQueryKey.Text = "人员列表：";
            this.LblQueryKey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PnlStaffQuery
            // 
            this.PnlStaffQuery.Controls.Add(this.LblMonthEnd);
            this.PnlStaffQuery.Controls.Add(this.CmbMonthEnd);
            this.PnlStaffQuery.Controls.Add(this.LblYearEnd);
            this.PnlStaffQuery.Controls.Add(this.CmbYearEnd);
            this.PnlStaffQuery.Controls.Add(this.LblMonthStart);
            this.PnlStaffQuery.Controls.Add(this.CmbMonthStart);
            this.PnlStaffQuery.Controls.Add(this.LblYearStart);
            this.PnlStaffQuery.Controls.Add(this.CmbYearStart);
            this.PnlStaffQuery.Controls.Add(this.BtnStaffQuery);
            this.PnlStaffQuery.Controls.Add(this.LblDash);
            this.PnlStaffQuery.Controls.Add(this.LblDashEnd);
            this.PnlStaffQuery.Controls.Add(this.LblStaffQueryDate);
            this.PnlStaffQuery.Location = new System.Drawing.Point(347, 3);
            this.PnlStaffQuery.Name = "PnlStaffQuery";
            this.PnlStaffQuery.Size = new System.Drawing.Size(444, 25);
            this.PnlStaffQuery.TabIndex = 1;
            // 
            // LblMonthEnd
            // 
            this.LblMonthEnd.AutoSize = true;
            this.LblMonthEnd.Location = new System.Drawing.Point(316, 7);
            this.LblMonthEnd.Name = "LblMonthEnd";
            this.LblMonthEnd.Size = new System.Drawing.Size(17, 12);
            this.LblMonthEnd.TabIndex = 13;
            this.LblMonthEnd.Text = "月";
            this.LblMonthEnd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CmbMonthEnd
            // 
            this.CmbMonthEnd.FormattingEnabled = true;
            this.CmbMonthEnd.Location = new System.Drawing.Point(278, 4);
            this.CmbMonthEnd.Name = "CmbMonthEnd";
            this.CmbMonthEnd.Size = new System.Drawing.Size(35, 20);
            this.CmbMonthEnd.TabIndex = 12;
            // 
            // LblYearEnd
            // 
            this.LblYearEnd.AutoSize = true;
            this.LblYearEnd.Location = new System.Drawing.Point(260, 7);
            this.LblYearEnd.Name = "LblYearEnd";
            this.LblYearEnd.Size = new System.Drawing.Size(17, 12);
            this.LblYearEnd.TabIndex = 11;
            this.LblYearEnd.Text = "年";
            this.LblYearEnd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CmbYearEnd
            // 
            this.CmbYearEnd.FormattingEnabled = true;
            this.CmbYearEnd.Location = new System.Drawing.Point(206, 3);
            this.CmbYearEnd.Name = "CmbYearEnd";
            this.CmbYearEnd.Size = new System.Drawing.Size(51, 20);
            this.CmbYearEnd.TabIndex = 10;
            // 
            // LblMonthStart
            // 
            this.LblMonthStart.AutoSize = true;
            this.LblMonthStart.Location = new System.Drawing.Point(172, 7);
            this.LblMonthStart.Name = "LblMonthStart";
            this.LblMonthStart.Size = new System.Drawing.Size(17, 12);
            this.LblMonthStart.TabIndex = 9;
            this.LblMonthStart.Text = "月";
            this.LblMonthStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CmbMonthStart
            // 
            this.CmbMonthStart.FormattingEnabled = true;
            this.CmbMonthStart.Location = new System.Drawing.Point(132, 3);
            this.CmbMonthStart.Name = "CmbMonthStart";
            this.CmbMonthStart.Size = new System.Drawing.Size(35, 20);
            this.CmbMonthStart.TabIndex = 8;
            // 
            // LblYearStart
            // 
            this.LblYearStart.AutoSize = true;
            this.LblYearStart.Location = new System.Drawing.Point(114, 7);
            this.LblYearStart.Name = "LblYearStart";
            this.LblYearStart.Size = new System.Drawing.Size(17, 12);
            this.LblYearStart.TabIndex = 7;
            this.LblYearStart.Text = "年";
            this.LblYearStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CmbYearStart
            // 
            this.CmbYearStart.FormattingEnabled = true;
            this.CmbYearStart.Location = new System.Drawing.Point(62, 3);
            this.CmbYearStart.Name = "CmbYearStart";
            this.CmbYearStart.Size = new System.Drawing.Size(51, 20);
            this.CmbYearStart.TabIndex = 6;
            // 
            // BtnStaffQuery
            // 
            this.BtnStaffQuery.Location = new System.Drawing.Point(339, 1);
            this.BtnStaffQuery.Name = "BtnStaffQuery";
            this.BtnStaffQuery.Size = new System.Drawing.Size(75, 23);
            this.BtnStaffQuery.TabIndex = 5;
            this.BtnStaffQuery.Text = "查询";
            this.BtnStaffQuery.UseVisualStyleBackColor = true;
            this.BtnStaffQuery.Click += new System.EventHandler(this.BtnStaffQuery_Click);
            // 
            // LblDash
            // 
            this.LblDash.AutoSize = true;
            this.LblDash.Location = new System.Drawing.Point(189, 6);
            this.LblDash.Name = "LblDash";
            this.LblDash.Size = new System.Drawing.Size(11, 12);
            this.LblDash.TabIndex = 3;
            this.LblDash.Text = "-";
            // 
            // LblDashEnd
            // 
            this.LblDashEnd.AutoSize = true;
            this.LblDashEnd.Location = new System.Drawing.Point(189, 3);
            this.LblDashEnd.Name = "LblDashEnd";
            this.LblDashEnd.Size = new System.Drawing.Size(11, 12);
            this.LblDashEnd.TabIndex = 2;
            this.LblDashEnd.Text = "-";
            // 
            // LblStaffQueryDate
            // 
            this.LblStaffQueryDate.AutoSize = true;
            this.LblStaffQueryDate.Location = new System.Drawing.Point(3, 6);
            this.LblStaffQueryDate.Name = "LblStaffQueryDate";
            this.LblStaffQueryDate.Size = new System.Drawing.Size(65, 12);
            this.LblStaffQueryDate.TabIndex = 0;
            this.LblStaffQueryDate.Text = "查询时间：";
            this.LblStaffQueryDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TlpStaffStatus
            // 
            this.TlpStaffStatus.ColumnCount = 2;
            this.TlpStaffStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.13779F));
            this.TlpStaffStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.86221F));
            this.TlpStaffStatus.Controls.Add(this.LblMonthView, 0, 0);
            this.TlpStaffStatus.Controls.Add(this.LblDetailsView, 1, 0);
            this.TlpStaffStatus.Controls.Add(this.LvwMonthView, 0, 1);
            this.TlpStaffStatus.Controls.Add(this.LvwDetailView, 1, 1);
            this.TlpStaffStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpStaffStatus.Location = new System.Drawing.Point(33, 67);
            this.TlpStaffStatus.Name = "TlpStaffStatus";
            this.TlpStaffStatus.RowCount = 2;
            this.TlpStaffStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.426357F));
            this.TlpStaffStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94.57365F));
            this.TlpStaffStatus.Size = new System.Drawing.Size(1017, 510);
            this.TlpStaffStatus.TabIndex = 1;
            // 
            // LblMonthView
            // 
            this.LblMonthView.AutoSize = true;
            this.LblMonthView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblMonthView.Location = new System.Drawing.Point(3, 0);
            this.LblMonthView.Name = "LblMonthView";
            this.LblMonthView.Size = new System.Drawing.Size(351, 27);
            this.LblMonthView.TabIndex = 0;
            this.LblMonthView.Text = "员工月度生产状况统计";
            this.LblMonthView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblDetailsView
            // 
            this.LblDetailsView.AutoSize = true;
            this.LblDetailsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblDetailsView.Location = new System.Drawing.Point(360, 0);
            this.LblDetailsView.Name = "LblDetailsView";
            this.LblDetailsView.Size = new System.Drawing.Size(654, 27);
            this.LblDetailsView.TabIndex = 1;
            this.LblDetailsView.Text = "员工当月生产明细";
            this.LblDetailsView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LvwMonthView
            // 
            this.LvwMonthView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LvwMonthView.Location = new System.Drawing.Point(3, 30);
            this.LvwMonthView.Name = "LvwMonthView";
            this.LvwMonthView.Size = new System.Drawing.Size(351, 477);
            this.LvwMonthView.TabIndex = 2;
            this.LvwMonthView.UseCompatibleStateImageBehavior = false;
            this.LvwMonthView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.LvwMonthView_ItemSelectionChanged);
            // 
            // LvwDetailView
            // 
            this.LvwDetailView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LvwDetailView.Location = new System.Drawing.Point(362, 32);
            this.LvwDetailView.Margin = new System.Windows.Forms.Padding(5);
            this.LvwDetailView.Name = "LvwDetailView";
            this.LvwDetailView.Size = new System.Drawing.Size(650, 473);
            this.LvwDetailView.TabIndex = 3;
            this.LvwDetailView.UseCompatibleStateImageBehavior = false;
            // 
            // BtnExport
            // 
            this.BtnExport.Location = new System.Drawing.Point(797, 3);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(75, 23);
            this.BtnExport.TabIndex = 2;
            this.BtnExport.Text = "导出列表";
            this.BtnExport.UseVisualStyleBackColor = true;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // OEEStaffManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1083, 610);
            this.Controls.Add(this.TlpStaffManager);
            this.Name = "OEEStaffManager";
            this.Text = "OEEStaffManager";
            this.TlpStaffManager.ResumeLayout(false);
            this.FlpQueryPanel.ResumeLayout(false);
            this.PnlStaffName.ResumeLayout(false);
            this.PnlStaffName.PerformLayout();
            this.PnlStaffQuery.ResumeLayout(false);
            this.PnlStaffQuery.PerformLayout();
            this.TlpStaffStatus.ResumeLayout(false);
            this.TlpStaffStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpStaffManager;
        private System.Windows.Forms.FlowLayoutPanel FlpQueryPanel;
        private System.Windows.Forms.Panel PnlStaffName;
        private System.Windows.Forms.Panel PnlStaffQuery;
        private System.Windows.Forms.Label LblStaffQueryDate;
        private System.Windows.Forms.Label LblDashEnd;
        private System.Windows.Forms.Label LblDash;
        private System.Windows.Forms.Label LblQueryKey;
        private System.Windows.Forms.Button BtnStaffQuery;
        private System.Windows.Forms.ComboBox CmbStaffName;
        private System.Windows.Forms.ComboBox CmbWorkshop;
        private System.Windows.Forms.Label LblWorkshop;
        private System.Windows.Forms.Label LblMonthStart;
        private System.Windows.Forms.ComboBox CmbMonthStart;
        private System.Windows.Forms.Label LblYearStart;
        private System.Windows.Forms.ComboBox CmbYearStart;
        private System.Windows.Forms.Label LblMonthEnd;
        private System.Windows.Forms.ComboBox CmbMonthEnd;
        private System.Windows.Forms.Label LblYearEnd;
        private System.Windows.Forms.ComboBox CmbYearEnd;
        private System.Windows.Forms.TableLayoutPanel TlpStaffStatus;
        private System.Windows.Forms.Label LblMonthView;
        private System.Windows.Forms.Label LblDetailsView;
        private System.Windows.Forms.ListView LvwMonthView;
        private System.Windows.Forms.ListView LvwDetailView;
        private System.Windows.Forms.Button BtnExport;
    }
}