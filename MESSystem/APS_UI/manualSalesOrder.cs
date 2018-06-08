using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;

namespace MESSystem.APS_UI
{
    public partial class manualSalesOrder : Form
    {
        public static manualSalesOrder manualSalesOrderImpl;

        gVariable.salesOrderStruct salesOrderImpl = new gVariable.salesOrderStruct();

        public manualSalesOrder(int index)
        {
            string commandText;

            InitializeComponent();

            this.TopMost = true;
            dateTimePicker1.Value = DateTime.Now.Date;

            if (index > 0)
            {
                commandText = "select * from `" + gVariable.salesOrderTableName + "` where id = " + index;
                mySQLClass.readSalesOrderInfo(ref salesOrderImpl, commandText);

                //textBox1.Text = salesOrderImpl.salesOrderCode;
                //dateTimePicker1.Value = salesOrderImpl.deliveryTime;
                textBox7.Text = salesOrderImpl.productCode;
                textBox6.Text = salesOrderImpl.productName;
                textBox9.Text = salesOrderImpl.requiredNum;
                textBox8.Text = salesOrderImpl.unit;
                textBox3.Text = salesOrderImpl.customer;
            }
        }

        //confirmed
        private void button1_Click(object sender, EventArgs e)
        {
            salesOrderImpl.salesOrderCode = textBox1.Text;
            salesOrderImpl.deliveryTime = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            salesOrderImpl.productCode = textBox7.Text;
            salesOrderImpl.productName = textBox6.Text;
            salesOrderImpl.requiredNum = textBox9.Text;
            salesOrderImpl.unit = textBox8.Text;
            salesOrderImpl.customer = textBox3.Text;

            salesOrderImpl.publisher = gVariable.userAccount;
            salesOrderImpl.ERPTime = DateTime.Now.ToString("yyyy-MM-dd");
            salesOrderImpl.APSTime = null;
            salesOrderImpl.planTime1 = null;
            salesOrderImpl.planTime2 = null;
            salesOrderImpl.realStartTime = null;
            salesOrderImpl.realFinishTime = null;
            salesOrderImpl.source = "手工输入";  //"ERP 导入"
            salesOrderImpl.status = gVariable.SALES_ORDER_STATUS_ERP_PUBLISHED.ToString();
            mySQLClass.writeDataToSalesOrderTable(gVariable.salesOrderTableName, gVariable.salesOrderFileName, salesOrderImpl);

            gVariable.APSScreenRefresh = 1;

            this.Close();
        }

        //exit
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
