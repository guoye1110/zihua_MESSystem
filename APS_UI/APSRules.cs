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
    public partial class APSRules : Form
    {
        int machineSelected1;
        int machineSelected2;
        int machineSelected3;
        int endTimeRemoveSelected;

        int batchOrderIDInTable;
        int batchOrderIndexInList;
        int selectedBOMIndex;

        string[,] batchTableArray;

        string[] BOMArray = new string[3];

        public class APSRulesDef
        {
            //public int ruleAlreadyDefined;
            public int listviewIndex;
            public int assignedMachineID1;
            public int assignedMachineID2;
            public int assignedMachineID3;
            public int assignedStartTime;
            public int assignedEndTime;
            public int ignoreEndTime;
            public string BOMName;
            public int[] materialSelected;
            public string[] materialCode;
        };

        public APSRules(int batchOrderID, int batchOrderIndex)
        {
            batchOrderIDInTable = batchOrderID;
            batchOrderIndexInList = batchOrderIndex;

            InitializeComponent();
            initVariables();
        }

        void initVariables()
        {
            int i;

            string[] yesNo = { "是", "否" };
            string[] machineName1 = { "不指定", "1号流延机", "2号流延机", "3号流延机", "4号流延机", "5号流延机", "6号中试机", "5号吹膜机" };
            string[] machineName2 = { "不指定", "1号印刷机", "2号印刷机", "3号印刷机", "4号印刷机", "5号印刷机" };
            string[] machineName3 = { "不指定", "1号分切机", "3号分切机", "5号分切机", "6号分切机", "7号分切机" };

            machineSelected1 = APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedMachineID1;
            machineSelected2 = APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedMachineID2;
            machineSelected3 = APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedMachineID3;

            endTimeRemoveSelected = APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].ignoreEndTime;

            //start time not assigned
            if (APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedStartTime == -1)
            {
                dateTimePicker2.Format = DateTimePickerFormat.Custom;
                dateTimePicker2.CustomFormat = " ";
            }
            else
            {
                dateTimePicker2.Value = toolClass.GetTime(APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedStartTime.ToString());
            }

            //end time not assigned
            if (APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedEndTime == -1)
            {
                dateTimePicker3.Format = DateTimePickerFormat.Custom;
                dateTimePicker3.CustomFormat = " ";
            }
            else
            {
                dateTimePicker3.Value = toolClass.GetTime(APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedEndTime.ToString());
            }

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            for (i = 0; i < machineName1.Length; i++)
            {
                comboBox1.Items.Add(machineName1[i]);
            }
            comboBox1.SelectedIndex = machineSelected1;

            for (i = 0; i < machineName2.Length; i++)
            {
                comboBox2.Items.Add(machineName2[i]);
            }
            comboBox2.SelectedIndex = machineSelected2;

            for (i = 0; i < machineName3.Length; i++)
            {
                comboBox5.Items.Add(machineName3[i]);
            }
            comboBox5.SelectedIndex = machineSelected3;

            for (i = 0; i < yesNo.Length; i++)
            {
                comboBox3.Items.Add(yesNo[i]);
            }
            comboBox3.SelectedIndex = endTimeRemoveSelected;

        }

        private void APSRules_Load(object sender, EventArgs e)
        {
            int i;
            //int num;
            string code;
            string commandText;

            commandText = "select originalSalesOrder, productCode, deliveryTime, customer from `" + gVariable.productBatchTableName + "` where id = '" + batchOrderIDInTable + "'";
            batchTableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
            if (batchTableArray == null)
            {
                Console.WriteLine("Failed to find batch order of ID" + batchOrderIDInTable);
                return;
            }

            code = batchTableArray[0, 1];

            textBox1.Text = batchTableArray[0, 0];  //sales order code
            textBox7.Text = code;  //product code 
            textBox2.Text = batchTableArray[0, 2];  //delivery time 
            textBox3.Text = batchTableArray[0, 3];  //customer name  

            commandText = "select id, BOM, multiIngredient from `" + gVariable.productTableName + "` where productCode = '" + code + "' or productCode = '" + code + "_1' or productCode = '" + code + "_2'";
            batchTableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
            if (batchTableArray == null)
            {
                Console.WriteLine("Failed to find batch order of productCode" + batchTableArray[0, 1]);
                return;
            }

            for (i = 0; i < batchTableArray.GetLength(0); i++)
            {
                comboBox4.Items.Add(batchTableArray[i, 1]);
            }
            comboBox4.SelectedIndex = 0;
        }

        //find substitutes
        private void button3_Click(object sender, EventArgs e)
        {
            substitutes substitutesImpl = new substitutes(batchOrderIDInTable, comboBox4.SelectedItem.ToString());
            substitutesImpl.Show();

        }

        //confirmed
        private void button1_Click(object sender, EventArgs e)
        {
            if (dateTimePicker2.CustomFormat == null)
            {
                APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedStartTime = toolClass.timeStringToTimeStamp(dateTimePicker2.Value.ToString("yyyy-MM-dd 00:00:00"));
            }
            else
            {
                APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedStartTime = -1;
            }

            if (dateTimePicker3.CustomFormat == null)
            {
                APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedEndTime = toolClass.timeStringToTimeStamp(dateTimePicker3.Value.ToString("yyyy-MM-dd 00:00:00"));
            }
            else
            {
                APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedEndTime = -1;
            }

            //APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].ruleAlreadyDefined = 1;
            APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].listviewIndex = batchOrderIndexInList;
            APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedMachineID1 = machineSelected1;
            APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedMachineID2 = machineSelected2;
            APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].assignedMachineID3 = machineSelected3;
            APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].ignoreEndTime = endTimeRemoveSelected;


            gVariable.numOfBatchDefinedAPSRule++;

            this.Close();
        }

        //cancelled
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //adjusted BOM
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedBOMIndex = comboBox4.SelectedIndex;
        }

        //cast machine selected
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected1 = comboBox1.SelectedIndex;
        }

        //print machine selected
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected2 = comboBox2.SelectedIndex;
        }

        //slit machine selected
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected3 = comboBox5.SelectedIndex;
        }


        //start date
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Format = DateTimePickerFormat.Long;
            dateTimePicker2.CustomFormat = null;
        }

        //end date
        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker3.Format = DateTimePickerFormat.Long;
            dateTimePicker3.CustomFormat = null;
        }
    }
}
