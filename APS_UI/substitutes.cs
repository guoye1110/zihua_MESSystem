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
    public partial class substitutes : Form
    {
        //int MAX_NUM_SELECTED_SALES_ORDER_APS;

        int selectedBatchOrderID;
        int numOfMaterialType;  //number of material types
        int[] materialSelected = new int[gVariable.maxMaterialTypeNum];
        string strBOM;
        ComboBox[] comboBoxArray;

        string[,] batchTableArray;
        public substitutes(int batchOrderID, string strBOM_)
        {
            selectedBatchOrderID = batchOrderID;
            strBOM = strBOM_;

            InitializeComponent();
            initVariables();
        }

        private void initVariables()
        {
            int i;

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            comboBoxArray = new ComboBox[5];

            comboBoxArray[0] = comboBox1;
            comboBoxArray[1] = comboBox2; 
            comboBoxArray[2] = comboBox3;
            comboBoxArray[3] = comboBox4;
            comboBoxArray[4] = comboBox5;
        }

        private void substitutes_Load(object sender, EventArgs e)
        {
            int i, j, k;
            int found;
            string commandText;
            string[,] bomTableArray;
            string[,] materialTableArray;

            try
            {
                commandText = "select originalSalesOrder, productCode, deliveryTime, customer from `" + gVariable.productBatchTableName + "` where id = '" + selectedBatchOrderID + "'";
                batchTableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (batchTableArray == null)
                {
                    Console.WriteLine("Failed to find batch order of ID" + selectedBatchOrderID);
                    return;
                }

                textBox1.Text = batchTableArray[0, 0];  //sales order code
                textBox7.Text = batchTableArray[0, 1];  //product code 
                textBox4.Text = batchTableArray[0, 2];  //delivery time 
                textBox3.Text = batchTableArray[0, 3];  //customer name  
                textBox2.Text = strBOM;

                commandText = "select * from `" + gVariable.bomTableName + "` where BOMCode = '" + strBOM + "'";
                bomTableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (bomTableArray == null)
                {
                    Console.WriteLine("Failed to find batch order of productCode" + batchTableArray[0, 1]);
                    return;
                }

                numOfMaterialType = Convert.ToInt32(bomTableArray[0, 2]);
                commandText = "select * from `" + gVariable.substitutesTableName + "`";
                materialTableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (materialTableArray == null)
                {
                    Console.WriteLine("Failed to find batch order of productCode" + batchTableArray[0, 1]);
                    return;
                }

                for (i = 0; i < comboBoxArray.Length; i++)
                {
                    if (i < numOfMaterialType)
                    {
                        for (j = 0; j < materialTableArray.GetLength(0); j++)
                        {
                            found = 0;
                            if (bomTableArray[0, 3 + i * 3] == materialTableArray[j, 1])
                            {
                                for (k = 1; k <= 4; k++)
                                {
                                    if (materialTableArray[j, k] != null && materialTableArray[j, k] != "")
                                    {
                                        comboBoxArray[i].Items.Add(materialTableArray[j, k]);
                                        found = 1;
                                    }
                                }
                            }
                            if(found == 0)
                                comboBoxArray[i].Items.Add(materialTableArray[j, 1]);
                        }
                        comboBoxArray[i].SelectedIndex = materialSelected[i];
                    }
                    else
                    {
                        comboBoxArray[i].Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("substitutes_Load() failed!" + ex);
            }
        }


        //confirm
        private void button1_Click(object sender, EventArgs e)
        {
            int i;

            /*for (i = 0; i < numOfMaterialType; i++)
            {
                APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].materialSelected[i] = materialSelected[i];
                APSUI.APSRulesArray[gVariable.indexOfBatchDefinedAPSRule].materialCode[i] = comboBoxArray[i].SelectedItem.ToString();
            }*/

            this.Close();
        }

        //exit
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            materialSelected[0] = comboBoxArray[0].SelectedIndex;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            materialSelected[1] = comboBoxArray[1].SelectedIndex;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            materialSelected[2] = comboBoxArray[2].SelectedIndex;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            materialSelected[3] = comboBoxArray[3].SelectedIndex;
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            materialSelected[4] = comboBoxArray[4].SelectedIndex;
        }
    }
}
