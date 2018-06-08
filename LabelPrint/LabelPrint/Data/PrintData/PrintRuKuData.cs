using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.Data
{
    public partial class RuKuInputData : OutBoundingInputData
    {
        public override void UpdatePrintPrintData(DynamicPrintLabelData DyData)
        {


            //NeedBags = "301";
            //   TargetMachineNo = "1号流延机";
            // LiaoCangNo = "1号料仓";
            // RawMaterialCode = "3.001.002";
            //WorkNo = "0203 09:21";
            DyData.CustomName = Vendor;
            // DyData.Recipe = Recipe;
            DyData.BatchNo = RawMaterialBatchNo;
            DyData.DataTime = Date_Time;
            DyData.WorkerNo = WorkerNo;
            DyData.Width = TargetMachineNo;
            DyData.LittleRollNoStr = LiaoCangNo;
            //  DyData.QRBarCode = RawMaterialCode + ";" + TargetMachineNo + ";" + LiaoCangNo + ";" + NeedBags;

            DyData.QRBarCode = RawMaterialCode + ";" + TargetMachineNo.Remove(1) + ";" + LiaoCangNo + ";" + BenCiChuKuWeight;

            DyData.RawMaterialCode = RawMaterialCode;
            DyData.RollWeightLength = BenCiChuKuWeight;

            //OutputBarcode = DyData.QRBarCode;
            //DyData.QRBarCode =原料编码；设备编码；料框编码;代数
            // DyData.RollWeightLength = StackWeight;
            OutputBarcode = DyData.QRBarCode;
        }

    }
}
