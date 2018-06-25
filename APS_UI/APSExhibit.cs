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
    public partial class APSExhibit : Form
    {
        public static APSExhibit APSExhibitClass;

        const int x = 3;
        const int y = 130;

        //different sales order need to be displayed in different color, we support SALES_ORDER_NUM_MAX of sales order at one exhibition UI 
        const int SALES_ORDER_NUM_MAX = 50;

        const int DISPATCH_NUM_MAX = 1200;

        static SolidBrush colorGrayBrush = new SolidBrush(Color.DarkGray);
        static SolidBrush colorCadetBrush = new SolidBrush(Color.CadetBlue);
        static SolidBrush colorYellowBrush = new SolidBrush(Color.Yellow);
        static SolidBrush colorBrownBrush = new SolidBrush(Color.Brown);
        static SolidBrush colorBlueBrush = new SolidBrush(Color.Blue);
        static SolidBrush colorPinkBrush = new SolidBrush(Color.Pink);
        static SolidBrush colorOrangeBrush = new SolidBrush(Color.Orange);
        static SolidBrush colorDarkBlueBrush = new SolidBrush(Color.DarkBlue);
        static SolidBrush colorDarkGreenBrush = new SolidBrush(Color.DarkGreen);
        static SolidBrush colorCyanBrush = new SolidBrush(Color.Cyan);
        static SolidBrush colorGoldBrush = new SolidBrush(Color.Gold);
        static SolidBrush colorLavenderBrush = new SolidBrush(Color.Lavender);
        static SolidBrush colorSeaGreenBrush = new SolidBrush(Color.SeaGreen);
        static SolidBrush colorSkyBlueBrush = new SolidBrush(Color.SkyBlue);
        static SolidBrush colorVioletBrush = new SolidBrush(Color.Violet);
        static SolidBrush colorSilverBrush = new SolidBrush(Color.Silver);
        static SolidBrush colorAzureBrush = new SolidBrush(Color.Azure);
        static SolidBrush colorMagentaBrush = new SolidBrush(Color.Magenta);
        static SolidBrush colorGreenBrush = new SolidBrush(Color.Lime);
        static SolidBrush colorRedBrush = new SolidBrush(Color.Red);
        static SolidBrush colorDarkCyanBrush = new SolidBrush(Color.DarkCyan);
        static SolidBrush colorCrimsonBrush = new SolidBrush(Color.Crimson);
        static SolidBrush colorCornsilkBrush = new SolidBrush(Color.Cornsilk);
        static SolidBrush colorBlueVioletBrush = new SolidBrush(Color.BlueViolet);
        static SolidBrush colorBlanchedAlmondBrush = new SolidBrush(Color.BlanchedAlmond);
        static SolidBrush colorBeigeBrush = new SolidBrush(Color.Beige);
        static SolidBrush colorBisqueBrush = new SolidBrush(Color.Bisque);
        static SolidBrush colorBurlyWoodBrush = new SolidBrush(Color.BurlyWood);
        static SolidBrush colorCadetBlueBrush = new SolidBrush(Color.CadetBlue);
        static SolidBrush colorChartreuseBrush = new SolidBrush(Color.Chartreuse);
        static SolidBrush colorCoralBrush = new SolidBrush(Color.Coral);
        static SolidBrush colorAntiqueWhiteBrush = new SolidBrush(Color.AntiqueWhite);
        static SolidBrush colorAquamarineBrush = new SolidBrush(Color.Aquamarine);
        static SolidBrush colorDarkGoldenrodBrush = new SolidBrush(Color.DarkGoldenrod);
        static SolidBrush colorDarkGrayBrush = new SolidBrush(Color.DarkGray);
        static SolidBrush colorDarkKhakiBrush = new SolidBrush(Color.DarkKhaki);
        static SolidBrush colorDarkMagentaBrush = new SolidBrush(Color.DarkMagenta);
        static SolidBrush colorDarkOliveGreenBrush = new SolidBrush(Color.DarkOliveGreen);
        static SolidBrush colorDarkOrangeBrush = new SolidBrush(Color.DarkOrange);
        static SolidBrush colorDarkOrchidBrush = new SolidBrush(Color.DarkOrchid);
        static SolidBrush colorDarkSalmonBrush = new SolidBrush(Color.DarkSalmon);
        static SolidBrush colorDarkSlateGrayBrush = new SolidBrush(Color.DarkSlateGray);
        static SolidBrush colorDarkTurquoiseBrush = new SolidBrush(Color.DarkTurquoise);
        static SolidBrush colorDarkVioletBrush = new SolidBrush(Color.DarkViolet);
        static SolidBrush colorDeepPinkBrush = new SolidBrush(Color.DeepPink);
        static SolidBrush colorDimGrayBrush = new SolidBrush(Color.DimGray);
        static SolidBrush colorDodgerBlueBrush = new SolidBrush(Color.DodgerBlue);
        static SolidBrush colorFirebrickBrush = new SolidBrush(Color.Firebrick);
        static SolidBrush colorFloralWhiteBrush = new SolidBrush(Color.FloralWhite);
        static SolidBrush colorFuchsiaBrush = new SolidBrush(Color.Fuchsia);
         

        SolidBrush[] brushArray = 
        { 
            colorGrayBrush,           colorCadetBrush,      colorYellowBrush,    colorBrownBrush,      colorBlueBrush,       colorPinkBrush,        colorOrangeBrush,   colorDarkBlueBrush, 
            colorDarkGreenBrush,      colorCyanBrush,       colorGoldBrush,      colorLavenderBrush,   colorSeaGreenBrush,   colorSkyBlueBrush,     colorVioletBrush,   colorSilverBrush, 
            colorDarkOliveGreenBrush, colorMagentaBrush,    colorGreenBrush,     colorRedBrush,        colorDarkCyanBrush,   colorCrimsonBrush,     colorCornsilkBrush, colorBlueVioletBrush, 
            colorBlanchedAlmondBrush, colorBeigeBrush,      colorBisqueBrush,    colorBurlyWoodBrush,  colorCadetBlueBrush,  colorChartreuseBrush,  colorCoralBrush,    colorAntiqueWhiteBrush, 
            colorDarkGoldenrodBrush,  colorAquamarineBrush, colorDarkGrayBrush,  colorFuchsiaBrush,    colorDarkKhakiBrush,  colorDarkMagentaBrush, colorAzureBrush,    colorDarkOrangeBrush, 
            colorDarkTurquoiseBrush,  colorDarkSalmonBrush, colorFirebrickBrush, colorDarkOrchidBrush, colorDarkVioletBrush, colorDeepPinkBrush,    colorDimGrayBrush,  colorDodgerBlueBrush,
            colorDarkSlateGrayBrush,  colorFloralWhiteBrush,
        };

        SolidBrush colorBlackBrush = new SolidBrush(Color.Black);

        dispatchBlock[] dispatchBlockImpl = new dispatchBlock[DISPATCH_NUM_MAX];
        string[] salesOrderArray = new string[SALES_ORDER_NUM_MAX];  //we suppose there won't be more than 100 sales orders that are APSed but not confirmed
        //int[] salesOrderColorArray = new string[SALES_ORDER_NUM_MAX];

        int salesOrderSelected;
        int dispatchBlockIndex;
        int totalSalesOrderNum;

        public class dispatchBlock
        {
            public int x1;
            public int y1;
            public int x2;
            public int y2;

            public string salesOrderCode;
            public string machineName;
            public string customerName;
            public string productCode;
            public string productName;
            public string productBatchNum;
            public string dispatchCode;
            public string plannedStart;
            public string plannedFinish;
            public string outputNum;
            public string workShift;
            public string salesOrderBatchCode;
        };

        public APSExhibit()
        {
            InitializeComponent();
            initVariables();
            resizeForScreen();
        }

        void initVariables()
        {
            int i, j, k;
            //int num;
            string commandText;
            string[,] tableArray;

            this.TopMost = false;

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            for (i = 0; i < DISPATCH_NUM_MAX; i++)
            {
                dispatchBlockImpl[i] = new dispatchBlock();
            }

            k = 0;
            salesOrderSelected = 0;

            commandText = "select * from `" + gVariable.globalDispatchTableName + "` where status = -3";  //-3 means MACHINE_STATUS_DISPATCH_GENERATED
            tableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
            if (tableArray != null)
            {
                for (i = 0; i < tableArray.GetLength(0); i++)
                {
                    for (j = 0; j < k; j++)
                    {
                        if (tableArray[i, mySQLClass.SALESORDER_CODE_IN_DISPATCHLIST_DATABASE] == salesOrderArray[j])
                            break;
                    }

                    if (j >= k)
                    {
                        salesOrderArray[k] = tableArray[i, mySQLClass.SALESORDER_CODE_IN_DISPATCHLIST_DATABASE];
                        k++;
                    }
                }
            }
            comboBox1.Items.Add("所有订单");
            for (i = 0; i < k; i++)
            {
                comboBox1.Items.Add(salesOrderArray[i]);
            }
            comboBox1.SelectedIndex = salesOrderSelected;
            totalSalesOrderNum = k;
        }

        private void resizeForScreen()
        {
            int i;
            int x, y, w, h;
            float fontSize;
            float screenRatioX, screenRatioY;
            GroupBox[] groupBoxArray = { groupBox1 };
            Label[] labelArray = { label1, label2, label3, label4, label15, label14, label6, label7, label8, label9, label10, label11, label12, label13 };
            TextBox[] textBoxArray = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox14 };
            ComboBox[] comboBoxArray = { comboBox1 };

            float[,] commonFontSize = { 
                                        { 7F,  8F,  9F,  10F, 11F,  12F}, 
                                        { 6F,  7F,  8F, 8.5F, 9F,  10F},  
                                        { 5.5F, 6F, 6.5F, 7F, 7.5F, 8F},  
                                     };

            int[] titleFontSize = { 20, 22, 23, 24, 25, 28 };
            Rectangle rect = new Rectangle();

            rect = Screen.GetWorkingArea(this);

            screenRatioX = gVariable.screenRatioX;
            screenRatioY = gVariable.screenRatioY;

            label5.Text = gVariable.enterpriseTitle + "排程系统结果展示";
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", titleFontSize[gVariable.resolutionLevel], System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            x = (rect.Width - label5.Size.Width) / 2;
            y = (int)(label5.Location.Y * screenRatioY);
            label5.Location = new System.Drawing.Point(x, y);

            fontSize = commonFontSize[gVariable.dpiValue, gVariable.resolutionLevel];

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
                x = (int)(labelArray[i].Location.X * screenRatioX);
                y = (int)(labelArray[i].Location.Y * screenRatioY);
                labelArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < textBoxArray.Length; i++)
            {
                w = (int)(textBoxArray[i].Size.Width * screenRatioX);
                h = (int)(textBoxArray[i].Size.Height * screenRatioY);
                textBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(textBoxArray[i].Location.X * screenRatioX);
                y = (int)(textBoxArray[i].Location.Y * screenRatioY);
                textBoxArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < comboBoxArray.Length; i++)
            {
                x = (int)(comboBoxArray[i].Location.X * screenRatioX);
                y = (int)(comboBoxArray[i].Location.Y * screenRatioY);
                comboBoxArray[i].Location = new System.Drawing.Point(x, y);
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            const int verticalForMonth1 = 8;
            const int verticalForMonth2 = 5 * 7 + 5;  //40 days

            int numOfMachines;

            int[] contentArray_Y = { 58, 60, 64, 64, 74, 120};
            int x = 3;
            int y;
            int i, j, k;
            int w;
            int titleH;
            int titleW;
            int deltaX;
            int deltaY;
            int weekDayIndex;
            int xx, yy, ww, hh;
            int dispatchNum;
            int offsetX, offsetY;
            //int salesOrderNum;
            int timeStampToday;
            int startTime;
            int durationTime;
            int[] gapX = { 1, 2, 2, 3, 4, 5 };
            int[] gapY = { 1, 2, 2, 2, 3, 4 };
            float[] fontSizeArray = { 7.5F, 8.0F, 8.0F, 8.4F, 9F, 9F };
            string commandText;
            string dateName;
            string databaseName;
            string[,] tableArray;
            Pen separatorLine = new Pen(Color.LightGray);
            Font myFont;
            Pen blackLine = new Pen(Color.Black);
            string[] weekDay = { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };
            //string[] machineName = { "1号流延机", "2号流延机", "3号流延机", "4号流延机", "5号流延机", "6号流延机", "吹膜机", "2号印刷机", "3号印刷机", "4号印刷机", "5号印刷机", "6号印刷机", "1号分切机", "3号分切机", "5号分切机", "6号分切机", "7号分切机", "8号分切机" };

            try
            {
                timeStampToday = 0;
                //salesOrderNum = 0;
                dispatchBlockIndex = 0;
                numOfMachines = gVariable.machineNameArrayAPS.Length;

                myFont = new Font("宋体", fontSizeArray[gVariable.resolutionLevel], FontStyle.Bold);

                y = contentArray_Y[gVariable.resolutionLevel];
                xx = 400;
                yy = 10;
                offsetX = gapX[gVariable.resolutionLevel];
                offsetY = gapY[gVariable.resolutionLevel];

                titleH = (int)(25 * gVariable.screenRatioX);
                titleW = (int)(70 * gVariable.screenRatioY);
                deltaX = (int)(30 * gVariable.screenRatioX);
                deltaY = (int)(20 * gVariable.screenRatioY);

                w = x + titleW + verticalForMonth2 * deltaX;
                weekDayIndex = (Convert.ToInt32(DateTime.Now.DayOfWeek) + 6) % 7;

                //first 2 horizontal lines: title lines
                e.Graphics.DrawLine(separatorLine, x, y, w, y);
                e.Graphics.DrawLine(separatorLine, x, y + titleH, w, y + titleH);

                e.Graphics.DrawString("日期", myFont, colorBlackBrush, x + offsetX * 9, y + offsetY * 4);
                e.Graphics.DrawString("设备\\星期", myFont, colorBlackBrush, x + offsetX + 1, y + titleH + offsetY * 4);

                //normal horizontal lines for 13 machines
                for (i = 0; i < numOfMachines + 1; i++)
                {
                    e.Graphics.DrawLine(separatorLine, x, y + titleH * 2 + i * deltaY, w, y + titleH * 2 + i * deltaY);

                    if (i == numOfMachines)
                        break;

                    e.Graphics.DrawString(gVariable.machineNameArrayAPS[i], myFont, colorBlackBrush, x + offsetX * 2, y + titleH * 2 + i * deltaY + offsetY * 2);
                }

                //machine name vertical title line
                e.Graphics.DrawLine(separatorLine, x, y, x, y + titleH + numOfMachines * deltaY);

                //week separators(5 weeks, so 6 vertical lines)
                for (i = 0; i < verticalForMonth1; i++)
                {
                    e.Graphics.DrawLine(separatorLine, x + i * 7 * deltaX + titleW, y, x + i * 7 * deltaX + titleW, y + titleH * 2 + numOfMachines * deltaY);

                    if (i == verticalForMonth1 - 1)
                        break;

                    dateName = DateTime.Now.AddDays(7 * i).Date.ToString("yyyy-MM-dd");
                    e.Graphics.DrawString(dateName, myFont, colorBlackBrush, x + titleW + i * 7 * deltaX + offsetX * 2, y + offsetY * 4);
                }

                //day separators(5 weeks, so 5 * 7 vertical lines)
                for (i = 0; i < verticalForMonth2; i++)
                {
                    e.Graphics.DrawLine(separatorLine, x + i * deltaX + titleW, y + titleH, x + i * deltaX + titleW, y + titleH * 2 + numOfMachines * deltaY);
                    e.Graphics.DrawString(weekDay[(i + weekDayIndex) % 7], myFont, colorBlackBrush, x + i * deltaX + titleW + offsetX, y + titleH + offsetY * 4);
                }

                dateName = DateTime.Now.Date.ToString("yyyy-MM-dd");
                timeStampToday = toolClass.timeStringToTimeStamp(dateName);

                commandText = "select * from `" + gVariable.machineWorkingPlanTableName + "` where planTime1 > \'" + timeStampToday + "\'";

                //display all dispatches for different machines
                for (i = 0; i < numOfMachines + 1; i++)
                {
                    databaseName = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');

                    //get machine plan table
                    tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);
                    if (tableArray == null)
                        continue;

                    dispatchNum = tableArray.GetLength(0);

                    //go through all dispatches one by one, get sales order info and draw time duration on screen
                    for(j = 0; j < dispatchNum; j++)
                    {
                        if(salesOrderSelected != 0)
                        {
                            if (tableArray[j, mySQLClass.PLAN_SALES_ORDER_INDEX] != salesOrderArray[salesOrderSelected - 1])
                                continue;
                        }

                        for(k = 0; k < totalSalesOrderNum; k++)
                        {
                            if (tableArray[j, mySQLClass.PLAN_SALES_ORDER_INDEX] == salesOrderArray[k])
                                break;
                        }

                        startTime = Convert.ToInt32(tableArray[j, mySQLClass.PLAN_START_TIME_STAMP_INDEX]);
                        durationTime = Convert.ToInt32(tableArray[j, mySQLClass.PLAN_KEEP_DURATION_INDEX]);

                        if (startTime < timeStampToday)
                        {
                            if (startTime + durationTime * 3600 > timeStampToday)
                            {
                                xx = x + titleW;
                                ww = (durationTime * 3600 + 1800 + timeStampToday - startTime) * deltaX / (3600 * 24);
                            }
                            else
                                continue;
                        }
                        else
                        {
                            xx = x + (startTime + 1800 - timeStampToday) * deltaX / (3600 * 24) + titleW;
                            ww = (durationTime * 3600 + 1800) * deltaX / (3600 * 24);
                        }
                        yy = y + titleH * 2 + i * deltaY + offsetY;
                        hh = deltaY - offsetY * 2;

                        e.Graphics.FillRectangle(brushArray[k], xx, yy, ww, hh);
                        e.Graphics.DrawRectangle(blackLine, xx, yy, ww, hh);

                        k = mySQLClass.PLAN_SALES_ORDER_INDEX;

                        dispatchBlockImpl[dispatchBlockIndex].x1 = xx;
                        dispatchBlockImpl[dispatchBlockIndex].y1 = yy;
                        dispatchBlockImpl[dispatchBlockIndex].x2 = xx + ww;
                        dispatchBlockImpl[dispatchBlockIndex].y2 = yy + hh;
                        dispatchBlockImpl[dispatchBlockIndex].salesOrderCode = tableArray[j, k++];
                        dispatchBlockImpl[dispatchBlockIndex].dispatchCode = tableArray[j, k++];
                        dispatchBlockImpl[dispatchBlockIndex].plannedStart = tableArray[j, k];
                        k += 3;
                        dispatchBlockImpl[dispatchBlockIndex].plannedFinish = tableArray[j, k++];
                        dispatchBlockImpl[dispatchBlockIndex].machineName = gVariable.machineNameArrayAPS[i];
                        k++;
                        dispatchBlockImpl[dispatchBlockIndex].productCode = tableArray[j, k++];
                        dispatchBlockImpl[dispatchBlockIndex].productName = tableArray[j, k++];
                        dispatchBlockImpl[dispatchBlockIndex].outputNum = tableArray[j, k++];
                        dispatchBlockImpl[dispatchBlockIndex].customerName = tableArray[j, k++];
                        dispatchBlockImpl[dispatchBlockIndex].workShift = tableArray[j, k++];
                        dispatchBlockImpl[dispatchBlockIndex].productBatchNum = dispatchBlockImpl[dispatchBlockIndex].dispatchCode.Remove(7);
                        dispatchBlockImpl[dispatchBlockIndex].salesOrderBatchCode = tableArray[j, k++];
                        dispatchBlockIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("APS Exhibit failed " + ex);
            }

            base.OnPaint(e);
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            int i;

            for (i = 0; i < dispatchBlockIndex; i++)
            {
                if (e.Location.X >= dispatchBlockImpl[i].x1 && e.Location.X <= dispatchBlockImpl[i].x2 && e.Location.Y >= dispatchBlockImpl[i].y1 && e.Location.Y <= dispatchBlockImpl[i].y2)
                {
                    textBox1.Text = dispatchBlockImpl[i].salesOrderCode;
                    textBox10.Text = dispatchBlockImpl[i].dispatchCode;
                    textBox9.Text = dispatchBlockImpl[i].plannedStart;
                    textBox2.Text = dispatchBlockImpl[i].machineName;
                    textBox3.Text = dispatchBlockImpl[i].customerName;
                    textBox7.Text = dispatchBlockImpl[i].productCode;
                    textBox6.Text = dispatchBlockImpl[i].productName;
                    textBox5.Text = dispatchBlockImpl[i].productBatchNum;
                    textBox8.Text = dispatchBlockImpl[i].plannedFinish;
                    textBox14.Text = dispatchBlockImpl[i].outputNum;
                    textBox4.Text = dispatchBlockImpl[i].workShift;
                    textBox12.Text = dispatchBlockImpl[i].salesOrderBatchCode;
                    break;
                }
            }

            if(i >= dispatchBlockIndex)
            {
                textBox1.Text = null;
                textBox2.Text = null;
                textBox3.Text = null;
                textBox4.Text = null;
                textBox5.Text = null;
                textBox6.Text = null;
                textBox7.Text = null;
                textBox8.Text = null;
                textBox9.Text = null;
                textBox10.Text = null;
                textBox12.Text = null;
                textBox14.Text = null;
            }

            base.OnMouseMove(e);
        }

        private void APSExhibit_Load(object sender, EventArgs e)
        {
        }

        private void APSExhibit_FormClosing(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            salesOrderSelected = comboBox1.SelectedIndex;
            Invalidate();
        }
    }
}
