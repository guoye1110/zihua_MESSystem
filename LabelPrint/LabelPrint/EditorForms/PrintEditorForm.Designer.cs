namespace LabelPrint.EditorForms
{
    partial class PrintEditorForm
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
            this.tb_BatchNo1 = new System.Windows.Forms.TextBox();
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
            this.tb_PrintMachineNo = new System.Windows.Forms.TextBox();
            this.tb_Width = new System.Windows.Forms.TextBox();
            this.tb_CustomerName = new System.Windows.Forms.TextBox();
            this.tb_BigRollNo = new System.Windows.Forms.TextBox();
            this.tb_Desc = new System.Windows.Forms.TextBox();
            this.tb_RecipeCode = new System.Windows.Forms.TextBox();
            this.tb_worker = new System.Windows.Forms.TextBox();
            this.tb_ManHour = new System.Windows.Forms.TextBox();
            this.tb_WorkNo = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_ProductState = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_RollWeight = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cb_ProductQuality = new System.Windows.Forms.ComboBox();
            this.lb_ProductQulity = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bt_Save
            // 
            this.bt_Save.Location = new System.Drawing.Point(721, 599);
            this.bt_Save.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(107, 31);
            this.bt_Save.TabIndex = 189;
            this.bt_Save.Text = "确定";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // bt_Cancel
            // 
            this.bt_Cancel.Location = new System.Drawing.Point(540, 599);
            this.bt_Cancel.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Cancel.Name = "bt_Cancel";
            this.bt_Cancel.Size = new System.Drawing.Size(107, 31);
            this.bt_Cancel.TabIndex = 188;
            this.bt_Cancel.Text = "取消";
            this.bt_Cancel.UseVisualStyleBackColor = true;
            this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
            // 
            // bt_Printing
            // 
            this.bt_Printing.Location = new System.Drawing.Point(359, 599);
            this.bt_Printing.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Printing.Name = "bt_Printing";
            this.bt_Printing.Size = new System.Drawing.Size(107, 31);
            this.bt_Printing.TabIndex = 187;
            this.bt_Printing.Text = "打印";
            this.bt_Printing.UseVisualStyleBackColor = true;
            this.bt_Printing.Click += new System.EventHandler(this.bt_Printing_Click);
            // 
            // tb_BatchNo1
            // 
            this.tb_BatchNo1.Location = new System.Drawing.Point(712, 45);
            this.tb_BatchNo1.Margin = new System.Windows.Forms.Padding(2);
            this.tb_BatchNo1.Name = "tb_BatchNo1";
            this.tb_BatchNo1.Size = new System.Drawing.Size(122, 20);
            this.tb_BatchNo1.TabIndex = 269;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(610, 52);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(61, 13);
            this.label27.TabIndex = 325;
            this.label27.Text = "生产批号：";
            // 
            // tb_ProductName
            // 
            this.tb_ProductName.Location = new System.Drawing.Point(274, 87);
            this.tb_ProductName.Margin = new System.Windows.Forms.Padding(2);
            this.tb_ProductName.Name = "tb_ProductName";
            this.tb_ProductName.Size = new System.Drawing.Size(122, 20);
            this.tb_ProductName.TabIndex = 272;
            // 
            // cb_CustomerCodes
            // 
            this.cb_CustomerCodes.FormattingEnabled = true;
            this.cb_CustomerCodes.Location = new System.Drawing.Point(112, 123);
            this.cb_CustomerCodes.Margin = new System.Windows.Forms.Padding(2);
            this.cb_CustomerCodes.Name = "cb_CustomerCodes";
            this.cb_CustomerCodes.Size = new System.Drawing.Size(122, 21);
            this.cb_CustomerCodes.TabIndex = 274;
            this.cb_CustomerCodes.SelectionChangeCommitted += new System.EventHandler(this.cb_CustomerCodes_SelectionChangeCommitted);
            // 
            // tb_Time
            // 
            this.tb_Time.Location = new System.Drawing.Point(274, 49);
            this.tb_Time.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Time.Name = "tb_Time";
            this.tb_Time.Size = new System.Drawing.Size(122, 20);
            this.tb_Time.TabIndex = 268;
            // 
            // cb_WorkClass
            // 
            this.cb_WorkClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_WorkClass.FormattingEnabled = true;
            this.cb_WorkClass.Location = new System.Drawing.Point(678, 531);
            this.cb_WorkClass.Margin = new System.Windows.Forms.Padding(2);
            this.cb_WorkClass.Name = "cb_WorkClass";
            this.cb_WorkClass.Size = new System.Drawing.Size(112, 21);
            this.cb_WorkClass.TabIndex = 298;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(573, 533);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 324;
            this.label6.Text = "班     別";
            // 
            // cb_WorkTime
            // 
            this.cb_WorkTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_WorkTime.FormattingEnabled = true;
            this.cb_WorkTime.Location = new System.Drawing.Point(676, 497);
            this.cb_WorkTime.Margin = new System.Windows.Forms.Padding(2);
            this.cb_WorkTime.Name = "cb_WorkTime";
            this.cb_WorkTime.Size = new System.Drawing.Size(112, 21);
            this.cb_WorkTime.TabIndex = 295;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(573, 497);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 323;
            this.label5.Text = "班     次";
            // 
            // tb_Date
            // 
            this.tb_Date.Location = new System.Drawing.Point(113, 48);
            this.tb_Date.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Date.Name = "tb_Date";
            this.tb_Date.Size = new System.Drawing.Size(122, 20);
            this.tb_Date.TabIndex = 267;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(47, 52);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 322;
            this.label3.Text = "生产日期";
            // 
            // cb_ProductCode
            // 
            this.cb_ProductCode.FormattingEnabled = true;
            this.cb_ProductCode.Location = new System.Drawing.Point(112, 84);
            this.cb_ProductCode.Margin = new System.Windows.Forms.Padding(2);
            this.cb_ProductCode.Name = "cb_ProductCode";
            this.cb_ProductCode.Size = new System.Drawing.Size(122, 21);
            this.cb_ProductCode.TabIndex = 271;
            this.cb_ProductCode.SelectionChangeCommitted += new System.EventHandler(this.cb_ProductCode_SelectionChangeCommitted);
            // 
            // tb_PrintMachineNo
            // 
            this.tb_PrintMachineNo.Location = new System.Drawing.Point(630, 364);
            this.tb_PrintMachineNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_PrintMachineNo.Name = "tb_PrintMachineNo";
            this.tb_PrintMachineNo.Size = new System.Drawing.Size(158, 20);
            this.tb_PrintMachineNo.TabIndex = 285;
            // 
            // tb_Width
            // 
            this.tb_Width.Location = new System.Drawing.Point(712, 83);
            this.tb_Width.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Width.Name = "tb_Width";
            this.tb_Width.Size = new System.Drawing.Size(122, 20);
            this.tb_Width.TabIndex = 273;
            // 
            // tb_CustomerName
            // 
            this.tb_CustomerName.Location = new System.Drawing.Point(274, 124);
            this.tb_CustomerName.Margin = new System.Windows.Forms.Padding(2);
            this.tb_CustomerName.Name = "tb_CustomerName";
            this.tb_CustomerName.Size = new System.Drawing.Size(122, 20);
            this.tb_CustomerName.TabIndex = 275;
            // 
            // tb_BigRollNo
            // 
            this.tb_BigRollNo.Location = new System.Drawing.Point(104, 497);
            this.tb_BigRollNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_BigRollNo.Name = "tb_BigRollNo";
            this.tb_BigRollNo.Size = new System.Drawing.Size(152, 20);
            this.tb_BigRollNo.TabIndex = 293;
            // 
            // tb_Desc
            // 
            this.tb_Desc.Location = new System.Drawing.Point(106, 436);
            this.tb_Desc.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Desc.Multiline = true;
            this.tb_Desc.Name = "tb_Desc";
            this.tb_Desc.Size = new System.Drawing.Size(682, 54);
            this.tb_Desc.TabIndex = 292;
            // 
            // tb_RecipeCode
            // 
            this.tb_RecipeCode.Location = new System.Drawing.Point(712, 120);
            this.tb_RecipeCode.Margin = new System.Windows.Forms.Padding(2);
            this.tb_RecipeCode.Name = "tb_RecipeCode";
            this.tb_RecipeCode.Size = new System.Drawing.Size(122, 20);
            this.tb_RecipeCode.TabIndex = 277;
            // 
            // tb_worker
            // 
            this.tb_worker.Location = new System.Drawing.Point(370, 367);
            this.tb_worker.Margin = new System.Windows.Forms.Padding(2);
            this.tb_worker.Name = "tb_worker";
            this.tb_worker.Size = new System.Drawing.Size(152, 20);
            this.tb_worker.TabIndex = 287;
            // 
            // tb_ManHour
            // 
            this.tb_ManHour.Location = new System.Drawing.Point(106, 367);
            this.tb_ManHour.Margin = new System.Windows.Forms.Padding(2);
            this.tb_ManHour.Name = "tb_ManHour";
            this.tb_ManHour.Size = new System.Drawing.Size(152, 20);
            this.tb_ManHour.TabIndex = 286;
            this.tb_ManHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tb_WorkNo
            // 
            this.tb_WorkNo.Location = new System.Drawing.Point(106, 330);
            this.tb_WorkNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_WorkNo.Name = "tb_WorkNo";
            this.tb_WorkNo.Size = new System.Drawing.Size(152, 20);
            this.tb_WorkNo.TabIndex = 284;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(47, 128);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(61, 13);
            this.label26.TabIndex = 316;
            this.label26.Text = "客户名称：";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(41, 306);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(68, 17);
            this.label25.TabIndex = 315;
            this.label25.Text = "生产信息";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(610, 123);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(49, 13);
            this.label22.TabIndex = 313;
            this.label22.Text = "配方号：";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(569, 367);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(55, 13);
            this.label20.TabIndex = 311;
            this.label20.Text = "印刷机号";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(312, 367);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(55, 13);
            this.label19.TabIndex = 310;
            this.label19.Text = "员工编号";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(39, 436);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(31, 13);
            this.label18.TabIndex = 309;
            this.label18.Text = "备注";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(39, 367);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 13);
            this.label16.TabIndex = 307;
            this.label16.Text = "生产工时";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(39, 332);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 13);
            this.label15.TabIndex = 306;
            this.label15.Text = "工单号";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(39, 497);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 13);
            this.label13.TabIndex = 304;
            this.label13.Text = "大卷编号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(610, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 300;
            this.label2.Text = "宽度(mm)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 90);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 299;
            this.label1.Text = "产品代号";
            // 
            // cb_ProductState
            // 
            this.cb_ProductState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductState.FormattingEnabled = true;
            this.cb_ProductState.Location = new System.Drawing.Point(106, 401);
            this.cb_ProductState.Margin = new System.Windows.Forms.Padding(2);
            this.cb_ProductState.Name = "cb_ProductState";
            this.cb_ProductState.Size = new System.Drawing.Size(150, 21);
            this.cb_ProductState.TabIndex = 396;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 403);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 395;
            this.label4.Text = "产品状态";
            // 
            // tb_RollWeight
            // 
            this.tb_RollWeight.Location = new System.Drawing.Point(630, 404);
            this.tb_RollWeight.Margin = new System.Windows.Forms.Padding(2);
            this.tb_RollWeight.Name = "tb_RollWeight";
            this.tb_RollWeight.Size = new System.Drawing.Size(158, 20);
            this.tb_RollWeight.TabIndex = 398;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(569, 408);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 397;
            this.label8.Text = "卷重";
            // 
            // cb_ProductQuality
            // 
            this.cb_ProductQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductQuality.FormattingEnabled = true;
            this.cb_ProductQuality.Location = new System.Drawing.Point(370, 408);
            this.cb_ProductQuality.Margin = new System.Windows.Forms.Padding(2);
            this.cb_ProductQuality.Name = "cb_ProductQuality";
            this.cb_ProductQuality.Size = new System.Drawing.Size(152, 21);
            this.cb_ProductQuality.TabIndex = 399;
            // 
            // lb_ProductQulity
            // 
            this.lb_ProductQulity.AutoSize = true;
            this.lb_ProductQulity.Location = new System.Drawing.Point(312, 408);
            this.lb_ProductQulity.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lb_ProductQulity.Name = "lb_ProductQulity";
            this.lb_ProductQulity.Size = new System.Drawing.Size(55, 13);
            this.lb_ProductQulity.TabIndex = 400;
            this.lb_ProductQulity.Text = "产品质量";
            // 
            // PrintEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 651);
            this.Controls.Add(this.cb_ProductQuality);
            this.Controls.Add(this.lb_ProductQulity);
            this.Controls.Add(this.cb_ProductState);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tb_RollWeight);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tb_BatchNo1);
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
            this.Controls.Add(this.tb_PrintMachineNo);
            this.Controls.Add(this.tb_Width);
            this.Controls.Add(this.tb_CustomerName);
            this.Controls.Add(this.tb_BigRollNo);
            this.Controls.Add(this.tb_Desc);
            this.Controls.Add(this.tb_RecipeCode);
            this.Controls.Add(this.tb_worker);
            this.Controls.Add(this.tb_ManHour);
            this.Controls.Add(this.tb_WorkNo);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.bt_Cancel);
            this.Controls.Add(this.bt_Printing);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PrintEditorForm";
            this.Text = "印刷单修改";
            this.Load += new System.EventHandler(this.PrintEditorForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_Save;
        private System.Windows.Forms.Button bt_Cancel;
        private System.Windows.Forms.Button bt_Printing;
        private System.Windows.Forms.TextBox tb_BatchNo1;
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
        private System.Windows.Forms.TextBox tb_PrintMachineNo;
        private System.Windows.Forms.TextBox tb_Width;
        private System.Windows.Forms.TextBox tb_CustomerName;
        private System.Windows.Forms.TextBox tb_BigRollNo;
        private System.Windows.Forms.TextBox tb_Desc;
        private System.Windows.Forms.TextBox tb_RecipeCode;
        private System.Windows.Forms.TextBox tb_worker;
        private System.Windows.Forms.TextBox tb_ManHour;
        private System.Windows.Forms.TextBox tb_WorkNo;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cb_ProductState;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_RollWeight;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cb_ProductQuality;
        private System.Windows.Forms.Label lb_ProductQulity;
    }
}