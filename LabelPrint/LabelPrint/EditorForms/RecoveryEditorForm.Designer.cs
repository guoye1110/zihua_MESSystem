﻿namespace LabelPrint.EditorForms
{
    partial class RecoveryEditorForm
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
            this.bt_Save = new System.Windows.Forms.Button();
            this.bt_Cancel = new System.Windows.Forms.Button();
            this.bt_Printing = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tb_Desc = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tb_WorkerNo = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tb_TMaterialWeight = new System.Windows.Forms.TextBox();
            this.tb_OldCode1 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tb_OldCode10 = new System.Windows.Forms.TextBox();
            this.tb_RecoveryWeight = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_OldCode9 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tb_OldCode8 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tb_OldCode7 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tb_OldCode6 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tb_OldCode5 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_OldCode4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_OldCode3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_OldCode2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tb_Color = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tb_RecipeNo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tb_RecoveryMachineNo = new System.Windows.Forms.TextBox();
            this.tb_Time = new System.Windows.Forms.TextBox();
            this.tb_Date = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // bt_Save
            // 
            this.bt_Save.Location = new System.Drawing.Point(954, 608);
            this.bt_Save.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(142, 31);
            this.bt_Save.TabIndex = 189;
            this.bt_Save.Text = "确定";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // bt_Cancel
            // 
            this.bt_Cancel.Location = new System.Drawing.Point(711, 608);
            this.bt_Cancel.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Cancel.Name = "bt_Cancel";
            this.bt_Cancel.Size = new System.Drawing.Size(142, 31);
            this.bt_Cancel.TabIndex = 188;
            this.bt_Cancel.Text = "取消";
            this.bt_Cancel.UseVisualStyleBackColor = true;
            this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
            // 
            // bt_Printing
            // 
            this.bt_Printing.Location = new System.Drawing.Point(477, 608);
            this.bt_Printing.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Printing.Name = "bt_Printing";
            this.bt_Printing.Size = new System.Drawing.Size(142, 31);
            this.bt_Printing.TabIndex = 187;
            this.bt_Printing.Text = "打印";
            this.bt_Printing.UseVisualStyleBackColor = true;
            this.bt_Printing.Click += new System.EventHandler(this.bt_Printing_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(510, 445);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(531, 138);
            this.groupBox4.TabIndex = 193;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "标签信息";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tb_Time);
            this.groupBox3.Controls.Add(this.tb_Date);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.tb_Desc);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.tb_WorkerNo);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Location = new System.Drawing.Point(35, 419);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(438, 199);
            this.groupBox3.TabIndex = 192;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "生产信息";
            // 
            // tb_Desc
            // 
            this.tb_Desc.Location = new System.Drawing.Point(51, 93);
            this.tb_Desc.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Desc.Multiline = true;
            this.tb_Desc.Name = "tb_Desc";
            this.tb_Desc.Size = new System.Drawing.Size(348, 102);
            this.tb_Desc.TabIndex = 4;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(14, 93);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(37, 13);
            this.label22.TabIndex = 8;
            this.label22.Text = "备注：";
            // 
            // tb_WorkerNo
            // 
            this.tb_WorkerNo.Location = new System.Drawing.Point(89, 26);
            this.tb_WorkerNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_WorkerNo.Name = "tb_WorkerNo";
            this.tb_WorkerNo.Size = new System.Drawing.Size(138, 20);
            this.tb_WorkerNo.TabIndex = 6;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(14, 29);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(61, 13);
            this.label18.TabIndex = 5;
            this.label18.Text = "员工编号：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_TMaterialWeight);
            this.groupBox1.Controls.Add(this.tb_OldCode1);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.tb_OldCode10);
            this.groupBox1.Controls.Add(this.tb_RecoveryWeight);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tb_OldCode9);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.tb_OldCode8);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.tb_OldCode7);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tb_OldCode6);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tb_OldCode5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tb_OldCode4);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tb_OldCode3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tb_OldCode2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.tb_Color);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.tb_RecipeNo);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Location = new System.Drawing.Point(35, 76);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(1006, 311);
            this.groupBox1.TabIndex = 194;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "再造料来源";
            // 
            // tb_TMaterialWeight
            // 
            this.tb_TMaterialWeight.Location = new System.Drawing.Point(704, 81);
            this.tb_TMaterialWeight.Name = "tb_TMaterialWeight";
            this.tb_TMaterialWeight.Size = new System.Drawing.Size(132, 20);
            this.tb_TMaterialWeight.TabIndex = 232;
            // 
            // tb_OldCode1
            // 
            this.tb_OldCode1.Location = new System.Drawing.Point(99, 25);
            this.tb_OldCode1.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OldCode1.Name = "tb_OldCode1";
            this.tb_OldCode1.Size = new System.Drawing.Size(234, 20);
            this.tb_OldCode1.TabIndex = 38;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(611, 81);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(73, 13);
            this.label15.TabIndex = 231;
            this.label15.Text = "原料总重量：";
            // 
            // tb_OldCode10
            // 
            this.tb_OldCode10.Location = new System.Drawing.Point(99, 276);
            this.tb_OldCode10.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OldCode10.Name = "tb_OldCode10";
            this.tb_OldCode10.Size = new System.Drawing.Size(234, 20);
            this.tb_OldCode10.TabIndex = 56;
            // 
            // tb_RecoveryWeight
            // 
            this.tb_RecoveryWeight.Location = new System.Drawing.Point(704, 199);
            this.tb_RecoveryWeight.Name = "tb_RecoveryWeight";
            this.tb_RecoveryWeight.Size = new System.Drawing.Size(132, 20);
            this.tb_RecoveryWeight.TabIndex = 230;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 280);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 13);
            this.label10.TabIndex = 55;
            this.label10.Text = "原始标签10：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(598, 202);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 229;
            this.label3.Text = "再造料总重量：";
            // 
            // tb_OldCode9
            // 
            this.tb_OldCode9.Location = new System.Drawing.Point(99, 246);
            this.tb_OldCode9.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OldCode9.Name = "tb_OldCode9";
            this.tb_OldCode9.Size = new System.Drawing.Size(234, 20);
            this.tb_OldCode9.TabIndex = 54;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(26, 249);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 13);
            this.label12.TabIndex = 53;
            this.label12.Text = "原始标签9：";
            // 
            // tb_OldCode8
            // 
            this.tb_OldCode8.Location = new System.Drawing.Point(99, 219);
            this.tb_OldCode8.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OldCode8.Name = "tb_OldCode8";
            this.tb_OldCode8.Size = new System.Drawing.Size(234, 20);
            this.tb_OldCode8.TabIndex = 52;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(26, 222);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 13);
            this.label13.TabIndex = 51;
            this.label13.Text = "原始标签8：";
            // 
            // tb_OldCode7
            // 
            this.tb_OldCode7.Location = new System.Drawing.Point(99, 191);
            this.tb_OldCode7.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OldCode7.Name = "tb_OldCode7";
            this.tb_OldCode7.Size = new System.Drawing.Size(234, 20);
            this.tb_OldCode7.TabIndex = 50;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(26, 194);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 49;
            this.label9.Text = "原始标签7：";
            // 
            // tb_OldCode6
            // 
            this.tb_OldCode6.Location = new System.Drawing.Point(99, 161);
            this.tb_OldCode6.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OldCode6.Name = "tb_OldCode6";
            this.tb_OldCode6.Size = new System.Drawing.Size(234, 20);
            this.tb_OldCode6.TabIndex = 48;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(26, 164);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 47;
            this.label8.Text = "原始标签6：";
            // 
            // tb_OldCode5
            // 
            this.tb_OldCode5.Location = new System.Drawing.Point(99, 134);
            this.tb_OldCode5.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OldCode5.Name = "tb_OldCode5";
            this.tb_OldCode5.Size = new System.Drawing.Size(234, 20);
            this.tb_OldCode5.TabIndex = 46;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 137);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 45;
            this.label6.Text = "原始标签5：";
            // 
            // tb_OldCode4
            // 
            this.tb_OldCode4.Location = new System.Drawing.Point(99, 105);
            this.tb_OldCode4.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OldCode4.Name = "tb_OldCode4";
            this.tb_OldCode4.Size = new System.Drawing.Size(234, 20);
            this.tb_OldCode4.TabIndex = 44;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 108);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 43;
            this.label4.Text = "原始标签4：";
            // 
            // tb_OldCode3
            // 
            this.tb_OldCode3.Location = new System.Drawing.Point(99, 79);
            this.tb_OldCode3.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OldCode3.Name = "tb_OldCode3";
            this.tb_OldCode3.Size = new System.Drawing.Size(234, 20);
            this.tb_OldCode3.TabIndex = 42;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 82);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "原始标签3：";
            // 
            // tb_OldCode2
            // 
            this.tb_OldCode2.Location = new System.Drawing.Point(99, 52);
            this.tb_OldCode2.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OldCode2.Name = "tb_OldCode2";
            this.tb_OldCode2.Size = new System.Drawing.Size(234, 20);
            this.tb_OldCode2.TabIndex = 40;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 39;
            this.label1.Text = "原始标签2：";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(26, 27);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(67, 13);
            this.label14.TabIndex = 37;
            this.label14.Text = "原始标签1：";
            // 
            // tb_Color
            // 
            this.tb_Color.Location = new System.Drawing.Point(704, 157);
            this.tb_Color.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Color.Name = "tb_Color";
            this.tb_Color.Size = new System.Drawing.Size(132, 20);
            this.tb_Color.TabIndex = 18;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(649, 164);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "颜色:";
            // 
            // tb_RecipeNo
            // 
            this.tb_RecipeNo.Location = new System.Drawing.Point(704, 116);
            this.tb_RecipeNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_RecipeNo.Name = "tb_RecipeNo";
            this.tb_RecipeNo.Size = new System.Drawing.Size(132, 20);
            this.tb_RecipeNo.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(638, 120);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "配方号:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tb_RecoveryMachineNo);
            this.groupBox2.Location = new System.Drawing.Point(35, 11);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(109, 48);
            this.groupBox2.TabIndex = 228;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "再造料机号";
            // 
            // tb_RecoveryMachineNo
            // 
            this.tb_RecoveryMachineNo.Location = new System.Drawing.Point(13, 17);
            this.tb_RecoveryMachineNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_RecoveryMachineNo.Name = "tb_RecoveryMachineNo";
            this.tb_RecoveryMachineNo.Size = new System.Drawing.Size(76, 20);
            this.tb_RecoveryMachineNo.TabIndex = 57;
            // 
            // tb_Time
            // 
            this.tb_Time.Location = new System.Drawing.Point(273, 60);
            this.tb_Time.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Time.Name = "tb_Time";
            this.tb_Time.Size = new System.Drawing.Size(126, 20);
            this.tb_Time.TabIndex = 327;
            // 
            // tb_Date
            // 
            this.tb_Date.Location = new System.Drawing.Point(89, 60);
            this.tb_Date.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Date.Name = "tb_Date";
            this.tb_Date.Size = new System.Drawing.Size(138, 20);
            this.tb_Date.TabIndex = 326;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 60);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 328;
            this.label5.Text = "生产日期";
            // 
            // RecoveryEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1245, 670);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.bt_Cancel);
            this.Controls.Add(this.bt_Printing);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RecoveryEditorForm";
            this.Text = "再造料单修改";
            this.Load += new System.EventHandler(this.RecoveryEditorForm_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bt_Save;
        private System.Windows.Forms.Button bt_Cancel;
        private System.Windows.Forms.Button bt_Printing;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tb_Desc;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox tb_WorkerNo;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_OldCode1;
        private System.Windows.Forms.TextBox tb_OldCode10;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tb_OldCode9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tb_OldCode8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tb_OldCode7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tb_OldCode6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tb_OldCode5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_OldCode4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_OldCode3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_OldCode2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tb_Color;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tb_RecipeNo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tb_RecoveryMachineNo;
        private System.Windows.Forms.TextBox tb_TMaterialWeight;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tb_RecoveryWeight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_Time;
        private System.Windows.Forms.TextBox tb_Date;
        private System.Windows.Forms.Label label5;
    }
}