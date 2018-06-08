using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using MESSystem.common;
using MESSystem.quality;
using MESSystem.mainUI;

namespace MESSystem.curves
{
    public partial class multiCurve : Form
    {
        public static multiCurve multiCurveClass = null; //it is used to store the current winform class, will be referenced by other class

        //a flag to indicate where we want to exit
        //0 means exit to higher layer -> room function; 
        //1 means we want to move to other screen of the same layer
        int closeReason;

        //to check if new data arrived, if no new data for a period of time, we set value in text box to 0
//        int cycleCounts;
//        int cycleCounts_old;

        System.Windows.Forms.Timer aTimer;

        public static float[] cp = new float[gVariable.maxCurveNum];
        public static float[] cpk = new float[gVariable.maxCurveNum];
        public static float[] ppk = new float[gVariable.maxCurveNum];

        public multiCurve()
        {
            InitializeComponent();
            resizeForScreen();
        }

        //resize all components for screen from 1280 * 768 to 1920 * 1080
        private void resizeForScreen()
        {
            int i, j;
            int m, n;  //width and height of the separator
            int textBoxX, textBoxWidth;
            int labelStartX, labelStartY, labelDeltaY;
            int chartStartX, chartStartY, chartWidth, chartHeight;
            int panel1X = 0, panel1Y = 0;
            Chart[] chartArray = { chart1,  chart2,  chart3,  chart4,  chart5,  chart6,  chart7,  chart8,  chart9,  chart10, 
                                   chart11, chart12, chart13, chart14, chart15, chart16, chart17, chart18, chart19, chart20, 
                                   chart21, chart22, chart23, chart24, chart25, chart26, chart27, chart28, chart29, chart30, 
                                   chart31, chart32, chart33, chart34, chart35, chart36
                                 };

            TextBox[] textBoxArray = { textBox1,  textBox2,  textBox3,  textBox4,  textBox5,  textBox6,  textBox7,  textBox8,  textBox9,  textBox10, 
                                       textBox11, textBox12, textBox13, textBox14, textBox15, textBox16, textBox17, textBox18, textBox19, textBox20, 
                                       textBox21, textBox22, textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30, 
                                       textBox31, textBox32, textBox33, textBox34, textBox35, textBox36
                                     };

            Label[] labelArray = { label1,  label2,  label3,  label4,  label5,  label6,  label7,  label8,  label9,  label10, 
                                   label11, label12, label13, label14, label15, label16, label17, label18, label19, label20,
                                   label21, label22, label23, label24, label25, label26, label27, label28, label29, label30,
                                   label31, label32, label33, label34, label35, label36
                                 };

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            Rectangle rect = new Rectangle();

            rect = Screen.GetWorkingArea(this);

            m = 3; //width of the separator
            n = 1; //height of the separator

            if (rect.Width > 1920)
                rect.Width = 1920;
            if (rect.Height > 1080)
                rect.Height = 1080;

            textBoxX = 135 * rect.Width / 1920 + (1920 - rect.Width) / 50;
            textBoxWidth = 83 * 1920 / rect.Width;

            labelStartX = 8 * rect.Width / 1920;

            labelStartY = 64;
            labelDeltaY = 26;

            chartStartX = 0;
            chartStartY = 0;

            chartWidth = gVariable.CURVE_CHART_WIDTH * (rect.Width + 1920) / 3840;
            chartHeight = gVariable.CURVE_CHART_HEIGHT;

            for (i = 0; i < gVariable.maxCurveNum; i++)
            {
                j = i / 3;

                if (i % 3 == 0)
                    chartArray[i].Location = new System.Drawing.Point(chartStartX, chartStartY + (chartHeight + n) * j);
                else if (i % 3 == 1)
                    chartArray[i].Location = new System.Drawing.Point(chartStartX + chartWidth + m, chartStartY + (chartHeight + n) * j);
                else
                    chartArray[i].Location = new System.Drawing.Point(chartStartX + chartWidth * 2 + m * 2, chartStartY + (chartHeight + n) * j);

                chartArray[i].Size = new System.Drawing.Size(chartWidth, chartHeight);

                labelArray[i].Location = new System.Drawing.Point(labelStartX, labelStartY + labelDeltaY * (i - 1));
                labelArray[i].Size = new System.Drawing.Size(70, 20);

                textBoxArray[i].Location = new System.Drawing.Point(textBoxX, labelStartY + labelDeltaY * (i - 1));
                textBoxArray[i].Size = new System.Drawing.Size(textBoxWidth * rect.Width / 1920, 22);
            }

            panel1X = gVariable.CURVE_SPLIT_POSITION * (rect.Width + 1920) / 4000;
            panel1Y = (chartHeight + 1) * (gVariable.totalCurveNum[gVariable.boardIndexSelected] + 2) / 3 - 1;
            this.panel1.Size = new System.Drawing.Size(panel1X, panel1Y);

            panel1X += m;
            this.panel2.Location = new System.Drawing.Point(panel1X, 26);

            panel1X = chartWidth * 3 + m * 2;
            panel1Y = (chartHeight + 1) * (gVariable.totalCurveNum[gVariable.boardIndexSelected] + 2) / 3 - 1;
            this.panel2.Size = new System.Drawing.Size(panel1X, panel1Y);
        }


        private void multiCurve_Load(object sender, EventArgs e)
        {
            try
            {
                initmultiCurveScreen();

                aTimer = new System.Windows.Forms.Timer();

                //refresh screen every 100 ms
                aTimer.Interval = 1000;
                aTimer.Enabled = true;

                aTimer.Tick += new EventHandler(timer_listview);

                movingCurveFunc();

                this.Text = gVariable.programTitle + "生产状况监测";

                label41.Text = gVariable.currentCurveDatabaseName + " 实时数据：";

                closeReason = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("multiCurve_Load() in multiCurve function !" + ex);
            }
        }

        private void initmultiCurveScreen()
        {
            int i;

            Chart[] chartArray = { chart1,  chart2,  chart3,  chart4,  chart5,  chart6,  chart7,  chart8,  chart9,  chart10, 
                                   chart11, chart12, chart13, chart14, chart15, chart16, chart17, chart18, chart19, chart20, 
                                   chart21, chart22, chart23, chart24, chart25, chart26, chart27, chart28, chart29, chart30, 
                                   chart31, chart32, chart33, chart34, chart35, chart36
                                 };

            TextBox[] textBoxArray = { textBox1,  textBox2,  textBox3,  textBox4,  textBox5,  textBox6,  textBox7,  textBox8,  textBox9,  textBox10, 
                                       textBox11, textBox12, textBox13, textBox14, textBox15, textBox16, textBox17, textBox18, textBox19, textBox20, 
                                       textBox21, textBox22, textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30, 
                                       textBox31, textBox32, textBox33, textBox34, textBox35, textBox36
                                     };

            Label[] labelArray = { label1,  label2,  label3,  label4,  label5,  label6,  label7,  label8,  label9,  label10, 
                                   label11, label12, label13, label14, label15, label16, label17, label18, label19, label20,
                                   label21, label22, label23, label24, label25, label26, label27, label28, label29, label30,
                                   label31, label32, label33, label34, label35, label36
                                 };

//            cycleCounts = 0;
//            cycleCounts_old = 0;

            //display chart, textbox and textBox if this port is enabled
            for (i = 0; i < gVariable.totalCurveNum[gVariable.boardIndexSelected]; i++)
            {
//                gVariable.curveTextArray[i] = null;

                labelArray[i].Text = gVariable.curveTitle[i];
                labelArray[i].Show();

                textBoxArray[i].Text = gVariable.curveTextArray[i];
                textBoxArray[i].Show();
                textBoxArray[i].Enabled = true;

                initEveryChart(i, chartArray[i]);
                chartArray[i].Show();
            }

            //gVariable.maxCurveNum = 36
            for (; i < gVariable.maxCurveNum; i++)
            {
                labelArray[i].Hide();
                textBoxArray[i].Hide();
                chartArray[i].Hide();
            }
        }
        private void initEveryChart(int index, Chart chart)
        {
            chart.Titles.Clear();
            chart.Titles.Add(gVariable.curveTitle[index]);

//            foreach (var series in chart.Series)
//                series.Points.Clear();

            chart.Series[0].ChartType = SeriesChartType.Spline; //Line is streight line, SpLine is curve
            chart.Series[0].MarkerStyle = MarkerStyle.Diamond; //shape of the data point
            chart.Series[0].Color = Color.Green;
            chart.Series[0].BorderWidth = 1;
            chart.Series[0].ShadowOffset = 1;
            chart.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
            chart.Series[0].IsValueShownAsLabel = false;  //
            chart.Series[0].MarkerSize = 4; // size of the data point
            chart.Series[0].XValueType = ChartValueType.String;
            chart.Series[0].YValueType = ChartValueType.Double;
            //            chart.Series[0].IsShowTitle = true;

            chart.Series[1].ChartType = SeriesChartType.Line; //Line is streight line, SpLine is curve
            chart.Series[1].MarkerStyle = MarkerStyle.None; //shape of the data point
            chart.Series[1].Color = Color.Blue;
            chart.Series[1].BorderWidth = 1;
            chart.Series[1].ShadowOffset = 1;
            chart.Series[1].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
            chart.Series[1].IsValueShownAsLabel = false; //
            chart.Series[1].MarkerSize = 1; // size of the data point

            chart.BackColor = System.Drawing.Color.Azure;
            chart.BackGradientStyle = GradientStyle.DiagonalLeft;
            chart.BackSecondaryColor = System.Drawing.Color.SkyBlue;
            chart.Visible = true;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "N2";
            chart.ChartAreas[0].AxisY.IsStartedFromZero = false;  //whether we need to start at 0 for Y axis
            //chart.ChartAreas[0].AxisX.IsStartedFromZero = true;  //whether we need to start at 0 for X axis

            chart.ChartAreas[0].AlignmentOrientation = ((AreaAlignmentOrientations)((AreaAlignmentOrientations.Vertical | AreaAlignmentOrientations.Horizontal)));
            chart.ChartAreas[0].AxisX.Interval = 1;  //draw a vertical line every "Interval" point
//            chart.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd\nHH:mm:ss";
            chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart.ChartAreas[0].AxisX.LabelAutoFitStyle = ((LabelAutoFitStyles)((LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap)));
            chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
            chart.ChartAreas[0].AxisX.LabelStyle.Interval = gVariable.intervalForX;  //write a label string every "Interval" point
//            chart.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Seconds;
            chart.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0;
            chart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.Blue;
            chart.ChartAreas[0].AxisX.LineWidth = 2;
            chart.ChartAreas[0].AxisX.Title = "    ";
            chart.ChartAreas[0].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
            //chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Blue;
            chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.NotSet;
            chart.ChartAreas[0].AxisX.Minimum = 1;
            chart.ChartAreas[0].AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Years;
            chart.ChartAreas[0].AxisX.ScaleView.Size = gVariable.numOfRecordsInChart;
            chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisX.ScaleView.SmallScrollSizeType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;

            chart.ChartAreas[0].AxisY.StripLines.Clear();

            if (gVariable.curveUpperLimit[index] != 0 || gVariable.curveLowerLimit[index] != 0)  //for beat data, we may have no limit value
            {
                StripLine sprUsl = new StripLine();
                sprUsl.IntervalOffset = gVariable.curveUpperLimit[index];
                sprUsl.BorderColor = Color.Blue;
                sprUsl.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprUsl);

                StripLine sprlsl = new StripLine();
                sprlsl.IntervalOffset = gVariable.curveLowerLimit[index];
                sprlsl.BorderColor = Color.Blue;
                sprlsl.BorderDashStyle = ChartDashStyle.Dash;
                sprlsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprlsl);
            }
        }

        private void multiCurve_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (aTimer != null)
                aTimer.Enabled = false;

            if (closeReason == 0)
            {
                dispatchUI.dispatchUIClass.Show();
            }
        }


        //This function will be called every second
        private void timer_listview(Object source, EventArgs e)
        {
            int i;
            TextBox[] textBoxArray = { textBox1,  textBox2,  textBox3,  textBox4,  textBox5,  textBox6,  textBox7,  textBox8,  textBox9,  textBox10, 
                                       textBox11, textBox12, textBox13, textBox14, textBox15, textBox16, textBox17, textBox18, textBox19, textBox20, 
                                       textBox21, textBox22, textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30, 
                                       textBox31, textBox32, textBox33, textBox34, textBox35, textBox36
                                     };

            //if it takes a long time to draw these charts, we wait for the end of chart drawing, then start waiting for 1 second
            aTimer.Stop();

            movingCurveFunc();

            for (i = 0; i < gVariable.totalCurveNum[gVariable.boardIndexSelected]; i++)
            {
                textBoxArray[i].Text = gVariable.curveTextArray[i];
            }
            aTimer.Start();
        }

        public void resumemultiCurve()
        {
            aTimer.Start();
        }


        public void disableTimer()
        {
            aTimer.Enabled = false;
        }

        public void movingCurveFunc()
        {
            int i;
            Chart[] chartArray = { chart1,  chart2,  chart3,  chart4,  chart5,  chart6,  chart7,  chart8,  chart9,  chart10, 
                                   chart11, chart12, chart13, chart14, chart15, chart16, chart17, chart18, chart19, chart20, 
                                   chart21, chart22, chart23, chart24, chart25, chart26, chart27, chart28, chart29, chart30, 
                                   chart31, chart32, chart33, chart34, chart35, chart36
                                 };

            try
            {
                if (gVariable.refreshMultiCurve == 1)  //dispatch status changed, maybe started or completed, we need to change diaplay by dispatch sheet or to default display status
                {
                    resizeForScreen();
                    initmultiCurveScreen();
                    gVariable.refreshMultiCurve = 0; 
                }

                //we will read current board data only when database name is available, otherwise stop reading to avoid read error since we will set database name to null when entering a new room
                if (gVariable.currentCurveDatabaseName != null)
                {
                    //in curve display screen, we will read 125 data points for one port at one time, for chart display and cpk, ppk, pp display
                    mySQLClass.readSmallPartOfDataToArray(gVariable.currentCurveDatabaseName, gVariable.totalPointNumForNoSPCChart);

                    for (i = 0; i < gVariable.totalCurveNum[gVariable.boardIndexSelected]; i++)
                        readPointDataToChart(i, chartArray[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void dealWithOneCurveChart(int index)
        {
            OneCurve oneCurveClass = new OneCurve(index);

            //first 100 means display with minimum number of data point, second 100 means data at the end of data list 
            oneCurveClass.OneCurveChart(index, 1000, 1000); 
            oneCurveClass.Show();
//            Hide();
        }

        public static void readPointDataToChart(int index, Chart chart)
        {
            int i, j, start, num;
            int flag;
            int interval;
            int startRecordIndex;
            int currentdataNumForCurve;
            string xString;
            float delta, UCL, LCL, tmpMax, tmpMin;
            DateTime dateTime;

            try
            {
                chart.Series[0].Color = Color.Green;

                i = 0;
                UCL = gVariable.curveUpperLimit[index];
                LCL = gVariable.curveLowerLimit[index];

                currentdataNumForCurve = gVariable.dataNumForCurve[index];

                foreach (var series in chart.Series)
                {
                    series.Points.Clear();
                }

                //startRecordIndex is used as index displayed in x axis
                if (gVariable.dataNumForCurve[index] > gVariable.totalPointNumForNoSPCChart)
                {
                    //We will read totalPointNumForNoSPCChart(125) pieces of data for SPC every time, the last numOfRecordsInChart(25) of data
                    // will be used for curve display, so here we only get the last numOfRecordsInChart(25) of data
                    startRecordIndex = gVariable.dataNumForCurve[index] - gVariable.totalPointNumForNoSPCChart;  //start index in database
                    start = gVariable.totalPointNumForNoSPCChart - gVariable.numOfRecordsInChart;  //start index in dataInPint array
                    num = gVariable.numOfRecordsInChart + start;  //end point in curve
                }
                else if (gVariable.dataNumForCurve[index] > gVariable.numOfRecordsInChart)
                {
                    startRecordIndex = 0;
                    start = gVariable.dataNumForCurve[index] - gVariable.numOfRecordsInChart;
                    num = gVariable.numOfRecordsInChart + start; //end point in curve
                }
                else //if (gVariable.dataNumForCurve[index] < numOfRecordsInChart)
                {
                    startRecordIndex = 0;
                    start = 0;
                    num = gVariable.dataNumForCurve[index];
                }

                if (num == 0)
                {
                    xString = "1\n" + DateTime.Now.ToString("MM-dd HH:mm:ss");

                    chart.Series[0].Points.AddXY(xString, 0);
                    gVariable.minDataValue[index] = 0;
                    gVariable.maxDataValue[index] = 0;
                }
                else
                {
                    for (i = start; i < num; i++)
                    {
                        dateTime = toolClass.GetTime((gVariable.timeInPoint[index, i] - 3600 * 7).ToString());

                        j = i + startRecordIndex;
                        xString = j + "\n" + dateTime.ToString("MM-dd HH:mm:ss");

                        chart.Series[0].Points.AddXY(xString, gVariable.dataInPoint[index, i]);

                        if (i == start)
                        {
                            gVariable.minDataValue[index] = gVariable.dataInPoint[index, 0];
                            gVariable.maxDataValue[index] = gVariable.dataInPoint[index, 0];
                        }
                        else
                        {
                            if (gVariable.dataInPoint[index, i] > gVariable.maxDataValue[index])
                                gVariable.maxDataValue[index] = gVariable.dataInPoint[index, i];
                            if (gVariable.dataInPoint[index, i] < gVariable.minDataValue[index])
                                gVariable.minDataValue[index] = gVariable.dataInPoint[index, i];
                        }
                    }
                }

                if (UCL == 0 && LCL == 0)
                {
                    tmpMax = 1;
                    tmpMin = 0;
                }
                else
                {
                    tmpMax = UCL;
                    tmpMin = LCL;
                }

                if (gVariable.maxDataValue[index] > UCL)
                    tmpMax = gVariable.maxDataValue[index];

                if (gVariable.minDataValue[index] < LCL)
                    tmpMin = gVariable.minDataValue[index];

                delta = tmpMax - tmpMin;
                tmpMax += delta / 7;

//                if (tmpMin >= 0)
//                    tmpMin = 0;
//                else
                    tmpMin -= delta / 7;

                if (tmpMax == tmpMin)
                    tmpMax += 1;

                chart.ChartAreas[0].AxisY.Maximum = tmpMax;
                chart.ChartAreas[0].AxisY.Minimum = tmpMin;

                chart.Annotations.Clear();

                flag = 0;
                interval = 5;
                i = 0;
                foreach (DataPoint point in chart.Series[0].Points)    //遍历数据点
                {
                    float tmp = (float)point.YValues[0];

                    if (flag == 1 || tmp > UCL || tmp < LCL)
                    {
                        point.Color = Color.Red;
                        if (flag == 0)
                            flag = 1;
                        else
                            flag = 0;
                    }
                    else
                    {
                        point.Color = Color.Green;
                    }

                    if (i == interval)
                    {
                        var annotation = new TextAnnotation { Text = point.YValues[0].ToString("F3") };
                        annotation.SetAnchor(point);
                        annotation.AnchorY = point.YValues[0];

                        chart.Annotations.Add(annotation);
                        i = 0;
                    }
                    i++;
                }

                if (UCL != 0 || LCL != 0)  //for beat data, since there is no limit value, no SPC
                {
                    if (gVariable.dataNumForCurve[index] > gVariable.totalPointNumForNoSPCChart)  //范本太少则不作SPC计算
                    {
                        getSPCValues(index, UCL, LCL);
                        chart.ChartAreas[0].AxisX.Title = "Cp:" + cp[index].ToString("f3") + "   Cpk:" + cpk[index].ToString("f3") + "   Ppk:" + ppk[index].ToString("f3");
                    }
                    else
                        chart.ChartAreas[0].AxisX.Title = "Cp:          Cpk:          Ppk:          ";
                }
            }

            catch (Exception ex)
            {
                Console.Write("draw curve error :" + ex);
            }
        }

        public static void getSPCValues(int curveIndex, float UCL, float LCL)
        {
            SPCFunctions SPCClass = new SPCFunctions();

            SPCClass.CalculateSPC(UCL, LCL, curveIndex, gVariable.dataInPoint, gVariable.CHART_TYPE_NO_SPC);

            cp[curveIndex] = SPCClass.getSPCCp();
            cpk[curveIndex] = SPCClass.getSPCCpk();
            ppk[curveIndex] = SPCClass.getSPCPpk();
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(0);
        }

        private void chart2_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(1);
        }

        private void chart3_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(2);
        }

        private void chart4_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(3);
        }

        private void chart5_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(4);
        }

        private void chart6_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(5);
        }

        private void chart7_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(6);
        }

        private void chart8_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(7);
        }

        private void chart9_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(8);
        }

        private void chart10_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(9);
        }

        private void chart11_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(10);
        }

        private void chart12_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(11);
        }

        private void chart15_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(14);
        }

        private void chart14_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(13);
        }

        private void chart13_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(12);
        }

        private void chart16_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(15);
        }

        private void chart30_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(29);
        }

        private void chart20_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(19);
        }

        private void chart18_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(17);
        }

        private void chart36_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(35);
        }

        private void chart34_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(33);
        }

        private void chart17_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(16);
        }

        private void chart28_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(27);
        }

        private void chart32_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(31);
        }

        private void chart31_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(30);
        }

        private void chart27_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(26);
        }

        private void chart33_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(32);
        }

        private void chart35_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(34);
        }

        private void chart24_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(23);
        }

        private void chart22_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(21);
        }

        private void chart21_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(20);
        }

        private void chart23_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(22);
        }

        private void chart26_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(25);
        }

        private void chart25_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(24);
        }

        private void chart19_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(18);
        }

        private void chart29_Click(object sender, EventArgs e)
        {
            dealWithOneCurveChart(28);
        }

    }
}
