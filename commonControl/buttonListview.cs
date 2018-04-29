using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;

namespace MESSystem.commonControl
{
    //this list view will fist read excel file to get titles for a list view
    //then read database table related to the excel file and put all data inside this table into list view
    //***********************
    //this is a simple version, if the table has too many lines, resource may get exhausted when read table data into memory
    //***********************
    public partial class buttonListview : Form
    {
        const int BUTTON_NUM = 6;

        int len;
        int buttonIndex;
        string databaseName;
        string tableName;
        string fileName;
        string title;
        string [] buttonName;
        Button[] buttonArray = new Button[BUTTON_NUM];

        public static buttonListview buttonListviewClass = null; //用来引用主窗口

        //buttonNameArray: this is a button name list that need to be displayed on top of screen
        //buttonIndex: this button is the function we come from, so this button should not be displayed, for example:
        //             we have 4 buttons: aa, bb, cc, dd, the user selected dd to enter this function, he can only see aa/bb/cc 3 buttons
        public buttonListview(string databaseName_, string tableName_, string fileName_, string [] buttonNameArray, int buttonIndex_)
        {
            int i;
            int len;
            databaseName = databaseName_;
            tableName = tableName_;
            fileName = fileName_;

            buttonIndex = buttonIndex_ - 1;
            if (buttonIndex < 0)
                buttonIndex = 0;

            title = buttonNameArray[buttonIndex];

            len = buttonNameArray.Length;
            buttonName = new string[len];

            for (i = 0; i < len; i++)
                buttonName[i] = buttonNameArray[i];

            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        void buttonDefinition(int i)
        {
            int x, y, h, w;
            int ceilingValue = 10;  //this is ceiling for windows screen, its title area
            int start_x, start_y;
            int width, height;
            int gap;
            Rectangle rect = new Rectangle();

            start_x = 3;
            start_y = 4;

            width = 120;
            height = 45;
            gap = 6;

            rect = Screen.GetWorkingArea(this);

            buttonArray[i].AutoEllipsis = true;
            buttonArray[i].FlatAppearance.BorderSize = 3;
            buttonArray[i].FlatStyle = System.Windows.Forms.FlatStyle.System;
            if (gVariable.resolutionLevel >= gVariable.resolution_1600)
            {
                buttonArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                x = start_x;
                y = start_y;
                h = height;
                w = width;
            }
            else if (gVariable.resolutionLevel >= gVariable.resolution_1280)
            {
                buttonArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                x = start_x;
                y = start_y;
                h = height;
                w = width;
            }
            else //if (gVariable.resolutionLevel == gVariable.resolution_1024)
            {
                buttonArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                x = start_x * rect.Width / gVariable.EFFECTIVE_SCREEN_X;
                y = (start_y - ceilingValue) * rect.Height / gVariable.EFFECTIVE_SCREEN_Y + ceilingValue;
                h = height * rect.Height / gVariable.EFFECTIVE_SCREEN_Y;
                w = width * rect.Width / gVariable.EFFECTIVE_SCREEN_X;
            }
            buttonArray[i].Location = new System.Drawing.Point(x + i * (gap + w), y);
            buttonArray[i].Size = new System.Drawing.Size(w, h);
            buttonArray[i].UseVisualStyleBackColor = true;
        }

        private void listView_Load(object sender, EventArgs e)
        {
            int i, j;
            int len1, len2;
            int ret;
            string[] tableTitle;
            string[, ] tableArray;
            string commandText;
            Button[] bArray = { button1, button2, button3, button4, button5, button6 };

            tableTitle = null;
            tableArray = null;
            try
            {
                len = buttonName.Length;
                for (i = 0; i < len; i++)
                {
                    buttonArray[i] = bArray[i];
                    buttonArray[i].Text = buttonName[i];
                    buttonDefinition(i);
                }

                for (; i < BUTTON_NUM; i++)
                {
                    buttonArray[i] = bArray[i];
                    buttonArray[i].Visible = false;
                }


                ret = mySQLClass.getListTitleFromExcel(fileName, ref tableTitle, gVariable.EXCEL_FIRSTLINE_DATA);
                if(ret < 0)
                {
                    //fill buttonListviewTitle[]
                }

                commandText = "select * from `" + tableName + "`";
                tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);
                if (tableArray != null)
                {
                    this.listView1.BeginUpdate();
                    listView1.GridLines = true;
                    listView1.Dock = DockStyle.Fill;
                    listView1.Columns.Add(" ", 1, HorizontalAlignment.Center);

                    for (i = 0; i < tableTitle.Length; i++)
                    {
                        len1 = System.Text.Encoding.Default.GetBytes(tableTitle[i]).Length;
                        listView1.Columns.Add(tableTitle[i], len1 * 10, HorizontalAlignment.Center);
                    }

                    len1 = tableArray.GetLength(0);
                    len2 = tableArray.GetLength(1);
                    for(i = 0; i < len1; i++)
                    {
                        ListViewItem OptionItem = new ListViewItem();

                        for (j = 1; j < len2; j++ )
                            OptionItem.SubItems.Add(tableArray[i, j]);

                        listView1.Items.Add(OptionItem);
                    }
                    this.listView1.EndUpdate();
                }

                this.Text = gVariable.programTitle + title;
                groupBox4.Text = title;
            }
            catch (Exception ex)
            {
                Console.WriteLine("buttonListview listView_load failed! ", ex);
            }
        }

        private void dispatchList_FormClosing(object sender, EventArgs e)
        {
//            dispatchUI.dispatchUIClass.Show();
//            dispatchUI.closeReason = 0;
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
                Console.WriteLine("buttonListview select failed " + ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gVariable.buttonListviewSelected = gVariable.BUTTON_1_SELECTED;
            if (gVariable.buttonListviewSelected != gVariable.buttonListviewSelectedOld)
                this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gVariable.buttonListviewSelected = gVariable.BUTTON_2_SELECTED;
            if (gVariable.buttonListviewSelected != gVariable.buttonListviewSelectedOld)
                this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            gVariable.buttonListviewSelected = gVariable.BUTTON_3_SELECTED;
            if (gVariable.buttonListviewSelected != gVariable.buttonListviewSelectedOld)
                this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            gVariable.buttonListviewSelected = gVariable.BUTTON_4_SELECTED;
            if (gVariable.buttonListviewSelected != gVariable.buttonListviewSelectedOld)
                this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            gVariable.buttonListviewSelected = gVariable.BUTTON_5_SELECTED;
            if (gVariable.buttonListviewSelected != gVariable.buttonListviewSelectedOld)
                this.Close();
        }

        //normally we should not come this place, because we have only 6 buttons, so this button is alwasy invisible
        private void button6_Click(object sender, EventArgs e)
        {
             gVariable.buttonListviewSelected = gVariable.BUTTON_6_SELECTED;
             if (gVariable.buttonListviewSelected != gVariable.buttonListviewSelectedOld)
                 this.Close();
        }
    }
}
