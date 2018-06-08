using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using MESSystem.common;
using MESSystem.quality;
using MESSystem.mainUI;

namespace MESSystem.curves
{
    public partial class SPCCurve : Form
    {
        public static SPCCurve SPCCurveClass = null; //用来引用主窗口
        static SPCFunctions SPCClass;

        System.Windows.Forms.Timer aTimer;

        int closeReason;

        string[] SPCChartNameArray = { "XBar-R 图", "XBar-S 图", "XMed-R 图", "X-Rm 图", "P 图", "Pn 图", "R 图", "C 图" };
        string[] SPCChartName1 = { "XBar", "XBar", "XMed", "X", "P", "Pn", "R", "C" };
        string[] SPCChartName2 = { "R", "S", "R", "Rm", "", "", "", "" };
        public SPCCurve()
        {
            InitializeComponent();
            InitializeComponent2();
            resizeForScreen();
        }

        private void InitializeComponent2()
        {
            int i;

            ChartArea[] chartAreaArray = new ChartArea[40];
            Series[] seriesArray = new Series[80];
            Chart[] chartArray = 
            { 
                chart1,  chart2,  chart3,  chart4,  chart5,  chart6,  chart7,  chart8,  chart9,  chart10, 
                chart11, chart12, chart13, chart14, chart15, chart16, chart17, chart18, chart19, chart20, 
                chart21, chart22, chart23, chart24, chart25, chart26, chart27, chart28, chart29, chart30, 
                chart31, chart32, chart33, chart34, chart35, chart36,
            };

            for (i = 0; i < gVariable.maxCurveNum; i++)
            {
                chartAreaArray[i] = new ChartArea();
                seriesArray[i * 2] = new Series();
                seriesArray[i * 2 + 1] = new Series();

                chartAreaArray[i].Name = "ChartArea2";
                seriesArray[i * 2].ChartArea = "ChartArea1";
                seriesArray[i * 2].Legend = "Legend1";
                seriesArray[i * 2].Name = SPCChartName1[gVariable.SPCChartIndex];
                seriesArray[i * 2 + 1].ChartArea = "ChartArea2";
                seriesArray[i * 2 + 1].Legend = "Legend1";
                seriesArray[i * 2 + 1].Name = SPCChartName2[gVariable.SPCChartIndex];
                chartArray[i].Series.Add(seriesArray[i * 2]);
                if (gVariable.SPCChartIndex <= gVariable.CHART_TYPE_SPC_X_RM)
                    chartArray[i].Series.Add(seriesArray[i * 2 + 1]);
                chartArray[i].ChartAreas.Add(chartAreaArray[i]);
            }

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
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
            this.panel2.Location = new System.Drawing.Point(panel1X, 0);

            panel1X = chartWidth * 3 + m * 2;
            panel1Y = (chartHeight + 1) * (gVariable.totalCurveNum[gVariable.boardIndexSelected] + 2) / 3 - 1;
            this.panel2.Size = new System.Drawing.Size(panel1X, panel1Y);
        }


        private void Form3_Load(object sender, EventArgs e)
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

            try
            {
                //we need to first call this to get SPCClass
                SPCClass = new SPCFunctions();

                aTimer = new System.Windows.Forms.Timer();

                //refresh screen every 100 ms
                aTimer.Interval = 1000;
                aTimer.Enabled = true;

            	aTimer.Tick += new EventHandler(timer_listview);

            	drawCurveFunc();

                //display chart, textbox and label if this port is enabled
                for (i = 0; i < gVariable.totalCurveNum[gVariable.boardIndexSelected]; i++)
                {
                    labelArray[i].Text = gVariable.curveTitle[i]; //.Remove(gVariable.curveTitle[i].IndexOf('('));
                    labelArray[i].Show();

                    textBoxArray[i].Text = gVariable.curveTextArray[i];
                    textBoxArray[i].Show();
                    textBoxArray[i].Enabled = true;

                    initEveryChart(i, chartArray[i], gVariable.curveTitle[i] + "  ——  " + SPCChartNameArray[gVariable.SPCChartIndex]);
                    chartArray[i].Show();
                }

                //we have altogether 36 charts, index equal or larger than gVariable.totalCurveNum does not used so will be disabled
                for (; i < gVariable.maxCurveNum; i++)
                {
                    labelArray[i].Hide();
                    textBoxArray[i].Hide();
                    chartArray[i].Hide();
                }
                this.Text = gVariable.programTitle + "SPC状况监测";
 
                closeReason = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Form3_Load() in SPCCurve function !" + ex);
            }
        }


        public void initEveryChart(int index, Chart chart, string strTitle)
        {
            try
            {
                chart.Titles.Clear();
                chart.Titles.Add(strTitle);
                chart.Series[0].ChartType = SeriesChartType.Line; //Line is streight line, SpLine is curve
                chart.Series[0].MarkerStyle = MarkerStyle.Diamond; //shape of the data point
                chart.Series[0].Color = Color.Green;
                chart.Series[0].BorderWidth = 1;
                chart.Series[0].ShadowOffset = 1;
                chart.Series[0].IsVisibleInLegend = true; //whether we need a description of the data in curve
                chart.Series[0].IsValueShownAsLabel = false;  //
                chart.Series[0].MarkerSize = 2; // size of the data point
                //            chart.Series[0].XValueType = ChartValueType.String;
                chart.Series[0].YValueType = ChartValueType.Double;
                //            chart.Series[0].IsShowTitle = true;

                chart.Series[1].ChartType = SeriesChartType.Line; //Line is streight line, SpLine is curve
                chart.Series[1].MarkerStyle = MarkerStyle.Diamond; //shape of the data point
                chart.Series[1].Color = Color.Blue;
                chart.Series[1].BorderWidth = 1;
                chart.Series[1].ShadowOffset = 1;
                chart.Series[1].IsVisibleInLegend = true; //whether we need a description of the data in curve
                chart.Series[1].IsValueShownAsLabel = false; //
                chart.Series[1].MarkerSize = 2; // size of the data point
                chart.Series[1].YValueType = ChartValueType.Double;

                chart.BackColor = System.Drawing.Color.Azure;
                chart.BackGradientStyle = GradientStyle.DiagonalLeft;
                chart.BackSecondaryColor = System.Drawing.Color.SkyBlue;
                chart.Visible = true;

                chart.ChartAreas[0].Position.Auto = true;
                //            chart.ChartAreas[0].AxisY2.CustomLabels
                chart.ChartAreas[0].AxisY.LabelStyle.Format = "N4";
                chart.ChartAreas[0].AxisY.IsStartedFromZero = false;  //whether we need to start at 0 for Y axis
                //chart.ChartAreas[0].AxisX.IsStartedFromZero = true;  //whether we need to start at 0 for X axis

                chart.ChartAreas[0].AlignmentOrientation = ((AreaAlignmentOrientations)((AreaAlignmentOrientations.Vertical | AreaAlignmentOrientations.Horizontal)));
                chart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
                chart.ChartAreas[0].AxisX.Interval = 1;  //draw a vertical line every "Interval" point
                //            chart.ChartAreas[0].AxisX.LabelStyle.Format = "yy-MM-dd\nHH:mm:ss";//时间格式。
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                chart.ChartAreas[0].AxisX.LabelAutoFitStyle = ((LabelAutoFitStyles)((LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap)));
                chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = gVariable.intervalForX;  //write a label string every "Interval" point
                //            chart.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Seconds;
                chart.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0;
                chart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.LineWidth = 1;
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

                chart.ChartAreas[1].Position.Auto = true;
                chart.ChartAreas[1].AxisY.LabelStyle.Format = "N4";
                chart.ChartAreas[1].AxisY.IsStartedFromZero = false;  //whether we need to start at 0 for Y axis
                //chart.ChartAreas[0].AxisX.IsStartedFromZero = true;  //whether we need to start at 0 for X axis

                chart.ChartAreas[1].AlignmentOrientation = ((AreaAlignmentOrientations)((AreaAlignmentOrientations.Vertical | AreaAlignmentOrientations.Horizontal)));
                chart.ChartAreas[1].AxisX.LabelStyle.Enabled = false;
                chart.ChartAreas[1].AxisX.Interval = 1;  //draw a vertical line every "Interval" point
                //            chart.ChartAreas[0].AxisX.LabelStyle.Format = "yy-MM-dd\nHH:mm:ss";//时间格式。
                chart.ChartAreas[1].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                chart.ChartAreas[1].AxisX.LabelAutoFitStyle = ((LabelAutoFitStyles)((LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap)));
                chart.ChartAreas[1].AxisX.LabelStyle.IsStaggered = false;
                chart.ChartAreas[1].AxisX.LabelStyle.Interval = gVariable.intervalForX;  //write a label string every "Interval" point
                //            chart.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Seconds;
                chart.ChartAreas[1].AxisX.LabelStyle.IntervalOffset = 0;
                chart.ChartAreas[1].AxisX.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[1].AxisX.LineWidth = 1;
                chart.ChartAreas[1].AxisX.Title = "    ";
                chart.ChartAreas[1].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
                //chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[1].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chart.ChartAreas[1].AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.NotSet;
                chart.ChartAreas[1].AxisX.Minimum = 1;
                chart.ChartAreas[1].AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Years;
                chart.ChartAreas[1].AxisX.ScaleView.Size = gVariable.numOfRecordsInChart;
                chart.ChartAreas[1].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
                chart.ChartAreas[1].AxisX.ScaleView.SmallScrollSizeType = DateTimeIntervalType.Number;
                chart.ChartAreas[1].AxisX.ScaleView.Zoomable = false;

                StripLine sprUsl = new StripLine();
                sprUsl.IntervalOffset = gVariable.SPCControlValue[index, 0];
                sprUsl.BorderColor = Color.Red;
                sprUsl.Text = sprUsl.IntervalOffset.ToString("N4");
                //            sprUsl.TextAlignment = StringAlignment.Far;
                sprUsl.TextLineAlignment = StringAlignment.Far;
                sprUsl.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprUsl);

                StripLine sprSl = new StripLine();
                sprSl.IntervalOffset = gVariable.SPCControlValue[index, 1];
                sprSl.BorderColor = Color.Red;
                sprSl.Text = sprSl.IntervalOffset.ToString("N4");
                sprSl.TextLineAlignment = StringAlignment.Center;
                sprSl.BorderDashStyle = ChartDashStyle.Dash;
                sprSl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprSl);

                StripLine sprLsl = new StripLine();
                sprLsl.IntervalOffset = gVariable.SPCControlValue[index, 2];
                sprLsl.BorderColor = Color.Red;
                sprLsl.Text = sprLsl.IntervalOffset.ToString("N4");
                sprLsl.TextAlignment = StringAlignment.Far;
                sprLsl.BorderDashStyle = ChartDashStyle.Dash;
                sprLsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprLsl);

                StripLine sprUsl1 = new StripLine();
                sprUsl1.IntervalOffset = gVariable.SPCControlValue[index, 3];
                sprUsl1.BorderColor = Color.Red;
                sprUsl1.Text = sprUsl1.IntervalOffset.ToString("N4");
                sprUsl1.TextLineAlignment = StringAlignment.Far;
                sprUsl1.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl1.BorderWidth = 1;
                chart.ChartAreas[1].AxisY.StripLines.Add(sprUsl1);

                StripLine sprSl1 = new StripLine();
                sprSl1.IntervalOffset = gVariable.SPCControlValue[index, 4];
                sprSl1.BorderColor = Color.Red;
                sprSl1.Text = sprSl1.IntervalOffset.ToString("N4");
                sprSl1.TextLineAlignment = StringAlignment.Center;
                sprSl1.BorderDashStyle = ChartDashStyle.Dash;
                sprSl1.BorderWidth = 1;
                chart.ChartAreas[1].AxisY.StripLines.Add(sprSl1);

                StripLine sprLsl1 = new StripLine();
                sprLsl1.IntervalOffset = gVariable.SPCControlValue[index, 5];
                sprLsl1.BorderColor = Color.Red;
                sprLsl1.Text = sprLsl1.IntervalOffset.ToString("N4");
                sprLsl1.TextAlignment = StringAlignment.Far;
                sprLsl1.BorderDashStyle = ChartDashStyle.Dash;
                sprLsl1.BorderWidth = 1;
                chart.ChartAreas[1].AxisY.StripLines.Add(sprLsl1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("initEveryChart() in SPCCurve function !" + ex);
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
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

            for (i = 0; i < gVariable.totalCurveNum[gVariable.boardIndexSelected]; i++)
            {
                textBoxArray[i].Text = gVariable.curveTextArray[i];
            }

            aTimer.Start();
        }

        public void drawCurveFunc()
        {
            int i;
            Chart[] chartArray = { chart1,  chart2,  chart3,  chart4,  chart5,  chart6,  chart7,  chart8,  chart9,  chart10, 
                                   chart11, chart12, chart13, chart14, chart15, chart16, chart17, chart18, chart19, chart20, 
                                   chart21, chart22, chart23, chart24, chart25, chart26, chart27, chart28, chart29, chart30, 
                                   chart31, chart32, chart33, chart34, chart35, chart36 
                                 };

            try
            {
                //in SPCCurve, we need at most 9 * 25 = 225 of data points for one port, to draw XBar-R, XBar-S, X-Rs, Xmed-R, P, Pn, C chart, ect. 
                mySQLClass.readSmallPartOfDataToArray(gVariable.currentCurveDatabaseName, gVariable.totalPointNumForSChart);

                for (i = 0; i < gVariable.totalCurveNum[gVariable.boardIndexSelected]; i++)
                {
                    if (gVariable.dataNumForCurve[i] < gVariable.totalPointNumForSChart)
                        continue;  //not enough data point to draw SPC curve, check next chart

                    switch (gVariable.SPCChartIndex)
                    {
                        case gVariable.CHART_TYPE_SPC_XBAR_R:
                            SPCClass.get_XBar_R_Chart(i, chartArray[i], gVariable.SPC_DATA_AND_CHART);
                            break;
                        case gVariable.CHART_TYPE_SPC_XBAR_S:
                            SPCClass.get_XBar_S_Chart(i, chartArray[i], gVariable.SPC_DATA_AND_CHART, gVariable.dataInPoint, gVariable.timeInPoint, gVariable.SPCControlValue);
                            break;
                        case gVariable.CHART_TYPE_SPC_XMED_R:
                            SPCClass.get_XMed_R_Chart(i, chartArray[i]);
                            break;
                        case gVariable.CHART_TYPE_SPC_X_RM:
                            SPCClass.get_X_Rm_Chart(i, chartArray[i]);
                            break;
                        case gVariable.CHART_TYPE_SPC_P:
                            //                        SPCClass.get_P_Chart(i, chartArray[i]);
                            break;
                        case gVariable.CHART_TYPE_SPC_PN:
                            //                        SPCClass.get_Pn_Chart(i, chartArray[i]);
                            break;
                        case gVariable.CHART_TYPE_SPC_R:
                            //                        SPCClass.get_R_Chart(i, chartArray[i]);
                            break;
                        case gVariable.CHART_TYPE_SPC_C:
                            //                        SPCClass.get_C_Chart(i, chartArray[i]);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("movingCurveFunc() in SPCCurve !" + ex);
            }
        }
    }
}