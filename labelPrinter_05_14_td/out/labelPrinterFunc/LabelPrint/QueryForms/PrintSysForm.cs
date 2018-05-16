﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelPrint.Util;
//using LabelPrint.QueryForms;
using LabelPrint.Data;
using LabelPrint.EditorForms;
namespace LabelPrint.QueryForms
{
    public partial class PrintSysForm : Form
    {
        FilmPrintSysData PSysData;
        DataTable dt;

        public PrintSysForm()
        {
            InitializeComponent();
        }

        private void bt_New_Click(object sender, EventArgs e)
        {
            PrintForm f = new PrintForm();
            f.ShowDialog();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);
        }

        private void bt_Find_Click(object sender, EventArgs e)
        {
            UpdateUserInput();
            dt = PSysData.UpdateDBViewBySelection(dataGridView1);
        }

        private void bt_Print_Click(object sender, EventArgs e)
        {

            PSysData.PrintDataTablePriew(dt);

        }

        private void PintSysForm_Load(object sender, EventArgs e)
        {
            PSysData = new FilmPrintSysData();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;

            PSysData.SetDateTimePickerFormat(this.dateTimePicker1, this.dateTimePicker2);
            UpdateUserInput();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);
        }

        void UpdateUserInput()
        {
            PSysData.Date1 = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            PSysData.Date2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            PSysData.BatchNo = tb_Batchno.Text;
            PSysData.ProductCode = tb_ProductCode.Text;
            PSysData.BigRollNo = tb_BigRollNo.Text;
            PSysData.WorkNo = tb_WorkNo.Text;
            PSysData.Customer = tb_Customer.Text;
            //PSysData.Recipe = tb_Recipe.Text;
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            PSysData.RowIndex = e.RowIndex;
            DataTable dt = PSysData.CurDT;
            if (PSysData.RowIndex < 0 || PSysData.RowIndex >= dt.Rows.Count)
                return;

            int id = (int)dt.Rows[PSysData.RowIndex][0];

            PrintEditorForm f = new PrintEditorForm();
            f.SetSelItem(dt, id);
            f.ShowDialog();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);
        }
    }
}