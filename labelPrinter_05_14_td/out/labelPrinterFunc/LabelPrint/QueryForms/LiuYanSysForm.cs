﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelPrint.Data;
using LabelPrint.EditorForms;
namespace LabelPrint.QueryForms
{
    public partial class LiuYanSysForm : Form
    {
        LiuYanSysData PSysData;
        DataTable dt;

        public LiuYanSysForm()
        {
            InitializeComponent();
        }

        private void LiuYanSysForm_Load(object sender, EventArgs e)
        {
            PSysData = new LiuYanSysData();
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
            PSysData.Customer = tb_Customer.Text;
            PSysData.WorkNo = tb_WorkNo.Text;
            PSysData.ProductCode = tb_ProductCode.Text;
  //          PSysData.BigRollNo = tb_BigRollNo.Text;
//            PSysData.LittleRollNo = tb_LittleRollNo.Text;

            //PSysData.Recipe = tb_Recipe.Text;
        }

        private void bt_New_Click(object sender, EventArgs e)
        {
            LiuYanForm f = new LiuYanForm();
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

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            PSysData.RowIndex = e.RowIndex;
            DataTable dt = PSysData.CurDT;
            if (PSysData.RowIndex < 0 || PSysData.RowIndex >= dt.Rows.Count)
                return;

            int id = (int)dt.Rows[PSysData.RowIndex][0];

            LiuYanEditorForm f = new LiuYanEditorForm();
            f.SetSelItem(dt, id);
            f.ShowDialog();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);

        }
    }
}
