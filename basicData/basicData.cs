using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using MESSystem.common;
using MESSystem.mainUI;
using MESSystem.commonControl;

namespace MESSystem.mainUI
{
    public partial class basicData : Form
    {
        const int LABEL_NUM = 1;
        const int ButtonNum = 7;

        public static basicData basicDataClass = null; //it is used to reference this windows

        Button[] buttonArray = new Button[ButtonNum];
        Label[] labelArray = new Label[LABEL_NUM];

        public basicData()
        {
            InitializeComponent();
            InitializeForm();

            //use double video buffer to output display contents to screen, one for processing display data, and one for output, to avoid flickering
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            resizeForScreen();
        }

        private void InitializeForm()
        {
            switch (gVariable.CompanyIndex)
            {
                case gVariable.ZIHUA_ENTERPRIZE:
                    gVariable.programTitle = gVariable.enterpriseTitle + "智能生产信息管理系统";
                    label1.Text = gVariable.enterpriseTitle + "智能生产信息管理系统";
                    break;
                default:
                    break;
            }
            BackgroundImage = Image.FromFile(gVariable.backgroundArray[gVariable.CompanyIndex]);
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        private void resizeForScreen()
        {
            int i;
            int x, y, h, w;
            int ceilingValue = 10;  //this is ceiling for windows screen, its title area
            int start_x, start_y;
            int width, height;
            int gap;
            Button[] bArray = { button1, button2, button3, button4, button5, button6, button7};
            Label[] LArray = { label1};

            start_x = 3;
            start_y = 4;

            width = 120;
            height = 45;

            gap = 6;

            for (i = 0; i < buttonArray.Length; i++)
            {
                buttonArray[i] = bArray[i];
            }

            for (i = 0; i < LABEL_NUM; i++)
            {
                labelArray[i] = LArray[i];
            }

            Rectangle rect = new Rectangle();

            rect = Screen.GetWorkingArea(this);

            for (i = 0; i < 4; i++)
            {
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
                buttonArray[i].Text = gVariable.subMenuText[gVariable.MENU_BASIC_DATA, i];
                buttonArray[i].UseVisualStyleBackColor = true;
            }

            for( ; i < ButtonNum; i++)
                buttonArray[i].Visible = false;
 
            for (i = 0; i < LABEL_NUM; i++)
            {
                if (gVariable.resolutionLevel >= gVariable.resolution_1920)
                    x = labelArray[i].Location.X * rect.Width / gVariable.EFFECTIVE_SCREEN_X + 250;
                if (gVariable.resolutionLevel >= gVariable.resolution_1600)
                    x = labelArray[i].Location.X * rect.Width / gVariable.EFFECTIVE_SCREEN_X + 100;
                else
                    x = labelArray[i].Location.X * rect.Width / gVariable.EFFECTIVE_SCREEN_X;

                y = (labelArray[i].Location.Y - ceilingValue) * rect.Height / gVariable.EFFECTIVE_SCREEN_Y + ceilingValue;
                h = labelArray[i].Size.Height * rect.Height / gVariable.EFFECTIVE_SCREEN_Y;
                w = labelArray[i].Size.Width * rect.Width / gVariable.EFFECTIVE_SCREEN_X;

                labelArray[i].Location = new System.Drawing.Point(x, y);
                labelArray[i].Size = new System.Drawing.Size(w, h);
            }
        }


        private void basicData_Load(object sender, EventArgs e)
        {
            this.label1.BackColor = Color.Transparent;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Text = gVariable.programTitle;
        }


        private void basicData_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                firstScreen.firstScreenClass.Show();
            }
            catch (Exception ex)
            {
                Console.Write("close apsui class" + ex);
            }

        }

        //product list
        private void button1_Click(object sender, EventArgs e)
        {
            basicDataClass = this;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.productTableName, gVariable.productFileName, "产品信息");
            commonListview.commonListviewClass.Show();
        }

        //material list
        private void button2_Click(object sender, EventArgs e)
        {
            basicDataClass = this;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.materialTableName, gVariable.materialFileName, "物料信息");
            commonListview.commonListviewClass.Show();
        }

        //BOM list
        private void button3_Click(object sender, EventArgs e)
        {
            basicDataClass = this;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.bomTableName, gVariable.bomFileName, "BOM信息");
            commonListview.commonListviewClass.Show();
        }

        //machine list
        private void button4_Click(object sender, EventArgs e)
        {
            basicDataClass = this;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.machineTableName, gVariable.machineFileName, "设备信息");
            commonListview.commonListviewClass.Show();
        }

        //customer list
        private void button5_Click(object sender, EventArgs e)
        {
        }

        //
        private void button6_Click(object sender, EventArgs e)
        {
        }

        //
        private void button7_Click(object sender, EventArgs e)
        {
        }
    }
}
