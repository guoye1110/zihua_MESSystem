using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MESSystem.common;

namespace MESSystem.quality
{
    public partial class SPCAnalyze : Form
    {
        const int labelNumInCPKChart = 24;
        const int verticalLineEveryNumOfPoint = 14;

        int totalDataNumInBuffer;
        int oneCurveScreenSize;

        int vScrollBar1, vScrollBar2;

        string[] SPCTitleArray = { "XBar-S 控制图", "CPK 分析", "数据折线图", "均值折线图", "工单数据全景图", "XBar-S 控制图", "C 图", };
        string [] SPCChartName = {"XBar 图", "S 图"};
        string [] CPKLabelTextArray = 
        {
            "样本总数", "子组容量", "组数", "规格上限", "规格下限", "最大值", "最小值", "",
            "平均值", "极差", "组间标准差", "整体标准差", "XBar控制上限", "XBar控制下限", "S图控制上限", "S图控制下限", 
            "Cp", "Cpk", "Ca", "Pp", "Ppk", "Ppu", "Ppl", "不良预估PPM"  
        };


        private void XBar_S_chart_Init(Chart chart)
        {
            Series[] series = new Series[2];

            try
            {
                //set 2 series
                series[0] = new Series();
                series[1] = new Series();

                //the first series go with chartarea1
                series[0].ChartArea = "ChartArea1";
                series[0].Legend = "Legend1";
                series[0].Name = SPCChartName[0];
                //the second series go with chartarea2
                series[1].ChartArea = "ChartArea2";
                series[1].Legend = "Legend1";
                series[1].Name = SPCChartName[1];
                chart5.Series.Add(series[0]);
                chart5.Series.Add(series[1]);

                label25.Text = getSPCChartTitle(TAB_CONTROL_XBAR_S_DATA);

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
                chart.ChartAreas[0].AxisX.ScaleView.Size = gVariable.numOfGroupsInSChart;
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
                chart.ChartAreas[1].AxisX.ScaleView.Size = gVariable.numOfGroupsInSChart;
                chart.ChartAreas[1].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
                chart.ChartAreas[1].AxisX.ScaleView.SmallScrollSizeType = DateTimeIntervalType.Number;
                chart.ChartAreas[1].AxisX.ScaleView.Zoomable = false;

                StripLine sprUsl = new StripLine();
                sprUsl.IntervalOffset = controlCenterValueArray[indexInTable, 0];
                sprUsl.BorderColor = Color.Red;
                sprUsl.Text = sprUsl.IntervalOffset.ToString("N4");
                //            sprUsl.TextAlignment = StringAlignment.Far;
                sprUsl.TextLineAlignment = StringAlignment.Far;
                sprUsl.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprUsl);

                StripLine sprSl = new StripLine();
                sprSl.IntervalOffset = controlCenterValueArray[indexInTable, 1];
                sprSl.BorderColor = Color.Red;
                sprSl.Text = sprSl.IntervalOffset.ToString("N4");
                sprSl.TextLineAlignment = StringAlignment.Center;
                sprSl.BorderDashStyle = ChartDashStyle.Dash;
                sprSl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprSl);

                StripLine sprLsl = new StripLine();
                sprLsl.IntervalOffset = controlCenterValueArray[indexInTable, 2];
                sprLsl.BorderColor = Color.Red;
                sprLsl.Text = sprLsl.IntervalOffset.ToString("N4");
                sprLsl.TextAlignment = StringAlignment.Far;
                sprLsl.BorderDashStyle = ChartDashStyle.Dash;
                sprLsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprLsl);

                StripLine sprUsl1 = new StripLine();
                sprUsl1.IntervalOffset = controlCenterValueArray[indexInTable, 3];
                sprUsl1.BorderColor = Color.Red;
                sprUsl1.Text = sprUsl1.IntervalOffset.ToString("N4");
                sprUsl1.TextLineAlignment = StringAlignment.Far;
                sprUsl1.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl1.BorderWidth = 1;
                chart.ChartAreas[1].AxisY.StripLines.Add(sprUsl1);

                StripLine sprSl1 = new StripLine();
                sprSl1.IntervalOffset = controlCenterValueArray[indexInTable, 4];
                sprSl1.BorderColor = Color.Red;
                sprSl1.Text = sprSl1.IntervalOffset.ToString("N4");
                sprSl1.TextLineAlignment = StringAlignment.Center;
                sprSl1.BorderDashStyle = ChartDashStyle.Dash;
                sprSl1.BorderWidth = 1;
                chart.ChartAreas[1].AxisY.StripLines.Add(sprSl1);

                StripLine sprLsl1 = new StripLine();
                sprLsl1.IntervalOffset = controlCenterValueArray[indexInTable, 5];
                sprLsl1.BorderColor = Color.Red;
                sprLsl1.Text = sprLsl1.IntervalOffset.ToString("N4");
                sprLsl1.TextAlignment = StringAlignment.Far;
                sprLsl1.BorderDashStyle = ChartDashStyle.Dash;
                sprLsl1.BorderWidth = 1;
                chart.ChartAreas[1].AxisY.StripLines.Add(sprLsl1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("initChart() in SPC charts function !" + ex);
            }
        }


        private void C_chart_Init(Chart chart)
        {
            float usl, lsl;
            Series[] series = new Series[1];

            try
            {
                //set 2 series
                series[0] = new Series();

                //the first series go with chartarea1
                series[0].ChartArea = "ChartArea1";
                series[0].Legend = "Legend1";
                series[0].Name = SPCChartName[0];
                chart5.Series.Add(series[0]);

                label25.Text = getSPCChartTitle(TAB_CONTROL_C_CHART_DATA);

                chart.Series[0].ChartType = SeriesChartType.Spline; //Line is streight line, SpLine is curve
                chart.Series[0].MarkerStyle = MarkerStyle.Diamond; //shape of the data point
                chart.Series[0].Color = Color.Green;
                chart.Series[0].BorderWidth = 2;
                chart.Series[0].ShadowOffset = 1;
                chart.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                chart.Series[0].IsValueShownAsLabel = false;  //
                chart.Series[0].MarkerSize = 4; // size of the data point
                chart.Series[0].XValueType = ChartValueType.String;
                chart.Series[0].YValueType = ChartValueType.Double;

                chart.Visible = true;
                chart.ChartAreas[0].AxisY.LabelStyle.Format = "N2";
                chart.ChartAreas[0].AxisY.IsStartedFromZero = false;  //whether we need to start at 0 for Y axis
                //chart.ChartAreas[0].AxisX.IsStartedFromZero = true;  //whether we need to start at 0 for X axis

                chart.ChartAreas[0].AlignmentOrientation = ((AreaAlignmentOrientations)((AreaAlignmentOrientations.Vertical | AreaAlignmentOrientations.Horizontal)));
                chart.ChartAreas[0].AxisX.Interval = numOfPointNeedForChart / verticalLineEveryNumOfPoint;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                chart.ChartAreas[0].AxisX.LabelAutoFitStyle = ((LabelAutoFitStyles)((LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap)));
                chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = numOfPointNeedForChart / verticalLineEveryNumOfPoint;
                chart.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0;
                chart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.LineWidth = 2;
                chart.ChartAreas[0].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
                //            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chart.ChartAreas[0].AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.NotSet;
                chart.ChartAreas[0].AxisX.Minimum = 1;
                chart.ChartAreas[0].AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Number;
                chart.ChartAreas[0].AxisX.ScaleView.Size = numOfPointNeedForChart;
                chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;

                StripLine sprUsl = new StripLine();
                usl = SPCFuncClass.getSPCUSL();
                sprUsl.IntervalOffset = usl;
                sprUsl.BorderColor = Color.Red;
                sprUsl.Text = "USL=" + sprUsl.IntervalOffset.ToString("N1");
                sprUsl.TextAlignment = StringAlignment.Far;
                sprUsl.TextLineAlignment = StringAlignment.Far;
                sprUsl.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprUsl);

                StripLine sprLsl = new StripLine();
                lsl = SPCFuncClass.getSPCLSL();
                sprLsl.IntervalOffset = lsl;
                sprLsl.BorderColor = Color.Red;
                sprLsl.Text = "LSL=" + sprLsl.IntervalOffset.ToString("N1");
                sprLsl.TextAlignment = StringAlignment.Far;
                sprLsl.BorderDashStyle = ChartDashStyle.Dash;
                sprLsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprLsl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("initChart() in SPC charts function !" + ex);
            }
        }


        private void CPK_chart_Init(Chart chart)
        {
            int i;
            float usl, lsl;

            Label[] labelArray = 
            { 
                label1,  label2,  label3,  label4,  label5,  label6,  label7,  label8, 
                label16, label15, label14, label13, label12, label11, label10, label9, 
                label24, label23, label22, label21, label20, label19, label18, label17 
            };

            try
            {
                for(i = 0; i < labelNumInCPKChart; i++)
                {
                    labelArray[i].Text = CPKLabelTextArray[i];
                }

                label8.Visible = false;
                textBox8.Visible = false;

                if (analyzeChartType != gVariable.CHART_TYPE_SPC_XBAR_S)
                {
                    label9.Enabled = false;
                    label10.Enabled = false;
                    label11.Enabled = false;
                    label12.Enabled = false;
                }

                label26.Text = getSPCChartTitle(TAB_CONTROL_CPK_DATA);

                textBox1.Text = numOfPointNeedForChart.ToString();
                textBox1.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox5.ReadOnly = true;
                textBox6.ReadOnly = true;
                textBox7.ReadOnly = true;
                textBox9.ReadOnly = true;
                textBox10.ReadOnly = true;
                textBox11.ReadOnly = true;
                textBox12.ReadOnly = true;
                textBox13.ReadOnly = true;
                textBox14.ReadOnly = true;
                textBox15.ReadOnly = true;
                textBox16.ReadOnly = true;
                textBox17.ReadOnly = true;
                textBox18.ReadOnly = true;
                textBox19.ReadOnly = true;
                textBox20.ReadOnly = true;
                textBox21.ReadOnly = true;
                textBox22.ReadOnly = true;
                textBox23.ReadOnly = true;
                textBox24.ReadOnly = true;

                if (analyzeChartType == gVariable.CHART_TYPE_NO_SPC)
                {
                    textBox2.Text = gVariable.pointNumInNoSPCChartGroup.ToString();
                    textBox3.Text = gVariable.numOfGroupsInNoSPCChart.ToString();

                    textBox4.Text = SPCFuncClass.getSPCUSL().ToString();
                    textBox5.Text = SPCFuncClass.getSPCLSL().ToString();
                    textBox6.Text = SPCFuncClass.getSPCMAX().ToString();
                    textBox7.Text = SPCFuncClass.getSPCMIN().ToString();
                    textBox16.Text = SPCFuncClass.getSPCAverage().ToString();
                    textBox15.Text = SPCFuncClass.getSPCDelta().ToString();
                    textBox14.Text = SPCFuncClass.getSPCAverageSigma().ToString();
                    textBox13.Text = SPCFuncClass.getSPCTotalSigma().ToString();
                    textBox12.Enabled = false;
                    textBox11.Enabled = false;
                    textBox10.Enabled = false;
                    textBox9.Enabled = false;
                    textBox24.Text = SPCFuncClass.getSPCCp().ToString();
                    textBox23.Text = SPCFuncClass.getSPCCpk().ToString();
                    textBox22.Text = SPCFuncClass.getSPCCa().ToString();
                    textBox21.Text = SPCFuncClass.getSPCPp().ToString();
                    textBox20.Text = SPCFuncClass.getSPCPpk().ToString();
                    textBox19.Text = SPCFuncClass.getSPCPpu().ToString();
                    textBox18.Text = SPCFuncClass.getSPCPpl().ToString();
                    textBox17.Text = SPCFuncClass.getSPCPPM().ToString();
                }
                else
                {
                    if (analyzeChartType == gVariable.CHART_TYPE_SPC_C)
                    {
                        textBox2.Text = gVariable.pointNumInCChartGroup.ToString();
                        textBox3.Text = gVariable.numOfGroupsInCChart.ToString();
                    }
                    else //if(analyzeChartType == gVariable.CHART_TYPE_SPC_XBAR_S)
                    {
                        textBox2.Text = gVariable.pointNumInSChartGroup.ToString();
                        textBox3.Text = gVariable.numOfGroupsInSChart.ToString();
                    }

                    textBox4.Text = SPCFuncClass.getSPCUSL().ToString();
                    textBox5.Text = SPCFuncClass.getSPCLSL().ToString();
                    textBox6.Text = SPCFuncClass.getSPCMAX().ToString();
                    textBox7.Text = SPCFuncClass.getSPCMIN().ToString();
                    textBox16.Text = SPCFuncClass.getSPCAverage().ToString();
                    textBox15.Text = SPCFuncClass.getSPCDelta().ToString();
                    textBox14.Text = SPCFuncClass.getSPCAverageSigma().ToString();
                    textBox13.Text = SPCFuncClass.getSPCTotalSigma().ToString();
                    textBox12.Text = controlCenterValueArray[indexInTable, 0].ToString();
                    textBox11.Text = controlCenterValueArray[indexInTable, 2].ToString();
                    textBox10.Text = controlCenterValueArray[indexInTable, 3].ToString();
                    textBox9.Text = controlCenterValueArray[indexInTable, 5].ToString();
                    textBox24.Text = SPCFuncClass.getSPCCp().ToString();
                    textBox23.Text = SPCFuncClass.getSPCCpk().ToString();
                    textBox22.Text = SPCFuncClass.getSPCCa().ToString();
                    textBox21.Text = SPCFuncClass.getSPCPp().ToString();
                    textBox20.Text = SPCFuncClass.getSPCPpk().ToString();
                    textBox19.Text = SPCFuncClass.getSPCPpu().ToString();
                    textBox18.Text = SPCFuncClass.getSPCPpl().ToString();
                    textBox17.Text = SPCFuncClass.getSPCPPM().ToString();
                }

                chart.Series[0].BorderWidth = 1;
                chart.Series[0].ShadowOffset = 1;
                chart.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                chart.Series[0].IsValueShownAsLabel = true;  //
                chart.Series[0].MarkerSize = 4; // size of the data point
                chart.Series[0].EmptyPointStyle.CustomProperties = "Exploded=True";
                chart.Series[0].EmptyPointStyle.IsValueShownAsLabel = true;
                chart.Series[0]["PieLabelStyle"] = "Outside";
                chart.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve

                chart.ChartAreas[0].Area3DStyle.Enable3D = true;
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "N1";  //小数点后取四位
//                chart.ChartAreas[0].AxisY.IsStartedFromZero = true; // false;  //whether we need to start at 0 for Y axis
                chart.BackColor = System.Drawing.Color.Azure;
                chart.BackGradientStyle = GradientStyle.DiagonalLeft;
                chart.BackSecondaryColor = System.Drawing.Color.SkyBlue;

                chart.BackColor = System.Drawing.Color.Azure;
                chart.BackGradientStyle = GradientStyle.DiagonalLeft;
                chart.BackSecondaryColor = System.Drawing.Color.SkyBlue;

                chart.Visible = true;

                StripLine sprUsl = new StripLine();
                usl = specUpperLimitArray[indexInTable];
                sprUsl.IntervalOffset = usl;
                sprUsl.BorderColor = Color.Red;
                sprUsl.Text = "USL=" + usl.ToString("N1");
                sprUsl.TextAlignment = StringAlignment.Near;
                sprUsl.TextLineAlignment = StringAlignment.Near;
                sprUsl.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisX.StripLines.Add(sprUsl);

                StripLine sprLsl = new StripLine();
                lsl = specLowerLimitArray[indexInTable];
                sprLsl.IntervalOffset = lsl;
                sprLsl.BorderColor = Color.Red;
                sprLsl.Text = "LSL=" + lsl.ToString("N1");
                sprLsl.TextAlignment = StringAlignment.Near;
                sprLsl.TextLineAlignment = StringAlignment.Near;
                sprLsl.BorderDashStyle = ChartDashStyle.Dash;
                sprLsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisX.StripLines.Add(sprLsl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CPK chart init() in SPC charts function failed!" + ex);
            }
        }

        private void Ori_chart_Init(Chart chart)
        {
            float usl, lsl;

            try
            {
                label27.Text = getSPCChartTitle(TAB_CONTROL_ORIGINAL_DATA);

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

                chart.Series[1].ChartType = SeriesChartType.Line; //Line is streight line, SpLine is curve
                chart.Series[1].MarkerStyle = MarkerStyle.None; //shape of the data point
                chart.Series[1].Color = Color.White;
                chart.Series[1].BorderWidth = 1;
                chart.Series[1].ShadowOffset = 1;
                chart.Series[1].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                chart.Series[1].IsValueShownAsLabel = false; //
                chart.Series[1].MarkerSize = 1; // size of the data point

                chart.Visible = true;
                chart.ChartAreas[0].AxisY.LabelStyle.Format = "N2";
                chart.ChartAreas[0].AxisY.IsStartedFromZero = false;  //whether we need to start at 0 for Y axis
                //chart.ChartAreas[0].AxisX.IsStartedFromZero = true;  //whether we need to start at 0 for X axis

                chart.ChartAreas[0].AlignmentOrientation = ((AreaAlignmentOrientations)((AreaAlignmentOrientations.Vertical | AreaAlignmentOrientations.Horizontal)));
                chart.ChartAreas[0].AxisX.Interval = numOfPointNeedForChart / verticalLineEveryNumOfPoint;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                chart.ChartAreas[0].AxisX.LabelAutoFitStyle = ((LabelAutoFitStyles)((LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap)));
                chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = numOfPointNeedForChart / verticalLineEveryNumOfPoint;
                chart.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0;
                chart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.LineWidth = 2;
                chart.ChartAreas[0].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
                //            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chart.ChartAreas[0].AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.NotSet;
                chart.ChartAreas[0].AxisX.Minimum = 1;
                chart.ChartAreas[0].AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Number;
                chart.ChartAreas[0].AxisX.ScaleView.Size = numOfPointNeedForChart;
                chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;

                StripLine sprUsl = new StripLine();
                usl = SPCFuncClass.getSPCUSL();
                sprUsl.IntervalOffset = usl;
                sprUsl.BorderColor = Color.Red;
                sprUsl.Text = "USL=" + sprUsl.IntervalOffset.ToString("N1");
                sprUsl.TextAlignment = StringAlignment.Far;
                sprUsl.TextLineAlignment = StringAlignment.Far;
                sprUsl.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprUsl);

                StripLine sprLsl = new StripLine();
                lsl = SPCFuncClass.getSPCLSL();
                sprLsl.IntervalOffset = lsl;
                sprLsl.BorderColor = Color.Red;
                sprLsl.Text = "LSL=" + sprLsl.IntervalOffset.ToString("N1");
                sprLsl.TextAlignment = StringAlignment.Far;
                sprLsl.BorderDashStyle = ChartDashStyle.Dash;
                sprLsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprLsl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ORI chart init() in SPC charts function failed!" + ex);
            }

        }

        private void Average_chart_Init(Chart chart)
        {
            int i;
            float usl, lsl;

            try
            {
                label28.Text = getSPCChartTitle(TAB_CONTROL_AVERAGE_DATA);

                //            chart.Series[0].ChartType = SeriesChartType.Spline; //Line is streight line, SpLine is curve
                chart.Series[0].Color = Color.Green;
                chart.Series[0].BorderWidth = 2;
                //            chart.Series[0].ShadowOffset = 1;
                //            chart.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                //            chart.Series[0].IsValueShownAsLabel = false;  //
                //            chart.Series[0].MarkerSize = 4; // size of the data point
                //            chart.Series[0].XValueType = ChartValueType.String;
                //            chart.Series[0].YValueType = ChartValueType.Double;

                chart.Series[0].ChartType = SeriesChartType.FastPoint; //Line is streight line, SpLine is curve
                chart.Series[0].MarkerStyle = MarkerStyle.Diamond; //shape of the data point
                //            chart.Series[0].Color = Color.Blue;
                chart.Series[0].BorderWidth = 6;
                chart.Series[0].ShadowOffset = 1;
                chart.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                chart.Series[0].IsValueShownAsLabel = false; //
                chart.Series[0].MarkerSize = 8; // size of the data point

                for (i = 1; i < gVariable.pointNumInSChartGroup; i++)
                {
                    chart.Series[i].ChartType = SeriesChartType.FastPoint; //Line is streight line, SpLine is curve
                    chart.Series[i].MarkerStyle = MarkerStyle.Diamond; //shape of the data point
                    chart.Series[i].Color = Color.Blue;
                    chart.Series[i].BorderWidth = 6;
                    chart.Series[i].ShadowOffset = 1;
                    chart.Series[i].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                    chart.Series[i].IsValueShownAsLabel = false; //
                    chart.Series[i].MarkerSize = 8; // size of the data point
                }

                chart.Visible = true;
                chart.ChartAreas[0].AxisY.LabelStyle.Format = "N2";
                chart.ChartAreas[0].AxisY.IsStartedFromZero = false;  //whether we need to start at 0 for Y axis
                //chart.ChartAreas[0].AxisX.IsStartedFromZero = true;  //whether we need to start at 0 for X axis

                chart.ChartAreas[0].AlignmentOrientation = ((AreaAlignmentOrientations)((AreaAlignmentOrientations.Vertical | AreaAlignmentOrientations.Horizontal)));
                chart.ChartAreas[0].AxisX.Interval = 3;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                chart.ChartAreas[0].AxisX.LabelAutoFitStyle = ((LabelAutoFitStyles)((LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap)));
                chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = 3;
                chart.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0;
                chart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.LineWidth = 2;
                chart.ChartAreas[0].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
                //            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chart.ChartAreas[0].AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.NotSet;
                chart.ChartAreas[0].AxisX.Minimum = 1;
                chart.ChartAreas[0].AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Number;
                chart.ChartAreas[0].AxisX.ScaleView.Size = gVariable.numOfGroupsInSChart;
                chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;

                StripLine sprUsl = new StripLine();
                usl = SPCFuncClass.getSPCUSL();
                sprUsl.IntervalOffset = usl;
                sprUsl.BorderColor = Color.Red;
                sprUsl.Text = "USL=" + sprUsl.IntervalOffset.ToString("N1");
                sprUsl.TextAlignment = StringAlignment.Far;
                sprUsl.TextLineAlignment = StringAlignment.Far;
                sprUsl.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprUsl);

                StripLine sprLsl = new StripLine();
                lsl = SPCFuncClass.getSPCLSL();
                sprLsl.IntervalOffset = lsl;
                sprLsl.BorderColor = Color.Red;
                sprLsl.Text = "LSL=" + sprLsl.IntervalOffset.ToString("N1");
                sprLsl.TextAlignment = StringAlignment.Far;
                sprLsl.BorderDashStyle = ChartDashStyle.Dash;
                sprLsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprLsl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Average chart init() in SPC charts function failed!" + ex);
            }
        }

        private void Overall_chart_Init(Chart chart)
        {
            try
            {
                label29.Text = getSPCChartTitle(TAB_CONTROL_ALL_DATA);

                totalDataNumInBuffer = readAllDataByPercent(databaseName, tableName, indexInTable, 100, 100);
                if (totalDataNumInBuffer > gVariable.minDataForOneScreen)
                    oneCurveScreenSize = gVariable.minDataForOneScreen;
                else
                    oneCurveScreenSize = totalDataNumInBuffer;

                chart.Series[0].ChartType = SeriesChartType.Spline; //Line is streight line, SpLine is curve
                chart.Series[0].MarkerStyle = MarkerStyle.Diamond; //shape of the data point
                chart.Series[0].Color = Color.Green;
                chart.Series[0].BorderWidth = 2;
                chart.Series[0].ShadowOffset = 1;
                chart.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                chart.Series[0].IsValueShownAsLabel = false;  //
                chart.Series[0].MarkerSize = 4; // size of the data point
                chart.Series[0].XValueType = ChartValueType.String;
                chart.Series[0].YValueType = ChartValueType.Double;

                chart.Series[1].ChartType = SeriesChartType.Line; //Line is streight line, SpLine is curve
                chart.Series[1].MarkerStyle = MarkerStyle.None; //shape of the data point
                chart.Series[1].Color = Color.White;
                chart.Series[1].BorderWidth = 1;
                chart.Series[1].ShadowOffset = 1;
                chart.Series[1].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                chart.Series[1].IsValueShownAsLabel = false; //
                chart.Series[1].MarkerSize = 1; // size of the data point

                chart.Visible = true;
                chart.ChartAreas[0].AxisY.LabelStyle.Format = "N2";
                chart.ChartAreas[0].AxisY.IsStartedFromZero = false;  //whether we need to start at 0 for Y axis
                //chart.ChartAreas[0].AxisX.IsStartedFromZero = true;  //whether we need to start at 0 for X axis

                chart.ChartAreas[0].AlignmentOrientation = ((AreaAlignmentOrientations)((AreaAlignmentOrientations.Vertical | AreaAlignmentOrientations.Horizontal)));
                chart.ChartAreas[0].AxisX.Interval = numOfPointNeedForChart / verticalLineEveryNumOfPoint;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                chart.ChartAreas[0].AxisX.LabelAutoFitStyle = ((LabelAutoFitStyles)((LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap)));
                chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = numOfPointNeedForChart / verticalLineEveryNumOfPoint;
                chart.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0;
                chart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.LineWidth = 2;
                chart.ChartAreas[0].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
                //chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chart.ChartAreas[0].AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.NotSet;
                chart.ChartAreas[0].AxisX.Minimum = 1;
                chart.ChartAreas[0].AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Number;
                chart.ChartAreas[0].AxisX.ScaleView.Size = numOfPointNeedForChart;
                chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;

                hScrollBar1.Value = 100;
                hScrollBar2.Value = 100;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Overall chart init() in SPC charts function failed!" + ex);
            }
        }

        private string getSPCChartTitle(int tabIndex)
        {
            int boardIndex;
            string productName;
            string itemName;
//            string cause;
            string title;

            boardIndex = toolClass.getBoardIndexByDatabaseName(databaseName);

            if (SPCFromAlarmOrQualityManagement == gVariable.FROM_ALARM_DISPLAY_FUNC)
            {
                productName = mySQLClass.getAnothercolumnFromDatabaseByOneColumn(databaseName, gVariable.dispatchListTableName, "dispatchCode", dispatchCode, mySQLClass.PRODUCT_NAME_IN_DISPATCHLIST_DATABASE);

                title = SPCTitleArray[tabIndex] + " (产品名称: " + productName + "; 报警原因: " + errorItem + "; 报警人:" + alarmTableStructImpl.operatorName + ")";
            }
            else
            {
                productName = gVariable.productTaskSheet[boardIndex].productName;

                if (category >= gVariable.ALARM_CATEGORY_CRAFT_DATA_START)
                {
                    itemName = gVariable.craftList[boardIndex].paramName[indexInTable];
                }
                else
                {
                    itemName = gVariable.qualityList[boardIndex].checkItem[indexInTable];
                }

                alarmTableStructImpl.operatorName = gVariable.productTaskSheet[boardIndex].operatorName;

                title = SPCTitleArray[tabIndex] + " (产品名称: " + productName + "; 监测项目: " + itemName + "; 负责人:" + alarmTableStructImpl.operatorName + ")";
            }

            return title;
        }

        //XBar-S chart and c chart
        public void SPCChartSettings(Chart chart)
        {
            if (analyzeChartType == gVariable.CHART_TYPE_SPC_XBAR_S)
            {
                //indexInTable means which data item in quality data, we use this index to get data from dataInPoint
                SPCFuncClass.get_XBar_S_Chart(indexInTable, chart5, gVariable.SPC_DATA_AND_CHART, dataInPoint, timeInPoint, controlCenterValueArray);
            }
            else if (analyzeChartType == gVariable.CHART_TYPE_SPC_C)
            {
                //indexInTable means which data item in quality data, we use this index to get data from dataInPoint
                SPCFuncClass.get_C_Chart(indexInTable, chart5, gVariable.SPC_DATA_AND_CHART, dataInPoint, timeInPoint, controlCenterValueArray);
            }
        }

        //draw a column chart
        private void CPKChartSettings(Chart chart)
        {
            int i;
            int pointIndex;
            float delta;
            DataPoint dataPoint;

            try
            {
                foreach (var series in chart.Series)
                {
                    series.Points.Clear();
                }

                delta = gVariable.columnLimits[indexInTable, 1] - gVariable.columnLimits[indexInTable, 0];
                chart.Series[0].Points.AddXY(gVariable.columnLimits[indexInTable, 0] - delta, 0);
                for (i = 0; i < gVariable.numOfColumns; i++)
                {
                    pointIndex = chart.Series[0].Points.AddXY((gVariable.columnLimits[indexInTable, i] + gVariable.columnLimits[indexInTable, i + 1]) / 2, gVariable.columnData[indexInTable, i]);
                    dataPoint = chart.Series[0].Points[pointIndex];
                    dataPoint.LegendText = gVariable.columnLimits[indexInTable, i].ToString("f1");
//                    dataPoint.Label = "#PERCENT{P2}";
                }
                pointIndex = chart.Series[0].Points.AddXY(gVariable.columnLimits[indexInTable, i] + delta, 0);
            }
            catch (Exception e)
            {
                Console.Write("init column in SPC charts failed :" + e);
            }
        }

        private void OriChartSettings(Chart chart)
        {
            int i;
            int interval;
            int trendFlag;
            int dataNum;
            float f;
            float usl, lsl;
            float delta;
            float maxV, minV;
            string xString;
            DateTime dateTime;

            try
            {
                if (analyzeChartType == gVariable.CHART_TYPE_NO_SPC)
                {
                    dataNum = gVariable.totalPointNumForNoSPCChart;
                }
                else if (analyzeChartType == gVariable.CHART_TYPE_SPC_C)
                {
                    dataNum = gVariable.totalPointNumForCChart;
                }
                else //if (analyzeChartType == gVariable.CHART_TYPE_SPC_XBAR_S)
                {
                    dataNum = gVariable.totalPointNumForSChart;
                }

                foreach (var series in chart.Series)
                {
                    series.Points.Clear();
                }

                maxV = dataInPoint[indexInTable, 0];
                minV = dataInPoint[indexInTable, 0];

                usl = SPCFuncClass.getSPCUSL();
                lsl = SPCFuncClass.getSPCLSL();

                for (i = 0; i < dataNum; i++)
                {
                    dateTime = toolClass.GetTime((timeInPoint[indexInTable, i] - 3600 * 7).ToString());
                    xString = dateTime.ToString("MM-dd HH:mm:ss");
                    f = dataInPoint[indexInTable, i];
                    chart.Series[0].Points.AddXY(xString, f);

                    if (f > maxV)
                        maxV = f;

                    if (f < minV)
                        minV = f;
                }

                if (usl > maxV)
                    maxV = usl;

                if (lsl < minV)
                    minV = lsl;

                delta = maxV - minV;
                maxV += delta / 7;

                minV -= delta / 7;

                chart.ChartAreas[0].AxisY.Maximum = maxV;
                chart.ChartAreas[0].AxisY.Minimum = minV;

                interval = totalDataNumInBuffer / 15;

                i = 0;

                chart.Annotations.Clear();
                trendFlag = 0;
                foreach (var point in chart.Series[0].Points)
                {
                    f = (float)point.YValues[0];

                    if (f > usl || f < lsl)
                    {
                        point.Color = Color.Red;
                        if (trendFlag == 0)
                            trendFlag = 1;
                        else
                            trendFlag = 0;
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
            }
            catch (Exception e)
            {
                Console.Write("Ori chart init in SPC charts failed :" + e);
            }

        }


        private void AverageChartSettings(Chart chart)
        {
            int i, j;
            float f, sum;
            float usl, lsl;
            float delta;
            float maxV, minV;
            string xString;
            DateTime dateTime;

            foreach (var series in chart.Series)
            {
                series.Points.Clear();
            }

            maxV = dataInPoint[indexInTable, 0];
            minV = dataInPoint[indexInTable, 0];

            usl = SPCFuncClass.getSPCUSL();
            lsl = SPCFuncClass.getSPCLSL();

            for (i = 0; i < gVariable.numOfGroupsInSChart; i++)
            {
                sum = 0;
                xString = null;
                for (j = 0; j < gVariable.pointNumInSChartGroup; j++)
                {
                    dateTime = toolClass.GetTime((timeInPoint[indexInTable, i * gVariable.pointNumInSChartGroup + j] - 3600 * 7).ToString());
                    xString = dateTime.ToString("MM-dd HH:mm:ss");
                    f = dataInPoint[indexInTable, i * gVariable.pointNumInSChartGroup + j];
                    sum += f;

                    if (f > maxV)
                        maxV = f;

                    if (f < minV)
                        minV = f;

                    if(j != 0)
                        chart.Series[j].Points.AddXY("", f);
                }

                f = sum / gVariable.pointNumInSChartGroup;
                chart.Series[0].Points.AddXY("", f);
            }

            if (usl > maxV)
                maxV = usl;

            if (lsl < minV)
                minV = lsl;

            delta = maxV - minV;
            maxV += delta / 10;

            minV -= delta / 10;

            chart.ChartAreas[0].AxisY.Maximum = maxV;
            chart.ChartAreas[0].AxisY.Minimum = minV;
        }


        public int readAllDataByPercent(string databaseName, string tablename, int index, int dataNumPercent, int posPercent)
        {
            int totalDataNum;
            int numOfDataPointWanted;
            int dataStartPosition;

            try
            {
                totalDataNum = mySQLClass.getRecordNumInTable(databaseName, tablename);
                //we can display at least minDataForOneScreen of data point in one screen, if total number is less than tat, just display all
                if (totalDataNum <= gVariable.minDataForOneScreen)
                {
                    numOfDataPointWanted = totalDataNum;
                    dataStartPosition = 0;
                }
                else
                {
                    numOfDataPointWanted = (totalDataNum - gVariable.minDataForOneScreen) * (100 - dataNumPercent) / 100 + gVariable.minDataForOneScreen;
                    dataStartPosition = (int)((long)(totalDataNum - numOfDataPointWanted) * posPercent / 100);
                }

                return mySQLClass.readAllDataToArrayByNum(databaseName, tableName, index, dataStartPosition, numOfDataPointWanted, totalDataNum);
            }
            catch (Exception ex)
            {
                Console.WriteLine("oneCuve data reading all data fail!" + ex);
                return 0;
            }
        }



        private void OverallChartSettings(Chart chart, int dataNumPercent, int posPercent)
        {
            int i, j; // index;
            int interval;
            int trendFlag;
            float f;
            float delta;
            DateTime dateTime;
            string tableName;
            string xString;
            float maxV, minV, usl, lsl;

            try
            {
                lsl = SPCFuncClass.getSPCLSL();
                usl = SPCFuncClass.getSPCUSL();

                if (type == gVariable.ALARM_TYPE_QUALITY_DATA)
                    tableName = dispatchCode + gVariable.qualityTableNameAppendex;
                else //if (type == gVariable.ALARM_TYPE_CRAFT_DATA)
                    tableName = dispatchCode + gVariable.craftTableNameAppendex;

                totalDataNumInBuffer = readAllDataByPercent(databaseName, tableName, indexInTable, dataNumPercent, posPercent);

                if (totalDataNumInBuffer < gVariable.minDataForOneScreen)
                    oneCurveScreenSize = gVariable.minDataForOneScreen;
                else
                    oneCurveScreenSize = totalDataNumInBuffer;

                chart.ChartAreas[0].AxisX.ScaleView.Size = oneCurveScreenSize;

                if (totalDataNumInBuffer > gVariable.totalPointNumForSChart)
                {
                    //chart.ChartAreas[0].AxisX.Title = "Cp: " + gVariable.cp[index].ToString("f4") + "     Cpk: " + gVariable.cpk[index].ToString("f4") + "     Ppk: " + gVariable.ppk[index].ToString("f4");
                }

                foreach (var series in chart.Series)
                {
                    series.Points.Clear();
                }

                maxV = gVariable.oneCurveDataInPoint[0];
                minV = maxV;

                for (i = 0; i < totalDataNumInBuffer; i++)
                {
                    j = gVariable.oneCurveIndexInPoint[i];
                    dateTime = toolClass.GetTime((gVariable.oneCurveTimeInPoint[i] - 3600 * 7).ToString());
                    xString = j + "\n" + dateTime.ToString("MM-dd HH:mm:ss");
                    f = gVariable.oneCurveDataInPoint[i];
                    chart.Series[0].Points.AddXY(xString, f);

                    if (f > maxV)
                        maxV = f;

                    if (f < minV)
                        minV = f;
                }

                if (maxV < usl)
                    maxV = usl;

                if (minV > lsl)
                    minV = lsl;

                delta = maxV - minV;
                maxV += delta / 10;
                minV -= delta / 10;

                if (maxV == minV)
                    maxV += 1;

                chart.ChartAreas[0].AxisY.Maximum = maxV;
                chart.ChartAreas[0].AxisY.Minimum = minV;

                StripLine sprUsl = new StripLine();
                sprUsl.IntervalOffset = usl;
                sprUsl.BorderColor = Color.Red;
                sprUsl.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprUsl);

                StripLine sprlsl = new StripLine();
                sprlsl.IntervalOffset = lsl;
                sprlsl.BorderColor = Color.Red;
                sprlsl.BorderDashStyle = ChartDashStyle.Dash;
                sprlsl.BorderWidth = 1;
                chart.ChartAreas[0].AxisY.StripLines.Add(sprlsl);

                interval = totalDataNumInBuffer / 15;

                i = 0;

                chart.Annotations.Clear();
                trendFlag = 0;
                foreach (var point in chart.Series[0].Points)
                {
                    f = (float)point.YValues[0];

                    if (f > usl || f < lsl)
                    {
                        point.Color = Color.Red;
                        if (trendFlag == 0)
                            trendFlag = 1;
                        else
                            trendFlag = 0;
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

                hScrollBar1.Minimum = 0;
                hScrollBar2.Minimum = 0;
                hScrollBar1.Maximum = 109;
                hScrollBar2.Maximum = 109;

                vScrollBar1 = 100;
                vScrollBar2 = 100;

                if (totalDataNumInBuffer < oneCurveScreenSize)
                {
                    hScrollBar1.Enabled = false;
                    hScrollBar2.Enabled = false;
                }

                chart.ChartAreas[0].AxisX.ScaleView.Position = 1; //we have put many points to chart by Points.AddXY(), then from which point we start our display

            }
            catch (Exception ex)
            {
                Console.Write("draw one curve error in SPC" + ex);
            }

        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)  //shrink and enlarg
        {
            Chart chart;

            if (hScrollBar1.Value == vScrollBar1)
                return;

            chart = chart1;

            OverallChartSettings(chart, hScrollBar1.Value, hScrollBar2.Value);

            chart.ChartAreas[0].AxisX.ScaleView.Position = 1; // (totalDataNumForWholeCurve - oneCurveScreenSize) * hScrollBar2.Value / initialDatPointNum;
            chart.ChartAreas[0].AxisX.Interval = oneCurveScreenSize / verticalLineEveryNumOfPoint;  //draw a vertical line every "Interval" point
            chart.ChartAreas[0].AxisX.LabelStyle.Interval = oneCurveScreenSize / verticalLineEveryNumOfPoint;  //draw label string every "Interval" point

            vScrollBar1 = hScrollBar1.Value;
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)  //move back and forth
        {
            Chart chart;

            if (hScrollBar2.Value == vScrollBar2)
                return;

            chart = chart1;

            OverallChartSettings(chart, hScrollBar1.Value, hScrollBar2.Value);

            vScrollBar2 = hScrollBar2.Value;
        }
    }
}
