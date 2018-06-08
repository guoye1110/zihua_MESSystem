namespace LabelPrint.PrintForms
{
    partial class UpdateScanForm
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
            this.bt_Cancel = new System.Windows.Forms.Button();
            this.bt_Ok = new System.Windows.Forms.Button();
            this.tb_BarCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LstB_Barcode = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bt_Cancel
            // 
            this.bt_Cancel.Location = new System.Drawing.Point(640, 419);
            this.bt_Cancel.Margin = new System.Windows.Forms.Padding(4);
            this.bt_Cancel.Name = "bt_Cancel";
            this.bt_Cancel.Size = new System.Drawing.Size(157, 58);
            this.bt_Cancel.TabIndex = 7;
            this.bt_Cancel.Text = "取消";
            this.bt_Cancel.UseVisualStyleBackColor = true;
            this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
            // 
            // bt_Ok
            // 
            this.bt_Ok.Location = new System.Drawing.Point(143, 419);
            this.bt_Ok.Margin = new System.Windows.Forms.Padding(4);
            this.bt_Ok.Name = "bt_Ok";
            this.bt_Ok.Size = new System.Drawing.Size(157, 58);
            this.bt_Ok.TabIndex = 6;
            this.bt_Ok.Text = "确定";
            this.bt_Ok.UseVisualStyleBackColor = true;
            this.bt_Ok.Click += new System.EventHandler(this.bt_Ok_Click);
            // 
            // tb_BarCode
            // 
            this.tb_BarCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_BarCode.Location = new System.Drawing.Point(54, 128);
            this.tb_BarCode.Margin = new System.Windows.Forms.Padding(4);
            this.tb_BarCode.Name = "tb_BarCode";
            this.tb_BarCode.Size = new System.Drawing.Size(370, 26);
            this.tb_BarCode.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(50, 47);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "请扫描或输入标签号：";
            // 
            // LstB_Barcode
            // 
            this.LstB_Barcode.FormattingEnabled = true;
            this.LstB_Barcode.ItemHeight = 16;
            this.LstB_Barcode.Location = new System.Drawing.Point(532, 71);
            this.LstB_Barcode.Name = "LstB_Barcode";
            this.LstB_Barcode.Size = new System.Drawing.Size(342, 308);
            this.LstB_Barcode.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(529, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "记录更新图像码列表";
            // 
            // UpdateScanForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 499);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.LstB_Barcode);
            this.Controls.Add(this.bt_Cancel);
            this.Controls.Add(this.bt_Ok);
            this.Controls.Add(this.tb_BarCode);
            this.Controls.Add(this.label1);
            this.Name = "UpdateScanForm";
            this.Text = "UpdateScanForm";
            this.Activated += new System.EventHandler(this.UpdateScanForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateScanForm_FormClosing);
            this.Load += new System.EventHandler(this.UpdateScanForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_Cancel;
        private System.Windows.Forms.Button bt_Ok;
        private System.Windows.Forms.TextBox tb_BarCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox LstB_Barcode;
        private System.Windows.Forms.Label label2;
    }
}