namespace LabelPrint.EditorForms
{
    partial class RuKuEditorForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tb_Desc = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tb_WorkerNo = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cb_TargetMachineNo = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tb_wuliaoxinxi = new System.Windows.Forms.GroupBox();
            this.cb_LiaoCangNo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_RawMaterialCode = new System.Windows.Forms.ComboBox();
            this.tb_BenCiChuKuWeight = new System.Windows.Forms.TextBox();
            this.label64 = new System.Windows.Forms.Label();
            this.tb_YiChuKuWeight = new System.Windows.Forms.TextBox();
            this.label56 = new System.Windows.Forms.Label();
            this.tb_XuQiuWeight = new System.Windows.Forms.TextBox();
            this.label48 = new System.Windows.Forms.Label();
            this.tb_RawMaterialBachNo = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.tb_Time = new System.Windows.Forms.TextBox();
            this.tb_Date = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tb_wuliaoxinxi.SuspendLayout();
            this.SuspendLayout();
            // 
            // bt_Save
            // 
            this.bt_Save.Location = new System.Drawing.Point(1109, 660);
            this.bt_Save.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(104, 33);
            this.bt_Save.TabIndex = 192;
            this.bt_Save.Text = "确定";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // bt_Cancel
            // 
            this.bt_Cancel.Location = new System.Drawing.Point(930, 660);
            this.bt_Cancel.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Cancel.Name = "bt_Cancel";
            this.bt_Cancel.Size = new System.Drawing.Size(104, 33);
            this.bt_Cancel.TabIndex = 191;
            this.bt_Cancel.Text = "取消";
            this.bt_Cancel.UseVisualStyleBackColor = true;
            this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
            // 
            // bt_Printing
            // 
            this.bt_Printing.Location = new System.Drawing.Point(746, 660);
            this.bt_Printing.Margin = new System.Windows.Forms.Padding(2);
            this.bt_Printing.Name = "bt_Printing";
            this.bt_Printing.Size = new System.Drawing.Size(104, 33);
            this.bt_Printing.TabIndex = 190;
            this.bt_Printing.Text = "打印标签";
            this.bt_Printing.UseVisualStyleBackColor = true;
            this.bt_Printing.Click += new System.EventHandler(this.bt_Printing_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(591, 497);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(393, 95);
            this.groupBox4.TabIndex = 239;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "标签信息";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 18);
            this.label1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tb_Time);
            this.groupBox3.Controls.Add(this.tb_Date);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.tb_Desc);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.tb_WorkerNo);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(46, 497);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(453, 248);
            this.groupBox3.TabIndex = 238;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "生产信息";
            // 
            // tb_Desc
            // 
            this.tb_Desc.Location = new System.Drawing.Point(61, 125);
            this.tb_Desc.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Desc.Multiline = true;
            this.tb_Desc.Name = "tb_Desc";
            this.tb_Desc.Size = new System.Drawing.Size(359, 102);
            this.tb_Desc.TabIndex = 4;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(14, 119);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(46, 18);
            this.label22.TabIndex = 8;
            this.label22.Text = "备注：";
            // 
            // tb_WorkerNo
            // 
            this.tb_WorkerNo.Location = new System.Drawing.Point(106, 29);
            this.tb_WorkerNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_WorkerNo.Name = "tb_WorkerNo";
            this.tb_WorkerNo.Size = new System.Drawing.Size(138, 24);
            this.tb_WorkerNo.TabIndex = 6;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(14, 29);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(76, 18);
            this.label18.TabIndex = 5;
            this.label18.Text = "员工编号：";
            // 
            // cb_TargetMachineNo
            // 
            this.cb_TargetMachineNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_TargetMachineNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_TargetMachineNo.FormattingEnabled = true;
            this.cb_TargetMachineNo.Location = new System.Drawing.Point(662, 22);
            this.cb_TargetMachineNo.Margin = new System.Windows.Forms.Padding(2);
            this.cb_TargetMachineNo.Name = "cb_TargetMachineNo";
            this.cb_TargetMachineNo.Size = new System.Drawing.Size(118, 26);
            this.cb_TargetMachineNo.TabIndex = 241;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(579, 27);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 18);
            this.label10.TabIndex = 240;
            this.label10.Text = "目标设备";
            // 
            // tb_wuliaoxinxi
            // 
            this.tb_wuliaoxinxi.Controls.Add(this.cb_LiaoCangNo);
            this.tb_wuliaoxinxi.Controls.Add(this.label2);
            this.tb_wuliaoxinxi.Controls.Add(this.cb_RawMaterialCode);
            this.tb_wuliaoxinxi.Controls.Add(this.tb_BenCiChuKuWeight);
            this.tb_wuliaoxinxi.Controls.Add(this.label64);
            this.tb_wuliaoxinxi.Controls.Add(this.tb_YiChuKuWeight);
            this.tb_wuliaoxinxi.Controls.Add(this.label56);
            this.tb_wuliaoxinxi.Controls.Add(this.tb_XuQiuWeight);
            this.tb_wuliaoxinxi.Controls.Add(this.label48);
            this.tb_wuliaoxinxi.Controls.Add(this.tb_RawMaterialBachNo);
            this.tb_wuliaoxinxi.Controls.Add(this.label33);
            this.tb_wuliaoxinxi.Controls.Add(this.label31);
            this.tb_wuliaoxinxi.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_wuliaoxinxi.Location = new System.Drawing.Point(46, 61);
            this.tb_wuliaoxinxi.Margin = new System.Windows.Forms.Padding(2);
            this.tb_wuliaoxinxi.Name = "tb_wuliaoxinxi";
            this.tb_wuliaoxinxi.Padding = new System.Windows.Forms.Padding(2);
            this.tb_wuliaoxinxi.Size = new System.Drawing.Size(1377, 418);
            this.tb_wuliaoxinxi.TabIndex = 237;
            this.tb_wuliaoxinxi.TabStop = false;
            this.tb_wuliaoxinxi.Text = "物料信息";
            // 
            // cb_LiaoCangNo
            // 
            this.cb_LiaoCangNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_LiaoCangNo.FormattingEnabled = true;
            this.cb_LiaoCangNo.Location = new System.Drawing.Point(601, 98);
            this.cb_LiaoCangNo.Name = "cb_LiaoCangNo";
            this.cb_LiaoCangNo.Size = new System.Drawing.Size(133, 26);
            this.cb_LiaoCangNo.TabIndex = 200;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(523, 106);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 18);
            this.label2.TabIndex = 199;
            this.label2.Text = "料仓号";
            // 
            // cb_RawMaterialCode
            // 
            this.cb_RawMaterialCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_RawMaterialCode.FormattingEnabled = true;
            this.cb_RawMaterialCode.Location = new System.Drawing.Point(137, 200);
            this.cb_RawMaterialCode.Name = "cb_RawMaterialCode";
            this.cb_RawMaterialCode.Size = new System.Drawing.Size(173, 26);
            this.cb_RawMaterialCode.TabIndex = 196;
            // 
            // tb_BenCiChuKuWeight
            // 
            this.tb_BenCiChuKuWeight.Location = new System.Drawing.Point(1089, 204);
            this.tb_BenCiChuKuWeight.Margin = new System.Windows.Forms.Padding(2);
            this.tb_BenCiChuKuWeight.Name = "tb_BenCiChuKuWeight";
            this.tb_BenCiChuKuWeight.Size = new System.Drawing.Size(105, 24);
            this.tb_BenCiChuKuWeight.TabIndex = 188;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(976, 211);
            this.label64.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(102, 18);
            this.label64.TabIndex = 180;
            this.label64.Text = "本次出库重量:";
            // 
            // tb_YiChuKuWeight
            // 
            this.tb_YiChuKuWeight.Location = new System.Drawing.Point(829, 202);
            this.tb_YiChuKuWeight.Margin = new System.Windows.Forms.Padding(2);
            this.tb_YiChuKuWeight.Name = "tb_YiChuKuWeight";
            this.tb_YiChuKuWeight.Size = new System.Drawing.Size(105, 24);
            this.tb_YiChuKuWeight.TabIndex = 172;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(736, 209);
            this.label56.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(87, 18);
            this.label56.TabIndex = 164;
            this.label56.Text = "已出库重量:";
            // 
            // tb_XuQiuWeight
            // 
            this.tb_XuQiuWeight.Location = new System.Drawing.Point(619, 202);
            this.tb_XuQiuWeight.Margin = new System.Windows.Forms.Padding(2);
            this.tb_XuQiuWeight.Name = "tb_XuQiuWeight";
            this.tb_XuQiuWeight.Size = new System.Drawing.Size(76, 24);
            this.tb_XuQiuWeight.TabIndex = 156;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(541, 209);
            this.label48.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(76, 18);
            this.label48.TabIndex = 148;
            this.label48.Text = "需求重量：";
            // 
            // tb_RawMaterialBachNo
            // 
            this.tb_RawMaterialBachNo.Location = new System.Drawing.Point(406, 202);
            this.tb_RawMaterialBachNo.Margin = new System.Windows.Forms.Padding(2);
            this.tb_RawMaterialBachNo.Name = "tb_RawMaterialBachNo";
            this.tb_RawMaterialBachNo.Size = new System.Drawing.Size(105, 24);
            this.tb_RawMaterialBachNo.TabIndex = 140;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(315, 209);
            this.label33.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(91, 18);
            this.label33.TabIndex = 132;
            this.label33.Text = "原料批次号：";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(59, 208);
            this.label31.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(76, 18);
            this.label31.TabIndex = 124;
            this.label31.Text = "原料代码：";
            // 
            // tb_Time
            // 
            this.tb_Time.Location = new System.Drawing.Point(294, 75);
            this.tb_Time.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Time.Name = "tb_Time";
            this.tb_Time.Size = new System.Drawing.Size(126, 24);
            this.tb_Time.TabIndex = 324;
            // 
            // tb_Date
            // 
            this.tb_Date.Location = new System.Drawing.Point(106, 75);
            this.tb_Date.Margin = new System.Windows.Forms.Padding(2);
            this.tb_Date.Name = "tb_Date";
            this.tb_Date.Size = new System.Drawing.Size(138, 24);
            this.tb_Date.TabIndex = 323;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 75);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 18);
            this.label3.TabIndex = 325;
            this.label3.Text = "生产日期";
            // 
            // RuKuEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1469, 767);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.cb_TargetMachineNo);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tb_wuliaoxinxi);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.bt_Cancel);
            this.Controls.Add(this.bt_Printing);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RuKuEditorForm";
            this.Text = "RuKuEditorForm";
            this.Load += new System.EventHandler(this.RuKuEditorForm_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tb_wuliaoxinxi.ResumeLayout(false);
            this.tb_wuliaoxinxi.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_Save;
        private System.Windows.Forms.Button bt_Cancel;
        private System.Windows.Forms.Button bt_Printing;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tb_Desc;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox tb_WorkerNo;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox cb_TargetMachineNo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox tb_wuliaoxinxi;
        private System.Windows.Forms.ComboBox cb_RawMaterialCode;
        private System.Windows.Forms.TextBox tb_BenCiChuKuWeight;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.TextBox tb_YiChuKuWeight;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.TextBox tb_XuQiuWeight;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.TextBox tb_RawMaterialBachNo;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.ComboBox cb_LiaoCangNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_Time;
        private System.Windows.Forms.TextBox tb_Date;
        private System.Windows.Forms.Label label3;
    }
}