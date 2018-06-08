using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LabelPrint.Data;
namespace LabelPrint.Receipt
{
    class RukuLabel : ReceiptPrintPattern
    {
        //public void Printlabel()
        //{
        //    CutSampleData SysData;
        //    SystemSetting SysSetting;
        //    SysSetting = GlobalConfig.Setting;
        //    //CutProductItem ProdItem;
        //    SysData = CutSampleData.Instance;

        //    ReceiptPrintPattern receipt = ReceiptCreator.CreateReceipt();
        //    receipt.PrintReceipt();
        //}
        public void Printlabel(RuKuInputData userInput)
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
