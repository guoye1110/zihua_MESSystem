using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabelPrint.Data;
namespace LabelPrint.Receipt
{
    class QulityCheckLabel : ReceiptPrintPattern
    {

        //public void Printlabel()
        //{
        //    CutSampleData SysData;
        //    SystemSetting SysSetting;
        //    SysSetting = GlobalConfig.Setting;
        //    //CutProductItem ProdItem;
        //    SysData = CutSampleData.Instance;
        //    //CutProductItem Item = SysData.GetCurProductItem();
        //    // String str = SmallRollLabel.CreateLittleLabelString(Item.ProductCode[0], "1", SysSetting.CurSettingInfo.CutMachineNo, "Datetime", "1", "088", "087");
        //    //CreateLittleLabelString(String productIndex, String workProcess, String machineNo, String dataTime, String workOrder, String rollNo, String littleRollNo)


        //    ReceiptPrintPattern receipt = ReceiptCreator.CreateReceipt();
        //    //ReceiptPrintPattern a= new ReceiptPrintPattern();

        //    //SysSetting.DynPrintData.QRBarCode = str;

        //    //User may have input.
        //    receipt.PrintReceipt();
        //}
        public void Printlabel(QAUserinputData userInput)
        {
            SystemSetting SysSetting = GlobalConfig.Setting; 
    
            GetPrintLabelFromJsonFile();
            DynamicPrintLabelData PrintData = new DynamicPrintLabelData();
            userInput.UpdatePrintPrintData(PrintData);
            UpdateReceiptPatternDynamicPrintData(PrintInfo, PrintData);

           // SysSetting.DynPrintData.QRBarCode = "Hello";

            PrintReceipt();
        }
    }
}
