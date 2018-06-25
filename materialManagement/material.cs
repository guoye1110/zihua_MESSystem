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

namespace MESSystem.material
{
    public partial class materialManagement : Form
    {
        public static materialManagement materialManagementClass = null; //it is used to reference this windows

        //when closing room class, closeReason = 0 means go back to firstScreen class, 1 means go to multiCurve class
        int closeReason;

        public materialManagement()
        {
            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        private void material_Load(object sender, EventArgs e)
        {
            closeReason = 0;
            loadScreen();

            switch (gVariable.CompanyIndex)
            {
                case gVariable.ZIHUA_ENTERPRIZE:
                    this.label1.Text = gVariable.enterpriseTitle + "物料管理系统";
                    break;
            }
        }

        private void loadScreen()
        {
            int i;
//            int index;
//            string databaseName;
//            string timeS1, timeS2;
            ListViewItem OptionItem;

            listView1.Clear();

            this.listView1.BeginUpdate();

            listView1.GridLines = true;
            listView1.Dock = DockStyle.Fill;
            listView1.Columns.Add(" ", 1, HorizontalAlignment.Left);
            listView1.Columns.Add("序号", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("物料编码", 130, HorizontalAlignment.Left);
            listView1.Columns.Add("物料名称", 130, HorizontalAlignment.Left);
            listView1.Columns.Add("物料存放点", 120, HorizontalAlignment.Left);
            listView1.Columns.Add("物料类型", 120, HorizontalAlignment.Left);
            listView1.Columns.Add("规格型号", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("库存数量", 60, HorizontalAlignment.Left);
            listView1.Columns.Add("计量单位", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("供应商", 200, HorizontalAlignment.Left);
            listView1.Columns.Add("进货总量", 120, HorizontalAlignment.Left);
            listView1.Columns.Add("进货时间", 120, HorizontalAlignment.Left);

            for (i = 0; i < 10; i++)
            {
                OptionItem = new ListViewItem();

                OptionItem.SubItems.Add((i + 1).ToString());
                OptionItem.SubItems.Add("P1533645-1");
                OptionItem.SubItems.Add("包膜");
                OptionItem.SubItems.Add("线边库");
                OptionItem.SubItems.Add("主料");
                OptionItem.SubItems.Add("透气型");
                OptionItem.SubItems.Add("34");
                OptionItem.SubItems.Add("包");
                OptionItem.SubItems.Add("PPA 材料有限公司");
                OptionItem.SubItems.Add("166");
                OptionItem.SubItems.Add("2016-12-30");

                listView1.Items.Add(OptionItem);
            }

            this.listView1.EndUpdate();
        }

        private void material_FormClosing(object sender, EventArgs e)
        {
            if (closeReason == 0)
            {
                try
                {
                    firstScreen.firstScreenClass.Show();
                }
                catch (Exception ex)
                {
                    Console.Write("close material class" + ex);
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
                    
        }
    }
}
