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

            //UserInput.RawMaterialCode = cb_RawMaterialCodes[LiaoCangNo - 1].Text;
            //UserInput.RawMaterialBatchNo = tb_RawMaterialBachNos[LiaoCangNo - 1].Text;
            //UserInput.XuQiuWeight = tb_XuQiuWeights[LiaoCangNo - 1].Text;
            //UserInput.YiChuKuWeight = tb_YiChuKuWeights[LiaoCangNo - 1].Text;
            //UserInput.BenCiChuKuWeight = tb_BenCiChuKuWeights[LiaoCangNo - 1].Text;

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
            DyData.QRBarCode = RawMaterialCode + ";" + TargetMachineNo + ";" + LiaoCangNo + ";" + NeedBags;

            
            DyData.RawMaterialCode = RawMaterialCode;
            DyData.RollWeightLength = BenCiChuKuWeight;

            //OutputBarcode = DyData.QRBarCode;
            //DyData.QRBarCode =原料编码；设备编码；料框编码;代数
            // DyData.RollWeightLength = StackWeight;
            OutputBarcode = DyData.QRBarCode;
        }
    }
}
