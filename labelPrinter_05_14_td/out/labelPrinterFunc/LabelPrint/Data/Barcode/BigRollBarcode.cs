using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.Data
{
    public class BigRollBarcode : RollBarcode
    {
        //流延工序：
        //XXXXXXXXXXX(工单编码)+X（工序）+X（机台号）+XXXXXXXXXX（日期）+XX（卷号）；



        public String BigRollQality;
        public String createBarcode()
        {
            //  if (WorkNoStr.Length != WorkNoStrLen)
            //    return null;
            //      if (ProcessStr.Length != ProcessStrLen)
            //          return null;
            //     if (MachineIDStr.Length != MachineIDStrLen)
            //         return null;
            if (TimeStr.Length != TimeStrLen)
                return null;
            if (BigRStr.Length != BigRStrLen)
                return null;
            //if (LittleRStr.Length != LittleRStrLen)
            //    return null;
            //if (VendorStr.Length != VendorStrLen)
            //    return null;
            //if (QAStr.Length != QAStrLen)
            //    return null;
            String barcode = WorkNoStr + TimeStr + BigRStr + QAStr;
            return barcode;
        }


        public Boolean ParseBarcode(String str)
        {
            if (str.Length != getTotalStrLen())
                return false;
            try
            {
                WorkNoStr = str.Substring(getWorkNoStrIndex(), WorkNoStrLen);

                TimeStr = str.Substring(getTimeStrIndex(), TimeStrLen);
                BigRStr = str.Substring(getBigRStrIndex(), BigRStrLen);
                ParserWorNo();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }


        public static int getBigRQAStrIndex()
        {
            int index = getBigRStrIndex() + BigRStrLen;
            return index;
        }
        public static int getTotalStrLen()
        {
            int index = getBigRQAStrIndex() + QAStrLen;
            return index;
        }
    }
}
