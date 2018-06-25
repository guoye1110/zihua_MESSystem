using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using MESSystem.common;

namespace MESSystem.quality
{
//    using System;
    public partial class SPCFunctions
    {
        //SPC parameters, defined by SPC spec
        static float[] A2val = { 0, 0, 1.880f, 1.023f,0.729f,0.577f,0.483f,0.419f,0.373f,0.337f,0.308f,0.285f,0.266f,0.249f,0.235f,0.223f,0.212f,0.203f,0.194f,0.187f,0.180f,0.173f,0.167f,0.162f,0.157f,0.153f};
        static float[] A3val = { 0, 0, 2.659f, 1.954f, 1.628f, 1.427f, 1.287f, 1.182f, 1.099f, 1.032f, 0.975f, 0.927f, 0.886f, 0.850f, 0.817f, 0.789f, 0.763f, 0.739f, 0.718f, 0.698f, 0.680f, 0.663f, 0.647f, 0.633f, 0.619f, 0.606f };
        static float[] B3val = { 0, 0, 0f, 0f, 0f, 0f, 0.030f, 0.118f, 0.185f, 0.239f, 0.284f, 0.321f, 0.354f, 0.382f, 0.406f, 0.428f, 0.448f, 0.446f, 0.482f, 0.497f, 0.510f, 0.523f, 0.534f, 0.545f, 0.555f, 0.565f };
        static float[] B4val = { 0, 0, 3.276f, 2.568f, 2.266f, 2.089f, 1.970f, 1.882f, 1.815f, 1.761f, 1.716f, 1.679f, 1.640f, 1.618f, 1.594f, 1.572f, 1.552f, 1.534f, 1.518f, 1.503f, 1.490f, 1.477f, 1.466f, 1.455f, 1.445f, 1.435f };
        static float[] C4val = { 0, 0, 0.7979f, 0.8862f, 0.9213f, 0.9400f, 0.9515f, 0.9594f, 0.9650f, 0.9693f, 0.9727f, 0.9754f, 0.9776f, 0.9794f, 0.9810f, 0.9823f, 0.9835f, 0.9845f, 0.9854f, 0.9862f, 0.9869f, 0.9876f, 0.9882f, 0.9887f, 0.9892f, 0.9896f };
        static float[] D2val = { 0, 0, 1.128f, 1.693f, 2.059f, 2.326f, 2.543f, 2.704f, 2.847f, 2.970f, 3.078f, 3.173f, 3.258f, 3.336f, 3.407f, 3.472f, 3.532f, 3.588f, 3.640f, 3.689f, 3.735f, 3.778f, 3.819f, 3.858f, 3.895f, 3.931f };
        static float[] D3val = { 0, 0, 0, 0, 0, 0, 0, 0.076f, 0.136f, 0.184f, 0.223f, 0.256f, 0.283f, 0.307f, 0.328f, 0.347f, 0.363f, 0.378f, 0.391f, 0.403f, 0.415f, 0.425f, 0.434f, 0.443f, 0.451f, 0.459f };
        static float[] D4val = { 0, 0, 3.268f, 2.574f, 2.282f, 2.114f, 2.004f, 1.924f, 1.864f, 1.816f, 1.777f, 1.744f, 1.717f, 1.693f, 1.672f, 1.653f, 1.637f, 1.622f, 1.608f, 1.597f, 1.585f, 1.575f, 1.566f, 1.557f, 1.548f, 1.541f };

        static float[] A4val = { 0, 0, 1.880f, 1.187f, 0.796f, 0.691f, 0.548f, 0.508f, 0.433f, 0.412f, 0.362f };
        static float[] E2val = { 0, 0, 2.660f, 1.772f, 1.457f, 1.290f, 1.184f, 1.109f, 1.054f, 1.010f, 0.975f };

        static float vA2, vA3, vA4, vB3, vB4, vC4, vD2, vD3, vD4, vE2;

        //When we have a max value, we need to display a larger value of border, for example, the max value is 70, min value is 30, we should display the
        //curve from 25 to 78so we can see all data inside the curve with no data touch the border
        const int GAP_BETWEEN_MAX_AND_BOARD = 7;

        int boardIndex;
        int qualityItemIndex;

        public int pointNumInGroup;
        public int numOfGroupsInChart;
        public int totalPointNumForChart;

        float USL, LSL;//规格上下限　
        float Cp, Cpk, Ca, Pp, Ppk, Ppl, Ppu, PPM;
        float totalAverage, totalMax, totalMin, totalDelta, totalSigma, averageSigma;
        double vSum;

        public SPCFunctions()
        {
            SPCInit();
        }

        public SPCFunctions(int myBoardIndex)
        {
            boardIndex = myBoardIndex;

            SPCInit();
        }
        
        public void SPCInit()
        {
        }

        public void getSPCParameters()
        {
            vA2 = A2val[pointNumInGroup];
            vA3 = A3val[pointNumInGroup];
            vA4 = A4val[pointNumInGroup];
            vB3 = B3val[pointNumInGroup];
            vB4 = B4val[pointNumInGroup];
            vC4 = C4val[pointNumInGroup];
            vD2 = D2val[pointNumInGroup];
            vD3 = D3val[pointNumInGroup];
            vD4 = D4val[pointNumInGroup];
            vE2 = E2val[2];
        }

         public void CalculateSPC(float usl, float lsl, int index, float [,] dataInPointArray, int type)
         {
            try
            { 
                int i, j, k;
                float tmp;
                float SL; //公差带中间值
                float vMean, vMeanTotal;
                float vMax, vMin;
                float theta;
                float sigmaP; //总标准差
                float sigmaC; //组均标准差，子组内标准差的平均值
                float sigmaTotal; //子组内标准差的总合

                if (type == gVariable.CHART_TYPE_SPC_XBAR_S)
                {
                    pointNumInGroup = gVariable.pointNumInSChartGroup;
                    numOfGroupsInChart = gVariable.numOfGroupsInSChart;
                }
                else if (type == gVariable.CHART_TYPE_SPC_C)
                {
                    pointNumInGroup = gVariable.pointNumInCChartGroup;
                    numOfGroupsInChart = gVariable.numOfGroupsInCChart;
                }
                else // if (type == gVariable.CHART_TYPE_NO_SPC)
                {
                    pointNumInGroup = gVariable.pointNumInNoSPCChartGroup;
                    numOfGroupsInChart = gVariable.numOfGroupsInNoSPCChart;
                }
                totalPointNumForChart = pointNumInGroup * numOfGroupsInChart;

                getSPCParameters();

                qualityItemIndex = index;

                //公差带中间值
                SL = (usl + lsl) / 2;
                USL = usl;
                LSL = lsl;

                vSum = 0;
                vMax = dataInPointArray[index, 0];
                vMin = dataInPointArray[index, 0];

                for (i = 0; i < totalPointNumForChart; i++)
                {
                    vSum += dataInPointArray[index, i];
                    if (dataInPointArray[index, i] > vMax)
                        vMax = dataInPointArray[index, i];
                    if (dataInPointArray[index, i] < vMin)
                        vMin = dataInPointArray[index, i];
                }
                vMeanTotal = (float)(vSum / totalPointNumForChart);
                
                theta = vMeanTotal - SL;
                if(theta < 0)
                    theta *= -1;

                tmp = 0;
                for (i = 0; i < totalPointNumForChart; i++)
                {
                    tmp += (dataInPointArray[index, i] - vMeanTotal) * (dataInPointArray[index, i] - vMeanTotal);
                }

                //样本总体标准差
                sigmaP = (float)System.Math.Sqrt(tmp / (totalPointNumForChart - 1));

                sigmaTotal = 0;
                for(i = 0; i < numOfGroupsInChart; i++)
                {
                    tmp = 0;
                    k = i * pointNumInGroup;
                    for(j = 0; j < pointNumInGroup; j++)
                    {
                        tmp += dataInPointArray[index, k + j];
                    }
                    vMean = tmp / pointNumInGroup;

                    tmp = 0;
                    for (j = 0; j < pointNumInGroup; j++)
                    {
                        tmp += (dataInPointArray[index, k + j] - vMean) * (dataInPointArray[index, k + j] - vMean);
                    }
                    sigmaTotal += (float)System.Math.Sqrt(tmp / (pointNumInGroup - 1));
                }
                //组均标准差
                sigmaC = (sigmaTotal / numOfGroupsInChart) / vC4;

                if(sigmaC == 0)
                    Cp = 0xffff;
                else
                    Cp = (usl - lsl) / (6 * sigmaC);  //cp 等于公差除以六倍标准差
                
                Cpk = ((usl - lsl) - 2 * theta) / (6 * sigmaC);  //cpk 等于公差减去中心偏移量后除以六倍标准差（中心偏移量指规格中心与均值之间的差）--详见<<ISO/TS16949>> P151
                
                if(sigmaP == 0)
                {
                    Ppk = 0xffff;
                }
                else
                {
                    Pp = (usl - lsl) / (6 * sigmaP);
                    Ppu = (usl - vMeanTotal) / (3 * sigmaP);
                    Ppl = (vMeanTotal - lsl) / (3 * sigmaP);
                
                    if(Ppl < Ppu)
                        Ppk = Ppl;
                    else
                        Ppk = Ppu;
                }

                totalMax = vMax;
                totalMin = vMin;
                totalDelta = vMax - vMin;
                totalAverage = vMeanTotal;
                averageSigma = sigmaC;
                totalSigma = sigmaP;

                tmp = vMeanTotal - SL;
                if (tmp < 0)
                    tmp = 0 - tmp;
                Ca = tmp / (SL / 2);

                PPM = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CalculateSPC() in spc function !" + ex);
            }
        }

        public float getSPCUSL()
        {
            return USL;
        }

        public float getSPCLSL()
        {
            return LSL;
        }

        public float getSPCMAX()
        {
            return totalMax;
        }
        public float getSPCMIN()
        {
            return totalMin;
        }

        public float getSPCAverage()
        {
            return totalAverage;
        }

        public float getSPCDelta()
        {
            return totalDelta;
        }

        public float getSPCAverageSigma()
        {
            return averageSigma;
        }

        public float getSPCTotalSigma()
        {
            return totalSigma;
        }

        public float getSPCCp()
        {
            return Cp;
        }

        public float getSPCCpk()
        {
            return Cpk;
        }

        public float getSPCPp()
        {
            return Pp;
        }

        public float getSPCPpk()
        {
            return Ppk;
        }

        public float getSPCPpl()
        {
            return Ppl;
        }

        public float getSPCPpu()
        {
            return Ppu;
        }

        public float getSPCCa()
        {
            return Ca;
        }

        public float getSPCPPM()
        {
            return PPM;
        }

        public float getSPCUCL1()
        {
            return controlCenterValueArray[qualityItemIndex, 0];
        }

        public float getSPCLCL1()
        {
            return controlCenterValueArray[qualityItemIndex, 2];
        }

        public float getSPCUCL2()
        {
            return controlCenterValueArray[qualityItemIndex, 3];
        }

        public float getSPCLCL2()
        {
            return controlCenterValueArray[qualityItemIndex, 5];
        }

        public void get_XBar_R_Chart(int index, Chart chart, int SPCType)
        {
            int i, j, k;
            int num;
            int currentDataNumForCurve;
            float[] oneGroupTotal = new float[numOfGroupsInChart];
            float[] oneGroupAverage = new float[numOfGroupsInChart];
            float[] oneGroupDelta = new float[numOfGroupsInChart];
            float v, vMin, vMax;
//            float sigma1, sigma2, sigma3, sigma4;
            float realMax1, realMin1, realMax2, realMin2;
            float allGroupTotal, allGroupDelta, allGroupTotalAverage, allGroupDeltaAverage;
                
            vMax = 0;
            vMin = 0;
            allGroupTotal = 0;
            allGroupDelta = 0;

            try
            { 
                for (i = 0; i < numOfGroupsInChart; i++)
                {
                    oneGroupTotal[i] = 0;
                }
                
                currentDataNumForCurve = gVariable.dataNumForCurve[index];

                if (SPCType != gVariable.SPC_DATA_ONLY)
                {
                    foreach (var series in chart.Series)
                    {
                        series.Points.Clear();
                    }
                }

                if (totalPointNumForChart < currentDataNumForCurve)
                    num = totalPointNumForChart;
                else
                    num = currentDataNumForCurve;
                
                k = 0;
                realMax1 = 0;
                realMin1 = 0;
                realMax2 = 0;
                realMin2 = 0;
                
                for (i = 0, j = 0; i < num; i++, j++)
                {
                    v = gVariable.dataInPoint[index, i];
                    oneGroupTotal[k] += v;
                    if(j == 0)
                    {
                        vMax = v;
                        vMin = v;
                    }
                    else
                    {
                        if (v > vMax)
                            vMax = v;
                        if (v < vMin)
                            vMin = v;
                    }
                       
                    if( j == pointNumInGroup - 1)  //one group is OK
                    {
                        //get average value and delta value in a group
                        oneGroupAverage[k] = oneGroupTotal[k] / pointNumInGroup;
                        oneGroupDelta[k] = vMax - vMin;

                        //record max/min average value and delta value so far
                        if (i == pointNumInGroup - 1)  //first time inside, initialize
                        {
                            realMax1 = oneGroupAverage[0];
                            realMin1 = oneGroupAverage[0];
                
                            realMax2 = oneGroupDelta[0];
                            realMin2 = oneGroupDelta[0];
                        }
                        else
                        {
                            if (oneGroupAverage[k] > realMax1)
                                realMax1 = oneGroupAverage[k];
                            if (oneGroupAverage[k] < realMin1)
                                realMin1 = oneGroupAverage[k];
                
                            if (oneGroupDelta[k] > realMax2)
                                realMax2 = oneGroupDelta[k];
                            if (oneGroupDelta[k] < realMin2)
                                realMin2 = oneGroupDelta[k];
                        }
                
                        //get total value
                        allGroupTotal += oneGroupTotal[k];
                        allGroupDelta += oneGroupDelta[k];

                        if (SPCType != gVariable.SPC_DATA_ONLY)
                        {
                            chart.Series[0].Points.AddXY("11", oneGroupAverage[k]);
                            chart.Series[1].Points.AddXY("11", oneGroupDelta[k]);
                        }

                        k++;
                        j = -1;
                
                        if (k >= numOfGroupsInChart)
                            break;
                    }
                }
                
                allGroupTotalAverage = allGroupTotal / num;
                allGroupDeltaAverage = allGroupDelta / numOfGroupsInChart;
                
                if (SPCType != gVariable.SPC_DATA_ONLY)
                {
                    //Xbar chart control values
                    gVariable.SPCControlValue[index, 0] = allGroupTotalAverage + vA2 * allGroupDeltaAverage;
                    gVariable.SPCControlValue[index, 1] = allGroupTotalAverage;
                    gVariable.SPCControlValue[index, 2] = allGroupTotalAverage - vA2 * allGroupDeltaAverage;
                    //R chart control values
                    gVariable.SPCControlValue[index, 3] = vD4 * allGroupDeltaAverage;
                    gVariable.SPCControlValue[index, 4] = allGroupDeltaAverage;
                    gVariable.SPCControlValue[index, 5] = vD3 * allGroupDeltaAverage;

                    if (gVariable.SPCControlValue[index, 0] > realMax1)
                        realMax1 = gVariable.SPCControlValue[index, 0];
                    if (gVariable.SPCControlValue[index, 2] < realMin1)
                        realMin1 = gVariable.SPCControlValue[index, 2];

                    if (gVariable.SPCControlValue[index, 3] > realMax2)
                        realMax2 = gVariable.SPCControlValue[index, 3];
                    if (gVariable.SPCControlValue[index, 5] < realMin2)
                        realMin2 = gVariable.SPCControlValue[index, 5];

                    v = realMax1 - realMin1;
                    realMax1 += v / GAP_BETWEEN_MAX_AND_BOARD;
                    realMin1 -= v / GAP_BETWEEN_MAX_AND_BOARD;

                    v = realMax2 - realMin2;
                    realMax2 += v / GAP_BETWEEN_MAX_AND_BOARD;
                    realMin2 -= v / GAP_BETWEEN_MAX_AND_BOARD;

                    //when Maximum and Minimum are all 0, this chart will fail to be displayed, so we change this value to 0.001 and -0.001
                    if (realMax1 == 0 && realMin1 == 0)
                    {
                        chart.ChartAreas[0].AxisY.Maximum = 0.001;
                        chart.ChartAreas[0].AxisY.Minimum = -0.001;
                    }
                    else
                    {
                        chart.ChartAreas[0].AxisY.Maximum = realMax1;
                        chart.ChartAreas[0].AxisY.Minimum = realMin1;
                    }

                    if (realMax2 == 0 && realMin2 == 0)
                    {
                        chart.ChartAreas[1].AxisY.Maximum = 0.001;
                        chart.ChartAreas[1].AxisY.Minimum = -0.001;
                    }
                    else
                    {
                        chart.ChartAreas[1].AxisY.Maximum = realMax2;
                        chart.ChartAreas[1].AxisY.Minimum = realMin2;
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("get_XBar_R_Chart() in spc function !" + ex);
            }
        }


        //calaulate center/control values, and if chart is also needed, put data in chart array(so we can display them)
        //index: for SPCCurve screen, there are many craft data items, index means craft index
        //       for spc checking function, there are many data items for a qulity data, index means item index for quality data
        //SPCType: SPC_DATA_GET_UCL_SCL, SPC_DATA_ONLY or SPC_DATA_AND_CHART
        public int get_XBar_S_Chart(int index, Chart chart, int SPCType, float[,] dataInPointArray, int[,] timeInPoint, float[,] controlCenterValueArray)
        {
            try
            {
                int i, j, k, l;
                float currentAverage;
                float standardGroupTotal;
                float[] oneGroupTotal = new float[numOfGroupsInChart];
                float v;
                float realMax1, realMin1, realMax2, realMin2;
                float allGroupTotal, allGroupDelta, allGroupTotalAverage, allGroupDeltaAverage;

                allGroupTotal = 0;
                allGroupDelta = 0;

                for (i = 0; i < numOfGroupsInChart; i++)
                {
                    oneGroupTotal[i] = 0;
                }

                if (SPCType == gVariable.SPC_DATA_AND_CHART)
                {
                    foreach (var series in chart.Series)
                    {
                        series.Points.Clear();
                    }
                }

                k = 0;
                realMax1 = 0;
                realMin1 = 0;
                realMax2 = 0;
                realMin2 = 0;
                currentAverage = 0;
                standardGroupTotal = 0;

                for (l = 0; l < pointNumInGroup; l++)
                {
                    v = dataInPointArray[index, l];
                    oneGroupTotal[0] += v;
                }
                currentAverage = oneGroupTotal[0] / pointNumInGroup;

                for (i = 0, j = 0; i < totalPointNumForChart; i++, j++)
                {
                    v = dataInPointArray[index, i] - currentAverage;
                    standardGroupTotal += v * v;

                    if (j == pointNumInGroup - 1)  //one group is OK
                    {
                        oneGroupAverage[k] = oneGroupTotal[k] / pointNumInGroup;
                        standardGroupDelta[k] = (float)System.Math.Sqrt(standardGroupTotal / (pointNumInGroup - 1));

                        allGroupTotal += oneGroupTotal[k];
                        allGroupDelta += standardGroupDelta[k];

                        standardGroupTotal = 0;
                        if (k + 1 < numOfGroupsInChart)
                        {
                            for (l = i + 1; l < pointNumInGroup + i + 1; l++)
                            {

                                //get average value for next group
                                oneGroupTotal[k + 1] += dataInPointArray[index, l];
                            }
                            currentAverage = oneGroupTotal[k + 1] / pointNumInGroup;
                        }

                        if (SPCType == gVariable.SPC_DATA_AND_CHART)
                        {
                            if (i == pointNumInGroup - 1)  //first time inside, initialize
                            {
                                realMax1 = oneGroupAverage[0];
                                realMin1 = oneGroupAverage[0];

                                realMax2 = standardGroupDelta[0];
                                realMin2 = standardGroupDelta[0];
                            }
                            else
                            {
                                if (oneGroupAverage[k] > realMax1)
                                    realMax1 = oneGroupAverage[k];
                                if (oneGroupAverage[k] < realMin1)
                                    realMin1 = oneGroupAverage[k];

                                if (standardGroupDelta[k] > realMax2)
                                    realMax2 = standardGroupDelta[k];
                                if (standardGroupDelta[k] < realMin2)
                                    realMin2 = standardGroupDelta[k];
                            }

                            chart.Series[0].Points.AddXY("11", oneGroupAverage[k]);
                            chart.Series[1].Points.AddXY("11", standardGroupDelta[k]);
                        }

                        k++;
                        j = -1;

                        if (k >= numOfGroupsInChart)
                            break;
                    }
                }

                allGroupTotalAverage = allGroupTotal / totalPointNumForChart;
                allGroupDeltaAverage = allGroupDelta / numOfGroupsInChart;

                //Xbar chart control values
                controlCenterValueArray[index, 0] = allGroupTotalAverage + vA3 * allGroupDeltaAverage;
                controlCenterValueArray[index, 1] = allGroupTotalAverage;
                controlCenterValueArray[index, 2] = allGroupTotalAverage - vA3 * allGroupDeltaAverage;
                //S chart control values
                controlCenterValueArray[index, 3] = vB4 * allGroupDeltaAverage;
                controlCenterValueArray[index, 4] = allGroupDeltaAverage;
                controlCenterValueArray[index, 5] = vB3 * allGroupDeltaAverage;

                if (SPCType == gVariable.SPC_DATA_AND_CHART)
                {
                    //these data are used to draw range of the chart, that is, larger than the largest data in chart
                    if (controlCenterValueArray[index, 0] > realMax1)
                        realMax1 = controlCenterValueArray[index, 0];
                    if (controlCenterValueArray[index, 2] < realMin1)
                        realMin1 = controlCenterValueArray[index, 2];

                    if (controlCenterValueArray[index, 3] > realMax2)
                        realMax2 = controlCenterValueArray[index, 3];
                    if (controlCenterValueArray[index, 5] < realMin2)
                        realMin2 = controlCenterValueArray[index, 5];

                    v = realMax1 - realMin1;
                    realMax1 += v / GAP_BETWEEN_MAX_AND_BOARD;
                    realMin1 -= v / GAP_BETWEEN_MAX_AND_BOARD;

                    v = realMax2 - realMin2;
                    realMax2 += v / GAP_BETWEEN_MAX_AND_BOARD;
                    realMin2 -= v / GAP_BETWEEN_MAX_AND_BOARD;

                    //when Maximum and Minimum are all 0, this chart will fail to be displayed, so we change this value to 0.001 and -0.001
                    if (realMax1 == 0 && realMin1 == 0)
                    {
                        chart.ChartAreas[0].AxisY.Maximum = 0.001;
                        chart.ChartAreas[0].AxisY.Minimum = -0.001;
                    }
                    else
                    {
                        chart.ChartAreas[0].AxisY.Maximum = realMax1;
                        chart.ChartAreas[0].AxisY.Minimum = realMin1;
                    }

                    if (realMax2 == 0 && realMin2 == 0)
                    {
                        chart.ChartAreas[1].AxisY.Maximum = 0.001;
                        chart.ChartAreas[1].AxisY.Minimum = -0.001;
                    }
                    else
                    {
                        chart.ChartAreas[1].AxisY.Maximum = realMax2;
                        chart.ChartAreas[1].AxisY.Minimum = realMin2;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("get_XBar_S_Chart() in spc function !" + ex);
                return -2;
            }
        }

        //index: for SPCCurve screen, there are many craft data items, index means craft index
        //       for spc checking function, there are many data items for a qulity data, index means item index for quality data
        //SPCType: SPC_DATA_GET_UCL_SCL, SPC_DATA_ONLY or SPC_DATA_AND_CHART
        public int get_C_Chart(int index, Chart chart, int SPCType, float[,] dataInPointArray, int[,] timeInPoint, float[,] controlCenterValueArray)
        {
            try
            {
                int i, v;
                int allGroupTotal;
                float max, min, delta;
                float totalAverage;

                allGroupTotal = 0;

                if (SPCType == gVariable.SPC_DATA_AND_CHART)
                {
                    foreach (var series in chart.Series)
                    {
                        series.Points.Clear();
                    }
                }

                max = dataInPointArray[index, 0];
                min = max;

                for (i = 0; i < totalPointNumForChart; i++)
                {
                    v = (int)dataInPointArray[index, i];
                    allGroupTotal += v;

                    if (v > max)
                        max = v;
                    if (v < min)
                        min = v;

                    if (SPCType == gVariable.SPC_DATA_AND_CHART)
                    {
                        chart.Series[0].Points.AddXY("11", v);
                    }
                }

                totalAverage = allGroupTotal / totalPointNumForChart;

                controlCenterValueArray[index, 0] = totalAverage + 3 * (float)System.Math.Sqrt(totalAverage);
                controlCenterValueArray[index, 1] = totalAverage;
                controlCenterValueArray[index, 2] = totalAverage - 3 * (float)System.Math.Sqrt(totalAverage);

                if (SPCType == gVariable.SPC_DATA_AND_CHART)
                {
                    if (controlCenterValueArray[index, 0] > max)
                        max = controlCenterValueArray[index, 0];
                    if (controlCenterValueArray[index, 2] < min)
                        min = controlCenterValueArray[index, 2];

                    delta = max - min;
                    max += delta / 10;
                    min -= delta / 10;

                    //when Maximum and Minimum are all 0, this chart will fail to be displayed, so we change this value to 0.001 and -0.001
                    if (max == 0 && min == 0)
                    {
                        chart.ChartAreas[0].AxisY.Maximum = 0.001;
                        chart.ChartAreas[0].AxisY.Minimum = -0.001;
                    }
                    else
                    {
                        chart.ChartAreas[0].AxisY.Maximum = max;
                        chart.ChartAreas[0].AxisY.Minimum = min;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("get_XBar_S_Chart() in spc function !" + ex);
                return -2;
            }
        }

        public void get_XMed_R_Chart(int index, Chart chart)
        {
            try
            { 
                int i, j, k;
                int num;
                int currentDataNumForCurve;
                float[] oneGroupData = new float[pointNumInGroup];  //for one group of data
                float[] oneGroupTotal = new float[numOfGroupsInChart];  //total value for a group 
                float[] oneGroupMed = new float[numOfGroupsInChart];   //med value for a group
                float[] oneGroupDelta = new float[numOfGroupsInChart];  //R value for a group
                float v, vMin, vMax;
                float realMax1, realMin1, realMax2, realMin2;
                float allGroupMed, allGroupDelta, allGroupTotalMed, allGroupDeltaAverage;
                
                vMax = 0;
                vMin = 0;
                allGroupMed = 0;
                allGroupDelta = 0;
                
                for (i = 0; i < numOfGroupsInChart; i++)
                {
                    oneGroupTotal[i] = 0;
                }
                
                currentDataNumForCurve = gVariable.dataNumForCurve[index];
                
                foreach (var series in chart.Series)
                {
                    series.Points.Clear();
                }
                
                if (totalPointNumForChart < currentDataNumForCurve)
                    num = totalPointNumForChart;
                else
                    num = currentDataNumForCurve;
                
                k = 0;
                realMax1 = 0;
                realMin1 = 0;
                realMax2 = 0;
                realMin2 = 0;
                
                for (i = 0, j = 0; i < num; i++, j++)
                {
                    v = gVariable.dataInPoint[index, i];
                    oneGroupTotal[k] += v;
                
                    oneGroupData[j] = v;
                
                    if (j == 0)
                    {
                        vMax = v;
                        vMin = v;
                    }
                    else
                    {
                        if (v > vMax)
                            vMax = v;
                        if (v < vMin)
                            vMin = v;
                    }
                
                    if (j == pointNumInGroup - 1)  //one group is OK
                    {
                        oneGroupMed[k] = sortForMedValue(oneGroupData);
                        oneGroupDelta[k] = vMax - vMin;
                
                        if (i == pointNumInGroup - 1)  //first time inside, initialize
                        {
                            realMax1 = oneGroupMed[0];
                            realMin1 = oneGroupMed[0];
                
                            realMax2 = oneGroupDelta[0];
                            realMin2 = oneGroupDelta[0];
                        }
                        else
                        {
                            if (oneGroupMed[k] > realMax1)
                                realMax1 = oneGroupMed[k];
                            if (oneGroupMed[k] < realMin1)
                                realMin1 = oneGroupMed[k];
                
                            if (oneGroupDelta[k] > realMax2)
                                realMax2 = oneGroupDelta[k];
                            if (oneGroupDelta[k] < realMin2)
                                realMin2 = oneGroupDelta[k];
                        }
                
                        allGroupMed += oneGroupMed[k];
                        allGroupDelta += oneGroupDelta[k];
                
                        chart.Series[0].Points.AddXY("11", oneGroupMed[k]);
                        chart.Series[1].Points.AddXY("11", oneGroupDelta[k]);
                
                        k++;
                        j = -1;
                
                        if (k >= numOfGroupsInChart)
                            break;
                    }
                }
                
                allGroupTotalMed = allGroupMed / numOfGroupsInChart;
                allGroupDeltaAverage = allGroupDelta / numOfGroupsInChart;
                
                //XMed chart control values
                gVariable.SPCControlValue[index, 0] = allGroupTotalMed + vA4 * allGroupDeltaAverage;  //Xmed + A4 * RBar
                gVariable.SPCControlValue[index, 1] = allGroupTotalMed;
                gVariable.SPCControlValue[index, 2] = allGroupTotalMed - vA4 * allGroupDeltaAverage;
                //R chart control values
                gVariable.SPCControlValue[index, 3] = vD4 * allGroupDeltaAverage;  
                gVariable.SPCControlValue[index, 4] = allGroupDeltaAverage;
                gVariable.SPCControlValue[index, 5] = vD3 * allGroupDeltaAverage;
                
                if (gVariable.SPCControlValue[index, 0] > realMax1)
                    realMax1 = gVariable.SPCControlValue[index, 0];
                if (gVariable.SPCControlValue[index, 2] < realMin1)
                    realMin1 = gVariable.SPCControlValue[index, 2];
                
                if (gVariable.SPCControlValue[index, 3] > realMax2)
                    realMax2 = gVariable.SPCControlValue[index, 3];
                if (gVariable.SPCControlValue[index, 5] < realMin2)
                    realMin2 = gVariable.SPCControlValue[index, 5];
                
                v = realMax1 - realMin1;
                realMax1 += v / GAP_BETWEEN_MAX_AND_BOARD;
                realMin1 -= v / GAP_BETWEEN_MAX_AND_BOARD;
                
                v = realMax2 - realMin2;
                realMax2 += v / GAP_BETWEEN_MAX_AND_BOARD;
                realMin2 -= v / GAP_BETWEEN_MAX_AND_BOARD;

                //when Maximum and Minimum are all 0, this chart will fail to be displayed, so we change this value to 0.001 and -0.001
                if (realMax1 == 0 && realMin1 == 0)
                {
                    chart.ChartAreas[0].AxisY.Maximum = 0.001;
                    chart.ChartAreas[0].AxisY.Minimum = -0.001;
                }
                else
                {
                    chart.ChartAreas[0].AxisY.Maximum = realMax1;
                    chart.ChartAreas[0].AxisY.Minimum = realMin1;
                }

                if (realMax2 == 0 && realMin2 == 0)
                {
                    chart.ChartAreas[1].AxisY.Maximum = 0.001;
                    chart.ChartAreas[1].AxisY.Minimum = -0.001;
                }
                else
                {
                    chart.ChartAreas[1].AxisY.Maximum = realMax2;
                    chart.ChartAreas[1].AxisY.Minimum = realMin2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("get_XMed_R_Chart() in spc function !" + ex);
            }
        }

        //there is no real concept of group in this curve, one data is a group and totally 25 groups
        public void get_X_Rm_Chart(int index, Chart chart)
        {
            try
            { 
                int i;
                int num;
                int currentDataNumForCurve;
                float[] oneData = new float[numOfGroupsInChart];
                float[] oneDataDelta = new float[numOfGroupsInChart];
                float v;
                float realMax1, realMin1, realMax2, realMin2;
                float allDataValue, allDataDelta, allDataAverage, allDataDeltaAverage;
                
                allDataValue = 0;
                allDataDelta = 0;
                
                currentDataNumForCurve = gVariable.dataNumForCurve[index];
                
                foreach (var series in chart.Series)
                {
                    series.Points.Clear();
                }
                
                if (totalPointNumForChart < currentDataNumForCurve)
                    num = totalPointNumForChart;
                else
                    num = currentDataNumForCurve;
                
                realMax1 = 0;
                realMin1 = 0;
                realMax2 = 0;
                realMin2 = 0;
                v = 0;
                
                for (i = 0; i < num; i++)
                {
                    if(i == 0)
                    {
                        v = gVariable.dataInPoint[index, 0];
                
                        realMax1 = v;
                        realMin1 = v;
                    }
                    else
                    {
                        if (gVariable.dataInPoint[index, i] > v)
                            oneDataDelta[i] = gVariable.dataInPoint[index, i] - v;
                        else
                            oneDataDelta[i] = v - gVariable.dataInPoint[index, i];
                
                        v = gVariable.dataInPoint[index, i];
                
                        if (v > realMax1)
                            realMax1 = v;
                        if (v < realMin1)
                            realMin1 = v;
                
                        if (i == 1)
                        {
                            realMax2 = oneDataDelta[i];
                            realMin2 = oneDataDelta[i];
                        }
                        else
                        {
                            if (oneDataDelta[i] > realMax2)
                                realMax2 = oneDataDelta[i];
                            if (oneDataDelta[i] < realMin2)
                                realMin2 = oneDataDelta[i];
                        }
                    }
                    allDataValue += v;
                    allDataDelta += oneDataDelta[i];
                
                    chart.Series[0].Points.AddXY("11", v);
                    if (i == 0)
                        chart.Series[1].Points.AddXY("11", "");
                    else
                        chart.Series[1].Points.AddXY("11", oneDataDelta[i]);
                }
                
                allDataAverage = allDataValue / numOfGroupsInChart;
                allDataDeltaAverage = allDataDelta / numOfGroupsInChart;
                
                //X chart control values
                gVariable.SPCControlValue[index, 0] = allDataAverage + vE2 * allDataDeltaAverage;  //Xbar + vE2 * RBar
                gVariable.SPCControlValue[index, 1] = allDataAverage;
                gVariable.SPCControlValue[index, 2] = allDataAverage - vE2 * allDataDeltaAverage;
                //R chart control values
                gVariable.SPCControlValue[index, 3] = vD4 * allDataDeltaAverage;
                gVariable.SPCControlValue[index, 4] = allDataDeltaAverage;
                gVariable.SPCControlValue[index, 5] = vD3 * allDataDeltaAverage;
                
                if (gVariable.SPCControlValue[index, 0] > realMax1)
                    realMax1 = gVariable.SPCControlValue[index, 0];
                if (gVariable.SPCControlValue[index, 2] < realMin1)
                    realMin1 = gVariable.SPCControlValue[index, 2];
                
                if (gVariable.SPCControlValue[index, 3] > realMax2)
                    realMax2 = gVariable.SPCControlValue[index, 3];
                if (gVariable.SPCControlValue[index, 5] < realMin2)
                    realMin2 = gVariable.SPCControlValue[index, 5];
                
                v = realMax1 - realMin1;
                realMax1 += v / GAP_BETWEEN_MAX_AND_BOARD;
                realMin1 -= v / GAP_BETWEEN_MAX_AND_BOARD;
                
                v = realMax2 - realMin2;
                realMax2 += v / GAP_BETWEEN_MAX_AND_BOARD;
                realMin2 -= v / GAP_BETWEEN_MAX_AND_BOARD;

                //when Maximum and Minimum are all 0, this chart will fail to be displayed, so we change this value to 0.001 and -0.001
                if (realMax1 == 0 && realMin1 == 0)
                {
                    chart.ChartAreas[0].AxisY.Maximum = 0.001;
                    chart.ChartAreas[0].AxisY.Minimum = -0.001;
                }
                else
                {
                    chart.ChartAreas[0].AxisY.Maximum = realMax1;
                    chart.ChartAreas[0].AxisY.Minimum = realMin1;
                }

                if (realMax2 == 0 && realMin2 == 0)
                {
                    chart.ChartAreas[1].AxisY.Maximum = 0.001;
                    chart.ChartAreas[1].AxisY.Minimum = -0.001;
                }
                else
                {
                    chart.ChartAreas[1].AxisY.Maximum = realMax2;
                    chart.ChartAreas[1].AxisY.Minimum = realMin2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("get_X_Rm_Chart() in spc function !" + ex);
            }
        }

        private float sortForMedValue(float[] list)
        {
            try
            { 
                int i, j, len, pos;
                float temp;
                bool done = false;
                
                j = 1;
                len = list.Length;
                pos = len / 2;
                
                while ((j < len) && (!done))
                {
                    done = true;
                    for (i = 0; i < len - j; i++)
                    {
                        if (list[i] > list[i + 1])
                        {
                            done = false;
                            temp = list[i];
                            list[i] = list[i + 1];
                            list[i + 1] = temp;
                        }
                    }
                    j++;
                }
                
                return list[pos];
            }
            catch (Exception ex)
            {
                Console.WriteLine("sortForMedValue() in spc function !" + ex);
                return 0;
            }
        }

    }
}
