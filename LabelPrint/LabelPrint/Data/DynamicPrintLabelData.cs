using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Data
{
    public enum DynamicTextPrintType
    {
        WorkProcess,
        ProductCode,
        CustomName,
        WorkerNo,
        RawMaterialCode,
        MaterialName,
        BatchNo,
        Width,
        WorkNo,
        DataTime,
        RollWeightLength,
        BigRollNoStr,
        LittleRollNoStr,
        MachineNo,
        Quality,
        RawMaterialBagCount,
        JIZhong,
        SeXiPeiBi,
        RecipeCode,
        CheckResult,
        BarCode,
        QRBarCod,
            Amount,
            ProductLength,
			ProductName,
			ProductWeight,
            PlateNo,
			OrderNo,
    }

    public class DynamicPrintLabelData
    {
        public String WorkProcess;
        public String ProductCode;
        public String CustomName;
        public String WorkerNo;
        public String RawMaterialCode;
        public String MaterialName;
        public String BatchNo;
        public String Width;
        public String WorkNo;
        public String DataTime;
        public String RollWeightLength;
        public String BigRollNoStr;
        public String LittleRollNoStr;
        public String MachineNo;
        public String Quality;
        public String RawMaterialBagCount;
        public String JIZhong;
        public String SeXiPeiBi;
        //public String PeiFang;
        public String CheckResult;
        //public String BarCode;
        public String QRBarCode;
        public String RecipeCode;

        public String Amount;
        public String ProductLength;
		public String ProductName;
		public String ProductWeight;
		public String PlateNo;
		public String OrderNo;
        public static String[] PrintDatas =
        {
            "WorkProcess",
            "ProductCode",
            "CustomName",
            "WorkerNo",
            "RawMaterialCode",
            "MaterialName",
            "BatchNo",
            "Width",
            "WorkNo",
            "DataTime",
            "RollWeightLength",
            "BigRollNoStr",
            "LittleRollNoStr",
            "MachineNo",
            "Quality",
            "RawMaterialBagCount",
            "JIZhong",
            "SeXiPeiBi",
            "PeiFang",
            "CheckResult",
            "BarCode",
            "QRBarCod",
            "Amount",
            "ProductLength",
			"ProductName",
			"ProductWeight",
            "PlateNo",
			"OrderNo",
        };
        public static String[] sites =
        {

            "出库",
            "流延",
            "分切",
            "打印",
            "质检",
            "打包",
            "再造料",
            "入库",
        };
        public void setWorkProcess(int work)
        {

        }
        public void setWorkProcess(String str)
        {
            WorkProcess = str;
        }
    }
}
