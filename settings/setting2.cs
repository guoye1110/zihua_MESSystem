using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;
using MESSystem.mainUI;

namespace MESSystem.settings
{
    public partial class setting2 : Form
    {
        public static setting2 setting2Class = null; //用来引用主窗口

        private static string [] baudrateIndex = { "9600", "19200", "38400", "57600", "115200"};

        public setting2()
        {
            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
            initVariables();
            setComboboxes();
        }

        private void initVariables()
        {
            this.Text = "设备 " + gVariable.currentCurveDatabaseName + " 串行接口功能设定";
        }


        private void setting2_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = gVariable.uartSettingInfo[gVariable.boardIndexSelected].uartBaudrate[0];
            comboBox3.SelectedIndex = gVariable.uartSettingInfo[gVariable.boardIndexSelected].uartBaudrate[1];
            comboBox5.SelectedIndex = gVariable.uartSettingInfo[gVariable.boardIndexSelected].uartBaudrate[2];
        }

        private void setting2_FormClosing(object sender, EventArgs e)
        {
            dispatchUI.dispatchUIClass.Show();
        }

        //com1 clear display
        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        //com1 send
        private void button1_Click(object sender, EventArgs e)
        {
            sendDataToBoardUart(textBox1.Text, 1);
        }

        //com2 clear display
        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        //com2 send
        private void button2_Click(object sender, EventArgs e)
        {
            sendDataToBoardUart(textBox2.Text, 2);
        }

        //com3 dislay
        private void button5_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
        }

        //come3 send
        private void button4_Click(object sender, EventArgs e)
        {
            sendDataToBoardUart(textBox4.Text, 3);
        }

        //ok
        private void button6_Click(object sender, EventArgs e)
        {
            gVariable.uartSettingInfo[gVariable.boardIndexSelected].uartBaudrate[0] = comboBox2.SelectedIndex;
            gVariable.uartSettingInfo[gVariable.boardIndexSelected].uartBaudrate[1] = comboBox3.SelectedIndex;
            gVariable.uartSettingInfo[gVariable.boardIndexSelected].uartBaudrate[2] = comboBox5.SelectedIndex;

            gVariable.uartSettingInfo[gVariable.boardIndexSelected].selectedUart = gVariable.UARTNotSelected;

            //we are control this board for setting function
            gVariable.whereComesTheSettingData = gVariable.boardIndexSelected;
            gVariable.whatSettingDataModified = gVariable.UART_SETTING_DATA_TO_BOARD;

            this.Close();
        }

        //exit
        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void setComboboxes()
        {
            int i;

            for (i = 0; i < baudrateIndex.Length; i++)
            {
                comboBox2.Items.Add(baudrateIndex[i]);
            }
            comboBox2.SelectedIndex = gVariable.uartSettingInfo[gVariable.boardIndexSelected].uartBaudrate[0];

            for (i = 0; i < baudrateIndex.Length; i++)
            {
                comboBox3.Items.Add(baudrateIndex[i]);
            }
            comboBox3.SelectedIndex = gVariable.uartSettingInfo[gVariable.boardIndexSelected].uartBaudrate[1];

            for (i = 0; i < baudrateIndex.Length; i++)
            {
                comboBox5.Items.Add(baudrateIndex[i]);
            }
            comboBox5.SelectedIndex = gVariable.uartSettingInfo[gVariable.boardIndexSelected].uartBaudrate[2];
        }

        private void sendDataToBoardUart(string str, int index)
        {
            gVariable.uartSettingInfo[gVariable.boardIndexSelected].selectedUart = index - 1;
            gVariable.uartSettingInfo[gVariable.boardIndexSelected].uartOutputData[index] = str;

            //we are control this board for setting function
            gVariable.whereComesTheSettingData = gVariable.boardIndexSelected;
            gVariable.whatSettingDataModified = gVariable.UART_SETTING_DATA_TO_BOARD;
        }

    }
}
