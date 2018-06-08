using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint
{
    public  class CommonFormHelper
    {

        public static String GetLittleRollNoStr(int bigno, int littleno)
        {
            //String bigStr = null;
            //String littleStr = null;
            //bigStr = string.Format("{0:D3}", bigno);
            //littleStr = string.Format("{0:D2}", littleno);
            //return bigStr + '-' + littleStr;
           // littleno++; 
            return string.Format("{0:D2}", littleno);
        }

        public static String GetBigRollNoStr(int bigno)
        {
            return  string.Format("{0:D3}", bigno);
        }
        public static String UpdateBigRollNo(String BigRollNoStr)
        {
            int bigno = 0;
            String OutBigNoStr;
            bool result = int.TryParse(BigRollNoStr, out bigno);
            if (result)
            {
                bigno++;
                OutBigNoStr =  string.Format("{0:D3}", bigno);
            }
            else
            {
                OutBigNoStr = BigRollNoStr;
            }
            return OutBigNoStr;
        }
        public static String UpdateLittleRollNo(String LittleRollNoStr)
        {
            int little = 0;
            String OutLittleNoStr;
            bool result = int.TryParse(LittleRollNoStr, out little);
            if (result)
            {
                little++;
                OutLittleNoStr = string.Format("{0:D2}", little);
            }
            else
            {
                OutLittleNoStr = LittleRollNoStr;
            }
            return OutLittleNoStr;
        }
    }
}
