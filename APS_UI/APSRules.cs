using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MESSystem.APS_UI
{
    public partial class APSRules : Form
    {
        int machineSelected1;
        int machineSelected2;
        int machineSelected3;
        int endTimeRemoveSelected;

        public APSRules()
        {
            InitializeComponent();
            initVariables();
        }

        void initVariables()
        {
            int i;

            string[] yesNo = { "是", "否" };
            string[] machineName1 = { "不指定", "1号流延机", "2号流延机", "3号流延机", "4号流延机", "5号流延机" };
            string[] machineName2 = { "不指定", "2号印刷机", "3号印刷机", "4号印刷机" };
            string[] machineName3 = { "不指定", "1号分切机", "3号分切机", "5号分切机", "6号分切机", "7号分切机" };

            machineSelected1 = 0;
            machineSelected2 = 0;
            machineSelected3 = 0;
            endTimeRemoveSelected = 0;

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
    }
}
