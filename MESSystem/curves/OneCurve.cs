using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;
using MESSystem.common;
using MESSystem.quality;

namespace MESSystem.curves
{
    public partial class OneCurve : Form
    {
        private int curveIndexSelected;
        private int oneCurveScreenSize;  //current point number displayed for a curve after the user moved scrool bar1 (decreas/increase)
        private int totalDataNumForWholeCurve;  //total number of data points for the whole curve
        private int verticalLineEveryNumOfPoint;

        private int vScrollBar1, vScrollBar2;

//        private const int lengthForOneLine = 24;
 
        public OneCurve(int curveIndex)
        {
            InitializeComponent();

            curveIndexSelected = curveIndex;
            oneCurveScreenSize = gVariable.minDataForOneScreen;
            verticalLineEveryNumOfPoint = 12;
            initChart1(curveIndex);
            totalDataNumForWholeCurve = gVariable.dataNumForCurve[curveIndex];

            vScrollBar1 = 100;
            vScrollBar2 = 100;

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        public void OneCurveChart(int index, int dataNumPercent, int posPercent)
        {
            int i, j; // index;
            int flag;
            int interval;
            float f;
            float delta, tmpMax, tmpMin;
            DateTime dateTime;
            string xString;
            float maxV, minV, USL, LSL;
            int totalDataNumInBuffer;  //total number of data need to be dislayed, only part of totalDataNumWanted because of the limitation of oneCurveScreenSize 
            int initialDatPointNum;  //initial point number for a curve
           
            initialDatPointNum = gVariable.minDataForOneScreen;

            maxV = gVariable.maxDataValue[index];
            minV = gVariable.minDataValue[index];

//            USL = gVariable.curveSTDValue[index] + gVariable.curveDeltaValue[index];
//            LSL = gVariable.curveSTDValue[index] - gVariable.curveDeltaValue[index];
            USL = gVariable.curveUpperLimit[index];
            LSL = gVariable.curveLowerLimit[index];

            totalDataNumInBuffer = mySQLClass.readAllDataToArrayByPercent(gVariable.currentCurveDatabaseName, index, dataNumPercent, posPercent);

            if (totalDataNumInBuffer < initialDatPointNum)
                oneCurveScreenSize = initialDatPointNum;
            else
                oneCurveScreenSize = totalDataNumInBuffer;

            chart1.ChartAreas[0].AxisX.ScaleView.Size = oneCurveScreenSize;

            try
            {
//                if (totalDataNumInBuffer > gVariable.totalPointNumForSChart)
//                {
//                    getSPCValues(index, USL, LSL);
//                    chart1.ChartAreas[0].AxisX.Title = "Cp: " + gVariable.cp[index].ToString("f4") + "     Cpk: " + gVariable.cpk[index].ToString("f4") + "     Ppk: " + gVariable.ppk[index].ToString("f4");
//                }

                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                }

                for (i = 0; i < totalDataNumInBuffer; i++)
                {
                    j = gVariable.oneCurveIndexInPoint[i];
                    dateTime = toolClass.GetTime((gVariable.oneCurveTimeInPoint[i] - 3600 * 7).ToString());
                    xString = j + "\n" + dateTime.ToString("MM-dd HH:mm:ss");
                    f = gVariable.oneCurveDataInPoint[i];
                    chart1.Series[0].Points.AddXY(xString, f);

                    if (f > maxV)
                        maxV = f;

                    if (f < minV)
                        minV = f;
                }
                tmpMax = USL;
                tmpMin = LSL;

                if (maxV > USL)
                    tmpMax = maxV;

                if (minV < LSL)
                    tmpMin = minV;


                delta = tmpMax - tmpMin;
                tmpMax += delta / 7;
                tmpMin -= delta / 7;

                if (tmpMax == tmpMin)
                    tmpMax += 1;

                chart1.ChartAreas[0].AxisY.Maximum = tmpMax;
                chart1.ChartAreas[0].AxisY.Minimum = tmpMin;

                StripLine sprUsl = new StripLine();
                sprUsl.IntervalOffset = USL;
                sprUsl.BorderColor = Color.Blue;
                sprUsl.BorderDashStyle = ChartDashStyle.Dash;
                sprUsl.BorderWidth = 1;
                chart1.ChartAreas[0].AxisY.StripLines.Add(sprUsl);

                StripLine sprlsl = new StripLine();
                sprlsl.IntervalOffset = LSL;
                sprlsl.BorderColor = Color.Blue;
                sprlsl.BorderDashStyle = ChartDashStyle.Dash;
                sprlsl.BorderWidth = 1;
                chart1.ChartAreas[0].AxisY.StripLines.Add(sprlsl);

                interval = totalDataNumInBuffer / 15;

                i = 0;

                flag = 0;
                chart1.Annotations.Clear();
                foreach (var point in chart1.Series[0].Points)
                {
                    float tmp = (float)point.YValues[0];

                    if (flag == 1 || tmp > USL || tmp < LSL)
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

                        chart1.Annotations.Add(annotation);
                        i = 0;
                    }
                    i++;
                }

                hScrollBar1.Minimum = 0;
                hScrollBar2.Minimum = 0;
                hScrollBar1.Maximum = 1009;
                hScrollBar2.Maximum = 1009; 

                if (totalDataNumInBuffer < oneCurveScreenSize)
                {
//                    if (totalDataNumInBuffer < oneCurveScreenSize * 2)
                    hScrollBar1.Enabled = false;
                    hScrollBar2.Enabled = false;
                }

                chart1.ChartAreas[0].AxisX.ScaleView.Position = 1; //we have put many points to chart by Points.AddXY(), then from which point we start our display

            }
            catch (Exception ex)
            {
                Console.Write("draw curve error" + ex);
            }
        }

        //this function is not used in one curve function, because one curve may include a larger number of points, CPK is only a result of limited number of points
        private void getSPCValues(int index, float usl, float lsl)
        {
            float cp, pp, cpk, ppk;

            SPCFunctions SPCClass = new SPCFunctions();

            SPCClass.CalculateSPC(usl, lsl, index, gVariable.dataInPoint, gVariable.CHART_TYPE_NO_SPC);

            cp = SPCClass.getSPCCp();
            pp = SPCClass.getSPCPp();
            cpk = SPCClass.getSPCCpk();
            ppk = SPCClass.getSPCPpk();

            chart1.ChartAreas[0].AxisX.Title = "Cp: " + cp.ToString("f4") + " Cpk: " + cpk.ToString("f4") + "Pp: " + pp.ToString("f4") + "     Ppk: " + ppk.ToString("f4");
        }

        private void initChart1(int index)
        {
            this.Text = gVariable.curveTitle[index];

            chart1.Series[0].ChartType = SeriesChartType.Spline; //Line is streight line, SpLine is curve
            chart1.Series[0].MarkerStyle = MarkerStyle.Diamond; //shape of the data point
            chart1.Series[0].Color = Color.Green;
            chart1.Series[0].BorderWidth = 2;
            chart1.Series[0].ShadowOffset = 1;
            chart1.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
            chart1.Series[0].IsValueShownAsLabel = false;  //
            chart1.Series[0].MarkerSize = 4; // size of the data point
            chart1.Series[0].XValueType = ChartValueType.String;
            chart1.Series[0].YValueType = ChartValueType.Double;

            chart1.Series[1].ChartType = SeriesChartType.Line; //Line is streight line, SpLine is curve
            chart1.Series[1].MarkerStyle = MarkerStyle.None; //shape of the data point
            chart1.Series[1].Color = Color.White;
            chart1.Series[1].BorderWidth = 1;
            chart1.Series[1].ShadowOffset = 1;
            chart1.Series[1].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
            chart1.Series[1].IsValueShownAsLabel = false; //
            chart1.Series[1].MarkerSize = 1; // size of the data point

            chart1.Visible = true;
            chart1.Titles.Add(gVariable.curveTitle[index]);
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "N2";
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;  //whether we need to start at 0 for Y axis
            //chart1.ChartAreas[0].AxisX.IsStartedFromZero = true;  //whether we need to start at 0 for X axis

            chart1.ChartAreas[0].AlignmentOrientation = ((AreaAlignmentOrientations)((AreaAlignmentOrientations.Vertical | AreaAlignmentOrientations.Horizontal)));
            chart1.ChartAreas[0].AxisX.Interval = oneCurveScreenSize / verticalLineEveryNumOfPoint;
            chart1.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart1.ChartAreas[0].AxisX.LabelAutoFitStyle = ((LabelAutoFitStyles)((LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap)));
            chart1.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
            chart1.ChartAreas[0].AxisX.LabelStyle.Interval = oneCurveScreenSize / verticalLineEveryNumOfPoint;
            chart1.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0;
            chart1.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.Blue;
            chart1.ChartAreas[0].AxisX.LineWidth = 2;
            chart1.ChartAreas[0].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold); 
//            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Blue;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart1.ChartAreas[0].AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.NotSet;
            chart1.ChartAreas[0].AxisX.Minimum = 1;
            chart1.ChartAreas[0].AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Number;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = oneCurveScreenSize;
            chart1.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
/*            chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollSizeType = DateTimeIntervalType.NotSet; //.Number;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            chart1.ChartAreas[0].AxisX.ScrollBar.BackColor = System.Drawing.Color.Transparent;
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonColor = System.Drawing.Color.SkyBlue;
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = false;
            chart1.ChartAreas[0].AxisX.ScrollBar.LineColor = System.Drawing.Color.DarkTurquoise;
            chart1.ChartAreas[0].AxisX.ScrollBar.Size = 15D;
*/
            hScrollBar1.Value = gVariable.PERCENTAGE_VALUE_FOR_ONE_CURVE;
            hScrollBar2.Value = gVariable.PERCENTAGE_VALUE_FOR_ONE_CURVE;

        }
        //
        private void OneCurve_Load(object sender, EventArgs e)
        {
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)  //shrink and enlarg
        {
            if (hScrollBar1.Value == vScrollBar1)
                return;

            OneCurveChart(curveIndexSelected, hScrollBar1.Value, hScrollBar2.Value);

            chart1.ChartAreas[0].AxisX.ScaleView.Position = 1; // (totalDataNumForWholeCurve - oneCurveScreenSize) * hScrollBar2.Value / initialDatPointNum;
            chart1.ChartAreas[0].AxisX.Interval = oneCurveScreenSize / verticalLineEveryNumOfPoint;  //draw a vertical line every "Interval" point
            chart1.ChartAreas[0].AxisX.LabelStyle.Interval = oneCurveScreenSize / verticalLineEveryNumOfPoint;  //draw label string every "Interval" point

            vScrollBar1 = hScrollBar1.Value;
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)  //move back and forth
        {
            if (hScrollBar2.Value == vScrollBar2)
                return;

            OneCurveChart(curveIndexSelected, hScrollBar1.Value, hScrollBar2.Value);

            vScrollBar2 = hScrollBar2.Value;
        }

/*
        //return: put data for SPC to gVariable.oneCurveDataForSPC 
        public void readOneCurveDataforSPC()
        {
            int i, j, pos;
            string dataFileName;
            StreamReader streamReader;

            try
            {
                pos = totalDataNumForWholeCurve / 25;

                dataFileName = "..\\..\\data\\oneCurveData";
                streamReader = new StreamReader(dataFileName, System.Text.Encoding.Default);
                for (i = 0; i < 25; i++)
                {
                    for (j = 0; j < 5; j++)
                    {
                        streamReader.BaseStream.Seek((i * pos + j) * lengthForOneLine, SeekOrigin.Begin);
                        gVariable.oneCurveDataForSPC[pos * i + j] = (float)Convert.ToDouble(streamReader.ReadLine().Trim().Remove(0, 8));
                    }
                }
                streamReader.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("read oneCuvre data fail for SPC??");
            }
        }
*/
    }
}
