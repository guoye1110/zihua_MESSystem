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
    public partial class setting4 : Form
    {
        public static setting4 setting4Class = null; //用来引用主窗口

        public setting4()
        {
            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
            initVariables();
        }

        private void initVariables()
        {
            this.Text = "设备 " + gVariable.currentCurveDatabaseName + " 生产节拍设定";
        }

        private void setting4_Load(object sender, EventArgs e)
        {
            if (gVariable.beatPeriodInfo[gVariable.boardIndexSelected].deviceSelection == 0)
                this.radioButton1.Checked = true;
            else
                this.radioButton2.Checked = true;

            textBox2.Text = gVariable.beatPeriodInfo[gVariable.boardIndexSelected].idleCurrentHigh.ToString();
            textBox1.Text = gVariable.beatPeriodInfo[gVariable.boardIndexSelected].idleCurrentLow.ToString();
            textBox3.Text = gVariable.beatPeriodInfo[gVariable.boardIndexSelected].workCurrentHigh.ToString();
            textBox4.Text = gVariable.beatPeriodInfo[gVariable.boardIndexSelected].workCurrentLow.ToString();
        }

        private void setting4_FormClosing(object sender, EventArgs e)
        {
            dispatchUI.dispatchUIClass.Show();
        }

        // ok
        private void button1_Click(object sender, EventArgs e)
        {
            int ret1, ret2;

            ret1 = toolClass.isNumericOrNot(textBox1.Text);
            ret2 = toolClass.isNumericOrNot(textBox2.Text);
            if (ret1 == 0 || ret2 == 0)
            {
                MessageBox.Show("待机电流设定值有误，请修正", "信息提示", MessageBoxButtons.OK);
            }

            ret1 = toolClass.isNumericOrNot(textBox3.Text);
            ret2 = toolClass.isNumericOrNot(textBox4.Text);
            if (ret1 == 0 || ret2 == 0)
            {
                MessageBox.Show("工作电流设定值有误，请修正", "信息提示", MessageBoxButtons.OK);
            }

            if (radioButton1.Checked == true)
                gVariable.beatPeriodInfo[gVariable.boardIndexSelected].deviceSelection = 0;
            else
                gVariable.beatPeriodInfo[gVariable.boardIndexSelected].deviceSelection = 1;

            //internal/external flag, low level, low thresh, high level, high thresh, beat idle
            gVariable.beatPeriodInfo[gVariable.boardIndexSelected].idleCurrentHigh = (float)Convert.ToDouble(textBox2.Text);
            gVariable.beatPeriodInfo[gVariable.boardIndexSelected].idleCurrentLow = (float)Convert.ToDouble(textBox1.Text);
            gVariable.beatPeriodInfo[gVariable.boardIndexSelected].workCurrentHigh = (float)Convert.ToDouble(textBox3.Text);
            gVariable.beatPeriodInfo[gVariable.boardIndexSelected].workCurrentLow = (float)Convert.ToDouble(textBox4.Text);

            //we are control this board for setting function
            gVariable.whereComesTheSettingData = gVariable.boardIndexSelected;
            gVariable.whatSettingDataModified = gVariable.BEAT_SETTING_DATA_TO_BOARD;

            this.Close();
        }

        //exit
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
