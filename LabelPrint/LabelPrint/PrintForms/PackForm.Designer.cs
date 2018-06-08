namespace LabelPrint
{
    partial class packForm
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
            this.tb_DateTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lb_InputBarcode = new System.Windows.Forms.Label();
            this.cb_ProductCode = new System.Windows.Forms.ComboBox();
            this.bt_Printing = new System.Windows.Forms.Button();
            this.tb_Width = new System.Windows.Forms.TextBox();
            this.tb_PackMachineNo = new System.Windows.Forms.TextBox();
            this.tb_BatchNo = new System.Windows.Forms.TextBox();
            this.tb_CustomerName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rb_NightWork = new System.Windows.Forms.RadioButton();
            this.rb_DayWork = new System.Windows.Forms.RadioButton();
            this.tb_Desc = new System.Windows.Forms.TextBox();
            this.tb_RecipeCode = new System.Windows.Forms.TextBox();
            this.tb_worker = new System.Windows.Forms.TextBox();
            this.tb_ManHour = new System.Windows.Forms.TextBox();
            this.tb_WorkNo = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_LittleRollNo = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_OrderNo = new System.Windows.Forms.TextBox();
            this.tb_ProductWeight = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.tb_ManufactureDate = new System.Windows.Forms.TextBox();
            this.tb_LittleRollCount = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tb_MaterialName = new System.Windows.Forms.TextBox();
            this.tb_RawMaterialCode = new System.Windows.Forms.TextBox();
            this.tb_ProductLength = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_PlateNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_Roll_Weight = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tb_BigRollNo = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.rb_Jia = new System.Windows.Forms.RadioButton();
            this.rb_Yi = new System.Windows.Forms.RadioButton();
            this.rb_Bing = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_DateTime
            // 
            this.tb_DateTime.Location = new System.Drawing.Point(620, 29);
            this.tb_DateTime.Name = "tb_DateTime";
            this.tb_DateTime.Size = new System.Drawing.Size(166, 24);
            this.tb_DateTime.TabIndex = 221;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(516, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 18);
            this.label3.TabIndex = 220;
            this.label3.Text = "打印时间";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lb_InputBarcode);
            this.groupBox4.Location = new System.Drawing.Point(1092, 569);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(534, 100);
            this.groupBox4.TabIndex = 219;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "扫描标签";
            // 
            // lb_InputBarcode
            // 
            this.lb_InputBarcode.AutoSize = true;
            this.lb_InputBarcode.Location = new System.Drawing.Point(24, 39);
            this.lb_InputBarcode.Name = "lb_InputBarcode";
            this.lb_InputBarcode.Size = new System.Drawing.Size(0, 18);
            this.lb_InputBarcode.TabIndex = 229;
            // 
            // cb_ProductCode
            // 
            this.cb_ProductCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductCode.FormattingEnabled = true;
            this.cb_ProductCode.Location = new System.Drawing.Point(118, 109);
            this.cb_ProductCode.Name = "cb_ProductCode";
            this.cb_ProductCode.Size = new System.Drawing.Size(154, 26);
            this.cb_ProductCode.TabIndex = 218;
            this.cb_ProductCode.SelectedValueChanged += new System.EventHandler(this.cb_ProductCode_SelectedValueChanged);
            // 
            // bt_Printing
            // 
            this.bt_Printing.Location = new System.Drawing.Point(1218, 864);
            this.bt_Printing.Name = "bt_Printing";
            this.bt_Printing.Size = new System.Drawing.Size(141, 53);
            this.bt_Printing.TabIndex = 217;
            this.bt_Printing.Text = "打包标签打印";
            this.bt_Printing.UseVisualStyleBackColor = true;
            this.bt_Printing.Click += new System.EventHandler(this.bt_Printing_Click);
            // 
            // tb_Width
            // 
            this.tb_Width.Location = new System.Drawing.Point(1305, 105);
            this.tb_Width.Name = "tb_Width";
            this.tb_Width.Size = new System.Drawing.Size(154, 24);
            this.tb_Width.TabIndex = 215;
            // 
            // tb_PackMachineNo
            // 
            this.tb_PackMachineNo.Location = new System.Drawing.Point(6, 33);
            this.tb_PackMachineNo.Name = "tb_PackMachineNo";
            this.tb_PackMachineNo.Size = new System.Drawing.Size(144, 24);
            this.tb_PackMachineNo.TabIndex = 214;
            this.tb_PackMachineNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tb_BatchNo
            // 
            this.tb_BatchNo.Location = new System.Drawing.Point(408, 112);
            this.tb_BatchNo.Name = "tb_BatchNo";
            this.tb_BatchNo.Size = new System.Drawing.Size(154, 24);
            this.tb_BatchNo.TabIndex = 213;
            // 
            // tb_CustomerName
            // 
            this.tb_CustomerName.Location = new System.Drawing.Point(118, 43);
            this.tb_CustomerName.Name = "tb_CustomerName";
            this.tb_CustomerName.Size = new System.Drawing.Size(154, 24);
            this.tb_CustomerName.TabIndex = 212;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rb_NightWork);
            this.groupBox1.Controls.Add(this.rb_DayWork);
            this.groupBox1.Location = new System.Drawing.Point(1127, 179);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 80);
            this.groupBox1.TabIndex = 209;
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
            // tb_Desc
            // 
            this.tb_Desc.Location = new System.Drawing.Point(139, 203);
            this.tb_Desc.Multiline = true;
            this.tb_Desc.Name = "tb_Desc";
            this.tb_Desc.Size = new System.Drawing.Size(647, 150);
            this.tb_Desc.TabIndex = 206;
            // 
            // tb_RecipeCode
            // 
            this.tb_RecipeCode.Location = new System.Drawing.Point(984, 179);
            this.tb_RecipeCode.Name = "tb_RecipeCode";
            this.tb_RecipeCode.Size = new System.Drawing.Size(154, 24);
            this.tb_RecipeCode.TabIndex = 205;
            // 
            // tb_worker
            // 
            this.tb_worker.Location = new System.Drawing.Point(620, 79);
            this.tb_worker.Name = "tb_worker";
            this.tb_worker.Size = new System.Drawing.Size(166, 24);
            this.tb_worker.TabIndex = 203;
            // 
            // tb_ManHour
            // 
            this.tb_ManHour.Location = new System.Drawing.Point(136, 82);
            this.tb_ManHour.Name = "tb_ManHour";
            this.tb_ManHour.Size = new System.Drawing.Size(226, 24);
            this.tb_ManHour.TabIndex = 202;
            this.tb_ManHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tb_WorkNo
            // 
            this.tb_WorkNo.Location = new System.Drawing.Point(136, 29);
            this.tb_WorkNo.Name = "tb_WorkNo";
            this.tb_WorkNo.Size = new System.Drawing.Size(226, 24);
            this.tb_WorkNo.TabIndex = 201;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(141, 157);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(38, 18);
            this.label29.TabIndex = 199;
            this.label29.Text = "扫描";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(312, 116);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(83, 18);
            this.label27.TabIndex = 198;
            this.label27.Text = "生产批号：";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(28, 46);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(83, 18);
            this.label26.TabIndex = 197;
            this.label26.Text = "客户名称：";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(903, 183);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(68, 18);
            this.label22.TabIndex = 194;
            this.label22.Text = "配方号：";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(516, 82);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(68, 18);
            this.label19.TabIndex = 192;
            this.label19.Text = "员工编号";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(42, 203);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 18);
            this.label18.TabIndex = 191;
            this.label18.Text = "备注";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(38, 82);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(68, 18);
            this.label16.TabIndex = 190;
            this.label16.Text = "生产工时";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(36, 33);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 18);
            this.label15.TabIndex = 189;
            this.label15.Text = "工单号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1172, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 18);
            this.label2.TabIndex = 186;
            this.label2.Text = "宽度(mm)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 18);
            this.label1.TabIndex = 185;
            this.label1.Text = "产品代号";
            // 
            // tb_LittleRollNo
            // 
            this.tb_LittleRollNo.Location = new System.Drawing.Point(-424, 316);
            this.tb_LittleRollNo.Name = "tb_LittleRollNo";
            this.tb_LittleRollNo.Size = new System.Drawing.Size(226, 24);
            this.tb_LittleRollNo.TabIndex = 228;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tb_PackMachineNo);
            this.groupBox3.Location = new System.Drawing.Point(126, 179);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(158, 94);
            this.groupBox3.TabIndex = 229;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "打包机号";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.tb_OrderNo);
            this.groupBox5.Controls.Add(this.tb_ProductWeight);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.label38);
            this.groupBox5.Controls.Add(this.tb_ManufactureDate);
            this.groupBox5.Controls.Add(this.tb_LittleRollCount);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.tb_MaterialName);
            this.groupBox5.Controls.Add(this.tb_RawMaterialCode);
            this.groupBox5.Controls.Add(this.tb_ProductLength);
            this.groupBox5.Controls.Add(this.label20);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.tb_PlateNo);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.tb_Roll_Weight);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label26);
            this.groupBox5.Controls.Add(this.tb_CustomerName);
            this.groupBox5.Controls.Add(this.tb_BatchNo);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.cb_ProductCode);
            this.groupBox5.Controls.Add(this.tb_Width);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.tb_RecipeCode);
            this.groupBox5.Controls.Add(this.label27);
            this.groupBox5.Controls.Add(this.label22);
            this.groupBox5.Location = new System.Drawing.Point(126, 280);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1500, 252);
            this.groupBox5.TabIndex = 230;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "产品信息";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(902, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 18);
            this.label7.TabIndex = 250;
            this.label7.Text = "订单号：";
            // 
            // tb_OrderNo
            // 
            this.tb_OrderNo.Location = new System.Drawing.Point(981, 35);
            this.tb_OrderNo.Name = "tb_OrderNo";
            this.tb_OrderNo.Size = new System.Drawing.Size(154, 24);
            this.tb_OrderNo.TabIndex = 251;
            // 
            // tb_ProductWeight
            // 
            this.tb_ProductWeight.Location = new System.Drawing.Point(981, 110);
            this.tb_ProductWeight.Name = "tb_ProductWeight";
            this.tb_ProductWeight.Size = new System.Drawing.Size(154, 24);
            this.tb_ProductWeight.TabIndex = 249;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(902, 116);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(53, 18);
            this.label21.TabIndex = 248;
            this.label21.Text = "基重：";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(586, 183);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(83, 18);
            this.label38.TabIndex = 252;
            this.label38.Text = "生产日期：";
            // 
            // tb_ManufactureDate
            // 
            this.tb_ManufactureDate.Location = new System.Drawing.Point(696, 177);
            this.tb_ManufactureDate.Name = "tb_ManufactureDate";
            this.tb_ManufactureDate.Size = new System.Drawing.Size(154, 24);
            this.tb_ManufactureDate.TabIndex = 253;
            // 
            // tb_LittleRollCount
            // 
            this.tb_LittleRollCount.Location = new System.Drawing.Point(696, 110);
            this.tb_LittleRollCount.Name = "tb_LittleRollCount";
            this.tb_LittleRollCount.Size = new System.Drawing.Size(154, 24);
            this.tb_LittleRollCount.TabIndex = 251;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(584, 116);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 18);
            this.label8.TabIndex = 250;
            this.label8.Text = "卷数：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(585, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 18);
            this.label9.TabIndex = 248;
            this.label9.Text = "材料名称：";
            // 
            // tb_MaterialName
            // 
            this.tb_MaterialName.Location = new System.Drawing.Point(696, 36);
            this.tb_MaterialName.Name = "tb_MaterialName";
            this.tb_MaterialName.Size = new System.Drawing.Size(154, 24);
            this.tb_MaterialName.TabIndex = 249;
            // 
            // tb_RawMaterialCode
            // 
            this.tb_RawMaterialCode.Location = new System.Drawing.Point(1305, 36);
            this.tb_RawMaterialCode.Name = "tb_RawMaterialCode";
            this.tb_RawMaterialCode.Size = new System.Drawing.Size(154, 24);
            this.tb_RawMaterialCode.TabIndex = 247;
            // 
            // tb_ProductLength
            // 
            this.tb_ProductLength.Location = new System.Drawing.Point(408, 179);
            this.tb_ProductLength.Name = "tb_ProductLength";
            this.tb_ProductLength.Size = new System.Drawing.Size(154, 24);
            this.tb_ProductLength.TabIndex = 245;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(1162, 39);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(98, 18);
            this.label20.TabIndex = 246;
            this.label20.Text = "原材料代码：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(315, 183);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 18);
            this.label6.TabIndex = 244;
            this.label6.Text = "长度：";
            // 
            // tb_PlateNo
            // 
            this.tb_PlateNo.Location = new System.Drawing.Point(408, 41);
            this.tb_PlateNo.Name = "tb_PlateNo";
            this.tb_PlateNo.Size = new System.Drawing.Size(154, 24);
            this.tb_PlateNo.TabIndex = 243;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(314, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 18);
            this.label5.TabIndex = 242;
            this.label5.Text = "产板号：";
            // 
            // tb_Roll_Weight
            // 
            this.tb_Roll_Weight.Location = new System.Drawing.Point(118, 181);
            this.tb_Roll_Weight.Name = "tb_Roll_Weight";
            this.tb_Roll_Weight.Size = new System.Drawing.Size(154, 24);
            this.tb_Roll_Weight.TabIndex = 241;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 183);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 18);
            this.label4.TabIndex = 240;
            this.label4.Text = "重量：";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tb_BigRollNo);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.label15);
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.label19);
            this.groupBox6.Controls.Add(this.tb_WorkNo);
            this.groupBox6.Controls.Add(this.tb_ManHour);
            this.groupBox6.Controls.Add(this.tb_worker);
            this.groupBox6.Controls.Add(this.tb_DateTime);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.tb_Desc);
            this.groupBox6.Controls.Add(this.label18);
            this.groupBox6.Location = new System.Drawing.Point(129, 569);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(873, 381);
            this.groupBox6.TabIndex = 232;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "生产信息";
            // 
            // tb_BigRollNo
            // 
            this.tb_BigRollNo.Location = new System.Drawing.Point(136, 129);
            this.tb_BigRollNo.Multiline = true;
            this.tb_BigRollNo.Name = "tb_BigRollNo";
            this.tb_BigRollNo.Size = new System.Drawing.Size(650, 48);
            this.tb_BigRollNo.TabIndex = 233;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(44, 132);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(38, 18);
            this.label11.TabIndex = 232;
            this.label11.Text = "卷号";
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rb_Bing);
            this.groupBox2.Controls.Add(this.rb_Yi);
            this.groupBox2.Controls.Add(this.rb_Jia);
            this.groupBox2.Location = new System.Drawing.Point(667, 179);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(264, 80);
            this.groupBox2.TabIndex = 210;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "班別";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(612, 65);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(449, 44);
            this.label10.TabIndex = 233;
            this.label10.Text = "紫华企业打包工序操作单";
            // 
            // packForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1826, 1045);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.tb_LittleRollNo);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.bt_Printing);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox5);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MaximizeBox = false;
            this.Name = "packForm";
            this.Text = "打包标签";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.packing_FormClosing);
            this.Load += new System.EventHandler(this.PackForm_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_DateTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cb_ProductCode;
        private System.Windows.Forms.Button bt_Printing;
        private System.Windows.Forms.TextBox tb_Width;
        private System.Windows.Forms.TextBox tb_PackMachineNo;
        private System.Windows.Forms.TextBox tb_BatchNo;
        private System.Windows.Forms.TextBox tb_CustomerName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rb_NightWork;
        private System.Windows.Forms.RadioButton rb_DayWork;
        private System.Windows.Forms.TextBox tb_Desc;
        private System.Windows.Forms.TextBox tb_RecipeCode;
        private System.Windows.Forms.TextBox tb_worker;
        private System.Windows.Forms.TextBox tb_ManHour;
        private System.Windows.Forms.TextBox tb_WorkNo;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_LittleRollNo;
        private System.Windows.Forms.Label lb_InputBarcode;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox tb_ManufactureDate;
        private System.Windows.Forms.TextBox tb_LittleRollCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tb_MaterialName;
        private System.Windows.Forms.TextBox tb_ProductLength;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_PlateNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_Roll_Weight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_OrderNo;
        private System.Windows.Forms.TextBox tb_ProductWeight;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox tb_RawMaterialCode;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton rb_Jia;
        private System.Windows.Forms.RadioButton rb_Yi;
        private System.Windows.Forms.RadioButton rb_Bing;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tb_BigRollNo;
        private System.Windows.Forms.Label label11;
    }
}