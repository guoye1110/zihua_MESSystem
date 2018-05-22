﻿namespace LabelPrint
{
    partial class PrintForm
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lb_OutputBarCode = new System.Windows.Forms.Label();
            this.lb_outBarcode = new System.Windows.Forms.Label();
            this.cb_ProductCode = new System.Windows.Forms.ComboBox();
            this.bt_Printing = new System.Windows.Forms.Button();
            this.tb_Width = new System.Windows.Forms.TextBox();
            this.tb_PrintMachineNo = new System.Windows.Forms.TextBox();
            this.tb_BatchNo = new System.Windows.Forms.TextBox();
            this.tb_CustomerName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rb_Bing = new System.Windows.Forms.RadioButton();
            this.rb_Yi = new System.Windows.Forms.RadioButton();
            this.rb_Jia = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rb_NightWork = new System.Windows.Forms.RadioButton();
            this.rb_DayWork = new System.Windows.Forms.RadioButton();
            this.tx_ScaleSerialPort = new System.Windows.Forms.Label();
            this.tb_BigRollNo = new System.Windows.Forms.TextBox();
            this.tb_Desc = new System.Windows.Forms.TextBox();
            this.tb_RecipeCode = new System.Windows.Forms.TextBox();
            this.tb_worker = new System.Windows.Forms.TextBox();
            this.tb_ManHour = new System.Windows.Forms.TextBox();
            this.tb_WorkNo = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lb_InputBarCode1 = new System.Windows.Forms.Label();
            this.lb_InputBarcode = new System.Windows.Forms.Label();
            this.tb_DateTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bt_Record = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_RollWeight = new System.Windows.Forms.TextBox();
            this.cb_ProductState = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lb_ProductQulity = new System.Windows.Forms.Label();
            this.cb_ProductQuality = new System.Windows.Forms.ComboBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lb_OutputBarCode);
            this.groupBox4.Controls.Add(this.lb_outBarcode);
            this.groupBox4.Location = new System.Drawing.Point(1203, 594);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(436, 96);
            this.groupBox4.TabIndex = 217;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "印刷标签";
            // 
            // lb_OutputBarCode
            // 
            this.lb_OutputBarCode.AutoSize = true;
            this.lb_OutputBarCode.Location = new System.Drawing.Point(39, 50);
            this.lb_OutputBarCode.Name = "lb_OutputBarCode";
            this.lb_OutputBarCode.Size = new System.Drawing.Size(46, 18);
            this.lb_OutputBarCode.TabIndex = 225;
            this.lb_OutputBarCode.Text = "label4";
            // 
            // lb_outBarcode
            // 
            this.lb_outBarcode.AutoSize = true;
            this.lb_outBarcode.Location = new System.Drawing.Point(33, 50);
            this.lb_outBarcode.Name = "lb_outBarcode";
            this.lb_outBarcode.Size = new System.Drawing.Size(0, 18);
            this.lb_outBarcode.TabIndex = 225;
            // 
            // cb_ProductCode
            // 
            this.cb_ProductCode.FormattingEnabled = true;
            this.cb_ProductCode.Location = new System.Drawing.Point(105, 37);
            this.cb_ProductCode.Name = "cb_ProductCode";
            this.cb_ProductCode.Size = new System.Drawing.Size(112, 26);
            this.cb_ProductCode.TabIndex = 216;
            this.cb_ProductCode.SelectedIndexChanged += new System.EventHandler(this.cb_ProductCode1_SelectedIndexChanged);
            // 
            // bt_Printing
            // 
            this.bt_Printing.Location = new System.Drawing.Point(1321, 731);
            this.bt_Printing.Name = "bt_Printing";
            this.bt_Printing.Size = new System.Drawing.Size(141, 44);
            this.bt_Printing.TabIndex = 215;
            this.bt_Printing.Text = "标签打印";
            this.bt_Printing.UseVisualStyleBackColor = true;
            this.bt_Printing.Click += new System.EventHandler(this.bt_Printing_Click);
            // 
            // tb_Width
            // 
            this.tb_Width.Location = new System.Drawing.Point(656, 35);
            this.tb_Width.Name = "tb_Width";
            this.tb_Width.Size = new System.Drawing.Size(84, 24);
            this.tb_Width.TabIndex = 213;
            // 
            // tb_PrintMachineNo
            // 
            this.tb_PrintMachineNo.Location = new System.Drawing.Point(28, 24);
            this.tb_PrintMachineNo.Name = "tb_PrintMachineNo";
            this.tb_PrintMachineNo.Size = new System.Drawing.Size(86, 24);
            this.tb_PrintMachineNo.TabIndex = 212;
            this.tb_PrintMachineNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tb_BatchNo
            // 
            this.tb_BatchNo.Location = new System.Drawing.Point(1114, 35);
            this.tb_BatchNo.Name = "tb_BatchNo";
            this.tb_BatchNo.Size = new System.Drawing.Size(112, 24);
            this.tb_BatchNo.TabIndex = 211;
            // 
            // tb_CustomerName
            // 
            this.tb_CustomerName.Location = new System.Drawing.Point(1114, 86);
            this.tb_CustomerName.Name = "tb_CustomerName";
            this.tb_CustomerName.Size = new System.Drawing.Size(112, 24);
            this.tb_CustomerName.TabIndex = 210;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rb_Bing);
            this.groupBox2.Controls.Add(this.rb_Yi);
            this.groupBox2.Controls.Add(this.rb_Jia);
            this.groupBox2.Location = new System.Drawing.Point(527, 181);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(264, 80);
            this.groupBox2.TabIndex = 208;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "班別";
            // 
            // rb_Bing
            // 
            this.rb_Bing.AutoSize = true;
            this.rb_Bing.Location = new System.Drawing.Point(130, 35);
            this.rb_Bing.Name = "rb_Bing";
            this.rb_Bing.Size = new System.Drawing.Size(41, 22);
            this.rb_Bing.TabIndex = 2;
            this.rb_Bing.TabStop = true;
            this.rb_Bing.Text = "丙";
            this.rb_Bing.UseVisualStyleBackColor = true;
            // 
            // rb_Yi
            // 
            this.rb_Yi.AutoSize = true;
            this.rb_Yi.Location = new System.Drawing.Point(75, 35);
            this.rb_Yi.Name = "rb_Yi";
            this.rb_Yi.Size = new System.Drawing.Size(41, 22);
            this.rb_Yi.TabIndex = 1;
            this.rb_Yi.TabStop = true;
            this.rb_Yi.Text = "乙";
            this.rb_Yi.UseVisualStyleBackColor = true;
            // 
            // rb_Jia
            // 
            this.rb_Jia.AutoSize = true;
            this.rb_Jia.Location = new System.Drawing.Point(24, 35);
            this.rb_Jia.Name = "rb_Jia";
            this.rb_Jia.Size = new System.Drawing.Size(41, 22);
            this.rb_Jia.TabIndex = 0;
            this.rb_Jia.TabStop = true;
            this.rb_Jia.Text = "甲";
            this.rb_Jia.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rb_NightWork);
            this.groupBox1.Controls.Add(this.rb_DayWork);
            this.groupBox1.Location = new System.Drawing.Point(830, 181);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 80);
            this.groupBox1.TabIndex = 207;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "班次";
            // 
            // rb_NightWork
            // 
            this.rb_NightWork.AutoSize = true;
            this.rb_NightWork.Location = new System.Drawing.Point(124, 35);
            this.rb_NightWork.Name = "rb_NightWork";
            this.rb_NightWork.Size = new System.Drawing.Size(56, 22);
            this.rb_NightWork.TabIndex = 1;
            this.rb_NightWork.TabStop = true;
            this.rb_NightWork.Text = "夜班";
            this.rb_NightWork.UseVisualStyleBackColor = true;
            // 
            // rb_DayWork
            // 
            this.rb_DayWork.AutoSize = true;
            this.rb_DayWork.Location = new System.Drawing.Point(24, 35);
            this.rb_DayWork.Name = "rb_DayWork";
            this.rb_DayWork.Size = new System.Drawing.Size(56, 22);
            this.rb_DayWork.TabIndex = 0;
            this.rb_DayWork.TabStop = true;
            this.rb_DayWork.Text = "日班";
            this.rb_DayWork.UseVisualStyleBackColor = true;
            // 
            // tx_ScaleSerialPort
            // 
            this.tx_ScaleSerialPort.AutoSize = true;
            this.tx_ScaleSerialPort.Location = new System.Drawing.Point(599, 157);
            this.tx_ScaleSerialPort.Name = "tx_ScaleSerialPort";
            this.tx_ScaleSerialPort.Size = new System.Drawing.Size(0, 18);
            this.tx_ScaleSerialPort.TabIndex = 206;
            // 
            // tb_BigRollNo
            // 
            this.tb_BigRollNo.Location = new System.Drawing.Point(327, 581);
            this.tb_BigRollNo.Name = "tb_BigRollNo";
            this.tb_BigRollNo.Size = new System.Drawing.Size(226, 24);
            this.tb_BigRollNo.TabIndex = 205;
            // 
            // tb_Desc
            // 
            this.tb_Desc.Location = new System.Drawing.Point(327, 688);
            this.tb_Desc.Multiline = true;
            this.tb_Desc.Name = "tb_Desc";
            this.tb_Desc.Size = new System.Drawing.Size(600, 73);
            this.tb_Desc.TabIndex = 204;
            // 
            // tb_RecipeCode
            // 
            this.tb_RecipeCode.Location = new System.Drawing.Point(105, 90);
            this.tb_RecipeCode.Name = "tb_RecipeCode";
            this.tb_RecipeCode.Size = new System.Drawing.Size(112, 24);
            this.tb_RecipeCode.TabIndex = 203;
            // 
            // tb_worker
            // 
            this.tb_worker.Location = new System.Drawing.Point(651, 86);
            this.tb_worker.Name = "tb_worker";
            this.tb_worker.Size = new System.Drawing.Size(184, 24);
            this.tb_worker.TabIndex = 201;
            // 
            // tb_ManHour
            // 
            this.tb_ManHour.Location = new System.Drawing.Point(327, 530);
            this.tb_ManHour.Name = "tb_ManHour";
            this.tb_ManHour.Size = new System.Drawing.Size(226, 24);
            this.tb_ManHour.TabIndex = 200;
            this.tb_ManHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tb_WorkNo
            // 
            this.tb_WorkNo.Location = new System.Drawing.Point(327, 478);
            this.tb_WorkNo.Name = "tb_WorkNo";
            this.tb_WorkNo.Size = new System.Drawing.Size(226, 24);
            this.tb_WorkNo.TabIndex = 199;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(473, 157);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(87, 18);
            this.label30.TabIndex = 198;
            this.label30.Text = "电子秤串口:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(184, 157);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(38, 18);
            this.label29.TabIndex = 197;
            this.label29.Text = "扫描";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(1006, 40);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(76, 18);
            this.label27.TabIndex = 196;
            this.label27.Text = "生产批号：";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(1006, 86);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(76, 18);
            this.label26.TabIndex = 195;
            this.label26.Text = "客户名称：";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(15, 93);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(61, 18);
            this.label22.TabIndex = 192;
            this.label22.Text = "配方号：";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(567, 89);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(68, 18);
            this.label19.TabIndex = 190;
            this.label19.Text = "员工编号";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(229, 688);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 18);
            this.label18.TabIndex = 189;
            this.label18.Text = "备注";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(228, 530);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(68, 18);
            this.label16.TabIndex = 188;
            this.label16.Text = "生产工时";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(225, 482);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 18);
            this.label15.TabIndex = 187;
            this.label15.Text = "工单号";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(228, 581);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(68, 18);
            this.label13.TabIndex = 186;
            this.label13.Text = "大卷编号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(554, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 18);
            this.label2.TabIndex = 184;
            this.label2.Text = "宽度(mm)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 18);
            this.label1.TabIndex = 183;
            this.label1.Text = "产品代号";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lb_InputBarCode1);
            this.groupBox5.Controls.Add(this.lb_InputBarcode);
            this.groupBox5.Location = new System.Drawing.Point(1203, 462);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(436, 96);
            this.groupBox5.TabIndex = 218;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "扫描标签";
            // 
            // lb_InputBarCode1
            // 
            this.lb_InputBarCode1.AutoSize = true;
            this.lb_InputBarCode1.Location = new System.Drawing.Point(39, 50);
            this.lb_InputBarCode1.Name = "lb_InputBarCode1";
            this.lb_InputBarCode1.Size = new System.Drawing.Size(0, 18);
            this.lb_InputBarCode1.TabIndex = 226;
            // 
            // lb_InputBarcode
            // 
            this.lb_InputBarcode.AutoSize = true;
            this.lb_InputBarcode.Location = new System.Drawing.Point(24, 50);
            this.lb_InputBarcode.Name = "lb_InputBarcode";
            this.lb_InputBarcode.Size = new System.Drawing.Size(0, 18);
            this.lb_InputBarcode.TabIndex = 0;
            // 
            // tb_DateTime
            // 
            this.tb_DateTime.Location = new System.Drawing.Point(651, 33);
            this.tb_DateTime.Name = "tb_DateTime";
            this.tb_DateTime.Size = new System.Drawing.Size(184, 24);
            this.tb_DateTime.TabIndex = 220;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(567, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 18);
            this.label3.TabIndex = 219;
            this.label3.Text = "打印时间";
            // 
            // bt_Record
            // 
            this.bt_Record.Location = new System.Drawing.Point(1513, 730);
            this.bt_Record.Name = "bt_Record";
            this.bt_Record.Size = new System.Drawing.Size(126, 46);
            this.bt_Record.TabIndex = 225;
            this.bt_Record.Text = "交接班记录";
            this.bt_Record.UseVisualStyleBackColor = true;
            this.bt_Record.Click += new System.EventHandler(this.bt_Record_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tb_PrintMachineNo);
            this.groupBox3.Location = new System.Drawing.Point(186, 193);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(159, 68);
            this.groupBox3.TabIndex = 226;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "印刷机号";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(567, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 18);
            this.label4.TabIndex = 227;
            this.label4.Text = "卷重";
            // 
            // tb_RollWeight
            // 
            this.tb_RollWeight.Location = new System.Drawing.Point(651, 133);
            this.tb_RollWeight.Name = "tb_RollWeight";
            this.tb_RollWeight.Size = new System.Drawing.Size(184, 24);
            this.tb_RollWeight.TabIndex = 228;
            // 
            // cb_ProductState
            // 
            this.cb_ProductState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductState.FormattingEnabled = true;
            this.cb_ProductState.Location = new System.Drawing.Point(327, 631);
            this.cb_ProductState.Name = "cb_ProductState";
            this.cb_ProductState.Size = new System.Drawing.Size(224, 26);
            this.cb_ProductState.TabIndex = 230;
            this.cb_ProductState.SelectedIndexChanged += new System.EventHandler(this.cb_ProductState_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(221, 635);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 18);
            this.label5.TabIndex = 229;
            this.label5.Text = "产品状态";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lb_ProductQulity);
            this.groupBox6.Controls.Add(this.tb_worker);
            this.groupBox6.Controls.Add(this.cb_ProductQuality);
            this.groupBox6.Controls.Add(this.label19);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.tb_RollWeight);
            this.groupBox6.Controls.Add(this.tb_DateTime);
            this.groupBox6.Location = new System.Drawing.Point(157, 444);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(915, 367);
            this.groupBox6.TabIndex = 231;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "生产信息";
            // 
            // lb_ProductQulity
            // 
            this.lb_ProductQulity.AutoSize = true;
            this.lb_ProductQulity.Location = new System.Drawing.Point(567, 190);
            this.lb_ProductQulity.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lb_ProductQulity.Name = "lb_ProductQulity";
            this.lb_ProductQulity.Size = new System.Drawing.Size(76, 18);
            this.lb_ProductQulity.TabIndex = 235;
            this.lb_ProductQulity.Text = "产品质量：";
            // 
            // cb_ProductQuality
            // 
            this.cb_ProductQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductQuality.FormattingEnabled = true;
            this.cb_ProductQuality.Location = new System.Drawing.Point(651, 187);
            this.cb_ProductQuality.Margin = new System.Windows.Forms.Padding(2);
            this.cb_ProductQuality.Name = "cb_ProductQuality";
            this.cb_ProductQuality.Size = new System.Drawing.Size(184, 26);
            this.cb_ProductQuality.TabIndex = 236;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.cb_ProductCode);
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Controls.Add(this.tb_Width);
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.tb_RecipeCode);
            this.groupBox7.Controls.Add(this.label22);
            this.groupBox7.Controls.Add(this.label27);
            this.groupBox7.Controls.Add(this.tb_BatchNo);
            this.groupBox7.Controls.Add(this.tb_CustomerName);
            this.groupBox7.Controls.Add(this.label26);
            this.groupBox7.Location = new System.Drawing.Point(160, 285);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(1486, 143);
            this.groupBox7.TabIndex = 232;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "产品信息";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(594, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(449, 44);
            this.label6.TabIndex = 233;
            this.label6.Text = "紫华企业印刷工序操作单";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1118, 731);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 44);
            this.button1.TabIndex = 234;
            this.button1.Text = "印刷开工";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1916, 1053);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cb_ProductState);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.bt_Record);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.bt_Printing);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tx_ScaleSerialPort);
            this.Controls.Add(this.tb_BigRollNo);
            this.Controls.Add(this.tb_Desc);
            this.Controls.Add(this.tb_ManHour);
            this.Controls.Add(this.tb_WorkNo);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox7);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "PrintForm";
            this.Text = "印刷单";
            this.Load += new System.EventHandler(this.PrintForm_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cb_ProductCode;
        private System.Windows.Forms.Button bt_Printing;
        private System.Windows.Forms.TextBox tb_Width;
        private System.Windows.Forms.TextBox tb_PrintMachineNo;
        private System.Windows.Forms.TextBox tb_BatchNo;
        private System.Windows.Forms.TextBox tb_CustomerName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rb_Bing;
        private System.Windows.Forms.RadioButton rb_Yi;
        private System.Windows.Forms.RadioButton rb_Jia;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rb_NightWork;
        private System.Windows.Forms.RadioButton rb_DayWork;
        private System.Windows.Forms.Label tx_ScaleSerialPort;
        private System.Windows.Forms.TextBox tb_BigRollNo;
        private System.Windows.Forms.TextBox tb_Desc;
        private System.Windows.Forms.TextBox tb_RecipeCode;
        private System.Windows.Forms.TextBox tb_worker;
        private System.Windows.Forms.TextBox tb_ManHour;
        private System.Windows.Forms.TextBox tb_WorkNo;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox tb_DateTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lb_InputBarcode;
        private System.Windows.Forms.Label lb_OutputBarCode;
        private System.Windows.Forms.Label lb_outBarcode;
        private System.Windows.Forms.Label lb_InputBarCode1;
        private System.Windows.Forms.Button bt_Record;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_RollWeight;
        private System.Windows.Forms.ComboBox cb_ProductState;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lb_ProductQulity;
        private System.Windows.Forms.ComboBox cb_ProductQuality;
    }
}