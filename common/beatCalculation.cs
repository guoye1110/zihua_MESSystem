using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;

namespace MESSystem.common
{
    public class beatCalculation
    {
        const int maxMachineNum = 200;

        static float[] beatThreshold = 
                {
	                1.5f,  10f,4.44f, 10.5f, 5.0f,  8f,  6f,  20f,  15f, 0.9f,   //1 - 10
	                0.9f, 0.5f, 2.0f, 0.5f, 15f,    3f,   5f,  5f,    3f,   8f,  //11 - 20
	                2.5f, 0.5f, 0.5f,   1f, 0.5f, 1.3f, 0.5f, 1.0f, 0.5f, 1.0f,  //21 - 30
	                1.5f, 2.9f, 1.0f, 3.0f, 0.5f, 2.0f, 1.5f, 0.5f, 0.5f, 1.5f,  //31 - 40
	                2.0f, 2.0f, 0.5f,   1f,  20f, 0.5f, 0.5f,  5.0f, 0.5f, 0.6f, //41 - 50 
	                0.5f,   1f, 0.5f, 2.0f, 2.0f,  3f,   2f, 0.8f,   2f, 0.8f,   //51 - 60
	                0.5f,  17f, 0.5f, 2.0f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,  4f,   //61 - 70
	                2.5f,  0.5f, 0.5f, 2f,   0.5f, 0.5f, 0.5f, 3f, 3.5f,  1f,    //71 - 80
	                0.5f, 1f, 0.5f, 1.5f, 0.5f, 10.3f, 0.5f, 0.5f, 1.5f, 0.5f,   //81 - 90
	                1f, 0.5f, 2f, 0.5f, 0.5f,    2f, 0.5f, 3.0f, 8.0f, 0.5f,     //91 - 100
	                0.5f, 10.2f, 0.5f, 3f, 5f, 0.5f,   5f, 0.5f, 0.5f, 0.5f,     //101 - 110
	                1.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.9f, 1f, 2f,   3f, 2f,        //111 - 120 
	                2f,   0.5f, 0.5f, 17.8f,0.5f, 0.5f, 3f, 0.5f, 6f,  1f,       //121 - 130
	                3f,   0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,    //131 - 140
	                0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,    //141 - 150
	                0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,    //151 - 160
	                0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,    //161 - 170
	                0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,    //171 - 180
	                0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,    //181 - 190
	                0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,    //191 - 200
                };

        static int[] beatHighPeriod = 
                {
         	        40, 10, 30, 5, 5, 20, 20, 30, 10, 5,          //1 - 10   
                    5, 5, 30, 5, 8, 5, 20, 20, 10, 20,          //11 - 20  
	                40, 5, 5, 40, 5, 30, 5, 10, 5, 200,           //21 - 30  
	                200, 10, 200, 100, 5, 100, 100, 5, 5, 50,     //31 - 40  
	                50, 20, 5, 20, 7, 5, 5, 10, 5, 20,            //41 - 50  
	                5, 20, 5, 100, 5, 20, 20, 20, 30, 20,         //51 - 60  
	                5, 20, 5, 10, 5, 5, 5, 5, 5, 30,             //61 - 70  
	                100, 5, 5, 20, 5, 5, 5, 40, 3000, 40,        //71 - 80  
	                5, 40, 5, 10, 5, 20, 5, 5, 50, 5,             //81 - 90  
	                40, 5, 40, 5, 5, 40, 5, 20, 10, 5,            //91 - 100 
	                5, 8, 5, 40, 5, 5, 20, 5, 5, 5,                //101 - 110
  	                10, 5, 5, 5, 10, 5, 10, 20, 60, 20,         //111 - 120
    	            5, 5, 5, 10, 5, 5, 40, 5, 40, 20,             //121 - 130
      	            40, 5, 5, 5, 5, 5, 5, 5, 5, 5,              //131 - 140
      	            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,              //141 - 150
      	            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,              //151 - 160
      	            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,              //161 - 170
      	            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,              //171 - 180
      	            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,              //181 - 190
      	            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,              //191 - 200
                };

        static int[] beatLowPeriod = 
                {
        	        -20, -20, -30, -5,    -5, -20, -50, -20, -20, -5,      //1 - 10    
	                -40, -5, -20, -5, -10, -20, -70, -60, -5, -20,         //11 - 20  
	                -5, -5, -5, -20, -5,   -20, -5, -10, -5, -20,          //21 - 30  
	                -10, -10, -20, -20, -5,   -10, -10, -5, -5, -20,       //31 - 40  
	                -20, -20, -5, -10, -20,   -5, -5, -20, -5, -10,        //41 - 50  
	                -5, -10, -5, -10, -5,   -10, -5, -10, -30, -10,        //51 - 60  
	                -5, -20, -5, -10, -5,   -5, -5, -5, -5, -80,          //61 - 70  
	                -25, -5, -5, -20, -5,   -5, -5, -40, -50, -6,          //71 - 80  
	                -5, -40, -5, -100, -5,   -15, -5, -5, -20, -5,         //81 - 90  
	                -40, -5, -40, -5, -5,   -40, -5, -20, -10, -5,         //91 - 100 
	                -5, -30, -5, -40, -5,   -5, -20, -5, -5, -5,            //101 - 110
  	                -100, -5, -5, -5, -10,   -5, -10, -20, -30, -20,     //111 - 120
    	            -5, -5, -5, -20, -5,   -5, -40, -5, -5, -20,           //121 - 130
      	            -40, -5, -5, -5, -5,   -5, -5, -5, -5, -5,           //131 - 140
      	            -5,  -5, -5, -5, -5,   -5, -5, -5, -5, -5,           //141 - 150
      	            -5,  -5, -5, -5, -5,   -5, -5, -5, -5, -5,           //151 - 160
      	            -5,  -5, -5, -5, -5,   -5, -5, -5, -5, -5,           //161 - 170
      	            -5,  -5, -5, -5, -5,   -5, -5, -5, -5, -5,           //171 - 180
      	            -5,  -5, -5, -5, -5,   -5, -5, -5, -5, -5,           //181 - 190
      	            -5,  -5, -5, -5, -5,   -5, -5, -5, -5, -5,           //191 - 200
                };

        //for beat calculation
        public static int[] beatNum = new int[maxMachineNum + 1];  // number of beats for this dispatch by now

        public int firstBeatStatus;  //we need find a rising edge to start beat counting, if the first edge is a falling edge, we should ignore it, and wait for next rising edge
        public int beatDirection;  //larger than threshold or lower than threshold, if larger, howm many point keeps on larger, and if lower, how many point keeps on lower
        public int beatStatus;  // 0 means low level, 1 means high level 
        public int beatReadingNum;  //number of points for this level by now, if we change level this value will start from 0 again
        public int beatPeriodPoint;  //number of points for this beat, and if beatPeriodPoint * 4 / 10, we got beat period of time

        public void beatInitBVariables(int myBoardIndex)
        {
            firstBeatStatus = -1;
            beatDirection = 0;
            beatStatus = 0;
            beatReadingNum = 0;
            beatNum[myBoardIndex] = 0;
            beatPeriodPoint = 0;
        }

        public static void beatModifyThresholds()
        {

        }


        //input: machine index, 
        //       current value, 
        //       number of readings after dispatch started
        //return: > 0, beat time value counted by second, beat completed, 
        //        = 0, beat not finished, but the machine is working
        //        = -1, machine is in idle mode 
        public int beatDataInput(int myBoardIndex, float fData, int dataIndex)
        {
            int ret;
            int beatPeriodTime;

            ret = -1;
            try
            {
                if (fData > beatThreshold[myBoardIndex])
                {
                    if (beatDirection > 0)
                        beatDirection++;
                    else
                        beatDirection = 1;

                    ret = 0;
                }
                else
                {
                    if (beatDirection > 0)
                        beatDirection = -1;
                    else
                        beatDirection--;

                    ret = -1;
                }

                beatReadingNum++;

                if (firstBeatStatus == -1)
                {
                    //if we got first edge, start counting. The first edge condition is 5 points, easy to achieve 
                    if (beatDirection >= 5)
                        firstBeatStatus = 1;
                    else if (beatDirection <= -5)
                        firstBeatStatus = 0;
                }

                if (beatStatus == 0)
                {
                    //we got a rising edge
                    if (beatDirection >= beatHighPeriod[myBoardIndex])
                    {
                        beatStatus = 1;
                        beatPeriodPoint += beatReadingNum;
                        beatReadingNum = 0;
                        Console.WriteLine(DateTime.Now.ToString() + "a rising edge: " + dataIndex);
                    }
                }
                else
                {
                    if (beatDirection <= beatLowPeriod[myBoardIndex])
                    {
                        //this is a falling edge, noramlly a beat completed
                        beatStatus = 0;
                        if (firstBeatStatus == 0)
                        {
                            beatPeriodPoint += beatReadingNum;
                            beatNum[myBoardIndex]++;
                            beatPeriodTime = beatPeriodPoint * 4 / 10;
                            beatPeriodPoint = 0;

                            Console.WriteLine(DateTime.Now.ToString() + "a falling edge: " + dataIndex);

                            ret = beatPeriodTime;  //a beat completed
                        }
                        else
                        {
                            //we start from high current and it is the first time of low current, we need to start from here for beat counting and igore this falling edge
                            firstBeatStatus = 0;
                            Console.WriteLine(DateTime.Now.ToString() + "a futile falling edge: " + dataIndex);
                        }
                        beatReadingNum = 0;
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine("beatDataInput failed! " + ex);
                return ret;
            }
        }
    }
}
