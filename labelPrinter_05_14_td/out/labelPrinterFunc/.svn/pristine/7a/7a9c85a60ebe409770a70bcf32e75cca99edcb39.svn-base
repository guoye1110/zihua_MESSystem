using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabelPrint.Label;
using LabelPrint.Data;
namespace LabelPrint.Receipt
{
    class LiuYanLabel : ReceiptPrintPattern
    {
        //public class LiuYanUserinputData
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
        public void Printlabel(LiuYanUserinputData userInput)
        {
            CutSampleData SysData;
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            SysData = CutSampleData.Instance;

            GetPrintLabelFromJsonFile();
            DynamicPrintLabelData PrintData = new DynamicPrintLabelData();
            userInput.UpdatePrintPrintData(PrintData);
            UpdateReceiptPatternDynamicPrintData(PrintInfo, PrintData);

            //SysSetting.DynPrintData.QRBarCode = "Hello";

            PrintReceipt();
        }
    }
}
