using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LabelPrint.NetWork;

namespace LabelPrint.PrintForms
{
    public partial class RuKuForm : Form
    {
		//出入库工序
		private const int COMMUNICATION_TYPE_WAREHOUE_OUT_START = 0xB5;  //printing SW started and ask for material info, server send material info for all feeding machine to printing SW 
		private const int COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE = 0xB6;	//printing machine send barcode info to server whever a stack of material is moved out of the warehouse
		private const int COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE = 0xB7;  //printing machine send barcode info to server whever a stack of material is moved into the warehouse
		private FilmSocket m_FilmSocket;
		FilmSocket.networkstatehandler m_networkstatehandler;
  
        public RuKuForm(FilmSocket filmsocket)
        {
            InitializeComponent();
			m_FilmSocket = filmsocket;
        }
		~RuKuForm()
       	{
       		m_FilmSocket.network_state_event -= m_networkstatehandler;
		}

		public void network_status_change(bool status)
        {
        	Console.WriteLine("network changed to {0}", status);
		}

		private void RuKuForm_Load(object sender, EventArgs e)
		{
			
			m_networkstatehandler = new FilmSocket.networkstatehandler(network_status_change);
			m_FilmSocket.network_state_event += m_networkstatehandler;
		}

        private void bt_Record_Click(object sender, EventArgs e)
        {
            //String JiaoJieRecord;

            //JiaoJieBanForm f = new JiaoJieBanForm();
            //f.ShowDialog();
            //if (f.DialogResult == DialogResult.OK)
            //{                UpdateUserInput();
            //    JiaoJieRecord = f.GetJiaoBanRecord();
            //    if (JiaoJieRecord != null && JiaoJieRecord != "")
            //    {

            //        UserInput.InsertJIaoJieRecord();
            //        //write jiao JIe Record to DB

            //    }
            //}

        	//<原料代码>;<原料批次号>;<目标设备号>;<料仓号>;<重量>
        	string str="";
			byte[] send_buf = System.Text.Encoding.Default.GetBytes(str);
	
			m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE, send_buf.Length);

			int rsp = m_FilmSocket.RecvResponse(1000);
			if (rsp == 0)	System.Windows.Forms.MessageBox.Show("发送成功！");
        }
    }
}
