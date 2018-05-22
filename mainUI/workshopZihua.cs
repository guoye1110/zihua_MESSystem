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
using MESSystem.machine;
using MESSystem.alarmFun;
using MESSystem.common;
using MESSystem.commonControl;
using MESSystem.quality;
using MESSystem.communication;

namespace MESSystem.mainUI
{
    public partial class workshopZihua : Form
    {
        const int BUTTON_NUM = 24;

        const int PIC_WIDTH = 32;
        const int PIC_HEIGHT = 32;

        const int PIC_NUM1 = 7;
        const int PIC_NUM2 = 18;

        const int DEFAULT_NO_PRINTER1 = 2;
        const int DEFAULT_NO_PRINTER2 = 4;

        System.Windows.Forms.Timer aTimer;

        //button for machines
        Button[] buttonArray = new Button[BUTTON_NUM];

        //motion pics for prodyuction line
        PictureBox[] pictureBoxArray1 = new PictureBox[PIC_NUM1];

        //percentage progress bar
        Rectangle[] percentageRectArray = new Rectangle[BUTTON_NUM];
//        int[] percentageArray = new int[PIC_NUM2];

        //tips
        ToolTip[] tipArray = new ToolTip[BUTTON_NUM];

        int[] backupRectangleY = new int[BUTTON_NUM];

        SolidBrush colorGreenBrush = new SolidBrush(Color.Lime);
        SolidBrush colorGrayBrush = new SolidBrush(Color.DarkGray);  //not working

        //public string[] gVariable.machineNameArrayDatabase = { "1号上料机", "2号上料机", "3号上料机", "4号上料机", "5号上料机", "6号上料机", "7号上料机", 
        //                                     "1号流延机", "2号流延机", "3号流延机", "4号流延机", "5号流延机", "6号中试机", "7号吹模机", 
        //                                     "1号印刷机", "2号印刷机", "3号印刷机", "4号印刷机", "5号柔印机", 
        //                                     "1号分切机", "2号分切机", "3号分切机", "4号分切机", "5号分切机"
        //                                   };

        public static workshopZihua workshopZihuaClass = null; //it is used to reference this windows

        int pipelineIndex;

        int cycleCounts;
        int cycleCounts_old;

        int notVisible1, notVisible2;

        public workshopZihua()
        {
            InitializeComponent();

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            //use double video buffer to output display contents to screen, one for processing display data, and one for output, to avoid flickering
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            initWindowData();
            initCommonControls();

            refreshScreen();
        }


        private void initWindowData()
        {
            //int i;
            //int x;
            //int resIndex;

            pipelineIndex = 0;
            cycleCounts = 0;
            cycleCounts_old = 0;

            label6.BackColor = Color.Transparent;

            if (gVariable.mainFunctionIndex == gVariable.PRODUCT_CURRENT_STATUS)
            {
                switch (gVariable.CompanyIndex)
                {
                    case gVariable.ZIHUA_ENTERPRIZE:
                        label6.Text = gVariable.enterpriseTitle + "生产管理系统--当前生产状态";
                        break;
                }
            }
            else if (gVariable.mainFunctionIndex == gVariable.MACHINE_MANAGEMENT_MAINTENANCE)
            {
                switch (gVariable.CompanyIndex)
                {
                    case gVariable.ZIHUA_ENTERPRIZE:
                        label6.Text = gVariable.enterpriseTitle + "设备管理系统--设备维护";
                        break;
                }
            }
            else if (gVariable.mainFunctionIndex == gVariable.MACHINE_MANAGEMENT_REPAIRING)
            {
                switch (gVariable.CompanyIndex)
                {
                    case gVariable.ZIHUA_ENTERPRIZE:
                        label6.Text = gVariable.enterpriseTitle + "设备管理系统--设备维修";
                        break;
                }
            }
            else if (gVariable.mainFunctionIndex == gVariable.MACHINE_MANAGEMENT_ITEM_CHECKING)
            {
                switch (gVariable.CompanyIndex)
                {
                    case gVariable.ZIHUA_ENTERPRIZE:
                        label6.Text = gVariable.enterpriseTitle + "设备管理系统--日常检查";
                        break;
                }
            }
            else if (gVariable.mainFunctionIndex == gVariable.QUALITY_MANAGEMENT_SPC_CONTROL)
            {
                switch (gVariable.CompanyIndex)
                {
                    case gVariable.ZIHUA_ENTERPRIZE:
                        label6.Text = gVariable.enterpriseTitle + "质量管理系统--SPC监控";
                        break;
                }
            }
        }

        private void setTooltipData(int buttonIndex)
        {
            int myBoardIndex;
            int powerOnTime;
            string str;
            int[] workingTime = { 620, 808, 930, 210, 34, 412, 654, 290, 385, 321, 356, 349, 779, 1203, 398, 453, 466, 355 };
            int[] idleTime = { 808, 930, 210, 34, 412, 654, 290, 385, 321, 356, 349, 779, 1203, 398, 453, 466, 355, 733 };
            string[] nextMaintain = { "2018-2-21", "2018-7-1", "2018-1-13", "2018-3-25", "2018-2-14", "2018-4-9", "2018-4-2", "2018-2-21", "2018-3-25", 
                                      "2018-4-22", "2018-1-24", "2018-2-28", "2018-5-12", "2018-2-6", "2018-5-18", "2018-3-6", "2018-3-10", "2018-5-1"}; 

            str = null;
            myBoardIndex = buttonIndex;

            if (gVariable.mainFunctionIndex == gVariable.MAIN_FUNCTION_PRODUCTION)
            {
                tipArray[buttonIndex].ToolTipTitle = "工单状态";

                if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                {
                    str = "派工单编号：" + gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                    str += "\r\n产品编码：" + gVariable.dispatchSheet[myBoardIndex].productCode;
                    str += "\r\n产品名称：" + gVariable.dispatchSheet[myBoardIndex].productName;
                    str += "\r\n操作员工：" + gVariable.dispatchSheet[myBoardIndex].operatorName;
                    str += "\r\n计划开始时间：" + gVariable.dispatchSheet[myBoardIndex].planTime1;
                    str += "\r\n计划完成时间：" + gVariable.dispatchSheet[myBoardIndex].planTime2;
                    str += "\r\n实际开始时间：" + gVariable.dispatchSheet[myBoardIndex].realStartTime;
                    str += "\r\n计划生产量：" + gVariable.dispatchSheet[myBoardIndex].plannedNumber.ToString();
                }
                else
                {
                    str = "设备处于无工单生产的待机状态";
                }
            }
            else if (gVariable.mainFunctionIndex == gVariable.MAIN_FUNCTION_MACHINE)
            {
                tipArray[buttonIndex].ToolTipTitle = "设备状态";
                powerOnTime = workingTime[myBoardIndex] + idleTime[myBoardIndex];
                str = gVariable.machineNameArrayDatabase[myBoardIndex] + "自上次正常维修保养到现在\r\n设备总体开机时间为 " + powerOnTime + " 个小时\r\n待机时间为 " + idleTime[myBoardIndex] + 
                      " 个小时\r\n工作时间为 " + + workingTime[myBoardIndex] + " 小时\r\n预估下次保养日期为 " + nextMaintain[myBoardIndex];
                if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                {
                    if (buttonIndex < 5)  //流延机
                    {
                        str += "设备当前运行状态：\r\n挤出速度：3m/s；线速度：4m/s；\r\n橡胶辊压力：0.98 kg；橡胶辊代号：33452；橡胶辊温度 89 度；\r\n钢棍代号：434443；钢棍温度：72 度";
                    }
                    else if (buttonIndex < 8)  //印刷机
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
                    str += "设备当前运行状态：\r\n设备处于待机状态";
                }
            }
            tipArray[buttonIndex].SetToolTip(buttonArray[buttonIndex], str);
        }


        void initCommonControls()
        {
            int i, j;
            int index;
            int resIndex;
            int x, y;
            int x0, y0;
            int gapX, gapY;
            int pic2GapX;
            int width, height;
            int numOfLines;
            int numOfProcess;
            int[] buttonY = { 85, 100, 103, 124, 124, 134 };
            int[] buttonWidth = { 78, 86, 89, 89, 89, 100 };
            int[] buttonHeight = { 22, 27, 32, 33, 34, 36 };

            //production line motion PICs
            int[] pic1Width = { 842, 842, 842, 1052, 1052, 1052 };
            int[] pic1Height = { 80, 80, 80, 99, 99, 99 };

            //progresss bar for dispatch
            int[] pic2Width = { 88, 100, 100, 114, 114, 104 };
            int[] pic2Height = { 10, 12, 12, 14, 14, 14 };

            float[,] titleFontSize = 
            {
                { 21F, 24F, 27F, 28F, 30F, 32F },
                { 19F, 22F, 24F, 25F, 26F, 28F },
                { 15F, 16F, 18F, 19F, 20F, 24F },
            };

            float[,] buttonFontSize = 
            {
                { 8F, 8F, 9F, 9F, 10F, 12F },
                { 7F, 7F, 7F, 7F, 8F,  10F },
                { 6F, 6F, 6F, 7F, 7F,  8F },
            };
            int[] resIndexTableX = { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3 };
            int[] resIndexTableY = { 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4 };

            int[] labelY = { 28, 30, 30, 35, 37, 42 };

            int[] offsetY = { 10, 10, 8, 8, 10, 0};
            //buttons stand for machines
            Button[] bArray = {button1,  button2,   button3, button4,  button5,  button19, button28, 
                               button6,  button7,  button8,  button9,  button10, button18, button27,
                               button11, button14, button13, button15, button21, 
                               button22, button23, button24, button12, button20};

            //pictures stand for prodction lines
            PictureBox[] picArray1 = { pictureBox1, pictureBox2, pictureBox3, pictureBox5, pictureBox8, pictureBox4 };

            Rectangle rect = new Rectangle();

            numOfProcess = 4;
            numOfLines = 6;
            notVisible1 = 15;
            notVisible2 = 17;

            rect = Screen.GetWorkingArea(this);
            width = rect.Width;
            height = rect.Height;

            resIndex = gVariable.resolutionLevel;
            x0 = (width - pic1Width[resIndex]) / 2;
            y0 = buttonY[resIndex];

            gapX = pic1Width[resIndex] / numOfProcess;
            gapY = (height - y0 - 10) / numOfLines + offsetY[resIndex];

            for (i = 0, j = 0; i < resIndexTableX.Length; j++, i++)
            {
                if (i == notVisible1 || i == notVisible2)  //these 2 machine dosenot exist
                {
                //    j--;
                //    bArray[i].Visible = false;
                }

                x = x0 + resIndexTableX[i] * gapX + 5;
                y = y0 + resIndexTableY[i] * gapY;
                bArray[i].Location = new System.Drawing.Point(x, y);
                bArray[i].Size = new System.Drawing.Size(buttonWidth[resIndex], buttonHeight[resIndex]);

                index = j + 1; // gVariable.allMachineIDForZihua[j];
                bArray[i].Text = gVariable.machineNameArrayDatabase[index - 1];

                bArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", buttonFontSize[gVariable.dpiValue, resIndex], System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                buttonArray[i] = bArray[i];

                tipArray[i] = new ToolTip();

                tipArray[i].AutoPopDelay = 10000;
                tipArray[i].InitialDelay = 100;
                tipArray[i].ReshowDelay = 500;
                tipArray[i].ShowAlways = true;
            }

            for (i = 0; i < picArray1.Length; i++)
            {
                x = x0;
                y = y0 + i * gapY + buttonHeight[resIndex] + 3;
                picArray1[i].Location = new System.Drawing.Point(x, y);
                picArray1[i].Size = new System.Drawing.Size(pic1Width[resIndex], pic1Height[resIndex]);

                pictureBoxArray1[i] = picArray1[i];
            }

            pic2GapX = (pic1Width[resIndex] / numOfProcess - buttonWidth[resIndex] - pic2Width[resIndex]) / 5;
            for (i = 0; i < percentageRectArray.Length; i++)
            {
                //if (i == notVisible1 || i == notVisible2)
               //     continue;

                percentageRectArray[i].X = x0 + resIndexTableX[i] * gapX + pic2GapX + buttonWidth[resIndex] + 5;
                percentageRectArray[i].Y = y0 + resIndexTableY[i] * gapY + buttonHeight[resIndex] - pic2Height[resIndex] - 4;

                backupRectangleY[i] = percentageRectArray[i].Y;

                percentageRectArray[i].Width = pic2Width[resIndex];
                percentageRectArray[i].Height = pic2Height[resIndex];
            }

            //resIndex = gVariable.resolutionLevel;
            label6.Font = new System.Drawing.Font("Microsoft Sans Serif", titleFontSize[gVariable.dpiValue, resIndex], System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            x = (width - label6.Size.Width) / 2; 
            y = labelY[resIndex];
            label6.Location = new System.Drawing.Point(x, y);
        }

        void setProgressBar()
        {
            int i;

            for (i = 0; i < percentageRectArray.Length; i++)
            {
                //if (i == notVisible1 || i == notVisible2)
                //    continue;

                percentageRectArray[i].Y = backupRectangleY[i] + AutoScrollPosition.Y;
            }

        }


        //resize all components for screen from 1280 * 768 to 1920 * 1080
        private void refreshScreen()
        {
            int i, j;
            int ret;
            int resIndex;
            int index;
            int num;
            string tName, dName;
            string today;
            string commandText;

            string[,] lineFullIconArray = 
            {
                {
                   "..\\..\\resource\\1024\\zihua\\lineFull1.jpg", "..\\..\\resource\\1280\\zihua\\lineFull1.jpg", "..\\..\\resource\\1366\\zihua\\lineFull1.jpg", 
                   "..\\..\\resource\\1440\\zihua\\lineFull1.jpg", "..\\..\\resource\\1600\\zihua\\lineFull1.jpg", "..\\..\\resource\\1920\\zihua\\lineFull1.jpg",
                },
                {
                   "..\\..\\resource\\1024\\zihua\\lineFull2.jpg", "..\\..\\resource\\1280\\zihua\\lineFull2.jpg", "..\\..\\resource\\1366\\zihua\\lineFull2.jpg", 
                   "..\\..\\resource\\1440\\zihua\\lineFull2.jpg", "..\\..\\resource\\1600\\zihua\\lineFull2.jpg", "..\\..\\resource\\1920\\zihua\\lineFull2.jpg",
                },
                {
                   "..\\..\\resource\\1024\\zihua\\lineFull3.jpg", "..\\..\\resource\\1280\\zihua\\lineFull3.jpg", "..\\..\\resource\\1366\\zihua\\lineFull3.jpg", 
                   "..\\..\\resource\\1440\\zihua\\lineFull3.jpg", "..\\..\\resource\\1600\\zihua\\lineFull3.jpg", "..\\..\\resource\\1920\\zihua\\lineFull3.jpg",
                },
                {
                   "..\\..\\resource\\1024\\zihua\\lineFull4.jpg", "..\\..\\resource\\1280\\zihua\\lineFull4.jpg", "..\\..\\resource\\1366\\zihua\\lineFull4.jpg", 
                   "..\\..\\resource\\1440\\zihua\\lineFull4.jpg", "..\\..\\resource\\1600\\zihua\\lineFull4.jpg", "..\\..\\resource\\1920\\zihua\\lineFull4.jpg",
                },
                {
                   "..\\..\\resource\\1024\\zihua\\lineFull5.jpg", "..\\..\\resource\\1280\\zihua\\lineFull5.jpg", "..\\..\\resource\\1366\\zihua\\lineFull5.jpg", 
                   "..\\..\\resource\\1440\\zihua\\lineFull5.jpg", "..\\..\\resource\\1600\\zihua\\lineFull5.jpg", "..\\..\\resource\\1920\\zihua\\lineFull5.jpg",
                },
                {
                   "..\\..\\resource\\1024\\zihua\\lineFull6.jpg", "..\\..\\resource\\1280\\zihua\\lineFull6.jpg", "..\\..\\resource\\1366\\zihua\\lineFull6.jpg", 
                   "..\\..\\resource\\1440\\zihua\\lineFull6.jpg", "..\\..\\resource\\1600\\zihua\\lineFull6.jpg", "..\\..\\resource\\1920\\zihua\\lineFull6.jpg",
                },
            };
            string[,] linePartIconArray = 
            {
                {
                   "..\\..\\resource\\1024\\zihua\\linePart1.jpg", "..\\..\\resource\\1280\\zihua\\linePart1.jpg", "..\\..\\resource\\1366\\zihua\\linePart1.jpg", 
                   "..\\..\\resource\\1440\\zihua\\linePart1.jpg", "..\\..\\resource\\1600\\zihua\\linePart1.jpg", "..\\..\\resource\\1920\\zihua\\linePart1.jpg",
                },
                {
                   "..\\..\\resource\\1024\\zihua\\linePart2.jpg", "..\\..\\resource\\1280\\zihua\\linePart2.jpg", "..\\..\\resource\\1366\\zihua\\linePart2.jpg", 
                   "..\\..\\resource\\1440\\zihua\\linePart2.jpg", "..\\..\\resource\\1600\\zihua\\linePart2.jpg", "..\\..\\resource\\1920\\zihua\\linePart2.jpg",
                },
                {
                   "..\\..\\resource\\1024\\zihua\\linePart3.jpg", "..\\..\\resource\\1280\\zihua\\linePart3.jpg", "..\\..\\resource\\1366\\zihua\\linePart3.jpg", 
                   "..\\..\\resource\\1440\\zihua\\linePart3.jpg", "..\\..\\resource\\1600\\zihua\\linePart3.jpg", "..\\..\\resource\\1920\\zihua\\linePart3.jpg",
                },
                {
                   "..\\..\\resource\\1024\\zihua\\linePart4.jpg", "..\\..\\resource\\1280\\zihua\\linePart4.jpg", "..\\..\\resource\\1366\\zihua\\linePart4.jpg", 
                   "..\\..\\resource\\1440\\zihua\\linePart4.jpg", "..\\..\\resource\\1600\\zihua\\linePart4.jpg", "..\\..\\resource\\1920\\zihua\\linePart4.jpg",
                },
                {
                   "..\\..\\resource\\1024\\zihua\\linePart5.jpg", "..\\..\\resource\\1280\\zihua\\linePart5.jpg", "..\\..\\resource\\1366\\zihua\\linePart5.jpg", 
                   "..\\..\\resource\\1440\\zihua\\linePart5.jpg", "..\\..\\resource\\1600\\zihua\\linePart5.jpg", "..\\..\\resource\\1920\\zihua\\linePart5.jpg",
                },
                {
                   "..\\..\\resource\\1024\\zihua\\linePart6.jpg", "..\\..\\resource\\1280\\zihua\\linePart6.jpg", "..\\..\\resource\\1366\\zihua\\linePart6.jpg", 
                   "..\\..\\resource\\1440\\zihua\\linePart6.jpg", "..\\..\\resource\\1600\\zihua\\linePart6.jpg", "..\\..\\resource\\1920\\zihua\\linePart6.jpg",
                },
            };

            string[] bmpArray = new string[10];

            try
            {
                resIndex = gVariable.resolutionLevel;

                //7 production lines
                for (i = 0; i < gVariable.castingProcess.Length - 1; i++)
                {
                    //production line with no printer
                    if (i == (DEFAULT_NO_PRINTER1 - 1) || i == (DEFAULT_NO_PRINTER2 - 1))
                    {
                        if (gVariable.machineCurrentStatus[i] < gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            pictureBoxArray1[i].Image = Image.FromFile(linePartIconArray[0, resIndex]);
                        else
                            pictureBoxArray1[i].Image = Image.FromFile(linePartIconArray[pipelineIndex, resIndex]);
                    }
                    else  //with printer
                    {
                        if (gVariable.machineCurrentStatus[i] < gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            pictureBoxArray1[i].Image = Image.FromFile(lineFullIconArray[0, resIndex]);
                        else
                            pictureBoxArray1[i].Image = Image.FromFile(lineFullIconArray[pipelineIndex, resIndex]);
                    }
                }
                pipelineIndex++;

                if (pipelineIndex > 5)
                    pipelineIndex = 0;

                if (pipelineIndex != 1)
                    return;

                cycleCounts++;

                for (i = 0; i < 10; i++)
                    bmpArray[i] = "..\\..\\resource\\" + (i + 1) + ".png";

                today = DateTime.Now.Date.ToString("yy-MM-dd HH:mm:ss");

//                flag = 0;
                for (i = 0, j = 0; i < buttonArray.Length; i++, j++)
                {
                    if (i == notVisible1 || i == notVisible2)  //these 2 machine dosenot exist
                    {
                    //    j--;
                    //    continue;
                    }
                    index = j + 1; // gVariable.allMachineIDForZihua[j];
                    //if (index == 0)  // 0 means this button doesnot have a machine connected
                    //    continue;

                    index--;  //originally starts from 1, now from 0

                    dName = gVariable.DBHeadString + (j + 1).ToString().PadLeft(3, '0');
                    if (dName.Remove(0, 1) != "000")
                    {
                        tName = gVariable.alarmListTableName;

                        //status 2 means completed, 3 means cancelled, < 2 means still alive
                        commandText = "select count(*) from `" + tName + "` where status < 2";
                        num = mySQLClass.getNumOfRecordByCondition(dName, commandText);
                        if (num != 0)
                        {
                            if (num < 10)
                                buttonArray[i].Image = Image.FromFile(bmpArray[num - 1]);
                            else
                                buttonArray[i].Image = Image.FromFile(bmpArray[9]);

                            buttonArray[i].ImageAlign = ContentAlignment.TopLeft;
                        }
                    }

                    if (cycleCounts - cycleCounts_old > 10)  //30 seconds limitation
                    {
                        if (gVariable.connectionCount[index] == gVariable.connectionCount_old[index])
                        {
                            if (gVariable.socketArray[index] == null)
                                buttonArray[i].BackColor = System.Drawing.Color.LightGray;
                            else
                            {
                                if (gVariable.socketArray[index].IsBound == false)
                                    buttonArray[i].BackColor = System.Drawing.Color.LightGray;
                                else
                                {
                                    ret = toolClass.checkRemoteTCPClientConnection(gVariable.socketArray[index]);
                                    if (ret == 0)
                                    {
                                        buttonArray[i].BackColor = System.Drawing.Color.LightYellow;
                                    }
                                    else
                                    {
                                        buttonArray[i].BackColor = System.Drawing.Color.LightGray;
                                        gVariable.connectionStatus[index] = 0;
                                    }
                                }
                            }
                            gVariable.connectionStatus[index] = 0;
                        }
                        else
                        {
                            if (gVariable.machineCurrentStatus[index] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            {
                                buttonArray[i].BackColor = System.Drawing.Color.Lime;
                                gVariable.connectionStatus[index] = 1;
                            }
                            else
                            {
                                buttonArray[i].BackColor = System.Drawing.Color.CadetBlue;
                                gVariable.connectionStatus[index] = 1;
                            }
                        }

                        if (i == buttonArray.Length - 1)
                            cycleCounts_old = cycleCounts;
                        gVariable.connectionCount_old[index] = gVariable.connectionCount[index];
                    }
                    else
                    {
                        if (gVariable.socketArray[index] == null)
                            buttonArray[i].BackColor = System.Drawing.Color.LightGray;
                        else
                        {
                            if (gVariable.socketArray[index].IsBound == false)
                                buttonArray[i].BackColor = System.Drawing.Color.LightGray;
                            else
                            {
                                if (gVariable.connectionStatus[index] == 1)
                                {

                                    if (gVariable.machineCurrentStatus[index] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                                    {
                                        buttonArray[i].BackColor = System.Drawing.Color.Lime;
                                    }
                                    else
                                    {
                                        buttonArray[i].BackColor = System.Drawing.Color.CadetBlue;
                                    }
                                }
                                else
                                {
                                    ret = toolClass.checkRemoteTCPClientConnection(gVariable.socketArray[index]);
                                    if (ret == 0)
                                    {
                                        buttonArray[i].BackColor = System.Drawing.Color.LightYellow;
                                    }
                                    else
                                    {
                                        buttonArray[i].BackColor = System.Drawing.Color.LightGray;
                                        gVariable.connectionStatus[index] = 0;
                                    }
                                }
                            }
                        }
                    }
                }

                Invalidate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("refreshScreen failed! " + ex);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int i;
            int x, y;
            int width, height;
            int percentage;

            try
            {
                setProgressBar();

                for (i = 0; i < percentageRectArray.Length; i++)
                {
                    x = percentageRectArray[i].X;
                    y = percentageRectArray[i].Y;

                    width = percentageRectArray[i].Width;
                    height = percentageRectArray[i].Height;

                    if (gVariable.machineCurrentStatus[i] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                    {
                        if (gVariable.dispatchSheet[i].plannedNumber == 0)
                            percentage = 0;
                        else
                        {
                            if (gVariable.dispatchSheet[i].qualifiedNumber > gVariable.dispatchSheet[i].plannedNumber)
                            {
                                gVariable.dispatchSheet[i].qualifiedNumber = gVariable.dispatchSheet[i].plannedNumber;
                            }
                            percentage = (int)(gVariable.dispatchSheet[i].qualifiedNumber * width / gVariable.dispatchSheet[i].plannedNumber);
                        }
                        //if (percentage == percentageArray[i])
                        //    continue;  //there is no change, 

                        //percentageArray[i] = percentage;

                        if (percentage != 0)
                        {
                            e.Graphics.FillRectangle(colorGreenBrush, x, y, percentage, height);
                        }
                    }
                    else
                    {
                        percentage = 0;
                    }

                    if(percentage < width)
                    {
                        e.Graphics.FillRectangle(colorGrayBrush, x + percentage, y, width - percentage, height);
                    }
                }

                base.OnPaint(e);
//                Console.WriteLine("OnpPaint OK, " + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("OnpPaint :" + ex);
            }
        }


        private void room_Load(object sender, EventArgs e)
        {
            gVariable.currentCurveDatabaseName = null;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            switch (gVariable.CompanyIndex)
            {
                case gVariable.ZIHUA_ENTERPRIZE:
                    this.Text = gVariable.enterpriseTitle + "生产状况一览";
                    break;
            }
            this.KeyPreview = true;  //to make sure this window will accept a key press event

            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 2 seconds
            aTimer.Interval = 500;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_checkAlarm);
        }

        private void timer_checkAlarm(Object source, EventArgs e)
        {
            refreshScreen();
        }


        //not really used, we use this function to get the size and position of buttons, we need to change button size for different screen resolution
        private void room_MouseDown(object sender, EventArgs e)
        {
//            Point ms = Control.MousePosition;
//            Rectangle rect = new Rectangle();
        }


        private void room_Selected(int i)
        {
            //string commandText;
            //string[,] dispatchDataArray;

            if (gVariable.mainFunctionIndex == gVariable.PRODUCT_CURRENT_STATUS)
            {
                if (gVariable.CompanyIndex == gVariable.ZIHUA_ENTERPRIZE)
                {
                    if (i > gVariable.machineNameArrayDatabase.Length)
                        return;

                    gVariable.currentCurveDatabaseName = gVariable.DBHeadString + i.ToString().PadLeft(3, '0');
                    
                    /*
                    commandText = "select * from `" + gVariable.dispatchListTableName + "` order by id DESC";
                    dispatchDataArray = mySQLClass.databaseCommonReading(gVariable.currentCurveDatabaseName, commandText);
                    if(dispatchDataArray == null)
                        Console.WriteLine("Read dispatch failed!!! " + gVariable.currentCurveDatabaseName);

                    gVariable.boardIndexSelected = Convert.ToInt32(gVariable.currentCurveDatabaseName.Remove(0, 1)) - 1;

                    gVariable.dispatchSheet[gVariable.boardIndexSelected] = mySQLClass.getDispatchByID(gVariable.currentCurveDatabaseName, gVariable.dispatchListTableName, Convert.ToInt32(dispatchDataArray[0, 0]));
                     * */

                    gVariable.boardIndexSelected = Convert.ToInt32(gVariable.currentCurveDatabaseName.Remove(0, 1)) - 1;

                    toolClass.getCurrentDispatch(i);
                    if (gVariable.dispatchSheet[gVariable.boardIndexSelected].dispatchCode == null)
                    {
                        Console.WriteLine("Read dispatch failed!!! " + gVariable.currentCurveDatabaseName);
                    }

                    gVariable.dispatchUnderReview = gVariable.dispatchSheet[gVariable.boardIndexSelected].dispatchCode;

                    preparationForCurvedisplay(gVariable.currentCurveDatabaseName);
                }

                //this button is not defined
                if (gVariable.currentCurveDatabaseName.Remove(0, 1) != "000")
                {
                //    gVariable.boardIndexSelected = Convert.ToInt32(gVariable.currentCurveDatabaseName.Remove(0, 1)) - 1;

                    //if this dispatch already started, we don't neeed to get dummy data
                //    if (gVariable.machineCurrentStatus[gVariable.boardIndexSelected] <= gVariable.MACHINE_STATUS_DISPATCH_DUMMY)
                //        toolClass.getDummyData(gVariable.boardIndexSelected);

                //    preparationForCurvedisplay(gVariable.currentCurveDatabaseName);
                }
            }
            else if (gVariable.mainFunctionIndex == gVariable.MACHINE_MANAGEMENT_MAINTENANCE)
            {
                machineManagement.machineManagementClass = new machineManagement(i - 1, gVariable.MACHINE_MANAGEMENT_MAINTENANCE);
                machineManagement.machineManagementClass.Show();
                this.Hide();
            }
            else if (gVariable.mainFunctionIndex == gVariable.MACHINE_MANAGEMENT_REPAIRING)
            {
                machineManagement.machineManagementClass = new machineManagement(i - 1, gVariable.MACHINE_MANAGEMENT_REPAIRING);
                machineManagement.machineManagementClass.Show();
                this.Hide();
            }
            else if (gVariable.mainFunctionIndex == gVariable.MACHINE_MANAGEMENT_ITEM_CHECKING)
            {
                machineManagement.machineManagementClass = new machineManagement(i - 1, gVariable.MACHINE_MANAGEMENT_ITEM_CHECKING);
                machineManagement.machineManagementClass.Show();
                this.Hide();
            }
            else if (gVariable.mainFunctionIndex == gVariable.QUALITY_MANAGEMENT_SPC_CONTROL)
            {
                if (gVariable.CompanyIndex == gVariable.ZIHUA_ENTERPRIZE)
                {
                    if (i > gVariable.machineNameArrayDatabase.Length)
                        return;

                    gVariable.currentCurveDatabaseName = gVariable.DBHeadString + i.ToString().PadLeft(3, '0'); ;
                }

                //four paramters:
                //1: database name
                //2: where comes this requirement, from alarm or quality
                //3: if this function is triggered by alarm, id means index in alarm table
                //4: this is a quaity data or craft data triggered alarm 
                SPCAnalyze SPCAnalyzeClass = new SPCAnalyze(gVariable.internalMachineName[i - 1], gVariable.FROM_QUALITY_MANAGEMENT_FUNC, 0, gVariable.ALARM_TYPE_QUALITY_DATA);  //we only know we are working on the first board/machine, no other info
                if (SPCAnalyzeClass != null && SPCAnalyzeClass.getSPCFuncClassStatus() == 0)  //when data number too small, SPC analyze will fail
                {
                    SPCAnalyzeClass.Show();
                    if(SPCAnalyzeClass.checkResult() >= 0)   //SPC analyze function performed correctly
                        this.Hide();
                }
            }
        }

        private void room_FormClosing(object sender, EventArgs e)
        {
            if (aTimer != null)
            {
                aTimer.Stop();
                aTimer.Enabled = false;
            }

            firstScreen.firstScreenClass.Show();
        }

        private void preparationForCurvedisplay(string databaseName)
        {
            int ret;

            gVariable.currentDatabaseName = databaseName;

            if (databaseName.Remove(0, 1) == "000")  
            {
                MessageBox.Show("抱歉，选中的机床设备没有连入数据采集系统！", "信息提示", MessageBoxButtons.OK);
                return; //still not, go back to room screen, since dispatch sheet not started yet
            }

            ret = mySQLClass.getRecordNumInTable(databaseName, gVariable.dispatchListTableName);
            if (ret == 0)  //database not generated yet
            {
                //there is no dispatch in for this machine, even dummy dispatch is not available
                MessageBox.Show("抱歉，选中的机床设备尚未握手成功，请确认相连的数据采集板已上电，并且操作员在触屏上已启动工单！", "信息提示", MessageBoxButtons.OK);
                return; //still not, go back to room screen, since dispatch sheet not started yet
            }

            gVariable.contemporarydispatchUI = 1;

            toolClass.getCurveInfoIngVariable(databaseName, gVariable.boardIndexSelected, gVariable.CURRENT_READING);

            //go to curve display screen
            dispatchUI.dispatchUIClass = new dispatchUI(gVariable.FUNCTION_WORKSHOP_UI);
            dispatchUI.dispatchUIClass.Show();

            //hide room screen for the time being
            this.Hide();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {  
            switch (e.KeyCode)  
            {  
                case Keys.F1:
                    if (gVariable.CompanyIndex == gVariable.ZIHUA_ENTERPRIZE)
                    {
                        //closeReason = 1;
                        //machineProgress.machineProgressClass = new machineProgress();
                        //machineProgress.machineProgressClass.Show();
                        //hide room screen for the time being
                        //this.Hide();
                    }
                    break;  
                default:  
                    break;  
            }  
        }  


        private void button1_Click(object sender, EventArgs e)
        {
            room_Selected(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            room_Selected(2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            room_Selected(3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            room_Selected(4);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            room_Selected(5);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            room_Selected(8);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            room_Selected(9);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            room_Selected(10);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            room_Selected(11);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            room_Selected(12);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            room_Selected(15);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            room_Selected(16);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            room_Selected(17);
        }


        private void room_MouseHover1(object sender, EventArgs e)
        {
            setTooltipData(0);
        }

        private void room_MouseHover2(object sender, EventArgs e)
        {
            setTooltipData(1);
        }

        private void room_MouseHover3(object sender, EventArgs e)
        {
            setTooltipData(2);
        }

        private void room_MouseHover4(object sender, EventArgs e)
        {
            setTooltipData(3);
        }

        private void room_MouseHover5(object sender, EventArgs e)
        {
            setTooltipData(4);
        }

        private void room_MouseHover6(object sender, EventArgs e)
        {
            setTooltipData(7);
        }

        private void room_MouseHover7(object sender, EventArgs e)
        {
            setTooltipData(8);
        }

        private void room_MouseHover8(object sender, EventArgs e)
        {
            setTooltipData(9);
        }

        private void room_MouseHover9(object sender, EventArgs e)
        {
            setTooltipData(10);
        }

        private void room_MouseHover10(object sender, EventArgs e)
        {
            setTooltipData(11);
        }

        private void room_MouseHover11(object sender, EventArgs e)
        {
            setTooltipData(14);
        }

        private void room_MouseHover12(object sender, EventArgs e)
        {
            setTooltipData(15);
        }

        private void room_MouseHover13(object sender, EventArgs e)
        {
            setTooltipData(16);
        }

        private void room_MouseHover20(object sender, EventArgs e)
        {
            setTooltipData(21);
        }
        private void room_MouseHover21(object sender, EventArgs e)
        {
            setTooltipData(22);
        }
        private void room_MouseHover22(object sender, EventArgs e)
        {
            setTooltipData(23);
        }
        private void room_MouseHover23(object sender, EventArgs e)
        {
            setTooltipData(24);
        }

        private void room_MouseHover24(object sender, EventArgs e)
        {
            setTooltipData(25);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            room_Selected(5);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            room_Selected(12);

        }

        private void button28_Click(object sender, EventArgs e)
        {
            room_Selected(6);

        }

        private void button27_Click(object sender, EventArgs e)
        {
            room_Selected(13);
        }

        private void button14_Click(object sender, EventArgs e)
        {

        }
    }
}
