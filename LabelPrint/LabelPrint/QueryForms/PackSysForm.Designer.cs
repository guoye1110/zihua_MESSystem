﻿namespace LabelPrint.QueryForms
{
    partial class PackSysForm
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
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bt_Print = new System.Windows.Forms.Button();
            this.bt_Find = new System.Windows.Forms.Button();
            this.bt_New = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tb_Customer = new System.Windows.Forms.TextBox();
            this.tb_ProductCode = new System.Windows.Forms.TextBox();
            this.tb_LittleRollNo = new System.Windows.Forms.TextBox();
            this.tb_Batchno = new System.Windows.Forms.TextBox();
            this.tb_BigRollNo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "";
            this.dateTimePicker2.Location = new System.Drawing.Point(307, 70);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(136, 22);
            this.dateTimePicker2.TabIndex = 48;
            this.dateTimePicker2.Value = new System.DateTime(2018, 1, 14, 0, 0, 0, 0);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "";
            this.dateTimePicker1.Location = new System.Drawing.Point(148, 70);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(153, 22);
            this.dateTimePicker1.TabIndex = 35;
            this.dateTimePicker1.Value = new System.DateTime(2018, 1, 14, 0, 0, 0, 0);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bt_Print);
            this.groupBox1.Controls.Add(this.bt_Find);
            this.groupBox1.Controls.Add(this.bt_New);
            this.groupBox1.Location = new System.Drawing.Point(9, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 60);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            // 
            // bt_Print
            // 
            this.bt_Print.Location = new System.Drawing.Point(216, 14);
            this.bt_Print.Name = "bt_Print";
            this.bt_Print.Size = new System.Drawing.Size(75, 40);
            this.bt_Print.TabIndex = 2;
            this.bt_Print.Text = "打印";
            this.bt_Print.UseVisualStyleBackColor = true;
            this.bt_Print.Click += new System.EventHandler(this.bt_Print_Click);
            // 
            // bt_Find
            // 
            this.bt_Find.Location = new System.Drawing.Point(114, 14);
            this.bt_Find.Name = "bt_Find";
            this.bt_Find.Size = new System.Drawing.Size(75, 40);
            this.bt_Find.TabIndex = 1;
            this.bt_Find.Text = "查找";
            this.bt_Find.UseVisualStyleBackColor = true;
            this.bt_Find.Click += new System.EventHandler(this.bt_Find_Click);
            // 
            // bt_New
            // 
            this.bt_New.Location = new System.Drawing.Point(12, 14);
            this.bt_New.Name = "bt_New";
            this.bt_New.Size = new System.Drawing.Size(75, 40);
            this.bt_New.TabIndex = 0;
            this.bt_New.Text = "新增";
            this.bt_New.UseVisualStyleBackColor = true;
            this.bt_New.Click += new System.EventHandler(this.bt_New_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(21, 168);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1460, 514);
            this.dataGridView1.TabIndex = 46;
            this.dataGridView1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick);
            // 
            // tb_Customer
            // 
            this.tb_Customer.Location = new System.Drawing.Point(902, 113);
            this.tb_Customer.Name = "tb_Customer";
            this.tb_Customer.Size = new System.Drawing.Size(295, 22);
            this.tb_Customer.TabIndex = 45;
            // 
            // tb_ProductCode
            // 
            this.tb_ProductCode.Location = new System.Drawing.Point(902, 79);
            this.tb_ProductCode.Name = "tb_ProductCode";
            this.tb_ProductCode.Size = new System.Drawing.Size(295, 22);
            this.tb_ProductCode.TabIndex = 44;
            // 
            // tb_LittleRollNo
            // 
            this.tb_LittleRollNo.Location = new System.Drawing.Point(531, 118);
            this.tb_LittleRollNo.Name = "tb_LittleRollNo";
            this.tb_LittleRollNo.Size = new System.Drawing.Size(295, 22);
            this.tb_LittleRollNo.TabIndex = 43;
            // 
            // tb_Batchno
            // 
            this.tb_Batchno.Location = new System.Drawing.Point(531, 77);
            this.tb_Batchno.Name = "tb_Batchno";
            this.tb_Batchno.Size = new System.Drawing.Size(295, 22);
            this.tb_Batchno.TabIndex = 42;
            // 
            // tb_BigRollNo
            // 
            this.tb_BigRollNo.Location = new System.Drawing.Point(148, 118);
            this.tb_BigRollNo.Name = "tb_BigRollNo";
            this.tb_BigRollNo.Size = new System.Drawing.Size(295, 22);
            this.tb_BigRollNo.TabIndex = 41;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(832, 118);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 17);
            this.label7.TabIndex = 40;
            this.label7.Text = "客户编号";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(832, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 17);
            this.label6.TabIndex = 39;
            this.label6.Text = "产品代号";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(445, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 17);
            this.label5.TabIndex = 38;
            this.label5.Text = "小卷编号";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(445, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 17);
            this.label4.TabIndex = 37;
            this.label4.Text = "生产批号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 17);
            this.label2.TabIndex = 36;
            this.label2.Text = "大卷编号";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 34;
            this.label1.Text = "日期范围";
            // 
            // PackSysForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1493, 722);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.tb_Customer);
            this.Controls.Add(this.tb_ProductCode);
            this.Controls.Add(this.tb_LittleRollNo);
            this.Controls.Add(this.tb_Batchno);
            this.Controls.Add(this.tb_BigRollNo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PackSysForm";
            this.Text = "打包系统";
            this.Load += new System.EventHandler(this.PackSysForm_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bt_Print;
        private System.Windows.Forms.Button bt_Find;
        private System.Windows.Forms.Button bt_New;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox tb_Customer;
        private System.Windows.Forms.TextBox tb_ProductCode;
        private System.Windows.Forms.TextBox tb_LittleRollNo;
        private System.Windows.Forms.TextBox tb_Batchno;
        private System.Windows.Forms.TextBox tb_BigRollNo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}