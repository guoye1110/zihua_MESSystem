using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using MESSystem.common;
using MESSystem.communication;

namespace MESSystem.mainUI
{
    public partial class machineProgress : Form
    {
        const int PIC_WIDTH = 32;
        const int PIC_HEIGHT = 32;

        const int deviceStatusOff = 2;
        const int deviceStatusIdle = 3;
        const int deviceStatusWorking = 4;
        const int deviceStatusRepairing = 0;
        const int deviceStatusNoMaterial = 1;

        const int dummyValue = 100;
        const int numberOfMachineZihua = 18;

        const int UpdatePeriod = 5 * 2;  //this screen update itself every 5 second 

        System.Windows.Forms.Timer aTimer;

        static string[] machineCodeZihua = new string[numberOfMachineZihua];

        string[] buttonName = new string[machineCodeZihua.Length];

        public static machineProgress machineProgressClass = null; //it is used to reference this windows

        public gVariable.alarmTableStruct alarmTableStructImpl;

        int[] statusBar_y = { 56, 56, 56, 71, 71, 76 };   //start pos to display color bar
        int[] statusBarHeight = { 23, 23, 27, 27, 27, 27 };
        int[] gapBetweenTextStatusBar_x = { 10, 10, 2, 5, 6, 8 };   //start pos to display text in color bar
        int[] gapBetweenTextStatusBar_y = { 4, 4, 2, 3, 3, 3 };   //start pos to display text in color bar

        int[] start_x = { 15, 18, 24, 26, 28, 30 };  //start pos to display color bar
        int[] start_y = { 95, 95, 140, 140, 140, 140 };   //start pos to display color bar
        int[] gap_x = { 2, 2, 2, 2, 2, 1 };   //how many minutes a point in color bar stands for
        int[] gap_y =   {19,  19,   19,  29,  29,  35};  //gap between 2 color bars
        int[] width =   {720, 720, 720, 720, 720, 1440};  //length of the whole color bar
        int[] height =  {14,  14,  15,  18,  18,  20};  //height of a color bar
        int[] labelLocationX = { 200, 290, 350, 400, 530, 528 };  //location of the screen title 
        int[] labelLocationY = { 12, 12, 12, 18, 18, 22 }; //location of the screen title 

        int[] titleGapX = {21, 20, 18, 16, 14, 13};  //gap between the end of color bar and the start of the text

        int resIndex;
        int mouseX, mouseY;

        int timerIndex;

        int dpiValue;
        float dpiScreenFactorX, dpiScreenFactorY;

        ComboBox[] comboBoxArray = new ComboBox[50];

        SolidBrush colorGrayBrush = new SolidBrush(Color.DarkGray);  //machine turned off   
        SolidBrush colorYellowBrush = new SolidBrush(Color.CadetBlue);  //machine standby
        SolidBrush colorGreenBrush = new SolidBrush(Color.Lime);  //machine working
        SolidBrush colorRedBrush = new SolidBrush(Color.Red);  //device alarm
        SolidBrush colorChocolateBrush = new SolidBrush(Color.Yellow); //material alarm
        SolidBrush colorBrownBrush = new SolidBrush(Color.Brown);  //data alarm

        Font font0;  //like: "设备停机", "设备待机", "设备运行", "设备报警", "缺料报警", "数据报警"
        Font font1;
        Font font2;  //time value and title like: 0:00  2:00  4:00 and "当前工单: "

        public machineProgress()
        {
            InitializeComponent();

            //use double video buffer to output display contents to screen, one for processing display data, and one for output, to avoid flickering
//            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            initWindowData();
        }


        private void initWindowData()
        {
            int i; //, j;
            int index;

            try
            {
                resIndex = gVariable.resolutionLevel;

                timerIndex = 0;

                mouseX = -1;
                mouseY = -1;

                for (i = 0; i < numberOfMachineZihua; i++)
                {
                    machineCodeZihua[i] = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');
                }

                switch (resIndex)
                {
                    case gVariable.resolution_1024:
                        font0 = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                        font1 = new Font("Microsoft Sans Serif", 9, FontStyle.Bold);
                        font2 = new Font("Microsoft Sans Serif", 9);
                        break;
                    case gVariable.resolution_1280:
                        font0 = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                        font1 = new Font("Microsoft Sans Serif", 9, FontStyle.Bold);
                        font2 = new Font("Microsoft Sans Serif", 9);
                        break;
                    case gVariable.resolution_1366:
                        font0 = new Font("Microsoft Sans Serif", 11, FontStyle.Bold);
                        font1 = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                        font2 = new Font("Microsoft Sans Serif", 9);
                        break;
                    case gVariable.resolution_1440:
                        font0 = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                        font1 = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                        font2 = new Font("Microsoft Sans Serif", 9);
                        break;
                    case gVariable.resolution_1600:
                        font0 = new Font("Microsoft Sans Serif", 13, FontStyle.Bold);
                        font1 = new Font("Microsoft Sans Serif", 11, FontStyle.Bold);
                        font2 = new Font("Microsoft Sans Serif", 10);
                        break;
                    case gVariable.resolution_1920:
                        font0 = new Font("Microsoft Sans Serif", 14, FontStyle.Bold);
                        font1 = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                        font2 = new Font("Microsoft Sans Serif", 10);
                        break;
                }

                for (i = 0; i < machineCodeZihua.Length; i++)
                {
                    index = Convert.ToInt16(machineCodeZihua[i].Remove(0, 1));
                    if (index == 0)
                        buttonName[i] = " ";
                    else
                        buttonName[i] = "设备" + index + "\r\n" + gVariable.machineNameArray[index - 1];
                }

                label1.BackColor = Color.Transparent;
                label1.Location = new System.Drawing.Point(labelLocationX[resIndex], labelLocationY[resIndex]);

                switch (gVariable.CompanyIndex)
                {
                    case gVariable.ZIHUA_ENTERPRIZE:
                        if (gVariable.mainFunctionIndex == gVariable.MAIN_FUNCTION_PRODUCTION)
                            label1.Text = gVariable.enterpriseTitle + "当日生产状况一览";
                        else
                            label1.Text = gVariable.enterpriseTitle + "当日设备状况一览";
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("dispatchUI init window failed" + ex);
            }
        }

        private void getScalingFactorForScreen()
        {
            dpiValue = toolClass.getScalingFactorForScreen();

            if (dpiValue == gVariable.SMALLER_DPI)
            {
                dpiScreenFactorX = 1.0f;
                dpiScreenFactorY = 1.0f;
            }
            else if (dpiValue == gVariable.MEDIUM_DPI)
            {
                dpiScreenFactorX = 1.333f;
                dpiScreenFactorY = 1.230f;
            }
            else if (dpiValue == gVariable.LARGER_DPI)
            {
                dpiScreenFactorX = 1.600f;
                dpiScreenFactorY = 1.470f;
            }
            else
            {
                dpiScreenFactorX = 1.0f;
                dpiScreenFactorY = 1.0f;
            }

            Console.WriteLine("screen scale is " + dpiScreenFactorX + dpiScreenFactorY);
        }


        private void room_Load(object sender, EventArgs e)
        {
            gVariable.currentCurveDatabaseName = null;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Text = gVariable.programTitle + "生产状况一览";

            this.KeyPreview = true;  //to make sure this window will accept a key press event

            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 2 seconds
            aTimer.Interval = 500;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_checkAlarm);
        }

        private void timer_checkAlarm(Object source, EventArgs e)
        {
            if (timerIndex == UpdatePeriod)
            {
                Invalidate();
                timerIndex = 0;
            }
            else
            {
                timerIndex++;
                emulateHoverFunc();
            }
        }


        private void room_Selected(int i)
        {
            gVariable.currentCurveDatabaseName = machineCodeZihua[i - 1];
            //this button is not defined
            if (gVariable.currentCurveDatabaseName.Remove(0, 1) != "000")
            {
                gVariable.boardIndexSelected = Convert.ToInt32(gVariable.currentCurveDatabaseName.Remove(0, 1)) - 1;

                //if this dispatch already started, we don't neeed to get dummy data
                if (gVariable.machineCurrentStatus[gVariable.boardIndexSelected] <= gVariable.MACHINE_STATUS_DISPATCH_DUMMY)
                    toolClass.getDummyData(gVariable.boardIndexSelected);

                preparationForCurvedisplay(gVariable.currentCurveDatabaseName);
            }
        }

        private void room_FormClosing(object sender, EventArgs e)
        {
            if (aTimer != null)
                aTimer.Enabled = false;

            firstScreen.firstScreenClass.Show();
        }

        private void preparationForCurvedisplay(string databaseName)
        {
            int ret;

            gVariable.currentDatabaseName = databaseName;

            if (databaseName.Remove(0, 1) == "000")  
            {
                MessageBox.Show("抱歉，选中的机床设备没有连入数据采集系统！", "信息提示", MessageBoxButtons.OK);
                return; //still not, go back to workshop screen, since dispatch sheet not started yet
            }

            ret = mySQLClass.getRecordNumInTable(databaseName, gVariable.dispatchListTableName);
            if (ret == 0)  //database not generated yet
            {
                //there is no dispatch in for this machine, even dummy dispatch is not available
                MessageBox.Show("抱歉，选中的机床设备尚未握手成功，请确认相连的数据采集板已上电，并且操作员在触屏上已启动工单！", "信息提示", MessageBoxButtons.OK);
                return; //still not, go back to workshop screen, since dispatch sheet not started yet
            }

            gVariable.contemporarydispatchUI = 1;

            toolClass.getCurveInfoIngVariable(databaseName, gVariable.boardIndexSelected, gVariable.CURRENT_READING);

            //go to curve display screen
            dispatchUI.dispatchUIClass = new dispatchUI(gVariable.PRODUCT_TIME_DIVISION_STATUS);
            dispatchUI.dispatchUIClass.Show();

            //hide workshop screen for the time being
            this.Hide();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int i, j, k, n;
            int msgWidth;
            int value;
            int valueNow;
            int len; //, num;
            int start1, start2;
            int index;
//            int timeV0;
            int timeV1, timeV2;
            int[] colorArray = new int[1500];
            int [] timeArray = new int[1000 * 3]; //
//            float thresh1, thresh2;
//            string timeS1, timeS2;
            string[] timeStr = { "0:00", "2:00", "4:00", "6:00", "8:00", "10:00", "12:00", "14:00", "16:00", "18:00", "20:00", "22:00", "24:00" };
            string str;
            string databaseName = null;
//            float data;
//            gVariable.dispatchSheetStruct[] dispatchListArray;
            SolidBrush[] brushArray = { colorGrayBrush, colorYellowBrush, colorGreenBrush, colorRedBrush, colorChocolateBrush, colorBrownBrush };
            string[] msgArray1 = { "设备停机", "设备待机", "设备运行", "设备报警", "缺料报警", "数据报警" };
            string[] msgArray2 = { "设备停机", "工单待机", "工单启动", "设备报警", "缺料报警", "数据报警" };

            try
            {
                msgWidth = 120;

                //Console.WriteLine("OnpPaint staus4 " + DateTime.Now.ToString());

                for (i = 0; i < brushArray.Length; i++)
                {
                    e.Graphics.FillRectangle(brushArray[i], start_x[resIndex] + i * msgWidth, statusBar_y[resIndex] - gapBetweenTextStatusBar_y[resIndex], msgWidth, statusBarHeight[resIndex]);
                    if (gVariable.mainFunctionIndex == gVariable.MAIN_FUNCTION_PRODUCTION)
                        e.Graphics.DrawString(msgArray2[i], font0, Brushes.Black, start_x[resIndex] + i * msgWidth + gapBetweenTextStatusBar_x[resIndex], statusBar_y[resIndex]);
                    else
                        e.Graphics.DrawString(msgArray1[i], font0, Brushes.Black, start_x[resIndex] + i * msgWidth + gapBetweenTextStatusBar_x[resIndex], statusBar_y[resIndex]);
                }

                i = 0;  //i and n are the same meaning, but n may include z000 that is not really exists, so we need to use i as index, n only as device/database name index
                for (n = 0; n < machineCodeZihua.Length; n++)
                {
                    if (machineCodeZihua[n].Remove(0, 1) == "000")
                        continue;

                    index = Convert.ToInt16(machineCodeZihua[n].Remove(0, 1)) - 1;

                    databaseName = machineCodeZihua[n];

                    timeV1 = toolClass.ConvertDateTimeInt(DateTime.Now.Date);
                    timeV2 = timeV1 + 3600 * 24;

                    //get machine status for one day by minutes
                    len = mySQLClass.readMachineStatusForOneDay(databaseName, gVariable.machineStatusRecordTableName, timeV1, timeV2);

                    start1 = 0;
                    start2 = 0;

                    e.Graphics.FillRectangle(colorGrayBrush, start_x[resIndex] + start1, start_y[resIndex] + i * (gap_y[resIndex] + height[resIndex]), width[resIndex], height[resIndex]);

                    valueNow = gVariable.currentStatusForOneDay[0];
                    for (k = 0; k < len; k++)
                    {
                        value = gVariable.currentStatusForOneDay[k];

//                        dispatchAlarmIDForOneDay[i, k] = value;
                        //(k != (len - 1) || start1 != 0) means it is not that case that the value is 0 from start to end
                        if (value == valueNow && k != (len - 1))
                        {
                            start2++;
                        }
                        else
                        {
                            switch(valueNow)
                            {
                                case gVariable.MACHINE_STATUS_DOWN:
                                    e.Graphics.FillRectangle(colorGrayBrush, start_x[resIndex] + start1, start_y[resIndex] + i * (gap_y[resIndex] + height[resIndex]), start2 - start1 + 1, height[resIndex]);
                                    break;
                                case gVariable.MACHINE_STATUS_IDLE:
                                    e.Graphics.FillRectangle(colorYellowBrush, start_x[resIndex] + start1, start_y[resIndex] + i * (gap_y[resIndex] + height[resIndex]), start2 - start1 + 1, height[resIndex]);
                                    break;
                                case gVariable.MACHINE_STATUS_STARTED:
                                    e.Graphics.FillRectangle(colorGreenBrush, start_x[resIndex] + start1, start_y[resIndex] + i * (gap_y[resIndex] + height[resIndex]), start2 - start1 + 1, height[resIndex]);
                                    break;
                                case gVariable.MACHINE_STATUS_DEVICE_ALARM:
                                    e.Graphics.FillRectangle(colorRedBrush, start_x[resIndex] + start1, start_y[resIndex] + i * (gap_y[resIndex] + height[resIndex]), start2 - start1 + 1, height[resIndex]);
                                    break;
                                case gVariable.MACHINE_STATUS_MATERIAL_ALARM:
                                    e.Graphics.FillRectangle(colorChocolateBrush, start_x[resIndex] + start1, start_y[resIndex] + i * (gap_y[resIndex] + height[resIndex]), start2 - start1 + 1, height[resIndex]);
                                    break;
                                case gVariable.MACHINE_STATUS_DATA_ALARM:
                                    e.Graphics.FillRectangle(colorBrownBrush, start_x[resIndex] + start1, start_y[resIndex] + i * (gap_y[resIndex] + height[resIndex]), start2 - start1 + 1, height[resIndex]);
                                    break;
                            }
                            start1 = start2;
                            start2++;
                            valueNow = value;
                        }
                    }

                    //for 12 timer strings from 00:00 to 24:00
                    for (j = 0; j <= 12; j++)
                        e.Graphics.DrawString(timeStr[j], font2, Brushes.Black, start_x[resIndex] + (120 / gap_x[resIndex] * j) - 15, start_y[resIndex] + i * (gap_y[resIndex] + height[resIndex]) + height[resIndex]);

                    //machine name and dispatch brief info
                    if (gVariable.dispatchSheet[index].dispatchCode == null)
                        str = gVariable.machineNameArray[index] + "(" + machineCodeZihua[i] + ")   当前工单: " + "工单未启动 ";
                    else
                        str = gVariable.machineNameArray[index] + "(" + machineCodeZihua[i] + ")   当前工单: " + gVariable.dispatchSheet[index].dispatchCode + "; 操作员: " + gVariable.dispatchSheet[index].operatorName;
                    e.Graphics.DrawString(str, font2, Brushes.Black, start_x[resIndex] + width[resIndex] + titleGapX[resIndex], start_y[resIndex] + i * (gap_y[resIndex] + height[resIndex]));

                    i++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(databaseName + "failed to draw working load bar:" + ex);
            }

            base.OnPaint(e);
        }

        private void Form1_MouseDown(object sender, EventArgs e)
        {
            int index;
            Point ms = Control.MousePosition;

            if (ms.X < start_x[resIndex] || ms.X > start_x[resIndex] + width[resIndex] || ms.Y < start_y[resIndex] || ms.Y > start_y[resIndex] + (height[resIndex] + gap_y[resIndex]) * machineCodeZihua.Length)
                return;

            index = (ms.Y - start_y[resIndex]) / (gap_y[resIndex] + height[resIndex]) + 1;

            if (index > machineCodeZihua.Length)
                return;

            room_Selected(index);
        }

        private void Form1_MouseMove(object sender, EventArgs e)
        {
//            toolTip1.Active = false;
//            toolTip1.Active = true;
        }

        private void emulateHoverFunc()
        {
            int effective;
            int k, n, v;
//            int len, num;
            int id;
            int index;
            int machineIndex;
            string str;
            string databaseName;
            int type = 0;

            int timeV1, timeV2;

            string statusStr = null;
            Point ms = Control.MousePosition;

            effective = 0;

            if (mouseX != ms.X || mouseY != ms.Y)  //the mouse is still moving, not in hovering mode
            {
                mouseX = ms.X;
                mouseY = ms.Y;
                return;
            }

            try
            {
                if (ms.Y < 31)
                    return;

                machineIndex = 0;
                ms.Y -= 30;  //height[resIndex] for the title bar of this screen, that is out side of the mouse position
                databaseName = null;

//                Console.WriteLine(DateTime.Now.ToString() + ":got hover info, ms.X = " + ms.X + "; ms.Y = " + ms.Y);

//                i = 0;
                for (n = 0; n < machineCodeZihua.Length; n++)
                {
                    if (machineCodeZihua[n].Remove(0, 1) == "000")
                        continue;

                    machineIndex = Convert.ToInt16(machineCodeZihua[n].Remove(0, 1)) - 1;

                    databaseName = machineCodeZihua[n];

                    if (ms.X >= start_x[resIndex] && ms.X <= start_x[resIndex] + width[resIndex] && ms.Y >= start_y[resIndex] + n * (gap_y[resIndex] + height[resIndex]) && ms.Y <= start_y[resIndex] + n * (gap_y[resIndex] + height[resIndex]) + height[resIndex])
                    {
                        effective = 1;
                        break;
                    }
                }

                if (effective == 0)
                    return;

                v = ms.X - start_x[resIndex];  //the minute value of the time frame appointed by the user, or how long will this status keep on 
                if (v < 0 || v >= (1440 / gVariable.onePointstandForHowManyMinutes))
                    return;

//                Console.WriteLine(DateTime.Now.ToString() + ":info into position");

                timeV1 = toolClass.ConvertDateTimeInt(DateTime.Now.Date);
                timeV2 = timeV1 + 3600 * 24;
                //get machine status for one day by minutes
                mySQLClass.readMachineStatusForOneDay(databaseName, gVariable.machineStatusRecordTableName, timeV1, timeV2);

                type = gVariable.currentStatusForOneDay[v];
                index = gVariable.dispatchAlarmIDForOneDay[machineIndex, v];

                switch (type)
                {
                    case gVariable.MACHINE_STATUS_DOWN:
                        k = (toolClass.ConvertDateTimeInt(DateTime.Now) - toolClass.ConvertDateTimeInt(DateTime.Now.Date)) / 60;

                        if(k < v)
                            toolTip1.SetToolTip(this, "时间未到，设备状态未知");
                        else
                            toolTip1.SetToolTip(this, "设备处于停机状态");
                        break;
                    case gVariable.MACHINE_STATUS_IDLE:
                    case gVariable.MACHINE_STATUS_STARTED:
                        str = getTooltipData(databaseName, v, index);
                        toolTip1.SetToolTip(this, str);
                        break;
                    case gVariable.MACHINE_STATUS_MATERIAL_ALARM:
                    case gVariable.MACHINE_STATUS_DEVICE_ALARM:
                    case gVariable.MACHINE_STATUS_DATA_ALARM:
                         id = gVariable.dispatchAlarmIDForOneDay[n, v];
                         alarmTableStructImpl = mySQLClass.getAlarmTableContent(databaseName, gVariable.alarmListTableName, id);

                         if (alarmTableStructImpl.status < gVariable.strAlarmStatus.Length)
                             statusStr = gVariable.strAlarmStatus[alarmTableStructImpl.status];
                         else
                            statusStr = gVariable.strAlarmStatus[0];

                         type = alarmTableStructImpl.type;
                         if (type == gVariable.ALARM_TYPE_DEVICE)
                         {
                            toolTip1.SetToolTip(this, "设备处于设备报警状态\n\r派工单编号：" + alarmTableStructImpl.dispatchCode + "\r\n操作员：" + alarmTableStructImpl.operatorName + "\n\r报警原因：" + alarmTableStructImpl.errorDesc + "\r\n报警时间：" +
                                                alarmTableStructImpl.time + "\r\n报警状态：" + statusStr);
                         }
                         else if (type == gVariable.ALARM_TYPE_MATERIAL)
                         {
                            toolTip1.SetToolTip(this, "设备处于物料报警状态\n\r派工单编号：" + alarmTableStructImpl.dispatchCode + "\r\n操作员：" + alarmTableStructImpl.operatorName + "\n\r缺料批次：" + alarmTableStructImpl.errorDesc + "\r\n报警时间：" +
                                                alarmTableStructImpl.time + "\r\n报警状态：" + statusStr);
                         }
                         else //if (type == gVariable.ALARM_TYPE_QUALITY/CRAFT_DATA)
                         {
                            toolTip1.SetToolTip(this, "设备处于数据报警状态\n\r派工单编号：" + alarmTableStructImpl.dispatchCode + "\r\n操作员：" + alarmTableStructImpl.operatorName + "\n\r报警原因：" + alarmTableStructImpl.errorDesc + "\r\n报警时间：" +
                                                alarmTableStructImpl.time + "\r\n报警状态：" + statusStr);
                         }
                         break;
                    default:
                         break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed in hover function:" + ex);
            }
        }

        //input: time means the minute value of the time frame that need to pop up a tooltip
        private string getTooltipData(string databaseName, int time, int index )
        {
            int i; //, k;
//            int timeV0, timeV1, timeV2;
            string str;
//            string timeS1, timeS2;
            gVariable.dispatchSheetStruct dispatchListImpl;

            if (gVariable.mainFunctionIndex == gVariable.MAIN_FUNCTION_MACHINE)
            {
                for (i = 0; i < gVariable.allMachineIDForZihua.Length; i++)
                {
                    if (gVariable.allMachineIDForZihua[i] == index + 1)
                        break;
                }

                str = "自上次正常维修保养到现在\r\n设备总体开机时间为 1480 个小时\r\n待机时间为 1245 个小时\r\n工作时间为 235 小时\r\n预估下次保养日期为 2017-08-13\r\n\r\n";
                if (i < 5)  //流延机
                {
                    str += "设备当前运行状态：\r\n挤出速度：3m/s；线速度：4m/s；\r\n橡胶辊压力：0.98 kg；橡胶辊代号：33452；橡胶辊温度 89 度；\r\n钢棍代号：434443；钢棍温度：72 度";
                }
                else if (i < 8)  //印刷机
                {
                    str += "设备当前运行状态：\r\n线速度：125m/min；烘箱温度：52 度\r\n刮刀压力：0.31 kg；刮刀角度：40 度；\r\n油墨黏度：17 S\r\n压印辊压力：0.22 kg；放卷张力：6.8 V；\r\n进料张力：0.14 kg；出料张力：6.7 kg";
                }
                else  //分切机
                {
                    str += "设备当前运行状态：\r\n上收卷压辊压力：0.22 kg\r\n下收卷压辊压力：0.34 kg；\r\n线速度：4.2m/s\r\n放卷张力：0.45 kg；收卷张力：0.86 kg";
                }

            }
            else
            {
                dispatchListImpl = mySQLClass.getDispatchByID(databaseName, gVariable.dispatchListTableName, index);
                if (dispatchListImpl.dispatchCode != null)
                {
                    str = "设备处于工作状态\n\r派工单编号：" + dispatchListImpl.dispatchCode + "\r\n产品编码：" + dispatchListImpl.productCode +
                                    "\r\n产品名称：" + dispatchListImpl.productName + "\r\n操作员：" + dispatchListImpl.operatorName +
                                    "\r\n计划开始时间：" + dispatchListImpl.planTime1 + "\r\n计划完工时间：" + dispatchListImpl.planTime2 +
                                    "\n\r计划生产数量：" + dispatchListImpl.plannedNumber + "\r\n已经生产数量：" + dispatchListImpl.outputNumber;
                }
                else
                {
                    str = "设备处于待机状态";
                }
            }

            return str;
        }
    }
}
