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
    public partial class RecoverySysForm : Form
    {
		private FilmSocket m_FilmSocket;

        RecoverySysData PSysData;
        DataTable dt;

        public RecoverySysForm()
        {
            InitializeComponent();
        }

		public void network_status_change(bool status)
        {
        	Console.WriteLine("network changed to {0}", status);

			if (status == true) {//connected
				HandShake();
			}
		}

		public void HandShake()
        {
        	int machineID;
        	byte[] data = new byte[4];
			int rsp;

			machineID = 220 + Convert.ToInt16(GlobalConfig.Setting.CurSettingInfo.MachineNo);
			data[0] = (byte)(machineID&0xff);
			data[1] = (byte)((machineID&0xff00)>>8);
        	m_FilmSocket.sendDataPacketToServer(data, 0x3, 2);

			rsp = m_FilmSocket.RecvResponse(1000);
		}

        private void RecoverySysForm_Load(object sender, EventArgs e)
        {
            PSysData = new RecoverySysData();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;

            PSysData.SetDateTimePickerFormat(this.dateTimePicker1, this.dateTimePicker2);
            UpdateUserInput();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);

			m_FilmSocket = new FilmSocket();
			m_FilmSocket.network_state_event += new FilmSocket.networkstatehandler(network_status_change);
        }

        private void bt_Print_Click(object sender, EventArgs e)
        {
            PSysData.PrintDataTablePriew(dt);
        }

        private void bt_Find_Click(object sender, EventArgs e)
        {
            UpdateUserInput();
            dt = PSysData.UpdateDBViewBySelection(dataGridView1);

        }

        private void bt_New_Click(object sender, EventArgs e)
        {
            RecoveryForm f = new RecoveryForm(m_FilmSocket);
            f.ShowDialog();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);
        }
        void UpdateUserInput()
        {
            PSysData.Date1 = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            PSysData.Date2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");
           // PSysData.Color = tb_Color.Text;
            PSysData.Recipe = tb_Recipe.Text;
           // PSysData.Vendor = tb_Vendor.Text;
           // PSysData.WeightPerBag = tb_WeightPerBag.Text;
            PSysData.Worker_No = tb_WorkerNo.Text;
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            PSysData.RowIndex = e.RowIndex;
            DataTable dt = PSysData.CurDT;
            if (PSysData.RowIndex < 0 || PSysData.RowIndex >= dt.Rows.Count)
                return;

            int id = (int)dt.Rows[PSysData.RowIndex][0];

            RecoveryEditorForm f = new RecoveryEditorForm();
            f.SetSelItem(dt, id);
            f.ShowDialog();
            dt = PSysData.UpdateDBViewBy2Date(dataGridView1);

        }
    }
}
