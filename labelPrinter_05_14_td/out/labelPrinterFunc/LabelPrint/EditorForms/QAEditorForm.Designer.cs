namespace LabelPrint.EditorForms
{
    partial class QAEditorForm
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
            this.label7 = new System.Windows.Forms.Label();
            this.tb_BatchNo = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.tb_ProductName = new System.Windows.Forms.TextBox();
            this.cb_CustomerCodes = new System.Windows.Forms.ComboBox();
            this.tb_Time = new System.Windows.Forms.TextBox();
            this.cb_WorkClass = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cb_WorkTime = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_Date = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_ProductCode = new System.Windows.Forms.ComboBox();
            this.tb_CustomerName = new System.Windows.Forms.TextBox();
            this.cb_ProductQuality = new System.Windows.Forms.ComboBox();
            this.lb_ProductQulity = new System.Windows.Forms.Label();
            this.cb_ProductState = new System.Windows.Forms.ComboBox();
            this.tb_LittleRollNo = new System.Windows.Forms.TextBox();
            this.tb_BigRollNo = new System.Windows.Forms.TextBox();
            this.tb_Desc = new System.Windows.Forms.TextBox();
            this.tb_RecipeCode = new System.Windows.Forms.TextBox();
            this.tb_worker = new System.Windows.Forms.TextBox();
            this.tb_ManHour = new System.Windows.Forms.TextBox();
            this.tb_WorkNo = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bt_Save
            // 
            this.bt_Save.Location = new System.Drawing.Point(781, 611);
            this.bt_Save.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(74, 36);
            this.bt_Save.TabIndex = 189;
            this.bt_Save.Text = "确定";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // bt_Cancel
            // 
            this.bt_Cancel.Location = new System.Drawing.Point(641, 611);
            this.bt_Cancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bt_Cancel.Name = "bt_Cancel";
            this.bt_Cancel.Size = new System.Drawing.Size(74, 36);
            this.bt_Cancel.TabIndex = 188;
            this.bt_Cancel.Text = "取消";
            this.bt_Cancel.UseVisualStyleBackColor = true;
            this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
            // 
            // bt_Printing
            // 
            this.bt_Printing.Location = new System.Drawing.Point(501, 611);
            this.bt_Printing.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bt_Printing.Name = "bt_Printing";
            this.bt_Printing.Size = new System.Drawing.Size(74, 36);
            this.bt_Printing.TabIndex = 187;
            this.bt_Printing.Text = "打印";
            this.bt_Printing.UseVisualStyleBackColor = true;
            this.bt_Printing.Click += new System.EventHandler(this.bt_Printing_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(34, 11);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 17);
            this.label7.TabIndex = 266;
            this.label7.Text = "基础信息";
            // 
            // tb_BatchNo
            // 
            this.tb_BatchNo.Location = new System.Drawing.Point(778, 60);
            this.tb_BatchNo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_BatchNo.Name = "tb_BatchNo";
            this.tb_BatchNo.Size = new System.Drawing.Size(91, 20);
            this.tb_BatchNo.TabIndex = 207;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(715, 63);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(61, 13);
            this.label27.TabIndex = 263;
            this.label27.Text = "生产批号：";
            // 
            // tb_ProductName
            // 
            this.tb_ProductName.Location = new System.Drawing.Point(237, 102);
            this.tb_ProductName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_ProductName.Name = "tb_ProductName";
            this.tb_ProductName.Size = new System.Drawing.Size(91, 20);
            this.tb_ProductName.TabIndex = 210;
            // 
            // cb_CustomerCodes
            // 
            this.cb_CustomerCodes.FormattingEnabled = true;
            this.cb_CustomerCodes.Location = new System.Drawing.Point(115, 140);
            this.cb_CustomerCodes.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cb_CustomerCodes.Name = "cb_CustomerCodes";
            this.cb_CustomerCodes.Size = new System.Drawing.Size(100, 21);
            this.cb_CustomerCodes.TabIndex = 212;
            this.cb_CustomerCodes.SelectionChangeCommitted += new System.EventHandler(this.cb_CustomerCodes_SelectionChangeCommitted);
            // 
            // tb_Time
            // 
            this.tb_Time.Location = new System.Drawing.Point(237, 63);
            this.tb_Time.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_Time.Name = "tb_Time";
            this.tb_Time.Size = new System.Drawing.Size(91, 20);
            this.tb_Time.TabIndex = 206;
            // 
            // cb_WorkClass
            // 
            this.cb_WorkClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_WorkClass.FormattingEnabled = true;
            this.cb_WorkClass.Location = new System.Drawing.Point(743, 527);
            this.cb_WorkClass.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cb_WorkClass.Name = "cb_WorkClass";
            this.cb_WorkClass.Size = new System.Drawing.Size(112, 21);
            this.cb_WorkClass.TabIndex = 236;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(638, 530);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 262;
            this.label6.Text = "班     別";
            // 
            // cb_WorkTime
            // 
            this.cb_WorkTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_WorkTime.FormattingEnabled = true;
            this.cb_WorkTime.Location = new System.Drawing.Point(742, 494);
            this.cb_WorkTime.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cb_WorkTime.Name = "cb_WorkTime";
            this.cb_WorkTime.Size = new System.Drawing.Size(112, 21);
            this.cb_WorkTime.TabIndex = 233;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(638, 494);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 261;
            this.label5.Text = "班     次";
            // 
            // tb_Date
            // 
            this.tb_Date.Location = new System.Drawing.Point(115, 62);
            this.tb_Date.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_Date.Name = "tb_Date";
            this.tb_Date.Size = new System.Drawing.Size(100, 20);
            this.tb_Date.TabIndex = 205;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 63);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 260;
            this.label3.Text = "生产日期";
            // 
            // cb_ProductCode
            // 
            this.cb_ProductCode.FormattingEnabled = true;
            this.cb_ProductCode.Location = new System.Drawing.Point(115, 100);
            this.cb_ProductCode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cb_ProductCode.Name = "cb_ProductCode";
            this.cb_ProductCode.Size = new System.Drawing.Size(100, 21);
            this.cb_ProductCode.TabIndex = 209;
            this.cb_ProductCode.SelectionChangeCommitted += new System.EventHandler(this.cb_ProductCode_SelectionChangeCommitted);
            // 
            // tb_CustomerName
            // 
            this.tb_CustomerName.Location = new System.Drawing.Point(237, 142);
            this.tb_CustomerName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_CustomerName.Name = "tb_CustomerName";
            this.tb_CustomerName.Size = new System.Drawing.Size(91, 20);
            this.tb_CustomerName.TabIndex = 213;
            // 
            // cb_ProductQuality
            // 
            this.cb_ProductQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductQuality.FormattingEnabled = true;
            this.cb_ProductQuality.Location = new System.Drawing.Point(702, 390);
            this.cb_ProductQuality.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cb_ProductQuality.Name = "cb_ProductQuality";
            this.cb_ProductQuality.Size = new System.Drawing.Size(152, 21);
            this.cb_ProductQuality.TabIndex = 228;
            // 
            // lb_ProductQulity
            // 
            this.lb_ProductQulity.AutoSize = true;
            this.lb_ProductQulity.Location = new System.Drawing.Point(644, 390);
            this.lb_ProductQulity.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lb_ProductQulity.Name = "lb_ProductQulity";
            this.lb_ProductQulity.Size = new System.Drawing.Size(55, 13);
            this.lb_ProductQulity.TabIndex = 255;
            this.lb_ProductQulity.Text = "产品质量";
            // 
            // cb_ProductState
            // 
            this.cb_ProductState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductState.FormattingEnabled = true;
            this.cb_ProductState.Location = new System.Drawing.Point(94, 390);
            this.cb_ProductState.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cb_ProductState.Name = "cb_ProductState";
            this.cb_ProductState.Size = new System.Drawing.Size(152, 21);
            this.cb_ProductState.TabIndex = 227;
            this.cb_ProductState.SelectedValueChanged += new System.EventHandler(this.cb_ProductState_SelectedValueChanged);
            // 
            // tb_LittleRollNo
            // 
            this.tb_LittleRollNo.Location = new System.Drawing.Point(343, 523);
            this.tb_LittleRollNo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_LittleRollNo.Name = "tb_LittleRollNo";
            this.tb_LittleRollNo.Size = new System.Drawing.Size(152, 20);
            this.tb_LittleRollNo.TabIndex = 232;
            // 
            // tb_BigRollNo
            // 
            this.tb_BigRollNo.Location = new System.Drawing.Point(98, 525);
            this.tb_BigRollNo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_BigRollNo.Name = "tb_BigRollNo";
            this.tb_BigRollNo.Size = new System.Drawing.Size(152, 20);
            this.tb_BigRollNo.TabIndex = 231;
            // 
            // tb_Desc
            // 
            this.tb_Desc.Location = new System.Drawing.Point(94, 424);
            this.tb_Desc.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_Desc.Multiline = true;
            this.tb_Desc.Name = "tb_Desc";
            this.tb_Desc.Size = new System.Drawing.Size(760, 54);
            this.tb_Desc.TabIndex = 230;
            // 
            // tb_RecipeCode
            // 
            this.tb_RecipeCode.Location = new System.Drawing.Point(778, 141);
            this.tb_RecipeCode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_RecipeCode.Name = "tb_RecipeCode";
            this.tb_RecipeCode.Size = new System.Drawing.Size(91, 20);
            this.tb_RecipeCode.TabIndex = 215;
            // 
            // tb_worker
            // 
            this.tb_worker.Location = new System.Drawing.Point(702, 355);
            this.tb_worker.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_worker.Name = "tb_worker";
            this.tb_worker.Size = new System.Drawing.Size(152, 20);
            this.tb_worker.TabIndex = 225;
            // 
            // tb_ManHour
            // 
            this.tb_ManHour.Location = new System.Drawing.Point(94, 355);
            this.tb_ManHour.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_ManHour.Name = "tb_ManHour";
            this.tb_ManHour.Size = new System.Drawing.Size(152, 20);
            this.tb_ManHour.TabIndex = 224;
            this.tb_ManHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tb_WorkNo
            // 
            this.tb_WorkNo.Location = new System.Drawing.Point(94, 318);
            this.tb_WorkNo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_WorkNo.Name = "tb_WorkNo";
            this.tb_WorkNo.Size = new System.Drawing.Size(152, 20);
            this.tb_WorkNo.TabIndex = 222;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(40, 147);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(61, 13);
            this.label26.TabIndex = 254;
            this.label26.Text = "客户名称：";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(28, 293);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(68, 17);
            this.label25.TabIndex = 253;
            this.label25.Text = "生产信息";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(717, 141);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(49, 13);
            this.label22.TabIndex = 251;
            this.label22.Text = "配方号：";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(643, 355);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(55, 13);
            this.label19.TabIndex = 248;
            this.label19.Text = "员工编号";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(28, 424);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(31, 13);
            this.label18.TabIndex = 247;
            this.label18.Text = "备注";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(28, 390);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(55, 13);
            this.label17.TabIndex = 246;
            this.label17.Text = "产品状态";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(28, 355);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 13);
            this.label16.TabIndex = 245;
            this.label16.Text = "生产工时";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(26, 320);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 13);
            this.label15.TabIndex = 244;
            this.label15.Text = "工单号";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(32, 525);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 13);
            this.label13.TabIndex = 242;
            this.label13.Text = "大卷编号";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(269, 525);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 13);
            this.label12.TabIndex = 241;
            this.label12.Text = "小卷编号";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 103);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 237;
            this.label1.Text = "产品代号";
            // 
            // QAEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 667);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tb_BatchNo);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.tb_ProductName);
            this.Controls.Add(this.cb_CustomerCodes);
            this.Controls.Add(this.tb_Time);
            this.Controls.Add(this.cb_WorkClass);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cb_WorkTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tb_Date);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cb_ProductCode);
            this.Controls.Add(this.tb_CustomerName);
            this.Controls.Add(this.cb_ProductQuality);
            this.Controls.Add(this.lb_ProductQulity);
            this.Controls.Add(this.cb_ProductState);
            this.Controls.Add(this.tb_LittleRollNo);
            this.Controls.Add(this.tb_BigRollNo);
            this.Controls.Add(this.tb_Desc);
            this.Controls.Add(this.tb_RecipeCode);
            this.Controls.Add(this.tb_worker);
            this.Controls.Add(this.tb_ManHour);
            this.Controls.Add(this.tb_WorkNo);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.bt_Cancel);
            this.Controls.Add(this.bt_Printing);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "QAEditorForm";
            this.Text = "质检单修改";
            this.Load += new System.EventHandler(this.QAEditorForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_Save;
        private System.Windows.Forms.Button bt_Cancel;
        private System.Windows.Forms.Button bt_Printing;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_BatchNo;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox tb_ProductName;
        private System.Windows.Forms.ComboBox cb_CustomerCodes;
        private System.Windows.Forms.TextBox tb_Time;
        private System.Windows.Forms.ComboBox cb_WorkClass;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cb_WorkTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_Date;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cb_ProductCode;
        private System.Windows.Forms.TextBox tb_CustomerName;
        private System.Windows.Forms.ComboBox cb_ProductQuality;
        private System.Windows.Forms.Label lb_ProductQulity;
        private System.Windows.Forms.ComboBox cb_ProductState;
        private System.Windows.Forms.TextBox tb_LittleRollNo;
        private System.Windows.Forms.TextBox tb_BigRollNo;
        private System.Windows.Forms.TextBox tb_Desc;
        private System.Windows.Forms.TextBox tb_RecipeCode;
        private System.Windows.Forms.TextBox tb_worker;
        private System.Windows.Forms.TextBox tb_ManHour;
        private System.Windows.Forms.TextBox tb_WorkNo;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
    }
}