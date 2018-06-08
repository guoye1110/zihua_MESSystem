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
    public partial class APSBatchResult : Form
    {
        int printProcessExist;
        string productBatchNUm;
        float screenRatioX, screenRatioY;

        public APSBatchResult(string productBatchNUm_)
        {
            productBatchNUm = productBatchNUm_;

            InitializeComponent();
            initVariables();
            resizeScreen();
        }

        void initVariables()
        {
            //int i;

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            label1.Text = "生产任务单排程结果查询";
            this.Text = "生产任务单排程结果查询";

            //this.TopMost = true;
        }

        void resizeScreen()
        {
            int i;
            int x, y, w, h;
            float fontSize;
            GroupBox[] groupBoxArray = { groupBox1, groupBox2, groupBox3, groupBox4, groupBox5, groupBox6 };
            Label[] labelArray = { label2, label3, label4, label5, label6 };
            TextBox[] textBoxArray = { textBox1, textBox2, textBox3, textBox4, textBox5 };
            Button[] buttonArray = { button1 };
            //ComboBox[] comboBoxArray = { comboBox3, comboBox4, comboBox6 };
            //CheckBox[] checkBoxArray = { checkBox3 };
            //DateTimePicker[] timePickerArray = { dateTimePicker3, dateTimePicker4 };
            float[,] commonFontSize = { 
                                        { 7F,  8F,  9F,  10F, 11F,  12F}, 
                                        { 6F,  6.5F,  8F, 8.5F, 9F,  10F},  
                                        { 5.5F, 6F, 6.5F, 7F, 7.5F, 8F},  
                                     };

            //float[] commonFontSize = { 6F, 6.5F, 7F, 7.5F, 8F, 8.5F };
            float[] titleFontSize = { 20F, 22F, 23F, 24F, 25F, 28F };
            Rectangle rect = new Rectangle();

            screenRatioX = gVariable.screenRatioX;
            screenRatioY = gVariable.screenRatioY;

            fontSize = commonFontSize[gVariable.dpiValue, gVariable.resolutionLevel];

            rect = Screen.GetWorkingArea(this);
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", titleFontSize[gVariable.resolutionLevel], System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            x = (rect.Width - label1.Size.Width) / 2;
            y = (int)(label1.Location.Y * screenRatioY);
            label1.Location = new System.Drawing.Point(x, y);

            button1.Visible = false;

            for (i = 0; i < groupBoxArray.Length; i++)
            {
                groupBoxArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(groupBoxArray[i].Size.Width * screenRatioX);
                h = (int)(groupBoxArray[i].Size.Height * screenRatioY);
                groupBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(groupBoxArray[i].Location.X * screenRatioX);
                y = (int)(groupBoxArray[i].Location.Y * screenRatioY);
                groupBoxArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < labelArray.Length; i++)
            {
                labelArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(labelArray[i].Size.Width * screenRatioX);
                h = (int)(labelArray[i].Size.Height * screenRatioY);
                labelArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(labelArray[i].Location.X * screenRatioX);
                y = (int)(labelArray[i].Location.Y * screenRatioY);
                labelArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < textBoxArray.Length; i++)
            {
                textBoxArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(textBoxArray[i].Size.Width * screenRatioX);
                h = (int)(textBoxArray[i].Size.Height * screenRatioY);
                textBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(textBoxArray[i].Location.X * screenRatioX);
                y = (int)(textBoxArray[i].Location.Y * screenRatioY);
                textBoxArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < buttonArray.Length; i++)
            {
                buttonArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(buttonArray[i].Size.Width * screenRatioX);
                h = (int)(buttonArray[i].Size.Height * screenRatioY);
                buttonArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(buttonArray[i].Location.X * screenRatioX);
                y = (int)(buttonArray[i].Location.Y * screenRatioY);
                buttonArray[i].Location = new System.Drawing.Point(x, y);
            }

            /*
            for (i = 0; i < checkBoxArray.Length; i++)
            {
                w = (int)(checkBoxArray[i].Size.Width * screenRatioX);
                h = (int)(checkBoxArray[i].Size.Height * screenRatioY);
                checkBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(checkBoxArray[i].Location.X * screenRatioX);
                y = (int)(checkBoxArray[i].Location.Y * screenRatioY);
                checkBoxArray[i].Location = new System.Drawing.Point(x, y);
            }
             

            for (i = 0; i < comboBoxArray.Length; i++)
            {
                w = (int)(comboBoxArray[i].Size.Width * screenRatioX);
                h = (int)(comboBoxArray[i].Size.Height * screenRatioY);
                comboBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(comboBoxArray[i].Location.X * screenRatioX);
                y = (int)(comboBoxArray[i].Location.Y * screenRatioY);
                comboBoxArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < timePickerArray.Length; i++)
            {
                w = (int)(timePickerArray[i].Size.Width * screenRatioX);
                h = (int)(timePickerArray[i].Size.Height * screenRatioY);
                timePickerArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(timePickerArray[i].Location.X * screenRatioX);
                y = (int)(timePickerArray[i].Location.Y * screenRatioY);
                timePickerArray[i].Location = new System.Drawing.Point(x, y);
            }
            */
            //listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        private void APSBatchResult_Load(object sender, EventArgs e)
        {
            fillInTextBoxContents();
            taskListviewFunc();
            materialListviewFunc();
        }

        void fillInTextBoxContents()
        {
            string commandText;
            string[,] batchTableArray;

            try
            {
                commandText = "select * from `" + gVariable.productBatchTableName + "` where batchNum = '" + productBatchNUm + "'";
                batchTableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (batchTableArray == null)
                    return;

                textBox1.Text = batchTableArray[0, 2];
                textBox2.Text = batchTableArray[0, 1];
                textBox3.Text = batchTableArray[0, 19];
                textBox4.Text = batchTableArray[0, 4];
                textBox5.Text = batchTableArray[0, 8];
            }
            catch (Exception ex)
            {
                Console.Write("fillInContents for APS batch result failed! " + ex);
            }

        }

        void taskListviewFunc()
        {
            int i, j, k;
            int index;
            int minutes;
            int status;
            string commandText;
            string[,] batchTableArray;
            ListViewItem OptionItem;
            ListView[] listViewArray = { listView1, listView2, listView3, };

            int[] productBatchLenArray = { 5, 100, 100, 110, 110, 90, 100, 80 };
            string[] productBatchListHeader = 
            {
                "", "任务单编号", "承接设备", "计划开始时间", "计划完成时间", "总工时", "物料总量", "任务单状态"
            };

            try
            {
                printProcessExist = 0;

                commandText = "select * from `" + gVariable.globalProductTaskTableName + "` where batchNum = '" + productBatchNUm + "'";
                batchTableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (batchTableArray == null)
                    return;

                if (batchTableArray.GetLength(0) == 2)
                    printProcessExist = 0;
                else
                    printProcessExist = 1;

                k = 0;
                for (i = 0; i < 3; i++)
                {
                    listViewArray[i].Clear();

                    listViewArray[i].BeginUpdate();

                    listViewArray[i].GridLines = true;
                    listViewArray[i].Dock = DockStyle.Fill;

                    for (j = 0; j < productBatchLenArray.Length; j++)
                        listViewArray[i].Columns.Add(productBatchListHeader[j], (int)(productBatchLenArray[j] * screenRatioX), HorizontalAlignment.Center);

                    OptionItem = new ListViewItem();

                    //if print process doesnot exist, leave a empty listview
                    if (printProcessExist == 1 || i != 1)
                    {
                        //OptionItem.SubItems.Add((i + 1).ToString());
                        OptionItem.SubItems.Add(batchTableArray[k, 2]);
                        index = Convert.ToInt32(batchTableArray[k, 1]) - 1;
                        OptionItem.SubItems.Add(gVariable.machineNameArrayAPS[index]);
                        OptionItem.SubItems.Add(batchTableArray[k, 3]);
                        OptionItem.SubItems.Add(batchTableArray[k, 4]);

                        minutes = (toolClass.timeStringToTimeStamp(batchTableArray[k, 4]) - toolClass.timeStringToTimeStamp(batchTableArray[k, 3])) / 60;
                        OptionItem.SubItems.Add(minutes + "分钟");

                        OptionItem.SubItems.Add(batchTableArray[k, 17] + " kg");

                        status = Convert.ToInt16(batchTableArray[k, 16]);
                        if (status >= gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED)
                            OptionItem.SubItems.Add("已发布");
                        else if (status >= gVariable.MACHINE_STATUS_DISPATCH_CONFIRMED)
                            OptionItem.SubItems.Add("已确认");
                        else
                            OptionItem.SubItems.Add("已排程");

                        k++;
                    }
                    else
                    {
                        OptionItem.SubItems.Add("没有印刷任务单");
                    }

                    listViewArray[i].Items.Add(OptionItem);

                    listViewArray[i].EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Console.Write("screen fresh for APS batch result failed! " + ex);
            }
        }

        void materialListviewFunc()
        {
            int i, j, k;
            int num;
            //string aa;
            string commandText;
            string[,] batchTableArray;
            ListViewItem OptionItem;
            ListView[] listViewArray = { listView4, listView5, listView6 };

            int[] productBatchLenArray = { 5, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            string[] productBatchListHeader = 
            {
                "", "物料单编号", "承接设备", "物料信息1", "物料信息2", "物料信息3", "物料信息4", "物料信息5", "物料信息6", "物料信息7", 
            };

            try
            {
                commandText = "select * from `" + gVariable.globalMaterialTaskTableName + "` where batchNum = '" + productBatchNUm + "'";
                batchTableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (batchTableArray == null)
                    return;

                if (batchTableArray.GetLength(0) == 2)
                    printProcessExist = 0;
                else
                    printProcessExist = 1;

                k = 0;
                for (i = 0; i < 3; i++)
                {
                    num = Convert.ToInt32(batchTableArray[k, 5]); 

                    listViewArray[i].Clear();

                    listViewArray[i].BeginUpdate();

                    listViewArray[i].GridLines = true;
                    listViewArray[i].Dock = DockStyle.Fill;

                    for (j = 0; j < productBatchLenArray.Length; j++)
                        listViewArray[i].Columns.Add(productBatchListHeader[j], (int)(productBatchLenArray[j] * screenRatioX), HorizontalAlignment.Center);

                    OptionItem = new ListViewItem();

                    //if print process doesnot exist, leave a empty listview
                    if (printProcessExist == 1 || i != 1)
                    {
                        OptionItem.SubItems.Add(batchTableArray[k, 2]);
                        OptionItem.SubItems.Add(batchTableArray[k, 4]);
                        OptionItem.SubItems.Add(batchTableArray[k, 6]);
                        OptionItem.SubItems.Add(batchTableArray[k, 8]);
                        OptionItem.SubItems.Add(batchTableArray[k, 10]);
                        OptionItem.SubItems.Add(batchTableArray[k, 12]);
                        OptionItem.SubItems.Add(batchTableArray[k, 14]);
                        OptionItem.SubItems.Add(batchTableArray[k, 16]);
                        OptionItem.SubItems.Add(batchTableArray[k, 18]);
                    }
                    else
                    {
                        OptionItem.SubItems.Add("没有油墨物料单");
                    }
                    listViewArray[i].Items.Add(OptionItem);

                    OptionItem = new ListViewItem();

                    if (printProcessExist == 1 || i != 1)
                    {
                        j = 0;
                        if (i < 2)
                        {
                            OptionItem.SubItems.Add("");
                            OptionItem.SubItems.Add("");
                            OptionItem.SubItems.Add(batchTableArray[k, 7] + "kg");
                            j++;
                            OptionItem.SubItems.Add(batchTableArray[k, 9] + "kg");
                            j++;
                            OptionItem.SubItems.Add(batchTableArray[k, 11] + "kg");
                            j++;
                            if (j >= num)
                                OptionItem.SubItems.Add("");
                            else
                                OptionItem.SubItems.Add(batchTableArray[k, 13] + "kg");
                            j++;
                            if (j >= num)
                                OptionItem.SubItems.Add("");
                            else
                                OptionItem.SubItems.Add(batchTableArray[k, 15] + "kg");
                            j++;
                            if (j >= num)
                                OptionItem.SubItems.Add("");
                            else
                                OptionItem.SubItems.Add(batchTableArray[k, 17] + "kg");
                            j++;
                            if (j >= num)
                                OptionItem.SubItems.Add("");
                            else
                                OptionItem.SubItems.Add(batchTableArray[k, 19] + "kg");
                        }
                        else
                        {
                            OptionItem.SubItems.Add("");
                            OptionItem.SubItems.Add("");
                            OptionItem.SubItems.Add(batchTableArray[k, 7] + "块");
                            OptionItem.SubItems.Add(batchTableArray[k, 9] + "米");
                            OptionItem.SubItems.Add(batchTableArray[k, 11] + "块");
                            OptionItem.SubItems.Add(batchTableArray[k, 13] + "平方米");
                            OptionItem.SubItems.Add(batchTableArray[k, 15] + "平方米");
                            OptionItem.SubItems.Add(batchTableArray[k, 17] + "公斤");
                            OptionItem.SubItems.Add(batchTableArray[k, 19] + "公斤");
                        }
                        k++;
                    }
                    listViewArray[i].Items.Add(OptionItem);

                    listViewArray[i].EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Console.Write("screen fresh for APS UI failed! " + ex);
            }
        }
    
    }
}
