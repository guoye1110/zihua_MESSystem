using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabelPrint.Data;
namespace LabelPrint.Receipt
{

    class OutBoundingLabel : ReceiptPrintPattern
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
        public void Printlabel(OutBoundingInputData userInput)
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
