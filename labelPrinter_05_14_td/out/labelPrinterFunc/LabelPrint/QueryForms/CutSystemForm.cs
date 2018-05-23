using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelPrint.Util;
using LabelPrint.Data;
using LabelPrint.excelOuput;
//using LabelPrint.excelOuput;
using LabelPrint.NetWork;

namespace LabelPrint
{
    public partial class CutSystemForm : Form
    {
	    FilmSocket m_FilmSocket;
        CutSysData PSysData;

        public CutSystemForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        /*
         * 分切清单报表界面支持已完成分切单的查询功能，界面以列表形式显示、查看，并且支持驱动打印机A4纸张打印。
         */
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        
        }

		public void network_status_change(bool status)
        {
        	Console.WriteLine("network changed to {0}", status);

			if (status == true) {//connected
				HandShake();
			}
		}

        /*
         * 当该小卷为不合格品时，产品质量栏位需显示不合格分类编码，如“图 5 分切打印中的问题点选择”中的A/B/C/D/DC/E/W。
         * 
         * Quality Problem:
         * A.晶点孔洞
         * B.厚薄暴筋
         * C.折皱
         * D.端面错位(毛糙）
         * DC待处理
         * E.油污
         * W.蚊虫
         * 
         */
        DataTable dt;
        private void CutSystemForm_Load(object sender, EventArgs e)
        {
            PSysData = new CutSysData();

            this.Icon = new Icon("..\\..\\resources\\zihualogo_48X48.ico");
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;

            PSysData.SetDateTimePickerFormat(this.dateTimePicker1, this.dateTimePicker2);
            UpdateUserInput();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);

            dataGridView1_widthSetting(dataGridView1);

			m_FilmSocket = new FilmSocket();
			m_FilmSocket.network_state_event += new FilmSocket.networkstatehandler(network_status_change);
        }

        private void bt_New_Click(object sender, EventArgs e)
        {
            ProductCutForm f = new ProductCutForm(m_FilmSocket);
            f.ShowDialog();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);
        }

        void UpdateUserInput()
        {
            PSysData.Date1 = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            PSysData.Date2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            PSysData.BatchNo = tb_Batchno.Text;
            PSysData.ProductCode = tb_ProductCode.Text;
            PSysData.BigRollNo = tb_BigRollNo.Text;
            PSysData.LittleRollNo = tb_LittleRollNo.Text;
            PSysData.Customer = tb_Customer.Text;
            PSysData.Recipe = tb_Recipe.Text;
            PSysData.PlateNo = tb_PlateNo.Text;
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            UpdateUserInput();
            dt = PSysData.UpdateDBViewBySelection(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (tb_Batchno.Text == null || tb_Batchno.Text == "")
            {
                MessageBox.Show("需要指定生产批号");
            }
            if (cb_Banzu.Text == null || cb_Banzu.Text == "")
            {
                MessageBox.Show("需要指定班组号");
            }

#if false
            String Date1 = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            String  Date2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            if (Date1!=Date2)
            {
                MessageBox.Show("需要指定的时间间隔为一天");
            }

#endif

            UpdateUserInput();
            dt = PSysData.UpdateDBViewBySelection(dataGridView1);
            dt = PSysData.GetDTSelItemsFromDB();
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("没有数据记录");
            }
            else
            {

                DataTable newdt = null;// new DataTable();
                List<DataTable> dtlist = new List<DataTable>();
                int DateIndex = CutUserinputData.GetIndexByString("Date");
                //newdt = dt.Clone(); // 克隆dt 的结构，包括所有 dt 架构和约束,并无数据； 
                // DataRow[] rows = dt.Select(conditions); // 从dt 中查询符合条件的记录； 
                String PreDate = dt.Rows[0][DateIndex].ToString();
                List<String> DateList = new List<String>();
                foreach (DataRow Row in dt.Rows)
                {
                    DateList.Add(Row[DateIndex].ToString());
                }
                List<String> UniDateList = DateList.Distinct().ToList();

                Log.e("Cut", "List count  =" + UniDateList.Count);
                foreach (String date in UniDateList)
                {
                    newdt = new DataTable();
                    newdt = dt.Clone();
                    foreach (DataRow Row in dt.Rows)  // 将查询的结果添加到dt中； 
                    {
                        if (date == Row[DateIndex].ToString())
                            newdt.Rows.Add(Row.ItemArray);
                    }

                    dtlist.Add(newdt);
                }

                PSysData.PrintBangMaReport(dtlist);

                excelClass excelClassImpl = new excelClass();
                excelClassImpl.weightListFunc();
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
                        
            PSysData.RowIndex = e.RowIndex;
            DataTable dt = PSysData.CurDT;
            if (PSysData.RowIndex < 0 || PSysData.RowIndex >= dt.Rows.Count)
                return;

            int id = (int)dt.Rows[PSysData.RowIndex][0];

            CutEditorForm f = new CutEditorForm();
            f.SetSelItem(dt, id);
            f.ShowDialog();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);
        }

        private void dataGridView1_widthSetting(DataGridView dataGridView1)
        {
            int i;
            int [] gridColumnWidth = {60, 100, 90, 100, 150, 120, 150, 100, 90, 90, 100, 100, 120, 100,370};
            if (dataGridView1.Columns.Count > 0) { 
            for (i = 0; i < gridColumnWidth.Length; i++ )
                dataGridView1.Columns[i].Width = gridColumnWidth[i];
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            excelClass excelClassImpl = new excelClass();
            excelClassImpl.weightListFunc();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (tb_Batchno.Text==null|| tb_Batchno.Text=="")
            {
                MessageBox.Show("需要指定生产批号");
            }
            if (cb_Banzu.Text == null || cb_Banzu.Text == "")
            {
                MessageBox.Show("需要指定班组号");
            }

#if false
                 String Date1 = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            String  Date2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            if (Date1!=Date2)
            {
                MessageBox.Show("需要指定的时间间隔为一天");
            }

#endif

            UpdateUserInput();
            dt = PSysData.UpdateDBViewBySelection(dataGridView1);
            dt = PSysData.GetDTSelItemsFromDB();
            if (dt==null ||dt.Rows.Count==0)
            {
                MessageBox.Show("没有数据记录");
            }
            else
            {

                DataTable newdt = null;// new DataTable();
                List<DataTable> dtlist = new List<DataTable>();
                int DateIndex = CutUserinputData.GetIndexByString("Date");
                //newdt = dt.Clone(); // 克隆dt 的结构，包括所有 dt 架构和约束,并无数据； 
                // DataRow[] rows = dt.Select(conditions); // 从dt 中查询符合条件的记录； 
                String PreDate = dt.Rows[0][DateIndex].ToString();
                List<String> DateList = new List<String>();
                foreach (DataRow Row in dt.Rows)
                {
                        DateList.Add(Row[DateIndex].ToString());
                }
                List<String> UniDateList  = DateList.Distinct().ToList();

                Log.e("Cut", "List count  =" + UniDateList.Count);
                foreach (String date in UniDateList)
                {
                    newdt = new DataTable();
                    newdt = dt.Clone();
                    foreach (DataRow Row in dt.Rows)  // 将查询的结果添加到dt中； 
                    {
                        if (date == Row[DateIndex].ToString())
                            newdt.Rows.Add(Row.ItemArray);
                    }

                    dtlist.Add(newdt);
                }

                PSysData.PrintSplitCutDailyReport(dtlist);

                excelClass excelClassImpl = new excelClass();
                excelClassImpl.slitReportFunc();

            }
        }

        /*
         * 分切清单界面还可以根据标签的生产批号/产品编号/大卷号/小卷号/客户编号/配方号进行查询，列表显示。
         */

		public void HandShake()
		{
			int machineID;
			byte[] data = new byte[4];
			int rsp;
			
			machineID = 180 + Convert.ToInt16(GlobalConfig.Setting.CurSettingInfo.MachineNo);
			data[0] = (byte)(machineID&0xff);
			data[1] = (byte)((machineID&0xff00)>>8);
			m_FilmSocket.sendDataPacketToServer(data, 0x3, 2);
			
			rsp = m_FilmSocket.RecvResponse(1000);
		}
    }
}
