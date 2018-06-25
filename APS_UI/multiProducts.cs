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
    public partial class multiProducts : Form
    {
        int productSelected1 = 0;
        int productSelected2 = 0;
        int productSelected3 = 0;

        string productCode;
        string productBatchCode;

        string[,] productTableArray;

        public multiProducts(string productCode_, string productBatchCode_)
        {
            productCode = productCode_;
            productBatchCode = productBatchCode_;

            InitializeComponent();
            initVariables();
        }

        void initVariables()
        {
            string commandText;

            commandText = "select productCode, productName, customer, productWidth from `" + gVariable.productTableName + "`"; // where productCode = '" + productImpl.productCode + "'";
            productTableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
            if (productTableArray == null)
                return;
        }

        private void multiProducts_Load(object sender, EventArgs e)
        {
            int i;
            int num;

            try
            {
                for(i = 0; i < productTableArray.GetLength(0); i++)
                {
                    if(productTableArray[i, 0] == productCode)
                        break;
                }

                if(i >= productTableArray.GetLength(0))
                {
                    Console.WriteLine("Cannot find this product of " + productCode + " in product table.");
                    this.Close();
                }

                textBox1.Text = productTableArray[i, 0];
                textBox2.Text = productTableArray[i, 1];
                textBox3.Text = productTableArray[i, 2];
                textBox4.Text = productTableArray[i, 3];

                num = productTableArray.GetLength(0);

                comboBox1.Items.Add("不设定套作产品");
                for (i = 0; i < num; i++)
                {
                    comboBox1.Items.Add(productTableArray[i, 0]);
                }
                comboBox1.SelectedIndex = productSelected1;

                comboBox2.Items.Add("不设定套作产品");
                for (i = 0; i < num; i++)
                {
                    comboBox2.Items.Add(productTableArray[i, 0]);
                }
                comboBox2.SelectedIndex = productSelected2;

                comboBox3.Items.Add("不设定套作产品");
                for (i = 0; i < num; i++)
                {
                    comboBox3.Items.Add(productTableArray[i, 0]);
                }
                comboBox3.SelectedIndex = productSelected3;
            }
            catch (Exception ex)
            {
                Console.Write("multiProducts_Load failed! " + ex);
            }
        }

        //OK
        private void button1_Click(object sender, EventArgs e)
        {
            int index;
            string updateStr;
            string productCode2;
            string productCode3;
            string productCode4;
            string customer2;
            string customer3;
            string customer4;

            if (comboBox1.SelectedIndex == 0)
            {
                productCode2 = null;
                customer2 = null;
            }
            else
            {
                index = comboBox1.SelectedIndex;
                productCode2 = productTableArray[index, 0];
                customer2 = productTableArray[index, 2];
            }

            if (comboBox2.SelectedIndex == 0)
            {
                productCode3 = null;
                customer3 = null;
            }
            else
            {
                index = comboBox2.SelectedIndex;
                productCode3 = productTableArray[index, 0];
                customer3 = productTableArray[index, 2];
            }

            if (comboBox3.SelectedIndex == 0)
            {
                productCode4 = null;
                customer4 = null;
            }
            else
            {
                index = comboBox3.SelectedIndex;
                productCode4 = productTableArray[index, 0];
                customer4 = productTableArray[index, 2];
            }

            updateStr = "update `" + gVariable.productBatchTableName + "` set productCode2= '" + productCode2 + "', customer2 = '" + customer2 + "', productCode3 = '" +
                        productCode3 + "', customer3 = '" + customer3 + "', productCode4 = '" + productCode4 + "', customer4 = '" + customer4 + "' where productBatchCode = '" + productBatchCode + "'";
            mySQLClass.updateTableItems(gVariable.globalDatabaseName, updateStr);

            this.Close();
        }

        //exit
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index;
            index = comboBox1.SelectedIndex;

            if (index == 0)
            {
                textBox7.Text = "";
                textBox6.Text = "";
                textBox5.Text = "";
            }
            else
            {
                index--;
                textBox7.Text = productTableArray[index, 1];
                textBox6.Text = productTableArray[index, 2];
                textBox5.Text = productTableArray[index, 3];
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index;
            index = comboBox2.SelectedIndex;

            if (index == 0)
            {
                textBox10.Text = "";
                textBox9.Text = "";
                textBox8.Text = "";
            }
            else
            {
                index--;
                textBox10.Text = productTableArray[index, 1];
                textBox9.Text = productTableArray[index, 2];
                textBox8.Text = productTableArray[index, 3];
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index;
            index = comboBox3.SelectedIndex;

            if (index == 0)
            {
                textBox13.Text = "";
                textBox12.Text = "";
                textBox11.Text = "";
            }
            else
            {
                index--;
                textBox13.Text = productTableArray[index, 1];
                textBox12.Text = productTableArray[index, 2];
                textBox11.Text = productTableArray[index, 3];
            }
        }
    }
}
