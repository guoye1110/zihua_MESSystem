using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabelPrint.Data
{
    public partial class CutUserinputData : ProcessData
    {
        override public void UpdatePrintPrintData(DynamicPrintLabelData DyData)
        {
            String str = null;
            base.UpdatePrintPrintData(DyData);
            DyData.BigRollNoStr = BigRollNo;
            DyData.LittleRollNoStr = BigRollNo + "-" + LittleRollNo;
            DyData.RollWeightLength = Weight;
            DyData.WorkerNo = WorkerNo + "  " + WorkTime.Substring(0, 5);


            if (ProductState == "合格品" || ProductState == null || ProductState == "")
            {
                DyData.Quality = "合格品";
            }
            else
            {
                DyData.Quality = ProductQuality;
            }
            DyData.RawMaterialCode = RawMaterialCode;
            // UserInput.customerCode = customerCode;
            // UserInput.productLength = productLength;
            //    UserInput.productName = productName;
            //  UserInput.productWeight = productWeight;
            DyData.RecipeCode = ProductName + " " + ProductWeight;
            str = Weight;
            if (Weight == null || Weight == "")
                str = "0";
            DyData.RollWeightLength = str + " kg " + ProductLength;
            LittleRollBarcode barcode = new LittleRollBarcode();
            //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
            //客户序号：单作为0，套作为序号 1 - 3
            //质量编码：共一位，0（未检验），1（合格），2以上为错误原因
            barcode.WorkNoStr = WorkNo;
            barcode.BatchNo = BatchNo;
            //   barcode.ProcessStr = "F";
            // barcode.MachineIDStr = CutMachineNo;
            barcode.TimeStr = GetDateTimeForBarcode();
            barcode.BigRStr = GetRealBigRollNo(BigRollNo);
            barcode.LittleRStr = GetRealLittleRollNo(LittleRollNo);
            barcode.VendorStr = (MType == ManufactureType.M_SINGLE) ? "0" : "1";

            if (ProductStateIndex == 0)
            {
                barcode.QAStr = "1";
            }
            else
            {
                int QAstr = 2 + ProductQualityIndex;
                barcode.QAStr = "" + QAstr;
            }

            //String barcode = WorkNoStr + ProcessStr + MachineIDStr + TimeStr + BigRStr + LittleRStr + VendorStr + QAStr;
            //WorkDate

            //DyData.QRBarCode =  "S17110906L302S118012014310500100";
            DyData.QRBarCode = barcode.createBarcode(); ;
            OutputBarcode = DyData.QRBarCode;
            // System.Diagnostics.Debug.Assert(DyData.QRBarCode.Length == LittleRollBarcode.getTotalStrLen());

        }


         public void UpdateCutPackPrintPrintData(DynamicPrintLabelData DyData)
        {
            String str = null;
            base.UpdatePrintPrintData(DyData);
            DyData.BigRollNoStr = BigRollNo;
            DyData.LittleRollNoStr = BigRollNo + "-" + LittleRollNo;
            DyData.RollWeightLength = Weight;
            DyData.WorkerNo = WorkerNo + "  " + WorkTime.Substring(0, 5);


            if (ProductState == "合格品" || ProductState == null || ProductState == "")
            {
                DyData.Quality = "合格品";
            }
            else
            {
                DyData.Quality = ProductQuality;
            }
            DyData.RawMaterialCode = RawMaterialCode;
            // UserInput.customerCode = customerCode;
            // UserInput.productLength = productLength;
            //    UserInput.productName = productName;
            //  UserInput.productWeight = productWeight;
            DyData.RecipeCode = ProductName + " " + ProductWeight;
            str = Weight;
            if (Weight == null || Weight == "")
                str = "0";
            DyData.RollWeightLength = str + " kg " + ProductLength;


            DyData.QRBarCode = BatchNo + PlateNo;
            OutputBarcode = DyData.QRBarCode;
            // System.Diagnostics.Debug.Assert(DyData.QRBarCode.Length == LittleRollBarcode.getTotalStrLen());

        }
    }
}
