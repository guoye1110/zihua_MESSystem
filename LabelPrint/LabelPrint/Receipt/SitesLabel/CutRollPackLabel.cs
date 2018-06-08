using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LabelPrint.Data;
namespace LabelPrint.Receipt
{
    class CutRollPackLabel: ReceiptPrintPattern
    {
        public CutRollPackLabel()
        {

        }
        public void Printlabel(CutUserinputData userInput)
        {
            CutSampleData SysData;
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            SysData = CutSampleData.Instance;

            GetCutPackLabelFromJsonFile();
            DynamicPrintLabelData PrintData = new DynamicPrintLabelData();
            userInput.UpdateCutPackPrintPrintData(PrintData);
            UpdateReceiptPatternDynamicPrintData(PrintInfo, PrintData);

            // SysSetting.DynPrintData.QRBarCode = "Hello";

            PrintReceipt();
        }
    }
}
