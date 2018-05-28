using System;
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
using LabelPrint.NetWork;

namespace LabelPrint.QueryForms
{
    public partial class QASysForm : Form
    {
        QASysData PSysData;
        DataTable dt;
		FilmSocket m_FilmSocket;

        public QASysForm()
        {
            InitializeComponent();
        }

        private void bt_New_Click(object sender, EventArgs e)
        {
            QAForm f = new QAForm(m_FilmSocket);
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

		public void network_status_change(bool status)
        {
        	Console.WriteLine("network changed to {0}", status);

			if (status == true) {//connected
				HandShake();
			}
		}
		public void network_data_received(int communicationType, byte[] data_buf, int len)
		{
			if (communicationType == 0x03) {
				int result = data_buf[0];
			}
		}

		public void HandShake()
        {
        	int machineID;
        	byte[] data = new byte[4];
			int rsp;

			machineID = 200 + Convert.ToInt16(GlobalConfig.Setting.CurSettingInfo.MachineNo);
			data[0] = (byte)(machineID&0xff);
			data[1] = (byte)((machineID&0xff00)>>8);
        	m_FilmSocket.sendDataPacketToServer(data, 0x3, 2);

			//rsp = m_FilmSocket.RecvResponse(1000);
		}		

        private void QASysForm_Load(object sender, EventArgs e)
        {
            PSysData = new QASysData();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;

            PSysData.SetDateTimePickerFormat(this.dateTimePicker1, this.dateTimePicker2);
            UpdateUserInput();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);

			m_FilmSocket = new FilmSocket();
			m_FilmSocket.network_state_event += new FilmSocket.networkstatehandler(network_status_change);
			m_FilmSocket.network_data_event += new FilmSocket.networkdatahandler(network_data_received);
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
            //PSysData.RecipeCode = tb_RecipeCode.Text;
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            PSysData.RowIndex = e.RowIndex;
            DataTable dt = PSysData.CurDT;
            if (PSysData.RowIndex < 0 || PSysData.RowIndex >= dt.Rows.Count)
                return;

            int id = (int)dt.Rows[PSysData.RowIndex][0];

            QAEditorForm f = new QAEditorForm();
            f.SetSelItem(dt, id);
            f.ShowDialog();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);

        }
        void InitProductStateComboBox(ComboBox productStates)
        {
            productStates.Items.AddRange(ProcessData.ProductStateStr);

        }

        void InitProductQualityComboBox(ComboBox productQulity)
        {
            productQulity.Items.AddRange(ProcessData.ProductQualityStrForComBoList);

        }
    }
}
