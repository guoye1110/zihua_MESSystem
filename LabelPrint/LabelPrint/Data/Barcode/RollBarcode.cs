using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LabelPrint.Util;
namespace LabelPrint.Data
{
    public class RollBarcode
    {
        public const int WorkNoStrLen = 12;
        public const int TimeStrLen = 4;
        public const int BigRStrLen = 3;
        public const int LittleRStrLen = 2;
        public const int VendorStrLen = 1;
        public const int QAStrLen = 1;



        public String WorkNoStr;
        public String TimeStr;
        public String BigRStr;
        public String QAStr;

        public String BatchNo;




        /// <summary>
        /// Big Roll
        /// </summary>
        public String BigRollQality;

        /// <summary>
        /// Little Roll
        /// </summary>
        public String LittleRStr;
        public String VendorStr;


        public Boolean Valid = false;
        public RollBarcode()
        {
            WorkNoStr = null;
            TimeStr = null;
            BigRStr = null;
            QAStr = null;
            BatchNo = null;
        }


        public Boolean ParserWorNo()
        {
            try
            {
                //String order = WorkNoStr.Substring(5, 2);
                //String LiuYanDevice = WorkNoStr.Substring(4, 1);
                //String YearMonth = TimeStr.Substring(0, 4);
                String WorkNoSn;
                String DeviceNo;
             //   BatchNo = YearMonth + LiuYanDevice + order;



                DeviceNo = WorkNoStr.Substring(11, 1);
                WorkNoSn = WorkNoStr.Substring(5, 2);


                BatchNo = WorkNoStr.Substring(0, 7);
                //   SalesOrder = WorkNoStr.Substring(0, 7);
            }
            catch (Exception ex)
            {
                Log.d("RollBarcode", ex.Message);
                return false;
            }

            return true;
        }

        //WorkDate = DateTime.Now.ToString("yyyy-MM-dd");
        //WorkTime = DateTime.Now.ToString("HH:mm:ss");
        public static String CreateDateTimeforBarcode(String date, String time)
        {
            String str = "";

            //str += date.Substring(8, 2);
            str += time.Substring(0, 2);
            str += time.Substring(3, 2);
            return str;
        }

        /*
         * 工单编码：1804306121L3 含义： 121：4月12号 日班  L3：三号流延机
            工单编码：1804306122Y1 含义： 122：4月12号 夜班  Y1：一号印刷机
         */
        public static Boolean ParseWorkNo(String workno, out String batchNo, out String DeviceNo, out String WorkNoSn)
        {
            batchNo = workno.Substring(0, 7);
            DeviceNo = workno.Substring(11, 1);
            WorkNoSn = workno.Substring(5, 2);
            return true;
        }


        public static int getWorkNoStrIndex()
        {
            return 0;
        }

        public static int getTimeStrIndex()
        {
            int index = getWorkNoStrIndex() + WorkNoStrLen;
            return index;
        }
        public static int getBigRStrIndex()
        {
            int index = getTimeStrIndex() + TimeStrLen;
            return index;
        }
    }
}
