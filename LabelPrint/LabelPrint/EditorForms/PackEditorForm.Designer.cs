namespace LabelPrint.EditorForms
{
    partial class PackEditorForm
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
            this.cb_WorkClass = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cb_WorkTime = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cb_ProductCode = new System.Windows.Forms.ComboBox();
            this.tb_PackMachineNo = new System.Windows.Forms.TextBox();
            this.tb_Width = new System.Windows.Forms.TextBox();
            this.tb_CustomerName = new System.Windows.Forms.TextBox();
            this.tb_LittleRollNo = new System.Windows.Forms.TextBox();
            this.tb_BigRollNo = new System.Windows.Forms.TextBox();
            this.tb_Desc = new System.Windows.Forms.TextBox();
            this.tb_RecipeCode = new System.Windows.Forms.TextBox();
            this.tb_worker = new System.Windows.Forms.TextBox();
            this.tb_ManHour = new System.Windows.Forms.TextBox();
            this.tb_WorkNo = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
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
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tb_PlateNo = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tb_Roll_Weight = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bt_Save
            // 
            this.bt_Save.Location = new System.Drawing.Point(876, 583);
            this.bt_Save.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(140, 39);
            this.bt_Save.TabIndex = 189;
            this.bt_Save.Text = "确定";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // bt_Cancel
            // 
            this.bt_Cancel.Location = new System.Drawing.Point(685, 583);
            this.bt_Cancel.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Cancel.Name = "bt_Cancel";
            this.bt_Cancel.Size = new System.Drawing.Size(140, 39);
            this.bt_Cancel.TabIndex = 188;
            this.bt_Cancel.Text = "取消";
            this.bt_Cancel.UseVisualStyleBackColor = true;
            this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
            // 
            // bt_Printing
            // 
            this.bt_Printing.Location = new System.Drawing.Point(494, 583);
            this.bt_Printing.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Printing.Name = "bt_Printing";
            this.bt_Printing.Size = new System.Drawing.Size(140, 39);
            this.bt_Printing.TabIndex = 187;
            this.bt_Printing.Text = "打印";
            this.bt_Printing.UseVisualStyleBackColor = true;
            this.bt_Printing.Click += new System.EventHandler(this.bt_Printing_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(105, 22);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 17);
            this.label7.TabIndex = 390;
            this.label7.Text = "基础信息";
            // 
            // tb_BatchNo
            // 
            this.tb_BatchNo.Location = new System.Drawing.Point(358, 128);
            this.tb_BatchNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_BatchNo.Name = "tb_BatchNo";
            this.tb_BatchNo.Size = new System.Drawing.Size(94, 20);
            this.tb_BatchNo.TabIndex = 331;
            // 
            // cb_WorkClass
            // 
            this.cb_WorkClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_WorkClass.FormattingEnabled = true;
            this.cb_WorkClass.Location = new System.Drawing.Point(863, 529);
            this.cb_WorkClass.Margin = new System.Windows.Forms.Padding(2);
            this.cb_WorkClass.Name = "cb_WorkClass";
            this.cb_WorkClass.Size = new System.Drawing.Size(112, 21);
            this.cb_WorkClass.TabIndex = 360;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(758, 532);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 386;
            this.label6.Text = "班     別";
            // 
            // cb_WorkTime
            // 
            this.cb_WorkTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_WorkTime.FormattingEnabled = true;
            this.cb_WorkTime.Location = new System.Drawing.Point(861, 496);
            this.cb_WorkTime.Margin = new System.Windows.Forms.Padding(2);
            this.cb_WorkTime.Name = "cb_WorkTime";
            this.cb_WorkTime.Size = new System.Drawing.Size(112, 21);
            this.cb_WorkTime.TabIndex = 357;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(758, 496);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 385;
            this.label5.Text = "班     次";
            // 
            // cb_ProductCode
            // 
            this.cb_ProductCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductCode.FormattingEnabled = true;
            this.cb_ProductCode.Location = new System.Drawing.Point(164, 131);
            this.cb_ProductCode.Margin = new System.Windows.Forms.Padding(2);
            this.cb_ProductCode.Name = "cb_ProductCode";
            this.cb_ProductCode.Size = new System.Drawing.Size(105, 21);
            this.cb_ProductCode.TabIndex = 333;
            this.cb_ProductCode.SelectionChangeCommitted += new System.EventHandler(this.cb_ProductCode_SelectionChangeCommitted);
            // 
            // tb_PackMachineNo
            // 
            this.tb_PackMachineNo.Location = new System.Drawing.Point(816, 366);
            this.tb_PackMachineNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_PackMachineNo.Name = "tb_PackMachineNo";
            this.tb_PackMachineNo.Size = new System.Drawing.Size(158, 20);
            this.tb_PackMachineNo.TabIndex = 347;
            // 
            // tb_Width
            // 
            this.tb_Width.Location = new System.Drawing.Point(898, 128);
            this.tb_Width.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Width.Name = "tb_Width";
            this.tb_Width.Size = new System.Drawing.Size(86, 20);
            this.tb_Width.TabIndex = 335;
            // 
            // tb_CustomerName
            // 
            this.tb_CustomerName.Location = new System.Drawing.Point(168, 77);
            this.tb_CustomerName.Margin = new System.Windows.Forms.Padding(2);
            this.tb_CustomerName.Name = "tb_CustomerName";
            this.tb_CustomerName.Size = new System.Drawing.Size(101, 20);
            this.tb_CustomerName.TabIndex = 337;
            // 
            // tb_LittleRollNo
            // 
            this.tb_LittleRollNo.Location = new System.Drawing.Point(409, 494);
            this.tb_LittleRollNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_LittleRollNo.Name = "tb_LittleRollNo";
            this.tb_LittleRollNo.Size = new System.Drawing.Size(152, 20);
            this.tb_LittleRollNo.TabIndex = 356;
            // 
            // tb_BigRollNo
            // 
            this.tb_BigRollNo.Location = new System.Drawing.Point(163, 496);
            this.tb_BigRollNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_BigRollNo.Name = "tb_BigRollNo";
            this.tb_BigRollNo.Size = new System.Drawing.Size(152, 20);
            this.tb_BigRollNo.TabIndex = 355;
            // 
            // tb_Desc
            // 
            this.tb_Desc.Location = new System.Drawing.Point(165, 435);
            this.tb_Desc.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Desc.Multiline = true;
            this.tb_Desc.Name = "tb_Desc";
            this.tb_Desc.Size = new System.Drawing.Size(808, 54);
            this.tb_Desc.TabIndex = 354;
            // 
            // tb_RecipeCode
            // 
            this.tb_RecipeCode.Location = new System.Drawing.Point(704, 175);
            this.tb_RecipeCode.Margin = new System.Windows.Forms.Padding(2);
            this.tb_RecipeCode.Name = "tb_RecipeCode";
            this.tb_RecipeCode.Size = new System.Drawing.Size(82, 20);
            this.tb_RecipeCode.TabIndex = 339;
            // 
            // tb_worker
            // 
            this.tb_worker.Location = new System.Drawing.Point(515, 366);
            this.tb_worker.Margin = new System.Windows.Forms.Padding(2);
            this.tb_worker.Name = "tb_worker";
            this.tb_worker.Size = new System.Drawing.Size(152, 20);
            this.tb_worker.TabIndex = 349;
            // 
            // tb_ManHour
            // 
            this.tb_ManHour.Location = new System.Drawing.Point(165, 366);
            this.tb_ManHour.Margin = new System.Windows.Forms.Padding(2);
            this.tb_ManHour.Name = "tb_ManHour";
            this.tb_ManHour.Size = new System.Drawing.Size(152, 20);
            this.tb_ManHour.TabIndex = 348;
            this.tb_ManHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tb_WorkNo
            // 
            this.tb_WorkNo.Location = new System.Drawing.Point(165, 329);
            this.tb_WorkNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_WorkNo.Name = "tb_WorkNo";
            this.tb_WorkNo.Size = new System.Drawing.Size(152, 20);
            this.tb_WorkNo.TabIndex = 346;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(100, 304);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(68, 17);
            this.label25.TabIndex = 377;
            this.label25.Text = "生产信息";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(648, 178);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(49, 13);
            this.label22.TabIndex = 375;
            this.label22.Text = "配方号：";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(755, 369);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(55, 13);
            this.label20.TabIndex = 373;
            this.label20.Text = "打包机号";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(462, 366);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(55, 13);
            this.label19.TabIndex = 372;
            this.label19.Text = "员工编号";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(100, 435);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(31, 13);
            this.label18.TabIndex = 371;
            this.label18.Text = "备注";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(100, 366);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 13);
            this.label16.TabIndex = 369;
            this.label16.Text = "生产工时";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(98, 331);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 13);
            this.label15.TabIndex = 368;
            this.label15.Text = "工单号";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(98, 496);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 13);
            this.label13.TabIndex = 366;
            this.label13.Text = "大卷编号";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(335, 496);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 13);
            this.label12.TabIndex = 365;
            this.label12.Text = "小卷编号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(803, 131);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 362;
            this.label2.Text = "宽度(mm)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(648, 74);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 414;
            this.label4.Text = "订单号：";
            // 
            // tb_OrderNo
            // 
            this.tb_OrderNo.Location = new System.Drawing.Point(701, 74);
            this.tb_OrderNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_OrderNo.Name = "tb_OrderNo";
            this.tb_OrderNo.Size = new System.Drawing.Size(85, 20);
            this.tb_OrderNo.TabIndex = 416;
            // 
            // tb_ProductWeight
            // 
            this.tb_ProductWeight.Location = new System.Drawing.Point(701, 127);
            this.tb_ProductWeight.Margin = new System.Windows.Forms.Padding(2);
            this.tb_ProductWeight.Name = "tb_ProductWeight";
            this.tb_ProductWeight.Size = new System.Drawing.Size(85, 20);
            this.tb_ProductWeight.TabIndex = 412;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(648, 134);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(37, 13);
            this.label21.TabIndex = 410;
            this.label21.Text = "基重：";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(475, 182);
            this.label38.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(61, 13);
            this.label38.TabIndex = 417;
            this.label38.Text = "生产日期：";
            // 
            // tb_ManufactureDate
            // 
            this.tb_ManufactureDate.Location = new System.Drawing.Point(549, 175);
            this.tb_ManufactureDate.Margin = new System.Windows.Forms.Padding(2);
            this.tb_ManufactureDate.Name = "tb_ManufactureDate";
            this.tb_ManufactureDate.Size = new System.Drawing.Size(95, 20);
            this.tb_ManufactureDate.TabIndex = 418;
            // 
            // tb_LittleRollCount
            // 
            this.tb_LittleRollCount.Location = new System.Drawing.Point(549, 126);
            this.tb_LittleRollCount.Margin = new System.Windows.Forms.Padding(2);
            this.tb_LittleRollCount.Name = "tb_LittleRollCount";
            this.tb_LittleRollCount.Size = new System.Drawing.Size(95, 20);
            this.tb_LittleRollCount.TabIndex = 415;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(475, 134);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 413;
            this.label8.Text = "卷数：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(475, 77);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 409;
            this.label9.Text = "材料名称：";
            // 
            // tb_MaterialName
            // 
            this.tb_MaterialName.Location = new System.Drawing.Point(549, 74);
            this.tb_MaterialName.Margin = new System.Windows.Forms.Padding(2);
            this.tb_MaterialName.Name = "tb_MaterialName";
            this.tb_MaterialName.Size = new System.Drawing.Size(95, 20);
            this.tb_MaterialName.TabIndex = 411;
            // 
            // tb_RawMaterialCode
            // 
            this.tb_RawMaterialCode.Location = new System.Drawing.Point(898, 77);
            this.tb_RawMaterialCode.Margin = new System.Windows.Forms.Padding(2);
            this.tb_RawMaterialCode.Name = "tb_RawMaterialCode";
            this.tb_RawMaterialCode.Size = new System.Drawing.Size(85, 20);
            this.tb_RawMaterialCode.TabIndex = 408;
            // 
            // tb_ProductLength
            // 
            this.tb_ProductLength.Location = new System.Drawing.Point(357, 175);
            this.tb_ProductLength.Margin = new System.Windows.Forms.Padding(2);
            this.tb_ProductLength.Name = "tb_ProductLength";
            this.tb_ProductLength.Size = new System.Drawing.Size(95, 20);
            this.tb_ProductLength.TabIndex = 406;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(803, 77);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 13);
            this.label10.TabIndex = 407;
            this.label10.Text = "原材料代码：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(294, 182);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 405;
            this.label11.Text = "长度：";
            // 
            // tb_PlateNo
            // 
            this.tb_PlateNo.Location = new System.Drawing.Point(357, 77);
            this.tb_PlateNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_PlateNo.Name = "tb_PlateNo";
            this.tb_PlateNo.Size = new System.Drawing.Size(95, 20);
            this.tb_PlateNo.TabIndex = 404;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(294, 81);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(49, 13);
            this.label14.TabIndex = 403;
            this.label14.Text = "产板号：";
            // 
            // tb_Roll_Weight
            // 
            this.tb_Roll_Weight.Location = new System.Drawing.Point(164, 175);
            this.tb_Roll_Weight.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Roll_Weight.Name = "tb_Roll_Weight";
            this.tb_Roll_Weight.Size = new System.Drawing.Size(105, 20);
            this.tb_Roll_Weight.TabIndex = 402;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(103, 182);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(37, 13);
            this.label17.TabIndex = 401;
            this.label17.Text = "重量：";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(103, 85);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(61, 13);
            this.label23.TabIndex = 394;
            this.label23.Text = "客户名称：";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(103, 134);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(55, 13);
            this.label24.TabIndex = 391;
            this.label24.Text = "产品代号";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(294, 134);
            this.label29.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(61, 13);
            this.label29.TabIndex = 395;
            this.label29.Text = "生产批号：";
            // 
            // PackEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 685);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tb_OrderNo);
            this.Controls.Add(this.tb_ProductWeight);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.tb_ManufactureDate);
            this.Controls.Add(this.tb_LittleRollCount);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tb_MaterialName);
            this.Controls.Add(this.tb_RawMaterialCode);
            this.Controls.Add(this.tb_ProductLength);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.tb_PlateNo);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.tb_Roll_Weight);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tb_BatchNo);
            this.Controls.Add(this.cb_WorkClass);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cb_WorkTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cb_ProductCode);
            this.Controls.Add(this.tb_PackMachineNo);
            this.Controls.Add(this.tb_Width);
            this.Controls.Add(this.tb_CustomerName);
            this.Controls.Add(this.tb_LittleRollNo);
            this.Controls.Add(this.tb_BigRollNo);
            this.Controls.Add(this.tb_Desc);
            this.Controls.Add(this.tb_RecipeCode);
            this.Controls.Add(this.tb_worker);
            this.Controls.Add(this.tb_ManHour);
            this.Controls.Add(this.tb_WorkNo);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.bt_Cancel);
            this.Controls.Add(this.bt_Printing);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PackEditorForm";
            this.Text = "打包单修改";
            this.Load += new System.EventHandler(this.PackEditorForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_Save;
        private System.Windows.Forms.Button bt_Cancel;
        private System.Windows.Forms.Button bt_Printing;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_BatchNo;
        private System.Windows.Forms.ComboBox cb_WorkClass;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cb_WorkTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cb_ProductCode;
        private System.Windows.Forms.TextBox tb_PackMachineNo;
        private System.Windows.Forms.TextBox tb_Width;
        private System.Windows.Forms.TextBox tb_CustomerName;
        private System.Windows.Forms.TextBox tb_LittleRollNo;
        private System.Windows.Forms.TextBox tb_BigRollNo;
        private System.Windows.Forms.TextBox tb_Desc;
        private System.Windows.Forms.TextBox tb_RecipeCode;
        private System.Windows.Forms.TextBox tb_worker;
        private System.Windows.Forms.TextBox tb_ManHour;
        private System.Windows.Forms.TextBox tb_WorkNo;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_OrderNo;
        private System.Windows.Forms.TextBox tb_ProductWeight;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox tb_ManufactureDate;
        private System.Windows.Forms.TextBox tb_LittleRollCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tb_MaterialName;
        private System.Windows.Forms.TextBox tb_RawMaterialCode;
        private System.Windows.Forms.TextBox tb_ProductLength;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tb_PlateNo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tb_Roll_Weight;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label29;
    }
}