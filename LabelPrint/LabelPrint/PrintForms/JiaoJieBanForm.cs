﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using LabelPrint.excelOuput;

namespace LabelPrint.PrintForms
{
    public partial class JiaoJieBanForm : Form
    {
        public JiaoJieBanForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();

            ///  excelClass excelClassImpl = new excelClass();
            // excelClassImpl.slitReportFunc();
            // Thread.Sleep(1000);
            //  System.Diagnostics.Process.Start("..\\..\\outputTables\\分切日报表\\2018-04-05.xlsx");
            //System.Diagnostics.Process.Start("..\\..\\a.txt");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public String GetJiaoBanRecord()
        {
            return tb_JiaoBan.Text;
        }

        private void JiaoJieBanForm_Load(object sender, EventArgs e)
        {

        }
    }
}