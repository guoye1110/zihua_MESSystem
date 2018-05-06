using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using MESSystem.common;

namespace MESSystem.machine
{
    public partial class mDispatchList : Form
    {
        const int totalNumOfCheckboxes = 40;
        public static mDispatchList mDispatchListClass = null;

        int machineSelected;  //machine index
        int taskType;  //routine check/maintenance/repairing task are 0/1/2 respectively
        int taskSelected;  //task index for this machine (for routine task, it is check/add oil/washup, for maintenance, it is maintenace)
        string notesData;

        DataTable dTable;
        CheckBox[] checkBoxArray = new CheckBox[totalNumOfCheckboxes];

        public mDispatchList(int machineSelected_, int taskType_, int taskSelected_, DataTable dt)
        {
            dTable = dt;
            machineSelected = machineSelected_;
            taskType = taskType_;
            taskSelected = taskSelected_;
            notesData = null;

            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        private void listView_Load(object sender, EventArgs e)
        {
            int i, j;
            int len;
            int numOfColumn;
            int[,] widthArrayRoutine = 
            {
                { 65, 120, 70, 90, 120, 50, 110, 560, 80, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120 },  //daily check
                { 65, 120, 70, 90, 120, 50, 50,  80, 400, 120, 80, 120, 120, 120, 120, 120, 120, 120, 120 },  //add oil 
                { 65, 120, 70, 90, 120, 50, 50,  80, 400, 120, 80, 120, 120, 120, 120, 120, 120, 120, 120 },  // washup
            };

            int[,] widthArrayMaintenance = 
            {
                { 65, 120, 70, 90, 120, 120, 50, 120, 120, 80, 50, 80, 120, 80, 120, 120, 120, 120, 120 },  //maintenance
            };

            int[,] widthArrayRepairing = 
            {
                { 65, 120, 90, 120, 120, 50, 200, 200, 200, 80, 120, 120, 80, 120, 120, 120, 120, 120 },  //repairing
            };

            checkBoxArray[0] = checkBox1;
            checkBoxArray[1] = checkBox2;
            checkBoxArray[2] = checkBox3;  
            checkBoxArray[3] = checkBox4;  
            checkBoxArray[4] = checkBox5;  
            checkBoxArray[5] = checkBox6;  
            checkBoxArray[6] = checkBox7;  
            checkBoxArray[7] = checkBox8;  
            checkBoxArray[8] = checkBox9;  
            checkBoxArray[9] = checkBox10;
            checkBoxArray[10] = checkBox11;
            checkBoxArray[11] = checkBox12;
            checkBoxArray[12] = checkBox13;
            checkBoxArray[13] = checkBox14;
            checkBoxArray[14] = checkBox15;
            checkBoxArray[15] = checkBox16;
            checkBoxArray[16] = checkBox17;
            checkBoxArray[17] = checkBox18;
            checkBoxArray[18] = checkBox19;
            checkBoxArray[19] = checkBox20;
            checkBoxArray[20] = checkBox21;
            checkBoxArray[21] = checkBox22;
            checkBoxArray[22] = checkBox23;
            checkBoxArray[23] = checkBox24;
            checkBoxArray[24] = checkBox25;
            checkBoxArray[25] = checkBox26;
            checkBoxArray[26] = checkBox27;
            checkBoxArray[27] = checkBox28;
            checkBoxArray[28] = checkBox29;
            checkBoxArray[29] = checkBox30;
            checkBoxArray[30] = checkBox31;
            checkBoxArray[31] = checkBox32;
            checkBoxArray[32] = checkBox33;
            checkBoxArray[33] = checkBox34;
            checkBoxArray[34] = checkBox35;
            checkBoxArray[35] = checkBox36;
            checkBoxArray[36] = checkBox37;
            checkBoxArray[37] = checkBox38;
            checkBoxArray[38] = checkBox39;
            checkBoxArray[39] = checkBox40;

            this.listView1.BeginUpdate();
            listView1.GridLines = true;
            listView1.Dock = DockStyle.Fill;

            listView1.CheckBoxes = true;

            i = 0;
            len = 0;
            listView1.Columns.Add(" ", 1, HorizontalAlignment.Center);
            if(taskType == machineManagement.Task_Type_Routine)
                listView1.Columns.Add("结果", widthArrayRoutine[taskSelected, 0], HorizontalAlignment.Center);
            else if (taskType == machineManagement.Task_Type_Maintenance)
                listView1.Columns.Add("结果", widthArrayMaintenance[taskSelected, 0], HorizontalAlignment.Center);
            else // if (taskType == machineManagement.Task_Type_Repairing)
                listView1.Columns.Add("结果", widthArrayRepairing[taskSelected, 0], HorizontalAlignment.Center);

            foreach (DataRow dr in dTable.Rows)
            {
                len = dr.ItemArray.Length;

                for (i = 0; i < len; i++)
                {
                    if (taskType == machineManagement.Task_Type_Routine)
                        listView1.Columns.Add(dr[i].ToString().Trim(), widthArrayRoutine[taskSelected, i + 1], HorizontalAlignment.Center);
                    else if (taskType == machineManagement.Task_Type_Maintenance)
                        listView1.Columns.Add(dr[i].ToString().Trim(), widthArrayMaintenance[taskSelected, i + 1], HorizontalAlignment.Center);
                    else // if (taskType == machineManagement.Task_Type_Repairing)
                        listView1.Columns.Add(dr[i].ToString().Trim(), widthArrayRepairing[taskSelected, i + 1], HorizontalAlignment.Center);
                }
                break;
            }
            numOfColumn = i;

            j = -1;
            foreach (DataRow dr in dTable.Rows)
            {
                ListViewItem OptionItem = new ListViewItem();

                if(j == -1)
                {
                    j++;
                    continue;
                }

                OptionItem.SubItems.Add("");
                setCheckBox(j, checkBoxArray[j]);

                j++;
                for(i = 0; i < len; i++)
                    OptionItem.SubItems.Add(dr[i].ToString().Trim());

                listView1.Items.Add(OptionItem);
            }

            for (; j < totalNumOfCheckboxes; j++)
                checkBoxArray[j].Visible = false;

            this.listView1.EndUpdate();
        }


        private void setCheckBox(int i, CheckBox checkBox)
        {
            checkBox.AutoSize = false;
            checkBox.Location = new System.Drawing.Point(21, 43 + i * 17);
            checkBox.Name = "checkBox" + (i + 1);
            checkBox.Size = new System.Drawing.Size(50, 15);
            checkBox.TabIndex = i + 2;
            checkBox.Text = "OK";
            checkBox.BackColor = System.Drawing.SystemColors.Window; 
            checkBox.UseVisualStyleBackColor = true;
            checkBox.Enabled = true;
        }


        //when the user clicked listview by mouse, we need to check if he want to add some notes or selected a checkbox
        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            int i;
            int columnX;
            ListViewItem item;

            item = listView1.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                columnX = 0;
                for (i = 0; i < listView1.Columns.Count; i++)
                {
                    if (e.X >= columnX && e.X < columnX + listView1.Columns[i].Width)
                    {
                        if (i == 10)  //notes column
                        {
                            this.Enabled = false;
                            popupEditBox.popupEditBoxClass = new popupEditBox();
                            popupEditBox.popupEditBoxClass.Show();
                            item.SubItems[10].Text = popupEditBox.popupEditBoxClass.popupEditBoxStarted();
                            notesData = item.SubItems[10].Text;
                            this.Enabled = true;
                            break;
                        }
                        else if(i == 1)  //checkbox column, this dispatch need to be recorded in database
                        {
                            checkBoxArray[item.Index].Checked = true;
                        }
                    }
                    columnX += listView1.Columns[i].Width;
                }
            }
        }


        //useless for the time being
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Console.Write("listView1_SelectedIndexChanged failed. ", ex);
            }
        }

        //save completed dispatch
        private void button1_Click(object sender, EventArgs e)
        {
            int i;
            int len;
            int databaseType; //type value defined in database, 0 is dispatch list, 1 is material list... 16 is daily check, 17 is add oil, 18 is washup, 19 is maintenance 
            int index;
            int first;
            string commandText;
            string[] titleStrArray;
            string databaseName;

            databaseType = machineManagement.getDatabaseType(taskType, taskSelected);

            if (databaseType == -1)
                return;

            index = 0;
            first = 0;
            try
            {
                databaseName = gVariable.DBHeadString + (machineSelected + 1).ToString().PadLeft(3, '0');

                titleStrArray = mySQLClass.insertDataTableString[databaseType].Split(',', ')');
                foreach (DataRow dr in dTable.Rows)
                {
                    if (first == 0)
                    {
                        first++;
                        continue;
                    }
                    first++;

                    if (checkBoxArray[index].Checked == true)
                    {
                        len = dr.ItemArray.Length + 1; //excel column number plus 1 for ID at the beginning

                        commandText = "insert into `" + gVariable.machineManagementTableName[taskType, taskSelected] + "`" + mySQLClass.insertDataTableString[databaseType];

                        MySqlParameter[] param = new MySqlParameter[len];

                        param[0] = new MySqlParameter("@id", 0);
                        for (i = 1; i < len - 2; i++)  //the second last column is user name, and the last one is user notes
                        {
                            param[i] = new MySqlParameter(titleStrArray[i].Trim(), dr[i - 1].ToString().Trim());
                        }
                        param[i] = new MySqlParameter(titleStrArray[i].Trim(), gVariable.userAccount);
                        i++;
                        param[i] = new MySqlParameter(titleStrArray[i].Trim(), notesData);

                        switch (gVariable.secondFunctionIndex)
                        {
                            case gVariable.MACHINE_MANAGEMENT_STATUS:
                                break;
                            case gVariable.MACHINE_MANAGEMENT_ITEM_CHECKING:
                                param[4] = new MySqlParameter(titleStrArray[4].Trim(), DateTime.Now.ToString("yyyy-MM-dd"));
                                break;
                            case gVariable.MACHINE_MANAGEMENT_MAINTENANCE:
                                param[8] = new MySqlParameter(titleStrArray[8].Trim(), DateTime.Now.ToString("yyyy-MM-dd"));
                                break;
                            case gVariable.MACHINE_MANAGEMENT_REPAIRING:
                                param[4] = new MySqlParameter(titleStrArray[4].Trim(), DateTime.Now.ToString("yyyy-MM-dd"));
                                break;
                        }
                        mySQLClass.databaseNonQueryAction(databaseName, commandText, param, gVariable.appendRecord);
                    }
                    index++;
                }

                gVariable.machineListFresh = 1;
                this.Close();
            }
            catch (Exception ex)
            {
                Console.Write("save completed mantenance/repairing dispatch failed! " + ex);
            }
        }

        private void dispatchList_FormClosing(object sender, EventArgs e)
        {
            if (machineManagement.machineManagementClass != null)
                machineManagement.machineManagementClass.Show();
        }

        //exit
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
