﻿namespace LabelPrint
{
    partial class QAForm
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
            this.cb_ProductCode = new System.Windows.Forms.ComboBox();
            this.bt_Printing = new System.Windows.Forms.Button();
            this.tb_BatchNo = new System.Windows.Forms.TextBox();
            this.tb_CustomerName = new System.Windows.Forms.TextBox();
            this.tx_ScaleSerialPort = new System.Windows.Forms.Label();
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
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_DateTime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lb_InputBarcode = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lb_OutputBarcode = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_ProductCode
            // 
            this.cb_ProductCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductCode.FormattingEnabled = true;
            this.cb_ProductCode.Location = new System.Drawing.Point(126, 65);
            this.cb_ProductCode.Name = "cb_ProductCode";
            this.cb_ProductCode.Size = new System.Drawing.Size(151, 26);
            this.cb_ProductCode.TabIndex = 181;
            this.cb_ProductCode.SelectedIndexChanged += new System.EventHandler(this.cb_ProductCode_SelectedIndexChanged);
            // 
            // bt_Printing
            // 
            this.bt_Printing.Location = new System.Drawing.Point(1164, 879);
            this.bt_Printing.Name = "bt_Printing";
            this.bt_Printing.Size = new System.Drawing.Size(144, 44);
            this.bt_Printing.TabIndex = 159;
            this.bt_Printing.Text = "质检标签打印";
            this.bt_Printing.UseVisualStyleBackColor = true;
            this.bt_Printing.Click += new System.EventHandler(this.bt_Printing_Click);
            // 
            // tb_BatchNo
            // 
            this.tb_BatchNo.Location = new System.Drawing.Point(1107, 55);
            this.tb_BatchNo.Name = "tb_BatchNo";
            this.tb_BatchNo.Size = new System.Drawing.Size(156, 24);
            this.tb_BatchNo.TabIndex = 143;
            // 
            // tb_CustomerName
            // 
            this.tb_CustomerName.Location = new System.Drawing.Point(1107, 134);
            this.tb_CustomerName.Name = "tb_CustomerName";
            this.tb_CustomerName.Size = new System.Drawing.Size(156, 24);
            this.tb_CustomerName.TabIndex = 140;
            // 
            // tx_ScaleSerialPort
            // 
            this.tx_ScaleSerialPort.AutoSize = true;
            this.tx_ScaleSerialPort.Location = new System.Drawing.Point(1226, 97);
            this.tx_ScaleSerialPort.Name = "tx_ScaleSerialPort";
            this.tx_ScaleSerialPort.Size = new System.Drawing.Size(0, 18);
            this.tx_ScaleSerialPort.TabIndex = 136;
            // 
            // cb_ProductQuality
            // 
            this.cb_ProductQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductQuality.FormattingEnabled = true;
            this.cb_ProductQuality.Location = new System.Drawing.Point(672, 169);
            this.cb_ProductQuality.Name = "cb_ProductQuality";
            this.cb_ProductQuality.Size = new System.Drawing.Size(139, 26);
            this.cb_ProductQuality.TabIndex = 135;
            // 
            // lb_ProductQulity
            // 
            this.lb_ProductQulity.AutoSize = true;
            this.lb_ProductQulity.Location = new System.Drawing.Point(586, 169);
            this.lb_ProductQulity.Name = "lb_ProductQulity";
            this.lb_ProductQulity.Size = new System.Drawing.Size(68, 18);
            this.lb_ProductQulity.TabIndex = 134;
            this.lb_ProductQulity.Text = "产品质量";
            // 
            // cb_ProductState
            // 
            this.cb_ProductState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_ProductState.FormattingEnabled = true;
            this.cb_ProductState.Location = new System.Drawing.Point(253, 631);
            this.cb_ProductState.Name = "cb_ProductState";
            this.cb_ProductState.Size = new System.Drawing.Size(226, 26);
            this.cb_ProductState.TabIndex = 131;
            this.cb_ProductState.SelectedValueChanged += new System.EventHandler(this.cb_ProductState_SelectedValueChanged);
            // 
            // tb_LittleRollNo
            // 
            this.tb_LittleRollNo.Location = new System.Drawing.Point(669, 320);
            this.tb_LittleRollNo.Name = "tb_LittleRollNo";
            this.tb_LittleRollNo.Size = new System.Drawing.Size(142, 24);
            this.tb_LittleRollNo.TabIndex = 130;
            // 
            // tb_BigRollNo
            // 
            this.tb_BigRollNo.Location = new System.Drawing.Point(114, 320);
            this.tb_BigRollNo.Name = "tb_BigRollNo";
            this.tb_BigRollNo.Size = new System.Drawing.Size(226, 24);
            this.tb_BigRollNo.TabIndex = 129;
            // 
            // tb_Desc
            // 
            this.tb_Desc.Location = new System.Drawing.Point(253, 678);
            this.tb_Desc.Multiline = true;
            this.tb_Desc.Name = "tb_Desc";
            this.tb_Desc.Size = new System.Drawing.Size(702, 73);
            this.tb_Desc.TabIndex = 128;
            // 
            // tb_RecipeCode
            // 
            this.tb_RecipeCode.Location = new System.Drawing.Point(126, 140);
            this.tb_RecipeCode.Name = "tb_RecipeCode";
            this.tb_RecipeCode.Size = new System.Drawing.Size(151, 24);
            this.tb_RecipeCode.TabIndex = 125;
            // 
            // tb_worker
            // 
            this.tb_worker.Location = new System.Drawing.Point(672, 120);
            this.tb_worker.Name = "tb_worker";
            this.tb_worker.Size = new System.Drawing.Size(139, 24);
            this.tb_worker.TabIndex = 121;
            // 
            // tb_ManHour
            // 
            this.tb_ManHour.Location = new System.Drawing.Point(253, 583);
            this.tb_ManHour.Name = "tb_ManHour";
            this.tb_ManHour.Size = new System.Drawing.Size(226, 24);
            this.tb_ManHour.TabIndex = 120;
            this.tb_ManHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tb_WorkNo
            // 
            this.tb_WorkNo.Location = new System.Drawing.Point(253, 530);
            this.tb_WorkNo.Name = "tb_WorkNo";
            this.tb_WorkNo.Size = new System.Drawing.Size(226, 24);
            this.tb_WorkNo.TabIndex = 119;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(963, 61);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(76, 18);
            this.label27.TabIndex = 116;
            this.label27.Text = "生产批号：";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(963, 140);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(76, 18);
            this.label26.TabIndex = 115;
            this.label26.Text = "客户名称：";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(33, 144);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(61, 18);
            this.label22.TabIndex = 112;
            this.label22.Text = "配方号：";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(584, 120);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(68, 18);
            this.label19.TabIndex = 109;
            this.label19.Text = "员工编号";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(156, 678);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 18);
            this.label18.TabIndex = 108;
            this.label18.Text = "备注";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(156, 631);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(68, 18);
            this.label17.TabIndex = 107;
            this.label17.Text = "产品状态";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(156, 583);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(68, 18);
            this.label16.TabIndex = 106;
            this.label16.Text = "生产工时";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(152, 534);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 18);
            this.label15.TabIndex = 105;
            this.label15.Text = "工单号";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 320);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(68, 18);
            this.label13.TabIndex = 103;
            this.label13.Text = "大卷编号";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(560, 324);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(68, 18);
            this.label12.TabIndex = 102;
            this.label12.Text = "小卷编号";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 18);
            this.label1.TabIndex = 98;
            this.label1.Text = "产品代号";
            // 
            // tb_DateTime
            // 
            this.tb_DateTime.Location = new System.Drawing.Point(676, 68);
            this.tb_DateTime.Name = "tb_DateTime";
            this.tb_DateTime.Size = new System.Drawing.Size(136, 24);
            this.tb_DateTime.TabIndex = 186;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(586, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 18);
            this.label4.TabIndex = 185;
            this.label4.Text = "打印时间";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lb_InputBarcode);
            this.groupBox4.Location = new System.Drawing.Point(1210, 486);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(424, 125);
            this.groupBox4.TabIndex = 187;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "待检标签";
            // 
            // lb_InputBarcode
            // 
            this.lb_InputBarcode.AutoSize = true;
            this.lb_InputBarcode.Location = new System.Drawing.Point(36, 65);
            this.lb_InputBarcode.Name = "lb_InputBarcode";
            this.lb_InputBarcode.Size = new System.Drawing.Size(0, 18);
            this.lb_InputBarcode.TabIndex = 226;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lb_OutputBarcode);
            this.groupBox5.Location = new System.Drawing.Point(1216, 637);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(424, 125);
            this.groupBox5.TabIndex = 188;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "质检标签";
            // 
            // lb_OutputBarcode
            // 
            this.lb_OutputBarcode.AutoSize = true;
            this.lb_OutputBarcode.Location = new System.Drawing.Point(30, 61);
            this.lb_OutputBarcode.Name = "lb_OutputBarcode";
            this.lb_OutputBarcode.Size = new System.Drawing.Size(0, 18);
            this.lb_OutputBarcode.TabIndex = 227;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cb_ProductCode);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.tb_RecipeCode);
            this.groupBox3.Controls.Add(this.label27);
            this.groupBox3.Controls.Add(this.tb_BatchNo);
            this.groupBox3.Controls.Add(this.label26);
            this.groupBox3.Controls.Add(this.tb_CustomerName);
            this.groupBox3.Location = new System.Drawing.Point(144, 223);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1491, 217);
            this.groupBox3.TabIndex = 228;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "产品信息";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tb_worker);
            this.groupBox6.Controls.Add(this.label19);
            this.groupBox6.Controls.Add(this.lb_ProductQulity);
            this.groupBox6.Controls.Add(this.cb_ProductQuality);
            this.groupBox6.Controls.Add(this.tb_DateTime);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.tb_LittleRollNo);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.tb_BigRollNo);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Location = new System.Drawing.Point(144, 458);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(898, 396);
            this.groupBox6.TabIndex = 229;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "质检信息";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(606, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(449, 44);
            this.label2.TabIndex = 230;
            this.label2.Text = "紫华企业质保工序操作单";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(979, 933);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 231;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // QAForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1916, 1053);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.bt_Printing);
            this.Controls.Add(this.tx_ScaleSerialPort);
            this.Controls.Add(this.cb_ProductState);
            this.Controls.Add(this.tb_Desc);
            this.Controls.Add(this.tb_ManHour);
            this.Controls.Add(this.tb_WorkNo);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "QAForm";
            this.Text = "质量检验单";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QAForm_FormClosing);
            this.Load += new System.EventHandler(this.QAForm_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cb_ProductCode;
        private System.Windows.Forms.Button bt_Printing;
        private System.Windows.Forms.TextBox tb_BatchNo;
        private System.Windows.Forms.TextBox tb_CustomerName;
        private System.Windows.Forms.Label tx_ScaleSerialPort;
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
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_DateTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lb_InputBarcode;
        private System.Windows.Forms.Label lb_OutputBarcode;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}