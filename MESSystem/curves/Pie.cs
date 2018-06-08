using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.DataVisualization.Charting.Utilities;
using MESSystem.common;
using MESSystem.mainUI;

namespace MESSystem.curves
{
    public partial class Pie : Form
    {
        int closeReason;
        public static Pie pieClass = null; //用来引用主窗口

        System.Windows.Forms.Timer aTimer;
        public Pie()
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

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        private void initEveryChart(int index, Chart chart)
        {
            try
            {
                int i;
                int pointIndex;
                DataPoint dataPoint;

                chart.Series[0].BorderWidth = 1;
                chart.Series[0].ShadowOffset = 1;
                chart.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                chart.Series[0].IsValueShownAsLabel = true;  //
                chart.Series[0].MarkerSize = 4; // size of the data point
                chart.Series[0].EmptyPointStyle.CustomProperties = "Exploded=True";
                chart.Series[0].EmptyPointStyle.IsValueShownAsLabel = true;
                chart.Series[0]["PieLabelStyle"] = "Outside";
                chart.Series[0]["PieLineColor"] = "Black";//绘制黑色的连线
                chart.Series[0].IsVisibleInLegend = true; //whether we need a description of the data in curve

                chart.ChartAreas[0].Area3DStyle.Enable3D = true;
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "N4";  //小数点后取四位
//                chart.ChartAreas[0].AxisY.LabelStyle.Format = "N2";  //小数点后取四位
//                chart.ChartAreas[0].AxisY.IsStartedFromZero = false;  //whether we need to start at 0 for Y axis
                chart.BackColor = System.Drawing.Color.Azure;
                chart.BackGradientStyle = GradientStyle.DiagonalLeft;
                chart.BackSecondaryColor = System.Drawing.Color.SkyBlue;

                chart.BackColor = System.Drawing.Color.Azure;
                chart.BackGradientStyle = GradientStyle.DiagonalLeft;
                chart.BackSecondaryColor = System.Drawing.Color.SkyBlue;
                chart.Visible = true;
                chart.Titles.Add(gVariable.curveTitle[index] + "   (采样数量：" + gVariable.dataNumForCurve[index] + " 个)");

                for (i = 0; i < 5; i++)
                { 
                    pointIndex = chart.Series[0].Points.AddXY(gVariable.columnLimits[index, i], gVariable.columnData[index, i]);
                    dataPoint = chart.Series[0].Points[pointIndex];  
                    dataPoint.LegendText = gVariable.columnLimits[index, i].ToString("f4");  //four digits
                    dataPoint.Label = "#PERCENT{P2}";
                }

//                chart.Legends[0].Alignment = StringAlignment.Center;
//                chart.Legends[0].HeaderSeparator = LegendSeparatorStyle.ThickLine;  

//                chart.ChartAreas[0].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
//                chart.ChartAreas[0].AxisX.Title = "Cp: " + gVariable.cp[index].ToString("f4") + "     Cpk: " + gVariable.cpk[index].ToString("f4") + "     Ppk: " + gVariable.ppk[index].ToString("f4");
 
                chart.Invalidate();
            }
            catch (Exception e)
            {
                Console.Write("draw pie error :" + e);
            }
        }

        private void Pie_Load(object sender, EventArgs e)
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
                //display chart, textbox and textBox if this port is enabled
                for (i = 0; i < gVariable.totalCurveNum[gVariable.boardIndexSelected]; i++)
                {
//                    gVariable.curveTextArray[i] = null;

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

                aTimer = new System.Windows.Forms.Timer();

                //refresh screen every 100 ms
                aTimer.Interval = 1000;
                aTimer.Enabled = true;

                aTimer.Tick += new EventHandler(timer_listview);

                this.Text = gVariable.programTitle + "生产状况监测之饼图";

                label41.Text = gVariable.currentCurveDatabaseName + " 实时数据：";

                closeReason = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("column_Load() in column function !" + ex);
            }
        }

        private void timer_listview(Object source, EventArgs e)
        {
            int i;
            TextBox[] textBoxArray = { textBox1,  textBox2,  textBox3,  textBox4,  textBox5,  textBox6,  textBox7,  textBox8,  textBox9,  textBox10, 
                                       textBox11, textBox12, textBox13, textBox14, textBox15, textBox16, textBox17, textBox18, textBox19, textBox20, 
                                       textBox21, textBox22, textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30, 
                                       textBox31, textBox32, textBox33, textBox34, textBox35, textBox36
                                     };

//            //in curve display screen, we will read 125 data points for one port at one time, for chart display and cpk, ppk, pp display
//            mySQLClass.readSmallPartOfDataToArray(gVariable.currentCurveDatabaseName, gVariable.totalPointNumForSChart, gVariable.ALL_DATA_IN_DATABASE);

            for (i = 0; i < gVariable.totalCurveNum[gVariable.boardIndexSelected]; i++)
            {
                textBoxArray[i].Text = gVariable.curveTextArray[i];
            }

        }

        private void Pie_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (aTimer != null)
                aTimer.Enabled = false;

            if (closeReason == 0)
            {
                dispatchUI.dispatchUIClass.Show();
            }
        }
    }
}
