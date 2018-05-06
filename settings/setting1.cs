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
    public partial class setting1 : Form
    {
        public static setting1 setting1Class = null; //用来引用主窗口

        public setting1()
        {
            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            initVariables();
        }

        private void setting1_Load(object sender, EventArgs e)
        {
            int i;

            TextBox[] textBoxArray1 = { textBox2, textBox8, textBox12, textBox16, textBox32, textBox28, textBox24, textBox20 };  //title
            TextBox[] textBoxArray2 = { textBox1, textBox7, textBox11, textBox15, textBox31, textBox27, textBox23, textBox13 };  //lower range
            TextBox[] textBoxArray3 = { textBox3, textBox6, textBox10, textBox14, textBox30, textBox26, textBox22, textBox9 };  //upper range
            TextBox[] textBoxArray4 = { textBox4, textBox36, textBox19, textBox29, textBox39, textBox42, textBox45, textBox48 };  //unit
            TextBox[] textBoxArray5 = { textBox34, textBox35, textBox18, textBox25, textBox38, textBox41, textBox44, textBox47 };  //lower limit
            TextBox[] textBoxArray6 = { textBox33, textBox5, textBox17, textBox21, textBox37, textBox40, textBox43, textBox46 };  //upper limit

            CheckBox[] checkBoxArray = { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8};

            Label[] labelArray =
            {
                label2,  label3,  label7,  label10, label22, label19, label16, label13, 
                label26, label28, label31, label34, label37, label40, label43, label46
            };


            for (i = 0; i < gVariable.MAX_NUM_ADC; i++)
            {
                if(i < gVariable.craftList[gVariable.boardIndexSelected].paramNumber)
                {
                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelEnabled[i] = 1;
                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelTitle[i] = gVariable.craftList[gVariable.boardIndexSelected].paramName[i];
                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelUnit[i] = gVariable.craftList[gVariable.boardIndexSelected].paramUnit[i];
                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].lowerRange[i] = gVariable.craftList[gVariable.boardIndexSelected].rangeLowerLimit[i];
                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].upperRange[i] = gVariable.craftList[gVariable.boardIndexSelected].rangeUpperLimit[i];
                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].lowerLimit[i] = gVariable.craftList[gVariable.boardIndexSelected].paramLowerLimit[i];
                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].upperLimit[i] = gVariable.craftList[gVariable.boardIndexSelected].paramUpperLimit[i];
                }
                else
                {
                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelEnabled[i] = 0;
                }
            }

            for (i = 0; i < gVariable.MAX_NUM_ADC; i++)
            {
                if (gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelEnabled[i] == 1)
                {
                    checkBoxArray[i].Checked = true;
                        
                    textBoxArray1[i].Text = gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelTitle[i];
                    textBoxArray4[i].Text = gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelUnit[i];

                    textBoxArray2[i].Text = gVariable.ADCChannelInfo[gVariable.boardIndexSelected].lowerRange[i].ToString();
                    textBoxArray3[i].Text = gVariable.ADCChannelInfo[gVariable.boardIndexSelected].upperRange[i].ToString();
                    textBoxArray5[i].Text = gVariable.ADCChannelInfo[gVariable.boardIndexSelected].lowerLimit[i].ToString();
                    textBoxArray6[i].Text = gVariable.ADCChannelInfo[gVariable.boardIndexSelected].upperLimit[i].ToString();
                }
            }

            for (i = 0; i < 16; i++)
            {
                labelArray[i].Text = "~";
            }
        }

        private void setting1_FormClosing(object sender, EventArgs e)
        {
            dispatchUI.dispatchUIClass.Show();
        }

        private void initVariables()
        {
            this.Text = "设备 " + gVariable.currentCurveDatabaseName + " ADC 功能设定";

            if (gVariable.ADCChannelInfo[gVariable.boardIndexSelected].workingVoltage == 0)
                this.radioButton1.Checked = true;
            else
                this.radioButton2.Checked = true;
        }

        //ok button
        private void button1_Click(object sender, EventArgs e)
        {
            putSetting1DataToCraft();

            //we are control this board for setting function
            gVariable.whereComesTheSettingData = gVariable.boardIndexSelected;
            gVariable.whatSettingDataModified = gVariable.ADC_SETTING_DATA_TO_BOARD;

            this.Close();
        }

        //cancel button
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void saveSetting1DataToDatabase()
        {
            int i;

            for (i = 0; i < gVariable.MAX_NUM_ADC; i++)
            {
//                if (checkBoxArray[i].Checked == true) //ADC setting effective
                {
//                    gVariable.settingFromPC = 1;
//                    gVariable.settingValueExist = 1;

//                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelEnabled[i] = 1;
//                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelTitle[i] = textBoxArray1[i].Text;
//                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelUnit[i] = textBoxArray4[i].Text;

//                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].lowerRange[i] = (float)Convert.ToDouble(textBoxArray2[i].Text);
//                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].upperRange[i] = (float)Convert.ToDouble(textBoxArray3[i].Text);
//                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].lowerLimit[i] = (float)Convert.ToDouble(textBoxArray5[i].Text);
//                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].upperLimit[i] = (float)Convert.ToDouble(textBoxArray6[i].Text);
//                    gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelEnabled[i] = 0;
                }
            }

        }

        private void putSetting1DataToCraft()
        {
            int i;
            int num;
            TextBox[] textBoxArray1 = { textBox2, textBox8, textBox12, textBox16, textBox32, textBox28, textBox24, textBox20 };  //title
            TextBox[] textBoxArray2 = { textBox1, textBox7, textBox11, textBox15, textBox31, textBox27, textBox23, textBox13 };  //lower range
            TextBox[] textBoxArray3 = { textBox3, textBox6, textBox10, textBox14, textBox30, textBox26, textBox22, textBox9 };  //upper range
            TextBox[] textBoxArray4 = { textBox4, textBox36, textBox19, textBox29, textBox39, textBox42, textBox45, textBox48 };  //unit
            TextBox[] textBoxArray5 = { textBox34,textBox35, textBox18, textBox25, textBox38, textBox41, textBox44, textBox47 };  //lower limit
            TextBox[] textBoxArray6 = { textBox33,textBox5,  textBox17, textBox21, textBox37, textBox40, textBox43, textBox46 };  //upper limit

            CheckBox[] checkBoxArray = { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8};

            try
            {
                num = 0;

                for (i = 0; i < gVariable.MAX_NUM_ADC; i++)
                {
                    if (checkBoxArray[i].Checked == true) //ADC setting effective
                    {
//                        gVariable.settingFromPC = 1;
//                        gVariable.settingValueExist = 1;

                        gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelEnabled[i] = 1;
                        gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelTitle[i] = textBoxArray1[i].Text;
                        gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelUnit[i] = textBoxArray4[i].Text;

                        if (toolClass.isNumericOrNot(textBoxArray2[i].Text) == 1)
                        {
                            gVariable.ADCChannelInfo[gVariable.boardIndexSelected].lowerRange[i] = (float)Convert.ToDouble(textBoxArray2[i].Text);
                        }
                        else
                        {
                            gVariable.ADCChannelInfo[gVariable.boardIndexSelected].lowerRange[i] = 0;
                        }

                        if (toolClass.isNumericOrNot(textBoxArray3[i].Text) == 1)
                        {
                            gVariable.ADCChannelInfo[gVariable.boardIndexSelected].upperRange[i] = (float)Convert.ToDouble(textBoxArray3[i].Text);
                        }
                        else
                        {
                            gVariable.ADCChannelInfo[gVariable.boardIndexSelected].upperRange[i] = 0;
                        }

                        if (toolClass.isNumericOrNot(textBoxArray5[i].Text) == 1)
                        {
                            gVariable.ADCChannelInfo[gVariable.boardIndexSelected].lowerLimit[i] = (float)Convert.ToDouble(textBoxArray5[i].Text);
                        }
                        else
                        {
                            gVariable.ADCChannelInfo[gVariable.boardIndexSelected].lowerLimit[i] = 0;
                        }

                        if (toolClass.isNumericOrNot(textBoxArray6[i].Text) == 1)
                        {
                            gVariable.ADCChannelInfo[gVariable.boardIndexSelected].upperLimit[i] = (float)Convert.ToDouble(textBoxArray6[i].Text);
                        }
                        else
                        {
                            gVariable.ADCChannelInfo[gVariable.boardIndexSelected].upperLimit[i] = 0;
                        }

                        num++;
                    }
                    else
                    {
                        gVariable.ADCChannelInfo[gVariable.boardIndexSelected].channelEnabled[i] = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("setting1 get setting data failed!!" + ex);
            }
        }

    }
}

