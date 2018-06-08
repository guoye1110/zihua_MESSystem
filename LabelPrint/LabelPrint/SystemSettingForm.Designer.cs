namespace LabelPrint
{
    partial class SystemSettingForm
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
            this.bt_cancel = new System.Windows.Forms.Button();
            this.bt_ok = new System.Windows.Forms.Button();
            this.tabpage_manage = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rb_LiuYan7 = new System.Windows.Forms.RadioButton();
            this.rb_LiuYan6 = new System.Windows.Forms.RadioButton();
            this.rb_Recovery3 = new System.Windows.Forms.RadioButton();
            this.rb_Recovery2 = new System.Windows.Forms.RadioButton();
            this.rb_Print5 = new System.Windows.Forms.RadioButton();
            this.rb_Print4 = new System.Windows.Forms.RadioButton();
            this.rb_LiuYan5 = new System.Windows.Forms.RadioButton();
            this.rb_pack2 = new System.Windows.Forms.RadioButton();
            this.rb_Cut5 = new System.Windows.Forms.RadioButton();
            this.rb_Cut4 = new System.Windows.Forms.RadioButton();
            this.rb_Cut3 = new System.Windows.Forms.RadioButton();
            this.rb_Cut2 = new System.Windows.Forms.RadioButton();
            this.rb_Print3 = new System.Windows.Forms.RadioButton();
            this.rb_Print2 = new System.Windows.Forms.RadioButton();
            this.rb_LiuYan4 = new System.Windows.Forms.RadioButton();
            this.rb_LiuYan3 = new System.Windows.Forms.RadioButton();
            this.rb_LiuYan2 = new System.Windows.Forms.RadioButton();
            this.rb_pack1 = new System.Windows.Forms.RadioButton();
            this.rb_Recovery1 = new System.Windows.Forms.RadioButton();
            this.rb_QualityCheck = new System.Windows.Forms.RadioButton();
            this.rb_Print1 = new System.Windows.Forms.RadioButton();
            this.rb_Cut1 = new System.Windows.Forms.RadioButton();
            this.rb_LiuYan1 = new System.Windows.Forms.RadioButton();
            this.rb_OutBounding = new System.Windows.Forms.RadioButton();
            this.tabpage_printer = new System.Windows.Forms.TabPage();
            this.cb_Printer = new System.Windows.Forms.ComboBox();
            this.cb_thermalPrinter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tabpage_communication = new System.Windows.Forms.TabPage();
            this.tb_ServerIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabpage_serial = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.cb_BoudRate = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cb_DataBits = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cb_Parity = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cb_StopBits = new System.Windows.Forms.ComboBox();
            this.cb_SerialPort = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabpage_Label = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb_label = new System.Windows.Forms.ListBox();
            this.tab_Setting = new System.Windows.Forms.TabControl();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.sm_SerialPort = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabpage_manage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabpage_printer.SuspendLayout();
            this.tabpage_communication.SuspendLayout();
            this.tabpage_serial.SuspendLayout();
            this.tabpage_Label.SuspendLayout();
            this.tab_Setting.SuspendLayout();
            this.SuspendLayout();
            // 
            // bt_cancel
            // 
            this.bt_cancel.Location = new System.Drawing.Point(330, 565);
            this.bt_cancel.Name = "bt_cancel";
            this.bt_cancel.Size = new System.Drawing.Size(114, 45);
            this.bt_cancel.TabIndex = 30;
            this.bt_cancel.Text = "取消";
            this.bt_cancel.UseVisualStyleBackColor = true;
            this.bt_cancel.Click += new System.EventHandler(this.bt_cancel_Click_1);
            // 
            // bt_ok
            // 
            this.bt_ok.Location = new System.Drawing.Point(559, 565);
            this.bt_ok.Name = "bt_ok";
            this.bt_ok.Size = new System.Drawing.Size(114, 45);
            this.bt_ok.TabIndex = 31;
            this.bt_ok.Text = "确定";
            this.bt_ok.UseVisualStyleBackColor = true;
            this.bt_ok.Click += new System.EventHandler(this.bt_ok_Click_1);
            // 
            // tabpage_manage
            // 
            this.tabpage_manage.Controls.Add(this.label14);
            this.tabpage_manage.Controls.Add(this.groupBox1);
            this.tabpage_manage.Location = new System.Drawing.Point(4, 27);
            this.tabpage_manage.Name = "tabpage_manage";
            this.tabpage_manage.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabpage_manage.Size = new System.Drawing.Size(989, 485);
            this.tabpage_manage.TabIndex = 1;
            this.tabpage_manage.Text = "机台管理";
            this.tabpage_manage.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(313, 43);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(413, 37);
            this.label14.TabIndex = 1;
            this.label14.Text = "本机的标签打印软件将用于";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton4);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.rb_LiuYan7);
            this.groupBox1.Controls.Add(this.rb_LiuYan6);
            this.groupBox1.Controls.Add(this.rb_Recovery3);
            this.groupBox1.Controls.Add(this.rb_Recovery2);
            this.groupBox1.Controls.Add(this.rb_Print5);
            this.groupBox1.Controls.Add(this.rb_Print4);
            this.groupBox1.Controls.Add(this.rb_LiuYan5);
            this.groupBox1.Controls.Add(this.rb_pack2);
            this.groupBox1.Controls.Add(this.rb_Cut5);
            this.groupBox1.Controls.Add(this.rb_Cut4);
            this.groupBox1.Controls.Add(this.rb_Cut3);
            this.groupBox1.Controls.Add(this.rb_Cut2);
            this.groupBox1.Controls.Add(this.rb_Print3);
            this.groupBox1.Controls.Add(this.rb_Print2);
            this.groupBox1.Controls.Add(this.rb_LiuYan4);
            this.groupBox1.Controls.Add(this.rb_LiuYan3);
            this.groupBox1.Controls.Add(this.rb_LiuYan2);
            this.groupBox1.Controls.Add(this.rb_pack1);
            this.groupBox1.Controls.Add(this.rb_Recovery1);
            this.groupBox1.Controls.Add(this.rb_QualityCheck);
            this.groupBox1.Controls.Add(this.rb_Print1);
            this.groupBox1.Controls.Add(this.rb_Cut1);
            this.groupBox1.Controls.Add(this.rb_LiuYan1);
            this.groupBox1.Controls.Add(this.rb_OutBounding);
            this.groupBox1.Location = new System.Drawing.Point(41, 98);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(908, 358);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // rb_LiuYan7
            // 
            this.rb_LiuYan7.AutoSize = true;
            this.rb_LiuYan7.Location = new System.Drawing.Point(147, 316);
            this.rb_LiuYan7.Name = "rb_LiuYan7";
            this.rb_LiuYan7.Size = new System.Drawing.Size(94, 22);
            this.rb_LiuYan7.TabIndex = 22;
            this.rb_LiuYan7.TabStop = true;
            this.rb_LiuYan7.Text = "流延设备7";
            this.rb_LiuYan7.UseVisualStyleBackColor = true;
            this.rb_LiuYan7.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_LiuYan6
            // 
            this.rb_LiuYan6.AutoSize = true;
            this.rb_LiuYan6.Location = new System.Drawing.Point(147, 267);
            this.rb_LiuYan6.Name = "rb_LiuYan6";
            this.rb_LiuYan6.Size = new System.Drawing.Size(94, 22);
            this.rb_LiuYan6.TabIndex = 21;
            this.rb_LiuYan6.TabStop = true;
            this.rb_LiuYan6.Text = "流延设备6";
            this.rb_LiuYan6.UseVisualStyleBackColor = true;
            this.rb_LiuYan6.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Recovery3
            // 
            this.rb_Recovery3.AutoSize = true;
            this.rb_Recovery3.Location = new System.Drawing.Point(531, 120);
            this.rb_Recovery3.Name = "rb_Recovery3";
            this.rb_Recovery3.Size = new System.Drawing.Size(109, 22);
            this.rb_Recovery3.TabIndex = 20;
            this.rb_Recovery3.TabStop = true;
            this.rb_Recovery3.Text = "再造料设备3";
            this.rb_Recovery3.UseVisualStyleBackColor = true;
            this.rb_Recovery3.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Recovery2
            // 
            this.rb_Recovery2.AutoSize = true;
            this.rb_Recovery2.Location = new System.Drawing.Point(531, 72);
            this.rb_Recovery2.Name = "rb_Recovery2";
            this.rb_Recovery2.Size = new System.Drawing.Size(109, 22);
            this.rb_Recovery2.TabIndex = 19;
            this.rb_Recovery2.TabStop = true;
            this.rb_Recovery2.Text = "再造料设备2";
            this.rb_Recovery2.UseVisualStyleBackColor = true;
            this.rb_Recovery2.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Print5
            // 
            this.rb_Print5.AutoSize = true;
            this.rb_Print5.Location = new System.Drawing.Point(267, 217);
            this.rb_Print5.Name = "rb_Print5";
            this.rb_Print5.Size = new System.Drawing.Size(94, 22);
            this.rb_Print5.TabIndex = 18;
            this.rb_Print5.TabStop = true;
            this.rb_Print5.Text = "印刷设备5";
            this.rb_Print5.UseVisualStyleBackColor = true;
            this.rb_Print5.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Print4
            // 
            this.rb_Print4.AutoSize = true;
            this.rb_Print4.Location = new System.Drawing.Point(267, 169);
            this.rb_Print4.Name = "rb_Print4";
            this.rb_Print4.Size = new System.Drawing.Size(94, 22);
            this.rb_Print4.TabIndex = 17;
            this.rb_Print4.TabStop = true;
            this.rb_Print4.Text = "印刷设备4";
            this.rb_Print4.UseVisualStyleBackColor = true;
            this.rb_Print4.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_LiuYan5
            // 
            this.rb_LiuYan5.AutoSize = true;
            this.rb_LiuYan5.Location = new System.Drawing.Point(147, 219);
            this.rb_LiuYan5.Name = "rb_LiuYan5";
            this.rb_LiuYan5.Size = new System.Drawing.Size(94, 22);
            this.rb_LiuYan5.TabIndex = 10;
            this.rb_LiuYan5.TabStop = true;
            this.rb_LiuYan5.Text = "流延设备5";
            this.rb_LiuYan5.UseVisualStyleBackColor = true;
            this.rb_LiuYan5.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_pack2
            // 
            this.rb_pack2.AutoSize = true;
            this.rb_pack2.Location = new System.Drawing.Point(792, 72);
            this.rb_pack2.Name = "rb_pack2";
            this.rb_pack2.Size = new System.Drawing.Size(64, 22);
            this.rb_pack2.TabIndex = 16;
            this.rb_pack2.TabStop = true;
            this.rb_pack2.Text = "打包2";
            this.rb_pack2.UseVisualStyleBackColor = true;
            this.rb_pack2.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Cut5
            // 
            this.rb_Cut5.AutoSize = true;
            this.rb_Cut5.Location = new System.Drawing.Point(402, 217);
            this.rb_Cut5.Name = "rb_Cut5";
            this.rb_Cut5.Size = new System.Drawing.Size(94, 22);
            this.rb_Cut5.TabIndex = 15;
            this.rb_Cut5.TabStop = true;
            this.rb_Cut5.Text = "分切设备5";
            this.rb_Cut5.UseVisualStyleBackColor = true;
            this.rb_Cut5.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Cut4
            // 
            this.rb_Cut4.AutoSize = true;
            this.rb_Cut4.Location = new System.Drawing.Point(402, 169);
            this.rb_Cut4.Name = "rb_Cut4";
            this.rb_Cut4.Size = new System.Drawing.Size(94, 22);
            this.rb_Cut4.TabIndex = 14;
            this.rb_Cut4.TabStop = true;
            this.rb_Cut4.Text = "分切设备4";
            this.rb_Cut4.UseVisualStyleBackColor = true;
            this.rb_Cut4.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Cut3
            // 
            this.rb_Cut3.AutoSize = true;
            this.rb_Cut3.Location = new System.Drawing.Point(402, 120);
            this.rb_Cut3.Name = "rb_Cut3";
            this.rb_Cut3.Size = new System.Drawing.Size(94, 22);
            this.rb_Cut3.TabIndex = 13;
            this.rb_Cut3.TabStop = true;
            this.rb_Cut3.Text = "分切设备3";
            this.rb_Cut3.UseVisualStyleBackColor = true;
            this.rb_Cut3.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Cut2
            // 
            this.rb_Cut2.AutoSize = true;
            this.rb_Cut2.Location = new System.Drawing.Point(402, 72);
            this.rb_Cut2.Name = "rb_Cut2";
            this.rb_Cut2.Size = new System.Drawing.Size(94, 22);
            this.rb_Cut2.TabIndex = 12;
            this.rb_Cut2.TabStop = true;
            this.rb_Cut2.Text = "分切设备2";
            this.rb_Cut2.UseVisualStyleBackColor = true;
            this.rb_Cut2.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Print3
            // 
            this.rb_Print3.AutoSize = true;
            this.rb_Print3.Location = new System.Drawing.Point(270, 120);
            this.rb_Print3.Name = "rb_Print3";
            this.rb_Print3.Size = new System.Drawing.Size(94, 22);
            this.rb_Print3.TabIndex = 11;
            this.rb_Print3.TabStop = true;
            this.rb_Print3.Text = "印刷设备3";
            this.rb_Print3.UseVisualStyleBackColor = true;
            this.rb_Print3.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Print2
            // 
            this.rb_Print2.AutoSize = true;
            this.rb_Print2.Location = new System.Drawing.Point(270, 72);
            this.rb_Print2.Name = "rb_Print2";
            this.rb_Print2.Size = new System.Drawing.Size(94, 22);
            this.rb_Print2.TabIndex = 10;
            this.rb_Print2.TabStop = true;
            this.rb_Print2.Text = "印刷设备2";
            this.rb_Print2.UseVisualStyleBackColor = true;
            this.rb_Print2.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_LiuYan4
            // 
            this.rb_LiuYan4.AutoSize = true;
            this.rb_LiuYan4.Location = new System.Drawing.Point(147, 170);
            this.rb_LiuYan4.Name = "rb_LiuYan4";
            this.rb_LiuYan4.Size = new System.Drawing.Size(94, 22);
            this.rb_LiuYan4.TabIndex = 9;
            this.rb_LiuYan4.TabStop = true;
            this.rb_LiuYan4.Text = "流延设备4";
            this.rb_LiuYan4.UseVisualStyleBackColor = true;
            this.rb_LiuYan4.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_LiuYan3
            // 
            this.rb_LiuYan3.AutoSize = true;
            this.rb_LiuYan3.Location = new System.Drawing.Point(147, 122);
            this.rb_LiuYan3.Name = "rb_LiuYan3";
            this.rb_LiuYan3.Size = new System.Drawing.Size(94, 22);
            this.rb_LiuYan3.TabIndex = 8;
            this.rb_LiuYan3.TabStop = true;
            this.rb_LiuYan3.Text = "流延设备3";
            this.rb_LiuYan3.UseVisualStyleBackColor = true;
            this.rb_LiuYan3.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_LiuYan2
            // 
            this.rb_LiuYan2.AutoSize = true;
            this.rb_LiuYan2.Location = new System.Drawing.Point(147, 73);
            this.rb_LiuYan2.Name = "rb_LiuYan2";
            this.rb_LiuYan2.Size = new System.Drawing.Size(94, 22);
            this.rb_LiuYan2.TabIndex = 7;
            this.rb_LiuYan2.TabStop = true;
            this.rb_LiuYan2.Text = "流延设备2";
            this.rb_LiuYan2.UseVisualStyleBackColor = true;
            this.rb_LiuYan2.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_pack1
            // 
            this.rb_pack1.AutoSize = true;
            this.rb_pack1.Location = new System.Drawing.Point(792, 25);
            this.rb_pack1.Name = "rb_pack1";
            this.rb_pack1.Size = new System.Drawing.Size(64, 22);
            this.rb_pack1.TabIndex = 6;
            this.rb_pack1.TabStop = true;
            this.rb_pack1.Text = "打包1";
            this.rb_pack1.UseVisualStyleBackColor = true;
            this.rb_pack1.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Recovery1
            // 
            this.rb_Recovery1.AutoSize = true;
            this.rb_Recovery1.Location = new System.Drawing.Point(531, 24);
            this.rb_Recovery1.Name = "rb_Recovery1";
            this.rb_Recovery1.Size = new System.Drawing.Size(109, 22);
            this.rb_Recovery1.TabIndex = 5;
            this.rb_Recovery1.TabStop = true;
            this.rb_Recovery1.Text = "再造料设备1";
            this.rb_Recovery1.UseVisualStyleBackColor = true;
            this.rb_Recovery1.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            this.rb_Recovery1.Click += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_QualityCheck
            // 
            this.rb_QualityCheck.AutoSize = true;
            this.rb_QualityCheck.Location = new System.Drawing.Point(676, 24);
            this.rb_QualityCheck.Name = "rb_QualityCheck";
            this.rb_QualityCheck.Size = new System.Drawing.Size(86, 22);
            this.rb_QualityCheck.TabIndex = 4;
            this.rb_QualityCheck.TabStop = true;
            this.rb_QualityCheck.Text = "质量检验";
            this.rb_QualityCheck.UseVisualStyleBackColor = true;
            this.rb_QualityCheck.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            this.rb_QualityCheck.Click += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Print1
            // 
            this.rb_Print1.AutoSize = true;
            this.rb_Print1.Location = new System.Drawing.Point(270, 24);
            this.rb_Print1.Name = "rb_Print1";
            this.rb_Print1.Size = new System.Drawing.Size(94, 22);
            this.rb_Print1.TabIndex = 3;
            this.rb_Print1.TabStop = true;
            this.rb_Print1.Text = "印刷设备1";
            this.rb_Print1.UseVisualStyleBackColor = true;
            this.rb_Print1.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            this.rb_Print1.Click += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_Cut1
            // 
            this.rb_Cut1.AutoSize = true;
            this.rb_Cut1.Location = new System.Drawing.Point(402, 24);
            this.rb_Cut1.Name = "rb_Cut1";
            this.rb_Cut1.Size = new System.Drawing.Size(94, 22);
            this.rb_Cut1.TabIndex = 2;
            this.rb_Cut1.TabStop = true;
            this.rb_Cut1.Text = "分切设备1";
            this.rb_Cut1.UseVisualStyleBackColor = true;
            this.rb_Cut1.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            this.rb_Cut1.Click += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_LiuYan1
            // 
            this.rb_LiuYan1.AutoSize = true;
            this.rb_LiuYan1.Location = new System.Drawing.Point(147, 25);
            this.rb_LiuYan1.Name = "rb_LiuYan1";
            this.rb_LiuYan1.Size = new System.Drawing.Size(94, 22);
            this.rb_LiuYan1.TabIndex = 1;
            this.rb_LiuYan1.TabStop = true;
            this.rb_LiuYan1.Text = "流延设备1";
            this.rb_LiuYan1.UseVisualStyleBackColor = true;
            this.rb_LiuYan1.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            this.rb_LiuYan1.Click += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // rb_OutBounding
            // 
            this.rb_OutBounding.AutoSize = true;
            this.rb_OutBounding.Location = new System.Drawing.Point(36, 24);
            this.rb_OutBounding.Name = "rb_OutBounding";
            this.rb_OutBounding.Size = new System.Drawing.Size(86, 22);
            this.rb_OutBounding.TabIndex = 0;
            this.rb_OutBounding.TabStop = true;
            this.rb_OutBounding.Text = "原料出库";
            this.rb_OutBounding.UseVisualStyleBackColor = true;
            this.rb_OutBounding.CheckedChanged += new System.EventHandler(this.rb_OutBounding_CheckedChanged);
            // 
            // tabpage_printer
            // 
            this.tabpage_printer.Controls.Add(this.cb_Printer);
            this.tabpage_printer.Controls.Add(this.cb_thermalPrinter);
            this.tabpage_printer.Controls.Add(this.label1);
            this.tabpage_printer.Controls.Add(this.label11);
            this.tabpage_printer.Location = new System.Drawing.Point(4, 27);
            this.tabpage_printer.Name = "tabpage_printer";
            this.tabpage_printer.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabpage_printer.Size = new System.Drawing.Size(989, 485);
            this.tabpage_printer.TabIndex = 4;
            this.tabpage_printer.Text = "打印机设定";
            this.tabpage_printer.UseVisualStyleBackColor = true;
            // 
            // cb_Printer
            // 
            this.cb_Printer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Printer.FormattingEnabled = true;
            this.cb_Printer.Location = new System.Drawing.Point(177, 105);
            this.cb_Printer.Name = "cb_Printer";
            this.cb_Printer.Size = new System.Drawing.Size(390, 26);
            this.cb_Printer.TabIndex = 38;
            this.cb_Printer.SelectedIndexChanged += new System.EventHandler(this.cb_Printer_SelectedIndexChanged);
            // 
            // cb_thermalPrinter
            // 
            this.cb_thermalPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_thermalPrinter.FormattingEnabled = true;
            this.cb_thermalPrinter.Location = new System.Drawing.Point(177, 42);
            this.cb_thermalPrinter.Name = "cb_thermalPrinter";
            this.cb_thermalPrinter.Size = new System.Drawing.Size(390, 26);
            this.cb_thermalPrinter.TabIndex = 37;
            this.cb_thermalPrinter.SelectedIndexChanged += new System.EventHandler(this.cb_thermalPrinter_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(60, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 18);
            this.label1.TabIndex = 35;
            this.label1.Text = "热敏打印机";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(60, 109);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 18);
            this.label11.TabIndex = 33;
            this.label11.Text = "打印机";
            // 
            // tabpage_communication
            // 
            this.tabpage_communication.Controls.Add(this.tb_ServerIP);
            this.tabpage_communication.Controls.Add(this.label2);
            this.tabpage_communication.Location = new System.Drawing.Point(4, 27);
            this.tabpage_communication.Name = "tabpage_communication";
            this.tabpage_communication.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabpage_communication.Size = new System.Drawing.Size(989, 485);
            this.tabpage_communication.TabIndex = 6;
            this.tabpage_communication.Text = "通讯设定";
            this.tabpage_communication.UseVisualStyleBackColor = true;
            // 
            // tb_ServerIP
            // 
            this.tb_ServerIP.Location = new System.Drawing.Point(255, 68);
            this.tb_ServerIP.Name = "tb_ServerIP";
            this.tb_ServerIP.Size = new System.Drawing.Size(182, 24);
            this.tb_ServerIP.TabIndex = 34;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 18);
            this.label2.TabIndex = 33;
            this.label2.Text = "服务器IP地址";
            // 
            // tabpage_serial
            // 
            this.tabpage_serial.Controls.Add(this.sm_SerialPort);
            this.tabpage_serial.Controls.Add(this.label6);
            this.tabpage_serial.Controls.Add(this.label10);
            this.tabpage_serial.Controls.Add(this.cb_BoudRate);
            this.tabpage_serial.Controls.Add(this.label9);
            this.tabpage_serial.Controls.Add(this.cb_DataBits);
            this.tabpage_serial.Controls.Add(this.label8);
            this.tabpage_serial.Controls.Add(this.cb_Parity);
            this.tabpage_serial.Controls.Add(this.label7);
            this.tabpage_serial.Controls.Add(this.cb_StopBits);
            this.tabpage_serial.Controls.Add(this.cb_SerialPort);
            this.tabpage_serial.Controls.Add(this.label3);
            this.tabpage_serial.Location = new System.Drawing.Point(4, 27);
            this.tabpage_serial.Name = "tabpage_serial";
            this.tabpage_serial.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabpage_serial.Size = new System.Drawing.Size(989, 485);
            this.tabpage_serial.TabIndex = 0;
            this.tabpage_serial.Text = "设备串口设定";
            this.tabpage_serial.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(82, 255);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 18);
            this.label10.TabIndex = 14;
            this.label10.Text = "停止位";
            // 
            // cb_BoudRate
            // 
            this.cb_BoudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_BoudRate.FormattingEnabled = true;
            this.cb_BoudRate.Location = new System.Drawing.Point(231, 107);
            this.cb_BoudRate.Name = "cb_BoudRate";
            this.cb_BoudRate.Size = new System.Drawing.Size(145, 26);
            this.cb_BoudRate.TabIndex = 33;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(82, 209);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 18);
            this.label9.TabIndex = 13;
            this.label9.Text = "奇偶校验";
            // 
            // cb_DataBits
            // 
            this.cb_DataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_DataBits.FormattingEnabled = true;
            this.cb_DataBits.Location = new System.Drawing.Point(231, 155);
            this.cb_DataBits.Name = "cb_DataBits";
            this.cb_DataBits.Size = new System.Drawing.Size(145, 26);
            this.cb_DataBits.TabIndex = 34;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(82, 159);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 18);
            this.label8.TabIndex = 12;
            this.label8.Text = "数据位";
            // 
            // cb_Parity
            // 
            this.cb_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Parity.FormattingEnabled = true;
            this.cb_Parity.Location = new System.Drawing.Point(231, 201);
            this.cb_Parity.Name = "cb_Parity";
            this.cb_Parity.Size = new System.Drawing.Size(145, 26);
            this.cb_Parity.TabIndex = 35;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(82, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 18);
            this.label7.TabIndex = 11;
            this.label7.Text = "波特率";
            // 
            // cb_StopBits
            // 
            this.cb_StopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_StopBits.FormattingEnabled = true;
            this.cb_StopBits.Location = new System.Drawing.Point(231, 246);
            this.cb_StopBits.Name = "cb_StopBits";
            this.cb_StopBits.Size = new System.Drawing.Size(145, 26);
            this.cb_StopBits.TabIndex = 36;
            // 
            // cb_SerialPort
            // 
            this.cb_SerialPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_SerialPort.FormattingEnabled = true;
            this.cb_SerialPort.Location = new System.Drawing.Point(231, 69);
            this.cb_SerialPort.Name = "cb_SerialPort";
            this.cb_SerialPort.Size = new System.Drawing.Size(145, 26);
            this.cb_SerialPort.TabIndex = 32;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(82, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 18);
            this.label3.TabIndex = 4;
            this.label3.Text = "磅秤串口通讯";
            // 
            // tabpage_Label
            // 
            this.tabpage_Label.Controls.Add(this.label4);
            this.tabpage_Label.Controls.Add(this.label5);
            this.tabpage_Label.Controls.Add(this.panel1);
            this.tabpage_Label.Controls.Add(this.lb_label);
            this.tabpage_Label.Location = new System.Drawing.Point(4, 22);
            this.tabpage_Label.Name = "tabpage_Label";
            this.tabpage_Label.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabpage_Label.Size = new System.Drawing.Size(931, 481);
            this.tabpage_Label.TabIndex = 3;
            this.tabpage_Label.Text = "标签设定";
            this.tabpage_Label.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(177, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 18);
            this.label4.TabIndex = 9;
            this.label4.Text = "标签";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 18);
            this.label5.TabIndex = 8;
            this.label5.Text = "请选择标签";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(182, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(639, 388);
            this.panel1.TabIndex = 7;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // lb_label
            // 
            this.lb_label.FormattingEnabled = true;
            this.lb_label.ItemHeight = 18;
            this.lb_label.Location = new System.Drawing.Point(12, 61);
            this.lb_label.Name = "lb_label";
            this.lb_label.Size = new System.Drawing.Size(138, 382);
            this.lb_label.TabIndex = 6;
            this.lb_label.SelectedIndexChanged += new System.EventHandler(this.lb_label_SelectedIndexChanged);
            // 
            // tab_Setting
            // 
            this.tab_Setting.Controls.Add(this.tabpage_Label);
            this.tab_Setting.Controls.Add(this.tabpage_serial);
            this.tab_Setting.Controls.Add(this.tabpage_communication);
            this.tab_Setting.Controls.Add(this.tabpage_printer);
            this.tab_Setting.Controls.Add(this.tabpage_manage);
            this.tab_Setting.Location = new System.Drawing.Point(10, 5);
            this.tab_Setting.Name = "tab_Setting";
            this.tab_Setting.SelectedIndex = 0;
            this.tab_Setting.Size = new System.Drawing.Size(997, 516);
            this.tab_Setting.TabIndex = 19;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(792, 120);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(64, 22);
            this.radioButton1.TabIndex = 23;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "打包3";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(792, 169);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(64, 22);
            this.radioButton2.TabIndex = 24;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "打包4";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(531, 170);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(109, 22);
            this.radioButton3.TabIndex = 25;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "再造料设备4";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(531, 219);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(109, 22);
            this.radioButton4.TabIndex = 26;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "再造料设备5";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // sm_SerialPort
            // 
            this.sm_SerialPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sm_SerialPort.FormattingEnabled = true;
            this.sm_SerialPort.Location = new System.Drawing.Point(654, 73);
            this.sm_SerialPort.Name = "sm_SerialPort";
            this.sm_SerialPort.Size = new System.Drawing.Size(145, 26);
            this.sm_SerialPort.TabIndex = 38;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(505, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 18);
            this.label6.TabIndex = 37;
            this.label6.Text = "扫描枪串口通讯";
            // 
            // SystemSettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1019, 650);
            this.Controls.Add(this.tab_Setting);
            this.Controls.Add(this.bt_cancel);
            this.Controls.Add(this.bt_ok);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "SystemSettingForm";
            this.Text = "系统设定";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SystemSettingForm_FormClosing);
            this.Load += new System.EventHandler(this.SystemSettingForm_Load);
            this.Leave += new System.EventHandler(this.SystemSettingForm_Leave);
            this.tabpage_manage.ResumeLayout(false);
            this.tabpage_manage.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabpage_printer.ResumeLayout(false);
            this.tabpage_printer.PerformLayout();
            this.tabpage_communication.ResumeLayout(false);
            this.tabpage_communication.PerformLayout();
            this.tabpage_serial.ResumeLayout(false);
            this.tabpage_serial.PerformLayout();
            this.tabpage_Label.ResumeLayout(false);
            this.tabpage_Label.PerformLayout();
            this.tab_Setting.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button bt_cancel;
        private System.Windows.Forms.Button bt_ok;
        private System.Windows.Forms.TabPage tabpage_manage;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rb_LiuYan5;
        private System.Windows.Forms.RadioButton rb_pack2;
        private System.Windows.Forms.RadioButton rb_Cut5;
        private System.Windows.Forms.RadioButton rb_Cut4;
        private System.Windows.Forms.RadioButton rb_Cut3;
        private System.Windows.Forms.RadioButton rb_Cut2;
        private System.Windows.Forms.RadioButton rb_Print3;
        private System.Windows.Forms.RadioButton rb_Print2;
        private System.Windows.Forms.RadioButton rb_LiuYan4;
        private System.Windows.Forms.RadioButton rb_LiuYan3;
        private System.Windows.Forms.RadioButton rb_LiuYan2;
        private System.Windows.Forms.RadioButton rb_pack1;
        private System.Windows.Forms.RadioButton rb_Recovery1;
        private System.Windows.Forms.RadioButton rb_QualityCheck;
        private System.Windows.Forms.RadioButton rb_Print1;
        private System.Windows.Forms.RadioButton rb_Cut1;
        private System.Windows.Forms.RadioButton rb_LiuYan1;
        private System.Windows.Forms.RadioButton rb_OutBounding;
        private System.Windows.Forms.TabPage tabpage_printer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabPage tabpage_communication;
        private System.Windows.Forms.TextBox tb_ServerIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabpage_serial;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cb_BoudRate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cb_DataBits;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cb_Parity;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cb_StopBits;
        private System.Windows.Forms.ComboBox cb_SerialPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabpage_Label;
        private System.Windows.Forms.TabControl tab_Setting;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lb_label;
        private System.Windows.Forms.ComboBox cb_Printer;
        private System.Windows.Forms.ComboBox cb_thermalPrinter;
        private System.Windows.Forms.RadioButton rb_LiuYan7;
        private System.Windows.Forms.RadioButton rb_LiuYan6;
        private System.Windows.Forms.RadioButton rb_Recovery3;
        private System.Windows.Forms.RadioButton rb_Recovery2;
        private System.Windows.Forms.RadioButton rb_Print5;
        private System.Windows.Forms.RadioButton rb_Print4;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.ComboBox sm_SerialPort;
        private System.Windows.Forms.Label label6;
    }
}