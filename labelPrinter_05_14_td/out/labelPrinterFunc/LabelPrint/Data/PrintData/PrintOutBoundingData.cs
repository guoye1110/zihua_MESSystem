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
            NeedBags = "301";
            TargetMachineNo = "1号流延机";
            LiaoCangNo = "1号料仓";
            materialCode = "3.001.002";
            WorkNo = "0203 09:21";
            DyData.CustomName = Vendor;
            DyData.RecipeCode = materialCode;
            DyData.BatchNo = RawMaterialBatchNo;
            DyData.DataTime = Date_Time;
            DyData.WorkerNo = WorkNo;
            DyData.Width = TargetMachineNo;
            DyData.LittleRollNoStr = LiaoCangNo;
            DyData.QRBarCode = materialCode + ";" + TargetMachineNo.Remove(1) + ";" + LiaoCangNo.Remove(1) + ";" + NeedBags;
            DyData.RollWeightLength = NeedBags;

            //OutputBarcode = DyData.QRBarCode;
            //DyData.QRBarCode =原料编码；设备编码；料框编码;代数
            // DyData.RollWeightLength = StackWeight;
            OutputBarcode = DyData.QRBarCode;
        }
    }
}
