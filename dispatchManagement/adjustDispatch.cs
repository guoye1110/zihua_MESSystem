using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;

namespace MESSystem.dispatchManagement
{
    public partial class adjustDispatch : Form
    {
        public adjustDispatch(string dispatchCode)
        {
            InitializeComponent();
            initVariables(dispatchCode);
        }

        void initVariables(string dispatchCode)
        {
            string commandText;
            string[,] tableArray;

            comboBox5.Items.Add("王晓明");
            comboBox5.Items.Add("刘丰");
            comboBox5.Items.Add("未定");

            comboBox3.Items.Add("王晓明");
            comboBox3.Items.Add("刘丰");
            comboBox3.Items.Add("未定");

            comboBox2.Items.Add("王晓明");
            comboBox2.Items.Add("刘丰");
            comboBox2.Items.Add("未定");

            comboBox4.Items.Add("甲班");
            comboBox4.Items.Add("乙班");
            comboBox4.Items.Add("丙班");

            commandText = "select * from `" + gVariable.globalDispatchTableName + "` where dispatchCode = '" + dispatchCode + "'";
            tableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
        
            textBox1.Text = gVariable.machineNameArrayAPS[mySQLClass.MACHINE_ID_IN_DISPATCHLIST_DATABASE];
            textBox2.Text = tableArray[0, mySQLClass.BATCH_NUMBER_IN_DISPATCHLIST_DATABASE];
            textBox9.Text = tableArray[0, mySQLClass.SALESORDER_CODE_IN_DISPATCHLIST_DATABASE];
            textBox7.Text = dispatchCode;
            textBox5.Text = tableArray[0, mySQLClass.PRODUCT_CODE_IN_DISPATCHLIST_DATABASE];
            textBox8.Text = tableArray[0, mySQLClass.CUSTOMER_IN_DISPATCHLIST_DATABASE];
        }
    }
}
