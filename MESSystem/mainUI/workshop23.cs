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
    public partial class workshop23 : Form
    {
        const int PIC_WIDTH = 32;
        const int PIC_HEIGHT = 32;

        int buttonWidth = 82;
        int buttonHeight = 61;

        int button_Start_X = 20;
        int button_Start_Y = 31;
        int button_Delta_X = 118;
        int button_Delta_Y = 90;

        int size_delta = 6;
        int delta_delta = 2;


        private static Button button1 = null,  button2 = null,  button3 = null,  button4 = null,  button5 = null,  button6 = null,  button7 = null,  button8 = null,  button9 = null,  button10 = null;
        private static Button button11 = null, button12 = null, button13 = null, button14 = null, button15 = null, button16 = null, button17 = null, button18 = null, button19 = null, button20 = null;
        private static Button button21 = null, button22 = null, button23 = null, button24 = null, button25 = null, button26 = null, button27 = null, button28 = null, button29 = null, button30 = null;
        private static Button button31 = null, button32 = null, button33 = null, button34 = null, button35 = null, button36 = null, button37 = null, button38 = null, button39 = null, button40 = null;
        private static Button button41 = null, button42 = null, button43 = null, button44 = null, button45 = null, button46 = null, button47 = null, button48 = null, button49 = null, button50 = null;
        private static Button button51 = null, button52 = null, button53 = null, button54 = null, button55 = null, button56 = null, button57 = null, button58 = null, button59 = null, button60 = null;
        private static Button button61 = null, button62 = null, button63 = null, button64 = null, button65 = null, button66 = null, button67 = null, button68 = null, button69 = null, button70 = null;
        private static Button button71 = null, button72 = null, button73 = null, button74 = null, button75 = null, button76 = null, button77 = null, button78 = null, button79 = null, button80 = null;
        private static Button button81 = null, button82 = null, button83 = null, button84 = null, button85 = null, button86 = null, button87 = null, button88 = null, button89 = null, button90 = null;
        private static Button button91 = null, button92 = null, button93 = null, button94 = null, button95 = null, button96 = null, button97 = null, button98 = null, button99 = null, button100 = null;
        private static Button button101 = null, button102 = null, button103 = null, button104 = null, button105 = null, button106 = null, button107 = null, button108 = null, button109 = null, button110 = null;
        private static Button button111 = null, button112 = null, button113 = null, button114 = null, button115 = null, button116 = null, button117 = null, button118 = null, button119 = null, button120 = null;
        private static Button button121 = null, button122 = null, button123 = null, button124 = null, button125 = null, button126 = null, button127 = null, button128 = null, button129 = null, button130 = null;
        private static Button button131 = null, button132 = null, button133 = null, button134 = null, button135 = null, button136 = null, button137 = null, button138 = null, button139 = null, button140 = null;
        private static Button button141 = null, button142 = null, button143 = null, button144 = null, button145 = null, button146 = null, button147 = null, button148 = null, button149 = null, button150 = null;
        private static Button button151 = null, button152 = null, button153 = null, button154 = null, button155 = null, button156 = null, button157 = null, button158 = null, button159 = null, button160 = null;

        private static Button[,] buttonArray = 
        {
	        {button1,   button2,   button3,   button4,   button5,   button6,   button7,   button8,   button9,   button10,  button11,  button12,  button13,  button14,  button15,  button16}, 
  	        {button17,  button18,  button19,  button20,  button21,  button22,  button23,  button24,  button25,  button26,  button27,  button28,  button29,  button30,  button31,  button32}, 
    	    {button33,  button34,  button35,  button36,  button37,  button38,  button39,  button40,  button41,  button42,  button43,  button44,  button45,  button46,  button47,  button48},  
      	    {button49,  button50,  button51,  button52,  button53,  button54,  button55,  button56,  button57,  button58,  button59,  button60,  button61,  button62,  button63,  button64},  
        	{button65,  button66,  button67,  button68,  button69,  button70,  button71,  button72,  button73,  button74,  button75,  button76,  button77,  button78,  button79,  button80},
	        {button81,  button82,  button83,  button84,  button85,  button86,  button87,  button88,  button89,  button90,  button91,  button92,  button93,  button94,  button95,  button96},  
  	        {button97,  button98,  button99,  button100, button101, button102, button103, button104, button105, button106, button107, button108, button109, button110, button111, button112}, 
    	    {button113, button114, button115, button116, button117, button118, button119, button120, button121, button122, button123, button124, button125, button126, button127, button128}, 
      	    {button129, button130, button131, button132, button133, button134, button135, button136, button137, button138, button139, button140, button141, button142, button143, button144},
      	    {button145, button146, button147, button148, button149, button150, button151, button152, button153, button154, button155, button156, button157, button158, button159, button160},
        };

        static string[] backgroundPic = { "..\\..\\resource\\workshop1.jpg", "..\\..\\resource\\workshop2.jpg", "..\\..\\resource\\workshop3.jpg", "..\\..\\resource\\workshop4.jpg" };
        static string[] workshopName = {"卡普车间", "精加工车间", "粗加工车间", "热处理车间" };

        System.Windows.Forms.Timer aTimer;

        public static workshop23 workshop23Class = null; //it is used to reference this windows

        //when closing workshop23 class, closeReason = 0 means go back to firstScreen class, 1 means go to multiCurve class
        int closeReason;

        int workshopIndex;
        int cycleCounts;
        int cycleCounts_old;
        //int dpiValue;
        //float dpiScreenFactorX, dpiScreenFactorY;

        ComboBox [] comboBoxArray = new ComboBox[50];

        public workshop23(int index)
        {
            workshopIndex = index;
            gVariable.realMachineNum = -1;

            InitializeComponent();
            InitializeComponent2();

//            initWindowData();
//            getScalingFactorForScreen();
            resizeForScreen(0);
        }


        private void InitializeComponent2()
        {
            int i, j;
            int x, y;
            int boardIndex;

            Rectangle rect = new Rectangle();

            rect = Screen.GetWorkingArea(this);

            if (rect.Width > gVariable.EFFECTIVE_SCREEN_X)
                rect.Width = gVariable.EFFECTIVE_SCREEN_X;
            if (rect.Height > gVariable.EFFECTIVE_SCREEN_Y)
                rect.Height = gVariable.EFFECTIVE_SCREEN_Y;

            for (i = 0; i < gVariable.totalButtonLineNum; i++)
            {
                for (j = 0; j < gVariable.totalButtonColumnNum; j++)
                {
                    buttonArray[i, j] = new System.Windows.Forms.Button();
                }
            }

            if(rect.Width < 1400)
            {
                buttonWidth -= size_delta;
                buttonHeight -= size_delta;

                button_Start_Y -= size_delta;
                button_Delta_X -= delta_delta;
                button_Delta_Y -= delta_delta;
            }

            for (i = 0; i < gVariable.totalButtonLineNum; i++)
            {
                for (j = 0; j < gVariable.totalButtonColumnNum; j++)
                {
                    x = button_Start_X + button_Delta_X * j * rect.Width / gVariable.EFFECTIVE_SCREEN_X;
                    y = button_Start_Y + button_Delta_Y * i * rect.Height / gVariable.EFFECTIVE_SCREEN_Y;
                    
                    buttonArray[i, j].Location = new System.Drawing.Point(x, y);
                    boardIndex = gVariable.buttonBoardIndexTable[workshopIndex, i * gVariable.totalButtonColumnNum + j];
                    if (boardIndex < 0)
                    {
                        buttonArray[i, j].Enabled = false;
                        buttonArray[i, j].Visible = false;
                    }
                    else if (boardIndex == gVariable.button_Index_Not_Valid)
                    {
                        buttonArray[i, j].Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        buttonArray[i, j].Text = " ";
                        buttonArray[i, j].Name = "button" + (i * gVariable.totalButtonColumnNum + j + 1);
                    }
                    else
                    {
                        buttonArray[i, j].Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        buttonArray[i, j].Text = gVariable.DBHeadString + boardIndex.ToString().PadLeft(3, '0') + "\r\n" + gVariable.machineCodeArray[boardIndex];
                        buttonArray[i, j].Name = "button" + (i * gVariable.totalButtonColumnNum + j + 1);
                        buttonArray[i, j].Click += new System.EventHandler(button_Clicked);
                    }

                    buttonArray[i, j].Size = new System.Drawing.Size(buttonWidth, buttonHeight);
                    buttonArray[i, j].TabIndex = (i * gVariable.totalButtonColumnNum + j + 1);
                    buttonArray[i, j].UseVisualStyleBackColor = true;
                }
            }

            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = Image.FromFile(backgroundPic[workshopIndex]);
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(1424, 862);

            for (i = 0; i < gVariable.totalButtonLineNum; i++)
            {
                for (j = 0; j < gVariable.totalButtonColumnNum; j++)
                {
                    this.Controls.Add(buttonArray[i, j]);
                }
            }

//            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            Name = "workshop23";
            Text = "车间生产状况一览";

            //use double video buffer to output display contents to screen, one for processing display data, and one for output, to avoid flickering
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

/*
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
        }
*/

        //resize all components for screen from 1280 * 768 to 1920 * 1080
        private void resizeForScreen(int flag)
        {
            int i, j;
            int ret;
            int num;
//            int ceilingValue = 10;  //this is ceiling for windows screen, its title area
            int boardIndex;
            int x, y;
            string tName, dName;
            string today;
            Rectangle rect = new Rectangle();


            string[] bmpArray = new string[10];

            cycleCounts++;

            for (i = 0; i < 10; i++)
                bmpArray[i] = "..\\..\\resource\\" + (i + 1) + ".png";

            rect = Screen.GetWorkingArea(this);

            if (rect.Width > gVariable.EFFECTIVE_SCREEN_X)
                rect.Width = gVariable.EFFECTIVE_SCREEN_X;
            if (rect.Height > gVariable.EFFECTIVE_SCREEN_Y)
                rect.Height = gVariable.EFFECTIVE_SCREEN_Y;

            today = DateTime.Now.Date.ToString("yy-MM-dd HH:mm:ss");

            for (i = 0; i < gVariable.totalButtonLineNum; i++)
            {
                for (j = 0; j < gVariable.totalButtonColumnNum; j++)
                {
                    x = button_Start_X + button_Delta_X * j * rect.Width / gVariable.EFFECTIVE_SCREEN_X;
                    y = button_Start_Y + button_Delta_Y * i * rect.Height / gVariable.EFFECTIVE_SCREEN_Y;

                    boardIndex = gVariable.buttonBoardIndexTable[workshopIndex, i * gVariable.totalButtonColumnNum + j];
                    if (boardIndex < 0 || boardIndex == gVariable.button_Index_Not_Valid)
                        continue;

                    boardIndex--;
                    dName = gVariable.internalMachineName[boardIndex];
                    if (dName.Remove(0, 1) != "000")
                    {
                        tName = gVariable.alarmListTableName;

                        num = mySQLClass.numOfAlarmForToday(dName, tName, today, gVariable.ALARM_TYPE_UNDEFINED);
                        if (num != 0)
                        {
                            if (num < 10)
                                buttonArray[i, j].Image = Image.FromFile(bmpArray[num - 1]);
                            else
                                buttonArray[i, j].Image = Image.FromFile(bmpArray[9]);

                            buttonArray[i, j].ImageAlign = ContentAlignment.TopLeft;
                        }
                        else
                            buttonArray[i, j].Image = null;
                    }

                    if (cycleCounts - cycleCounts_old > 10)  //30 seconds limitation
                    {
                        if (gVariable.connectionCount[boardIndex] == gVariable.connectionCount_old[boardIndex])
                        {
                            if (gVariable.socketArray[boardIndex] == null)
                                buttonArray[i, j].BackColor = System.Drawing.Color.LightGray;
                            else
                            {
                                if (gVariable.socketArray[boardIndex].IsBound == false)
                                    buttonArray[i, j].BackColor = System.Drawing.Color.LightGray;
                                else
                                {
                                    ret = toolClass.checkRemoteTCPClientConnection(gVariable.socketArray[boardIndex]);
                                    if (ret == 0)
                                    {
                                        buttonArray[i, j].BackColor = System.Drawing.Color.LightYellow;
                                    }
                                    else
                                    {
                                        buttonArray[i, j].BackColor = System.Drawing.Color.LightGray;
                                        gVariable.connectionStatus[boardIndex] = 0;
                                    }
                                }
                            }
                            gVariable.connectionStatus[boardIndex] = 0;
                        }
                        else
                        {
                            if (gVariable.machineCurrentStatus[boardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            {
                                buttonArray[i, j].BackColor = System.Drawing.Color.Lime;
                                gVariable.connectionStatus[boardIndex] = 1;
                            }
                            else
                            {
                                buttonArray[i, j].BackColor = System.Drawing.Color.Yellow;
                                gVariable.connectionStatus[boardIndex] = 1;
                            }
                        }
                        
                        if (i == buttonArray.Length - 1)
                            cycleCounts_old = cycleCounts;
                        gVariable.connectionCount_old[boardIndex] = gVariable.connectionCount[boardIndex];
                    }
                    else
                    {
                        if (gVariable.socketArray[boardIndex] == null)
                            buttonArray[i, j].BackColor = System.Drawing.Color.LightGray;
                        else
                        {
                            if(gVariable.socketArray[boardIndex].IsBound == false)
                                buttonArray[i, j].BackColor = System.Drawing.Color.LightGray;
                            else
                            {
                                if (gVariable.connectionStatus[boardIndex] == 1)
                                {
                    
                                    if (gVariable.machineCurrentStatus[boardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                                    {
                                        buttonArray[i, j].BackColor = System.Drawing.Color.Lime;
                                    }
                                    else
                                    {
                                        buttonArray[i, j].BackColor = System.Drawing.Color.Yellow;
                                    }
                                }
                                else
                                {
                                    ret = toolClass.checkRemoteTCPClientConnection(gVariable.socketArray[boardIndex]);
                                    if (ret == 0)
                                    {
                                        buttonArray[i, j].BackColor = System.Drawing.Color.LightYellow;
                                    }
                                    else
                                    {
                                        buttonArray[i, j].BackColor = System.Drawing.Color.LightGray;
                                        gVariable.connectionStatus[boardIndex] = 0;
                                    } 
                                }
                            }
                        }
                    }
                }
            }
        }


        private void workshop23_Load(object sender, EventArgs e)
        {
            closeReason = 0;
            gVariable.currentCurveDatabaseName = null;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Text = gVariable.programTitle + "（齿轮厂新厂" + workshopName[workshopIndex] + "生产状况一览）";

            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 2 seconds
            aTimer.Interval = 2000;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_checkAlarm);
        }

        private void timer_checkAlarm(Object source, EventArgs e)
        {
            resizeForScreen(1);
        }

        //not really used, we use this function to get the size and position of buttons, we need to change button size for different screen resolution
        private void workshop23_MouseDown(object sender, EventArgs e)
        {
//            Point ms = Control.MousePosition;
//            Rectangle rect = new Rectangle();
        }


        private void workshop23_Selected(int i)
        {
            gVariable.boardIndexSelected = gVariable.buttonBoardIndexTable[workshopIndex, i] - 1;

            if (gVariable.boardIndexSelected < 1 && gVariable.boardIndexSelected > gVariable.maxMachineNum + 2)
                return;

            gVariable.currentCurveDatabaseName = gVariable.internalMachineName[gVariable.boardIndexSelected];

            //if this dispatch already started, we don't need to get dummy data
            if (gVariable.machineCurrentStatus[gVariable.boardIndexSelected] <= gVariable.MACHINE_STATUS_DISPATCH_DUMMY)
                toolClass.getDummyData(gVariable.boardIndexSelected);

            preparationForCurvedisplay(gVariable.currentCurveDatabaseName);
        }

        private void workshop23_FormClosing(object sender, EventArgs e)
        {
//            saveworkshop23SettingFile();
            if (aTimer != null)
                aTimer.Enabled = false;

            if (closeReason == 0)
            {
                firstScreen.firstScreenClass.Show();
            }
        }

        private void preparationForCurvedisplay(string databaseName)
        {
            int ret;

            gVariable.programTitle2 = "流延车间";// +databaseName;

            gVariable.currentDatabaseName = databaseName;

            if (databaseName.Remove(0, 1) == "000")  
            {
                MessageBox.Show("抱歉，选中的机床设备没有连入数据采集系统！", "信息提示", MessageBoxButtons.OK);
                return; //still not, go back to workshop23 screen, since dispatch sheet not started yet
            }

            ret = mySQLClass.getRecordNumInTable(databaseName, gVariable.dispatchListTableName);
            if (ret == 0)  //database not generated yet
            {
                //there is no dispatch in for this machine, even dummy dispatch is not available
                MessageBox.Show("抱歉，选中的机床设备尚未握手成功，请确认相连的数据采集板已上电，并且操作员在触屏上已启动工单！", "信息提示", MessageBoxButtons.OK);
                return; //still not, go back to workshop23 screen, since dispatch sheet not started yet
            }

            gVariable.contemporarydispatchUI = 1;

            toolClass.getCurveInfoIngVariable(databaseName, gVariable.boardIndexSelected, gVariable.CURRENT_READING);

            //go to curve display screen
            dispatchUI.dispatchUIClass = new dispatchUI();
            dispatchUI.dispatchUIClass.Show();

            //hide workshop23 screen for the time being
            this.Hide();
        }


        private void button_Clicked(object sender, EventArgs e)
        {
            int i, j;
            Button sendButton = (Button)sender;

            for (i = 0; i < gVariable.totalButtonLineNum; i++)
            {
                for (j = 0; j < gVariable.totalButtonColumnNum; j++)
                {
                    if(buttonArray[i, j] == sendButton)
                        workshop23_Selected(i * gVariable.totalButtonColumnNum + j);
                }
            }
        }
    }
}
