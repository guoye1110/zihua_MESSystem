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
using HostPCDataCollection;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace MESSystem.mainUI
{
    public partial class room3 : Form
    {
        const int PIC_WIDTH = 32;
        const int PIC_HEIGHT = 32;

        System.Windows.Forms.Timer aTimer;

        static string[] machineCode = { 
                                 "z096", "z093", "z109", "z062", "z000", "z000", "z125", "z127", "z097", "z110", 
                                 "z092", "z121", "z000", "z000", "z000", "z000", "z095", "z104", "z131", "z078", 
                                 "z000", "z000", "z108", "z113", "z126", "z051", "z124", "z088", "z128", "z091",
                                 "z074", "z118", "z080", "z112", "z130", "z105", "z129", "z099", "z106", "z082",
                                 "z114", "z120", "z000", "z000", "z000", "z000", "z000", 
                                };

        string[] buttonName = new string[machineCode.Length];


        public static room3 room3Class = null; //it is used to reference this windows

        //when closing room class, closeReason = 0 means go back to face class, 1 means go to form1 class
        int closeReason;

        int dpiValue;
        int cycleCounts;
        int cycleCounts_old;
        float dpiScreenFactorX, dpiScreenFactorY;

        ComboBox [] comboBoxArray = new ComboBox[50];

        public room3()
        {
            InitializeComponent();

            //use double video buffer to output display contents to screen, one for processing display data, and one for output, to avoid flickering
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            initWindowData();
            getScalingFactorForScreen();
            resizeForScreen(0);
        }


        private void initWindowData()
        {
            int i, index;
			
            gVariable.fromWhichRoom = 3;
            cycleCounts = 0;
            cycleCounts_old = 0;

            for(i = 0; i < machineCode.Length; i++)
            {
                index = Convert.ToInt16(machineCode[i].Remove(0, 1));
                if(index == 0)
                    buttonName[i] = " ";
                else
                    buttonName[i] = "设备" + index + "\r\n" + gVariable.machineCodeArray[index] + "\r\n" + gVariable.machineNameArray[index];
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
            Console.WriteLine(dpiScreenFactorX + dpiScreenFactorY);
        }

        //resize all components for screen from 1280 * 768 to 1920 * 1080
        private void resizeForScreen(int flag)
        {
            int i; //, j;
            int ret;
            int index;
            int num;
            int ceilingValue = 10;  //this is ceiling for windows screen, its title area
            string tName, dName;
            string today;
            Rectangle rect = new Rectangle();

            int x, y, w, h;
            Button[] buttonArray = {button1,  button2,  button3,  button4,  button5,  button6,  button7,  button8,  button9,  button10, 
                                    button11, button12, button13, button14, button15, button16, button17, button18, button19, button20, 
                                    button21, button22, button23, button24, button25, button26, button27, button28, button29, button30, 
                                    button31, button32, button33, button34, button35, button36, button37, button38, button39, button40,
                                    button41, button42, button43, button44, button45, button46, button47, 
                                   };

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

            for (i = 0; i < buttonArray.Length; i++)
            {
//                j = i + 1;
                buttonArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                buttonArray[i].Text = buttonName[i];
                if (flag == 0)
                {
                    x = buttonArray[i].Location.X * rect.Width / gVariable.EFFECTIVE_SCREEN_X;
                    y = (buttonArray[i].Location.Y - ceilingValue) * rect.Height / gVariable.EFFECTIVE_SCREEN_Y + ceilingValue;
                    h = buttonArray[i].Size.Height * rect.Height / gVariable.EFFECTIVE_SCREEN_Y;
                    w = buttonArray[i].Size.Width * rect.Width / gVariable.EFFECTIVE_SCREEN_X;
                }
                else
                {
                    x = buttonArray[i].Location.X;
                    y = buttonArray[i].Location.Y;
                    h = buttonArray[i].Size.Height;
                    w = buttonArray[i].Size.Width;
                }
                buttonArray[i].Location = new System.Drawing.Point(x, y);
                buttonArray[i].Size = new System.Drawing.Size(w, h);

                index = Convert.ToInt16(machineCode[i].Remove(0, 1));
                if (index == 0)  // 0 means this button doesnot have a machine connected
                    continue;

                index--;  //originally starts from 1, now from 0

                dName = machineCode[i];
                if (dName != "z000")
                {
                    tName = gVariable.alarmTableName;

                    num = mySQLClass.mySQL.numOfAlarmForToday(dName, tName, today, gVariable.ALARM_TYPE_UNDEFAINED);
                    if (num != 0)
                    {
                        if(num < 10)
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
                        if (gVariable.dispatchCurrentStatus[index] >= gVariable.DISPATCH_STATUS_STARTED)
                        {
                            buttonArray[i].BackColor = System.Drawing.Color.Lime;
                            gVariable.connectionStatus[index] = 1;
                        }
                        else
                        {
                            buttonArray[i].BackColor = System.Drawing.Color.Yellow;
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
                        if(gVariable.socketArray[index].IsBound == false)
                            buttonArray[i].BackColor = System.Drawing.Color.LightGray;
                        else
                        {
                            if (gVariable.connectionStatus[index] == 1)
                            {

                                if (gVariable.dispatchCurrentStatus[index] >= gVariable.DISPATCH_STATUS_STARTED)
                                {
                                    buttonArray[i].BackColor = System.Drawing.Color.Lime;
                                }
                                else
                                {
                                    buttonArray[i].BackColor = System.Drawing.Color.Yellow;
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
        }


        private void room_Load(object sender, EventArgs e)
        {
            closeReason = 0;
            gVariable.currentCurveDatabaseName = null;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Text = gVariable.programTitle + "齿轮车间粗加工生产状况一览";

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
        private void room_MouseDown(object sender, EventArgs e)
        {
//            Point ms = Control.MousePosition;
//            Rectangle rect = new Rectangle();
        }


        private void room_Selected(int i)
        {
            gVariable.currentCurveDatabaseName = machineCode[i - 1];
            //this button is not defined
            if (gVariable.currentCurveDatabaseName != "z000")
            {
                gVariable.boardIndexSelected = Convert.ToInt32(gVariable.currentCurveDatabaseName.Remove(0, 1)) - 1;

                //if this dispatch already started, we don't neeed to get dummy data
                if (gVariable.dispatchCurrentStatus[gVariable.boardIndexSelected] == gVariable.DISPATCH_STATUS_DUMMY)
                    communication.ClientThread.getDummyData(gVariable.boardIndexSelected);

                preparationForCurvedisplay(gVariable.currentCurveDatabaseName);
            }
        }

        private void room_FormClosing(object sender, EventArgs e)
        {
//            saveRoomSettingFile();
            if (aTimer != null)
                aTimer.Enabled = false;

            if (closeReason == 0)
            {
                face.faceClass.Show();
            }
        }

        private void preparationForCurvedisplay(string databaseName)
        {
            int ret;

            gVariable.programTitle2 = "齿轮车间设备" + databaseName;

            gVariable.currentDatabaseName = databaseName;

            if (databaseName == "z000")  
            {
                MessageBox.Show("抱歉，选中的机床设备没有连入数据采集系统！", "信息提示", MessageBoxButtons.OK);
                return; //still not, go back to room screen, since dispatch sheet not started yet
            }

            ret = mySQLClass.mySQL.getRecordNumInTable(databaseName, gVariable.dispatchListTableName);
            if (ret == 0)  //database not generated yet
            {
                //there is no dispatch in for this machine, even dummy dispatch is not available
                MessageBox.Show("抱歉，选中的机床设备尚未握手成功，请确认相连的数据采集板已上电，并且操作员在触屏上已启动工单！", "信息提示", MessageBoxButtons.OK);
                return; //still not, go back to room screen, since dispatch sheet not started yet
            }

            gVariable.contemporaryForm0 = 1;

            communication.ClientThread.getCurveInfoIngVariable(databaseName, gVariable.boardIndexSelected, gVariable.CURRENT_READING);

            //go to curve display screen
            Form0.form0Class = new Form0();
            Form0.form0Class.Show();

            //hide room screen for the time being
            this.Hide();
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
            room_Selected(6);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            room_Selected(7);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            room_Selected(8);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            room_Selected(9);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            room_Selected(10);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            room_Selected(11);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            room_Selected(12);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            room_Selected(13);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            room_Selected(14);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            room_Selected(15);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            room_Selected(16);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            room_Selected(17);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            room_Selected(18);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            room_Selected(19);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            room_Selected(20);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            room_Selected(21);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            room_Selected(22);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            room_Selected(23);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            room_Selected(24);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            room_Selected(25);
        }

        private void button26_Click_1(object sender, EventArgs e)
        {
            room_Selected(26);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            room_Selected(27);
        }

        private void button28_Click_1(object sender, EventArgs e)
        {
            room_Selected(28);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            room_Selected(29);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            room_Selected(30);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            room_Selected(31);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            room_Selected(32);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            room_Selected(33);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            room_Selected(34);
        }

        private void button35_Click(object sender, EventArgs e)
        {
            room_Selected(35);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            room_Selected(36);
        }

        private void button37_Click_1(object sender, EventArgs e)
        {
            room_Selected(37);
        }

        private void button38_Click_1(object sender, EventArgs e)
        {
            room_Selected(38);
        }

        private void button39_Click(object sender, EventArgs e)
        {
            room_Selected(39);
        }

        private void button40_Click(object sender, EventArgs e)
        {
            room_Selected(40);
        }

        private void button41_Click_1(object sender, EventArgs e)
        {
            room_Selected(41);
        }

        private void button42_Click_1(object sender, EventArgs e)
        {
            room_Selected(42);
        }

        private void button43_Click(object sender, EventArgs e)
        {
            room_Selected(43);
        }

        private void button44_Click(object sender, EventArgs e)
        {
            room_Selected(44);
        }

        private void button45_Click(object sender, EventArgs e)
        {
            room_Selected(45);
        }
        private void button46_Click_1(object sender, EventArgs e)
        {
            room_Selected(46);
        }
        private void button47_Click_1(object sender, EventArgs e)
        {
            room_Selected(47);
        }
    }
}
