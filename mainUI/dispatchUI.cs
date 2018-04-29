using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MESSystem.common;
using MESSystem.commonControl;
using MESSystem.curves;
using MESSystem.mainUI;
using MESSystem.alarmFun;

namespace MESSystem.mainUI
{
    public partial class dispatchUI : Form
    {
        public static dispatchUI dispatchUIClass = null;

        const string castErrorCode1 = "流延不良   A:晶点孔洞 B:厚薄爆筋 C:皱褶 D:端面错位 E:油污 F:黑点 G:电晕不符 H:色差 I:厚度不符 J:条纹 K:规格不符 L:膜面不平 ";
        const string castErrorCode2 = "M:鱼眼 N:飘膜 O:水印 P:刮伤 Q:翘边 R:收卷 S:克重不符 T:针眼 U:米数不符 V:透明状拉丝 W:蚊虫 X:纹形 Y:其它";

        const string printErrorCode1 = "印刷不良   A:文字/图案方向有误 B:文字/图案位置有误 C:文字/图案完整性缺失 D:印刷牢度不佳 E:溅墨/墨泡 F:刀丝/拖墨 G:漏印/露白 H:色差明显 I:褶皱 J:刮痕";
        const string printErrorCode2 = "K:杂质晶点 L:穿孔破裂 M:刮伤油污 N:厚薄不均 O:端面错位 P:收卷松紧度松散不均 Q:飘膜 R:纹形不佳 S:条纹严重 T:针眼 U:米数不符 V:透明状拉丝 W:蚊虫 X:纹形";

        const string slitErrorCode1 = "分切不良   A:文字/图案方向有误 B:文字/图案完整性缺失 C:溅墨/墨泡 D:刀丝/拖墨 E:漏印/露白 F:收卷松散不均 G:褶皱 H:爆筋 I:穿孔破裂";
        const string slitErrorCode2 = "J:厚薄不均 K:端面错位 L:切边断裂 M:亮斑明显 N:杂质/晶点/鱼眼 O:贴黑角带或铝箔 P:其它";

        //percentage progress bar
        const int RectWidth = 200;
        const int RectHeight = 11;

        int whereFrom;

        Rectangle percentageRectBar = new Rectangle();

        SolidBrush colorGreenBrush = new SolidBrush(Color.Lime);
        SolidBrush colorGrayBrush = new SolidBrush(Color.DarkGray);  //not working

        System.Windows.Forms.Timer aTimer;

        public dispatchUI(int whereFrom_)
        {
            whereFrom = whereFrom_;
            InitializeComponent();
            initialization();
            resizeForScreen();
        }

        private void initialization()
        {
            //enableWMPlayer(0);
            //this notes are only for Zihua, the mean of A,B,C,D...
            if (gVariable.CompanyIndex == gVariable.ZIHUA_ENTERPRIZE)
            {
            }
            else
            {
                label1.Text = null;
                if (gVariable.CompanyIndex == gVariable.DONGFENG_23)
                {
                    label2.Text = "  刀具当前\r\n次数/寿命：";
                }
                else //if (gVariable.CompanyIndex == gVariable.DONGFENG_20)
                {
                    label2.Text = "  模具当前\r\n次数/寿命：";
                    label5.Enabled = false;
                    textBox5.Enabled = false;
                }
            }
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        private void resizeForScreen()
        {
            int i;
            int x, y, w, h;
            int x1, x2, x3, x4, x5;
            int y1, y2;
            int gapX, gapY;
            int textBoxW1, textBoxW2, textBoxH;
            float fontSize;
            //float dpiRatioX, dpiRatioY;
            float screenRatioX, screenRatioY;
            GroupBox[] groupBoxArray = { groupBox1, groupBox2, groupBox3, groupBox4, groupBox5, groupBox6, groupBox7 };
            Label[] labelArray1 =     { label10,   label7,   label14,   label16,   label17 };
            TextBox[] textBoxArray1 = { textBox10, textBox7, textBox14, textBox16, textBox17};
            Label[] labelArray2 =     { label9,  label6,    label13,   label15,   label5 };
            TextBox[] textBoxArray2 = {textBox9, textBox6,  textBox13, textBox15, textBox5};
            Label[] labelArray3 =     { label8,  label11,    label12,   label28,   label4 };
            TextBox[] textBoxArray3 = {textBox8, textBox11,  textBox12, textBox28, textBox4 };
            Label[] labelArray4 =     { label18,   label21,  label3,   label25,   label27 }; 
            TextBox[] textBoxArray4 = { textBox18, textBox21,textBox3, textBox25, textBox27}; 
            Label[] labelArray5 =     { label19,  label22,    label20,   label23,   label2 };
            TextBox[] textBoxArray5 = {textBox19, textBox22,  textBox20, textBox23, textBox2};
            Label[] labelArray6 = { label29, label30, label31};
            float[,] commonFontSize = { 
                                        { 7F,  8F,  9F,  10F, 11F,  12F}, 
                                        { 6F,  7F,  8F, 8.5F, 9F,  10F},  
                                        { 5.5F, 6F, 6.5F, 7F, 7.5F, 8F},  
                                     };
            int[,] menuSizeY = { 
                                  {25, 27, 28, 29, 31, 33}, 
                                  {23, 24, 25, 25, 26, 27}, 
                                  {19, 20, 20, 21, 22, 23},
                               };

            x1 = 15;
            x2 = 203;
            x3 = 391;
            x4 = 15;
            x5 = 191;

            y1 = 40;
            gapX = 60;
            gapY = 41;
            y2 = 38;

            textBoxW1 = 105;
            textBoxW2 = 90;
            textBoxH = 20;

            fontSize = commonFontSize[gVariable.dpiValue, gVariable.resolutionLevel];

            myMenu1.Font = new System.Drawing.Font("Segoe UI", fontSize);
            myMenu1.Size = new System.Drawing.Size(1580, menuSizeY[gVariable.dpiValue, gVariable.resolutionLevel]);

            screenRatioX = gVariable.screenRatioX;
            screenRatioY = gVariable.screenRatioY;
            //dpiRatioX = gVariable.dpiRatioX;
            //dpiRatioY = gVariable.dpiRatioY;

            x = (int)(label1.Location.X * screenRatioX);
            y = (int)(label1.Location.Y * screenRatioY);
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label1.Location = new System.Drawing.Point(x, y);
            x = (int)(label32.Location.X * screenRatioX);
            y = (int)(label32.Location.Y * screenRatioY);
            label32.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label32.Location = new System.Drawing.Point(x, y);

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

            for (i = 0; i < labelArray1.Length; i++)
            {
                //labelArray1[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                x = (int)(x1 * screenRatioX);
                y = (int)((y1 + gapY * i) * screenRatioY);
                labelArray1[i].Location = new System.Drawing.Point(x, y);

                //textBoxArray1[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(textBoxW1 * screenRatioX);
                h = (int)(textBoxH); // * screenRatioY);
                textBoxArray1[i].Size = new System.Drawing.Size(w, h);
                x = (int)((x1 + gapX) * screenRatioX);
                y = (int)((y2 + gapY * i) * screenRatioY);
                textBoxArray1[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < labelArray2.Length; i++)
            {
                //labelArray2[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                x = (int)(x2 * screenRatioX);
                y = (int)((y1 + gapY * i) * screenRatioY);
                labelArray2[i].Location = new System.Drawing.Point(x, y);

                //textBoxArray2[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(textBoxW1 * screenRatioX);
                h = (int)(textBoxH * screenRatioY);
                textBoxArray2[i].Size = new System.Drawing.Size(w, h);
                x = (int)((x2 + gapX) * screenRatioX);
                y = (int)((y2 + gapY * i) * screenRatioY);
                textBoxArray2[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < labelArray3.Length; i++)
            {
                //labelArray3[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                x = (int)(x3 * screenRatioX);
                y = (int)((y1 + gapY * i) * screenRatioY);
                labelArray3[i].Location = new System.Drawing.Point(x, y);

                //textBoxArray3[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(textBoxW1 * screenRatioX);
                h = (int)(textBoxH * screenRatioY);
                textBoxArray3[i].Size = new System.Drawing.Size(w, h);
                x = (int)((x3 + gapX) * screenRatioX);
                y = (int)((y2 + gapY * i) * screenRatioY);
                textBoxArray3[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < labelArray4.Length; i++)
            {
                //labelArray4[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                //w = (int)(labelArray4[i].Size.Width * screenRatioX);
                //h = (int)(labelArray4[i].Size.Height * screenRatioY);
                //labelArray4[i].Size = new System.Drawing.Size(w, h);
                x = (int)(x4 * screenRatioX);
                y = (int)((y1 + gapY * i) * screenRatioY);
                labelArray4[i].Location = new System.Drawing.Point(x, y);

                //textBoxArray4[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(textBoxW2 * screenRatioX);
                h = (int)(textBoxH * screenRatioY);
                textBoxArray4[i].Size = new System.Drawing.Size(w, h);
                x = (int)((x4 + gapX) * screenRatioX);
                y = (int)((y2 + gapY * i) * screenRatioY);
                textBoxArray4[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < labelArray5.Length; i++)
            {
                //labelArray5[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                //w = (int)(textBoxW2 * screenRatioX);
                //h = (int)(textBoxH); // * screenRatioY);
                //labelArray5[i].Size = new System.Drawing.Size(w, h);
                x = (int)(x5 * screenRatioX);
                y = (int)((y1 + gapY * i) * screenRatioY);
                labelArray5[i].Location = new System.Drawing.Point(x, y);

                //textBoxArray5[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(textBoxW2 * screenRatioX);
                h = (int)(textBoxH * screenRatioY);
                textBoxArray5[i].Size = new System.Drawing.Size(w, h);
                x = (int)((x5 + gapX) * screenRatioX);
                y = (int)((y2 + gapY * i) * screenRatioY);
                textBoxArray5[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < labelArray6.Length; i++)
            {
                labelArray6[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(labelArray6[i].Size.Width * screenRatioX);
                h = (int)(labelArray6[i].Size.Height * screenRatioY);
                labelArray6[i].Size = new System.Drawing.Size(w, h);
                x = (int)(labelArray6[i].Location.X * screenRatioX);
                y = (int)(labelArray6[i].Location.Y * screenRatioY);
                labelArray6[i].Location = new System.Drawing.Point(x, y);
            }

            label30.Text = "当前无工单";
            percentageRectBar.X = label30.Location.X + label30.Size.Width + 5;
            percentageRectBar.Y = label30.Location.Y + 2;
            percentageRectBar.Width = (int)(RectWidth * screenRatioX);
            percentageRectBar.Height = (int)(RectHeight * screenRatioY);

            label1.Text = "  ";
            label32.Text = "   ";
        }


        private void listView_Load(object sender, EventArgs e)
        {
            gVariable.previousDispatch = null;

            updateScreen(0);

            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 100 ms
            aTimer.Interval = 4000;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_listview);
        }


        private void timer_listview(Object source, EventArgs e)
        {
            updateScreen(1);
        }


        private void updateScreen(int flag)
        {
            int i, j;
            int x;
            int num;
            int ret;
            int index;
            int machineID;
            int numCraft, numQuality, numMaterial;
            int times1, times2;
            const int MATERIAL_COL_W1 = 40;
            const int MATERIAL_COL_W2 = 92;
            const int MATERIAL_COL_W3 = 61;
            const int MATERIAL_COL_W4 = 88;
            string[] dataArray = new string[30];
            string[] columnName = new string[1];
            string[] columnContent = new string[1];

            try
            {
                machineID = 0;
                if (gVariable.contemporarydispatchUI == 1)
                {
                    label29.Text = gVariable.scannedData[gVariable.boardIndexSelected];

                    switch (gVariable.machineCurrentStatus[gVariable.boardIndexSelected])
                    {
                        case gVariable.MACHINE_STATUS_DISPATCH_DUMMY:
                        case gVariable.MACHINE_STATUS_SHUTDOWN:
                            label30.Text = "当前无工单";
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_APPLIED:
                            label30.Text = "工单已申请";
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_STARTED:
                            label30.Text = "工单已开工";
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_COMPLETED:
                            label30.Text = "工单已完工";
                            break;
                        default:
                            label30.Text = "当前无工单";
                            break;
                    }

                    if (gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber == 0)
                        num = 0;
                    else
                        num = (beatCalculation.beatNum[gVariable.boardIndexSelected] * 100 / gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber);

                    if (gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber != 0)
                        label31.Text = num.ToString() + "% (计划生产数" + gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber + ")";
                    else
                        label31.Text = "0% (计划数" + gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber + ")";

                    //we are in current dispatch displaying mode
                    if (gVariable.dispatchSheet[gVariable.boardIndexSelected].dispatchCode != null)
                    {
                        if (gVariable.machineCurrentStatus[gVariable.boardIndexSelected] == gVariable.MACHINE_STATUS_DISPATCH_COMPLETED)
                            textBox10.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].dispatchCode + " 已完工";
                        else
                            textBox10.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].dispatchCode;

                        //store machine ID
                        if (gVariable.dispatchSheet[gVariable.boardIndexSelected].machineID == null)
                            machineID = 0;
                        else
                            machineID = Convert.ToInt32(gVariable.dispatchSheet[gVariable.boardIndexSelected].machineID);

                        textBox9.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].planTime1;
                        textBox8.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].planTime2;
                        textBox7.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].productCode;
                        textBox6.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].productName;
                        textBox11.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].operatorName;
                        textBox14.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber.ToString();
                        textBox13.Text = (gVariable.dispatchSheet[gVariable.boardIndexSelected].qualifiedNumber + gVariable.dispatchSheet[gVariable.boardIndexSelected].unqualifiedNumber).ToString(); //outputNumber.ToString();
                        textBox12.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].qualifiedNumber.ToString();
                        textBox16.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].unqualifiedNumber.ToString();
                        textBox15.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].processName;
                        textBox17.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].customer;
                        textBox28.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].realStartTime;
                        textBox5.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].prepareTimePoint;
                        textBox4.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].realFinishTime;
                    }

                    if (gVariable.machineStatus[gVariable.boardIndexSelected].machineName != null)
                    {
                        textBox18.Text = gVariable.machineStatus[gVariable.boardIndexSelected].machineCode;
                        textBox19.Text = gVariable.machineStatus[gVariable.boardIndexSelected].machineName;
                        textBox21.Text = gVariable.machineStatus[gVariable.boardIndexSelected].totalWorkingTime.ToString() + "秒";
                        textBox20.Text = beatCalculation.beatNum[gVariable.boardIndexSelected] + "个(" + gVariable.machineStatus[gVariable.boardIndexSelected].productBeat.ToString() + "秒)";
                        textBox23.Text = (gVariable.machineStatus[gVariable.boardIndexSelected].powerConsumed / 3600).ToString() + "度";
                        textBox22.Text = gVariable.machineStatus[gVariable.boardIndexSelected].standbyTime.ToString() + "秒";
                        textBox25.Text = gVariable.machineStatus[gVariable.boardIndexSelected].power.ToString() + "瓦";
                        //textBox24.Text = gVariable.machineStatus[gVariable.boardIndexSelected].collectedNumber.ToString();
                        textBox27.Text = gVariable.machineStatus[gVariable.boardIndexSelected].revolution.ToString();
                        //textBox26.Text = gVariable.machineStatus[gVariable.boardIndexSelected].prepareTime.ToString();
                        textBox3.Text = gVariable.machineStatus[gVariable.boardIndexSelected].workingTime.ToString() + "秒";
                        textBox2.Text = gVariable.dispatchSheet[gVariable.boardIndexSelected].toolUsedTimes.ToString() + "/" + gVariable.dispatchSheet[gVariable.boardIndexSelected].toolLifeTimes.ToString();
                    }
                }
                else
                {
                    //we are now in old dispatch reviewing mode
                    if (gVariable.dispatchUnderReview == null)
                        return;

                    mySQLClass.getOneWholeRecordFromDatabaseByOneStrColumn(gVariable.currentDatabaseName, gVariable.dispatchListTableName, "dispatchCode", gVariable.dispatchUnderReview, dataArray);

                    if (dataArray[0] == null)
                        machineID = 0;
                    else
                        machineID = Convert.ToInt32(dataArray[0]);

                    i = 1;
                    textBox10.Text = dataArray[i++];
                    textBox9.Text = dataArray[i++];
                    textBox8.Text = dataArray[i++];
                    textBox7.Text = dataArray[i++];
                    textBox6.Text = dataArray[i++];
                    textBox11.Text = dataArray[i++];
                    textBox14.Text = dataArray[i++];
                    textBox13.Text = (Convert.ToInt32(dataArray[i + 1]) + Convert.ToInt32(dataArray[i + 2])).ToString();
                    i++;
                    textBox12.Text = dataArray[i++];
                    textBox16.Text = dataArray[i++];
                    textBox15.Text = dataArray[i++];
                    textBox28.Text = dataArray[i++];
                    textBox4.Text = dataArray[i++];
                    textBox5.Text = dataArray[i++];
                    textBox17.Text = dataArray[i++];

                    times1 = Convert.ToInt32(dataArray[i++]);
                    times2 = Convert.ToInt32(dataArray[i++]);

                    ret = mySQLClass.getOneWholeRecordFromDatabaseByOneStrColumn(gVariable.currentDatabaseName, gVariable.machineStatusListTableName, "dispatchCode", gVariable.dispatchUnderReview, dataArray);
                    if (ret >= 0) //otherwise, status table not exist, that means this dspatch is not completed
                    {
                        i = 1;
                        textBox18.Text = gVariable.machineCodeArray[gVariable.boardIndexSelected + 1];
                        textBox19.Text = gVariable.machineNameArray[gVariable.boardIndexSelected + 1];
                        textBox21.Text = dataArray[i++];
                        //textBox24.Text = dataArray[i++];
                        textBox20.Text = dataArray[i++];
                        textBox3.Text = dataArray[i++];
                        //textBox26.Text = dataArray[i++];
                        textBox22.Text = dataArray[i++];
                        textBox25.Text = dataArray[i++];
                        textBox23.Text = dataArray[i++];
                        textBox27.Text = dataArray[i++];
                        textBox2.Text = times2.ToString() + "/" + times1.ToString();
                    }
                }

                //            if (refreshFlag == 0)
                //                return;

                if (gVariable.contemporarydispatchUI == 1)
                {
                    numCraft = gVariable.craftList[gVariable.boardIndexSelected].paramNumber;
                    numQuality = gVariable.qualityList[gVariable.boardIndexSelected].checkItemNumber;
                    numMaterial = gVariable.materialList[gVariable.boardIndexSelected].numberOfTypes;
                }
                else
                {
                    columnName[0] = "dispatchCode";
                    columnContent[0] = gVariable.dispatchUnderReview;

                    numCraft = mySQLClass.getRecordNumByColumnContent(gVariable.currentDatabaseName, gVariable.craftListTableName, columnName, columnContent);
                    numQuality = mySQLClass.getRecordNumByColumnContent(gVariable.currentDatabaseName, gVariable.qualityListTableName, columnName, columnContent);

                    ///***************************************** temp solution, need to change database format
                    numMaterial = 2; // mySQLClass.getRecordNumByColumnContent(gVariable.currentDatabaseName, gVariable.materialListTableName, columnName, columnContent);
                }

                listView1.Clear();
                listView2.Clear();
                listViewNF1.Clear();

                this.listView2.BeginUpdate();

                listView2.GridLines = true;
                listView2.Dock = DockStyle.Fill;
                listView2.Columns.Add(" ", 1, HorizontalAlignment.Center);
                listView2.Columns.Add("序号", 36, HorizontalAlignment.Center);
                listView2.Columns.Add("参数名称", 92, HorizontalAlignment.Center);
                listView2.Columns.Add("参数下限", 80, HorizontalAlignment.Center);
                listView2.Columns.Add("参数上限", 80, HorizontalAlignment.Center);
                listView2.Columns.Add("参数值", 80, HorizontalAlignment.Center);
                listView2.Columns.Add("单位", 70, HorizontalAlignment.Center);

                index = 0;
                for (i = 0; i < numCraft; i++)
                {
                    ListViewItem OptionItem = new ListViewItem();

                    OptionItem.SubItems.Add((i + 1).ToString());
                    if (gVariable.contemporarydispatchUI == 1)
                    {
                        OptionItem.SubItems.Add(gVariable.craftList[gVariable.boardIndexSelected].paramName[i]);
                        OptionItem.SubItems.Add(gVariable.craftList[gVariable.boardIndexSelected].paramLowerLimit[i].ToString());
                        OptionItem.SubItems.Add(gVariable.craftList[gVariable.boardIndexSelected].paramUpperLimit[i].ToString());
                        OptionItem.SubItems.Add(gVariable.craftList[gVariable.boardIndexSelected].paramValue[i].ToString());
                        OptionItem.SubItems.Add(gVariable.craftList[gVariable.boardIndexSelected].paramUnit[i]);
                    }
                    else
                    {
                        j = 1;
                        index = mySQLClass.getNextRecordByOneStrColumn(gVariable.currentDatabaseName, gVariable.craftListTableName, "dispatchCode", gVariable.dispatchUnderReview, index, dataArray);
                        OptionItem.SubItems.Add(dataArray[j++]);
                        OptionItem.SubItems.Add(dataArray[j++]);
                        OptionItem.SubItems.Add(dataArray[j++]);
                        OptionItem.SubItems.Add(dataArray[j++]);
                        OptionItem.SubItems.Add(dataArray[j++]);

                        //refreshFlag = 0;  //so we don't need to do screen refresh next time, since history dispatch/quality/craft/material won't change, one display is OK
                    }

                    listView2.Items.Add(OptionItem);
                }
                listView2.HideSelection = true;
                this.listView2.EndUpdate();

                this.listView1.BeginUpdate();
                listView1.GridLines = true;
                listView1.Dock = DockStyle.Fill;
                listView1.Columns.Add(" ", 1, HorizontalAlignment.Center);
                listView1.Columns.Add("序号", 36, HorizontalAlignment.Center);
                listView1.Columns.Add("检验项目", 125, HorizontalAlignment.Center);
                listView1.Columns.Add("公差下限", 80, HorizontalAlignment.Center);
                listView1.Columns.Add("公差上限", 80, HorizontalAlignment.Center);
                listView1.Columns.Add(gConstText.dispatchUIListTitle8, 70, HorizontalAlignment.Center);
                listView1.Columns.Add(gConstText.dispatchUIListTitle9, 80, HorizontalAlignment.Center);
                listView1.Columns.Add("数值单位", 80, HorizontalAlignment.Center);
                listView1.Columns.Add("结果判断", 61, HorizontalAlignment.Center);
                listView1.Columns.Add("检验要求", 606, HorizontalAlignment.Center);

                index = 0;
                j = 0;
                for (i = 0; i < numQuality; i++)
                {
                    ListViewItem OptionItem = new ListViewItem();

                    OptionItem.SubItems.Add((i + 1).ToString());
                    if (gVariable.contemporarydispatchUI == 1)
                    {
                        OptionItem.SubItems.Add(gVariable.qualityList[gVariable.boardIndexSelected].checkItem[i]);
                        OptionItem.SubItems.Add(gVariable.qualityList[gVariable.boardIndexSelected].specLowerLimit[i].ToString());
                        OptionItem.SubItems.Add(gVariable.qualityList[gVariable.boardIndexSelected].specUpperLimit[i].ToString());
                        OptionItem.SubItems.Add(gVariable.qualityList[gVariable.boardIndexSelected].sampleRatio[i].ToString());
                        OptionItem.SubItems.Add(gVariable.qualityList[gVariable.boardIndexSelected].checkResultData[i]);
                        OptionItem.SubItems.Add(gVariable.qualityList[gVariable.boardIndexSelected].unit[i]);
                        OptionItem.SubItems.Add(gVariable.qualityList[gVariable.boardIndexSelected].checkResult[i]);
                        OptionItem.SubItems.Add(gVariable.qualityList[gVariable.boardIndexSelected].checkRequirement[i]);
                    }
                    else
                    {
                        index = mySQLClass.getNextRecordByOneStrColumn(gVariable.currentDatabaseName, gVariable.qualityListTableName, "dispatchCode", gVariable.dispatchUnderReview, index, dataArray);
                        OptionItem.SubItems.Add(dataArray[1]);
                        OptionItem.SubItems.Add(dataArray[5]);
                        OptionItem.SubItems.Add(dataArray[8]);
                        OptionItem.SubItems.Add(dataArray[11]);
                        OptionItem.SubItems.Add(dataArray[12]);
                        OptionItem.SubItems.Add(dataArray[14]);
                        OptionItem.SubItems.Add(dataArray[13]);
                        OptionItem.SubItems.Add(dataArray[2]);
                        //                    OptionItem.SubItems.Add(dataArray[10]);
                        //                    OptionItem.SubItems.Add(dataArray[11]);
                    }

                    listView1.Items.Add(OptionItem);
                }

                this.listView1.EndUpdate();

                this.listViewNF1.BeginUpdate();
                listViewNF1.GridLines = true;
                listViewNF1.Dock = DockStyle.Fill;

                listViewNF1.Columns.Add(" ", 1, HorizontalAlignment.Center);
                x = (int)(MATERIAL_COL_W1 * gVariable.screenRatioX);
                listViewNF1.Columns.Add("序号", x, HorizontalAlignment.Center);
                x = (int)(MATERIAL_COL_W2 * gVariable.screenRatioX);
                listViewNF1.Columns.Add("物料编码", x, HorizontalAlignment.Center);
                x = (int)(MATERIAL_COL_W3 * gVariable.screenRatioX);
                listViewNF1.Columns.Add("数量", x, HorizontalAlignment.Center);
                x = (int)(MATERIAL_COL_W4 * gVariable.screenRatioX);
                listViewNF1.Columns.Add("供应商", x, HorizontalAlignment.Center);

                index = 0;
                for (i = 0; i < numMaterial; i++)
                {
                    ListViewItem OptionItem = new ListViewItem();

                    OptionItem.SubItems.Add((i + 1).ToString());
                    if (gVariable.contemporarydispatchUI == 1)
                    {
                        OptionItem.SubItems.Add(gVariable.materialList[gVariable.boardIndexSelected].materialCode[i]);
                        OptionItem.SubItems.Add(gVariable.materialList[gVariable.boardIndexSelected].materialRequired[i].ToString());
                        OptionItem.SubItems.Add("杜邦");
                    }
                    else
                    {
                        ///***************************************** temp solution, need to change database format
                        j = 4 + i * 3;
                        ///***************************************** temp solution, need to change database format
                        index = mySQLClass.getNextRecordByOneStrColumn(gVariable.currentDatabaseName, gVariable.materialListTableName, "dispatchCode", gVariable.dispatchUnderReview, index, dataArray);
                        OptionItem.SubItems.Add(dataArray[j++]);
                        OptionItem.SubItems.Add(dataArray[j++]);
                        OptionItem.SubItems.Add("杜邦");
                    }

                    listViewNF1.Items.Add(OptionItem);
                }

                this.listViewNF1.EndUpdate();

                if (machineID <= gVariable.castingProcess.Length)
                {
                    label1.Text = castErrorCode1;
                    label32.Text = castErrorCode2;
                }
                else if (machineID <= gVariable.castingProcess.Length + gVariable.printingProcess.Length)
                {
                    label1.Text = printErrorCode1;
                    label32.Text = printErrorCode2;
                }
                else
                {
                    label1.Text = slitErrorCode1;
                    label32.Text = slitErrorCode2;
                }

                this.Text = gVariable.programTitle + "工单及设备信息表";

                Invalidate();
            }
            catch (Exception ex)
            {
                Console.Write("updateScreen failed in dispatchUI function!" + ex);
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int index;
            int x, y;
            int width, height;
            int greenWidth;
            int beatNum;

            try
            {
                index = gVariable.boardIndexSelected;
                x = percentageRectBar.X;
                y = percentageRectBar.Y;

                width = percentageRectBar.Width;
                height = percentageRectBar.Height;

                if (beatCalculation.beatNum[index] > gVariable.dispatchSheet[index].plannedNumber)
                    beatNum = gVariable.dispatchSheet[index].plannedNumber;
                else
                    beatNum = beatCalculation.beatNum[index];

                Graphics gGraphics = groupBox7.CreateGraphics();

                //this version uses beat value as output number
                if (gVariable.dispatchSheet[index].plannedNumber != 0)
                    greenWidth = beatNum * width / gVariable.dispatchSheet[index].plannedNumber;
                else
                    greenWidth = 0;

                if (greenWidth != 0)
                {
                    gGraphics.FillRectangle(colorGreenBrush, x, y, greenWidth, height);
                }
                else
                {
                    greenWidth = 0;
                }

                if (greenWidth < width)
                {
                    gGraphics.FillRectangle(colorGrayBrush, x + greenWidth, y, width - greenWidth, height);
                }

                base.OnPaint(e);
                //                Console.WriteLine("OnpPaint OK, " + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("dispatchUI OnPaint failed!" + ex);
            }
        }


        private void dispatchUI_FormClosing(object sender, EventArgs e)
        {
            if (aTimer != null)
                aTimer.Enabled = false;

            try
            {
                if (whereFrom == gVariable.FUNCTION_WORKSHOP_UI)
                    workshopZihua.workshopZihuaClass.Show();
                else// if (whereFrom == gVariable.PRODUCT_TIME_DIVISION_STATUS)
                    machineProgress.machineProgressClass.Show();
            }
            catch (Exception ex)
            {
                Console.Write("close workshop class falied" + ex);
            }

        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                listView1.SelectedItems[0].ForeColor = Color.Red;//设置当前选择项为红色
            }
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string str1, str2;

            str1 = Convert.ToInt16(gVariable.currentDatabaseName.Remove(0, 1)).ToString();
            str2 = Application.StartupPath.Remove(Application.StartupPath.Length - 10, 10);  //remove last 10 bytes, that is "\bin\debug"

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = str2 + @"\files\" + str1;
            openFileDialog.Filter = "所有文件|*.*|音频文件|*.mp4|照片文件|*.jpg";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //enableWMPlayer(1);
                //axWindowsMediaPlayer1.URL = openFileDialog.FileName;
                //axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }


        private void enableWMPlayer(int enable)
        {
            // 
            // axWindowsMediaPlayer1
            // 
            //axWindowsMediaPlayer1.Enabled = true;
            if (enable == 0)
            {
                //axWindowsMediaPlayer1.Location = new System.Drawing.Point(635, 50);
                //axWindowsMediaPlayer1.Size = new System.Drawing.Size(1, 1);
            }
            else
            {
                //axWindowsMediaPlayer1.Location = new System.Drawing.Point(635, 50);
                //axWindowsMediaPlayer1.Size = new System.Drawing.Size(881, 654);
            }
            //axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            //axWindowsMediaPlayer1.uiMode = "Full";
            //axWindowsMediaPlayer1.TabIndex = 47;
        }

        /*private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch(e.newState)
            {
                case 3:  //start playing
                    break;
                case 8:  //playing finished
                    //enableWMPlayer(0);
                    break;
                default:
                    break;
            }
        }*/

        private void excecuteFunc(string strExeFunc, string parameter)
        {
//            Process myProcess = new Process();
//            string fileName = "C:/Test.exe";
//            string para = "";

//            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(strExeFunc, parameter);
//            myProcess.StartInfo = myProcessStartInfo;
//            myProcess.Start();
        }

        //multi-curve
        private void aaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multiCurve.multiCurveClass = new multiCurve();
            multiCurve.multiCurveClass.Show();

            this.Hide();
        }

        //column curve
        private void abToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolClass.readPointDataToArrayForPie();

            Column.columnClass = new Column();
            Column.columnClass.Show();

            this.Hide();
        }

        //pie curve
        private void acToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolClass.readPointDataToArrayForPie();

            Pie.pieClass = new Pie();
            Pie.pieClass.Show();

            this.Hide();
        }

        //XBar-R
        private void adaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SPCCurve.SPCCurveClass = new SPCCurve();

            gVariable.SPCChartIndex = gVariable.CHART_TYPE_SPC_XBAR_R;
            SPCCurve.SPCCurveClass.Show();

            this.Hide();
        }

        //XBar-S
        private void adbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SPCCurve.SPCCurveClass = new SPCCurve();

            gVariable.SPCChartIndex = gVariable.CHART_TYPE_SPC_XBAR_S;
            SPCCurve.SPCCurveClass.Show();

            this.Hide();
        }

        //XMed-R
        private void adcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SPCCurve.SPCCurveClass = new SPCCurve();

            gVariable.SPCChartIndex = gVariable.CHART_TYPE_SPC_XMED_R;
            SPCCurve.SPCCurveClass.Show();

            this.Hide();
        }

        //XMed-S
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SPCCurve.SPCCurveClass = new SPCCurve();

            gVariable.SPCChartIndex = gVariable.CHART_TYPE_SPC_X_RM;
            SPCCurve.SPCCurveClass.Show();

            this.Hide();
        }

        //history dispatch
        private void baToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showDispatchList.dispatchListClass = new showDispatchList(gVariable.currentDatabaseName, gVariable.MACHINE_STATUS_DISPATCH_ALL,
                                                                      gVariable.TIME_CHECK_TYPE_REAL_START, gVariable.FUNCTION_DISPATCH_LIST_UI);
            showDispatchList.dispatchListClass.Show();

            this.Hide();
        }

        //andon list
        private void bbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int num;
            int index;
            string today;
            string dName, tName;

            today = DateTime.Now.Date.ToString("yy-MM-dd HH:mm:ss");

            dName = gVariable.currentDatabaseName;
            tName = gVariable.alarmListTableName;

            index = Convert.ToInt16(dName.Remove(0, 1)) - 1;

            if (index >= gVariable.maxMachineNum)
                index = 0;

            num = mySQLClass.getRecordNumInTable(dName, tName);
            if (num != 0)
            {
                alarmListView.alarmListViewClass = new alarmListView(dName, index, gVariable.ALARM_TYPE_ALL_FOR_SELECTION, gVariable.ALARM_STATUS_ALL_FOR_SELECTION, gVariable.FUNCTION_DISPATCH_LIST_UI);
                alarmListView.alarmListViewClass.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("抱歉，本设备无安灯报警记录！", "信息提示", MessageBoxButtons.OK);
            }
        }


        //maintenance record
        private void caToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //repairing record
        private void cbToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //daily check record
        private void ceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //change parts record
        private void cfToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}

