namespace LabelPrint.QueryForms
{
    partial class RecoverySysForm
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
            this.tb_WorkerNo = new System.Windows.Forms.TextBox();
            this.tb_Recipe = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "";
            this.dateTimePicker2.Location = new System.Drawing.Point(214, 61);
            this.dateTimePicker2.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(103, 20);
            this.dateTimePicker2.TabIndex = 63;
            this.dateTimePicker2.Value = new System.DateTime(2018, 1, 14, 0, 0, 0, 0);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "";
            this.dateTimePicker1.Location = new System.Drawing.Point(94, 61);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(116, 20);
            this.dateTimePicker1.TabIndex = 50;
            this.dateTimePicker1.Value = new System.DateTime(2018, 1, 14, 0, 0, 0, 0);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bt_Print);
            this.groupBox1.Controls.Add(this.bt_Find);
            this.groupBox1.Controls.Add(this.bt_New);
            this.groupBox1.Location = new System.Drawing.Point(0, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(228, 49);
            this.groupBox1.TabIndex = 62;
            this.groupBox1.TabStop = false;
            // 
            // bt_Print
            // 
            this.bt_Print.Location = new System.Drawing.Point(162, 11);
            this.bt_Print.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Print.Name = "bt_Print";
            this.bt_Print.Size = new System.Drawing.Size(56, 32);
            this.bt_Print.TabIndex = 2;
            this.bt_Print.Text = "打印";
            this.bt_Print.UseVisualStyleBackColor = true;
            this.bt_Print.Click += new System.EventHandler(this.bt_Print_Click);
            // 
            // bt_Find
            // 
            this.bt_Find.Location = new System.Drawing.Point(86, 11);
            this.bt_Find.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Find.Name = "bt_Find";
            this.bt_Find.Size = new System.Drawing.Size(56, 32);
            this.bt_Find.TabIndex = 1;
            this.bt_Find.Text = "查找";
            this.bt_Find.UseVisualStyleBackColor = true;
            this.bt_Find.Click += new System.EventHandler(this.bt_Find_Click);
            // 
            // bt_New
            // 
            this.bt_New.Location = new System.Drawing.Point(9, 11);
            this.bt_New.Margin = new System.Windows.Forms.Padding(2);
            this.bt_New.Name = "bt_New";
            this.bt_New.Size = new System.Drawing.Size(56, 32);
            this.bt_New.TabIndex = 0;
            this.bt_New.Text = "新增";
            this.bt_New.UseVisualStyleBackColor = true;
            this.bt_New.Click += new System.EventHandler(this.bt_New_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(9, 93);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1370, 516);
            this.dataGridView1.TabIndex = 61;
            this.dataGridView1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick);
            // 
            // tb_WorkerNo
            // 
            this.tb_WorkerNo.Location = new System.Drawing.Point(644, 61);
            this.tb_WorkerNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_WorkerNo.Name = "tb_WorkerNo";
            this.tb_WorkerNo.Size = new System.Drawing.Size(222, 20);
            this.tb_WorkerNo.TabIndex = 60;
            // 
            // tb_Recipe
            // 
            this.tb_Recipe.Location = new System.Drawing.Point(363, 61);
            this.tb_Recipe.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Recipe.Name = "tb_Recipe";
            this.tb_Recipe.Size = new System.Drawing.Size(222, 20);
            this.tb_Recipe.TabIndex = 59;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(601, 65);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 55;
            this.label7.Text = "工号";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(332, 65);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 54;
            this.label6.Text = "配方";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "日期范围";
            // 
            // RecoverySysForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1402, 616);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.tb_WorkerNo);
            this.Controls.Add(this.tb_Recipe);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RecoverySysForm";
            this.Text = "再造料系统";
            this.Load += new System.EventHandler(this.RecoverySysForm_Load);
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
        private System.Windows.Forms.TextBox tb_WorkerNo;
        private System.Windows.Forms.TextBox tb_Recipe;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
    }
}