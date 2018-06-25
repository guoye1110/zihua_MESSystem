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
    public partial class setting3 : Form
    {
        public static setting3 setting3Class = null; //用来引用主窗口
        System.Windows.Forms.Timer aTimer;

        public setting3()
        {
            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
            initVariables();
        }

        private void initVariables()
        {
            this.Text = "设备 " + gVariable.currentCurveDatabaseName + " GPIO 功能设定";
        }


        private void setting3_Load(object sender, EventArgs e)
        {
            int i;
            //int x, y;

            RadioButton [] radioButtonArray1 =
            {
                radioButton1, radioButton3, radioButton5, radioButton7, radioButton9, radioButton11,radioButton13,radioButton15,
                radioButton17,radioButton19,radioButton21,radioButton23,radioButton25,radioButton27,radioButton29,radioButton31,
            };

            RadioButton [] radioButtonArray2 =
            {
                radioButton2, radioButton4, radioButton6, radioButton8, radioButton10,radioButton12,radioButton14,radioButton16,
                radioButton18,radioButton20,radioButton22,radioButton24,radioButton26,radioButton28,radioButton30,radioButton32,
            };

            TextBox[] textBoxArray1 = 
            { 
                textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13, textBox14, textBox15, textBox16
            };

            GroupBox[] groupBoxArray1 = 
            {
                groupBox2, groupBox3, groupBox4, groupBox5, groupBox6, groupBox7, groupBox8, groupBox9, groupBox10, groupBox11, groupBox12, groupBox13, groupBox14, groupBox15, groupBox16, groupBox35 
            };

            GroupBox[] groupBoxArray2 = 
            {
                groupBox19, groupBox20, groupBox21, groupBox22, groupBox23, groupBox24, groupBox25, groupBox26, groupBox27, groupBox28, groupBox29, groupBox30, groupBox31, groupBox32, groupBox33, groupBox34 
            };

            GroupBox[] groupBoxArray3 = 
            {
                groupBox85, groupBox86, groupBox87, groupBox88, groupBox89, groupBox90, groupBox91, groupBox92, groupBox93, groupBox94, groupBox95, groupBox96, groupBox97, groupBox98, groupBox99, groupBox100 
            };

            Panel[] panelArray1 = 
            {
                panel1, panel2, panel3, panel4, panel5, panel6, panel7, panel8, panel9, panel10, panel11, panel12, panel13, panel14, panel15, panel16 
            };

            for (i = 0; i < gVariable.numOfGPIOs; i++)
            {
                radioButtonArray1[i].Location = new System.Drawing.Point(16, 30);
                radioButtonArray1[i].Size = new System.Drawing.Size(85, 17);

                radioButtonArray2[i].Location = new System.Drawing.Point(16, 61);
                radioButtonArray2[i].Size = new System.Drawing.Size(85, 17);

                if (gVariable.GPIOSettingInfo[gVariable.boardIndexSelected].GPIOTriggerVoltage[i] == 0)
                    radioButtonArray2[i].Checked = true;
                else
                    radioButtonArray1[i].Checked = true;

                if (i < 8)
                {
                    groupBoxArray1[i].Location = new System.Drawing.Point(22 + 149 * i, 20);
                    groupBoxArray1[i].Size = new System.Drawing.Size(114, 100);

                    groupBoxArray2[i].Location = new System.Drawing.Point(22 + 149 * i, 20);
                    groupBoxArray2[i].Size = new System.Drawing.Size(114, 67);
                }
                else
                {
                    groupBoxArray1[i].Location = new System.Drawing.Point(22 + 149 * (i - 8), 140);
                    groupBoxArray1[i].Size = new System.Drawing.Size(114, 100);

                    groupBoxArray2[i].Location = new System.Drawing.Point(22 + 149 * (i - 8), 110);
                    groupBoxArray2[i].Size = new System.Drawing.Size(114, 67);
                }

                textBoxArray1[i].Text = gVariable.GPIOSettingInfo[gVariable.boardIndexSelected].GPIOName[i];

                groupBoxArray1[i].Text = "GPIO" + (i + 1);
                groupBoxArray2[i].Text = "GPIO" + (i + 1);

                groupBoxArray3[i].Location = new System.Drawing.Point(11 + 74 * i, 27);
                groupBoxArray3[i].Size = new System.Drawing.Size(70, 70);
                groupBoxArray3[i].Text = textBoxArray1[i].Text;

                textBoxArray1[i].Location = new System.Drawing.Point(13, 26);
                textBoxArray1[i].Size = new System.Drawing.Size(85, 20);

                panelArray1[i].Location = new System.Drawing.Point(14, 18);
                panelArray1[i].Size = new System.Drawing.Size(42, 42);
            }

            aTimer = new System.Windows.Forms.Timer();

            aTimer.Interval = 1000;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_listview);
        
        }

        private void setting3_FormClosing(object sender, EventArgs e)
        {
            if (aTimer != null)
                aTimer.Enabled = false;

            dispatchUI.dispatchUIClass.Show();
        }

        //This function will be called every second
        private void timer_listview(Object source, EventArgs e)
        {
            int i;
            Panel[] panelArray1 = 
            {
                panel1, panel2, panel3, panel4, panel5, panel6, panel7, panel8, panel9, panel10, panel11, panel12, panel13, panel14, panel15, panel16 
            };

            for (i = 0; i < gVariable.numOfGPIOs; i++)
            {
                if ((gVariable.gpioStatus & (uint)(1 << i)) == 0)
                {
                      panelArray1[i].BackColor = System.Drawing.Color.Green;
//                    gpioLabelArray[i].Text = "正常";
//                    gpioLabelArray[i].ForeColor = System.Drawing.Color.Black;
                }
                else
                {
                      panelArray1[i].BackColor = System.Drawing.Color.Red;
//                    gpioLabelArray[i].Text = "异常";
//                    gpioLabelArray[i].ForeColor = System.Drawing.Color.Black;
                }
            }
        }


        //ok
        private void button1_Click(object sender, EventArgs e)
        {
            int i;
            TextBox[] textBoxArray1 = 
            { 
                textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13, textBox14, textBox15, textBox16
            };

            GroupBox[] groupBoxArray3 = 
            {
                groupBox85, groupBox86, groupBox87, groupBox88, groupBox89, groupBox90, groupBox91, groupBox92, groupBox93, groupBox94, groupBox95, groupBox96, groupBox97, groupBox98, groupBox99, groupBox100 
            };

            RadioButton[] radioButtonArray2 =
            {
                radioButton2, radioButton4, radioButton6, radioButton8, radioButton10,radioButton12,radioButton14,radioButton16,
                radioButton18,radioButton20,radioButton22,radioButton24,radioButton26,radioButton28,radioButton30,radioButton32,
            };

            for (i = 0; i < gVariable.numOfGPIOs; i++)
            {
                if(radioButtonArray2[i].Checked == true)
                    gVariable.GPIOSettingInfo[gVariable.boardIndexSelected].GPIOTriggerVoltage[i] = 0;
                else
                    gVariable.GPIOSettingInfo[gVariable.boardIndexSelected].GPIOTriggerVoltage[i] = 1;

                gVariable.GPIOSettingInfo[gVariable.boardIndexSelected].GPIOName[i] = textBoxArray1[i].Text;

                groupBoxArray3[i].Text = textBoxArray1[i].Text;
            }

            //we are control this board for setting function
            gVariable.whereComesTheSettingData = gVariable.boardIndexSelected;
            gVariable.whatSettingDataModified = gVariable.GPIO_SETTING_DATA_TO_BOARD;
        }

        //exit
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
