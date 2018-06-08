using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabelPrint.Label;
using LabelPrint.Data;

namespace LabelPrint.Receipt
{

    /*
     * 
     * 分切标签 （大、小标签） ：
     *大标签包含的栏位内容：客户名称、原材料代码、产品批号、材料名称、宽度、生产者、生产日期、卷重/卷长、卷号；
     *小标签包含的栏位内容：客户名称、原材料代码、产品批号、材料名称、宽度、生产者、生产日期、卷重/卷长、卷号、分卷号；
     * 
     */




    public class CutLabel: ReceiptPrintPattern
    {

        //List<PrintInfo> PrintInfo;
        public CutLabel()
        {

        }
        //List<PrintInfo> GetPrintLabelFromJsonFile()
        //{
        //    Receipt1 a = new Receipt1();
        //     PrintInfo = Receipt1.printProductLabel(null, 400, 400);
        //    return PrintInfo;
        //}


        public void  Printlabel(CutUserinputData userInput)
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
