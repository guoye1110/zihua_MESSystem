using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
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

        string[,] dispatchDataArray;
        string[,] productDataArray;
        string[,] productDataArray2;
        string[,] productDataArray3;
        string[,] materialDataArray;

        int whereFrom;

        Color foreColor;
        Color backgroundColor;
        int oneCurveScreenSize;
        int dispatchPrepareTimeStamp;
        int verticalLineEveryNumOfPoint;

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
            initialization2();
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
            }
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        private void resizeForScreen()
        {
            int i;
            int x, y, w, h;
            int x1, x2, x3, x4, x5, x6;
            int y1, y2;
            int gapX, gapX2, gapY;
            int textBoxW1, textBoxW2, textBoxH;
            float fontSize;
            //float dpiRatioX, dpiRatioY;
            float screenRatioX, screenRatioY;
            GroupBox[] groupBoxArray = { groupBox1, groupBox2, groupBox3, groupBox5, groupBox6, groupBox7 };
            Label[] labelArray1 =     { label10,   label7,   label14,   label16,   label17, label38 };
            TextBox[] textBoxArray1 = { textBox10, textBox7, textBox14, textBox16, textBox17, textBox33};
            Label[] labelArray2 =     { label9,  label6,    label13,   label15,   label5, label37 };
            TextBox[] textBoxArray2 = {textBox9, textBox6,  textBox13, textBox15, textBox5, textBox32};
            Label[] labelArray3 =     { label8,  label11,    label12,   label28,   label4, label36 };
            TextBox[] textBoxArray3 = {textBox8, textBox11,  textBox12, textBox28, textBox4, textBox31 };

            Label[] labelArray4 =     { label18,   label21,  label3,   label25,   label27,   label41 }; 
            TextBox[] textBoxArray4 = { textBox18, textBox21,textBox3, textBox25, textBox27, textBox36};
            Label[] labelArray5 = { label19, label22, label20, label23, label2, label40 };
            TextBox[] textBoxArray5 = { textBox19, textBox22, textBox20, textBox23, textBox2, textBox35 };
            Label[] labelArray6 = { label35, label33, label26, label34, label24, label39 };
            TextBox[] textBoxArray6 = { textBox30, textBox26, textBox24, textBox29, textBox1, textBox34 };

            Chart[] chartArray1 = { chart2 };
            Button[] buttonArray1 = { button1 };

            Label[] labelArray7 = { label29, label30, label31};
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

            x1 = 8;
            x2 = 179;
            x3 = 349;
            x4 = 8;
            x5 = 151;
            x6 = 295;

            y1 = 40;
            gapX = 60;
            gapX2 = 68;
            gapY = 41;
            y2 = 38;

            textBoxW1 = 92;
            textBoxW2 = 73;
            textBoxH = 20;

            fontSize = commonFontSize[gVariable.dpiValue, gVariable.resolutionLevel];

            myMenu1.Font = new System.Drawing.Font("Segoe UI", fontSize);
            myMenu1.Size = new System.Drawing.Size(1580, menuSizeY[gVariable.dpiValue, gVariable.resolutionLevel]);

            screenRatioX = gVariable.screenRatioX;
            screenRatioY = gVariable.screenRatioY;
            //dpiRatioX = gVariable.dpiRatioX;
            //dpiRatioY = gVariable.dpiRatioY;

            //x = (int)(label1.Location.X * screenRatioX);
            //y = (int)(label1.Location.Y * screenRatioY);
            //label1.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            //label1.Location = new System.Drawing.Point(x, y);
            //x = (int)(label32.Location.X * screenRatioX);
            //y = (int)(label32.Location.Y * screenRatioY);
            //label32.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            //label32.Location = new System.Drawing.Point(x, y);

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
                //labelArray5[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                //w = (int)(textBoxW2 * screenRatioX);
                //h = (int)(textBoxH); // * screenRatioY);
                //labelArray5[i].Size = new System.Drawing.Size(w, h);
                x = (int)(x6 * screenRatioX);
                y = (int)((y1 + gapY * i) * screenRatioY);
                labelArray6[i].Location = new System.Drawing.Point(x, y);

                //textBoxArray5[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(textBoxW2 * screenRatioX);
                h = (int)(textBoxH * screenRatioY);
                textBoxArray6[i].Size = new System.Drawing.Size(w, h);
                x = (int)((x6 + gapX2) * screenRatioX);
                y = (int)((y2 + gapY * i) * screenRatioY);
                textBoxArray6[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < labelArray7.Length; i++)
            {
                labelArray7[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(labelArray7[i].Size.Width * screenRatioX);
                h = (int)(labelArray7[i].Size.Height * screenRatioY);
                labelArray7[i].Size = new System.Drawing.Size(w, h);
                x = (int)(labelArray7[i].Location.X * screenRatioX);
                y = (int)(labelArray7[i].Location.Y * screenRatioY);
                labelArray7[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < chartArray1.Length; i++)
            {
                chartArray1[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(chartArray1[i].Size.Width * screenRatioX);
                h = (int)(chartArray1[i].Size.Height * screenRatioY);
                chartArray1[i].Size = new System.Drawing.Size(w, h);
                x = (int)(chartArray1[i].Location.X * screenRatioX);
                y = (int)(chartArray1[i].Location.Y * screenRatioY);
                chartArray1[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < buttonArray1.Length; i++)
            {
                buttonArray1[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(buttonArray1[i].Size.Width * screenRatioX);
                h = (int)(buttonArray1[i].Size.Height * screenRatioY);
                buttonArray1[i].Size = new System.Drawing.Size(w, h);
                x = (int)(buttonArray1[i].Location.X * screenRatioX);
                y = (int)(buttonArray1[i].Location.Y * screenRatioY);
                buttonArray1[i].Location = new System.Drawing.Point(x, y);
            }

            //label30.Text = "当前无工单";
            percentageRectBar.X = label30.Location.X + label30.Size.Width + 5;
            percentageRectBar.Y = label30.Location.Y + 2;
            percentageRectBar.Width = (int)(RectWidth * screenRatioX);
            percentageRectBar.Height = (int)(RectHeight * screenRatioY);

            //label1.Text = "  ";
            //label32.Text = "   ";
        }

        private void initialization2()
        {
            verticalLineEveryNumOfPoint = 6;
            foreColor = Color.Black;
            backgroundColor = Color.Transparent;
            chart2.BackColor = Color.Transparent;
            initCurveChart(chart2);
        }

        private void listView_Load(object sender, EventArgs e)
        {
            gVariable.previousDispatch = null;

            updateScreen(0);

            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 100 ms
            aTimer.Interval = 10000;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_listview);
        }


        private void timer_listview(Object source, EventArgs e)
        {
            updateScreen(1);
        }


        private void updateScreen(int flag)
        {
            int i;
            int x;
            float num;
            //int ret;
            //int index;
            int machineID;
            //int numCraft, numQuality, numMaterial;
            int numMaterial;
            int plannedMaterialNum;
            string date;
            string strTime;
            string productCode;
            string commandText;

            const int MATERIAL_COL_W1 = 38;
            const int MATERIAL_COL_W2 = 88;
            const int MATERIAL_COL_W3 = 61;
            const int MATERIAL_COL_W4 = 68;

            string[] columnName = new string[1];
            string[] columnContent = new string[1];

            try
            {
                machineID = 0;

                label29.Text = "当前流延膜条码信息： 1804406011L320120030400"; // gVariable.scannedData[gVariable.boardIndexSelected];

                if (gVariable.dispatchUnderReview.Length != gVariable.dispatchCodeLength)
                {
                    Console.WriteLine("dispatchLength Error for " + gVariable.dispatchUnderReview);
                    //return;
                }

                date = "20" + gVariable.dispatchUnderReview.Remove(2) + "-" + gVariable.dispatchUnderReview.Remove(4).Remove(0, 2) + "-" + gVariable.dispatchUnderReview.Remove(9).Remove(0, 7);

                commandText = "select * from `" + gVariable.dispatchListTableName + "` where dispatchCode = '" + gVariable.dispatchUnderReview + "'";
                dispatchDataArray = mySQLClass.databaseCommonReading(gVariable.currentDatabaseName, commandText);
                if (dispatchDataArray == null)
                {
                    Console.WriteLine("The input dispatch of " + gVariable.dispatchUnderReview + "is not found in dispatch table");
                    return;
                }
                else
                    machineID = Convert.ToInt32(dispatchDataArray[0, 1]);

                strTime = dispatchDataArray[0, mySQLClass.PREPARE_TIME_IN_DISPATCHLIST_DATABASE];
                if (strTime == null)
                {
                    strTime = dispatchDataArray[0, mySQLClass.START_TIME_IN_DISPATCHLIST_DATABASE];
                    if (strTime == null)  
                    {
                        dispatchPrepareTimeStamp = (int)(TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now) - gVariable.worldStartTime).TotalSeconds;
                    }
                    else  //use dispatch start time
                    {
                        dispatchPrepareTimeStamp = toolClass.timeStringToTimeStamp(strTime);
                    }
                }
                else  //prepare time
                {
                    dispatchPrepareTimeStamp = toolClass.timeStringToTimeStamp(strTime);
                }

                productCode = dispatchDataArray[0, mySQLClass.PRODUCT_CODE_IN_DISPATCHLIST_DATABASE];
                commandText = "select * from `" + gVariable.productTableName + "` where productCode = '" + productCode + "'";
                productDataArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (productDataArray == null)
                {
                    Console.WriteLine("The input product of " + productCode + "is not found in product table");
                    return;
                }

                commandText = "select * from `" + gVariable.materialListTableName + "` where dispatchCode = '" + gVariable.dispatchUnderReview + "'";
                materialDataArray = mySQLClass.databaseCommonReading(gVariable.currentDatabaseName, commandText);
                if (materialDataArray == null)
                {
                    Console.WriteLine("The input dispatch of " + gVariable.dispatchUnderReview + "is not found in material table");
                    return;
                }

                switch (Convert.ToInt32(dispatchDataArray[0, mySQLClass.STATUS_IN_DISPATCHLIST_DATABASE]))
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
                    num = (gVariable.dispatchSheet[gVariable.boardIndexSelected].outputNumber * 100 / gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber);

                if (gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber != 0)
                    label31.Text = num.ToString() + "% (计划生产数" + gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber + ")";
                else
                    label31.Text = "0% (计划数" + gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber + ")";

                //we are now in old dispatch reviewing mode
                //if (gVariable.dispatchUnderReview == null)
                //    return;

                numMaterial = Convert.ToInt32(materialDataArray[0, 5].ToString());

                i = 1;
                textBox10.Text = date;
                textBox9.Text = gVariable.machineNameArrayDatabase[gVariable.boardIndexSelected];
                textBox8.Text = dispatchDataArray[0, mySQLClass.OPERATOR_NAME_IN_DISPATCHLIST_DATABASE];
                textBox7.Text = dispatchDataArray[0, mySQLClass.SALESORDER_CODE_IN_DISPATCHLIST_DATABASE];
                textBox6.Text = dispatchDataArray[0, mySQLClass.BATCH_NUMBER_IN_DISPATCHLIST_DATABASE];
                textBox11.Text = dispatchDataArray[0, mySQLClass.DISPATCH_CODE_IN_DISPATCHLIST_DATABASE];
                textBox14.Text = dispatchDataArray[0, mySQLClass.WORK_SHIFT_IN_DISPATCHLIST_DATABASE];
                textBox13.Text = dispatchDataArray[0, mySQLClass.PLANNED_STARTTIME_IN_DISPATCHLIST_DATABASE];
                textBox12.Text = dispatchDataArray[0, mySQLClass.PLANNED_COMPLETETIME_IN_DISPATCHLIST_DATABASE];
                textBox16.Text = dispatchDataArray[0, mySQLClass.START_TIME_IN_DISPATCHLIST_DATABASE];
                textBox15.Text = dispatchDataArray[0, mySQLClass.PREPARE_TIME_IN_DISPATCHLIST_DATABASE];
                textBox28.Text = dispatchDataArray[0, mySQLClass.COMPLETE_TIME_IN_DISPATCHLIST_DATABASE];

                plannedMaterialNum = 0;
                for (i = 0; i < numMaterial; i++)
                {
                    plannedMaterialNum += Convert.ToInt32(materialDataArray[0, 7 + i * 2]);
                }

                textBox17.Text = plannedMaterialNum.ToString() + "kg"; //planned quantity of material
                textBox5.Text = ""; //real quantity of material 
                textBox4.Text = ""; //yielding rate
                textBox33.Text = dispatchDataArray[0, mySQLClass.FORCAST_NUM_IN_DISPATCHLIST_DATABASE] + "卷";
                textBox32.Text = dispatchDataArray[0, mySQLClass.RECEIVE_NUM_IN_DISPATCHLIST_DATABASE] + "卷";
                textBox31.Text = dispatchDataArray[0, mySQLClass.QUALIFIED_NUM_IN_DISPATCHLIST_DATABASE] + "卷";

                textBox18.Text = productDataArray[0, 3];
                textBox19.Text = productDataArray[0, 2];
                textBox30.Text = productDataArray[0, 4];
                textBox21.Text = productDataArray[0, 6];
                textBox22.Text = productDataArray[0, 7];
                textBox26.Text = productDataArray[0, 10];
                textBox3.Text = productDataArray[0, 11];
                textBox20.Text = productDataArray[0, 12];

                if (productDataArray[0, 5] == "A11" || productDataArray[0, 5] == "A01")
                    textBox24.Text = "需要";
                else
                    textBox24.Text = "不需要";

                textBox25.Text = productDataArray[0, 1];
                textBox23.Text = productDataArray[0, 8];
                textBox29.Text = productDataArray[0, 23];

                if (dispatchDataArray[0, mySQLClass.PRODUCT_CODE2_IN_DISPATCHLIST_DATABASE] != null && dispatchDataArray[0, mySQLClass.PRODUCT_CODE2_IN_DISPATCHLIST_DATABASE].Length > 0)
                {
                    productCode = dispatchDataArray[0, mySQLClass.PRODUCT_CODE2_IN_DISPATCHLIST_DATABASE];
                    commandText = "select * from `" + gVariable.productTableName + "` where productCode = '" + productCode + "'";
                    productDataArray2 = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                    if (productDataArray2 != null)
                    {
                        Console.WriteLine("The input dispatch of " + gVariable.dispatchUnderReview + "is not found in material table");
                        return;
                    }
                    textBox27.Text = productDataArray2[0, 1];
                    textBox2.Text = productDataArray2[0, 8];
                    textBox1.Text = productDataArray2[0, 23];
                }
                else
                {
                    textBox27.Text = "无";
                }


                if (dispatchDataArray[0, mySQLClass.PRODUCT_CODE3_IN_DISPATCHLIST_DATABASE] != null && dispatchDataArray[0, mySQLClass.PRODUCT_CODE3_IN_DISPATCHLIST_DATABASE].Length > 0)
                {
                    productCode = dispatchDataArray[0, mySQLClass.PRODUCT_CODE3_IN_DISPATCHLIST_DATABASE];
                    commandText = "select * from `" + gVariable.productTableName + "` where productCode = '" + productCode + "'";
                    productDataArray3 = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                    if (productDataArray3 == null)
                    {
                        Console.WriteLine("The input dispatch of " + gVariable.dispatchUnderReview + "is not found in material table");
                        return;
                    }
                    textBox36.Text = productDataArray3[0, 1];
                    textBox35.Text = productDataArray3[0, 8];
                    textBox34.Text = productDataArray3[0, 23];
                }
                else
                {
                    textBox36.Text = "无";
                }

                listViewNF1.Clear();

                this.listViewNF1.BeginUpdate();
                listViewNF1.GridLines = true;
                listViewNF1.Dock = DockStyle.Fill;

                listViewNF1.Columns.Add(" ", 1, HorizontalAlignment.Center);
                x = (int)(MATERIAL_COL_W1 * gVariable.screenRatioX);
                listViewNF1.Columns.Add("料仓", x, HorizontalAlignment.Center);
                x = (int)(MATERIAL_COL_W2 * gVariable.screenRatioX);
                listViewNF1.Columns.Add("物料编码", x, HorizontalAlignment.Center);
                x = (int)(MATERIAL_COL_W3 * gVariable.screenRatioX);
                listViewNF1.Columns.Add("预估用量", x, HorizontalAlignment.Center);
                x = (int)(MATERIAL_COL_W4 * gVariable.screenRatioX);
                listViewNF1.Columns.Add("实际投料", x, HorizontalAlignment.Center);

                //index = 0;
                for (i = 0; i < numMaterial; i++)
                {
                    ListViewItem OptionItem = new ListViewItem();

                    OptionItem.SubItems.Add((i + 1).ToString() + "号");
                    OptionItem.SubItems.Add(materialDataArray[0, 6 + i * 2]);
                    OptionItem.SubItems.Add(materialDataArray[0, 7 + i * 2] + "kg");
                    OptionItem.SubItems.Add("");
                    listViewNF1.Items.Add(OptionItem);
                }

                this.listViewNF1.EndUpdate();

                this.Text = gVariable.programTitle + "工单及设备信息表";

                curveChartForDispatch(chart2);

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
            float beatNum;

            try
            {
                index = gVariable.boardIndexSelected;
                x = percentageRectBar.X;
                y = percentageRectBar.Y;

                width = percentageRectBar.Width;
                height = percentageRectBar.Height;

                if (gVariable.dispatchSheet[gVariable.boardIndexSelected].plannedNumber == 0)
                    beatNum = 0;
                else if (gVariable.dispatchSheet[gVariable.boardIndexSelected].outputNumber > gVariable.dispatchSheet[index].plannedNumber)
                    beatNum = gVariable.dispatchSheet[index].plannedNumber;
                else
                    beatNum = gVariable.dispatchSheet[gVariable.boardIndexSelected].outputNumber;

                Graphics gGraphics = groupBox7.CreateGraphics();

                //this version uses beat value as output number
                if (gVariable.dispatchSheet[index].plannedNumber != 0)
                    greenWidth = (int)(beatNum * width / gVariable.dispatchSheet[index].plannedNumber);
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


        private void initCurveChart(Chart chart)
        {
            this.Text = "班组流延膜生产节拍";
            Color[] colorArray = { Color.Green, Color.Red, Color.Black, Color.Blue };

            try
            {
                chart.Series[0].ChartArea = "ChartArea1";
                chart.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                chart.Series[0].Color = System.Drawing.Color.MediumBlue;
                chart.Series[0].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
                chart.Series[0].LabelBackColor = System.Drawing.Color.Transparent;
                chart.Series[0].LabelBorderColor = System.Drawing.Color.Transparent;
                chart.Series[0].Legend = "Legend3";
                chart.Series[0].MarkerColor = System.Drawing.Color.Blue;
                chart.Series[0].MarkerSize = 8;
                chart.Series[0].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
                chart.Series[0].Name = "test";
                chart.Series[0].ShadowOffset = 2;
                chart.Series[0].ChartType = SeriesChartType.Spline; //Line is streight line, SpLine is curve
                chart.Series[0].MarkerStyle = MarkerStyle.Diamond; //shape of the data point
                chart.Series[0].Color = colorArray[0];
                chart.Series[0].BorderWidth = 2;
                chart.Series[0].ShadowOffset = 1;
                chart.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                chart.Series[0].IsValueShownAsLabel = false;
                chart.Series[0].MarkerSize = 4; // size of the data point
                chart.Series[0].XValueType = ChartValueType.String;
                chart.Series[0].YValueType = ChartValueType.Int32;

                chart.Visible = true;
                chart.Titles.Add("班组流延膜生产节拍(秒)");
                chart.Titles[0].ForeColor = foreColor;
                chart.Titles[0].Font = new Font("微软雅黑", 12f, FontStyle.Bold);
                chart.BackColor = backgroundColor;

                //chart.ChartAreas[0].AxisY.LabelStyle.Format = "N1";
                chart.ChartAreas[0].AxisY.IsStartedFromZero = true;  //whether we need to start at 0 for Y axis
                chart.ChartAreas[0].AxisY.IntervalOffset = 1;
                chart.ChartAreas[0].AxisY.Maximum = 1000;
                chart.ChartAreas[0].AxisY.Minimum = 0;
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = foreColor;
                chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                //Y坐标轴标题
                chart.ChartAreas[0].AxisY.TitleFont = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.ChartAreas[0].AxisY.TitleForeColor = foreColor;
                chart.ChartAreas[0].AxisY.Title = "膜卷花费时间";
                //chart1.ChartAreas[0].AxisX.IsStartedFromZero = true;  //whether we need to start at 0 for X axis

                chart.ChartAreas[0].AlignmentOrientation = ((AreaAlignmentOrientations)((AreaAlignmentOrientations.Vertical | AreaAlignmentOrientations.Horizontal)));
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = foreColor;
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.ChartAreas[0].AxisX.Interval = oneCurveScreenSize / verticalLineEveryNumOfPoint;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                chart.ChartAreas[0].AxisX.LabelAutoFitStyle = ((LabelAutoFitStyles)((LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap)));
                chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = oneCurveScreenSize / verticalLineEveryNumOfPoint;
                chart.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0;
                chart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.LineWidth = 2;
                chart.ChartAreas[0].AxisX.TitleFont = new Font("Sans Serif", 8.25f, FontStyle.Regular);
                chart.ChartAreas[0].AxisX.Title = "膜卷完成时间";
                //chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chart.ChartAreas[0].AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.NotSet;
                chart.ChartAreas[0].AxisX.Minimum = 1;
                chart.ChartAreas[0].AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Number;
                chart.ChartAreas[0].AxisX.ScaleView.Size = oneCurveScreenSize;
                chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            }
            catch (Exception ex)
            {
                Console.WriteLine("initCurveChart() failed" + ex);
            }

        }

        private void curveChartForDispatch(Chart chart)
        {
            int i;
            int stamp;
            float f;
            float delta, tmpMax, tmpMin;
            DateTime dateTime;
            string xString;
            string databaseName; 
            string tableName; 
            int totalDataNumInBuffer;  //total number of data need to be dislayed, only part of totalDataNumWanted because of the limitation of oneCurveScreenSize 
            int initialDatPointNum;  //initial point number for a curve

            tmpMax = 0;
            tmpMin = 0;
            initialDatPointNum = 50;

            totalDataNumInBuffer = 0;

            stamp = dispatchPrepareTimeStamp;

            databaseName = gVariable.DBHeadString + (gVariable.boardIndexSelected + 1).ToString().PadLeft(3, '0');
            tableName = gVariable.productBeatTableName;

            if (Convert.ToInt32(dispatchDataArray[0, mySQLClass.STATUS_IN_DISPATCHLIST_DATABASE]) >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                totalDataNumInBuffer = mySQLClass.readProductBeatTable(databaseName, tableName, gVariable.dispatchUnderReview, gVariable.totalPointNumForSChart); //MAX_OUTPUT_ONE_DISPATCH);

            if (totalDataNumInBuffer < initialDatPointNum)
                oneCurveScreenSize = initialDatPointNum;
            else
                oneCurveScreenSize = totalDataNumInBuffer;

            chart.ChartAreas[0].AxisX.ScaleView.Size = oneCurveScreenSize;

            try
            {
                foreach (var series in chart.Series)
                {
                    series.Points.Clear();
                }

                if (totalDataNumInBuffer == 0)
                {
                    xString = DateTime.Now.ToString("MM-dd HH:mm");

                    chart.Series[0].Points.AddXY(xString, 0);
                }
                else
                {
                    for (i = 0; i < totalDataNumInBuffer; i++)
                    {
                        dateTime = toolClass.GetTime((gVariable.timeInPoint[0, i] - 3600 * 7).ToString());
                        xString = dateTime.ToString("MM-dd HH:mm:ss");
                        f = gVariable.timeInPoint[0, i] - stamp; //get time used to complete this roll
                        stamp = gVariable.timeInPoint[0, i];
                        chart.Series[0].Points.AddXY(xString, f);

                        if(gVariable.dataInPoint[0, i] == 0)
                            chart.Series[0].Points[i].Color = Color.Green;
                        else
                            chart.Series[0].Points[i].Color = Color.Red;

                        if (i == 0)
                        {
                            tmpMax = f;
                            tmpMin = f;
                        }

                        if (f > tmpMax)
                            tmpMax = f;

                        if (f < tmpMin)
                            tmpMin = f;
                    }

                
                }

                if (tmpMax < 10)
                    tmpMax = 10;

                delta = tmpMax - tmpMin;
                tmpMax += delta / 7;
                tmpMin = 0;

                if (tmpMax == tmpMin)
                {
                    tmpMax += 1;
                    tmpMin -= 1;
                }

                chart.ChartAreas[0].AxisY.Maximum = tmpMax;
                chart.ChartAreas[0].AxisY.Minimum = tmpMin;
            }
            catch (Exception ex)
            {
                Console.Write("curveChartForDispatch failed! " + ex);
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

