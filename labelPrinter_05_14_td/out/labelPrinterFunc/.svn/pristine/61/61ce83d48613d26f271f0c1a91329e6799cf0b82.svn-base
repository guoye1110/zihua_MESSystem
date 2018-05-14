using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabelPrint.Label;
using LabelPrint.Data;
using System.IO;

namespace LabelPrint.Receipt
{
    class FilmPrintLabel : ReceiptPrintPattern
    {
        //public class FilmPrintUserinputData
        //{
        //    public String CurProductState;// = GetProductState();
        //    public String CurProductQuality;// = GetProductQuality();
        //    public String CurShowRealWeight;// = GetShowRealWeight();
        //    public String CurDesc;
        //    public String CurBigRollNo;
        //    public String CurLittleRollNo;
        //    public int BigRollNo = 1;
        //    public int LittleRoleNo = 1;
        //    public int TotalRoll = 0;
        //    public PlateInfo CurPlatInfo;

        //}


        String[] GetJsonFileUnderPath(String path)
        {
            
            int i = 0;
            DirectoryInfo root = new DirectoryInfo(path);
           int len = root.GetFiles().Length;
            String[] strs = new String[len];
            foreach (FileInfo f in root.GetFiles())
            {
                if (f.Name.IndexOf(".json")!=-1)
                {
                    strs[i] = f.Name;
                    i++;
                }
            }
            return strs;
        }



        public void Printlabel(FilmPrintUserinputData userInput)
        {
            CutSampleData SysData;
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            SysData = CutSampleData.Instance;

            GetPrintLabelFromJsonFile();
            DynamicPrintLabelData PrintData = new DynamicPrintLabelData();
            userInput.UpdatePrintPrintData(PrintData);

            UpdateReceiptPatternDynamicPrintData(PrintInfo, PrintData);

           // SysSetting.DynPrintData.QRBarCode = "Hello";

            PrintReceipt();
        }
    }
}
