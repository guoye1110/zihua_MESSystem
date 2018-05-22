using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.Data
{
    public partial class RcvInputData : ProcessData
    {
        public override void UpdatePrintPrintData(DynamicPrintLabelData DyData)
        {
            // DyData.CustomName = Vendor;
            DyData.RecipeCode = RecipeCode;
            DyData.DataTime = WorkDate + "" + WorkTime;

            BigRollBarcode barcode = new BigRollBarcode();
            //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
            //客户序号：单作为0，套作为序号 1 - 3
            //质量编码：共一位，0（未检验），1（合格），2以上为错误原因
            barcode.WorkNoStr = "0120"; // WorkNo;
            barcode.BatchNo = BatchNo;
            //barcode.ProcessStr = "Z";
            //barcode.MachineIDStr = "1";
            barcode.TimeStr = "2018-05-18 20:55"; // GetDateTimeForBarcode();
            barcode.BigRStr = string.Format("{0:D3}", ++GlobalConfig.Setting.recoveryBigrollIndex);
            //.ToString();

            // barcode.LittleRStr = "00";
            // barcode.VendorStr = (MType == ManufactureType.M_SINGLE) ? "0" : "1";
            // barcode.QAStr = "1";


            //DyData.QRBarCode =  "S17110906L302S118012014310500100";
            DyData.QRBarCode = barcode.createBarcode(); ;
            OutputBarcode = DyData.QRBarCode;
            DyData.WorkerNo = "0120 2018-05-18 20:55";
            DyData.LittleRollNoStr = "25.5";
            DyData.RecipeCode = "44-6";
            //System.Diagnostics.Debug.Assert(DyData.QRBarCode.Length == BigRollBarcode.getTotalStrLen());

            // DyData.RollWeightLength = StackWeight;
        }
    }
}
