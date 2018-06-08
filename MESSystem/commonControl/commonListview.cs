using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.mainUI;
using MESSystem.common;

namespace MESSystem.commonControl
{
    //this list view will fist read excel file to get titles for a list view
    //then read database table related to the excel file and put all data inside this table into list view
    //***********************
    //this is a simple version, if the table has too many lines, resource may get exhausted when read table data into memory
    //***********************
    public partial class commonListview : Form
    {
        int numOfColumn;
        float screenRatioX;
        float screenRatioY;
        string databaseName;
        string tableName;
        string fileName;

        public static commonListview commonListviewClass = null; //用来引用主窗口

        //numOfColumn_ means how many columns do we want to display, if the number is 0, then we need to display all the columns in the table
        public commonListview(string databaseName_, string tableName_, string fileName_, string title, int numOfColumn_)
        {
            databaseName = databaseName_;
            tableName = tableName_;
            fileName = fileName_;
            numOfColumn = numOfColumn_;

            InitializeComponent();
            initializeVariables(title);
            resizeScreen();
        }

        private void initializeVariables(string title)
        {
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            this.Text = gVariable.enterpriseTitle + title;
            label1.Text = gVariable.enterpriseTitle + title;
            groupBox4.Text = title;
        }

        void resizeScreen()
        {
            int i;
            int x, y, w, h;
            float fontSize;
            Button[] buttonArray = { button1, button2, button3 };
            GroupBox[] groupBoxArray = { groupBox4 };
            float[,] commonFontSize = { 
                                        { 7F,  8F,  9F,  10F, 11F,  12F}, 
                                        { 6F,  7F,  8F, 8.5F, 9F,  10F},  
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
        }

        private void listView_Load(object sender, EventArgs e)
        {
            int i, j;
            int x;
            int num;
            int len1, len2;
            int ret;
            float ratio;
            float LISTVIEW_SIZE_X = 1248F;
            string[] tableTitle;
            string[, ] tableArray;
            string commandText;

            tableTitle = null;
            tableArray = null;
            try
            {
                ret = mySQLClass.getListTitleFromExcel(fileName, ref tableTitle, gVariable.EXCEL_FIRSTLINE_DATA);
                if(ret < 0)
                {
                    //fill commonListviewTitle[]
                }

                commandText = "select * from `" + tableName + "`";
                tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);

                if (numOfColumn == 0)
                    num = tableTitle.Length;
                else
                    num = numOfColumn;

                len1 = 45;
                for (i = 0; i < num; i++)
                {
                    len1 += (System.Text.Encoding.Default.GetBytes(tableTitle[i]).Length * 10);
                }

                ratio = LISTVIEW_SIZE_X * gVariable.screenRatioX / len1;

                if (ratio > 2.5)
                {
                    len1 = (int)(LISTVIEW_SIZE_X * gVariable.screenRatioX * 2.5f / ratio );

                    x = (int)((LISTVIEW_SIZE_X * gVariable.screenRatioX - len1) / 2);

                    groupBox4.Size = new System.Drawing.Size(len1, groupBox4.Size.Height);
                    groupBox4.Location = new System.Drawing.Point(x, groupBox4.Location.Y);

                    ratio = 2.5f;
                }

                if (tableArray != null)
                {
                    this.listView1.BeginUpdate();
                    listView1.GridLines = true;
                    listView1.Dock = DockStyle.Fill;
                    listView1.Columns.Add(" ", 1, HorizontalAlignment.Center);

                    listView1.Columns.Add("序号", (int)(45 * ratio), HorizontalAlignment.Center);

                    for (i = 0; i < num; i++)
                    {
                        len1 = System.Text.Encoding.Default.GetBytes(tableTitle[i]).Length * 10;
                        listView1.Columns.Add(tableTitle[i], (int)(len1 * ratio), HorizontalAlignment.Center);
                    }

                    len1 = tableArray.GetLength(0);
                    len2 = tableArray.GetLength(1);
                    for(i = 0; i < len1; i++)
                    {
                        ListViewItem OptionItem = new ListViewItem();

                        OptionItem.SubItems.Add((i + 1).ToString());
                        for (j = 1; j < len2; j++)
                            OptionItem.SubItems.Add(tableArray[i, j]);

                        listView1.Items.Add(OptionItem);
                    }
                    this.listView1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("commonListview listView_load failed! "+ ex);
            }
        }

        private void dispatchList_FormClosing(object sender, EventArgs e)
        {
            firstScreen.firstScreenClass.Show();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.listView1.SelectedItems.Count > 0)
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("commonListview select failed " + ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            manualInput manualInputImpl = new manualInput();
            manualInputImpl.Show();
        }
    }
}
