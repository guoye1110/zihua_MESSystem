using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.Data
{
    public partial class OutBoundingInputData : ProcessData
    {
        public override void UpdatePrintPrintData(DynamicPrintLabelData DyData)
        {
            DyData.CustomName = Vendor;
            // DyData.Recipe = Recipe;
            DyData.BatchNo = RawMaterialBatchNo;
            DyData.DataTime = Date_Time;

            DyData.QRBarCode = RawMaterialCode + ";" + TargetMachineNo + ";" + LiaoCangNo + ";" + NeedBags;
            OutputBarcode = DyData.QRBarCode;
            //DyData.QRBarCode =原料编码；设备编码；料框编码;代数
            // DyData.RollWeightLength = StackWeight;
            OutputBarcode = DyData.QRBarCode;
        }
    }
}
