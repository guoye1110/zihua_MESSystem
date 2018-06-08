using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabelPrint.Util;
namespace LabelPrint.Data
{
    public class LittleRollBarcode : RollBarcode
    {


        public const int TotalLen = WorkNoStrLen + TimeStrLen + BigRStrLen + LittleRStrLen + VendorStrLen + QAStrLen;

        public LittleRollBarcode()
        {
        }


        public LittleRollBarcode(String barcode)
        {
            ParseBarcode(barcode);
        }

        public Boolean ParseBarcode(String str)
        {
            if (str.Length != getTotalStrLen())
                return false;
            try
            {
                WorkNoStr = str.Substring(getWorkNoStrIndex(), WorkNoStrLen);
                //ProcessStr = str.Substring(getProcessStrIndex(), ProcessStrLen);
                //MachineIDStr = str.Substring(getMachineIDSrIndex(), MachineIDStrLen);
                TimeStr = str.Substring(getTimeStrIndex(), TimeStrLen);
                BigRStr = str.Substring(getBigRStrIndex(), BigRStrLen);
                LittleRStr = str.Substring(getLittleRStrIndex(), LittleRStrLen);
                VendorStr = str.Substring(getVendorStrIndex(), VendorStrLen);
                QAStr = str.Substring(getQAStrIndex(), QAStrLen);
                ParserWorNo();
                Valid = true;
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
                return false;
            }
            return true;
        }

        public String createBarcode()
        {
            if (WorkNoStr.Length != WorkNoStrLen)
                return null;
            //            if (ProcessStr.Length != ProcessStrLen)
            //                return null;
            //            if (MachineIDStr.Length != MachineIDStrLen)
            //                return null;
            if (TimeStr.Length != TimeStrLen)
                return null;
            if (BigRStr.Length != BigRStrLen)
                return null;
            if (LittleRStr.Length != LittleRStrLen)
                return null;
            if (VendorStr.Length != VendorStrLen)
                return null;
            if (QAStr.Length != QAStrLen)
                return null;
            String barcode = WorkNoStr + TimeStr + BigRStr + LittleRStr + VendorStr + QAStr;
            Valid = true;
            return barcode;
        }


        public static int getLittleRStrIndex()
        {
            int index = getBigRStrIndex() + BigRStrLen;
            return index;
        }
        public static int getVendorStrIndex()
        {
            int index = getLittleRStrIndex() + LittleRStrLen;
            return index;
        }
        public static int getQAStrIndex()
        {
            int index = getVendorStrIndex() + VendorStrLen;
            return index;
        }
        public static int getTotalStrLen()
        {
            int index = getQAStrIndex() + QAStrLen;
            return index;
        }
    }
}
